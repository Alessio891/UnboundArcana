using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnboundArcana.Core.Rooms.Editor
{
	public static class RoomSectionPreviewBuilder
	{
		private readonly struct PlannedPlacementKey : IEquatable<PlannedPlacementKey>
		{
			public readonly RoomSectionStampRenderPass RenderPass;
			public readonly Vector2Int Cell;

			public PlannedPlacementKey(RoomSectionStampRenderPass renderPass, Vector2Int cell)
			{
				RenderPass = renderPass;
				Cell = cell;
			}

			public bool Equals(PlannedPlacementKey other)
			{
				return RenderPass == other.RenderPass && Cell == other.Cell;
			}

			public override bool Equals(object obj)
			{
				return obj is PlannedPlacementKey other && Equals(other);
			}

			public override int GetHashCode()
			{
				return HashCode.Combine(RenderPass, Cell);
			}
		}

		private sealed class PlannedWallPlacement
		{
			public readonly RoomSectionStampRenderPass RenderPass;
			public readonly Vector2Int Cell;
			public readonly TileBase Tile;
			public readonly string Source;

			public PlannedWallPlacement(RoomSectionStampRenderPass renderPass, Vector2Int cell, TileBase tile, string source)
			{
				RenderPass = renderPass;
				Cell = cell;
				Tile = tile;
				Source = source;
			}
		}

		public static RoomSectionPreviewResult Build(RoomSectionShape shape, RoomSectionStyleProfile profile, string name)
		{
			GameObject root = new(name);
			Grid grid = root.AddComponent<Grid>();
			grid.cellSize = profile == null ? new Vector3(0.3f, 0.3f, 0.3f) : profile.CellSize;
			RoomSection section = root.AddComponent<RoomSection>();
			RoomSectionFootprint footprint = root.AddComponent<RoomSectionFootprint>();
			GameObject floorObject = CreateTilemap(root.transform, "Floor", "Background", 0, false, false);
			Tilemap floor = floorObject.GetComponent<Tilemap>();
			RoomSectionPreviewResult result = new(root);
			foreach (RoomSectionDiagnostic diagnostic in shape.Diagnostics) result.Diagnostics.Add(diagnostic);
			if (profile == null) result.Diagnostics.Add(new RoomSectionDiagnostic(RoomSectionDiagnosticSeverity.Warning, "No style profile assigned. The preview is geometry-only."));
			else foreach (string diagnostic in profile.ValidateProfile()) result.Diagnostics.Add(new RoomSectionDiagnostic(RoomSectionDiagnosticSeverity.Warning, diagnostic));
			if (profile != null && profile.FloorTile != null)
			{
				foreach (Vector2Int cell in shape.Cells) floor.SetTile(new Vector3Int(cell.x, cell.y, 0), profile.FloorTile);
				floor.RefreshAllTiles();
			}
			GameObject backObject = CreateTilemap(root.transform, "Walls_Back", "Interactives", 1, true, true);
			GameObject sidesObject = CreateTilemap(root.transform, "Walls_Sides", "Interactives", 1, true, true);
			GameObject frontObject = CreateTilemap(root.transform, "Walls_Front", "Interactives", 1, true, true);
			GameObject overlayObject = CreateTilemap(root.transform, "Walls_Overlay", "Interactives", 2, false, true);
			BuildWalls(shape, profile, backObject.GetComponent<Tilemap>(), sidesObject.GetComponent<Tilemap>(), frontObject.GetComponent<Tilemap>(), overlayObject.GetComponent<Tilemap>(), result);
			SerializedObject serialized = new(section);
			serialized.FindProperty("sectionId").stringValue = name;
			serialized.FindProperty("grid").objectReferenceValue = grid;
			serialized.FindProperty("footprint").objectReferenceValue = footprint;
			serialized.FindProperty("connectors").arraySize = 0;
			serialized.FindProperty("props").arraySize = 0;
			serialized.ApplyModifiedPropertiesWithoutUndo();
			SetFootprint(footprint, shape.Cells);
			RoomSectionGeneratorSceneOverlay.Set(root, shape, result.Diagnostics);
			return result;
		}

		private static void BuildWalls(RoomSectionShape shape, RoomSectionStyleProfile profile, Tilemap back, Tilemap sides, Tilemap front, Tilemap overlay, RoomSectionPreviewResult result)
		{
			List<PlannedWallPlacement> placements = BuildPlacementPlan(shape, profile, result);
			foreach (PlannedWallPlacement placement in placements)
			{
				Tilemap target = GetTilemap(placement.RenderPass, back, sides, front, overlay);
				target.SetTile(new Vector3Int(placement.Cell.x, placement.Cell.y, 0), placement.Tile);
			}
			back.RefreshAllTiles();
			sides.RefreshAllTiles();
			front.RefreshAllTiles();
			overlay.RefreshAllTiles();
		}

		private static List<PlannedWallPlacement> BuildPlacementPlan(RoomSectionShape shape, RoomSectionStyleProfile profile, RoomSectionPreviewResult result)
		{
			List<PlannedWallPlacement> placements = new();
			Dictionary<PlannedPlacementKey, PlannedWallPlacement> occupied = new();
			if (shape.BoundaryLoops == null || shape.BoundaryLoops.Count == 0)
			{
				AddError(result, "Room section has no BoundaryLoops for stamp-based wall rendering.");
				return placements;
			}
			for (int loopIndex = 0; loopIndex < shape.BoundaryLoops.Count; loopIndex++) BuildLoopPlacementPlan(shape.BoundaryLoops[loopIndex], loopIndex, profile, placements, occupied, result);
			return placements;
		}

		private static void BuildLoopPlacementPlan(GeneratedBoundaryLoop loop, int loopIndex, RoomSectionStyleProfile profile, List<PlannedWallPlacement> placements, Dictionary<PlannedPlacementKey, PlannedWallPlacement> occupied, RoomSectionPreviewResult result)
		{
			if (loop == null)
			{
				AddError(result, $"Boundary loop {loopIndex} is null.");
				return;
			}
			if (!ValidateLoop(loop, loopIndex, result)) return;
			Dictionary<GeneratedBoundaryCorner, RoomSectionCornerWallStamp> cornerStamps = new();
			for (int cornerIndex = 0; cornerIndex < loop.Corners.Count; cornerIndex++)
			{
				GeneratedBoundaryCorner corner = loop.Corners[cornerIndex];
				if (corner == null)
				{
					AddError(result, $"Boundary loop {loopIndex} corner {cornerIndex} is null.");
					continue;
				}
				if (profile == null || !profile.TryGetCornerStamp(corner.Kind, corner.Orientation, out RoomSectionCornerWallStamp stamp))
				{
					AddError(result, $"No unique corner stamp lookup for {corner.Kind} {corner.Orientation}.", corner.Position);
					continue;
				}
				cornerStamps[corner] = stamp;
				PlanStampPlacements(stamp.Placements, corner.Position, $"{corner.Kind} {corner.Orientation} corner", placements, occupied, result);
			}
			for (int runIndex = 0; runIndex < loop.WallRuns.Count; runIndex++)
			{
				GeneratedWallRun run = loop.WallRuns[runIndex];
				if (run == null) continue;
				GeneratedBoundaryCorner startCorner = FindRunCorner(loop, runIndex, false, result);
				GeneratedBoundaryCorner endCorner = FindRunCorner(loop, runIndex, true, result);
				if (startCorner == null || endCorner == null) continue;
				if (!cornerStamps.TryGetValue(startCorner, out RoomSectionCornerWallStamp startStamp) || !cornerStamps.TryGetValue(endCorner, out RoomSectionCornerWallStamp endStamp))
				{
					AddError(result, $"Run {run.Direction} from {run.StartVertex} to {run.EndVertex} has a missing corner stamp association.", run.StartVertex, run.Direction);
					continue;
				}
				if (startStamp.OutgoingRunConsumption < 0 || endStamp.IncomingRunConsumption < 0)
				{
					AddError(result, $"Run {run.Direction} from {run.StartVertex} to {run.EndVertex} has negative corner consumption.", run.StartVertex, run.Direction);
					continue;
				}
				int usableLength = run.Length - startStamp.OutgoingRunConsumption - endStamp.IncomingRunConsumption;
				if (usableLength < 0)
				{
					AddError(result, $"Run {run.Direction} from {run.StartVertex} to {run.EndVertex} has negative usable length {usableLength}.", run.StartVertex, run.Direction);
					continue;
				}
				if (profile == null || !profile.TryGetStraightStamp(run.Direction, out RoomSectionStraightWallStamp straightStamp))
				{
					AddError(result, $"No unique straight stamp lookup for {run.Direction}.", run.StartVertex, run.Direction);
					continue;
				}
				for (int unitIndex = startStamp.OutgoingRunConsumption; unitIndex < run.Length - endStamp.IncomingRunConsumption; unitIndex++)
				{
					Vector2Int anchor = GetStraightAnchor(run, unitIndex);
					PlanStampPlacements(straightStamp.Placements, anchor, $"Straight {run.Direction} run {run.StartVertex} to {run.EndVertex} unit {unitIndex}", placements, occupied, result);
				}
			}
		}

		private static bool ValidateLoop(GeneratedBoundaryLoop loop, int loopIndex, RoomSectionPreviewResult result)
		{
			bool valid = true;
			if (loop.Vertices == null || loop.WallRuns == null || loop.Corners == null)
			{
				AddError(result, $"Boundary loop {loopIndex} has missing vertices, runs, or corners.");
				return false;
			}
			if (!loop.IsClosed)
			{
				AddError(result, $"Boundary loop {loopIndex} is not closed.");
				valid = false;
			}
			if (loop.Vertices.Count != loop.WallRuns.Count + 1)
			{
				AddError(result, $"Boundary loop {loopIndex} does not expose one vertex per wall-run junction.");
				valid = false;
			}
			if (loop.Corners.Count != loop.WallRuns.Count)
			{
				AddError(result, $"Boundary loop {loopIndex} has {loop.Corners.Count} corners for {loop.WallRuns.Count} wall runs.");
				valid = false;
			}
			for (int runIndex = 0; runIndex < loop.WallRuns.Count; runIndex++)
			{
				GeneratedWallRun run = loop.WallRuns[runIndex];
				if (run == null)
				{
					AddError(result, $"Boundary loop {loopIndex} wall run {runIndex} is null.");
					valid = false;
					continue;
				}
				if (run.Length <= 0)
				{
					AddError(result, $"Boundary loop {loopIndex} wall run {runIndex} has non-positive length.", run.StartVertex, run.Direction);
					valid = false;
				}
				if (runIndex + 1 < loop.Vertices.Count && (loop.Vertices[runIndex] != run.StartVertex || loop.Vertices[runIndex + 1] != run.EndVertex))
				{
					AddError(result, $"Boundary loop {loopIndex} wall run {runIndex} does not match its ordered vertices.", run.StartVertex, run.Direction);
					valid = false;
				}
			}
			return valid;
		}

		private static GeneratedBoundaryCorner FindRunCorner(GeneratedBoundaryLoop loop, int runIndex, bool incoming, RoomSectionPreviewResult result)
		{
			GeneratedWallRun run = loop.WallRuns[runIndex];
			int adjacentIndex = incoming ? (runIndex + 1) % loop.WallRuns.Count : (runIndex + loop.WallRuns.Count - 1) % loop.WallRuns.Count;
			GeneratedWallRun adjacent = loop.WallRuns[adjacentIndex];
			Vector2Int position = incoming ? run.EndVertex : run.StartVertex;
			GeneratedWallDirection incomingSide = incoming ? run.Direction : adjacent.Direction;
			GeneratedWallDirection outgoingSide = incoming ? adjacent.Direction : run.Direction;
			List<GeneratedBoundaryCorner> matches = loop.Corners.Where(corner => corner != null && corner.Position == position && corner.IncomingWallSide == incomingSide && corner.OutgoingWallSide == outgoingSide).ToList();
			if (matches.Count == 0) AddError(result, $"Run {run.Direction} at {position} has no {(incoming ? "incoming" : "outgoing")} corner association.", position, run.Direction);
			if (matches.Count > 1) AddError(result, $"Run {run.Direction} at {position} has duplicate {(incoming ? "incoming" : "outgoing")} corner associations.", position, run.Direction);
			return matches.Count == 0 ? null : matches[0];
		}

		private static void PlanStampPlacements(List<RoomSectionTilePlacement> stampPlacements, Vector2Int anchor, string stampSource, List<PlannedWallPlacement> placements, Dictionary<PlannedPlacementKey, PlannedWallPlacement> occupied, RoomSectionPreviewResult result)
		{
			if (stampPlacements == null || stampPlacements.Count == 0)
			{
				AddError(result, $"{stampSource} has no tile placements.", anchor);
				return;
			}
			for (int placementIndex = 0; placementIndex < stampPlacements.Count; placementIndex++)
			{
				RoomSectionTilePlacement placement = stampPlacements[placementIndex];
				string source = $"{stampSource} placement {placementIndex}";
				if (placement == null)
				{
					AddError(result, $"{source} is null.", anchor);
					continue;
				}
				if (placement.Tile == null)
				{
					AddError(result, $"{source} has a null tile.", anchor);
					continue;
				}
				AddPlannedPlacement(new Vector2Int(anchor.x + placement.Offset.x, anchor.y + placement.Offset.y), placement.RenderPass, placement.Tile, source, placements, occupied, result);
			}
		}

		private static void AddPlannedPlacement(Vector2Int cell, RoomSectionStampRenderPass renderPass, TileBase tile, string source, List<PlannedWallPlacement> placements, Dictionary<PlannedPlacementKey, PlannedWallPlacement> occupied, RoomSectionPreviewResult result)
		{
			PlannedPlacementKey key = new(renderPass, cell);
			if (occupied.TryGetValue(key, out PlannedWallPlacement existing))
			{
				AddError(result, $"Placement conflict on {renderPass} cell {cell}: first {existing.Source}; conflicting {source}.", cell);
				return;
			}
			PlannedWallPlacement planned = new(renderPass, cell, tile, source);
			occupied.Add(key, planned);
			placements.Add(planned);
		}

		private static Vector2Int GetStraightAnchor(GeneratedWallRun run, int unitIndex)
		{
			return run.Direction switch
			{
				GeneratedWallDirection.North => run.StartVertex + new Vector2Int(unitIndex, -1),
				GeneratedWallDirection.East => run.StartVertex + new Vector2Int(-1, -unitIndex - 1),
				GeneratedWallDirection.South => run.StartVertex + new Vector2Int(-unitIndex - 1, 0),
				_ => run.StartVertex + new Vector2Int(0, unitIndex)
			};
		}

		private static Tilemap GetTilemap(RoomSectionStampRenderPass renderPass, Tilemap back, Tilemap sides, Tilemap front, Tilemap overlay)
		{
			return renderPass switch
			{
				RoomSectionStampRenderPass.Back => back,
				RoomSectionStampRenderPass.Sides => sides,
				RoomSectionStampRenderPass.Front => front,
				_ => overlay
			};
		}

		private static void AddError(RoomSectionPreviewResult result, string message, Vector2Int? cell = null, GeneratedWallDirection? direction = null)
		{
			result.Diagnostics.Add(new RoomSectionDiagnostic(RoomSectionDiagnosticSeverity.Error, message, cell, direction));
		}

		private static GameObject CreateTilemap(Transform parent, string name, string sortingLayerName, int order, bool collider, bool individual)
		{
			GameObject obj = new(name);
			obj.transform.SetParent(parent);
			obj.AddComponent<Tilemap>();
			TilemapRenderer renderer = obj.AddComponent<TilemapRenderer>();
			renderer.sortingLayerName = sortingLayerName;
			renderer.sortingOrder = order;
			if (individual) renderer.mode = TilemapRenderer.Mode.Individual;
			if (collider) obj.AddComponent<TilemapCollider2D>();
			return obj;
		}

		private static void SetFootprint(RoomSectionFootprint footprint, HashSet<Vector2Int> cells)
		{
			SerializedObject serialized = new(footprint);
			SerializedProperty rectangles = serialized.FindProperty("rectangles");
			List<FootprintRectangle> rows = new();
			foreach (int y in cells.Select(cell => cell.y).Distinct().OrderBy(value => value))
			{
				List<int> xs = cells.Where(cell => cell.y == y).Select(cell => cell.x).OrderBy(value => value).ToList();
				int start = xs[0];
				int previous = start;
				for (int i = 1; i <= xs.Count; i++)
				{
					if (i < xs.Count && xs[i] == previous + 1) { previous = xs[i]; continue; }
					rows.Add(new FootprintRectangle { Position = new Vector2Int(start, y), Size = new Vector2Int(previous - start + 1, 1) });
					if (i < xs.Count) start = previous = xs[i];
				}
			}
			rectangles.arraySize = rows.Count;
			for (int i = 0; i < rows.Count; i++)
			{
				SerializedProperty rectangle = rectangles.GetArrayElementAtIndex(i);
				rectangle.FindPropertyRelative("Position").vector2IntValue = rows[i].Position;
				rectangle.FindPropertyRelative("Size").vector2IntValue = rows[i].Size;
			}
			serialized.ApplyModifiedPropertiesWithoutUndo();
		}
	}

	[InitializeOnLoad]
	internal static class RoomSectionGeneratorSceneOverlay
	{
		private static GameObject activeRoot;
		private static RoomSectionShape activeShape;
		private static List<RoomSectionDiagnostic> activeDiagnostics;

		static RoomSectionGeneratorSceneOverlay()
		{
			SceneView.duringSceneGui += Draw;
		}

		public static void Set(GameObject root, RoomSectionShape shape, IEnumerable<RoomSectionDiagnostic> diagnostics)
		{
			activeRoot = root;
			activeShape = shape;
			activeDiagnostics = diagnostics.ToList();
			SceneView.RepaintAll();
		}

		public static void Clear(GameObject root)
		{
			if (activeRoot != root) return;
			activeRoot = null;
			activeShape = null;
			activeDiagnostics = null;
			SceneView.RepaintAll();
		}

		private static void Draw(SceneView sceneView)
		{
			if (activeRoot == null || activeShape == null || Selection.activeGameObject != activeRoot) return;
			Grid grid = activeRoot.GetComponent<Grid>();
			if (grid == null) return;
			Handles.color = new Color(1f, 0.75f, 0.1f, 0.8f);
			foreach (RoomSectionDiagnostic diagnostic in activeDiagnostics ?? new List<RoomSectionDiagnostic>())
			{
				if (!diagnostic.Cell.HasValue) continue;
				Vector3 center = grid.GetCellCenterWorld(new Vector3Int(diagnostic.Cell.Value.x, diagnostic.Cell.Value.y, 0));
				Handles.DrawWireCube(center, grid.cellSize * 0.9f);
			}
		}
	}
}
