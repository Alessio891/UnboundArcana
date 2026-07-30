using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnboundArcana.Core.Entities;
using UnboundArcana.Spells.Behaviors;
using UnboundArcana.Spells.Behaviors.Projectile;
using UnityEngine;

public class InitialCorePickerUI : MonoBehaviour
{
	[SerializeField] private InitialCorePickerSlot slotPrefab;
	[SerializeField] private Transform listTransform;

	[SerializeField] private List<SpellBehaviorDefinition> availableBehaviors;
	CanvasGroup canvasGroup;

	bool pickingChoice = false;

	private void Awake()
	{
		canvasGroup = GetComponent<CanvasGroup>();
	}
	private void Start()
	{
		Close();
		LoadBehaviors(availableBehaviors);
	}
	public IEnumerator openAndForcePick()
	{
		Open();
		pickingChoice = true;
		while (pickingChoice) { yield return null; }

	}
	public void Open() {
		canvasGroup.alpha = 1.0f;
		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;
	}
	public void Close() {
		canvasGroup.alpha = 0.0f;
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
	}
	public void LoadBehaviors(List<SpellBehaviorDefinition> behaviors) {
		foreach(Transform t in listTransform) {
			Destroy(t.gameObject);
		}

		foreach (SpellBehaviorDefinition behavior in behaviors)
		{
			InitialCorePickerSlot slot = Instantiate(slotPrefab);
			slot.transform.SetParent(listTransform, false);
			slot.SetBehavior(behavior);
			slot.manager = this;
			if (!(behavior is ProjectileBehaviorDefinition)) slot.SetSlotEnabled(false);
			else slot.SetSlotEnabled(true);
		}
	}

	public void PickBehavior(SpellBehaviorDefinition behavior) {
		var spellConfig = ExpeditionController.Instance.Player.GetComponent<SpellCaster>().SpellLoadout.GetCurrentSpell();
		GameRuntimeManager.Instance.SpellModification.TrySetBehavior(spellConfig.Configuration, behavior);
		Close();
		pickingChoice = false;
	}
}
