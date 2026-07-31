using UnboundArcana.Core.Combat;
using UnboundArcana.Core.Entities.Statuses;
using UnboundArcana.Core.Events;

namespace UnboundArcana.Spells.Modules.Principles
{
	public class CorrodedStatus : StatusInstance
	{
		private float timer;

		public CorrodedStatus(CorrodedStatusDefinition definition) : base(definition)
		{
			timer = definition.damageInterval;
		}

		public override void Tick(float deltaTime)
		{
			base.Tick(deltaTime);
			if (IsExpired) { return; }

			timer -= deltaTime;
			if (timer > 0f) { return; }

			CorrodedStatusDefinition definition = (CorrodedStatusDefinition)Definition;
			timer += definition.damageInterval;
			GameRuntimeManager.Instance.Events.Publish(new DamageEvent(source.gameObject, target, definition.damagePerStack * Stacks, DamageType.Acid));
		}
	}
}
