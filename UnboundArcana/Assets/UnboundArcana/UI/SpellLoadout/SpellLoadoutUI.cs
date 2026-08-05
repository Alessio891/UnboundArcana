using System.Collections.Generic;
using UnboundArcana.Core.Entities;
using UnboundArcana.Spells.Data;
using UnboundArcana.Spells.Modules;
using UnityEngine;
using UnityEngine.UI;

namespace UnboundArcana.UI
{
	public class SpellLoadoutUI : MonoBehaviour
	{
		private class SlotView
		{
			public GameObject Root;
			public Image Background;
			public Image Icon;
			public Image Cooldown;
			public RectTransform CooldownRect;
			public Text CooldownText;
			public Text Name;
			public Text Key;
			public Button Button;
		}

		[SerializeField] private int maxVisibleSlots = 4;
		[SerializeField] private Vector2 slotSize = new(58f, 58f);
		[SerializeField] private float spacing = 6f;
		[SerializeField] private float bottomOffset = 18f;
		[SerializeField] private Color backgroundColor = new(0.08f, 0.06f, 0.13f, 0.92f);
		[SerializeField] private Color selectedColor = new(0.45f, 0.22f, 0.7f, 0.98f);
		[SerializeField] private Color cooldownColor = new(0.02f, 0.01f, 0.04f, 0.72f);

		private readonly List<SlotView> views = new();
		private RectTransform root;
		private SpellLoadout loadout;
		private Font font;

		private void Awake()
		{
			font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
			CreateRoot();
		}

		private void OnEnable()
		{
			GameRuntimeManager.Instance.Events.Subscribe<PlayerSpawnedEvent>(OnPlayerSpawned);
		}

		private void OnDisable()
		{
			if (GameRuntimeManager.Instance != null)
			{
				GameRuntimeManager.Instance.Events.Unsubscribe<PlayerSpawnedEvent>(OnPlayerSpawned);
			}

			UnbindLoadout();
		}

		private void Update()
		{
			if (loadout == null)
			{
				return;
			}

			for (int i = 0; i < views.Count; i++)
			{
				SpellSlot slot = loadout.Slots[i];
				float cooldown = slot.Configuration.Cooldown;
				float progress = cooldown > 0f ? Mathf.Clamp01(slot.CooldownTimer / cooldown) : 0f;
				views[i].Cooldown.gameObject.SetActive(progress > 0f);
				views[i].CooldownRect.localScale = new Vector3(1f, progress, 1f);
				views[i].CooldownText.gameObject.SetActive(slot.CooldownTimer >= 0.1f);
				views[i].CooldownText.text = slot.CooldownTimer >= 1f ? Mathf.CeilToInt(slot.CooldownTimer).ToString() : slot.CooldownTimer.ToString("0.0");
			}
		}

		private void OnPlayerSpawned(PlayerSpawnedEvent evt)
		{
			BindLoadout(evt.player != null ? evt.player.GetComponent<SpellCaster>()?.SpellLoadout : null);
		}

		private void BindLoadout(SpellLoadout newLoadout)
		{
			UnbindLoadout();
			loadout = newLoadout;

			if (loadout == null)
			{
				root.gameObject.SetActive(false);
				return;
			}

			loadout.CurrentSpellChanged += OnSelectionChanged;
			loadout.SlotsChanged += Rebuild;
			Rebuild();
		}

		private void UnbindLoadout()
		{
			if (loadout == null)
			{
				return;
			}

			loadout.CurrentSpellChanged -= OnSelectionChanged;
			loadout.SlotsChanged -= Rebuild;
			loadout = null;
		}

		private void CreateRoot()
		{
			GameObject rootObject = new("SpellLoadout", typeof(RectTransform), typeof(Image), typeof(HorizontalLayoutGroup), typeof(ContentSizeFitter));
			rootObject.layer = gameObject.layer;
			rootObject.transform.SetParent(transform, false);
			root = rootObject.GetComponent<RectTransform>();
			root.anchorMin = new Vector2(0.5f, 0f);
			root.anchorMax = new Vector2(0.5f, 0f);
			root.pivot = new Vector2(0.5f, 0f);
			root.anchoredPosition = new Vector2(0f, bottomOffset);
			Image rootImage = rootObject.GetComponent<Image>();
			rootImage.color = new Color(0.025f, 0.018f, 0.04f, 0.82f);
			HorizontalLayoutGroup layout = rootObject.GetComponent<HorizontalLayoutGroup>();
			layout.padding = new RectOffset(7, 7, 7, 7);
			layout.spacing = spacing;
			layout.childAlignment = TextAnchor.MiddleCenter;
			layout.childControlWidth = false;
			layout.childControlHeight = false;
			layout.childForceExpandWidth = false;
			layout.childForceExpandHeight = false;
			ContentSizeFitter fitter = rootObject.GetComponent<ContentSizeFitter>();
			fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
			fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
			rootObject.SetActive(false);
		}

		private void Rebuild()
		{
			foreach (SlotView view in views)
			{
				Destroy(view.Root);
			}

			views.Clear();
			int count = Mathf.Min(loadout.Slots.Count, maxVisibleSlots);

			for (int i = 0; i < count; i++)
			{
				views.Add(CreateSlot(i, loadout.Slots[i].Configuration));
			}

			root.gameObject.SetActive(count > 0);
			OnSelectionChanged(loadout.CurrentSpell);
		}

