using UnboundArcana.Core.Events;

namespace UnboundArcana.Spells.Runtime
{
	public abstract class SpellModule
	{
		protected SpellInstance spell;
		protected SpellEventBus Events => spell.Events;

		public virtual void Initialize(SpellInstance spell)
		{
			this.spell = spell;
		}
	}
}