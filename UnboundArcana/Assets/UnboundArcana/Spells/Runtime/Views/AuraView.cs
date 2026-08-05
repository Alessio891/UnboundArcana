using UnityEngine;
using UnboundArcana.Spells.Runtime.Objects;
using UnboundArcana.Core.Visuals;

namespace UnboundArcana.Spells.Runtime.Views
{
	public class AuraView : SpellRuntimeView
	{
		protected override ProceduralShape VisualShape => ProceduralShape.Ring;
		protected override bool AddTrail => false;
		protected override bool AddHalo => false;
		protected override bool AddAccent => false;
		protected override bool AddParticles => true;
		protected override float ParticleEmissionRate => 6f;
		protected override float ParticleLifetimeMin => 0.35f;
		protected override float ParticleLifetimeMax => 0.65f;
		protected override float ParticleSpeedMin => 0.01f;
		protected override float ParticleSpeedMax => 0.06f;
		protected override float ParticleSizeMin => 0.016f;
		protected override float ParticleSizeMax => 0.03f;
		protected override float ParticleRadius => 0.47f;

		private const int AuraMoteCount = 8;
		private const float AuraMoteRadius = 0.43f;
		private const float AuraMoteSize = 0.055f;
		private ProceduralShapeVisual[] auraMotes;
		private float animationTime;
		private float moteAlpha = 0.72f;

		private AuraRuntimeObject runtimeObject;

		protected override void Awake()
		{
			base.Awake();
			auraMotes = new ProceduralShapeVisual[AuraMoteCount];
			for (int i = 0; i < AuraMoteCount; i++)
			{
				ProceduralShape shape = i % 3 == 0 ? ProceduralShape.Diamond : ProceduralShape.Circle;
				auraMotes[i] = ProceduralShapeVisual.Create(transform, "Aura Mote", shape, DefaultAccentColor, AuraMoteSize, 14);
				auraMotes[i].SetAlpha(moteAlpha);
			}
		}

		public void Initialize(AuraRuntimeObject runtimeObject)
		{
			this.runtimeObject = runtimeObject;
			runtimeObject.SetView(this);
			runtimeObject.SyncView();
		}

		public override void ApplyVisualStyle(Color color, Color accentColor)
		{
			base.ApplyVisualStyle(color, accentColor);
			ProceduralVisual?.SetAlpha(0.52f);
			Color resolvedAccent = ProceduralPalette.SpellColor(accentColor);
			for (int i = 0; i < auraMotes.Length; i++)
			{
				auraMotes[i].SetColor(resolvedAccent);
				auraMotes[i].SetAlpha(moteAlpha);
			}
		}

		private void Update()
		{
			if (auraMotes == null)
			{
				return;
			}

			animationTime += Time.unscaledDeltaTime;
			for (int i = 0; i < auraMotes.Length; i++)
			{
				float phase = i * Mathf.PI * 2f / AuraMoteCount;
				float angle = phase + animationTime * (0.72f + i % 2 * 0.18f);
				float radius = AuraMoteRadius + Mathf.Sin(animationTime * 1.7f + phase) * 0.035f;
				Transform moteTransform = auraMotes[i].transform;
				moteTransform.localPosition = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
				float pulse = 0.72f + Mathf.Sin(animationTime * 2.4f + phase) * 0.28f;
				auraMotes[i].SetScale(AuraMoteSize * (0.82f + pulse * 0.28f));
				auraMotes[i].SetAlpha(moteAlpha * (0.55f + pulse * 0.45f));
			}
			ProceduralVisual?.SetAlpha(0.48f + Mathf.Sin(animationTime * 2.1f) * 0.05f);
		}
	}
}
