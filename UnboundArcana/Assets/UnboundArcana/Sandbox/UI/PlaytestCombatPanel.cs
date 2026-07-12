using UnityEngine;
using UnityEngine.UI;

namespace UnboundArcana.Sandbox.UI
{
	public class PlaytestCombatPanel : MonoBehaviour
	{
		public Text text;

		private int hits;
		private int kills;
		private float damage;

		public void AddHit()
		{
			hits++;
			Refresh();
		}

		public void AddDamage(float amount)
		{
			damage += amount;
			Refresh();
		}

		public void AddKill()
		{
			kills++;
			Refresh();
		}

		public void ResetStats()
		{
			hits = 0;
			kills = 0;
			damage = 0;

			Refresh();
		}

		private void Refresh()
		{
			text.text =
				"Combat\n\n" +
				"Hits: " + hits +
				"\n" +
				"Damage: " + damage +
				"\n" +
				"Kills: " + kills;
		}
	}
}