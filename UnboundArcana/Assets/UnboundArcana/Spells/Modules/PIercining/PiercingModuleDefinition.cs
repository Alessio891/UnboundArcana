using UnboundArcana.Spells.Runtime;
using UnityEngine;
using UnboundArcana.Spells.Behaviors;
using UnboundArcana.Spells.Behaviors.Projectile;

namespace UnboundArcana.Spells.Modules.Piercing
{
	[CreateAssetMenu(menuName = "Spells/Modules/Piercing")]
	public class PiercingModuleDefinition : SpellModuleDefinition
	{
		public int additionalHits = 1;

		public override bool SupportsBehavior(SpellBehaviorDefinition behavior)
		{
			return behavior is ProjectileBehaviorDefinition;
		}

		public override SpellModule CreateRuntime()
		{
			return new PiercingModule(this);
		}
	}
}
