using UnityEngine;
using UnboundArcana.Core.Events;

namespace UnboundArcana.Core.Rooms
{
	public abstract class EncounterInstance
	{
		protected readonly RoomInstance room;
		public RoomInstance Room => room;
		public bool IsCompleted { get; private set; }

		protected EncounterInstance(RoomInstance room)
		{
			this.room = room;
		}

		public virtual void Start() {
			GameRuntimeManager.Instance.Events.Publish(
				new EncounterStartEvent());
		}

		public virtual void Tick()
		{
		}

		public virtual void Stop()
		{
		}

		protected void Complete()
		{
			if (IsCompleted)
				return;

			IsCompleted = true;

			Debug.Log(
				$"Encounter completed in room {room.Definition.RoomId}");

			GameRuntimeManager.Instance.Events.Publish(
				new EncounterCompletedEvent(this));
		}
	}
}