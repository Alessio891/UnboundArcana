using System.Collections.Generic;
using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Entities.Events;
using UnityEngine;

namespace UnboundArcana.Core.Rooms
{

	[CreateAssetMenu(
		menuName = "Unbound Arcana/Rooms/Behaviours/Combat")]
	public class CombatRoomBehaviour : RoomBehaviour
	{
		[SerializeField]
		private List<EntityDefinition> enemies = new();

		[SerializeField]
		private int enemiesToSpawn = 5;

		private readonly Dictionary<RoomInstance, RoomEncounter> encounters = new();

		public override void StartRoom(
			RoomInstance room)
		{
			var encounter =
				new RoomEncounter(
					room,
					enemies,
					enemiesToSpawn);

			encounters[room] = encounter;

			encounter.Start();
		}

		public override void StopRoom(
			RoomInstance room)
		{
			if (encounters.TryGetValue(
				room,
				out var encounter))
			{
				encounter.Stop();

				encounters.Remove(room);
			}
		}

		public override void Tick(
			RoomInstance room)
		{
			if (encounters.TryGetValue(
				room,
				out var encounter))
			{
				encounter.Tick();
			}
		}
	}
}