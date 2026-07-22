using UnboundArcana.Core.Entities;
using UnityEngine;

namespace UnboundArcana.Core.Runtime
{
	public class EntitySpawnService
	{
		public Entity Spawn(
			EntityDefinition definition,
			Vector3 position,
			Transform parent = null)
		{
			if (definition == null)
			{
				Debug.LogError(
					"Cannot spawn entity. Definition is null.");

				return null;
			}

			if (definition.Prefab == null)
			{
				Debug.LogError(
					$"Entity definition {definition.name} has no prefab.");

				return null;
			}

			GameObject instance =
				Object.Instantiate(
					definition.Prefab,
					position,
					Quaternion.identity,
					parent);

			Entity entity =
				instance.GetComponent<Entity>();

			if (entity == null)
			{
				Debug.LogError(
					$"Prefab {definition.Prefab.name} has no Entity component.");

				Object.Destroy(instance);

				return null;
			}

			return entity;
		}
	}
}