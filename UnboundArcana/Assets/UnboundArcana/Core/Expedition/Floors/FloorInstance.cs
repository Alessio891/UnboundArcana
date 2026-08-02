using System.Collections.Generic;
using UnboundArcana.Core.Rooms;

namespace UnboundArcana.Core.Expedition
{
	public class FloorInstance
	{
		public FloorDefinition Definition { get; }

		private readonly List<RoomDefinition> rooms =
			new();


		public IReadOnlyList<RoomDefinition> Rooms =>
			rooms;


		public int CurrentRoomIndex { get; private set; }


		public FloorInstance(
			FloorDefinition definition)
		{
			Definition = definition;
		}


		public void AddRoom(
			RoomDefinition room)
		{
			rooms.Add(room);
		}


		public RoomDefinition GetCurrentRoom()
		{
			if (CurrentRoomIndex >= rooms.Count)
				return null;

			return rooms[CurrentRoomIndex];
		}

		public RoomDefinition GetNextRoom()
		{
			int nextIndex = CurrentRoomIndex + 1;
			return nextIndex < rooms.Count ? rooms[nextIndex] : null;
		}


		public bool Advance()
		{
			CurrentRoomIndex++;

			return CurrentRoomIndex < rooms.Count;
		}
	}
}
