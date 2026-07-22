using UnityEditor;
using UnityEngine;

namespace UnboundArcana.Core.Rooms
{
	public class RoomSectionConnector : MonoBehaviour
	{
		[SerializeField]
		private Vector2Int cellPosition;

		[SerializeField]
		private ConnectorDirection direction;

		[SerializeField]
		private ConnectorType type;

		[SerializeField]
		private int width = 1;

		public Vector2Int CellPosition => cellPosition;
		public ConnectorDirection Direction => direction;
		public ConnectorType Type => type;
		public int Width => width;

		public RoomSectionConnector ConnectedTo { get; set; }
		public void RefreshCellPosition(Grid grid)
		{
			Vector3Int cell =
				grid.WorldToCell(transform.position);

			cellPosition = new Vector2Int(
				cell.x,
				cell.y
			);

			transform.position =
				grid.GetCellCenterWorld(cell);
		}
		private void OnDrawGizmosSelected()
		{
			if (ConnectedTo == null)
				return;

			Gizmos.color = Color.green;

			Gizmos.DrawLine(
				transform.position,
				ConnectedTo.transform.position
			);
		}

		public Vector2Int GetDirectionOffset()
		{
			return Direction switch
			{
				ConnectorDirection.North => Vector2Int.up,
				ConnectorDirection.East => Vector2Int.right,
				ConnectorDirection.South => Vector2Int.down,
				ConnectorDirection.West => Vector2Int.left,
				_ => Vector2Int.zero
			};
		}

		public void SnapToGrid(Grid grid)
		{
			Vector3Int cell = grid.WorldToCell(transform.position);

			cellPosition = new Vector2Int(
				cell.x,
				cell.y
			);

			transform.position = grid.GetCellCenterWorld(cell);
		}
	}
}