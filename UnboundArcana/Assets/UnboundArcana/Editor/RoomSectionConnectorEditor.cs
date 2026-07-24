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
		private RoomSectionPreviewLibrary previewLibrary;

		private void OnEnable()
		{
			connector =
				(RoomSectionConnector)target;
		}

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			if (GUILayout.Button("Snap Cell Position"))
			{
				Grid grid =
					connector.GetComponentInParent<Grid>();

				if (grid != null)
				{
					Undo.RecordObject(
						connector.transform,
						"Snap Connector");

					Undo.RecordObject(
						connector,
						"Update Connector Position");

					connector.SnapToGrid(grid);

					EditorUtility.SetDirty(connector);
				}
			}

			EditorGUILayout.Space();

			EditorGUILayout.LabelField(
				"Connector Overlay",
				EditorStyles.boldLabel);

			if (GUILayout.Button("Create Overlay Roots"))
			{
				CreateOverlayRoots();
			}

			if (GUILayout.Button("Validate Overlay"))
			{
				ValidateOverlay();
			}

			if (GUILayout.Button("Trim Overlay To Shape"))
			{
				TrimOverlay();
			}

			ConnectorTilemapOverride overlay =
				connector.TilemapOverride;

			if (overlay != null)
			{
				if (GUILayout.Button("Preview Open"))
				{
					overlay.ApplyOpen();
					RoomSectionPreviewSpawner.ApplyOpen();
				}

				if (GUILayout.Button("Preview Closed"))
				{
					overlay.ApplyClosed();
					RoomSectionPreviewSpawner.ApplyClosed();
				}
			}

			EditorGUILayout.Space();

			EditorGUILayout.LabelField(
				"Room Preview",
				EditorStyles.boldLabel);

			previewLibrary =
				(RoomSectionPreviewLibrary)
				EditorGUILayout.ObjectField(
					"Preview Library",
					previewLibrary,
					typeof(RoomSectionPreviewLibrary),
					false);

			if (GUILayout.Button("Spawn Compatible Sections"))
			{
				RoomSectionPreviewSpawner.Spawn(
					connector,
					previewLibrary);
			}

			if (GUILayout.Button("Clear Preview"))
			{
				RoomSectionPreviewSpawner.Clear();
			}
		}

		private void CreateOverlayRoots()
		{
			ConnectorTilemapOverride component =
				connector.TilemapOverride;

			if (component == null)
			{
				component =
					Undo.AddComponent<ConnectorTilemapOverride>(
						connector.gameObject);
			}

			GameObject open =
				CreateRoot("OpenOverlay");

			GameObject closed =
				CreateRoot("ClosedOverlay");

			CreateLayerIfMissing(open);
			CreateLayerIfMissing(closed);

			component.Assign(
				open,
				closed);

			EditorUtility.SetDirty(component);
		}

		private GameObject CreateRoot(
			string name)
		{
			Transform existing =
				connector.transform.Find(name);

			if (existing != null)
				return existing.gameObject;

			GameObject obj =
				new GameObject(name);

			Undo.RegisterCreatedObjectUndo(
				obj,
				"Create Overlay Root");

			obj.transform.SetParent(
				connector.transform);

			obj.transform.localPosition =
				Vector3.zero;

			return obj;
		}

		private void CreateLayerIfMissing(
			GameObject root)
		{
			if (root.GetComponentInChildren<Tilemap>() != null)
				return;

			GameObject obj =
				new GameObject("Layer_0");

			Undo.RegisterCreatedObjectUndo(
				obj,
				"Create Overlay Layer");

			obj.transform.SetParent(
				root.transform);

			Tilemap tilemap =
				obj.AddComponent<Tilemap>();

			TilemapRenderer renderer =
				obj.AddComponent<TilemapRenderer>();

			obj.AddComponent<TilemapCollider2D>();

			CopyRendererSettings(renderer);
		}

		private IEnumerable<Tilemap> GetOverlayTilemaps()
		{
			ConnectorTilemapOverride overlay =
				connector.TilemapOverride;

			if (overlay == null)
				yield break;

			if (overlay.OpenRoot != null)
			{
				foreach (var tilemap in
					overlay.OpenRoot
					.GetComponentsInChildren<Tilemap>(true))
				{
					yield return tilemap;
				}
			}

			if (overlay.ClosedRoot != null)
			{
				foreach (var tilemap in
					overlay.ClosedRoot
					.GetComponentsInChildren<Tilemap>(true))
				{
					yield return tilemap;
				}
			}
		}

		private void TrimOverlay()
		{
			HashSet<Vector2Int> valid =
				new(
					connector.GetOverlayContractCells());

			foreach (Tilemap tilemap in GetOverlayTilemaps())
			{
				TrimTilemap(
					tilemap,
					valid);
			}
		}

		private void TrimTilemap(
			Tilemap tilemap,
			HashSet<Vector2Int> valid)
		{
			Undo.RecordObject(
				tilemap,
				"Trim Connector Overlay");

			foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
			{
				if (tilemap.GetTile(pos) == null)
					continue;

				Vector2Int cell =
					new(
						pos.x,
						pos.y);

				if (!valid.Contains(cell))
					tilemap.SetTile(
						pos,
						null);
			}

			EditorUtility.SetDirty(tilemap);
		}

		private void ValidateOverlay()
		{
			if (connector.TilemapOverride == null ||
				!connector.TilemapOverride.IsValid())
			{
				Debug.LogWarning(
					"Invalid connector overlay",
					connector);

				return;
			}

			Debug.Log(
				"Connector overlay valid",
				connector);
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

			renderer.sortingLayerName = "Interactives";

			renderer.sortingOrder =
				reference.sortingOrder + 1;
		}
	}
}
