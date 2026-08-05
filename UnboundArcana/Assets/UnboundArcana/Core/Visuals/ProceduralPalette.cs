using UnityEngine;

namespace UnboundArcana.Core.Visuals
{
	public static class ProceduralPalette
	{
		public static readonly Color Ink = new(0.035f, 0.047f, 0.09f, 1f);
		public static readonly Color Player = new(0.38f, 0.68f, 0.78f, 1f);
		public static readonly Color PlayerBody = new(0.06f, 0.15f, 0.21f, 1f);
		public static readonly Color PlayerAccent = new(0.7f, 0.92f, 0.95f, 1f);
		public static readonly Color Enemy = new(0.56f, 0.17f, 0.25f, 1f);
		public static readonly Color EnemyBody = new(0.16f, 0.06f, 0.12f, 1f);
		public static readonly Color EnemyAccent = new(0.82f, 0.56f, 0.24f, 1f);
		public static readonly Color Arcane = new(0.56f, 0.45f, 0.82f, 1f);
		public static readonly Color ArcaneAccent = new(0.86f, 0.78f, 1f, 1f);
		public static readonly Color Fire = new(1f, 0.32f, 0.08f, 1f);
		public static readonly Color FireAccent = new(1f, 0.78f, 0.28f, 1f);
		public static readonly Color Ice = new(0.34f, 0.78f, 1f, 1f);
		public static readonly Color IceAccent = new(0.84f, 0.97f, 1f, 1f);
		public static readonly Color Acid = new(0.5f, 1f, 0.15f, 1f);
		public static readonly Color AcidAccent = new(0.84f, 1f, 0.48f, 1f);
		public static readonly Color Air = new(0.72f, 0.9f, 1f, 1f);
		public static readonly Color AirAccent = new(0.94f, 1f, 1f, 1f);
		public static readonly Color Earth = new(0.65f, 0.45f, 0.2f, 1f);
		public static readonly Color EarthAccent = new(0.9f, 0.72f, 0.42f, 1f);
		public static readonly Color Lightning = new(0.7f, 0.85f, 1f, 1f);
		public static readonly Color LightningAccent = new(1f, 0.95f, 0.56f, 1f);
		public static readonly Color Water = new(0.2f, 0.65f, 1f, 1f);
		public static readonly Color WaterAccent = new(0.58f, 0.9f, 1f, 1f);
		public static readonly Color Charging = new(1f, 0.82f, 0.35f, 1f);
		public static readonly Color Overload = new(1f, 0.28f, 0.58f, 1f);
		public static readonly Color Damage = new(1f, 0.73f, 0.36f, 1f);
		public static readonly Color EnvironmentFloor = new(0.055f, 0.105f, 0.16f, 1f);
		public static readonly Color EnvironmentWall = new(0.14f, 0.25f, 0.35f, 1f);
		public static readonly Color EnvironmentAccent = new(0.18f, 0.48f, 0.58f, 1f);

		public static Color Outline(Color color)
		{
			return Color.Lerp(Ink, color, 0.18f);
		}

		public static Color SpellColor(Color requested)
		{
			if (requested == Color.white || requested.a <= 0f)
			{
				return Arcane;
			}

			return requested;
		}

		public static Color SpellAccent(Color requested)
		{
			if (requested == Color.white || requested.a <= 0f)
			{
				return ArcaneAccent;
			}

			return Color.Lerp(requested, Color.white, 0.42f);
		}
	}
}
