using UnityEngine;
using System.Collections.Generic;
using UnboundArcana.Core.Stats;
using UnboundArcana.Spells.Modules.SizeModifier;
using UnboundArcana.Spells.Runtime;

namespace UnboundArcana.Spells.Modules.Modifiers
{
	public class SizeModifierModule : SpellModule, ISpellModifierProvider
	{
		private readonly SizeModifierModuleDefinition definition;

		public SizeModifierModule(
			SizeModifierModuleDefinition definition)
		{
			this.definition = definition;
		}

		public override void Initialize(SpellInstance spell)
		{
			base.Initialize(spell);

			
		}
		public IEnumerable<StatModifier> GetModifiers()
		{
			yield return new StatModifier(
				StatKeys.Spell.Size,
				definition.percentage,
				ModifierOperation.Percent,
				this
			);
		}
		
	}
}