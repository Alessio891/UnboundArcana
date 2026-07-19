using UnboundArcana.Core.Stats;

namespace UnboundArcana.Core.Entities.Statuses
{
	public class FrozenStatus : StatusInstance
	{
		public FrozenStatus(
			StatusDefinition definition)
			: base(definition)
		{
		}

		public override void Initialize(Entity target, Entity source)
		{
			base.Initialize(target, source);
			target.Stats.AddModifier(new Stats.StatModifier(StatKeys.Entity.MoveSpeed, 0.0f, ModifierOperation.Multiplier, this));
		}
		public override void OnRemove()
		{
			base.OnRemove();
			target.Stats.RemoveModifiersFromSource(this);
		}
	}
}