using UnboundArcana.Core.Combat;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Stats;

namespace UnboundArcana.Core.Entities.Statuses
{
	public class BurningStatus : StatusInstance
	{
		float damageInterval = 1.0f;
		float damage = 1.0f;
		float timer = 0.0f;
		public BurningStatus(
			BurningStatusDefinition definition)
			: base(definition)
		{
			damageInterval = definition.secondsBetweenTicks;
			damage = definition.magnitude;
			timer = damageInterval;
		}

		public override void Tick(float deltaTime)
		{
			base.Tick(deltaTime);
			if (IsExpired) return;
			timer -= deltaTime;
			if (timer <= 0)
			{
				GameRuntimeManager.Instance.Events.Publish(
						new DamageEvent(
							source.gameObject,
							target,
							damage,
							DamageType.Fire
						)
					);
				timer = damageInterval;
			}
		}
	}
}
