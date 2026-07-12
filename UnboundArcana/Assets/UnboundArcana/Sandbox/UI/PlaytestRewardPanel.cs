using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnboundArcana.Spells.Modules;

namespace UnboundArcana.Sandbox.UI
{
	public class PlaytestRewardPanel : MonoBehaviour
	{
		public Text text;

		public void ShowRewards(
			IReadOnlyList<SpellModuleDefinition> rewards)
		{
			string result = "Choose Reward\n\n";

			for (int i = 0; i < rewards.Count; i++)
			{
				result +=
					(i + 1) +
					": " +
					rewards[i].name +
					"\n";
			}

			text.text = result;

			gameObject.SetActive(true);
		}

		public void Hide()
		{
			gameObject.SetActive(false);
		}
	}
}