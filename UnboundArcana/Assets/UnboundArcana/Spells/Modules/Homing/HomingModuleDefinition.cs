using UnboundArcana.Spells.Runtime;
using UnityEngine;
using UnboundArcana.Spells.Behaviors;
using UnboundArcana.Spells.Behaviors.Projectile;

namespace UnboundArcana.Spells.Modules.Homing
{
	[CreateAssetMenu(menuName = "Spells/Modules/Homing")]
	public class HomingModuleDefinition : SpellModuleDefinition
	{
		public float strength = 5f;

		public override bool SupportsBehavior(SpellBehaviorDefinition behavior)
		{
			return behavior is ProjectileBehaviorDefinition;
		}

		public override SpellModule CreateRuntime()
		{
			return new HomingModule(this);
		}
	}
}
