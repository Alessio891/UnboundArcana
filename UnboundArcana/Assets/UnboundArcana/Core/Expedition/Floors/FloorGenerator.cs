using UnityEngine;
using UnboundArcana.Core.Rooms;

namespace UnboundArcana.Core.Expedition
{
	public class FloorGenerator
	{
		public FloorInstance Generate(
			FloorDefinition definition)
		{
			if (definition == null)
				return null;


			FloorInstance floor =
				new FloorInstance(definition);
			int combatRoomCount = 0;
			bool laboratoryAdded = false;


			for (int i = 0; i < definition.RoomCount; i++)
			{
				RoomDefinition room =
					PickRoom(
						definition.AvailableRooms);

				if (room != null)
				{
					floor.AddRoom(room);
					if (room.Type == RoomType.Combat) { combatRoomCount++; }
					if (!laboratoryAdded && combatRoomCount == 2 && definition.LaboratoryRoom != null)
					{
						floor.AddRoom(definition.LaboratoryRoom);
						laboratoryAdded = true;
					}
				}
			}


			if (definition.BossRoom != null)
			{
				if (definition.LaboratoryRoom != null) { floor.AddRoom(definition.LaboratoryRoom); }
				floor.AddRoom(
					definition.BossRoom);
			}


			return floor;
		}


		private RoomDefinition PickRoom(
			System.Collections.Generic.IReadOnlyList<RoomDefinition> rooms)
		{
			if (rooms == null ||
				rooms.Count == 0)
			{
				return null;
			}


			return rooms[
				Random.Range(
					0,
					rooms.Count)];
		}
	}
}
