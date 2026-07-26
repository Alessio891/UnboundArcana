using System;
using System.Collections;
using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Expedition;
using UnboundArcana.Core.Research;
using UnboundArcana.Core.Rooms;
using UnboundArcana.Core.Runtime;
using UnityEngine;

public class UAUnitTester : MonoBehaviour
{
	[SerializeField] private Entity player;
	[SerializeField] private ResearchDefinition researchDefinition;

	private void Start()
	{
		StartCoroutine(testStart());
	}

	private void OnRoomStart(RoomStartedEvent @event)
	{
		StartCoroutine(testStart());
	}

	IEnumerator testStart() {
		yield return new WaitForSeconds(0.5f);
		Debug.Log("[UnitTest] Adding research");
		GameSession.Instance.Player.AddResearch(researchDefinition);
		yield return new WaitForSeconds(1.0f);
		GameSession.Instance.Player.AddKnowledge(50);
		yield return new WaitForSeconds(1.0f);
		GameSession.Instance.Player.AddKnowledge(50);
		yield return new WaitForSeconds(3.0f);
		ExpeditionRuntimeController.Instance.AdvanceToNextRoom();

	}
	
}
