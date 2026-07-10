using UnboundArcana.Spells.Runtime;

namespace UnboundArcana.Core.Events
{
	public class CastEvent : SpellEvent
	{
		public SpellInstance Spell { get; }
		public CastContext Context { get; }

		public CastEvent(
			SpellInstance spell,
			CastContext context)
		{
			Spell = spell;
			Context = context;
		}
	}
}