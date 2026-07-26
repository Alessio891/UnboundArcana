using UnityEngine;
using UnboundArcana.Core.Entities;

namespace UnboundArcana.Core.Runtime
{
	public class PlayerSpawner
	{
		public Entity Spawn(
			PlayerState state,
			Vector3 position,
			Transform parent)
		{
			if (state == null ||
				state.Definition == null)
			{
				Debug.LogError(
					"Cannot spawn player. Missing player state.");

				return null;
			}

			GameObject instance =
				Object.Instantiate(
					state.Definition.Prefab,
					position,
					Quaternion.identity,
					parent);

			Entity entity =
				instance.GetComponent<Entity>();

			if (entity == null)
			{
				Debug.LogError(
					"Player prefab has no Entity component.");

				Object.Destroy(instance);

				return null;
			}
			GameRuntimeManager.Instance.Events.Publish(new PlayerSpawnedEvent(entity));
			return entity;
		}
	}
}