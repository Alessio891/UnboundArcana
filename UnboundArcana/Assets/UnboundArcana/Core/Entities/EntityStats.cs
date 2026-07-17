using System.Collections.Generic;

namespace UnboundArcana.Core.Entities
{
	public enum EntityStatId
	{
		MaxHealth,
		MoveSpeed,
		CastSpeed,
		Armor
	}

	public class EntityStats
	{
		private readonly Dictionary<EntityStatId, float> values = new();

		public void Set(
			EntityStatId stat,
			float value)
		{
			values[stat] = value;
		}

		public float Get(
			EntityStatId stat)
		{
			if (values.TryGetValue(stat, out float value))
			{
				return value;
			}

			return 0f;
		}
	}
}