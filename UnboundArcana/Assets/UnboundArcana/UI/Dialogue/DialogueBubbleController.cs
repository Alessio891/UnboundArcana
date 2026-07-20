using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBubbleController : MonoBehaviour
{
	private CanvasGroup canvasGroup;
	[SerializeField] private Text dialogueText;

	private void Awake()
	{
		canvasGroup = GetComponent<CanvasGroup>();
		HidePanel();
	}

	private void OnEnable()
	{
		GameRuntimeManager.Instance.Events.Subscribe<ShowDialogueEvent>(OnShowDialogue);
	}


	private void OnDisable()
	{
		GameRuntimeManager.Instance.Events.Unsubscribe<ShowDialogueEvent>(OnShowDialogue);
	}
	private void OnShowDialogue(ShowDialogueEvent evt)
	{
		ShowDialogue(evt.Message);
	}

	void ShowPanel() {
		canvasGroup.alpha = 1.0f;
		canvasGroup.blocksRaycasts = true;
	}
	void HidePanel() {
		canvasGroup.alpha = 0.0f;
		canvasGroup.blocksRaycasts = false;
	}

	public void ShowDialogue(string message) {
		ShowPanel();
		dialogueText.text = message;
		StopAllCoroutines();
		StartCoroutine(waitAndHide());
	}
	IEnumerator waitAndHide() {
		yield return new WaitForSeconds(3.5f);
		HidePanel();
	}
}
