using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnboundArcana.Core.Rooms.Editor
{
	public static class RoomSectionConnectorAuthoringView
	{
		private static readonly Dictionary<TilemapRenderer, bool> savedRendererStates = new();
		private static RoomSectionConnector isolatedConnector;
		private static Tilemap isolatedTilemap;

		public static bool ShowContract { get; private set; }
		public static bool ShowColliders { get; private set; }
		public static bool ShowLayerBoundaries { get; private set; }
		public static bool ShowInvalidCells { get; private set; }

		public static Tilemap SelectedTilemap => isolatedTilemap;

		public static void ToggleContract()
		{
			ShowContract = !ShowContract;
			SceneView.RepaintAll();
		}

		public static void ToggleColliders()
		{
			ShowColliders = !ShowColliders;
			SceneView.RepaintAll();
		}

		public static void ToggleLayerBoundaries()
		{
			ShowLayerBoundaries = !ShowLayerBoundaries;
			SceneView.RepaintAll();
		}

		public static void ToggleInvalidCells()
		{
			ShowInvalidCells = !ShowInvalidCells;
			SceneView.RepaintAll();
		}

		public static void SelectLayer(Tilemap tilemap)
		{
			if (tilemap == null)
				return;

			isolatedTilemap = tilemap;
			Selection.activeGameObject = tilemap.gameObject;
			SceneView.lastActiveSceneView?.FrameSelected();
			SceneView.RepaintAll();
		}

		public static void IsolateLayer(RoomSectionConnector connector, Tilemap tilemap)
		{
			if (connector == null || tilemap == null)
				return;

			RestoreVisibility();
			savedRendererStates.Clear();
			isolatedConnector = connector;
			isolatedTilemap = tilemap;

			foreach (TilemapRenderer renderer in connector.GetComponentsInChildren<TilemapRenderer>(true))
			{
				savedRendererStates[renderer] = renderer.enabled;
				renderer.enabled = renderer == tilemap.GetComponent<TilemapRenderer>();
			}

			Selection.activeGameObject = tilemap.gameObject;
			SceneView.lastActiveSceneView?.FrameSelected();
			SceneView.RepaintAll();
		}

		public static void RestoreVisibility()
		{
			foreach (KeyValuePair<TilemapRenderer, bool> state in savedRendererStates)
			{
				if (state.Key != null)
					state.Key.enabled = state.Value;
			}

			savedRendererStates.Clear();
			isolatedConnector = null;
			isolatedTilemap = null;
			SceneView.RepaintAll();
		}

		public static bool ShouldDraw(RoomSectionConnector connector, Tilemap tilemap)
		{
			return isolatedConnector == null ||
				(isolatedConnector == connector && isolatedTilemap == tilemap);
		}
	}
}
