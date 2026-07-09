using UnboundArcana.Spells.Runtime;

namespace UnboundArcana.Core.Events
{
	public class CastEvent : SpellEvent
	{
		public SpellInstance Spell { get; }

		public CastEvent(SpellInstance spell)
		{
			Spell = spell;
		}
	}
}