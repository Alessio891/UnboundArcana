using UnboundArcana.Spells.Behaviors;
using UnityEngine;
using UnityEngine.UI;

public class InitialCorePickerSlot : MonoBehaviour
{
	[SerializeField] private Image icon;
	[SerializeField] private Image disabledImage;
	SpellBehaviorDefinition spellBehaviorDefinition;

	public InitialCorePickerUI manager;
	public void SetBehavior(SpellBehaviorDefinition behavior) {
		this.spellBehaviorDefinition = behavior;

		icon.sprite = behavior.Icon;
	}

	public void SetSlotEnabled(bool enabled) {
		disabledImage.enabled = !enabled;
	}

	public void OnClicked() {
		manager.PickBehavior(spellBehaviorDefinition);
	}
}
