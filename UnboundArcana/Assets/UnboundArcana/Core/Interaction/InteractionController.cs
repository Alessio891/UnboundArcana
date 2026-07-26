using System.Collections.Generic;
using UnityEngine;
using UnboundArcana.Core.Entities;
using System.Linq;

namespace UnboundArcana.Core.Interaction
{
	[RequireComponent(typeof(Entity))]
	public class InteractionController : MonoBehaviour
	{
		private Entity entity;
		private IInteractable selectedInteractable;
		private readonly List<IInteractable> interactables =
			new();

		private void Awake()
		{
			entity = GetComponent<Entity>();
		}

		public void Register(
			IInteractable interactable)
		{
			if (interactable == null)
			{
				return;
			}

			if (!interactables.Contains(interactable))
			{
				interactables.Add(interactable);

				interactable.OnInteractionRangeEnter(entity);
			}
		}

		public void Unregister(
			IInteractable interactable)
		{
			if (interactable == null)
			{
				return;
			}

			interactables.Remove(interactable);

			interactable.OnInteractionRangeExit(entity);

			if (selectedInteractable == interactable)
			{
				selectedInteractable.OnDeselected(entity);
				GameRuntimeManager.Instance.Events.Publish(
					new InteractionDeselectedEvent(selectedInteractable)
				);
				selectedInteractable = null;
			}
		}

		public bool Interact()
		{
			IInteractable interactable =
				selectedInteractable;

			if (interactable == null)
			{
				return false;
			}

			if (!interactable.CanInteract(entity))
			{
				return false;
			}

			return interactable.Interact(entity);
		}

		private IInteractable GetBestInteractable()
		{
			if (interactables.Count == 0)
			{
				return null;
			}
			IInteractable closest = interactables
				.OrderBy(e => ((e as MonoBehaviour).transform.position - entity.transform.position).sqrMagnitude)
				.FirstOrDefault();
			return closest;
		}

		private void Update()
		{
			UpdateSelection();
		}
		private void UpdateSelection()
		{
			IInteractable best =
				GetBestInteractable();

			if (best == selectedInteractable)
			{
				return;
			}

			if (selectedInteractable != null)
			{
				selectedInteractable.OnDeselected(entity);
				GameRuntimeManager.Instance.Events.Publish(
					new InteractionDeselectedEvent(selectedInteractable)
				);
			}

			selectedInteractable = best;

			if (selectedInteractable != null)
			{
				selectedInteractable.OnSelected(entity);
				GameRuntimeManager.Instance.Events.Publish(
					new InteractionSelectedEvent(selectedInteractable)
				);
			}
		}
	}
}