using UnityEngine;

namespace UnboundArcana.Core.Rooms
{
	public abstract class RoomBehaviour : ScriptableObject
	{
		public abstract void StartRoom(RoomInstance room);

		public virtual void Tick(RoomInstance room)
		{
		}

		public virtual void StopRoom(RoomInstance room)
		{
		}
	}
}