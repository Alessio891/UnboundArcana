using System.Collections.Generic;
using UnboundArcana.Core.Entities.Events;
using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Rooms;
using UnityEngine;

public class RoomEncounter
{
	private readonly RoomInstance room;
	private readonly List<EntityDefinition> enemyDefinitions;
	private readonly List<Entity> spawned = new();

	private readonly int amount;

	public RoomEncounter(
		RoomInstance room,
		List<EntityDefinition> enemies,
		int amount)
	{
		this.room = room;
		enemyDefinitions = enemies;
		this.amount = amount;
	}

	public void Start()
	{
		foreach (var marker in room.GetMarkers(
			RoomMarkerType.EnemySpawn))
		{
			for (int i = 0; i < amount; i++)
			{
				SpawnEnemy(
					marker.transform.position);
			}
		}
	}

	private void SpawnEnemy(
		Vector3 position)
	{
		if (enemyDefinitions.Count == 0)
			return;

		EntityDefinition definition =
			enemyDefinitions[
				Random.Range(
					0,
					enemyDefinitions.Count)];

		GameObject instance =
			Object.Instantiate(
				definition.Prefab,
				position,
				Quaternion.identity,
				room.transform);

		Entity entity =
			instance.GetComponent<Entity>();

		if (entity == null)
			return;

		entity.Events.Subscribe<EntityDeathEvent>(
			OnEnemyDeath);

		spawned.Add(entity);
	}

	private void OnEnemyDeath(
		EntityDeathEvent evt)
	{
		spawned.Remove(
			evt.Entity);

		if (spawned.Count == 0)
		{
			room.Complete();
		}
	}

	public void Tick()
	{
	}

	public void Stop()
	{
		foreach (var entity in spawned)
		{
			if (entity != null)
				Object.Destroy(entity.gameObject);
		}

		spawned.Clear();
	}
}