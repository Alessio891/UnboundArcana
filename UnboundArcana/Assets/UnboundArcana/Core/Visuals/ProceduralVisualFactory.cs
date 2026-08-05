using UnityEngine;

namespace UnboundArcana.Core.Visuals
{
	public static class ProceduralVisualFactory
	{
		public const string InteractiveSortingLayer = "Interactives";
		public const string EnvironmentSortingLayer = "Background";
		private static Material sharedMaterial;

		public static Material SharedMaterial
		{
			get
			{
				if (sharedMaterial != null)
				{
					return sharedMaterial;
				}

				Shader shader = Shader.Find("UnboundArcana/ProceduralGlow");
				if (shader == null)
				{
					shader = Shader.Find("Sprites/Default");
				}

				sharedMaterial = new Material(shader)
				{
					name = "Procedural Visual Shared Material",
					hideFlags = HideFlags.HideAndDontSave
				};
				return sharedMaterial;
			}
		}

		public static SpriteRenderer CreateRenderer(Transform parent, string name, ProceduralShape shape, Color color, Vector3 position, Vector3 scale, string sortingLayer, int sortingOrder)
		{
			GameObject instance = new(name);
			instance.transform.SetParent(parent, false);
			instance.transform.position = position;
			instance.transform.localScale = scale;
			SpriteRenderer renderer = instance.AddComponent<SpriteRenderer>();
			renderer.sprite = ProceduralSpriteLibrary.Get(shape);
			renderer.color = color;
			renderer.sharedMaterial = SharedMaterial;
			renderer.sortingLayerName = sortingLayer;
			renderer.sortingOrder = sortingOrder;
			return renderer;
		}
	}
}
