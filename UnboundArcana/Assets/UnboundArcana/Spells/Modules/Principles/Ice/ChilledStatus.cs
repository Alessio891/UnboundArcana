using UnboundArcana.Core.Stats;
using UnityEngine;

namespace UnboundArcana.Core.Entities.Statuses
{
	public class ChilledStatus : StatusInstance
	{
		public ChilledStatus(
			StatusDefinition definition)
			: base(definition)
		{
		}
		public override void AddStack()
		{
			base.AddStack();
			ApplySlow();

			ChilledStatusDefinition definition = Definition as ChilledStatusDefinition;

			if (definition.frozenStatus != null && Stacks >= definition.stacksToFreeze)
			{
				RemainingDuration = 0.0f;
				target.Status.Apply(definition.frozenStatus, source);
			}
		}
		public override void Initialize(Entity target, Entity source)
		{
			base.Initialize(target, source);

			ApplySlow();
		}
		public override bool CanApply(Entity target)
		{
			ChilledStatusDefinition definition = (ChilledStatusDefinition)Definition;
			return definition.frozenStatus == null || !target.Status.Has(definition.frozenStatus);
		}
		public override void OnRemove()
		{
			base.OnRemove();
			target.Stats.RemoveModifiersFromSource(this);
		}

		private void ApplySlow()
		{
			ChilledStatusDefinition definition = (ChilledStatusDefinition)Definition;
			float multiplier = Mathf.Max(0.1f, 1f - definition.slowPerStack * Stacks);
			target.Stats.RemoveModifiersFromSource(this);
			target.Stats.AddModifier(new StatModifier(StatKeys.Entity.MoveSpeed, multiplier, ModifierOperation.Multiplier, this));
		}
	}
}
