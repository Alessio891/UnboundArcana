using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Runtime;

public class NewResearchController : MonoBehaviour
{
	[SerializeField] EntityDefinition defaultPlayerDefinition;
	[SerializeField] List<ResearcherArchetypeDefinition> archetypes;
	int selectedArchetype = 0;

	[SerializeField] Image archetypeArt;
	[SerializeField] Text archetypeName;
	[SerializeField] VerticalLayoutGroup innates;
	[SerializeField] VerticalLayoutGroup perks;
	[SerializeField] GameObject textEntryPrefab;
	private void Start()
	{
		UpdateArchetypeInfo();
	}
	public void SelectArchetype(int archetype) {
		selectedArchetype = archetype;
		UpdateArchetypeInfo();
	}

	public void BeginNewRun()
	{
		if (defaultPlayerDefinition == null)
		{
			Debug.LogError("Cannot begin a new run without a default player definition.", this);
			return;
		}

		GameSession.Instance.BeginNewRun(RunConfiguration.CreateDefault(defaultPlayerDefinition));
		SceneManager.LoadScene("Intro", LoadSceneMode.Single);
	}

	void UpdateArchetypeInfo() {
		archetypeArt.sprite = archetypes[selectedArchetype].CharacterArt;
		archetypeName.text = archetypes[selectedArchetype].Name;

		foreach (Transform child in innates.transform)
		{
			Destroy(child.gameObject);
		}
		foreach (Transform child in perks.transform)
		{
			Destroy(child.gameObject);
		}

		foreach(string innate in archetypes[selectedArchetype].Innates) {
			GameObject innateEntry = Instantiate(textEntryPrefab);
			innateEntry.GetComponent<Text>().text = "- " + innate;
			innateEntry.transform.SetParent(innates.transform);
		}
		foreach (string perk in archetypes[selectedArchetype].Perks)
		{
			GameObject innateEntry = Instantiate(textEntryPrefab);
			innateEntry.GetComponent<Text>().text = "- " + perk;
			innateEntry.transform.SetParent(perks.transform);
		}

	}
}
