using System.Collections.Generic;

namespace UnboundArcana.Core.Stats
{
	public class StatCollection
	{
		private readonly Dictionary<string, List<StatModifier>> baseValues = new();
		private readonly List<StatModifier> modifiers = new();

		public void AddBase(
			string stat,
			float value,
			object source)
		{
			if (!baseValues.TryGetValue(
				stat,
				out List<StatModifier> values))
			{
				values = new List<StatModifier>();
				baseValues.Add(stat, values);
			}

			values.Add(
				new StatModifier(
					stat,
					value,
					ModifierOperation.Flat,
					source
				)
			);
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

			foreach (List<StatModifier> values in baseValues.Values)
			{
				values.RemoveAll(
					x => x.Source == source
				);
			}
		}

		public float Get(
			string stat)
		{
			float value = 0;

			if (baseValues.TryGetValue(
				stat,
				out List<StatModifier> values))
			{
				foreach (StatModifier modifier in values)
				{
					value += modifier.Value;
				}
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