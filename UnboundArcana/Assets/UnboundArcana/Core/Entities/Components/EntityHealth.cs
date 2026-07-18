using UnityEngine;
using UnboundArcana.Core.Combat;
using UnboundArcana.Core.Entities.Events;

namespace UnboundArcana.Core.Entities
{
	public class EntityHealth : MonoBehaviour, IDamageable
	{
		private Entity entity;

		private float currentHealth;

		private void Awake()
		{
			entity = GetComponent<Entity>();
		}

		private void Start()
		{
			currentHealth =
				entity.Stats.Get(
					EntityStatId.MaxHealth
				);
		}

		public void TakeDamage(
			DamageInfo damage)
		{
			currentHealth -= damage.Amount;
			Debug.Log("[Health] Publishing entity damage event");
			entity.Events.Publish(
				new EntityDamagedEvent(
					entity,
					damage)
			);
			
			if (currentHealth <= 0)
			{
				entity.Events.Publish(
					new EntityDeathEvent(
						entity)
				);
			}
		}
	}
}