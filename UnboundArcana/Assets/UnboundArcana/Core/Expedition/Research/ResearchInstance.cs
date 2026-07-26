using UnboundArcana.Core.Runtime;
using UnityEngine;

namespace UnboundArcana.Core.Research
{
	public class ResearchInstance
	{
		public ResearchDefinition Definition { get; }

		public int Knowledge { get; private set; }

		public bool IsCompleted =>
			Knowledge >= Definition.RequiredKnowledge;

		public bool IsActivated { get; private set; }

		public ResearchInstance(
			ResearchDefinition definition)
		{
			Definition = definition;
		}

		public void AddKnowledge(int amount)
		{
			if (IsCompleted)
				return;

			Knowledge += amount;

			if (Knowledge > Definition.RequiredKnowledge)
			{
				Knowledge = Definition.RequiredKnowledge;
			}

			Debug.Log(
				$"Research '{Definition.DisplayName}': " +
				$"{Knowledge}/{Definition.RequiredKnowledge}");

			if (IsCompleted)
			{
				Debug.Log(
					$"Research '{Definition.DisplayName}' completed. " +
					"Waiting for next room to activate.");
			}
		}

		public RunModifier CreateModifier()
		{
			return new RunModifier(
				Definition.ModifierStat,
				Definition.ModifierValue,
				Definition.ModifierOperation,
				this);
		}

		public void Activate()
		{
			if (!IsCompleted || IsActivated)
				return;

			IsActivated = true;
		}
	}
}