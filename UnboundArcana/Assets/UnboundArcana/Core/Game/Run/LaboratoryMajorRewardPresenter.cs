using System;
using System.Collections;
using System.Collections.Generic;
using UnboundArcana.Player;
using UnboundArcana.Spells.Modules;
using UnityEngine;
using UnityEngine.UI;

namespace UnboundArcana.Core.Expedition
{
	public class LaboratoryMajorRewardPresenter : MonoBehaviour
	{
		private const float ConfirmationDuration = 1.1f;
		private static readonly Color OverlayColor = new(0.015f, 0.01f, 0.025f, 0.88f);
		private static readonly Color PanelColor = new(0.025f, 0.018f, 0.04f, 0.98f);
		private static readonly Color CardColor = new(0.08f, 0.06f, 0.13f, 0.98f);
		private static readonly Color AccentColor = new(0.45f, 0.22f, 0.7f, 1f);
		private readonly List<Button> selectionButtons = new();
		private GameObject presentationRoot;
		private RectTransform offersRoot;
		private Text feedback;
		private Font font;
		private LaboratoryMajorRewardSession session;
		private PlayerInput playerInput;
		private bool restoreInputEnabled;
		private bool presenterOpen;
		private bool selectionLocked;
		private Coroutine closeRoutine;

		public event Action<LaboratorySelectionResult> SelectionSucceeded;
		public bool IsOpen => presenterOpen;
		public string FailureMessage { get; private set; }
		public LaboratorySelectionResult LastSelectionResult { get; private set; }

		private void Awake()
		{
			EnsurePresentation();
		}

		private void OnDisable()
		{
			if (closeRoutine != null) { StopCoroutine(closeRoutine); }
			closeRoutine = null;
			RestoreInput();
		}

		public LaboratoryOfferStatus Open(LaboratoryMajorRewardSession rewardSession, PlayerInput input)
		{
			EnsurePresentation();
			FailureMessage = string.Empty;
			if (presenterOpen)
			{
				FailureMessage = "A Laboratory reward selection is already open.";
				return session != null ? session.OfferStatus : LaboratoryOfferStatus.MissingRewardService;
			}

			if (rewardSession == null)
			{
				FailureMessage = "The Laboratory reward session is unavailable.";
				return LaboratoryOfferStatus.MissingRewardService;
			}

			if (input == null)
			{
				FailureMessage = "Player input is unavailable.";
				return LaboratoryOfferStatus.MissingSpellCaster;
			}

			LaboratoryOfferStatus status = rewardSession.GenerateOffers();
			if (status != LaboratoryOfferStatus.Success || rewardSession.Offers.Count == 0)
			{
				FailureMessage = GetOfferFailureMessage(status);
				return status;
			}

			session = rewardSession;
			playerInput = input;
			restoreInputEnabled = playerInput.InputEnabled;
			selectionLocked = false;
			presenterOpen = true;
			feedback.text = string.Empty;
			RebuildOffers(session.Offers);
			presentationRoot.SetActive(true);
			playerInput.SetInputEnabled(false);
			return LaboratoryOfferStatus.Success;
		}

		public LaboratorySelectionResult SelectOffer(SpellModuleDefinition module)
		{
			if (selectionLocked)
			{
				LastSelectionResult = new LaboratorySelectionResult(LaboratorySelectionStatus.AlreadySelected, session?.ActiveConfiguration, module);
				FailureMessage = "A Laboratory reward has already been selected.";
				return LastSelectionResult;
			}

			if (!presenterOpen || session == null)
			{
				LastSelectionResult = new LaboratorySelectionResult(LaboratorySelectionStatus.SessionNotReady, session?.ActiveConfiguration, module);
				FailureMessage = "The Laboratory reward selection is not available.";
				return LastSelectionResult;
			}

			LastSelectionResult = session.TrySelect(module);
			if (!LastSelectionResult.Success)
			{
				FailureMessage = GetSelectionFailureMessage(LastSelectionResult.Status);
				feedback.color = new Color(1f, 0.55f, 0.55f, 1f);
				feedback.text = FailureMessage;
				return LastSelectionResult;
			}

			selectionLocked = true;
			FailureMessage = string.Empty;
			SetButtonsInteractable(false);
			feedback.color = new Color(0.72f, 1f, 0.75f, 1f);
			feedback.text = $"{GetModuleName(module)} added to the active spell.";
			SelectionSucceeded?.Invoke(LastSelectionResult);
			closeRoutine = StartCoroutine(CloseAfterConfirmation());
			return LastSelectionResult;
		}

		private IEnumerator CloseAfterConfirmation()
		{
			yield return new WaitForSecondsRealtime(ConfirmationDuration);
			presentationRoot.SetActive(false);
			presenterOpen = false;
			closeRoutine = null;
			RestoreInput();
		}

