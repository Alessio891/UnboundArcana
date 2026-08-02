using UnityEngine;
using UnboundArcana.Core.Events;

namespace UnboundArcana.Core.Rooms
{
	public class RoomService
	{
		private readonly RoomGenerator generator;
		private readonly GameEventBus events;

		private RoomInstance currentRoom;

		public RoomInstance CurrentRoom => currentRoom;

		public RoomService(
			RoomGenerator generator,
			GameEventBus events)
		{
			this.generator = generator;
			this.events = events;
		}

		public RoomInstance GenerateRoom(
			RoomDefinition definition,
			Transform parent)
		{
			ClearCurrentRoom();

			currentRoom =
				generator.Generate(
					definition,
					parent);

			if (currentRoom != null)
			{
				events.Publish(
					new RoomGeneratedEvent(currentRoom));
			}

			return currentRoom;
		}

		public void StartRoom()
		{
			if (currentRoom == null)
				return;

			currentRoom.StartRoom();
		}

		public void CompleteRoom()
		{
			if (currentRoom == null)
				return;

			currentRoom.Complete();
		}
		public void ClearCurrentRoom()
		{
			if (currentRoom == null)
				return;

			RoomInstance room =
				currentRoom;

			currentRoom = null;
			room.PrepareForDestruction();

			Object.Destroy(
				room.gameObject);

			events.Publish(
				new RoomClearedEvent(room));
		}
	}
}
