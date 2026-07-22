using UnityEngine;

namespace UnboundArcana.Core.Rooms
{
	public abstract class RoomObjective : ScriptableObject
	{
		public abstract void StartObjective(
			RoomInstance room);

		public virtual void Tick(
			RoomInstance room)
		{
		}

		public virtual void StopObjective(
			RoomInstance room)
		{
		}
	}
}