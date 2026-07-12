using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnboundArcana.Core.Events;
using UnboundArcana.Spells.Data;
using UnboundArcana.Spells.Modules;

namespace UnboundArcana.Sandbox
{
	public class RewardController : MonoBehaviour
	{
		public SpellTester SpellTester;

		public SpellModuleDefinition[] availableRewards;

		private readonly List<SpellModuleDefinition> currentRewards = new();

		private bool waitingForChoice;

		private void Start()
		{
			SpellTester.RuntimeManager.GameEvents
				.Subscribe<EncounterCompletedEvent>(
					OnEncounterCompleted
				);
		}

		private void Update()
		{
			if (!waitingForChoice)
			{
				return;
			}

			if (Keyboard.current.digit1Key.wasPressedThisFrame)
			{
				ChooseReward(0);
			}

			if (Keyboard.current.digit2Key.wasPressedThisFrame)
			{
				ChooseReward(1);
			}

			if (Keyboard.current.digit3Key.wasPressedThisFrame)
			{
				ChooseReward(2);
			}
		}

		private void OnEncounterCompleted(
			EncounterCompletedEvent eventData)
		{
			GenerateRewards();

			waitingForChoice = true;

			Debug.Log("Choose reward:");

			for (int i = 0; i < currentRewards.Count; i++)
			{
				Debug.Log($"{i + 1}: {currentRewards[i].name}");
			}
		}

		private void GenerateRewards()
		{
			currentRewards.Clear();

			List<SpellModuleDefinition> candidates = new();

			foreach (SpellModuleDefinition module in availableRewards)
			{
				if (CanChoose(module))
				{
					candidates.Add(module);
				}
			}

			int rewardCount = Mathf.Min(3, candidates.Count);

			for (int i = 0; i < rewardCount; i++)
			{
				int randomIndex = Random.Range(0, candidates.Count);

				currentRewards.Add(candidates[randomIndex]);

				candidates.RemoveAt(randomIndex);
			}
		}

		private void ChooseReward(int index)
		{
			if (index < 0 || index >= currentRewards.Count)
			{
				return;
			}

			SpellModuleDefinition module = currentRewards[index];

			SpellTester.Configuration.AddModule(module);

			SpellTester.RuntimeManager.GameEvents.Publish(
				new RewardSelectedEvent()
			);

			waitingForChoice = false;

			Debug.Log($"Added module: {module.name}");
		}

		private bool CanChoose(
			SpellModuleDefinition module)
		{
			return module != null &&
				!SpellTester.Configuration.HasModule(module);
		}
	}
}