using UnityEngine;
using System.Collections.Generic;
using UnboundArcana.Spells.Data;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Modules;
using UnboundArcana.Core.Entities;
using UnityEngine.UI;
using UnboundArcana.Spells.Behaviors;
using UnityEngine.EventSystems;

public class ReserachExperimentUI : MonoBehaviour
{
	[Header("Slots")]
	[SerializeField] private ReserachExperimentModuleSlot coreSlot;
	[SerializeField] private List<ReserachExperimentModuleSlot> principleSlots;
	[SerializeField] private List<ReserachExperimentModuleSlot> catalystSlots;
	[SerializeField] private List<ReserachExperimentModuleSlot> fluxSlots;

	[Header("Rewards")]
	[SerializeField] private ResearchExperimentRewardSlot rewardPrefab;
	[SerializeField] private VerticalLayoutGroup rewardList;
	[SerializeField] private Image draggedReward;

	[Header("Debug")]
	[SerializeField] private SpellCaster testCaster;
	[SerializeField] private List<SpellModuleDefinition> testRewardsModule;
	[SerializeField] private List<SpellBehaviorDefinition> testRewardsBehavior;


	private bool isOpen = false;

	private SpellConfiguration spellConfiguration;

	private void Awake()
	{
		GameRuntimeManager.Instance.Events.Subscribe<ResearchExperimentStationEvent>(OnStationUsed);
		gameObject.SetActive(false);
		SetSpellConfiguration(testCaster.SpellConfiguration);
	}

	void GenerateDebugRewards() {
		foreach(Transform t in rewardList.transform) { Destroy(t.gameObject); }

		foreach(var module in testRewardsModule) {
			SpellModuleReward reward = new();

			reward.module = module;
			reward.icon = module.Icon;
			reward.cost = 10;

			ResearchExperimentRewardSlot slot = Instantiate(rewardPrefab);
			slot.transform.SetParent(rewardList.transform, false);
			slot.uiManager = this;
			slot.SetReward(reward);
		}
		foreach (var behavior in testRewardsBehavior)
		{
			SpellBehaviorReward reward = new();

			reward.behavior = behavior;
			reward.icon = behavior.Icon;
			reward.cost = 50;

			ResearchExperimentRewardSlot slot = Instantiate(rewardPrefab);
			slot.uiManager = this;
			slot.transform.SetParent(rewardList.transform, false);
			slot.SetReward(reward);
		}
	}

	public void StartDragReward(GameReward reward) {
		draggedReward.enabled = true;
		draggedReward.sprite = reward.icon;
	}
	public void UpdateDragReward(PointerEventData eventData) { 
		draggedReward.transform.position = eventData.position;
	}
	public void EndDragReward() {
		draggedReward.enabled = false;
	}

	void OnStationUsed(ResearchExperimentStationEvent evt) {
		if (!isOpen)
		{
			gameObject.SetActive(true);
			isOpen = true;
			SetSpellConfiguration(testCaster.SpellConfiguration);
			GenerateDebugRewards();
		}
	}

	public void SetSpellConfiguration(SpellConfiguration spell) { 
		spellConfiguration = spell;
		
		if (spell == null) return;
		Debug.Log("Setting spell config");
		int index = 0;
		foreach(var module in spell.modules) {
			if (index >= principleSlots.Count) break;

			principleSlots[index].SetSpellModule(module);
			index++;
		}
		coreSlot.SetSpellBehavior(spell.behavior);

	}
}
