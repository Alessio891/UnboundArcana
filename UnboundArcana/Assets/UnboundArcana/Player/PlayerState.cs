using System.Collections.Generic;
using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Research;
using UnboundArcana.Spells.Data;

namespace UnboundArcana.Core.Runtime
{
	public class PlayerState
	{
		public EntityDefinition Definition { get; }

		public List<SpellConfiguration> Spells { get; } = new();

		public List<ResearchInstance> Researches { get; } = new();

		public List<RunModifier> Modifiers { get; } = new();

		public PlayerState(
			EntityDefinition definition)
		{
			Definition = definition;
		}
		public void AddKnowledge(int amount)
		{
			foreach (ResearchInstance research in Researches)
			{
				research.AddKnowledge(amount);
			}
		}
		public void AddResearch(
			ResearchDefinition definition)
		{
			if (definition == null)
				return;

			Researches.Add(
				new ResearchInstance(definition));
		}
	}
}