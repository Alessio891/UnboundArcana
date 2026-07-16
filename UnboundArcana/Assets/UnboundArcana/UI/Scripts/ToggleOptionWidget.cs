using UnityEngine;
using UnityEngine.UI;

public class ToggleOptionWidget : MonoBehaviour
{
	private bool currentValue;
	public bool CurrentValue => currentValue;

	[SerializeField] private Image CheckedImage, UncheckedImage;

	BoolSetting setting;
	public void Initialize(BoolSetting setting) { 
		this.setting = setting;
		SetState(SettingsManager.Instance.Get<bool>(setting.id));

	}

	public void SetState(bool toggled)
	{
		CheckedImage.gameObject.SetActive(toggled);
		UncheckedImage.gameObject.SetActive(!toggled);
		currentValue = toggled;
		SettingsManager.Instance.Set<bool>(setting.id, currentValue);
	}

	public void Toggle() {
		SetState(!currentValue);
	}
}
