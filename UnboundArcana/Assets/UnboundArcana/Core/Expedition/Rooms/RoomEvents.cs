namespace UnboundArcana.Core.Rooms
{
	public class RoomGeneratedEvent
	{
		public RoomInstance Room;

		public RoomGeneratedEvent(RoomInstance room)
		{
			Room = room;
		}
	}

	public class RoomStartedEvent
	{
		public RoomInstance Room;

		public RoomStartedEvent(RoomInstance room)
		{
			Room = room;
		}
	}

	public class RoomCompletedEvent
	{
		public RoomInstance Room;

		public RoomCompletedEvent(RoomInstance room)
		{
			Room = room;
		}
	}

	public class RoomClearedEvent
	{
		public RoomInstance Room;

		public RoomClearedEvent(RoomInstance room)
		{
			Room = room;
		}
	}
}