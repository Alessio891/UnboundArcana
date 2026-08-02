using System.Collections;
using UnboundArcana.Core.Rooms;
using UnityEngine;
using UnityEngine.UI;

namespace UnboundArcana.Core.Expedition
{
	public class RoomCompletionFeedbackPresenter : MonoBehaviour
	{
		private const float RewardMessageDuration = 4f;
		private const float RoomCompleteMessageDuration = 1.5f;
		private const float FadeDuration = 0.5f;

		private CanvasGroup canvasGroup;
		private Text title;
		private Text message;
		private Coroutine activeRoutine;

		private void Awake()
		{
			BuildPresentation();
			canvasGroup.alpha = 0f;
		}

		private void OnEnable()
		{
			if (GameRuntimeManager.Instance == null)
				return;

			GameRuntimeManager.Instance.Events.Subscribe<RoomCompletedEvent>(OnRoomCompleted);
			GameRuntimeManager.Instance.Events.Subscribe<ExpeditionRewardStartedEvent>(OnRewardsAvailable);
			GameRuntimeManager.Instance.Events.Subscribe<RoomStartedEvent>(OnRoomStarted);
		}

		private void OnDisable()
		{
			if (GameRuntimeManager.Instance == null)
				return;

			GameRuntimeManager.Instance.Events.Unsubscribe<RoomCompletedEvent>(OnRoomCompleted);
			GameRuntimeManager.Instance.Events.Unsubscribe<ExpeditionRewardStartedEvent>(OnRewardsAvailable);
			GameRuntimeManager.Instance.Events.Unsubscribe<RoomStartedEvent>(OnRoomStarted);
		}

		private void OnRoomCompleted(RoomCompletedEvent evt)
		{
			Show("ROOM COMPLETE", string.Empty);
			activeRoutine = StartCoroutine(HideRoutine(RoomCompleteMessageDuration));
		}

		private void OnRewardsAvailable(ExpeditionRewardStartedEvent evt)
		{
			Show("CHOOSE ONE REWARD", "Approach a reward and press F");
			activeRoutine = StartCoroutine(HideRoutine(RewardMessageDuration));
		}

		private void OnRoomStarted(RoomStartedEvent evt)
		{
			Hide();
		}

		private void Show(string titleText, string messageText)
		{
			if (activeRoutine != null)
			{
				StopCoroutine(activeRoutine);
				activeRoutine = null;
			}

			title.text = titleText;
			message.text = messageText;
			canvasGroup.alpha = 1f;
		}

		private void Hide()
		{
			if (activeRoutine != null) { StopCoroutine(activeRoutine); }
			activeRoutine = null;
			canvasGroup.alpha = 0f;
		}

		private IEnumerator HideRoutine(float duration)
		{
			yield return new WaitForSecondsRealtime(duration);

			float elapsed = 0f;
			while (elapsed < FadeDuration)
			{
				elapsed += Time.unscaledDeltaTime;
				canvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / FadeDuration);
				yield return null;
			}

			canvasGroup.alpha = 0f;
			activeRoutine = null;
		}

		private void BuildPresentation()
		{
			GameObject panel = new GameObject("RoomCompletionFeedback", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(CanvasGroup), typeof(Image));
			panel.transform.SetParent(transform, false);

			Canvas canvas = panel.GetComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingOrder = 90;

			CanvasScaler scaler = panel.GetComponent<CanvasScaler>();
			scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			scaler.referenceResolution = new Vector2(1920f, 1080f);
			scaler.matchWidthOrHeight = 0.5f;

			RectTransform panelRect = panel.GetComponent<RectTransform>();
			panelRect.anchorMin = new Vector2(0.5f, 0.72f);
			panelRect.anchorMax = new Vector2(0.5f, 0.72f);
			panelRect.anchoredPosition = Vector2.zero;
			panelRect.sizeDelta = new Vector2(620f, 118f);

			Image background = panel.GetComponent<Image>();
			background.color = new Color(0.025f, 0.02f, 0.035f, 0.82f);
			background.raycastTarget = false;

			canvasGroup = panel.GetComponent<CanvasGroup>();
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;

			title = CreateText(panel.transform, "Title", new Vector2(0f, 20f), 34, FontStyle.Bold);
			message = CreateText(panel.transform, "Message", new Vector2(0f, -27f), 22, FontStyle.Normal);
		}

		private Text CreateText(Transform parent, string objectName, Vector2 position, int fontSize, FontStyle style)
		{
			GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(Text));
			textObject.transform.SetParent(parent, false);

			RectTransform rect = textObject.GetComponent<RectTransform>();
			rect.anchorMin = new Vector2(0.5f, 0.5f);
			rect.anchorMax = new Vector2(0.5f, 0.5f);
			rect.anchoredPosition = position;
			rect.sizeDelta = new Vector2(580f, 44f);

			Text text = textObject.GetComponent<Text>();
			text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
			text.fontSize = fontSize;
			text.fontStyle = style;
			text.alignment = TextAnchor.MiddleCenter;
			text.color = Color.white;
			text.raycastTarget = false;
			return text;
		}
	}
}
