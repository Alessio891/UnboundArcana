using UnityEngine;
using UnityEngine.UI;

public class GameSettingEntryController : MonoBehaviour
{
	public Text DisplayName;

	public Slider sliderController;
	public ToggleOptionWidget checkboxController;
	public OptionSelectionWidget optionsController;


	NumberSetting numberSetting;
	void OnSliderChanged(float value)
	{
		SettingsManager.Instance.Set<float>(numberSetting.id, value);
	}

	public void LoadSetting(SettingDefinition setting) {
		DisplayName.text = setting.displayName;

		checkboxController.gameObject.SetActive(false);
		sliderController.gameObject.SetActive(false);
		optionsController.gameObject.SetActive(false);

		if (setting is BoolSetting boolSetting) {
			checkboxController.gameObject.SetActive(true);
			checkboxController.Initialize(boolSetting);
		} else if (setting is OptionSetting optionSetting) {
			optionsController.gameObject.SetActive(true);
			optionsController.Initialize(optionSetting);
		} else if (setting is NumberSetting numSetting) {
			this.numberSetting = numSetting;
			sliderController.gameObject.SetActive(true);
			sliderController.minValue = numSetting.min;
			sliderController.maxValue = numSetting.max;
			sliderController.value = SettingsManager.Instance.Get<float>(numSetting.id);
			sliderController.onValueChanged.AddListener(OnSliderChanged);
		}
	}
}
