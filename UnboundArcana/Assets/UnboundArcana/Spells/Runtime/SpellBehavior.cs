namespace UnboundArcana.Spells.Runtime
{
	public abstract class SpellBehavior
	{
		protected SpellInstance spell;

		public virtual void Initialize(SpellInstance spell)
		{
			this.spell = spell;
		}

		public abstract void Cast(CastContext context);
	}
}