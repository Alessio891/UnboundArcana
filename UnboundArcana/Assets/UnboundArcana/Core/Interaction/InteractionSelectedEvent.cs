using UnboundArcana.Core.Interaction;

public class InteractionSelectedEvent
{
	public IInteractable Interactable { get; }

	public InteractionSelectedEvent(
		IInteractable interactable)
	{
		Interactable = interactable;
	}
}

public class InteractionDeselectedEvent
{
	public IInteractable Interactable { get; }

	public InteractionDeselectedEvent(
		IInteractable interactable)
	{
		Interactable = interactable;
	}
}