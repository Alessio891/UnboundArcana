using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnboundArcana.Core.Rooms
{
	public class ConnectorTilemapOverride : MonoBehaviour
	{
		[SerializeField]
		private Tilemap openOverlay;

		[SerializeField]
		private Tilemap closedOverlay;

		public Tilemap OpenOverlay => openOverlay;
		public Tilemap ClosedOverlay => closedOverlay;
		private void Awake()
		{
			ApplyClosed();
		}
		public void ApplyOpen()
		{
			if (openOverlay != null)
				openOverlay.gameObject.SetActive(true);

			if (closedOverlay != null)
				closedOverlay.gameObject.SetActive(false);
		}

		public void ApplyClosed()
		{
			if (openOverlay != null)
				openOverlay.gameObject.SetActive(false);

			if (closedOverlay != null)
				closedOverlay.gameObject.SetActive(true);
		}

		public bool IsValid()
		{
			return openOverlay != null &&
				closedOverlay != null;
		}
#if UNITY_EDITOR
		public void Assign(
			Tilemap open,
			Tilemap closed)
		{
			openOverlay = open;
			closedOverlay = closed;

			UnityEditor.EditorUtility.SetDirty(this);
		}
#endif
#if UNITY_EDITOR
		public IEnumerable<Vector3Int> GetInvalidTiles(
			RoomSectionConnector connector)
		{
			if (closedOverlay == null ||
				connector.Shape == null)
				yield break;

			Grid grid =
				connector.GetComponentInParent<Grid>();

			if (grid == null)
				yield break;

			HashSet<Vector2Int> valid =
				connector.Shape.GetOverlaySet(
					connector.CellPosition);

			foreach (var pos in closedOverlay.cellBounds.allPositionsWithin)
			{
				if (closedOverlay.GetTile(pos) == null)
					continue;

				Vector2Int cell = new(
					pos.x,
					pos.y);

				if (!valid.Contains(cell))
					yield return pos;
			}
		}
#endif
#if UNITY_EDITOR
		public void TrimInvalidTiles(
			RoomSectionConnector connector)
		{
			if (closedOverlay == null)
				return;

			var invalid =
				new List<Vector3Int>(
					GetInvalidTiles(connector));

			if (invalid.Count == 0)
				return;

			bool confirm =
				UnityEditor.EditorUtility.DisplayDialog(
					"Remove invalid tiles?",
					$"{invalid.Count} tiles are outside the connector shape.",
					"Remove",
					"Cancel");

			if (!confirm)
				return;


			Undo.RecordObject(
				closedOverlay,
				"Trim Connector Tiles");

			foreach (var cell in invalid)
			{
				closedOverlay.SetTile(
					cell,
					null);
			}

			EditorUtility.SetDirty(
				closedOverlay);
		}
#endif
	}
}