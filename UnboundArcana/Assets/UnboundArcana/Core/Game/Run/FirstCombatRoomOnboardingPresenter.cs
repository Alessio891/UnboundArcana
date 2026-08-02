using System.Collections;
using UnboundArcana.Core.Rooms;
using UnityEngine;
using UnityEngine.UI;

namespace UnboundArcana.Core.Expedition
{
	public class FirstCombatRoomOnboardingPresenter : MonoBehaviour
	{
		private const float DisplayDuration = 6f;
		private const float FadeDuration = 0.5f;

		private CanvasGroup canvasGroup;
		private bool shown;

		private void Awake()
		{
			BuildPresentation();
			canvasGroup.alpha = 0f;
		}

		private void OnEnable()
		{
			if (GameRuntimeManager.Instance != null)
				GameRuntimeManager.Instance.Events.Subscribe<RoomStartedEvent>(OnRoomStarted);
		}

		private void OnDisable()
		{
			if (GameRuntimeManager.Instance != null)
				GameRuntimeManager.Instance.Events.Unsubscribe<RoomStartedEvent>(OnRoomStarted);
		}

		private void OnRoomStarted(RoomStartedEvent evt)
		{
			if (shown || evt.Room?.Definition?.Type != RoomType.Combat)
				return;

			shown = true;
			StartCoroutine(ShowRoutine());
		}

		private IEnumerator ShowRoutine()
		{
			canvasGroup.alpha = 1f;
			yield return new WaitForSecondsRealtime(DisplayDuration);

			float elapsed = 0f;
			while (elapsed < FadeDuration)
			{
				elapsed += Time.unscaledDeltaTime;
				canvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / FadeDuration);
				yield return null;
			}

			canvasGroup.alpha = 0f;
		}

		private void BuildPresentation()
		{
			GameObject panel = new GameObject("FirstCombatRoomOnboarding", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(CanvasGroup), typeof(Image));
			panel.transform.SetParent(transform, false);

			Canvas canvas = panel.GetComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingOrder = 100;

			CanvasScaler scaler = panel.GetComponent<CanvasScaler>();
			scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			scaler.referenceResolution = new Vector2(1920f, 1080f);
			scaler.matchWidthOrHeight = 0.5f;

			RectTransform panelRect = panel.GetComponent<RectTransform>();
			panelRect.anchorMin = new Vector2(0.5f, 0.8f);
			panelRect.anchorMax = new Vector2(0.5f, 0.8f);
			panelRect.anchoredPosition = Vector2.zero;
			panelRect.sizeDelta = new Vector2(760f, 128f);

			Image background = panel.GetComponent<Image>();
			background.color = new Color(0.025f, 0.02f, 0.035f, 0.82f);
			background.raycastTarget = false;

			canvasGroup = panel.GetComponent<CanvasGroup>();
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;

			CreateText(panel.transform, "WASD  Move     Mouse  Aim     Left Mouse  Cast", new Vector2(0f, 24f), 28, FontStyle.Bold);
			CreateText(panel.transform, "Objective: Defeat all enemies", new Vector2(0f, -28f), 24, FontStyle.Normal);
		}

		private void CreateText(Transform parent, string content, Vector2 position, int fontSize, FontStyle style)
		{
			GameObject textObject = new GameObject("Text", typeof(RectTransform), typeof(Text));
			textObject.transform.SetParent(parent, false);

			RectTransform rect = textObject.GetComponent<RectTransform>();
			rect.anchorMin = new Vector2(0.5f, 0.5f);
			rect.anchorMax = new Vector2(0.5f, 0.5f);
			rect.anchoredPosition = position;
			rect.sizeDelta = new Vector2(720f, 44f);

			Text text = textObject.GetComponent<Text>();
			text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
			text.fontSize = fontSize;
			text.fontStyle = style;
			text.alignment = TextAnchor.MiddleCenter;
			text.color = Color.white;
			text.raycastTarget = false;
			text.text = content;
		}
	}
}
