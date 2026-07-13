using UnityEngine;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Data;

namespace UnboundArcana.Spells.Modules.CastSpellOnHit
{
	[CreateAssetMenu(menuName = "Spells/Modules/Cast Spell On Hit")]
	public class CastSpellOnHitModuleDefinition : SpellModuleDefinition
	{
		public SpellDefinition spellToCast;

		public override SpellModule CreateRuntime()
		{
			return new CastSpellOnHitModule(this);
		}
	}
}