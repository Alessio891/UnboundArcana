using UnboundArcana.Spells.Modules.Chain;
using UnboundArcana.Spells.Modules;
using UnboundArcana.Spells.Runtime;
using UnityEngine;
using UnboundArcana.Spells.Behaviors;
using UnboundArcana.Spells.Behaviors.Projectile;
[CreateAssetMenu(menuName = "Spells/Modules/Chain")]
	public class ChainModuleDefinition : SpellModuleDefinition
	{
		[Tooltip("Range in world units. One dungeon tile is 0.3 units.")]
		public float range = 1.5f;
		public int maxChains = 3;

		public override bool SupportsBehavior(SpellBehaviorDefinition behavior)
		{
			return behavior is ProjectileBehaviorDefinition;
		}

	public override SpellModule CreateRuntime()
	{
		return new ChainModule(this);
	}
}