		private SlotView CreateSlot(int index, SpellConfiguration configuration)
		{
			GameObject slotObject = new($"SpellSlot{index + 1}", typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
			slotObject.layer = gameObject.layer;
			slotObject.transform.SetParent(root, false);
			RectTransform slotRect = slotObject.GetComponent<RectTransform>();
			slotRect.sizeDelta = slotSize;
			LayoutElement layout = slotObject.GetComponent<LayoutElement>();
			layout.preferredWidth = slotSize.x;
			layout.preferredHeight = slotSize.y;
			Image background = slotObject.GetComponent<Image>();
			background.color = backgroundColor;
			Button button = slotObject.GetComponent<Button>();
			button.targetGraphic = background;
			int slotIndex = index;
			button.onClick.AddListener(() => loadout?.SelectSpell(slotIndex));

			Image icon = CreateImage("Icon", slotRect, new Vector2(7f, 13f), new Vector2(-7f, -5f));
			icon.preserveAspect = true;
			icon.sprite = GetIcon(configuration);
			icon.color = icon.sprite != null ? Color.white : new Color(0.38f, 0.3f, 0.5f, 0.8f);
			icon.raycastTarget = false;

			Image cooldown = CreateImage("Cooldown", slotRect, Vector2.zero, Vector2.zero);
			cooldown.color = cooldownColor;
			cooldown.raycastTarget = false;
			RectTransform cooldownRect = cooldown.rectTransform;
			cooldownRect.pivot = new Vector2(0.5f, 0f);
			cooldownRect.localScale = new Vector3(1f, 0f, 1f);

			Text key = CreateText("Key", slotRect, $"{index + 1}", 11, TextAnchor.UpperLeft);
			SetOffsets(key.rectTransform, new Vector2(4f, 2f), new Vector2(-2f, -2f));
			key.color = new Color(0.95f, 0.86f, 1f, 1f);

			Text name = CreateText("Name", slotRect, GetName(configuration), 9, TextAnchor.LowerCenter);
			SetOffsets(name.rectTransform, new Vector2(2f, 2f), new Vector2(-2f, -43f));
			name.color = Color.white;

			Text cooldownText = CreateText("CooldownTime", slotRect, "", 14, TextAnchor.MiddleCenter);
			SetOffsets(cooldownText.rectTransform, Vector2.zero, Vector2.zero);
			cooldownText.fontStyle = FontStyle.Bold;
			cooldownText.color = Color.white;
			cooldownText.gameObject.SetActive(false);

			return new SlotView { Root = slotObject, Background = background, Icon = icon, Cooldown = cooldown, CooldownRect = cooldownRect, CooldownText = cooldownText, Name = name, Key = key, Button = button };
		}

		private Image CreateImage(string objectName, RectTransform parent, Vector2 minOffset, Vector2 maxOffset)
		{
			GameObject imageObject = new(objectName, typeof(RectTransform), typeof(Image));
			imageObject.layer = gameObject.layer;
			imageObject.transform.SetParent(parent, false);
			RectTransform rect = imageObject.GetComponent<RectTransform>();
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.one;
			SetOffsets(rect, minOffset, maxOffset);
			return imageObject.GetComponent<Image>();
		}

		private Text CreateText(string objectName, RectTransform parent, string value, int fontSize, TextAnchor alignment)
		{
			GameObject textObject = new(objectName, typeof(RectTransform), typeof(Text));
			textObject.layer = gameObject.layer;
			textObject.transform.SetParent(parent, false);
			Text text = textObject.GetComponent<Text>();
			text.font = font;
			text.text = value;
			text.fontSize = fontSize;
			text.alignment = alignment;
			text.raycastTarget = false;
			RectTransform rect = text.rectTransform;
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.one;
			return text;
		}

		private void SetOffsets(RectTransform rect, Vector2 minOffset, Vector2 maxOffset)
		{
			rect.offsetMin = minOffset;
			rect.offsetMax = maxOffset;
		}

		private void OnSelectionChanged(int selectedIndex)
		{
			for (int i = 0; i < views.Count; i++)
			{
				bool selected = i == selectedIndex;
				views[i].Background.color = selected ? selectedColor : backgroundColor;
				views[i].Root.transform.localScale = selected ? Vector3.one * 1.08f : Vector3.one;
			}
		}

		private Sprite GetIcon(SpellConfiguration configuration)
		{
			if (configuration == null)
			{
				return null;
			}

			foreach (SpellModuleDefinition module in configuration.Modules)
			{
				if (module != null && module.Type == SpellModuleType.Principle && module.Icon != null)
				{
					return module.Icon;
				}
			}

			return configuration.Behavior != null ? configuration.Behavior.Icon : null;
		}

		private string GetName(SpellConfiguration configuration)
		{
			if (configuration == null)
			{
				return "Empty";
			}

			foreach (SpellModuleDefinition module in configuration.Modules)
			{
				if (module != null && module.Type == SpellModuleType.Principle)
				{
					return module.name.Replace("Principle", "").Trim();
				}
			}

			return configuration.Behavior != null ? configuration.Behavior.name.Replace("Behavior", "").Trim() : "Spell";
		}
	}
}
