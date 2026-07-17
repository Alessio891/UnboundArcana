using UnboundArcana.Core.Entities;
using UnityEngine;

namespace UnboundArcana.Core.Interaction
{
	public interface IInteractable
	{
		bool CanInteract(Entity entity);

		bool Interact(Entity entity);

		void OnInteractionRangeEnter(Entity entity);

		void OnInteractionRangeExit(Entity entity);

		void OnSelected(Entity entity);

		void OnDeselected(Entity entity);
	}

	public interface IInteractionVisualProvider
	{
		Transform InteractionAnchor { get; }

		string InteractionText { get; }
	}
}