using System;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
	public static SettingsManager Instance;

	Dictionary<string, object> values = new();

	public List<SettingGroup> groups;

	void Awake()
	{
		Instance = this;
		Initialize();
	}

	void Initialize()
	{
		foreach (var group in groups)
		{
			foreach (var setting in group.settings)
			{
				LoadSetting(setting);
			}
		}
	}

	void LoadSetting(SettingDefinition setting)
	{
		if (setting is BoolSetting boolSetting)
		{
			values[setting.id] = PlayerPrefs.GetInt(
				setting.id,
				boolSetting.defaultValue ? 1 : 0
			) == 1;
		}

		else if (setting is NumberSetting numberSetting)
		{
			values[setting.id] = PlayerPrefs.GetFloat(
				setting.id,
				numberSetting.defaultValue
			);
		}

		else if (setting is OptionSetting optionSetting)
		{
			values[setting.id] = PlayerPrefs.GetInt(
				setting.id,
				optionSetting.defaultValue
			);
		}
	}
	public void Save()
	{
		foreach (var value in values)
		{
			if (value.Value is bool boolValue)
			{
				PlayerPrefs.SetInt(
					value.Key,
					boolValue ? 1 : 0
				);
			}

			else if (value.Value is float floatValue)
			{
				PlayerPrefs.SetFloat(
					value.Key,
					floatValue
				);
			}

			else if (value.Value is int intValue)
			{
				PlayerPrefs.SetInt(
					value.Key,
					intValue
				);
			}
		}

		PlayerPrefs.Save();
	}
	public T Get<T>(string id)
	{
		return (T)values[id];
	}

	public void Set<T>(string id, T value)
	{
		values[id] = value;
	}
}