		private void RestoreInput()
		{
			if (playerInput != null) { playerInput.SetInputEnabled(restoreInputEnabled); }
			playerInput = null;
			presenterOpen = false;
		}

		private void RebuildOffers(IReadOnlyList<SpellModuleDefinition> offers)
		{
			for (int i = offersRoot.childCount - 1; i >= 0; i--) { Destroy(offersRoot.GetChild(i).gameObject); }
			selectionButtons.Clear();
			for (int i = 0; i < offers.Count; i++) { CreateOfferCard(offers[i]); }
		}

		private void CreateOfferCard(SpellModuleDefinition module)
		{
			GameObject cardObject = CreateUiObject("Offer", offersRoot, typeof(Image), typeof(VerticalLayoutGroup), typeof(LayoutElement));
			cardObject.GetComponent<RectTransform>().sizeDelta = new Vector2(350f, 500f);
			Image card = cardObject.GetComponent<Image>();
			card.color = CardColor;
			LayoutElement cardSize = cardObject.GetComponent<LayoutElement>();
			cardSize.preferredWidth = 350f;
			cardSize.preferredHeight = 500f;
			VerticalLayoutGroup layout = cardObject.GetComponent<VerticalLayoutGroup>();
			layout.padding = new RectOffset(20, 20, 20, 20);
			layout.spacing = 14f;
			layout.childAlignment = TextAnchor.UpperCenter;
			layout.childControlWidth = true;
			layout.childControlHeight = true;
			layout.childForceExpandWidth = true;
			layout.childForceExpandHeight = false;

			GameObject iconObject = CreateUiObject("Icon", cardObject.transform, typeof(Image), typeof(LayoutElement));
			Image icon = iconObject.GetComponent<Image>();
			icon.sprite = module != null ? module.Icon : null;
			icon.preserveAspect = true;
			icon.raycastTarget = false;
			iconObject.GetComponent<LayoutElement>().preferredHeight = 105f;
			iconObject.SetActive(icon.sprite != null);

			Text name = CreateText("Name", cardObject.transform, GetModuleName(module), 26, FontStyle.Bold, TextAnchor.MiddleCenter, 64f);
			name.color = Color.white;
			name.resizeTextForBestFit = true;
			name.resizeTextMinSize = 20;
			name.resizeTextMaxSize = 26;
			Text description = CreateText("Description", cardObject.transform, GetModuleDescription(module), 18, FontStyle.Normal, TextAnchor.UpperCenter, icon.sprite != null ? 185f : 280f);
			description.color = new Color(0.88f, 0.84f, 0.94f, 1f);
			description.resizeTextForBestFit = true;
			description.resizeTextMinSize = 15;
			description.resizeTextMaxSize = 18;

			GameObject buttonObject = CreateUiObject("Select", cardObject.transform, typeof(Image), typeof(Button), typeof(LayoutElement));
			Image buttonImage = buttonObject.GetComponent<Image>();
			buttonImage.color = AccentColor;
			Button button = buttonObject.GetComponent<Button>();
			button.targetGraphic = buttonImage;
			buttonObject.GetComponent<LayoutElement>().preferredHeight = 58f;
			SpellModuleDefinition selectedModule = module;
			button.onClick.AddListener(() => SelectOffer(selectedModule));
			selectionButtons.Add(button);
			Text buttonLabel = CreateText("Label", buttonObject.transform, "SELECT", 22, FontStyle.Bold, TextAnchor.MiddleCenter, 58f);
			SetStretch(buttonLabel.rectTransform);
		}

		private void SetButtonsInteractable(bool interactable)
		{
			foreach (Button button in selectionButtons) { button.interactable = interactable; }
		}

