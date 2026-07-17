using UnityEngine;

namespace UnboundArcana.Core.Interaction
{
	public class InteractionSensor : MonoBehaviour
	{
		[SerializeField]
		private InteractionController controller;

		private void Awake()
		{
			if (controller == null)
			{
				controller =
					GetComponentInParent<InteractionController>();
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			IInteractable interactable =
				other.GetComponent<IInteractable>();

			if (interactable == null)
			{
				return;
			}

			controller.Register(interactable);
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			IInteractable interactable =
				other.GetComponent<IInteractable>();

			if (interactable == null)
			{
				return;
			}

			controller.Unregister(interactable);
		}
	}
}