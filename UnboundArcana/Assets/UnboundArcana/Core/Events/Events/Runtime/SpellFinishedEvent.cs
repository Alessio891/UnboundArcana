using UnboundArcana.Spells.Runtime;

namespace UnboundArcana.Core.Events
{
	public class SpellFinishedEvent : SpellEvent
	{
		public SpellInstance Spell { get; }

		public SpellFinishedEvent(
			SpellInstance spell)
		{
			Spell = spell;
		}
	}
}