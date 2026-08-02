using System.Collections.Generic;
using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Entities.Events;
using UnityEngine;

namespace UnboundArcana.Core.Rooms
{
	public class CombatEncounterInstance : EncounterInstance
	{
		private readonly EncounterDefinition definition;

		private readonly List<Entity> spawnedEntities = new();

		public CombatEncounterInstance(
			RoomInstance room,
			EncounterDefinition definition)
			: base(room)
		{
			this.definition = definition;
		}

		public override void Start()
		{
			base.Start();
			Debug.Log(
		$"Starting combat encounter in room {room.Definition.RoomId}");

			if (definition == null)
			{
				Complete();
				return;
			}

			foreach (var group in definition.Groups)
			{
				for (int i = 0; i < group.Count; i++)
				{
					Spawn(group.Entity);
				}
			}
			Debug.Log(
	$"Combat encounter spawned {spawnedEntities.Count} enemies");
			if (spawnedEntities.Count == 0)
			{
				Complete();
			}
		}

		private void Spawn(EntityDefinition definition)
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
		}

		private void OnEntityDeath(
			EntityDeathEvent evt)
		{
			if (!spawnedEntities.Contains(evt.Entity))
				return;

			evt.Entity.Events.Unsubscribe<EntityDeathEvent>(
				OnEntityDeath);

			spawnedEntities.Remove(evt.Entity);

			if (spawnedEntities.Count == 0)
			{
				Complete();
			}
		}

		private Vector3 GetSpawnPosition()
		{
			List<RoomMarker> markers =
				new(room.GetMarkers(
					RoomMarkerType.EnemySpawn));

			if (markers.Count == 0)
				return room.transform.position;

			return markers[
				Random.Range(
					0,
					markers.Count)]
				.transform.position;
		}

		public override void Stop()
		{
			foreach (Entity entity in spawnedEntities)
			{
				if (entity != null)
				{
					entity.Events.Unsubscribe<EntityDeathEvent>(OnEntityDeath);
					Object.Destroy(entity.gameObject);
				}
			}

			spawnedEntities.Clear();
		}
	}
}
