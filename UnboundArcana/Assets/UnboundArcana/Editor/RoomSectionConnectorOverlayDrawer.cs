using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnboundArcana.Core.Rooms.Editor
{
	[InitializeOnLoad]
	public static class RoomSectionConnectorOverlayDrawer
	{
		static RoomSectionConnectorOverlayDrawer()
		{
			SceneView.duringSceneGui += OnSceneGUI;
		}

		private static void OnSceneGUI(
	SceneView sceneView)
		{
			GameObject selected =
				Selection.activeGameObject;

			if (selected == null)
				return;

			RoomSectionConnector[] connectors =
				Object.FindObjectsByType<RoomSectionConnector>(
					FindObjectsInactive.Include,
					FindObjectsSortMode.None);

			foreach (RoomSectionConnector connector in connectors)
			{
				if (!IsSelectedOrChild(
					connector,
					selected))
				{
					continue;
				}

				DrawConnector(connector);
			}
		}

		private static bool IsSelectedOrChild(
			RoomSectionConnector connector,
			GameObject selected)
		{
			return selected.transform == connector.transform ||
				selected.transform.IsChildOf(
					connector.transform);
		}

		private static void DrawConnector(
			RoomSectionConnector connector)
		{
			if (connector.TilemapOverride == null)
				return;

			foreach (Tilemap tilemap in GetTilemaps(connector))
			{
				if (tilemap == null)
					continue;

				if (!tilemap.gameObject.activeInHierarchy)
					continue;

				if (!RoomSectionConnectorAuthoringView.ShouldDraw(connector, tilemap))
					continue;

				if (RoomSectionConnectorAuthoringView.ShowContract)
					DrawContract(connector, tilemap);

				if (RoomSectionConnectorAuthoringView.ShowInvalidCells)
					DrawInvalid(connector, tilemap);

				if (RoomSectionConnectorAuthoringView.ShowLayerBoundaries)
					DrawLayerBoundaries(tilemap);

				if (RoomSectionConnectorAuthoringView.ShowColliders)
					DrawColliderBounds(tilemap);
			}
		}

		private static void DrawLayerBoundaries(Tilemap tilemap)
		{
			Color color = tilemap.name == "Layer_2" ? new Color(0.2f, 0.6f, 1f, 0.18f) : new Color(1f, 0.75f, 0.2f, 0.18f);
			foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin)
			{
				if (tilemap.GetTile(position) == null)
					continue;
				Vector3 center = tilemap.CellToWorld(position);
				Handles.DrawSolidRectangleWithOutline(new Rect(center, tilemap.layoutGrid.cellSize), Color.clear, color);
			}
		}

		private static void DrawColliderBounds(Tilemap tilemap)
		{
			TilemapCollider2D collider = tilemap.GetComponent<TilemapCollider2D>();
			if (collider == null || !collider.enabled)
				return;
			Handles.color = new Color(1f, 0.2f, 0.1f, 0.8f);
			Handles.DrawWireCube(collider.bounds.center, collider.bounds.size);
		}

		private static IEnumerable<Tilemap> GetTilemaps(
			RoomSectionConnector connector)
		{
			if (connector.TilemapOverride.OpenRoot != null)
			{
				foreach (var tilemap in connector.TilemapOverride.OpenRoot
					.GetComponentsInChildren<Tilemap>(true))
				{
					yield return tilemap;
				}
			}

			if (connector.TilemapOverride.ClosedRoot != null)
			{
				foreach (var tilemap in connector.TilemapOverride.ClosedRoot
					.GetComponentsInChildren<Tilemap>(true))
				{
					yield return tilemap;
				}
			}
		}

		private static void DrawContract(
			RoomSectionConnector connector,
			Tilemap tilemap)
		{
			HashSet<Vector2Int> cells =
				new(
					connector.GetOverlayContractCells());

			foreach (Vector2Int cell in cells)
			{
				Vector3 center =
					tilemap.CellToWorld(
						new Vector3Int(
							cell.x,
							cell.y,
							0));

				Handles.DrawSolidRectangleWithOutline(
					new Rect(
						center,
						tilemap.layoutGrid.cellSize),
					new Color(
						0,
						1,
						0,
						0.05f),
					Color.green);
			}
		}

		private static void DrawInvalid(
			RoomSectionConnector connector,
			Tilemap tilemap)
		{
			HashSet<Vector2Int> valid =
				new(
					connector.GetOverlayContractCells());

			foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
			{
				if (tilemap.GetTile(pos) == null)
					continue;

				Vector2Int cell =
					new(
						pos.x,
						pos.y);

				if (valid.Contains(cell))
					continue;

				Vector3 center =
					tilemap.CellToWorld(pos);

				Handles.DrawSolidRectangleWithOutline(
					new Rect(
						center,
						tilemap.layoutGrid.cellSize),
					new Color(
						1,
						0,
						0,
						0.35f),
					Color.red);
			}
		}
	}
}
