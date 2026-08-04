using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnboundArcana.Core.Rooms.Editor
{
	public enum RoomSectionRenderPass
	{
		OtherWalls,
		SouthWalls
	}

	public enum RoomSectionStampRenderPass
	{
		Back,
		Sides,
		Front,
		Overlay
	}

	[Serializable]
	public sealed class RoomSectionTilePlacement
	{
		public Vector2Int Offset;
		public TileBase Tile;
		public RoomSectionStampRenderPass RenderPass;
	}

	[Serializable]
	public sealed class RoomSectionStraightWallStamp
	{
		public GeneratedWallDirection Direction;
		public List<RoomSectionTilePlacement> Placements = new();

		// Straight stamp anchor is the adjacent floor cell.
	}

	[Serializable]
	public sealed class RoomSectionCornerWallStamp
	{
		public GeneratedBoundaryCornerKind Kind;
		public GeneratedBoundaryCornerOrientation Orientation;
		public int IncomingRunConsumption = 1;
		public int OutgoingRunConsumption = 1;
		public List<RoomSectionTilePlacement> Placements = new();

		// Corner stamp anchor is the boundary corner lattice vertex.
	}

	[CreateAssetMenu(menuName = "Unbound Arcana/Rooms/Room Section Style Profile")]
	public sealed class RoomSectionStyleProfile : ScriptableObject
	{
		public Vector3 CellSize = new(0.3f, 0.3f, 0.3f);
		public bool RequireSquareCells = true;
		public int SouthWallSortingOrder = 1;
		public int OtherWallSortingOrder = 3;
		public TileBase FloorTile;
		public TileBase DebugFallbackTile;
		public int NorthWallLayerCount = 1;
		public int EastWallLayerCount = 1;
		public int SouthWallLayerCount = 1;
		public int WestWallLayerCount = 1;
		public List<RoomSectionWallVisualMapping> WallMappings = new();
		public List<RoomSectionStraightWallStamp> StraightWallStamps = new();
		public List<RoomSectionCornerWallStamp> CornerWallStamps = new();

		private static readonly GeneratedWallDirection[] RequiredDirections = { GeneratedWallDirection.North, GeneratedWallDirection.East, GeneratedWallDirection.South, GeneratedWallDirection.West };
		private static readonly GeneratedBoundaryCornerKind[] RequiredCornerKinds = { GeneratedBoundaryCornerKind.Outer, GeneratedBoundaryCornerKind.Inner };
		private static readonly GeneratedBoundaryCornerOrientation[] RequiredCornerOrientations = { GeneratedBoundaryCornerOrientation.NorthWest, GeneratedBoundaryCornerOrientation.NorthEast, GeneratedBoundaryCornerOrientation.SouthEast, GeneratedBoundaryCornerOrientation.SouthWest };

		public int GetLayerCount(GeneratedWallDirection direction)
		{
			return Mathf.Max(1, direction switch
			{
				GeneratedWallDirection.North => NorthWallLayerCount,
				GeneratedWallDirection.East => EastWallLayerCount,
				GeneratedWallDirection.South => SouthWallLayerCount,
				_ => WestWallLayerCount
			});
		}

		public bool TryGetTile(GeneratedBoundaryEdge edge, int layer, out TileBase tile)
		{
			return TryGetTile(edge, layer, out tile, out _);
		}

		public bool TryGetTile(GeneratedBoundaryEdge edge, int layer, out TileBase tile, out RoomSectionRenderPass renderPass)
		{
			RoomSectionRenderPass defaultPass = GetDefaultRenderPass(edge.Direction);
			RoomSectionWallVisualMapping mapping = WallMappings.Where(item => item != null && item.Direction == edge.Direction && item.Topology == edge.Topology && item.Handedness == edge.Handedness && item.Layer == layer).OrderByDescending(item => item.TargetPass == defaultPass).FirstOrDefault();
			tile = mapping == null ? null : mapping.Tile;
			renderPass = mapping == null ? GetDefaultRenderPass(edge.Direction) : mapping.TargetPass;
			return tile != null;
		}

		public static RoomSectionRenderPass GetDefaultRenderPass(GeneratedWallDirection direction)
		{
			return direction == GeneratedWallDirection.South ? RoomSectionRenderPass.SouthWalls : RoomSectionRenderPass.OtherWalls;
		}

		public bool TryGetStraightStamp(GeneratedWallDirection direction, out RoomSectionStraightWallStamp stamp)
		{
			stamp = null;
			if (StraightWallStamps == null) return false;
			int matches = 0;
			foreach (RoomSectionStraightWallStamp candidate in StraightWallStamps)
				if (candidate != null && candidate.Direction == direction)
				{
					stamp = candidate;
					matches++;
				}
			if (matches != 1) stamp = null;
			return matches == 1;
		}

		public bool TryGetCornerStamp(GeneratedBoundaryCornerKind kind, GeneratedBoundaryCornerOrientation orientation, out RoomSectionCornerWallStamp stamp)
		{
			stamp = null;
			if (CornerWallStamps == null) return false;
			int matches = 0;
			foreach (RoomSectionCornerWallStamp candidate in CornerWallStamps)
				if (candidate != null && candidate.Kind == kind && candidate.Orientation == orientation)
				{
					stamp = candidate;
					matches++;
				}
			if (matches != 1) stamp = null;
			return matches == 1;
		}

		public List<string> ValidateProfile()
		{
			List<string> diagnostics = new();
			if (CellSize.x <= 0f || CellSize.y <= 0f || CellSize.z <= 0f) diagnostics.Add("Grid cell size must be positive.");
			if (!Mathf.Approximately(CellSize.x, CellSize.y)) diagnostics.Add("Grid X and Y cell size must match.");
			if (!Mathf.Approximately(CellSize.x, 0.3f) || !Mathf.Approximately(CellSize.y, 0.3f)) diagnostics.Add("Grid cell size must match the RoomGenerator contract of 0.3 x 0.3.");
			if (FloorTile == null) diagnostics.Add("Floor tile is missing.");
			if (NorthWallLayerCount < 1 || EastWallLayerCount < 1 || SouthWallLayerCount < 1 || WestWallLayerCount < 1) diagnostics.Add("Every wall direction must have at least one layer.");
			HashSet<string> keys = new();
			foreach (RoomSectionWallVisualMapping mapping in WallMappings)
			{
				if (mapping == null) continue;
				string key = $"{mapping.Direction}:{mapping.Topology}:{mapping.Handedness}:{mapping.Layer}:{mapping.TargetPass}";
				if (!keys.Add(key)) diagnostics.Add($"Duplicate wall mapping: {key}.");
				if (mapping.Layer < 0) diagnostics.Add($"Wall mapping layer cannot be negative: {key}.");
				if (mapping.Tile == null) diagnostics.Add($"Wall mapping has no tile: {key}.");
			}
			ValidateStraightStamps(diagnostics);
			ValidateCornerStamps(diagnostics);
			return diagnostics;
		}

		private void ValidateStraightStamps(List<string> diagnostics)
		{
			foreach (GeneratedWallDirection direction in RequiredDirections)
			{
				string key = $"Straight {direction}";
				List<RoomSectionStraightWallStamp> matches = StraightWallStamps == null ? new List<RoomSectionStraightWallStamp>() : StraightWallStamps.Where(stamp => stamp != null && stamp.Direction == direction).ToList();
				if (matches.Count == 0) diagnostics.Add($"Missing {key} stamp.");
				if (matches.Count > 1) diagnostics.Add($"Duplicate {key} stamps.");
				foreach (RoomSectionStraightWallStamp stamp in matches) ValidatePlacements(diagnostics, key, stamp.Placements);
			}
			if (StraightWallStamps != null && StraightWallStamps.Any(stamp => stamp == null)) diagnostics.Add("Straight wall stamp collection contains a null stamp.");
		}

		private void ValidateCornerStamps(List<string> diagnostics)
		{
			foreach (GeneratedBoundaryCornerKind kind in RequiredCornerKinds)
				foreach (GeneratedBoundaryCornerOrientation orientation in RequiredCornerOrientations)
				{
					string key = $"{kind} {orientation}";
					List<RoomSectionCornerWallStamp> matches = CornerWallStamps == null ? new List<RoomSectionCornerWallStamp>() : CornerWallStamps.Where(stamp => stamp != null && stamp.Kind == kind && stamp.Orientation == orientation).ToList();
					if (matches.Count == 0) diagnostics.Add($"Missing {key} corner stamp.");
					if (matches.Count > 1) diagnostics.Add($"Duplicate {key} corner stamps.");
					foreach (RoomSectionCornerWallStamp stamp in matches)
					{
						if (stamp.IncomingRunConsumption < 0) diagnostics.Add($"{key} corner stamp has negative incoming consumption.");
						if (stamp.OutgoingRunConsumption < 0) diagnostics.Add($"{key} corner stamp has negative outgoing consumption.");
						ValidatePlacements(diagnostics, key, stamp.Placements);
					}
				}
			if (CornerWallStamps != null && CornerWallStamps.Any(stamp => stamp == null)) diagnostics.Add("Corner wall stamp collection contains a null stamp.");
		}

		private static void ValidatePlacements(List<string> diagnostics, string key, List<RoomSectionTilePlacement> placements)
		{
			if (placements == null || placements.Count == 0)
			{
				diagnostics.Add($"{key} stamp has no tile placements.");
				return;
			}
			HashSet<string> keys = new();
			foreach (RoomSectionTilePlacement placement in placements)
			{
				if (placement == null)
				{
					diagnostics.Add($"{key} stamp contains a null tile placement.");
					continue;
				}
				if (placement.Tile == null) diagnostics.Add($"{key} stamp has a null tile reference at offset {placement.Offset} on render pass {placement.RenderPass}.");
				string placementKey = $"{placement.Offset.x}:{placement.Offset.y}:{placement.RenderPass}";
				if (!keys.Add(placementKey)) diagnostics.Add($"{key} stamp has conflicting tile placements at offset {placement.Offset} on render pass {placement.RenderPass}.");
			}
		}
	}

	[Serializable]
	public sealed class RoomSectionWallVisualMapping
	{
		public GeneratedWallDirection Direction;
		public GeneratedBoundaryTopology Topology;
		public GeneratedBoundaryHandedness Handedness;
		public int Layer;
		public RoomSectionRenderPass TargetPass;
		public TileBase Tile;
	}

	public sealed class RoomSectionPreviewResult
	{
		public readonly GameObject Root;
		public readonly List<RoomSectionDiagnostic> Diagnostics = new();

		public IEnumerable<RoomSectionDiagnostic> Errors => Diagnostics.Where(diagnostic => diagnostic.Severity == RoomSectionDiagnosticSeverity.Error);
		public IEnumerable<RoomSectionDiagnostic> Warnings => Diagnostics.Where(diagnostic => diagnostic.Severity == RoomSectionDiagnosticSeverity.Warning);

		public RoomSectionPreviewResult(GameObject root)
		{
			Root = root;
		}
	}
}
