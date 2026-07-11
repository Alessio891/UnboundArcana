using UnboundArcana.Spells.Modules.Fire;
using UnboundArcana.Spells.Modules.Modifiers;
using UnboundArcana.Spells.Runtime;
using UnityEngine;

namespace UnboundArcana.Spells.Modules.SizeModifier
{
	[CreateAssetMenu(menuName = "Spells/Modules/Size Modifier")]
	public class SizeModifierModuleDefinition : SpellModuleDefinition
	{
		public float percentage = 0.5f;

		public override SpellModule CreateRuntime()
		{
			return new SizeModifierModule(this);
		}
	}
}