using System.Collections.Generic;
using UnityEngine;

namespace UnboundArcana.Core.Rooms
{
	[System.Serializable]
	public class FootprintRectangle
	{
		public Vector2Int Position;
		public Vector2Int Size = Vector2Int.one;

		public IEnumerable<Vector2Int> GetCells()
		{
			for (int x = 0; x < Size.x; x++)
			{
				for (int y = 0; y < Size.y; y++)
				{
					yield return Position + new Vector2Int(x, y);
				}
			}
		}
	}
	public class RoomSectionFootprint : MonoBehaviour
	{
#if UNITY_EDITOR
		public bool showGrid = false;
#endif
		[SerializeField]
		private List<FootprintRectangle> rectangles = new();

		public IReadOnlyList<FootprintRectangle> Rectangles => rectangles;

		public IEnumerable<Vector2Int> GetCells()
		{
			HashSet<Vector2Int> cells = new();

			foreach (var rectangle in rectangles)
			{
				foreach (var cell in rectangle.GetCells())
				{
					cells.Add(cell);
				}
			}

			return cells;
		}

#if UNITY_EDITOR
		[ContextMenu("Generate 10x10 Rectangle")]
		private void GenerateDefaultRectangle()
		{
			rectangles.Clear();

			rectangles.Add(new FootprintRectangle
			{
				Position = Vector2Int.zero,
				Size = new Vector2Int(10, 10)
			});

			UnityEditor.EditorUtility.SetDirty(this);
		}

		[ContextMenu("Clear Footprint")]
		private void Clear()
		{
			rectangles.Clear();

			UnityEditor.EditorUtility.SetDirty(this);
		}

		private void OnDrawGizmosSelected()
		{
			if (!showGrid) return;
			var grid = GetComponentInParent<Grid>();


			if (grid == null)
				return;

			Gizmos.color = Color.yellow;

			foreach (var cell in GetCells())
			{
				Vector3 center =
					grid.GetCellCenterWorld(
						new Vector3Int(
							cell.x,
							cell.y,
							0));

				Gizmos.DrawWireCube(
					center,
					grid.cellSize);
			}
		}
#endif
	}
}
