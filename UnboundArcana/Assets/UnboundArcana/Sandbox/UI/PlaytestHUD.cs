using UnityEngine;
using UnboundArcana.Core.Events;

namespace UnboundArcana.Sandbox.UI
{
	public class PlaytestHUD : MonoBehaviour
	{

		public PlaytestSpellPanel SpellPanel;
		public PlaytestWavePanel WavePanel;
		public PlaytestRewardPanel RewardPanel;
		public PlaytestCombatPanel CombatPanel;
		private void Start()
		{
			GameRuntimeManager.Instance.Events
				.Subscribe<WaveStartedEvent>(OnWaveStarted);

			GameRuntimeManager.Instance.Events
				.Subscribe<EncounterCompletedEvent>(OnEncounterCompleted);

			GameRuntimeManager.Instance.Events
				.Subscribe<RewardOfferedEvent>(OnRewardOffered);

			GameRuntimeManager.Instance.Events
				.Subscribe<RewardSelectedEvent>(OnRewardSelected);

			GameRuntimeManager.Instance.Events
				.Subscribe<DamageEvent>(OnDamage);


			GameRuntimeManager.Instance.Events
				.Subscribe<SpellKillEvent>(OnEnemyKilled);

			RefreshSpell();
		}

		private void OnDestroy()
		{

			GameRuntimeManager.Instance.Events
				.Unsubscribe<WaveStartedEvent>(OnWaveStarted);

			GameRuntimeManager.Instance.Events
				.Unsubscribe<EncounterCompletedEvent>(OnEncounterCompleted);

			GameRuntimeManager.Instance.Events
				.Unsubscribe<RewardOfferedEvent>(OnRewardOffered);

			GameRuntimeManager.Instance.Events
				.Unsubscribe<RewardSelectedEvent>(OnRewardSelected);

			GameRuntimeManager.Instance.Events
				.Unsubscribe<DamageEvent>(OnDamage);
					
			GameRuntimeManager.Instance.Events
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
			
		}
	}
}