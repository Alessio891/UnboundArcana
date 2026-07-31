using UnboundArcana.Spells.Runtime;
using UnityEngine;
using UnboundArcana.Spells.Behaviors;
using UnboundArcana.Spells.Behaviors.Projectile;

namespace UnboundArcana.Spells.Modules.Split
{
	[CreateAssetMenu(menuName = "Spells/Modules/Split On Destroy")]
	public class SplitOnDestroyModuleDefinition : SpellModuleDefinition
	{
		public int count = 2;

		[Range(0, 180)]
		public float spreadAngle = 45f;

		public override bool SupportsBehavior(SpellBehaviorDefinition behavior)
		{
			return behavior is ProjectileBehaviorDefinition;
		}

		public override SpellModule CreateRuntime()
		{
			return new SplitOnDestroyModule(this);
		}
	}
}
