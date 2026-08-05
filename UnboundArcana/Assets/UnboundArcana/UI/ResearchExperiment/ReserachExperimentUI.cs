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

	private GameReward draggedRewardData;
	private bool isOpen = false;

	private SpellConfiguration spellConfiguration;
	private CanvasGroup canvasGroup;
	private void Awake()
	{
		canvasGroup = GetComponent<CanvasGroup>();
		Close();
	}

	private void OnEnable()
	{
		GameRuntimeManager.Instance.Events.Subscribe<ResearchExperimentStationEvent>(OnStationUsed);
	}

	private void OnDisable()
	{
		GameRuntimeManager.Instance.Events.Unsubscribe<ResearchExperimentStationEvent>(OnStationUsed);
	}

	void GenerateDebugRewards()
	{
		foreach (Transform t in rewardList.transform) { Destroy(t.gameObject); }

		List<SpellModuleDefinition> compatibleModules = GameDatabase.Instance.Spells.modules.FindAll(module => GameRuntimeManager.Instance.SpellModification.CanAddModule(spellConfiguration, module));
		var rewards = GameRuntimeManager.Instance.ModuleReward.RollModules(compatibleModules, 3);

		foreach (var module in rewards)
		{
			SpellModuleReward reward = new();

			reward.module = module;
			reward.icon = module.Icon;
			reward.cost = 10;

			ResearchExperimentRewardSlot slot = Instantiate(rewardPrefab);
			slot.transform.SetParent(rewardList.transform, false);
			slot.uiManager = this;
			slot.SetReward(reward);
		}
		//foreach (var behavior in testRewardsBehavior)
		//{
		//	SpellBehaviorReward reward = new();

		//	reward.behavior = behavior;
		//	reward.icon = behavior.Icon;
		//	reward.cost = 50;

		//	ResearchExperimentRewardSlot slot = Instantiate(rewardPrefab);
		//	slot.uiManager = this;
		//	slot.transform.SetParent(rewardList.transform, false);
		//	slot.SetReward(reward);
		//}
	}

	public void StartDragReward(GameReward reward)
	{
		draggedReward.enabled = true;
		draggedReward.sprite = reward.icon;
		draggedRewardData = reward;
	}
	public void UpdateDragReward(PointerEventData eventData)
	{
		draggedReward.transform.position = eventData.position;
	}
	public void EndDragReward()
	{
		draggedReward.enabled = false;
	}
	public void RewardDroppedOnSlot()
	{
		if (draggedRewardData is SpellModuleReward moduleReward)
		{
			GameRuntimeManager.Instance.SpellModification.TryAddModule(spellConfiguration, moduleReward.module);
			isOpen = false;
			Close();
		}
		else if (draggedRewardData is SpellBehaviorReward behaviorReward)
		{
			GameRuntimeManager.Instance.SpellModification.TrySetBehavior(spellConfiguration, behaviorReward.behavior);
			isOpen = false;
			Close();
		}
	}

	void Open()
	{
		if (!isOpen)
		{
			canvasGroup.alpha = 1.0f;
			canvasGroup.blocksRaycasts = true;
			isOpen = true;
		}
	}
	void Close()
	{
		canvasGroup.alpha = 0.0f;
		canvasGroup.blocksRaycasts = false;
		isOpen = false;

	}
	void OnStationUsed(ResearchExperimentStationEvent evt)
	{
		if (!isOpen)
		{
			Open();
			SetSpellConfiguration(evt.Entity.GetComponent<SpellCaster>().SpellLoadout.GetCurrentSpell().Configuration);
			GenerateDebugRewards();
		}
	}

	public void SetSpellConfiguration(SpellConfiguration spell)
	{
		spellConfiguration = spell;

		if (spell == null) return;
		Debug.Log("Setting spell config");
		if (principleSlots.Count > 0) principleSlots[0].SetSpellModule(spell.Principle);
		if (catalystSlots.Count > 0) catalystSlots[0].SetSpellModule(spell.CatalystA);
		if (catalystSlots.Count > 1) catalystSlots[1].SetSpellModule(spell.CatalystB);
		if (fluxSlots.Count > 0) fluxSlots[0].SetSpellModule(spell.Flux);
		coreSlot.SetSpellBehavior(spell.Behavior);

	}
}
