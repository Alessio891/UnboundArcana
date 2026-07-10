using UnityEngine;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Data;

namespace UnboundArcana.Spells.Modules.CastSpellOnDestroy
{
	[CreateAssetMenu(menuName = "Unbound Arcana/Modules/Cast Spell On Destroy")]
	public class CastSpellOnDestroyModuleDefinition : SpellModuleDefinition
	{
		public SpellDefinition spellToCast;

		public override SpellModule CreateRuntime()
		{
			return new CastSpellOnDestroyModule(this);
		}
	}
}