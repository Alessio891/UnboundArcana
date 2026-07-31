using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Entities.Statuses;

namespace UnboundArcana.Spells.Modules.Principles
{
	public class DrenchedStatus : StatusInstance
	{
		private CharacterMotor motor;

		public DrenchedStatus(DrenchedStatusDefinition definition) : base(definition)
		{
		}

		public override void Initialize(Entity target, Entity source)
		{
			base.Initialize(target, source);
			motor = target.GetComponent<CharacterMotor>();
			ApplySlow();
		}

		public override void AddStack()
		{
			base.AddStack();
			ApplySlow();
		}

		private void ApplySlow()
		{
			if (motor == null) { return; }

			DrenchedStatusDefinition definition = (DrenchedStatusDefinition)Definition;
			motor.SetSpeedMultiplier(1f - definition.slow * Stacks);
		}

		public override void OnRemove()
		{
			if (motor != null) { motor.SetSpeedMultiplier(1f); }
		}
	}
}
