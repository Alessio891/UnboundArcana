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


			for (int i = 0; i < definition.RoomCount; i++)
			{
				RoomDefinition room =
					PickRoom(
						definition.AvailableRooms);

				if (room != null)
				{
					floor.AddRoom(room);
				}
			}


			if (definition.BossRoom != null)
			{
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