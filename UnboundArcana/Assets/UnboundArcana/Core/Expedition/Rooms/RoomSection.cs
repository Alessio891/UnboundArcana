using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnboundArcana.Core.Rooms
{
	public class RoomSection : MonoBehaviour
	{
		[SerializeField]
		private string sectionId;

		[SerializeField]
		private List<RoomSectionConnector> connectors = new();
		[SerializeField]
		private RoomSectionFootprint footprint;
		public RoomSectionFootprint Footprint => footprint;
		[SerializeField]
		private List<RoomMarker> markers = new();

		public IReadOnlyList<RoomMarker> Markers => markers;
		[SerializeField]
		private Grid grid;

		[SerializeField]
		private Vector3 gridOffset;
		public string SectionId => sectionId;
		public IReadOnlyList<RoomSectionConnector> Connectors => connectors;

		public bool ContainsCell(Vector2Int localCell)
		{
			if (footprint == null)
				return false;

			foreach (var cell in footprint.GetCells())
			{
				if (cell == localCell)
					return true;
			}

			return false;
		}

		public IEnumerable<Vector2Int> GetFootprintCells()
		{
			if (footprint == null)
				yield break;

			foreach (var cell in footprint.GetCells())
			{
				yield return cell;
			}
		}

		public Bounds GetBounds()
		{
			var renderers = GetComponentsInChildren<Renderer>();

			if (renderers.Length == 0)
				return new Bounds(transform.position, Vector3.zero);

			Bounds bounds = renderers[0].bounds;

			for (int i = 1; i < renderers.Length; i++)
				bounds.Encapsulate(renderers[i].bounds);

			return bounds;
		}
#if UNITY_EDITOR
		[ContextMenu("Refresh Markers")]
		private void RefreshMarkers()
		{
			markers.Clear();

			markers.AddRange(
				GetComponentsInChildren<RoomMarker>());

			EditorUtility.SetDirty(this);
		}
#endif
#if UNITY_EDITOR
		[ContextMenu("Align Grid To Section Origin")]
		private void AlignGrid()
		{
			if (grid == null)
				return;

			grid.transform.localPosition = gridOffset;

			UnityEditor.EditorUtility.SetDirty(grid);
		}
#endif
#if UNITY_EDITOR
		private void OnDrawGizmosSelected()
		{
			if (grid == null)
				return;
			if (!footprint.showGrid) return;
			Gizmos.color = Color.yellow;

			foreach (var cell in GetFootprintCells())
			{
				Vector3 center =
					grid.GetCellCenterWorld(
						new Vector3Int(
							cell.x,
							cell.y,
							0));

				Gizmos.DrawWireCube(
					center,
					grid.cellSize
				);
			}
		}
#endif
#if UNITY_EDITOR
		[ContextMenu("Normalize Tilemaps To Origin")]
		private void NormalizeTilemaps()
		{
			if (grid == null)
			{
				Debug.LogWarning(
					"Cannot normalize tilemaps. No Grid assigned.",
					this);
				return;
			}

			var tilemaps = GetComponentsInChildren<Tilemap>();

			Vector3Int? normalizationOffset = null;

			foreach (var tilemap in tilemaps)
			{
				BoundsInt bounds = tilemap.cellBounds;

				if (bounds.size == Vector3Int.zero)
					continue;

				normalizationOffset = -bounds.min;
				break;
			}

			if (!normalizationOffset.HasValue)
			{
				Debug.Log("No tiles found.");
				return;
			}

			Vector3Int offset = normalizationOffset.Value;

			Undo.RecordObject(this, "Normalize Room Section");

			foreach (var tilemap in tilemaps)
			{
				Undo.RecordObject(tilemap, "Normalize Tilemap");

				BoundsInt bounds = tilemap.cellBounds;

				var tiles = new List<(Vector3Int, TileBase)>();

				foreach (var position in bounds.allPositionsWithin)
				{
					TileBase tile = tilemap.GetTile(position);

					if (tile != null)
					{
						tiles.Add((position, tile));
					}
				}

				tilemap.ClearAllTiles();

				foreach (var tile in tiles)
				{
					tilemap.SetTile(
						tile.Item1 + offset,
						tile.Item2
					);
				}

				EditorUtility.SetDirty(tilemap);
			}

			Debug.Log(
				$"Normalized room section by {offset}"
			);
		}
#endif
	}
}