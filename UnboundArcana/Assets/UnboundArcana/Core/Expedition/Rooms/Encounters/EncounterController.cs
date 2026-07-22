using System.Collections.Generic;
using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Entities.Events;
using UnityEngine;

namespace UnboundArcana.Core.Rooms
{
	public class EncounterController
	{
		private readonly RoomInstance room;

		private readonly List<Entity> spawnedEntities = new();

		private int aliveEntities;

		public EncounterController(
			RoomInstance room)
		{
			this.room = room;
		}

		public void Start(
			EncounterDefinition definition)
		{
			if (definition == null)
			{
				room.Complete();
				return;
			}

			foreach (var group in definition.Groups)
			{
				for (int i = 0; i < group.Count; i++)
				{
					Spawn(group.Entity);
				}
			}
		}

		private void Spawn(
			EntityDefinition definition)
		{
			Vector3 position =
				GetSpawnPosition();

			Entity entity =
				GameRuntimeManager.Instance.EntitySpawn.Spawn(
					definition,
					position,
					room.transform);

			if (entity == null)
				return;

			entity.Events.Subscribe<EntityDeathEvent>(
				OnEntityDeath);

			spawnedEntities.Add(entity);

			aliveEntities++;
		}

		private void OnEntityDeath(
			EntityDeathEvent evt)
		{
			if (!spawnedEntities.Contains(evt.Entity))
				return;

			evt.Entity.Events.Unsubscribe<EntityDeathEvent>(
				OnEntityDeath);

			aliveEntities--;

			if (aliveEntities <= 0)
			{
				room.Complete();
			}
		}

		private Vector3 GetSpawnPosition()
		{
			var markers =
				room.GetMarkers(
					RoomMarkerType.EnemySpawn);

			List<RoomMarker> available =
				new(markers);

			if (available.Count == 0)
			{
				return room.transform.position;
			}

			return available[
				Random.Range(
					0,
					available.Count)
			].transform.position;
		}
	}
}