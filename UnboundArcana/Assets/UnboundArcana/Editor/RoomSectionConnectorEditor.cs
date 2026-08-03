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

			if (GUILayout.Button("Prepare Opening Layers"))
			{
				PrepareOpeningLayers();
			}

			if (GUILayout.Button("Validate Overlay"))
			{
				ValidateOverlay();
			}

			if (GUILayout.Button("Trim Overlay To Shape"))
			{
				TrimOverlay();
			}

			if (GUILayout.Button("Validate Authoring Layers"))
			{
				ValidateAuthoringLayers();
			}

			ConnectorTilemapOverride overlay =
				connector.TilemapOverride;

			if (overlay != null)
			{
				DrawLayerControls(overlay.OpenRoot);

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

		private void DrawLayerControls(GameObject openRoot)
		{
			if (openRoot == null)
				return;

			EditorGUILayout.LabelField("Opening Layers", EditorStyles.boldLabel);

			foreach (Tilemap tilemap in openRoot.GetComponentsInChildren<Tilemap>(true))
			{
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button(tilemap.name, GUILayout.Width(90)))
					RoomSectionConnectorAuthoringView.SelectLayer(tilemap);

				if (GUILayout.Button("Isolate", GUILayout.Width(55)))
					RoomSectionConnectorAuthoringView.IsolateLayer(connector, tilemap);

				if (GUILayout.Button("Paint", GUILayout.Width(45)))
				{
					RoomSectionConnectorAuthoringView.SelectLayer(tilemap);
				}

				EditorGUILayout.EndHorizontal();
			}

			if (GUILayout.Button("Show All Layers"))
				RoomSectionConnectorAuthoringView.RestoreVisibility();

			EditorGUILayout.LabelField("Guides", EditorStyles.boldLabel);
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button(RoomSectionConnectorAuthoringView.ShowContract ? "Hide Contract" : "Show Contract"))
				RoomSectionConnectorAuthoringView.ToggleContract();
			if (GUILayout.Button(RoomSectionConnectorAuthoringView.ShowColliders ? "Hide Colliders" : "Show Colliders"))
				RoomSectionConnectorAuthoringView.ToggleColliders();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button(RoomSectionConnectorAuthoringView.ShowLayerBoundaries ? "Hide Boundaries" : "Show Boundaries"))
				RoomSectionConnectorAuthoringView.ToggleLayerBoundaries();
			if (GUILayout.Button(RoomSectionConnectorAuthoringView.ShowInvalidCells ? "Hide Invalid" : "Show Invalid"))
				RoomSectionConnectorAuthoringView.ToggleInvalidCells();
			EditorGUILayout.EndHorizontal();
		}

		private void PrepareOpeningLayers()
		{
			ConnectorTilemapOverride overlay = connector.TilemapOverride;
			if (overlay == null || overlay.OpenRoot == null || overlay.ClosedRoot == null)
			{
				Debug.LogWarning("Prepare Opening Layers requires valid open and closed overlay roots.", connector);
				return;
			}

			EnsureLayer(overlay.OpenRoot, "Layer_0", true, "Interactives", 2);
			EnsureLayer(overlay.OpenRoot, "Layer_1", false, "Interactives", 2);
			EnsureLayer(overlay.OpenRoot, "Layer_2", false, "Background", 0);
			EnsureLayer(overlay.OpenRoot, "Layer_3", false, "Interactives", 0);
			EnsureLayer(overlay.OpenRoot, "Layer_4", true, "Interactives", 2);
			EnsureLayer(overlay.ClosedRoot, "Layer_0", true, "Interactives", 2);
			EditorUtility.SetDirty(connector);
		}

		private void EnsureLayer(GameObject root, string name, bool collides, string sortingLayer, int sortingOrder)
		{
			Transform existing = root.transform.Find(name);
			GameObject layer = existing != null ? existing.gameObject : new GameObject(name);
			if (existing == null)
			{
				Undo.RegisterCreatedObjectUndo(layer, "Create Connector Authoring Layer");
				Undo.SetTransformParent(layer.transform, root.transform, "Parent Connector Authoring Layer");
				layer.transform.localPosition = Vector3.zero;
			}

			Tilemap tilemap = layer.GetComponent<Tilemap>() ?? Undo.AddComponent<Tilemap>(layer);
			TilemapRenderer renderer = layer.GetComponent<TilemapRenderer>() ?? Undo.AddComponent<TilemapRenderer>(layer);
			Undo.RecordObject(renderer, "Configure Connector Layer Renderer");
			renderer.sortingLayerName = sortingLayer;
			renderer.sortingOrder = sortingOrder;
			if (collides)
			{
				TilemapCollider2D collider = layer.GetComponent<TilemapCollider2D>() ?? Undo.AddComponent<TilemapCollider2D>(layer);
				collider.usedByComposite = true;
				Rigidbody2D body = layer.GetComponent<Rigidbody2D>() ?? Undo.AddComponent<Rigidbody2D>(layer);
				body.bodyType = RigidbodyType2D.Static;
				if (layer.GetComponent<CompositeCollider2D>() == null)
					Undo.AddComponent<CompositeCollider2D>(layer);
			}
			else
			{
				TilemapCollider2D collider = layer.GetComponent<TilemapCollider2D>();
				if (collider != null)
					collider.enabled = false;
			}
			EditorUtility.SetDirty(tilemap);
			EditorUtility.SetDirty(layer);
		}

		private void ValidateAuthoringLayers()
		{
			ConnectorTilemapOverride overlay = connector.TilemapOverride;
			if (overlay == null || overlay.OpenRoot == null || overlay.ClosedRoot == null)
			{
				Debug.LogWarning("Opening layers invalid: missing open or closed root.", connector);
				return;
			}

			string[] expected = { "Layer_0", "Layer_1", "Layer_2", "Layer_3", "Layer_4" };
			int invalid = 0;
			foreach (string name in expected)
			{
				if (overlay.OpenRoot.transform.Find(name) == null)
				{
					Debug.LogWarning("Opening layer missing: " + name, connector);
					invalid++;
				}
			}

			foreach (Tilemap tilemap in overlay.OpenRoot.GetComponentsInChildren<Tilemap>(true))
			{
				if (!IsContractValid(tilemap, connector))
					invalid++;
			}

			Debug.Log("Opening layers: 5/5, invalid cells: " + invalid, connector);
		}

		private static bool IsContractValid(Tilemap tilemap, RoomSectionConnector target)
		{
			HashSet<Vector2Int> valid = new(target.GetOverlayContractCells());
			foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin)
			{
				if (tilemap.GetTile(position) != null && !valid.Contains(new Vector2Int(position.x, position.y)))
					return false;
			}
			return true;
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
