using System.Collections.Generic;
using UnboundArcana.Core.Rooms;

namespace UnboundArcana.Core.Expedition
{
	public class GeneratedFloor
	{
		private readonly List<RoomDefinition> rooms;

		public IReadOnlyList<RoomDefinition> Rooms =>
			rooms;


		public GeneratedFloor(
			List<RoomDefinition> rooms)
		{
			this.rooms = rooms;
		}
	}
}