using UnityEngine;
using UnboundArcana.Spells.Runtime;

namespace UnboundArcana.Spells.Modules.AuraDamage
{
	[CreateAssetMenu(menuName = "Spells/Modules/Aura Damage")]
	public class AuraDamageModuleDefinition : SpellModuleDefinition
	{
		public float damage = 1f;
		public float interval = 1f;

		public override SpellModule CreateRuntime()
		{
			return new AuraDamageModule(this);
		}
	}
}