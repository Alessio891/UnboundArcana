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

		public bool TryActivate(out RunModifier modifier)
		{
			modifier = null;

			if (!IsCompleted || IsActivated)
				return false;

			modifier = new RunModifier(
				Definition.ModifierStat,
				Definition.ModifierValue,
				Definition.ModifierOperation,
				this);

			IsActivated = true;
			return true;
		}
	}
}
