using System.Collections.Generic;
using UnboundArcana.Core.Rooms;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnboundArcana.Core.Rooms.Editor
{
	[CustomEditor(typeof(RoomSection))]
	public class RoomSectionEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			EditorGUILayout.Space();

			RoomSection section = (RoomSection)target;

			if (GUILayout.Button("Setup Section"))
			{
				SetupSection(section);
			}

			if (GUILayout.Button("Gather Props"))
			{
				GatherProps(section);
			}

			if (GUILayout.Button("Refresh Markers"))
			{
				RefreshMarkers(section);
			}

			if (GUILayout.Button("Align Grid To Section Origin"))
			{
				AlignGrid(section);
			}

			if (GUILayout.Button("Normalize Tilemaps To Origin"))
			{
				NormalizeTilemaps(section);
			}
		}

		[DrawGizmo(GizmoType.Selected)]
		private static void DrawSectionGrid(RoomSection section, GizmoType gizmoType)
		{
			if (section.SectionGrid == null || section.Footprint == null || !section.Footprint.showGrid)
			{
				return;
			}

			Gizmos.color = Color.yellow;

			foreach (Vector2Int cell in section.GetFootprintCells())
			{
				Vector3 center = section.SectionGrid.GetCellCenterWorld(new Vector3Int(cell.x, cell.y, 0));
				Gizmos.DrawWireCube(center, section.SectionGrid.cellSize);
			}
		}

		private static void GatherProps(RoomSection section)
		{
			SerializedObject serializedSection = new(section);
			SerializedProperty props = serializedSection.FindProperty("props");
			props.ClearArray();

			Transform propsRoot = section.transform.Find("_props_");

			if (propsRoot != null)
			{
				int index = 0;

				foreach (Transform child in propsRoot)
				{
					SpriteRenderer renderer = child.GetComponent<SpriteRenderer>();

					if (renderer == null)
					{
						continue;
					}

					props.InsertArrayElementAtIndex(index);
					props.GetArrayElementAtIndex(index).objectReferenceValue = renderer;
					index++;
				}
			}

			serializedSection.ApplyModifiedProperties();
		}

		private static void RefreshMarkers(RoomSection section)
		{
			SerializedObject serializedSection = new(section);
			SerializedProperty markers = serializedSection.FindProperty("markers");
			RoomMarker[] foundMarkers = section.GetComponentsInChildren<RoomMarker>();
			markers.ClearArray();

			for (int i = 0; i < foundMarkers.Length; i++)
			{
				markers.InsertArrayElementAtIndex(i);
				markers.GetArrayElementAtIndex(i).objectReferenceValue = foundMarkers[i];
			}

			serializedSection.ApplyModifiedProperties();
		}

		private static void AlignGrid(RoomSection section)
		{
			SerializedObject serializedSection = new(section);
			Grid grid = serializedSection.FindProperty("grid").objectReferenceValue as Grid;

			if (grid == null)
			{
				return;
			}

			Vector3 offset = serializedSection.FindProperty("gridOffset").vector3Value;
			Undo.RecordObject(grid.transform, "Align Section Grid");
			grid.transform.localPosition = offset;
			EditorUtility.SetDirty(grid.transform);
		}

		private static void NormalizeTilemaps(RoomSection section)
		{
			if (section.SectionGrid == null)
			{
				Debug.LogWarning("Cannot normalize tilemaps. No Grid assigned.", section);
				return;
			}

			Tilemap[] tilemaps = section.GetComponentsInChildren<Tilemap>();
			Vector3Int? normalizationOffset = null;

			foreach (Tilemap tilemap in tilemaps)
			{
				if (tilemap.cellBounds.size == Vector3Int.zero)
				{
					continue;
				}

				normalizationOffset = -tilemap.cellBounds.min;
				break;
			}

			if (!normalizationOffset.HasValue)
			{
				Debug.Log("No tiles found.", section);
				return;
			}

			Vector3Int offset = normalizationOffset.Value;

			foreach (Tilemap tilemap in tilemaps)
			{
				Undo.RecordObject(tilemap, "Normalize Tilemap");
				List<(Vector3Int Position, TileBase Tile)> tiles = new();

				foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin)
				{
					TileBase tile = tilemap.GetTile(position);

					if (tile != null)
					{
						tiles.Add((position, tile));
					}
				}

				tilemap.ClearAllTiles();

				foreach ((Vector3Int position, TileBase tile) in tiles)
				{
					tilemap.SetTile(position + offset, tile);
				}

				EditorUtility.SetDirty(tilemap);
			}

			Debug.Log($"Normalized room section by {offset}", section);
		}

		private static void SetupSection(RoomSection section)
		{
			SerializedObject serializedSection = new(section);
			SerializedProperty gridProperty = serializedSection.FindProperty("grid");
			Grid grid = gridProperty.objectReferenceValue as Grid;

			if (grid == null)
			{
				Transform existing = section.transform.Find("Grid");

				if (existing != null)
				{
					grid = existing.GetComponent<Grid>();
				}

				if (grid == null)
				{
					GameObject gridObject = new("Grid");
					Undo.RegisterCreatedObjectUndo(gridObject, "Create Section Grid");
					Undo.SetTransformParent(gridObject.transform, section.transform, "Parent Section Grid");
					gridObject.transform.localPosition = Vector3.zero;
					grid = Undo.AddComponent<Grid>(gridObject);
				}

				gridProperty.objectReferenceValue = grid;
				serializedSection.ApplyModifiedProperties();
			}

			CreateTilemap(grid, "Floor", "Background", 0);
			CreateTilemap(grid, "Walls", "Interactives", 0);
			EditorUtility.SetDirty(section);
		}

		private static void CreateTilemap(Grid grid, string name, string sortingLayer, int sortingOrder)
		{
			Transform existing = grid.transform.Find(name);

			if (existing != null)
			{
				SetupRenderer(existing.GetComponent<TilemapRenderer>(), sortingLayer, sortingOrder);
				return;
			}

			GameObject tilemapObject = new(name);
			Undo.RegisterCreatedObjectUndo(tilemapObject, "Create Section Tilemap");
			Undo.SetTransformParent(tilemapObject.transform, grid.transform, "Parent Section Tilemap");
			tilemapObject.transform.localPosition = Vector3.zero;
			tilemapObject.transform.localRotation = Quaternion.identity;
			tilemapObject.transform.localScale = Vector3.one;
			Undo.AddComponent<Tilemap>(tilemapObject);
			TilemapRenderer renderer = Undo.AddComponent<TilemapRenderer>(tilemapObject);
			Undo.AddComponent<TilemapCollider2D>(tilemapObject);
			SetupRenderer(renderer, sortingLayer, sortingOrder);
		}

		private static void SetupRenderer(TilemapRenderer renderer, string sortingLayer, int sortingOrder)
		{
			if (renderer == null)
			{
				return;
			}

			Undo.RecordObject(renderer, "Configure Tilemap Renderer");
			renderer.sortingLayerName = sortingLayer;
			renderer.sortingOrder = sortingOrder;
			EditorUtility.SetDirty(renderer);
		}
	}
}
