using System.Collections.Generic;
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
		private ConnectorShapeDefinition shape;

		[SerializeField]
		private ConnectorType type;

		[SerializeField]
		private Vector2Int contractOffset;

		[SerializeField]
		private ConnectorTilemapOverride tilemapOverride;

		public Vector2Int CellPosition => cellPosition;
		public ConnectorDirection Direction => direction;
		public ConnectorShapeDefinition Shape => shape;
		public ConnectorType Type => type;
		public Vector2Int ContractOffset => contractOffset;

		public RoomSectionConnector ConnectedTo { get; set; }

		public ConnectorTilemapOverride TilemapOverride
		{
			get
			{
				if (tilemapOverride == null)
					tilemapOverride =
						GetComponent<ConnectorTilemapOverride>();

				return tilemapOverride;
			}
		}

		public void RefreshCellPosition(Grid grid)
		{
			Vector3Int cell =
				grid.WorldToCell(transform.position);

			cellPosition = new Vector2Int(
				cell.x,
				cell.y);

			transform.position =
				grid.GetCellCenterWorld(cell);
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
			Vector3Int cell =
				grid.WorldToCell(transform.position);

			cellPosition = new Vector2Int(
				cell.x,
				cell.y);

			transform.position =
				grid.GetCellCenterWorld(cell);
		}

		public void SetConnected(bool connected)
		{
			if (TilemapOverride == null)
				return;

			if (connected)
				TilemapOverride.ApplyOpen();
			else
				TilemapOverride.ApplyClosed();
		}

		public IEnumerable<Vector2Int> GetContractCells()
		{
			if (Shape == null)
				yield break;

			Vector2Int start =
				cellPosition +
				contractOffset;

			foreach (var cell in Shape.GetCells(start))
			{
				yield return cell;
			}
		}

		public IEnumerable<Vector2Int> GetOverlayContractCells()
		{
			foreach (var cell in GetContractCells())
			{
				yield return cell;
			}
		}


#if UNITY_EDITOR
		private void OnDrawGizmosSelected()
		{
			Grid grid =
				GetComponentInParent<Grid>();

			if (grid == null)
				return;

			Gizmos.color =
				new Color(
					1f,
					1f,
					1f,
					0.65f);
			Gizmos.DrawSphere(transform.position, 0.05f);

			//foreach (Vector2Int cell in GetContractCells())
			//{
			//	Vector3 center =
			//		grid.GetCellCenterWorld(
			//			new Vector3Int(
			//				cell.x,
			//				cell.y,
			//				0));

			//	Gizmos.DrawCube(
			//		center,
			//		grid.cellSize);
			//}
		}
#endif
	}
}