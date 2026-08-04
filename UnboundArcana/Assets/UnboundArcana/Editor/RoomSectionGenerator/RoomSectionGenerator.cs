using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnboundArcana.Core.Rooms.Editor
{
	public enum GeneratedWallDirection
	{
		North,
		East,
		South,
		West
	}

	public enum GeneratedWallCorner
	{
		None,
		NorthWest,
		NorthEast,
		SouthWest,
		SouthEast,
		InnerNorthWest,
		InnerNorthEast,
		InnerSouthWest,
		InnerSouthEast,
		SouthWestCap,
		SouthEastCap
	}

	public sealed class GeneratedWall
	{
		public readonly Vector2Int Cell;
		public readonly GeneratedWallDirection Direction;
		public readonly int Layer;
		public readonly GeneratedBoundaryTopology Topology;
		public readonly GeneratedBoundaryHandedness Handedness;
		public readonly bool HasMultipleFeatures;
		public GeneratedWallCorner Corner;

		public GeneratedWall(Vector2Int cell, GeneratedWallDirection direction, int layer, GeneratedWallCorner corner = GeneratedWallCorner.None, GeneratedBoundaryTopology topology = GeneratedBoundaryTopology.Straight, GeneratedBoundaryHandedness handedness = GeneratedBoundaryHandedness.None, bool hasMultipleFeatures = false)
		{
			Cell = cell;
			Direction = direction;
			Layer = layer;
			Topology = topology;
			Handedness = handedness;
			HasMultipleFeatures = hasMultipleFeatures;
			Corner = corner;
		}
	}

	public sealed class GeneratedRoomSection
	{
		public readonly HashSet<Vector2Int> Cells = new();
		public readonly List<GeneratedWall> Walls = new();
		public readonly List<Vector2Int> Props = new();
		public readonly List<string> Errors = new();
		public readonly List<string> Warnings = new();
	}

	public static class RoomSectionGenerationLogic
	{
		public static GeneratedRoomSection Generate(int seed, int width, int height, int alcoves, bool symmetric, RoomSectionGeneratorSettings settings)
		{
			GeneratedRoomSection result = new();
			System.Random random = new(seed);
			width = Mathf.Max(3, width);
			height = Mathf.Max(3, height);

			for (int x = 0; x < width; x++)
				for (int y = 0; y < height; y++)
					result.Cells.Add(new Vector2Int(x, y));

			for (int i = 0; i < Mathf.Max(0, alcoves); i++)
			{
				int side = random.Next(4);
				int size = random.Next(2, 4);
				int offset = random.Next(1, Mathf.Max(2, height - size));
				if (side == 0) AddRectangle(result.Cells, new Vector2Int(random.Next(1, Mathf.Max(2, width - 2)), height), new Vector2Int(size, 2));
				if (side == 1) AddRectangle(result.Cells, new Vector2Int(width, Mathf.Clamp(offset, 1, height - 2)), new Vector2Int(2, size));
				if (side == 2) AddRectangle(result.Cells, new Vector2Int(random.Next(1, Mathf.Max(2, width - 2)), -2), new Vector2Int(size, 2));
				if (side == 3) AddRectangle(result.Cells, new Vector2Int(-2, Mathf.Clamp(offset, 1, height - 2)), new Vector2Int(2, size));
			}

			foreach (Vector2Int cell in result.Cells)
			{
				AddBoundary(result, cell, new Vector2Int(0, 1), GeneratedWallDirection.North, settings == null ? 0 : settings.NorthWallLayers.Count);
				AddBoundary(result, cell, new Vector2Int(1, 0), GeneratedWallDirection.East, 1);
				AddBoundary(result, cell, new Vector2Int(0, -1), GeneratedWallDirection.South, 1);
				AddBoundary(result, cell, new Vector2Int(-1, 0), GeneratedWallDirection.West, 1);
			}
			ClassifyBoundaryVertices(result);

			PlaceProps(result, random, settings);
			Validate(result);
			return result;
		}

		private static void AddRectangle(HashSet<Vector2Int> cells, Vector2Int origin, Vector2Int size)
		{
			for (int x = 0; x < size.x; x++)
				for (int y = 0; y < size.y; y++)
					cells.Add(origin + new Vector2Int(x, y));
		}

		private static void AddBoundary(GeneratedRoomSection result, Vector2Int cell, Vector2Int offset, GeneratedWallDirection direction, int layers)
		{
			if (result.Cells.Contains(cell + offset)) return;
			for (int layer = 0; layer < layers; layer++) result.Walls.Add(new GeneratedWall(cell, direction, layer));
		}

		private static void ClassifyBoundaryVertices(GeneratedRoomSection result)
		{
			HashSet<(Vector2Int Cell, GeneratedWallDirection Direction)> edges = result.Walls.Select(wall => (wall.Cell, wall.Direction)).ToHashSet();
			Dictionary<Vector2Int, List<(Vector2Int Cell, GeneratedWallDirection Direction)>> vertexEdges = new();
			foreach ((Vector2Int cell, GeneratedWallDirection direction) edge in edges)
				foreach (Vector2Int vertex in GetVertices(edge.cell, edge.direction))
				{
					if (!vertexEdges.TryGetValue(vertex, out List<(Vector2Int Cell, GeneratedWallDirection Direction)> connectedEdges)) vertexEdges.Add(vertex, connectedEdges = new List<(Vector2Int Cell, GeneratedWallDirection Direction)>());
					connectedEdges.Add(edge);
				}
			Dictionary<Vector2Int, GeneratedWallCorner> vertexCorners = new();
			foreach (KeyValuePair<Vector2Int, List<(Vector2Int Cell, GeneratedWallDirection Direction)>> vertex in vertexEdges)
			{
				List<GeneratedWallDirection> directions = vertex.Value.Select(edge => edge.Direction).Distinct().ToList();
				if (directions.Count != 2) continue;
				GeneratedWallCorner corner = GetCorner(vertex.Key, directions[0], directions[1], result.Cells);
				if (corner != GeneratedWallCorner.None) vertexCorners[vertex.Key] = corner;
			}
			foreach (GeneratedWall wall in result.Walls)
			{
				if (wall.Direction != GeneratedWallDirection.North && wall.Direction != GeneratedWallDirection.South)
				{
					wall.Corner = GeneratedWallCorner.None;
					continue;
				}
				Vector2Int[] vertices = GetVertices(wall.Cell, wall.Direction);
				Vector2Int leftVertex = wall.Direction == GeneratedWallDirection.North ? vertices[0] : vertices[1];
				Vector2Int rightVertex = wall.Direction == GeneratedWallDirection.North ? vertices[1] : vertices[0];
				if (vertexCorners.TryGetValue(leftVertex, out GeneratedWallCorner leftCorner)) wall.Corner = leftCorner;
				else if (vertexCorners.TryGetValue(rightVertex, out GeneratedWallCorner rightCorner)) wall.Corner = rightCorner;
				else wall.Corner = GeneratedWallCorner.None;
			}
		}

		private static Vector2Int[] GetVertices(Vector2Int cell, GeneratedWallDirection direction)
		{
			return direction switch
			{
				GeneratedWallDirection.North => new[] { cell + Vector2Int.up, cell + Vector2Int.up + Vector2Int.right },
				GeneratedWallDirection.East => new[] { cell + Vector2Int.up + Vector2Int.right, cell + Vector2Int.right },
				GeneratedWallDirection.South => new[] { cell + Vector2Int.right, cell },
				_ => new[] { cell, cell + Vector2Int.up }
			};
		}

		private static GeneratedWallCorner GetCorner(Vector2Int vertex, GeneratedWallDirection first, GeneratedWallDirection second, HashSet<Vector2Int> cells)
		{
			if (!IsCornerPair(first, second)) return GeneratedWallCorner.None;
			int occupied = 0;
			foreach (Vector2Int cell in new[] { vertex + new Vector2Int(-1, -1), vertex + Vector2Int.down, vertex + Vector2Int.left, vertex }) if (cells.Contains(cell)) occupied++;
			bool inner = occupied >= 3;
			if ((first == GeneratedWallDirection.North && second == GeneratedWallDirection.West) || (first == GeneratedWallDirection.West && second == GeneratedWallDirection.North)) return inner ? GeneratedWallCorner.InnerNorthWest : GeneratedWallCorner.NorthWest;
			if ((first == GeneratedWallDirection.North && second == GeneratedWallDirection.East) || (first == GeneratedWallDirection.East && second == GeneratedWallDirection.North)) return inner ? GeneratedWallCorner.InnerNorthEast : GeneratedWallCorner.NorthEast;
			if ((first == GeneratedWallDirection.South && second == GeneratedWallDirection.West) || (first == GeneratedWallDirection.West && second == GeneratedWallDirection.South)) return inner ? GeneratedWallCorner.SouthWestCap : GeneratedWallCorner.SouthWest;
			if ((first == GeneratedWallDirection.South && second == GeneratedWallDirection.East) || (first == GeneratedWallDirection.East && second == GeneratedWallDirection.South)) return inner ? GeneratedWallCorner.SouthEastCap : GeneratedWallCorner.SouthEast;
			return GeneratedWallCorner.None;
		}

		private static bool IsCornerPair(GeneratedWallDirection first, GeneratedWallDirection second)
		{
			return first != second && ((int)first + (int)second) % 2 == 1;
		}

		private static void PlaceProps(GeneratedRoomSection result, System.Random random, RoomSectionGeneratorSettings settings)
		{
			if (settings == null || settings.Props == null || settings.Props.Count == 0) return;
			List<Vector2Int> candidates = result.Cells.Where(c => c.x > 0 && c.y > 0 && c.x < result.Cells.Max(v => v.x) && c.y < result.Cells.Max(v => v.y)).OrderBy(_ => random.Next()).ToList();
			int count = Mathf.Clamp(candidates.Count / 12, 0, 8);
			for (int i = 0; i < count; i++) result.Props.Add(candidates[i]);
		}

		private static void Validate(GeneratedRoomSection result)
		{
			if (result.Cells.Count == 0) result.Errors.Add("Generated shape is empty.");
			if (result.Walls.Count == 0) result.Errors.Add("Generated shape has no wall boundary.");
			HashSet<Vector2Int> visited = new();
			Queue<Vector2Int> queue = new();
			Vector2Int start = result.Cells.FirstOrDefault();
			queue.Enqueue(start);
			while (queue.Count > 0)
			{
				Vector2Int cell = queue.Dequeue();
				if (!visited.Add(cell)) continue;
				foreach (Vector2Int offset in new[] { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left }) if (result.Cells.Contains(cell + offset)) queue.Enqueue(cell + offset);
			}
			if (visited.Count != result.Cells.Count) result.Errors.Add("Generated shape is disconnected.");
		}
	}

	public class RoomSectionGeneratorWindow : EditorWindow
	{
		private RoomSectionStyleProfile styleProfile;
		private RoomSectionGeneratorSettings legacySettings;
		private int seed = 12345;
		private int width = 10;
		private int height = 8;
		private int extensionWidth = 2;
		private int extensionHeight = 2;
		private int extensions = 2;
		private RoomSectionShapePreset preset = RoomSectionShapePreset.Random;
		private bool symmetric;
		private int batchCount = 3;
		private RoomSectionShape shape;
		private RoomSectionPreviewResult preview;
		private GameObject previewObject;
		private Vector2 scroll;

		[MenuItem("Unbound Arcana/Rooms/Room Section Generator")]
		public static void Open() => GetWindow<RoomSectionGeneratorWindow>("Room Section Generator");

		private void OnGUI()
		{
			scroll = EditorGUILayout.BeginScrollView(scroll);
			styleProfile = (RoomSectionStyleProfile)EditorGUILayout.ObjectField("Style Profile", styleProfile, typeof(RoomSectionStyleProfile), false);
			legacySettings = (RoomSectionGeneratorSettings)EditorGUILayout.ObjectField("Legacy Settings", legacySettings, typeof(RoomSectionGeneratorSettings), false);
			if (legacySettings != null && GUILayout.Button("Create Style Profile From Legacy Settings")) CreateProfileFromLegacy();
			EditorGUILayout.Space(4);
			preset = (RoomSectionShapePreset)EditorGUILayout.EnumPopup("Shape Preset", preset);
			seed = EditorGUILayout.IntField("Seed", seed);
			width = EditorGUILayout.IntSlider("Width", width, 3, 40);
			height = EditorGUILayout.IntSlider("Height", height, 3, 40);
			extensionWidth = EditorGUILayout.IntSlider("Extension Width", extensionWidth, 1, 8);
			extensionHeight = EditorGUILayout.IntSlider("Extension Height", extensionHeight, 1, 8);
			extensions = EditorGUILayout.IntSlider("Extensions", extensions, 0, 8);
			symmetric = EditorGUILayout.Toggle("Symmetric", symmetric);
			EditorGUILayout.LabelField("Grid Cell Size", GetCellSize().ToString());
			EditorGUILayout.LabelField("North Wall Layers", styleProfile == null ? "Geometry-only" : styleProfile.NorthWallLayerCount.ToString());
			EditorGUILayout.Space(4);
			if (GUILayout.Button("Generate Preview")) GeneratePreview();
			if (GUILayout.Button("Reroll")) { seed++; GeneratePreview(); }
			if (preview != null)
			{
				List<string> errors = preview.Errors.Select(diagnostic => diagnostic.ToString()).ToList();
				List<string> warnings = preview.Warnings.Select(diagnostic => diagnostic.ToString()).Distinct().ToList();
				string message = errors.Count == 0 ? $"Valid shape: {shape.Cells.Count} cells, {shape.Boundaries.Count} boundary segments" : string.Join("\n", errors);
				if (warnings.Count > 0) message += $"\nWarnings: {warnings.Count}\n" + string.Join("\n", warnings.Take(8));
				EditorGUILayout.HelpBox(message, errors.Count == 0 ? warnings.Count == 0 ? MessageType.Info : MessageType.Warning : MessageType.Error);
				if (GUILayout.Button("Validate")) ValidatePreview();
				if (GUILayout.Button("Save Preview Prefab")) SavePrefab(seed);
				batchCount = EditorGUILayout.IntSlider("Batch Count", batchCount, 1, 50);
				if (GUILayout.Button("Save Batch")) for (int i = 0; i < batchCount; i++) { seed++; GeneratePreview(); SavePrefab(seed); }
			}
			EditorGUILayout.EndScrollView();
		}

		private void GeneratePreview()
		{
			if (previewObject != null)
			{
				RoomSectionGeneratorSceneOverlay.Clear(previewObject);
				DestroyImmediate(previewObject);
			}
			RoomSectionGenerationRequest request = new()
			{
				Seed = seed,
				Preset = preset,
				Size = new Vector2Int(width, height),
				ExtensionSize = new Vector2Int(extensionWidth, extensionHeight),
				Extensions = extensions,
				Symmetric = symmetric
			};
			shape = RoomSectionShapeGenerationLogic.Generate(request);
			preview = RoomSectionPreviewBuilder.Build(shape, styleProfile, $"RoomSectionPreview_{seed}");
			previewObject = preview.Root;
			Selection.activeGameObject = previewObject;
			SceneView.lastActiveSceneView?.FrameSelected();
		}

		private void ValidatePreview()
		{
			if (preview == null) GeneratePreview();
			Repaint();
		}

		private Vector3 GetCellSize()
		{
			return styleProfile == null ? new Vector3(0.3f, 0.3f, 0.3f) : styleProfile.CellSize;
		}

		private void SavePrefab(int value)
		{
			if (preview == null) GeneratePreview();
			if (preview.Errors.Any()) { Debug.LogError(string.Join("\n", preview.Errors.Select(diagnostic => diagnostic.ToString()))); return; }
			string folder = "Assets/RoomSections/Generated";
			EnsureFolder("Assets/RoomSections");
			EnsureFolder(folder);
			string path = AssetDatabase.GenerateUniqueAssetPath($"{folder}/GeneratedRoom_{value}.prefab");
			PrefabUtility.SaveAsPrefabAssetAndConnect(previewObject, path, InteractionMode.UserAction);
			AssetDatabase.SaveAssets();
			Debug.Log($"Created RoomSection prefab: {path}");
		}

		private void CreateProfileFromLegacy()
		{
			styleProfile = RoomSectionStyleProfileMigration.CreateFromLegacy(legacySettings);
			string folder = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(legacySettings)).Replace("\\", "/");
			if (string.IsNullOrEmpty(folder)) folder = "Assets/RoomSections";
			if (!AssetDatabase.IsValidFolder(folder)) AssetDatabase.CreateFolder("Assets", "RoomSections");
			string path = AssetDatabase.GenerateUniqueAssetPath($"{folder}/{legacySettings.name}_StyleProfile.asset");
			AssetDatabase.CreateAsset(styleProfile, path);
			AssetDatabase.SaveAssets();
			Selection.activeObject = styleProfile;
			Debug.Log($"Created room section style profile: {path}");
		}

		private void OnDisable()
		{
			if (previewObject != null) RoomSectionGeneratorSceneOverlay.Clear(previewObject);
		}

		private static void EnsureFolder(string folder)
		{
			if (AssetDatabase.IsValidFolder(folder)) return;
			string parent = System.IO.Path.GetDirectoryName(folder).Replace("\\", "/");
			string name = System.IO.Path.GetFileName(folder);
			EnsureFolder(parent);
			AssetDatabase.CreateFolder(parent, name);
		}
}

	public static class RoomSectionGeneratorBuilder
	{
		public static GameObject Build(GeneratedRoomSection data, RoomSectionGeneratorSettings settings, string name)
		{
			GameObject root = new(name);
			Grid grid = root.AddComponent<Grid>();
			grid.cellSize = settings == null ? new Vector3(0.3f, 0.3f, 0.3f) : settings.CellSize;
			RoomSection section = root.AddComponent<RoomSection>();
			RoomSectionFootprint footprint = root.AddComponent<RoomSectionFootprint>();
			GameObject floorObject = CreateTilemap(root.transform, "Floor", 0, false);
			Tilemap floor = floorObject.GetComponent<Tilemap>();
			if (settings != null && settings.FloorTile != null)
			{
				foreach (Vector2Int cell in data.Cells) floor.SetTile(new Vector3Int(cell.x, cell.y, 0), settings.FloorTile);
				floor.RefreshAllTiles();
			}
			CreateWallLayer(root.transform, "Walls_South", data, true, settings, settings == null ? 1 : settings.SouthWallSortingOrder);
			CreateWallLayer(root.transform, "Walls_Other", data, false, settings, 3);
			SerializedObject serialized = new(section);
			serialized.FindProperty("sectionId").stringValue = name;
			serialized.FindProperty("grid").objectReferenceValue = grid;
			serialized.FindProperty("footprint").objectReferenceValue = footprint;
			serialized.ApplyModifiedPropertiesWithoutUndo();
			SetFootprint(footprint, data.Cells);
			AddProps(root.transform, data, settings, grid);
			AddConnectors(root.transform, section, data, settings);
			serialized = new SerializedObject(section);
			SerializedProperty props = serialized.FindProperty("props");
			SpriteRenderer[] renderers = root.GetComponentsInChildren<SpriteRenderer>();
			props.arraySize = renderers.Length;
			for (int i = 0; i < renderers.Length; i++) props.GetArrayElementAtIndex(i).objectReferenceValue = renderers[i];
			serialized.ApplyModifiedPropertiesWithoutUndo();
			return root;
		}

		private static GameObject CreateTilemap(Transform parent, string name, int order, bool collider)
		{
			GameObject obj = new(name);
			obj.transform.SetParent(parent);
			obj.AddComponent<Tilemap>();
			TilemapRenderer renderer = obj.AddComponent<TilemapRenderer>();
			renderer.sortingOrder = order;
			if (collider) obj.AddComponent<TilemapCollider2D>();
			return obj;
		}

		private static void CreateWallLayer(Transform parent, string name, GeneratedRoomSection data, bool south, RoomSectionGeneratorSettings settings, int order)
		{
			GameObject obj = CreateTilemap(parent, name, order, true);
			Tilemap map = obj.GetComponent<Tilemap>();
			if (settings == null) return;
			foreach (GeneratedWall wall in data.Walls)
			{
				if ((wall.Direction == GeneratedWallDirection.South) != south) continue;
				TileBase tile = GetWallTile(settings, wall);
				if (tile != null) map.SetTile(new Vector3Int(wall.Cell.x, wall.Cell.y + wall.Layer, 0), tile);
			}
		}

		private static TileBase GetWallTile(RoomSectionGeneratorSettings settings, GeneratedWall wall)
		{
			if (wall.Direction == GeneratedWallDirection.North)
			{
				List<TileBase> layers = wall.Corner switch
				{
					GeneratedWallCorner.NorthWest => settings.NorthWestCornerLayers,
					GeneratedWallCorner.NorthEast => settings.NorthEastCornerLayers,
					GeneratedWallCorner.InnerNorthWest => settings.InnerNorthWestCornerLayers,
					GeneratedWallCorner.InnerNorthEast => settings.InnerNorthEastCornerLayers,
					_ => settings.NorthWallLayers
				};
				return wall.Layer < layers.Count ? layers[wall.Layer] : null;
			}
			return wall.Direction switch
			{
				GeneratedWallDirection.South => wall.Corner switch
				{
					GeneratedWallCorner.SouthWest => settings.SouthWestCornerTile,
					GeneratedWallCorner.SouthEast => settings.SouthEastCornerTile,
					GeneratedWallCorner.InnerSouthWest => settings.InnerSouthWestCornerTile,
					GeneratedWallCorner.InnerSouthEast => settings.InnerSouthEastCornerTile,
					GeneratedWallCorner.SouthWestCap => settings.SouthWestCapTile,
					GeneratedWallCorner.SouthEastCap => settings.SouthEastCapTile,
					_ => settings.SouthWallTile
				},
				GeneratedWallDirection.East => settings.EastWallTile,
				_ => settings.WestWallTile
			};
		}

		private static void SetFootprint(RoomSectionFootprint footprint, HashSet<Vector2Int> cells)
		{
			SerializedObject serialized = new(footprint);
			SerializedProperty rectangles = serialized.FindProperty("rectangles");
			List<FootprintRectangle> rows = new();
			foreach (int y in cells.Select(c => c.y).Distinct().OrderBy(value => value))
			{
				List<int> xs = cells.Where(c => c.y == y).Select(c => c.x).OrderBy(value => value).ToList();
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

		private static void AddProps(Transform parent, GeneratedRoomSection data, RoomSectionGeneratorSettings settings, Grid grid)
		{
			if (settings == null || settings.Props == null || settings.Props.Count == 0) return;
			for (int i = 0; i < data.Props.Count && i < settings.Props.Count; i++)
			{
				RoomSectionGeneratorProp definition = settings.Props[i];
				if (definition.Prefab == null) continue;
				GameObject prop = (GameObject)PrefabUtility.InstantiatePrefab(definition.Prefab, parent);
				Vector2Int cell = data.Props[i];
				prop.transform.localPosition = grid.GetCellCenterLocal(new Vector3Int(cell.x, cell.y, 0));
			}
		}

		private static void AddConnectors(Transform parent, RoomSection section, GeneratedRoomSection data, RoomSectionGeneratorSettings settings)
		{
			if (settings == null || settings.ConnectorShape == null) return;
			List<RoomSectionConnector> connectors = new();
			if (settings.NorthConnector) connectors.Add(CreateConnector(parent, data, settings, ConnectorDirection.North));
			if (settings.EastConnector) connectors.Add(CreateConnector(parent, data, settings, ConnectorDirection.East));
			if (settings.SouthConnector) connectors.Add(CreateConnector(parent, data, settings, ConnectorDirection.South));
			if (settings.WestConnector) connectors.Add(CreateConnector(parent, data, settings, ConnectorDirection.West));
			SerializedObject serialized = new(section);
			SerializedProperty property = serialized.FindProperty("connectors");
			property.arraySize = connectors.Count;
			for (int i = 0; i < connectors.Count; i++) property.GetArrayElementAtIndex(i).objectReferenceValue = connectors[i];
			serialized.ApplyModifiedPropertiesWithoutUndo();
		}

		private static RoomSectionConnector CreateConnector(Transform parent, GeneratedRoomSection data, RoomSectionGeneratorSettings settings, ConnectorDirection direction)
		{
			Vector2Int cell = direction switch
			{
				ConnectorDirection.North => data.Cells.OrderByDescending(c => c.y).ThenBy(c => c.x).First(),
				ConnectorDirection.East => data.Cells.OrderByDescending(c => c.x).ThenBy(c => c.y).First(),
				ConnectorDirection.South => data.Cells.OrderBy(c => c.y).ThenBy(c => c.x).First(),
				_ => data.Cells.OrderBy(c => c.x).ThenBy(c => c.y).First()
			};
			GameObject obj = new($"Connector_{direction}");
			obj.transform.SetParent(parent);
			Grid grid = parent.GetComponentInChildren<Grid>(true);
			obj.transform.localPosition = grid == null ? new Vector3(cell.x, cell.y, 0f) : grid.GetCellCenterLocal(new Vector3Int(cell.x, cell.y, 0));
			RoomSectionConnector connector = obj.AddComponent<RoomSectionConnector>();
			SerializedObject serialized = new(connector);
			serialized.FindProperty("cellPosition").vector2IntValue = cell;
			serialized.FindProperty("direction").enumValueIndex = (int)direction;
			serialized.FindProperty("shape").objectReferenceValue = settings.ConnectorShape;
			serialized.FindProperty("type").enumValueIndex = (int)settings.ConnectorType;
			serialized.ApplyModifiedPropertiesWithoutUndo();
			return connector;
		}
	}
}
