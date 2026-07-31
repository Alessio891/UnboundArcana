using UnityEngine;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Behaviors;
using UnboundArcana.Spells.Behaviors.Projectile;

namespace UnboundArcana.Spells.Modules.Orbit
{
	[CreateAssetMenu(menuName = "Spells/Modules/Orbit")]
	public class OrbitModuleDefinition : SpellModuleDefinition
	{
		[Tooltip("Radius in world units. One dungeon tile is 0.3 units.")]
		public float radius = 0.6f;
		public float angularSpeed = 180f;

		public override bool SupportsBehavior(SpellBehaviorDefinition behavior)
		{
			return behavior is ProjectileBehaviorDefinition;
		}

		public override SpellModule CreateRuntime()
		{
			return new OrbitModule(this);
		}
	}
}
