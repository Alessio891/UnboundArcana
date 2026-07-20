using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Interaction;
using UnityEngine;

public class ArcaneStationInteraction : MonoBehaviour, IInteractable, IInteractionVisualProvider
{
	[SerializeField]
	private Transform anchor;

	public Transform InteractionAnchor => anchor;

	public string InteractionText => "Experiment";
	public void OnSelected(Entity entity)
	{
	}

	public void OnDeselected(Entity entity)
	{
	}

	public void OnInteractionRangeEnter(Entity entity)
	{
		var animator = GetComponent<Animator>();
		if (animator)
			animator.SetBool("isActive", true);
	}

	public void OnInteractionRangeExit(Entity entity)
	{
		var animator = GetComponent<Animator>();
		if (animator)
			animator.SetBool("isActive", false);
	}

	public bool CanInteract(Entity entity)
	{
		return true;
	}

	public bool Interact(Entity entity)
	{
		GameRuntimeManager.Instance.Events.Publish(new ResearchExperimentStationEvent(gameObject, entity));
		return true;
	}
}