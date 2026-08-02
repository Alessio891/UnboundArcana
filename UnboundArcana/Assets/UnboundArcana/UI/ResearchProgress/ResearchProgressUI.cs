using System.Collections.Generic;
using UnboundArcana.Core.Expedition;
using UnityEngine;

public class ResearchProgressUI : MonoBehaviour
{
	[SerializeField] private ResearchProgressEntryUI entryPrefab;
	[SerializeField] private Transform root;
	[SerializeField] private CanvasGroup panel;

	List<ResearchProgressEntryUI> entries = new();

	private void Awake()
	{
		RefreshVisibility();
	}

	private void OnEnable()
	{
		GameRuntimeManager.Instance.Events.Subscribe<ResearchCollectedEvent>(OnResearchGained);
	}

	private void OnResearchGained(ResearchCollectedEvent evt)
	{
		var  entry = Instantiate(entryPrefab);
		entry.transform.SetParent(root, false);
		entry.Initialize(evt.Research);
		entries.Add(entry);
		RefreshVisibility();
	}

	private void RefreshVisibility()
	{
		bool hasActiveModifiers = entries.Exists(entry => entry != null);
		panel.alpha = hasActiveModifiers ? 1f : 0f;
		panel.interactable = hasActiveModifiers;
		panel.blocksRaycasts = hasActiveModifiers;
	}

	private void OnDisable()
	{
		GameRuntimeManager.Instance.Events.Unsubscribe<ResearchCollectedEvent>(OnResearchGained);
	}
}
