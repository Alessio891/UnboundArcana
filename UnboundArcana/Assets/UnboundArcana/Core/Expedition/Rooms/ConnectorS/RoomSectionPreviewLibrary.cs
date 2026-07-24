using System.Collections.Generic;
using UnityEngine;

namespace UnboundArcana.Core.Rooms
{
	[CreateAssetMenu(
		menuName = "Unbound Arcana/Rooms/Section Preview Library")]
	public class RoomSectionPreviewLibrary : ScriptableObject
	{
		[SerializeField]
		private List<RoomSection> sections = new();

		public IReadOnlyList<RoomSection> Sections =>
			sections;
	}
}