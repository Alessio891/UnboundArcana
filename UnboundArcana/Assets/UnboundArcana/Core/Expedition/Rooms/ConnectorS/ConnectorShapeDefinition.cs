using System.Collections.Generic;
using UnityEngine;

namespace UnboundArcana.Core.Rooms
{
	[CreateAssetMenu(
		menuName = "Unbound Arcana/Rooms/Connector Shape")]
	public class ConnectorShapeDefinition : ScriptableObject
	{
		[SerializeField]
		private string shapeId;

		[SerializeField]
		private Vector2Int size = Vector2Int.one;

		public string ShapeId => shapeId;
		public Vector2Int Size => size;

		public HashSet<Vector2Int> GetOverlaySet(
			Vector2Int start)
		{
			HashSet<Vector2Int> cells = new();

			foreach (var cell in GetCells(start))
			{
				cells.Add(cell);
			}

			return cells;
		}

		public IEnumerable<Vector2Int> GetCells(
			Vector2Int start)
		{
			for (int x = 0; x < size.x; x++)
			{
				for (int y = 0; y < size.y; y++)
				{
					yield return start +
						new Vector2Int(x, y);
				}
			}
		}
	}
}