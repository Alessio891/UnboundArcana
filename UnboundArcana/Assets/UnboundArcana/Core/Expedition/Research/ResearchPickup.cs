using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Expedition;
using UnboundArcana.Core.Interaction;
using UnboundArcana.Core.Research;
using UnboundArcana.Core.Runtime;
using UnityEngine;

public class ResearchPickup : MonoBehaviour, IInteractable
{
	private ResearchDefinition definition;

	public bool CanInteract(Entity entity)
	{
		return true;
	}

	public void Initialize(
		ResearchDefinition definition)
	{
		this.definition = definition;
	}

	public bool Interact(Entity entity)
	{
		GameRuntimeManager.Instance.Events.Publish(new ResearchCollectedEvent(definition));

		Destroy(gameObject);
		return true;
	}

	public void OnDeselected(Entity entity)
	{
	}

	public void OnInteractionRangeEnter(Entity entity)
	{
	}

	public void OnInteractionRangeExit(Entity entity)
	{
	}

	public void OnSelected(Entity entity)
	{
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