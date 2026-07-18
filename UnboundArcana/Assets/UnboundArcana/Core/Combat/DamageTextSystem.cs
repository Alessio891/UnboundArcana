using UnityEngine;
using UnboundArcana.Core.Entities.Events;
using UnboundArcana.Core.Events;

namespace UnboundArcana.Core.Combat
{
	public class DamageTextSystem
	{
		private readonly GameEventBus events;
		private readonly DamageTextView prefab;

		public DamageTextSystem(
			GameEventBus events,
			DamageTextView prefab)
		{
			this.events = events;
			this.prefab = prefab;

			events.Subscribe<EntityDamagedEvent>(
				OnEntityDamaged
			);
		}

		private void OnEntityDamaged(
			EntityDamagedEvent e)
		{
			DamageTextView instance =
				Object.Instantiate(
					prefab,
					e.Entity.transform.position,
					Quaternion.identity
				);

			instance.Initialize(
				e.Entity.transform.position,
				e.Damage.Amount
			);
		}
	}
}