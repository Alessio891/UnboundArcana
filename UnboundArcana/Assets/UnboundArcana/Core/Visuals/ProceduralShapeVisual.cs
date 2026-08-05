using UnityEngine;

namespace UnboundArcana.Core.Visuals
{
	public sealed class ProceduralShapeVisual : MonoBehaviour
	{
		private const float OutlineScale = 1.1f;
		private SpriteRenderer outlineRenderer;
		private SpriteRenderer fillRenderer;
		private SpriteRenderer haloRenderer;
		private Color baseColor;
		private float baseSize;
		private float pulseTimer;
		private float pulseDuration;
		private float pulseStrength;
		private float flashTimer;
		private Color flashColor;
		private float fadeTimer;
		private float fadeDuration;

		public SpriteRenderer FillRenderer => fillRenderer;
		public Color CurrentColor => baseColor;

		public static ProceduralShapeVisual Create(Transform parent, string name, ProceduralShape shape, Color color, float size, int sortingOrder, bool halo = false)
		{
			GameObject instance = new(name);
			instance.transform.SetParent(parent, false);
			ProceduralShapeVisual visual = instance.AddComponent<ProceduralShapeVisual>();
			visual.Initialize(shape, color, size, sortingOrder, halo);
			return visual;
		}

		private void Initialize(ProceduralShape shape, Color color, float size, int sortingOrder, bool halo)
		{
			baseColor = color;
			baseSize = size;
			transform.localScale = Vector3.one * size;
			Sprite sprite = ProceduralSpriteLibrary.Get(shape);
			outlineRenderer = CreateRenderer("Outline", sprite, ProceduralPalette.Outline(color), sortingOrder);
			outlineRenderer.transform.localScale = Vector3.one * OutlineScale;
			fillRenderer = CreateRenderer("Fill", sprite, color, sortingOrder + 1);

			if (halo)
			{
				haloRenderer = CreateRenderer("Halo", ProceduralSpriteLibrary.Get(ProceduralShape.Ring), WithAlpha(color, 0.18f), sortingOrder - 1);
				haloRenderer.transform.localScale = Vector3.one * 1.45f;
			}
		}

		private SpriteRenderer CreateRenderer(string name, Sprite sprite, Color color, int sortingOrder)
		{
			GameObject instance = new(name);
			instance.transform.SetParent(transform, false);
			SpriteRenderer renderer = instance.AddComponent<SpriteRenderer>();
			renderer.sprite = sprite;
			renderer.color = color;
			renderer.sharedMaterial = ProceduralVisualFactory.SharedMaterial;
			renderer.sortingLayerName = ProceduralVisualFactory.InteractiveSortingLayer;
			renderer.sortingOrder = sortingOrder;
			return renderer;
		}

		public void SetColor(Color color)
		{
			baseColor = color;
			if (fillRenderer != null)
			{
				fillRenderer.color = color;
			}
			if (outlineRenderer != null)
			{
				outlineRenderer.color = ProceduralPalette.Outline(color);
			}
			if (haloRenderer != null)
			{
				haloRenderer.color = WithAlpha(color, 0.18f);
			}
		}

		public void SetScale(float size)
		{
			baseSize = size;
			transform.localScale = Vector3.one * size;
		}

		public void SetAlpha(float alpha)
		{
			float normalizedAlpha = Mathf.Clamp01(alpha);
			Color color = baseColor;
			color.a *= normalizedAlpha;
			if (fillRenderer != null)
			{
				fillRenderer.color = color;
			}
			if (outlineRenderer != null)
			{
				Color outline = ProceduralPalette.Outline(baseColor);
				outline.a *= normalizedAlpha;
				outlineRenderer.color = outline;
			}
			if (haloRenderer != null)
			{
				haloRenderer.color = WithAlpha(baseColor, 0.18f * normalizedAlpha);
			}
		}

		public void Pulse(float duration, float strength)
		{
			pulseTimer = Mathf.Max(pulseTimer, duration);
			pulseDuration = Mathf.Max(pulseDuration, duration);
			pulseStrength = Mathf.Max(pulseStrength, strength);
		}

		public void Flash(Color color, float duration)
		{
			flashColor = Color.Lerp(color, Color.white, 0.3f);
			flashTimer = Mathf.Max(flashTimer, duration);
			if (fillRenderer != null)
			{
				fillRenderer.color = flashColor;
			}
			if (outlineRenderer != null)
			{
				outlineRenderer.color = ProceduralPalette.Outline(flashColor);
			}
		}

		public void PlayExit(float duration)
		{
			fadeDuration = Mathf.Max(0.01f, duration);
			fadeTimer = fadeDuration;
		}

		private void Update()
		{
			float deltaTime = Time.unscaledDeltaTime;
			if (pulseTimer > 0f)
			{
				pulseTimer = Mathf.Max(0f, pulseTimer - deltaTime);
				float progress = pulseDuration > 0f ? pulseTimer / pulseDuration : 0f;
				transform.localScale = Vector3.one * (baseSize * (1f + Mathf.Sin((1f - progress) * Mathf.PI) * pulseStrength));
			}
			else
			{
				transform.localScale = Vector3.one * baseSize;
			}

			if (flashTimer > 0f)
			{
				flashTimer = Mathf.Max(0f, flashTimer - deltaTime);
				if (flashTimer <= 0f)
				{
					SetColor(baseColor);
				}
			}

			if (fadeTimer > 0f)
			{
				fadeTimer = Mathf.Max(0f, fadeTimer - deltaTime);
				SetAlpha(fadeTimer / fadeDuration);
			}
		}

		private static Color WithAlpha(Color color, float alpha)
		{
			color.a = alpha;
			return color;
		}
	}
}
