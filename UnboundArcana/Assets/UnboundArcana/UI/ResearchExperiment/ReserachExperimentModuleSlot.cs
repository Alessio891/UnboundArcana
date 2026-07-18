using UnboundArcana.Spells.Behaviors;
using UnboundArcana.Spells.Modules;
using UnboundArcana.Spells.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public enum ResearchExperimentSlotType {
	Core, Principle, Catalyst, Flux
}

public class ReserachExperimentModuleSlot : MonoBehaviour, IDropHandler
{
	[SerializeField] private ResearchExperimentSlotType type;
	[SerializeField] private Image icon;

	[SerializeField]
	private SpellModuleDefinition module;
	[SerializeField]
	private SpellBehaviorDefinition spellBehavior;

	public ReserachExperimentUI uiManager;
	private void Awake()
	{
		if (module != null)
		{
			SetSpellModule(module);
		} else if (spellBehavior != null) {
			SetSpellBehavior(spellBehavior);
		} else {
			SetSpellModule(null);
		}
	}

	public ResearchExperimentSlotType Type => type;

	public void SetSpellModule(SpellModuleDefinition module) {
		
		this.module = module;
		if (module == null) {
			icon.enabled = false;
			return;
		} else {
			icon.enabled = true;
		}
		icon.sprite = module.Icon;
	}

	public void SetSpellBehavior(SpellBehaviorDefinition behavior) {
		this.spellBehavior = behavior;
		if (spellBehavior == null)
		{
			icon.enabled = false;
			return;
		}
		else
		{
			icon.enabled = true;
		}
		icon.sprite = behavior.Icon;
	}

	public void OnDrop(PointerEventData eventData)
	{
		Debug.Log("Dropped something on slot");
		uiManager.RewardDroppedOnSlot();
		uiManager.EndDragReward();
	}
}
