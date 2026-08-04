using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnboundArcana.Core.Rooms.Editor
{
	public static class RoomSectionStyleProfileMigration
	{
		[MenuItem("Unbound Arcana/Rooms/Create Style Profile From Selected Legacy Settings")]
		private static void CreateFromSelection()
		{
			RoomSectionGeneratorSettings legacy = Selection.activeObject as RoomSectionGeneratorSettings;
			if (legacy == null) return;
			RoomSectionStyleProfile profile = CreateFromLegacy(legacy);
			string sourcePath = AssetDatabase.GetAssetPath(legacy);
			string folder = string.IsNullOrEmpty(sourcePath) ? "Assets/RoomSections" : System.IO.Path.GetDirectoryName(sourcePath).Replace("\\", "/");
			EnsureFolder(folder);
			string path = AssetDatabase.GenerateUniqueAssetPath($"{folder}/{legacy.name}_StyleProfile.asset");
			AssetDatabase.CreateAsset(profile, path);
			AssetDatabase.SaveAssets();
			Selection.activeObject = profile;
			Debug.Log($"Created room section style profile: {path}");
		}

		[MenuItem("Unbound Arcana/Rooms/Create Style Profile From Selected Legacy Settings", true)]
		private static bool ValidateCreateFromSelection()
		{
			return Selection.activeObject is RoomSectionGeneratorSettings;
		}

		public static RoomSectionStyleProfile CreateFromLegacy(RoomSectionGeneratorSettings legacy)
		{
			RoomSectionStyleProfile profile = ScriptableObject.CreateInstance<RoomSectionStyleProfile>();
			profile.name = $"{legacy.name}_StyleProfile";
			profile.CellSize = legacy.CellSize;
			profile.RequireSquareCells = legacy.RequireSquareCells;
			profile.SouthWallSortingOrder = legacy.SouthWallSortingOrder;
			profile.OtherWallSortingOrder = 3;
			profile.FloorTile = legacy.FloorTile;
			profile.NorthWallLayerCount = Mathf.Max(1, legacy.NorthWallLayers.Count);
			profile.EastWallLayerCount = 1;
			profile.SouthWallLayerCount = 1;
			profile.WestWallLayerCount = 1;
			AddDirectionalMapping(profile, GeneratedWallDirection.South, GeneratedBoundaryTopology.Straight, GeneratedBoundaryHandedness.None, 0, legacy.SouthWallTile);
			AddDirectionalMapping(profile, GeneratedWallDirection.East, GeneratedBoundaryTopology.Straight, GeneratedBoundaryHandedness.None, 0, legacy.EastWallTile);
			AddDirectionalMapping(profile, GeneratedWallDirection.West, GeneratedBoundaryTopology.Straight, GeneratedBoundaryHandedness.None, 0, legacy.WestWallTile);
			for (int layer = 0; layer < legacy.NorthWallLayers.Count; layer++) AddDirectionalMapping(profile, GeneratedWallDirection.North, GeneratedBoundaryTopology.Straight, GeneratedBoundaryHandedness.None, layer, legacy.NorthWallLayers[layer]);
			AddLayerMappings(profile, GeneratedWallDirection.North, GeneratedBoundaryTopology.OuterTurn, GeneratedBoundaryHandedness.Left, legacy.NorthWestCornerLayers);
			AddLayerMappings(profile, GeneratedWallDirection.North, GeneratedBoundaryTopology.OuterTurn, GeneratedBoundaryHandedness.Right, legacy.NorthEastCornerLayers);
			AddLayerMappings(profile, GeneratedWallDirection.North, GeneratedBoundaryTopology.InnerTurn, GeneratedBoundaryHandedness.Left, legacy.InnerNorthWestCornerLayers);
			AddLayerMappings(profile, GeneratedWallDirection.North, GeneratedBoundaryTopology.InnerTurn, GeneratedBoundaryHandedness.Right, legacy.InnerNorthEastCornerLayers);
			AddDirectionalMapping(profile, GeneratedWallDirection.South, GeneratedBoundaryTopology.OuterTurn, GeneratedBoundaryHandedness.Left, 0, legacy.SouthWestCornerTile);
			AddDirectionalMapping(profile, GeneratedWallDirection.South, GeneratedBoundaryTopology.OuterTurn, GeneratedBoundaryHandedness.Right, 0, legacy.SouthEastCornerTile);
			AddDirectionalMapping(profile, GeneratedWallDirection.South, GeneratedBoundaryTopology.InnerTurn, GeneratedBoundaryHandedness.Left, 0, legacy.InnerSouthWestCornerTile != null ? legacy.InnerSouthWestCornerTile : legacy.SouthWestCapTile);
			AddDirectionalMapping(profile, GeneratedWallDirection.South, GeneratedBoundaryTopology.InnerTurn, GeneratedBoundaryHandedness.Right, 0, legacy.InnerSouthEastCornerTile != null ? legacy.InnerSouthEastCornerTile : legacy.SouthEastCapTile);
			AddDirectionalMapping(profile, GeneratedWallDirection.South, GeneratedBoundaryTopology.Cap, GeneratedBoundaryHandedness.None, 0, legacy.SouthWestCapTile != null ? legacy.SouthWestCapTile : legacy.SouthEastCapTile);
			return profile;
		}

		private static void AddLayerMappings(RoomSectionStyleProfile profile, GeneratedWallDirection direction, GeneratedBoundaryTopology topology, GeneratedBoundaryHandedness handedness, System.Collections.Generic.List<TileBase> tiles)
		{
			for (int layer = 0; layer < tiles.Count; layer++) AddDirectionalMapping(profile, direction, topology, handedness, layer, tiles[layer]);
		}

		private static void AddDirectionalMapping(RoomSectionStyleProfile profile, GeneratedWallDirection direction, GeneratedBoundaryTopology topology, GeneratedBoundaryHandedness handedness, int layer, TileBase tile)
		{
			profile.WallMappings.Add(new RoomSectionWallVisualMapping { Direction = direction, Topology = topology, Handedness = handedness, Layer = layer, TargetPass = RoomSectionStyleProfile.GetDefaultRenderPass(direction), Tile = tile });
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
}
