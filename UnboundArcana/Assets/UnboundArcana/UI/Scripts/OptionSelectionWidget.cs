using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionSelectionWidget : MonoBehaviour
{
	[SerializeField] private Text currentValue;
	OptionSetting setting;
	public void NextOption() {
		currentOption++;
		if (currentOption >= options.Length) currentOption = 0;

		currentValue.text = options[currentOption];

		SettingsManager.Instance.Set<int>(setting.id, currentOption);
	}
	public void PreviousOption() {
		Debug.Log("PREVIOUS");
		currentOption--;
		if (currentOption < 0) currentOption = options.Length - 1;
		currentValue.text = options[currentOption];
		SettingsManager.Instance.Set<int>(setting.id, currentOption);
	}

	private string[] options;
	private int currentOption = 0;

	public void Initialize(OptionSetting optionSetting) {
		currentOption = SettingsManager.Instance.Get<int>(optionSetting.id);

		options = optionSetting.options;

		currentValue.text = options[currentOption];
		this.setting = optionSetting;
	}

}
