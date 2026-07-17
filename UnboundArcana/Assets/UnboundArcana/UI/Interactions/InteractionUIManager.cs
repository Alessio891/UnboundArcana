using UnityEngine;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Interaction;

namespace UnboundArcana.UI
{
	public class InteractionUIManager : MonoBehaviour
	{
		[SerializeField]
		private InteractionPrompt promptPrefab;

		private InteractionPrompt prompt;

		private Transform currentAnchor;

		private void Start()
		{
			prompt = Instantiate(promptPrefab);

			prompt.gameObject.SetActive(false);
			prompt.transform.SetParent(transform, false);
		}

		private void OnEnable()
		{
			GameRuntimeManager.Instance.Events
				.Subscribe<InteractionSelectedEvent>(
					OnInteractionSelected
				);

			GameRuntimeManager.Instance.Events
				.Subscribe<InteractionDeselectedEvent>(
					OnInteractionDeselected
				);
		}

		private void OnDisable()
		{
			if (GameRuntimeManager.Instance == null)
			{
				return;
			}

			GameRuntimeManager.Instance.Events
				.Unsubscribe<InteractionSelectedEvent>(
					OnInteractionSelected
				);

			GameRuntimeManager.Instance.Events
				.Unsubscribe<InteractionDeselectedEvent>(
					OnInteractionDeselected
				);
		}

		private void LateUpdate()
		{
			if (currentAnchor == null)
			{
				return;
			}

			prompt.transform.position =
				currentAnchor.position;
		}

		private void OnInteractionSelected(
			InteractionSelectedEvent eventData)
		{
			if (eventData.Interactable
				is not IInteractionVisualProvider provider)
			{
				return;
			}

			currentAnchor =
				provider.InteractionAnchor;

			prompt.SetText(
				provider.InteractionText
			);

			prompt.gameObject.SetActive(true);
		}

		private void OnInteractionDeselected(
			InteractionDeselectedEvent eventData)
		{
			Debug.Log("UI ON INTERACTION DESELECTED");

			currentAnchor = null;

			prompt.gameObject.SetActive(false);
		}
	}
}