		private void BuildPresentation()
		{
			presentationRoot = CreateUiObject("LaboratoryMajorReward", transform, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(Image));
			Canvas canvas = presentationRoot.GetComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingOrder = 200;
			CanvasScaler scaler = presentationRoot.GetComponent<CanvasScaler>();
			scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			scaler.referenceResolution = new Vector2(1920f, 1080f);
			scaler.matchWidthOrHeight = 0.5f;
			RectTransform overlay = presentationRoot.GetComponent<RectTransform>();
			overlay.anchorMin = Vector2.zero;
			overlay.anchorMax = Vector2.one;
			overlay.offsetMin = Vector2.zero;
			overlay.offsetMax = Vector2.zero;
			presentationRoot.GetComponent<Image>().color = OverlayColor;

			GameObject panelObject = CreateUiObject("Panel", presentationRoot.transform, typeof(Image));
			RectTransform panel = panelObject.GetComponent<RectTransform>();
			panel.anchorMin = new Vector2(0.5f, 0.5f);
			panel.anchorMax = new Vector2(0.5f, 0.5f);
			panel.sizeDelta = new Vector2(1220f, 760f);
			panelObject.GetComponent<Image>().color = PanelColor;
			Text title = CreateText("Title", panel, "CHOOSE A MAJOR REWARD", 38, FontStyle.Bold, TextAnchor.MiddleCenter, 70f);
			SetAnchoredRect(title.rectTransform, new Vector2(0f, 320f), new Vector2(1140f, 70f));
			Text subtitle = CreateText("Subtitle", panel, "Modify the active spell", 22, FontStyle.Normal, TextAnchor.MiddleCenter, 42f);
			subtitle.color = new Color(0.78f, 0.7f, 0.9f, 1f);
			SetAnchoredRect(subtitle.rectTransform, new Vector2(0f, 274f), new Vector2(1140f, 42f));

			GameObject offersObject = CreateUiObject("Offers", panel, typeof(HorizontalLayoutGroup));
			offersRoot = offersObject.GetComponent<RectTransform>();
			SetAnchoredRect(offersRoot, new Vector2(0f, -4f), new Vector2(1160f, 520f));
			HorizontalLayoutGroup offersLayout = offersObject.GetComponent<HorizontalLayoutGroup>();
			offersLayout.spacing = 18f;
			offersLayout.childAlignment = TextAnchor.MiddleCenter;
			offersLayout.childControlWidth = false;
			offersLayout.childControlHeight = false;
			offersLayout.childForceExpandWidth = false;
			offersLayout.childForceExpandHeight = false;

			feedback = CreateText("Feedback", panel, string.Empty, 22, FontStyle.Bold, TextAnchor.MiddleCenter, 50f);
			SetAnchoredRect(feedback.rectTransform, new Vector2(0f, -338f), new Vector2(1140f, 50f));
		}

		private void EnsurePresentation()
		{
			if (presentationRoot != null) { return; }
			font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
			BuildPresentation();
			presentationRoot.SetActive(false);
		}

		private GameObject CreateUiObject(string objectName, Transform parent, params Type[] components)
		{
			Type[] allComponents = new Type[components.Length + 1];
			allComponents[0] = typeof(RectTransform);
			Array.Copy(components, 0, allComponents, 1, components.Length);
			GameObject uiObject = new(objectName, allComponents);
			uiObject.layer = gameObject.layer;
			uiObject.transform.SetParent(parent, false);
			return uiObject;
		}

		private Text CreateText(string objectName, Transform parent, string value, int fontSize, FontStyle style, TextAnchor alignment, float preferredHeight)
		{
			GameObject textObject = CreateUiObject(objectName, parent, typeof(Text), typeof(LayoutElement));
			Text text = textObject.GetComponent<Text>();
			text.font = font;
			text.text = value;
			text.fontSize = fontSize;
			text.fontStyle = style;
			text.alignment = alignment;
			text.color = Color.white;
			text.raycastTarget = false;
			text.horizontalOverflow = HorizontalWrapMode.Wrap;
			text.verticalOverflow = VerticalWrapMode.Truncate;
			textObject.GetComponent<LayoutElement>().preferredHeight = preferredHeight;
			return text;
		}

		private void SetAnchoredRect(RectTransform rect, Vector2 position, Vector2 size)
		{
			rect.anchorMin = new Vector2(0.5f, 0.5f);
			rect.anchorMax = new Vector2(0.5f, 0.5f);
			rect.anchoredPosition = position;
			rect.sizeDelta = size;
		}

		private void SetStretch(RectTransform rect)
		{
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.one;
			rect.offsetMin = Vector2.zero;
			rect.offsetMax = Vector2.zero;
		}

		private string GetModuleName(SpellModuleDefinition module)
		{
			if (module == null) { return "Unknown Module"; }
			return string.IsNullOrWhiteSpace(module.ModuleName) ? module.name : module.ModuleName;
		}

		private string GetModuleDescription(SpellModuleDefinition module)
		{
			if (module == null || string.IsNullOrWhiteSpace(module.ModuleDescription)) { return "Modify the active spell."; }
			return module.ModuleDescription;
		}

		private string GetOfferFailureMessage(LaboratoryOfferStatus status)
		{
			return status == LaboratoryOfferStatus.NoCompatibleModules ? "No compatible Modules are available for the active spell." : $"Unable to prepare Laboratory rewards: {status}.";
		}

		private string GetSelectionFailureMessage(LaboratorySelectionStatus status)
		{
			return status switch
			{
				LaboratorySelectionStatus.InvalidOffer => "That Module is not part of this Laboratory offer.",
				LaboratorySelectionStatus.OfferNoLongerCompatible => "That Module is no longer compatible with the active spell.",
				LaboratorySelectionStatus.ModificationRejected => "The active spell could not be modified.",
				LaboratorySelectionStatus.AlreadySelected => "A Laboratory reward has already been selected.",
				_ => "The Laboratory reward selection is not ready."
			};
		}
	}
}
