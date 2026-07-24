using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnboundArcana.Core.Rooms.Editor
{
	public static class RoomSectionStatsUtility
	{
		public static int GetConnectorCount(
			RoomSection section)
		{
			if (section == null)
				return 0;

			return section.Connectors.Count;
		}

		public static int GetMarkerCount(
			RoomSection section)
		{
			if (section == null)
				return 0;

			return section.Markers.Count;
		}

		public static int GetTilemapCount(
			RoomSection section)
		{
			if (section == null)
				return 0;

			return section.GetComponentsInChildren<Tilemap>()
				.Length;
		}

		public static int GetTileCount(
			RoomSection section)
		{
			if (section == null)
				return 0;

			int count = 0;

			foreach (Tilemap tilemap in
				section.GetComponentsInChildren<Tilemap>())
			{
				foreach (Vector3Int position in
					tilemap.cellBounds.allPositionsWithin)
				{
					if (tilemap.GetTile(position) != null)
						count++;
				}
			}

			return count;
		}

		public static string GetFootprintSize(
			RoomSection section)
		{
			if (section == null ||
				section.Footprint == null)
				return "-";

			HashSet<Vector2Int> cells =
				new();

			foreach (Vector2Int cell in
				section.GetFootprintCells())
			{
				cells.Add(cell);
			}

			if (cells.Count == 0)
				return "-";

			int minX = int.MaxValue;
			int minY = int.MaxValue;
			int maxX = int.MinValue;
			int maxY = int.MinValue;

			foreach (Vector2Int cell in cells)
			{
				minX = Mathf.Min(minX, cell.x);
				minY = Mathf.Min(minY, cell.y);
				maxX = Mathf.Max(maxX, cell.x);
				maxY = Mathf.Max(maxY, cell.y);
			}

			return
				$"{maxX - minX + 1} x {maxY - minY + 1}";
		}
	}
}