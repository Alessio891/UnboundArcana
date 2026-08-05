using UnityEngine;
using UnboundArcana.Spells.Runtime.Objects;
using UnboundArcana.Core.Visuals;

namespace UnboundArcana.Spells.Runtime.Views
{
	public class BeamView : SpellRuntimeView
	{
		protected override ProceduralShape VisualShape => ProceduralShape.Square;
		protected override bool AddTrail => false;
		protected override bool AddHalo => false;
		protected override bool AddAccent => false;
		protected override bool AddParticles => true;
		protected override int PrimarySortingOrder => -3;
		protected override int ParticleSortingOrder => -2;
		protected override ParticleSystemShapeType ParticleShape => ParticleSystemShapeType.Rectangle;
		protected override float ParticleEmissionRate => 5f;
		protected override float ParticleLifetimeMin => 0.12f;
		protected override float ParticleLifetimeMax => 0.28f;
		protected override float ParticleSpeedMin => 0.02f;
		protected override float ParticleSpeedMax => 0.08f;
		protected override float ParticleSizeMin => 0.01f;
		protected override float ParticleSizeMax => 0.022f;
		protected override float ParticleRadius => 0.5f;

		private BeamRuntimeObject runtimeObject;
		private LineRenderer glowLine;
		private LineRenderer coreLine;
		private LineRenderer pulseLine;
		private Color visualColor;
		private Color accentColor;
		private float animationTime;
		private float localStart;
		private float localEnd;

		protected override void Awake()
		{
			base.Awake();
			ProceduralVisual.SetAlpha(0f);
			Bounds spriteBounds = GetPrimaryRenderer().sprite.bounds;
			localStart = spriteBounds.min.x;
			localEnd = spriteBounds.max.x;
			glowLine = CreateLine("Beam Glow", localStart, localEnd, 0.86f, -2);
			coreLine = CreateLine("Beam Core", localStart, localEnd, 0.2f, -1);
			pulseLine = CreateLine("Beam Pulse", localStart, localStart + 0.1f, 0.34f, 0);
			visualColor = ProceduralPalette.SpellColor(DefaultVisualColor);
			accentColor = ProceduralPalette.SpellColor(DefaultAccentColor);
			ApplyLineColors();
		}

		public void Initialize(BeamRuntimeObject runtimeObject)
		{
			this.runtimeObject = runtimeObject;
			runtimeObject.SetView(this);
			runtimeObject.SyncView();
		}

		public override void ApplyVisualStyle(Color color, Color accentColor)
		{
			base.ApplyVisualStyle(color, accentColor);
			visualColor = ProceduralPalette.SpellColor(color);
			this.accentColor = ProceduralPalette.SpellColor(accentColor);
			ProceduralVisual?.SetAlpha(0f);
			ApplyLineColors();
		}

		private LineRenderer CreateLine(string name, float start, float end, float width, int sortingOrder)
		{
			GameObject instance = new(name);
			instance.transform.SetParent(transform, false);
			LineRenderer line = instance.AddComponent<LineRenderer>();
			line.useWorldSpace = false;
			line.positionCount = 2;
			line.SetPosition(0, new Vector3(start, 0f, 0f));
			line.SetPosition(1, new Vector3(end, 0f, 0f));
			line.startWidth = width;
			line.endWidth = width * 0.72f;
			line.numCapVertices = 2;
			line.numCornerVertices = 2;
			line.textureMode = LineTextureMode.Stretch;
			line.sharedMaterial = ProceduralVisualFactory.SharedMaterial;
			line.sortingLayerName = ProceduralVisualFactory.InteractiveSortingLayer;
			line.sortingOrder = sortingOrder;
			return line;
		}

		private void ApplyLineColors()
		{
			if (glowLine == null || coreLine == null)
			{
				return;
			}

			Color glowColor = visualColor;
			glowColor.a *= 0.18f;
			glowLine.startColor = glowColor;
			glowLine.endColor = new Color(glowColor.r, glowColor.g, glowColor.b, 0f);
			coreLine.startColor = accentColor;
			coreLine.endColor = new Color(accentColor.r, accentColor.g, accentColor.b, 0.15f);
			Color pulseColor = Color.Lerp(accentColor, Color.white, 0.35f);
			pulseColor.a *= 0.95f;
			pulseLine.startColor = pulseColor;
			pulseLine.endColor = new Color(pulseColor.r, pulseColor.g, pulseColor.b, 0f);
		}

		private void Update()
		{
			animationTime += Time.unscaledDeltaTime;
			float pulse = 0.88f + Mathf.Sin(animationTime * 14f) * 0.12f;
			if (glowLine != null)
			{
				glowLine.startWidth = 0.86f * pulse;
				glowLine.endWidth = 0.62f * pulse;
			}
			if (coreLine != null)
			{
				coreLine.startWidth = 0.2f * (0.94f + Mathf.Sin(animationTime * 18f) * 0.06f);
				coreLine.endWidth = 0.12f * pulse;
				Color animatedAccent = accentColor;
				animatedAccent.a *= 0.86f + Mathf.Sin(animationTime * 16f) * 0.14f;
				coreLine.startColor = animatedAccent;
				coreLine.endColor = new Color(animatedAccent.r, animatedAccent.g, animatedAccent.b, 0.08f);
			}
			if (pulseLine != null)
			{
				float pulseLength = 0.1f;
				float pulseCenter = Mathf.Lerp(localStart, localEnd, Mathf.Repeat(animationTime * 0.9f, 1f));
				pulseLine.SetPosition(0, new Vector3(pulseCenter - pulseLength * 0.5f, 0f, 0f));
				pulseLine.SetPosition(1, new Vector3(pulseCenter + pulseLength * 0.5f, 0f, 0f));
				Color pulseColor = Color.Lerp(accentColor, Color.white, 0.35f);
				pulseColor.a *= 0.82f + Mathf.Sin(animationTime * 10f) * 0.12f;
				pulseLine.startWidth = 0.34f;
				pulseLine.endWidth = 0.08f;
				pulseLine.startColor = pulseColor;
				pulseLine.endColor = new Color(pulseColor.r, pulseColor.g, pulseColor.b, 0f);
			}
		}
	}
}
