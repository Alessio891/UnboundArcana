using System.Collections.Generic;
using UnityEngine;

namespace UnboundArcana.Core.Visuals
{
	public static class ProceduralSpriteLibrary
	{
		private const int Resolution = 48;
		private static readonly Dictionary<ProceduralShape, Sprite> sprites = new();

		public static Sprite Get(ProceduralShape shape)
		{
			if (sprites.TryGetValue(shape, out Sprite sprite) && sprite != null)
			{
				return sprite;
			}

			Texture2D texture = new(Resolution, Resolution, TextureFormat.RGBA32, false);
			texture.name = $"Procedural {shape}";
			texture.filterMode = FilterMode.Bilinear;
			texture.wrapMode = TextureWrapMode.Clamp;
			texture.hideFlags = HideFlags.HideAndDontSave;
			Color32[] pixels = new Color32[Resolution * Resolution];

			for (int y = 0; y < Resolution; y++)
			{
				for (int x = 0; x < Resolution; x++)
				{
					float normalizedX = (x + 0.5f) / Resolution - 0.5f;
					float normalizedY = (y + 0.5f) / Resolution - 0.5f;
					float distance = Mathf.Sqrt(normalizedX * normalizedX + normalizedY * normalizedY);
					bool filled = IsFilled(shape, normalizedX, normalizedY, distance);
					pixels[y * Resolution + x] = filled ? new Color32(255, 255, 255, 255) : new Color32(255, 255, 255, 0);
				}
			}

			texture.SetPixels32(pixels);
			texture.Apply(false, true);
			sprite = Sprite.Create(texture, new Rect(0f, 0f, Resolution, Resolution), Vector2.one * 0.5f, Resolution);
			sprite.name = $"Procedural {shape}";
			sprite.hideFlags = HideFlags.HideAndDontSave;
			sprites[shape] = sprite;
			return sprite;
		}

		private static bool IsFilled(ProceduralShape shape, float x, float y, float distance)
		{
			switch (shape)
			{
				case ProceduralShape.Circle:
					return distance <= 0.47f;
				case ProceduralShape.Ring:
					return distance <= 0.47f && distance >= 0.33f;
				case ProceduralShape.Diamond:
					return Mathf.Abs(x) + Mathf.Abs(y) <= 0.46f;
				case ProceduralShape.Triangle:
					float height = y + 0.47f;
					float halfWidth = Mathf.Clamp01(height / 0.94f) * 0.47f;
					return height >= 0f && height <= 0.94f && Mathf.Abs(x) <= halfWidth;
				case ProceduralShape.Hexagon:
					return Mathf.Abs(x) <= 0.47f && Mathf.Abs(y) <= 0.4f && Mathf.Abs(x) * 0.28f + Mathf.Abs(y) <= 0.47f;
				default:
					return Mathf.Abs(x) <= 0.47f && Mathf.Abs(y) <= 0.47f;
			}
		}
	}
}
