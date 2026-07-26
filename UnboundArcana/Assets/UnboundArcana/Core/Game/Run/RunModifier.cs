namespace UnboundArcana.Core.Runtime
{
	public enum RunModifierStat
	{
		SpellDamage,
		SpellSpeed,
		SpellSize,
		SpellDuration,
		KnowledgeGain
	}

	public enum RunModifierOperation
	{
		Flat,
		Percent
	}

	public class RunModifier
	{
		public RunModifierStat Stat { get; }
		public float Value { get; }
		public RunModifierOperation Operation { get; }
		public object Source { get; }

		public RunModifier(
			RunModifierStat stat,
			float value,
			RunModifierOperation operation,
			object source)
		{
			Stat = stat;
			Value = value;
			Operation = operation;
			Source = source;
		}
	}
}