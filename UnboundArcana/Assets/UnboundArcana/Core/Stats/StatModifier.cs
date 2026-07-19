namespace UnboundArcana.Core.Stats
{
	public class StatModifier
	{
		public string Stat { get; }
		public float Value { get; }
		public ModifierOperation Operation { get; }
		public object Source { get; }

		public StatModifier(
			string stat,
			float value,
			ModifierOperation operation,
			object source)
		{
			Stat = stat;
			Value = value;
			Operation = operation;
			Source = source;
		}
	}
}