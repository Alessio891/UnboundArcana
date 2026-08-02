using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Expedition;
using UnboundArcana.Core.Interaction;
using UnboundArcana.Core.Research;
using UnboundArcana.Core.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class ResearchPickup : MonoBehaviour, IInteractable
{
	[SerializeField] private Canvas infoCanvas;
	[SerializeField] private Text researchTitle;
	[SerializeField] private Text researchKnowledge;
	[SerializeField] private Text researchDescription;

	private ResearchDefinition definition;


	void Start() {
		float hoverHeight = 0.01f;
		float hoverTime = 0.8f;
		iTween.MoveBy(gameObject, iTween.Hash(
	   "y", hoverHeight * 2f,
	   "time", hoverTime + Random.Range(-0.2f, 0.2f),
	   "easetype", iTween.EaseType.easeInOutSine,
	   "looptype", iTween.LoopType.pingPong,
	   "islocal", true
		));
		if (infoCanvas != null) { infoCanvas.gameObject.SetActive(false); }
  
	}

	public bool CanInteract(Entity entity)
	{
		return true;
	}

	public void Initialize(
		ResearchDefinition definition)
	{
		this.definition = definition;

		researchTitle.text = definition.DisplayName;
		researchKnowledge.text = string.Empty;
		researchDescription.text = definition.Description;
	}

	public bool Interact(Entity entity)
	{
		GameRuntimeManager.Instance.Events.Publish(new ResearchCollectedEvent(definition));

		Destroy(gameObject);
		return true;
	}

	public void OnDeselected(Entity entity)
	{
		if (infoCanvas != null) { infoCanvas.gameObject.SetActive(false); }
	}

	public void OnInteractionRangeEnter(Entity entity)
	{
	}

	public void OnInteractionRangeExit(Entity entity)
	{
	}

	public void OnSelected(Entity entity)
	{
		if (infoCanvas != null) { infoCanvas.gameObject.SetActive(true); }
	}

	//private void OnTriggerEnter2D(
	//	Collider2D other)
	//{
	//	if (!other.TryGetComponent<Entity>(
	//		out var entity))
	//		return;

	//	if (!entity.CompareTag("Player"))
	//		return;

		
	//}
}
