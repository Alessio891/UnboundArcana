using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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
		public Grid SectionGrid => grid;

		[FormerlySerializedAs("Props")]
		[SerializeField]
		private List<SpriteRenderer> props = new();
		public IReadOnlyList<SpriteRenderer> Props => props;

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
		public bool ContainsWorldPosition(Vector3 worldPosition)
		{
			if (grid == null || footprint == null)
				return false;

			Vector3Int cell =
				grid.WorldToCell(worldPosition);

			return ContainsCell(
				new Vector2Int(
					cell.x,
					cell.y));
		}
	}
}
