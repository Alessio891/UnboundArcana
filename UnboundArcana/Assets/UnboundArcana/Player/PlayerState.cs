using System.Collections.Generic;
using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Research;
using UnboundArcana.Spells.Data;

namespace UnboundArcana.Core.Runtime
{
	public class KnowledgeGainedEvent
	{
		int gained = 0;
		public KnowledgeGainedEvent(int gained) { this.gained = gained; }
	}

	public class PlayerState
	{
		public EntityDefinition Definition { get; }

		public List<SpellConfiguration> Spells { get; } = new();

		public List<ResearchInstance> Researches { get; } = new();

		public List<RunModifier> Modifiers { get; } = new();

		public int Knowledge { get; private set; }

		public PlayerState(
			EntityDefinition definition)
		{
			Definition = definition;
		}
		public void AddKnowledge(int amount)
		{
			float modifiedAmount = amount;

			foreach (RunModifier modifier in Modifiers)
			{
				if (modifier.Stat != RunModifierStat.KnowledgeGain)
					continue;

				modifiedAmount = modifier.Operation == RunModifierOperation.Flat ? modifiedAmount + modifier.Value : modifiedAmount * (1f + modifier.Value);
			}

			int gained = UnityEngine.Mathf.RoundToInt(modifiedAmount);
			Knowledge += gained;
			GameRuntimeManager.Instance.Events.Publish(new KnowledgeGainedEvent(gained));
		}
		public RunModifier AddMinorReward(
			ResearchDefinition definition)
		{
			if (definition == null)
				return null;

			RunModifier modifier = new(
				definition.ModifierStat,
				definition.ModifierValue,
				definition.ModifierOperation,
				definition);
			Modifiers.Add(modifier);
			return modifier;
		}
		public void AddResearch(
			ResearchDefinition definition)
		{
			if (definition == null)
				return;

			Researches.Add(
				new ResearchInstance(definition));
		}

		public bool TryActivateResearch(ResearchInstance research)
		{
			if (research == null || !Researches.Contains(research) || !research.TryActivate(out RunModifier modifier))
				return false;

			Modifiers.Add(modifier);
			return true;
		}

		public void ClearExpeditionProgress()
		{
			Knowledge = 0;
			Researches.Clear();
			Modifiers.Clear();
		}
	}
}
