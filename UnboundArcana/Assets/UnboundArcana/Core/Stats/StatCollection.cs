using System.Collections.Generic;

namespace UnboundArcana.Core.Stats
{
	public class StatCollection
	{
		private readonly Dictionary<StatId, float> baseValues = new();
		private readonly List<StatModifier> modifiers = new();

		public void SetBase(
			StatId stat,
			float value)
		{
			baseValues[stat] = value;
		}

		public void AddModifier(
			StatModifier modifier)
		{
			modifiers.Add(modifier);
		}

		public void RemoveModifiersFromSource(
			object source)
		{
			modifiers.RemoveAll(
				x => x.Source == source
			);
		}

		public float Get(
			StatId stat)
		{
			float value = 0;

			if (baseValues.TryGetValue(stat, out float baseValue))
			{
				value = baseValue;
			}

			foreach (StatModifier modifier in modifiers)
			{
				if (modifier.Stat != stat)
				{
					continue;
				}

				switch (modifier.Operation)
				{
					case ModifierOperation.Flat:
						value += modifier.Value;
						break;

					case ModifierOperation.Percent:
						value *= 1 + modifier.Value;
						break;

					case ModifierOperation.Multiplier:
						value *= modifier.Value;
						break;
				}
			}

			return value;
		}
	}
}