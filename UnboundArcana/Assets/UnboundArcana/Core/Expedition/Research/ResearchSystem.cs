using UnityEngine;
using UnboundArcana.Core.Runtime;

namespace UnboundArcana.Core.Research
{
	public class ResearchGrantedEvent
	{
		public ResearchInstance research;
		public ResearchGrantedEvent( ResearchInstance research) { this.research = research; }
	}
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
				Debug.Log($"Check research status {research.Definition.DisplayName}: {research.Knowledge}/{research.Definition.RequiredKnowledge} | isCompleted? {research.IsCompleted}");
				if (!research.IsCompleted )
				{
					continue;
				}

				RunModifier modifier =
					research.CreateModifier();

				player.Modifiers.Add(modifier);

				research.Activate();
				GameRuntimeManager.Instance.Events.Publish(new ResearchGrantedEvent(research));
				Debug.Log(
					$"Activated research '{research.Definition.DisplayName}'");
			}
		}
	}
}