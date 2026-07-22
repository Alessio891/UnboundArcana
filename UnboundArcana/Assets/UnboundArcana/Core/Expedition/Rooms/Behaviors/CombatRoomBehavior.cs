using System.Collections.Generic;
using UnityEngine;

namespace UnboundArcana.Core.Rooms
{
	[CreateAssetMenu(
		menuName = "Unbound Arcana/Rooms/Behaviours/Combat")]
	public class CombatRoomBehaviour : RoomBehaviour
	{
		private readonly Dictionary<RoomInstance, EncounterInstance> encounters = new();

		public override void StartRoom(
			RoomInstance room)
		{
			EncounterInstance encounter =
				new CombatEncounterInstance(
					room,
					room.Definition.Encounter);

			encounters[room] = encounter;

			encounter.Start();

			room.StartObjective();
		}

		public override void StopRoom(
			RoomInstance room)
		{
			if (encounters.TryGetValue(
				room,
				out EncounterInstance encounter))
			{
				encounter.Stop();

				encounters.Remove(room);
			}

			room.StopObjective();
		}

		public override void Tick(
			RoomInstance room)
		{
			if (encounters.TryGetValue(
				room,
				out EncounterInstance encounter))
			{
				encounter.Tick();
			}

			room.TickObjective();
		}
	}
}