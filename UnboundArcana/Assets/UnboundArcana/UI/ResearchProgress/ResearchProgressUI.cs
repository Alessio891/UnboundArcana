using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnboundArcana.Core.Expedition;
using UnboundArcana.Core.Research;
using UnboundArcana.Core.Runtime;
using UnityEngine;

public class ResearchProgressUI : MonoBehaviour
{
	[SerializeField] private ResearchProgressEntryUI entryPrefab;
	[SerializeField] private Transform root;

	List<ResearchProgressEntryUI> entries = new();

	private void OnEnable()
	{
		GameRuntimeManager.Instance.Events.Subscribe<ResearchCollectedEvent>(OnResearchGained);
		GameRuntimeManager.Instance.Events.Subscribe<KnowledgeGainedEvent>(OnKnowledgeGained);
		GameRuntimeManager.Instance.Events.Subscribe<ResearchGrantedEvent>(OnResearchGranted);
	}

	private void OnResearchGranted(ResearchGrantedEvent evt)
	{
		for (int i = 0; i < entries.Count; i++)
		{
			if (entries[i].researchInstance== evt.research) {
				Destroy(entries[i].gameObject);
				entries.RemoveAt(i);
				break;
			}
		}
	}

	private void OnKnowledgeGained(KnowledgeGainedEvent @event)
	{
		foreach(var research in GameSession.Instance.Player.Researches) {
			foreach(var entry in entries) {
				if (entry.definition == research.Definition) {
					entry.UpdateProgress(research);
				}
			}
		}
	}

	private void OnResearchGained(ResearchCollectedEvent evt)
	{
		var  entry = Instantiate(entryPrefab);
		entry.transform.SetParent(root, false);
		entry.Initialize(evt.Research);
		entries.Add(entry);
	}

	private void OnDisable()
	{
		GameRuntimeManager.Instance.Events.Unsubscribe<ResearchCollectedEvent>(OnResearchGained);
		GameRuntimeManager.Instance.Events.Unsubscribe<KnowledgeGainedEvent>(OnKnowledgeGained);
		GameRuntimeManager.Instance.Events.Unsubscribe<ResearchGrantedEvent>(OnResearchGranted);
	}

	private void OnPlayerSpawned(PlayerSpawnedEvent evt)
	{
		
	}
}
