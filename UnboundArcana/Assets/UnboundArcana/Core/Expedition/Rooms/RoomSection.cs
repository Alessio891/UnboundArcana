using System.Collections.Generic;
using UnityEngine;

namespace UnboundArcana.Core.Rooms
{
	public class RoomSection : MonoBehaviour
	{
		[SerializeField]
		private string sectionId;

		[SerializeField]
		private List<RoomSectionConnector> connectors = new();

		public string SectionId => sectionId;
		public IReadOnlyList<RoomSectionConnector> Connectors => connectors;

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
	}
}