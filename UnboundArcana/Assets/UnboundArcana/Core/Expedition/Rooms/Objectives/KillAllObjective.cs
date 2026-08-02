using UnityEngine;
using UnboundArcana.Core.Events;

namespace UnboundArcana.Core.Rooms
{
	[CreateAssetMenu(
		menuName = "Unbound Arcana/Rooms/Objectives/Kill All")]
	public class KillAllObjective : RoomObjective
	{
		private RoomInstance room;

		public override void StartObjective(
			RoomInstance room)
		{
			this.room = room;

			Debug.Log(
				$"Starting KillAll objective in room {room.Definition.RoomId}");

			GameRuntimeManager.Instance.Events.Subscribe<EncounterCompletedEvent>(
				OnEncounterCompleted);
		}

		private void OnEncounterCompleted(
			EncounterCompletedEvent evt)
		{
			if (evt.Encounter.Room != room)
				return;

			Debug.Log(
				$"KillAll objective completed in room {room.Definition.RoomId}");

			room.Complete();
		}

		public override void StopObjective(
			RoomInstance room)
		{
			Debug.Log(
				$"Stopping KillAll objective in room {room.Definition.RoomId}");

			GameRuntimeManager.Instance.Events.Unsubscribe<EncounterCompletedEvent>(
				OnEncounterCompleted);

			if (this.room == room)
				this.room = null;
		}
	}
}
