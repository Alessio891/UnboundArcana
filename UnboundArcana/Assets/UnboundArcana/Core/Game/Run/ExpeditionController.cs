using System.Collections;
using System.Collections.Generic;
using UnboundArcana.Core.Camera;
using UnboundArcana.Core.Entities;
using UnboundArcana.Player;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class ExpeditionController : MonoBehaviour
{

	private PlayerController player;
	public PlayerController Player => player;

	private static ExpeditionController instance;
	public static ExpeditionController Instance => instance;

	[Header("Placeholders?")]
	[SerializeField] private InitialCorePickerUI corePickerUI;
	[SerializeField] private GameTilemapController tilemapController;
	[SerializeField] private PlayerController playerPrefab;
	
	[SerializeField] private Transform playerStartPosition;
	[SerializeField] private GameObject arcanaCoreSprite;
	[SerializeField] private List<Transform> targets;
	[SerializeField] private Animator portalAnimator;
	PlayableDirector timeline;
	private void Awake()
	{
		timeline = GetComponent<PlayableDirector>();
		instance = this;
	}

	private void Start()
	{
		player = Instantiate(playerPrefab);
		player.transform.position = playerStartPosition.position;
		player.gameObject.SetActive(false);
		portalAnimator.gameObject.SetActive(false);
		StartCoroutine(startSequence());
	}

	IEnumerator startSequence() {
		tilemapController.StartConstructing();
		player.GetComponent<PlayerInput>().SetInputEnabled(false);
		yield return new WaitForSeconds(2.5f);
		portalAnimator.gameObject.SetActive(true);
		MainCameraManager.Instance.MoveTo(portalAnimator.transform.position);
		yield return new WaitForSeconds(0.5f);
		portalAnimator.SetBool("isActive", true);
		yield return new WaitForSeconds(0.8f);
		player.gameObject.SetActive(true);
		MainCameraManager.Instance.SetFollowTarget(player.transform);
		portalAnimator.SetBool("isActive", false);
		foreach (Transform t in targets) { 
			player.GetComponent<CharacterMotor>().MoveTo(t.position);
			yield return new WaitForSeconds(1.5f);
		}
		yield return new WaitForSeconds(1.5f);
		yield return StartCoroutine(corePickerUI.openAndForcePick());
		yield return new WaitForSeconds(0.5f);
		player.GetComponent<PlayerInput>().SetInputEnabled(true);
		Destroy(arcanaCoreSprite);
		GameRuntimeManager.Instance.Events.Publish(new ShowDialogueEvent("??? The environment is re-arranging???", null));
		yield return new WaitForSeconds(0.5f);
		GameRuntimeManager.Instance.Events.Publish(new MapConstructionEvent(false));
		yield return new WaitForSeconds(1.5f);
		yield return StartCoroutine(ExpeditionController.Instance.tilemapController.FadeOut());




	}
}
