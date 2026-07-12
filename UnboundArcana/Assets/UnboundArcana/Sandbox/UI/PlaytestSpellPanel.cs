using UnityEngine;
using UnityEngine.UI;
using UnboundArcana.Spells.Data;

namespace UnboundArcana.Sandbox.UI
{
	public class PlaytestSpellPanel : MonoBehaviour
	{
		public Text text;

		public void SetSpell(
			SpellConfiguration configuration)
		{
			if (configuration == null)
			{
				text.text = "";
				return;
			}

			string result = "Spell\n\n";

			result += "Behavior:\n";
			result += configuration.behavior.name;
			result += "\n\n";

			result += "Modules:\n";

			foreach (var module in configuration.modules)
			{
				result += "- ";
				result += module.name;
				result += "\n";
			}

			text.text = result;
		}
	}
}