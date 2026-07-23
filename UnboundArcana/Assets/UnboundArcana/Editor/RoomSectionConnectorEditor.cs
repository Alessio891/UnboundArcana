using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnboundArcana.Core.Rooms.Editor
{
	[CustomEditor(typeof(RoomSectionConnector))]
	public class RoomSectionConnectorEditor : UnityEditor.Editor
	{
		private RoomSectionConnector connector;

		private void OnEnable()
		{
			connector =
				(RoomSectionConnector)target;
		}

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			EditorGUILayout.Space();

			EditorGUILayout.LabelField(
				"Connector Overlay",
				EditorStyles.boldLabel);

			if (GUILayout.Button("Create Overlay Tilemaps"))
			{
				CreateOverlayTilemaps();
			}

			if (GUILayout.Button("Validate Overlay"))
			{
				ValidateOverlay();
			}

			if (GUILayout.Button("Trim Overlay To Shape"))
			{
				TrimOverlay();
			}

			EditorGUILayout.Space();

			ConnectorTilemapOverride overlay =
				connector.GetComponent<ConnectorTilemapOverride>();

			if (overlay != null)
			{
				if (GUILayout.Button("Preview Open"))
				{
					overlay.ApplyOpen();
				}

				if (GUILayout.Button("Preview Closed"))
				{
					overlay.ApplyClosed();
				}
			}
		}

		private void TrimOverlay()
		{
			ConnectorTilemapOverride overlay =
				connector.TilemapOverride;

			if (overlay == null || !overlay.IsValid())
			{
				Debug.LogWarning(
					"Invalid connector overlay",
					connector);

				return;
			}

			bool confirm =
				EditorUtility.DisplayDialog(
					"Trim Connector Overlay",
					"Remove all tiles outside the connector shape?",
					"Trim",
					"Cancel");

			if (!confirm)
				return;

			HashSet<Vector2Int> validCells =
				new(
					connector.GetOverlayContractCells());

			TrimTilemap(
				overlay.OpenOverlay,
				validCells);

			TrimTilemap(
				overlay.ClosedOverlay,
				validCells);
		}

		private void TrimTilemap(
			Tilemap tilemap,
			HashSet<Vector2Int> validCells)
		{
			if (tilemap == null)
				return;

			Undo.RegisterCompleteObjectUndo(
				tilemap,
				"Trim Connector Overlay");

			BoundsInt bounds =
				tilemap.cellBounds;

			foreach (Vector3Int position in bounds.allPositionsWithin)
			{
				if (tilemap.GetTile(position) == null)
					continue;

				Vector2Int cell =
					new(
						position.x,
						position.y);

				if (!validCells.Contains(cell))
				{
					tilemap.SetTile(
						position,
						null);
				}
			}

			EditorUtility.SetDirty(tilemap);
		}

		private void CreateOverlayTilemaps()
		{
			Undo.RecordObject(
				connector,
				"Create Connector Overlay");

			ConnectorTilemapOverride overrideComponent =
				connector.GetComponent<ConnectorTilemapOverride>();

			if (overrideComponent == null)
			{
				overrideComponent =
					Undo.AddComponent<ConnectorTilemapOverride>(
						connector.gameObject);
			}

			CreateChild("OpenOverlay");
			CreateChild("ClosedOverlay");

			Tilemap open =
				connector.transform
				.Find("OpenOverlay")
				.GetComponent<Tilemap>();

			Tilemap closed =
				connector.transform
				.Find("ClosedOverlay")
				.GetComponent<Tilemap>();

			overrideComponent.Assign(
				open,
				closed);

			EditorUtility.SetDirty(
				connector);
		}

		private void CreateChild(
			string name)
		{
			Transform existing =
				connector.transform.Find(name);

			if (existing != null)
				return;

			GameObject obj =
				new(name);

			Undo.RegisterCreatedObjectUndo(
				obj,
				"Create Connector Overlay");

			obj.transform.SetParent(
				connector.transform);

			SetupOverlayTransform(
				obj.transform);

			Tilemap tilemap =
				obj.AddComponent<Tilemap>();

			TilemapRenderer renderer =
				obj.AddComponent<TilemapRenderer>();

			CopyRendererSettings(
				renderer);
		}

		private void SetupOverlayTransform(
	Transform overlay)
		{
			Transform gridTransform =
				connector.GetComponentInParent<Grid>().transform;

			overlay.position =
				gridTransform.position;

			overlay.rotation =
				gridTransform.rotation;

			overlay.localScale =
				Vector3.one;
		}

		private void CopyRendererSettings(
			TilemapRenderer renderer)
		{
			Grid grid =
				connector.GetComponentInParent<Grid>();

			if (grid == null)
				return;

			TilemapRenderer reference =
				grid.GetComponentInChildren<TilemapRenderer>();

			if (reference == null)
				return;

			renderer.sortingLayerID =
				reference.sortingLayerID;

			renderer.sortingOrder =
				reference.sortingOrder + 1;

			renderer.mode =
				reference.mode;
		}

		private void ValidateOverlay()
		{
			ConnectorTilemapOverride overlay =
				connector.GetComponent<ConnectorTilemapOverride>();

			if (overlay == null)
			{
				Debug.LogWarning(
					"Missing ConnectorTilemapOverride",
					connector);

				return;
			}

			if (!overlay.IsValid())
			{
				Debug.LogWarning(
					"Connector overlay is missing tilemaps",
					connector);

				return;
			}

			Debug.Log(
				"Connector overlay valid",
				connector);
		}

#if UNITY_EDITOR
		private void OnSceneGUI()
		{
			if (connector.TilemapOverride == null)
				return;

			Tilemap activeTilemap = null;

			if (connector.TilemapOverride.OpenOverlay.gameObject.activeSelf)
			{
				activeTilemap =
					connector.TilemapOverride.OpenOverlay;
			}

			if (connector.TilemapOverride.ClosedOverlay.gameObject.activeSelf)
			{
				activeTilemap =
					connector.TilemapOverride.ClosedOverlay;
			}

			if (activeTilemap == null)
				return;

			DrawContractOverlay(activeTilemap);
			DrawInvalidTiles(activeTilemap);
		}
		private void DrawContractOverlay(
	Tilemap tilemap)
		{
			HashSet<Vector2Int> cells =
				new(
					connector.GetOverlayContractCells());

			Handles.color =
				new Color(
					0f,
					1f,
					0f,
					0.15f);

			foreach (Vector2Int cell in cells)
			{
				Vector3Int tilePosition =
					new(
						cell.x,
						cell.y,
						0);

				Vector3 center =
					tilemap.CellToWorld(
						tilePosition);

				center +=
					tilemap.layoutGrid.cellSize / 2f;

				Handles.DrawSolidRectangleWithOutline(
					new Rect(
						center -
						tilemap.layoutGrid.cellSize / 2f,
						tilemap.layoutGrid.cellSize),
					new Color(
						0f,
						1f,
						0f,
						0.15f),
					Color.green);
			}
		}
		private void DrawInvalidTiles(
			Tilemap tilemap)
		{
			if (tilemap == null)
				return;

			HashSet<Vector2Int> valid =
				new(
					connector.GetOverlayContractCells());

			Handles.color =
				new Color(
					1f,
					0f,
					0f,
					0.35f);

			foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin)
			{
				if (tilemap.GetTile(position) == null)
					continue;

				Vector2Int cell =
					new(
						position.x,
						position.y);

				if (valid.Contains(cell))
					continue;

				Vector3 center =
					tilemap.layoutGrid
					.GetCellCenterWorld(position);

				Handles.DrawSolidRectangleWithOutline(
					new Rect(
						center - tilemap.layoutGrid.cellSize / 2,
						tilemap.layoutGrid.cellSize),
					new Color(1f, 0f, 0f, 0.35f),
					Color.red);
			}
		}
#endif
	}
}