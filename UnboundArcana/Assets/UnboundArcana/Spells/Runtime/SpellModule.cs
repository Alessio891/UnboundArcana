using UnboundArcana.Core.Events;
using UnboundArcana.Core.Stats;

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

		public virtual void ApplyStats(
			StatCollection stats)
		{
		}

		public virtual void Destroy()
		{
		}
	}
}