using UnboundArcana.Core.Events;

namespace UnboundArcana.Core.Combat
{
	public class DamageSystem
	{
		public void Initialize(GameEventBus events)
		{
			events.Subscribe<DamageEvent>(OnDamage);
		}

		private void OnDamage(DamageEvent damageEvent)
		{
			if (!damageEvent.Target.TryGetComponent<IDamageable>(out var damageable))
			{
				return;
			}

			damageable.TakeDamage(
				new DamageInfo(
					damageEvent.Source,
					damageEvent.Amount,
					damageEvent.Type
				)
			);
		}
	}
}