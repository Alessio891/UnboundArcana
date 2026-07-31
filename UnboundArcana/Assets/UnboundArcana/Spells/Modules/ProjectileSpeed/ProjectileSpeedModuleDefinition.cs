using UnityEngine;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Behaviors;
using UnboundArcana.Spells.Behaviors.Projectile;

namespace UnboundArcana.Spells.Modules.ProjectileSpeed
{
	[CreateAssetMenu(menuName = "Spells/Modules/Projectile Speed")]
	public class ProjectileSpeedModuleDefinition : SpellModuleDefinition
	{
		public float acceleration = 5f;

		public override bool SupportsBehavior(SpellBehaviorDefinition behavior)
		{
			return behavior is ProjectileBehaviorDefinition;
		}

		public override SpellModule CreateRuntime()
		{
			return new ProjectileSpeedModule(this);
		}
	}
}
