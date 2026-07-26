using UnityEngine;
using UnboundArcana.Core.Runtime;

namespace UnboundArcana.Core.Research
{
	public class ResearchSystem
	{
		public void ActivateCompletedResearches(
			PlayerState player)
		{
			if (player == null)
				return;

			Debug.Log("Activating completed research");
			foreach (ResearchInstance research in player.Researches)
			{
				if (!research.IsCompleted ||
					research.IsActivated)
				{
					continue;
				}

				RunModifier modifier =
					research.CreateModifier();

				player.Modifiers.Add(modifier);

				research.Activate();

				Debug.Log(
					$"Activated research '{research.Definition.DisplayName}'");
			}
		}
	}
}