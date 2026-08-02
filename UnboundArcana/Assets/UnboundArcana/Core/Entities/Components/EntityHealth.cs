using UnityEngine;
using UnboundArcana.Core.Combat;
using UnboundArcana.Core.Entities.Events;
using UnboundArcana.Core.Stats;

namespace UnboundArcana.Core.Entities
{
	public class EntityHealth : MonoBehaviour, IDamageable
	{
		private Entity entity;

		public float currentHealth { get; private set; }

		private void Awake()
		{
			entity = GetComponent<Entity>();
		}

		private void Start()
		{
			currentHealth =
				entity.Stats.Get(
					StatKeys.Entity.MaxHealth
				);
			PublishHealthChanged();
		}

		public void TakeDamage(
			DamageInfo damage)
		{
			if (currentHealth <= 0) return;

			float amount = damage.Amount;

			if (damage.Source != null && damage.Source.TryGetComponent<Entity>(out Entity sourceEntity) && sourceEntity != entity)
				amount *= entity.Stats.Get(StatKeys.Entity.DamageTakenFromEnemies);

			DamageInfo appliedDamage = new(damage.Source, amount, damage.Type);
			currentHealth -= amount;
			entity.Events.Publish(
				new EntityDamagedEvent(
					entity,
					appliedDamage)
			);
			PublishHealthChanged();
			
			if (currentHealth <= 0)
			{
				entity.Events.Publish(
					new EntityDeathEvent(
						entity)
				);
			}
		}

		public void RestoreHealth(float amount)
		{
			if (amount <= 0f || currentHealth <= 0f)
				return;

			float previousHealth = currentHealth;
			currentHealth = Mathf.Min(currentHealth + amount, entity.Stats.Get(StatKeys.Entity.MaxHealth));

			if (!Mathf.Approximately(currentHealth, previousHealth))
				PublishHealthChanged();
		}

		private void PublishHealthChanged()
		{
			entity.Events.Publish(new EntityHealthChangedEvent(entity));
		}
	}
}
