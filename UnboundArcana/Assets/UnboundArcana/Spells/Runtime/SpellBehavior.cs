namespace UnboundArcana.Spells.Runtime
{
	public interface IContinuousSpellBehavior
	{
	}

	public abstract class SpellBehavior
	{
		protected SpellInstance spell;

		public virtual void Initialize(SpellInstance spell)
		{
			this.spell = spell;
		}

		public abstract void Cast(CastContext context);

		public virtual void End()
		{
		}
		public virtual void UpdateCast(CastContext context)
		{
		}

		public virtual void Destroy()
		{
		}
	}
}
