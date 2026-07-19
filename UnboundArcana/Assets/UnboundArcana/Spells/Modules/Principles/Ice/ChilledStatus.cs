using UnboundArcana.Core.Stats;
using UnityEngine;

namespace UnboundArcana.Core.Entities.Statuses
{
	public class ChilledStatus : StatusInstance
	{
		float originalSpeed = 0.0f;
		public ChilledStatus(
			StatusDefinition definition)
			: base(definition)
		{
		}
		public override void AddStack()
		{
			base.AddStack();
			if (Stacks >= 5) {
				RemainingDuration = 0.0f;
				target.Status.Apply((Definition as ChilledStatusDefinition).frozenStatus, source);
			}
			
		}
		public override void Initialize(Entity target, Entity source)
		{
			base.Initialize(target, source);

			StatModifier modifier = new StatModifier(StatKeys.Entity.MoveSpeed, 0.7f, ModifierOperation.Multiplier, this);
			
			target.Stats.AddModifier(modifier);
		}
		public override bool CanApply(Entity target)
		{
			ChilledStatusDefinition defin = (ChilledStatusDefinition)Definition;
			bool canApplyChill = !target.Status.Has(defin.frozenStatus);
			Debug.Log($"Can apply chill {canApplyChill} (does not have {defin.frozenStatus.name} status)");
			return canApplyChill;
		}
		public override void OnRemove()
		{
			base.OnRemove();
			target.Stats.RemoveModifiersFromSource(this);
		}
	}
}