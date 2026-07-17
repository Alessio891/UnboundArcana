using UnityEngine;
using UnboundArcana.Core.Events;

namespace UnboundArcana.Sandbox.UI
{
	public class PlaytestHUD : MonoBehaviour
	{
		public SpellTester SpellTester;

		public PlaytestSpellPanel SpellPanel;
		public PlaytestWavePanel WavePanel;
		public PlaytestRewardPanel RewardPanel;
		public PlaytestCombatPanel CombatPanel;
		private void Start()
		{
			SpellTester.RuntimeManager.GameEvents
				.Subscribe<WaveStartedEvent>(OnWaveStarted);

			SpellTester.RuntimeManager.GameEvents
				.Subscribe<EncounterCompletedEvent>(OnEncounterCompleted);

			SpellTester.RuntimeManager.GameEvents
				.Subscribe<RewardOfferedEvent>(OnRewardOffered);

			SpellTester.RuntimeManager.GameEvents
				.Subscribe<RewardSelectedEvent>(OnRewardSelected);

			SpellTester.RuntimeManager.GameEvents
				.Subscribe<DamageEvent>(OnDamage);


			SpellTester.RuntimeManager.GameEvents
				.Subscribe<SpellKillEvent>(OnEnemyKilled);

			RefreshSpell();
		}

		private void OnDestroy()
		{
			if (SpellTester == null)
			{
				return;
			}

			SpellTester.RuntimeManager.GameEvents
				.Unsubscribe<WaveStartedEvent>(OnWaveStarted);

			SpellTester.RuntimeManager.GameEvents
				.Unsubscribe<EncounterCompletedEvent>(OnEncounterCompleted);

			SpellTester.RuntimeManager.GameEvents
				.Unsubscribe<RewardOfferedEvent>(OnRewardOffered);

			SpellTester.RuntimeManager.GameEvents
				.Unsubscribe<RewardSelectedEvent>(OnRewardSelected);

			SpellTester.RuntimeManager.GameEvents
				.Unsubscribe<DamageEvent>(OnDamage);

			SpellTester.RuntimeManager.GameEvents
				.Unsubscribe<SpellKillEvent>(OnEnemyKilled);
		}
		private void OnDamage(
	DamageEvent eventData)
		{
			CombatPanel.AddDamage(
				eventData.Amount
			);

			CombatPanel.AddHit();
		}

		private void OnEnemyKilled(
			SpellKillEvent eventData)
		{
			CombatPanel.AddKill();
		}
		private void OnWaveStarted(
			WaveStartedEvent eventData)
		{
			WavePanel.SetWave(eventData.Wave);
			WavePanel.SetStatus("Combat");
			RefreshSpell();
		}

		private void OnEncounterCompleted(
			EncounterCompletedEvent eventData)
		{
			WavePanel.SetStatus("Reward");
		}

		private void OnRewardOffered(
			RewardOfferedEvent eventData)
		{
			RewardPanel.ShowRewards(
				eventData.Rewards
			);
		}

		private void OnRewardSelected(
			RewardSelectedEvent eventData)
		{
			RewardPanel.Hide();
			RefreshSpell();
		}

		private void RefreshSpell()
		{
			if (SpellTester.Configuration == null)
			{
				return;
			}

			SpellPanel.SetSpell(
				SpellTester.Configuration
			);
		}
	}
}