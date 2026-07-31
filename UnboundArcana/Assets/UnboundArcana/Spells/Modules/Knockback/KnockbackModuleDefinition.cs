using UnboundArcana.Spells.Runtime;
using UnityEngine;

namespace UnboundArcana.Spells.Modules.Knockback
{
	[CreateAssetMenu(menuName = "Spells/Modules/Knockback")]
	public class KnockbackModuleDefinition : SpellModuleDefinition
	{
		public float force = 3f;

		public override SpellModule CreateRuntime()
		{
			return new KnockbackModule(this);
		}
	}
}
