using UnityEngine;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Behaviors;
using UnboundArcana.Spells.Behaviors.Aura;

namespace UnboundArcana.Spells.Modules.AuraDamage
{
	[CreateAssetMenu(menuName = "Spells/Modules/Aura Damage")]
	public class AuraDamageModuleDefinition : SpellModuleDefinition
	{
		public float damage = 1f;
		[Tooltip("Delay before the aura can deal its first damage tick.")]
		public float startupDelay = 0.15f;
		public float interval = 1f;

		public override bool SupportsBehavior(SpellBehaviorDefinition behavior)
		{
			return behavior is AuraBehaviorDefinition;
		}

		public override SpellModule CreateRuntime()
		{
			return new AuraDamageModule(this);
		}
	}
}
