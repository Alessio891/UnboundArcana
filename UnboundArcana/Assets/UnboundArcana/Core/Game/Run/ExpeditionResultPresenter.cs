using UnboundArcana.Core.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UnboundArcana.Core.Expedition
{
	public class ExpeditionResultPresenter : MonoBehaviour
	{
		private const string IntroScene = "Intro";
		private const string MainMenuScene = "MainMenu";

		private GameObject overlay;
		private Text title;
		private Text message;
		private Button restartButton;
		private Button mainMenuButton;
		private bool actionStarted;

		private void Awake()
		{
			BuildPresentation();
			overlay.SetActive(false);
		}

		private void OnEnable()
		{
			if (GameRuntimeManager.Instance != null)
				GameRuntimeManager.Instance.Events.Subscribe<ExpeditionEndedEvent>(OnExpeditionEnded);
		}

		private void Start()
		{
			ExpeditionResult result = GetComponent<ExpeditionRuntimeController>()?.Result;

			if (result != null)
				Show(result);
		}

		private void OnDisable()
		{
			if (GameRuntimeManager.Instance != null)
				GameRuntimeManager.Instance.Events.Unsubscribe<ExpeditionEndedEvent>(OnExpeditionEnded);
		}

		private void OnExpeditionEnded(ExpeditionEndedEvent evt)
		{
			Show(evt.Result);
		}

		private void Show(ExpeditionResult result)
		{
			if (result == null || overlay.activeSelf)
				return;

			bool completed = result.Outcome == ExpeditionOutcome.Completed;
			title.text = completed ? "VICTORY" : "DEFEAT";
			title.color = completed ? new Color(0.55f, 0.9f, 0.55f) : new Color(0.95f, 0.45f, 0.45f);
			message.text = completed ? "The expedition is complete." : "The expedition has ended.";
			overlay.SetActive(true);
			Debug.Log($"Presenting expedition result: {result.Outcome}. Reason: {result.Reason}");
		}

		private void RestartRun()
		{
			if (!TryBeginAction())
				return;

			RunConfiguration configuration = GameSession.Instance.CurrentRun;

			if (configuration == null)
			{
				Debug.LogError("Cannot restart expedition without a current run configuration.");
				actionStarted = false;
				SetButtonsInteractable(true);
				return;
			}

			GameSession.Instance.BeginNewRun(configuration);
			SceneManager.LoadScene(IntroScene, LoadSceneMode.Single);
		}

		private void ReturnToMainMenu()
		{
			if (!TryBeginAction())
				return;

			GameSession.Instance.ClearRun();
			SceneManager.LoadScene(MainMenuScene, LoadSceneMode.Single);
		}

		private bool TryBeginAction()
		{
			if (actionStarted)
				return false;

			actionStarted = true;
			SetButtonsInteractable(false);
			return true;
		}

		private void SetButtonsInteractable(bool interactable)
		{
			restartButton.interactable = interactable;
			mainMenuButton.interactable = interactable;
		}

		private void BuildPresentation()
		{
			overlay = new GameObject("ExpeditionResultOverlay", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(Image));
			overlay.transform.SetParent(transform, false);

			RectTransform overlayRect = overlay.GetComponent<RectTransform>();
			overlayRect.anchorMin = Vector2.zero;
			overlayRect.anchorMax = Vector2.one;
			overlayRect.offsetMin = Vector2.zero;
			overlayRect.offsetMax = Vector2.zero;

			Canvas canvas = overlay.GetComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingOrder = 1000;

			CanvasScaler scaler = overlay.GetComponent<CanvasScaler>();
			scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			scaler.referenceResolution = new Vector2(1920f, 1080f);
			scaler.matchWidthOrHeight = 0.5f;

			Image background = overlay.GetComponent<Image>();
			background.color = new Color(0.025f, 0.02f, 0.035f, 0.94f);
			background.raycastTarget = true;

			title = CreateText("Title", new Vector2(0.5f, 0.62f), new Vector2(900f, 120f), 64, FontStyle.Bold);
			message = CreateText("Message", new Vector2(0.5f, 0.51f), new Vector2(900f, 80f), 28, FontStyle.Normal);
			restartButton = CreateButton("RestartRun", "RESTART RUN", new Vector2(0.5f, 0.38f));
			mainMenuButton = CreateButton("ReturnToMainMenu", "MAIN MENU", new Vector2(0.5f, 0.28f));
			restartButton.onClick.AddListener(RestartRun);
			mainMenuButton.onClick.AddListener(ReturnToMainMenu);
		}

		private Text CreateText(string objectName, Vector2 anchor, Vector2 size, int fontSize, FontStyle style)
		{
			GameObject textObject = new(objectName, typeof(RectTransform), typeof(Text));
			textObject.transform.SetParent(overlay.transform, false);

			RectTransform rect = textObject.GetComponent<RectTransform>();
			rect.anchorMin = anchor;
			rect.anchorMax = anchor;
			rect.anchoredPosition = Vector2.zero;
			rect.sizeDelta = size;

			Text textComponent = textObject.GetComponent<Text>();
			textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
			textComponent.fontSize = fontSize;
			textComponent.fontStyle = style;
			textComponent.alignment = TextAnchor.MiddleCenter;
			textComponent.color = Color.white;
			return textComponent;
		}

		private Button CreateButton(string objectName, string label, Vector2 anchor)
		{
			GameObject buttonObject = new(objectName, typeof(RectTransform), typeof(Image), typeof(Button));
			buttonObject.transform.SetParent(overlay.transform, false);

			RectTransform rect = buttonObject.GetComponent<RectTransform>();
			rect.anchorMin = anchor;
			rect.anchorMax = anchor;
			rect.anchoredPosition = Vector2.zero;
			rect.sizeDelta = new Vector2(360f, 72f);

			Image image = buttonObject.GetComponent<Image>();
			image.color = new Color(0.16f, 0.13f, 0.2f, 1f);

			Button button = buttonObject.GetComponent<Button>();
			ColorBlock colors = button.colors;
			colors.highlightedColor = new Color(0.28f, 0.22f, 0.36f, 1f);
			colors.pressedColor = new Color(0.1f, 0.08f, 0.13f, 1f);
			colors.disabledColor = new Color(0.1f, 0.09f, 0.12f, 0.7f);
			button.colors = colors;

			Text labelText = CreateText("Label", new Vector2(0.5f, 0.5f), new Vector2(340f, 64f), 24, FontStyle.Bold);
			labelText.transform.SetParent(buttonObject.transform, false);
			labelText.text = label;
			return button;
		}
	}
}
