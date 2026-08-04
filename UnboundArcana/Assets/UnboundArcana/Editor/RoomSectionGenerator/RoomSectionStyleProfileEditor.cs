using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnboundArcana.Core.Rooms.Editor
{
	[CustomEditor(typeof(RoomSectionStyleProfile))]
	public sealed class RoomSectionStyleProfileEditor : UnityEditor.Editor
	{
		private static readonly GeneratedWallDirection[] Directions = { GeneratedWallDirection.North, GeneratedWallDirection.East, GeneratedWallDirection.South, GeneratedWallDirection.West };
		private static readonly GeneratedBoundaryTopology[] Topologies = { GeneratedBoundaryTopology.Straight, GeneratedBoundaryTopology.OuterTurn, GeneratedBoundaryTopology.InnerTurn, GeneratedBoundaryTopology.Cap };
		private static readonly GeneratedBoundaryHandedness[] Handedness = { GeneratedBoundaryHandedness.None, GeneratedBoundaryHandedness.Left, GeneratedBoundaryHandedness.Right };

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			RoomSectionStyleProfile profile = (RoomSectionStyleProfile)target;
			EditorGUILayout.Space(8);
			if (GUILayout.Button("Validate Profile"))
			{
				List<string> diagnostics = profile.ValidateProfile();
				if (diagnostics.Count == 0) Debug.Log($"Style profile {profile.name} is valid.", profile);
				else Debug.LogWarning(string.Join("\n", diagnostics), profile);
			}
			if (GUILayout.Button("Create Canonical Preview")) CreateCanonicalPreview(profile);
			EditorGUILayout.LabelField("Coverage", EditorStyles.boldLabel);
			foreach (GeneratedWallDirection direction in Directions)
			{
				int layerCount = profile.GetLayerCount(direction);
				EditorGUILayout.LabelField($"{direction} ({layerCount} layer{(layerCount == 1 ? string.Empty : "s")})", EditorStyles.miniBoldLabel);
				foreach (GeneratedBoundaryTopology topology in Topologies)
				{
					string status = string.Empty;
					foreach (GeneratedBoundaryHandedness handedness in Handedness) status += $" {GetHandednessLabel(handedness)} {GetCoverage(profile, direction, topology, handedness, layerCount)}/{layerCount}";
					EditorGUILayout.LabelField($"{topology}:{status}");
				}
			}
		}

		private static int GetCoverage(RoomSectionStyleProfile profile, GeneratedWallDirection direction, GeneratedBoundaryTopology topology, GeneratedBoundaryHandedness handedness, int layerCount)
		{
			int covered = 0;
			for (int layer = 0; layer < layerCount; layer++) foreach (RoomSectionWallVisualMapping mapping in profile.WallMappings) if (mapping != null && mapping.Direction == direction && mapping.Topology == topology && mapping.Handedness == handedness && mapping.Layer == layer && mapping.Tile != null) { covered++; break; }
			return covered;
		}

		private static string GetHandednessLabel(GeneratedBoundaryHandedness handedness)
		{
			return handedness == GeneratedBoundaryHandedness.None ? "N" : handedness == GeneratedBoundaryHandedness.Left ? "L" : "R";
		}

		private static void CreateCanonicalPreview(RoomSectionStyleProfile profile)
		{
			HashSet<Vector2Int> cells = new();
			for (int x = 0; x < 7; x++)
				for (int y = 0; y < 7; y++)
					if (x >= 2 && x <= 4 || y >= 2 && y <= 4) cells.Add(new Vector2Int(x, y));
			RoomSectionShape shape = RoomSectionShapeGenerationLogic.GenerateFromCells(cells);
			RoomSectionPreviewResult preview = RoomSectionPreviewBuilder.Build(shape, profile, $"{profile.name}_CanonicalPreview");
			Selection.activeGameObject = preview.Root;
			SceneView.lastActiveSceneView?.FrameSelected();
		}
	}
}
