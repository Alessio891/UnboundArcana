using UnityEngine;
using UnboundArcana.Core.Visuals;

namespace UnboundArcana.Spells.Runtime.Views
{
	public abstract class SpellRuntimeView : MonoBehaviour
	{
		private ProceduralShapeVisual proceduralVisual;
		private ProceduralShapeVisual accentVisual;
		private TrailRenderer trailRenderer;
		private ParticleSystem particleSystem;

		public ProceduralShapeVisual ProceduralVisual => proceduralVisual;

		protected virtual ProceduralShape VisualShape => ProceduralShape.Circle;
		protected virtual ProceduralShape AccentShape => VisualShape;
		protected virtual float AccentScale => 0.52f;
		protected virtual Color DefaultVisualColor => ProceduralPalette.Arcane;
		protected virtual Color DefaultAccentColor => ProceduralPalette.ArcaneAccent;
		protected virtual bool AddTrail => false;
		protected virtual bool AddHalo => true;
		protected virtual bool AddAccent => true;
		protected virtual bool AddParticles => false;
		protected virtual int PrimarySortingOrder => 10;
		protected virtual int AccentSortingOrder => 12;
		protected virtual int ParticleSortingOrder => 11;
		protected virtual ParticleSystemShapeType ParticleShape => ParticleSystemShapeType.Circle;
		protected virtual int ParticleBurstCount => 0;
		protected virtual float ParticleEmissionRate => 10f;
		protected virtual float ParticleLifetimeMin => 0.2f;
		protected virtual float ParticleLifetimeMax => 0.48f;
		protected virtual float ParticleSpeedMin => 0.04f;
		protected virtual float ParticleSpeedMax => 0.18f;
		protected virtual float ParticleSizeMin => 0.018f;
		protected virtual float ParticleSizeMax => 0.04f;
		protected virtual float ParticleRadius => 0.42f;

		protected virtual void Awake()
		{
			foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>(true))
			{
				renderer.enabled = false;
			}

			proceduralVisual = ProceduralShapeVisual.Create(transform, "Procedural Spell", VisualShape, DefaultVisualColor, 1f, PrimarySortingOrder, AddHalo);
			if (AddAccent)
			{
				accentVisual = ProceduralShapeVisual.Create(transform, "Procedural Accent", AccentShape, DefaultAccentColor, AccentScale, AccentSortingOrder);
			}

			if (AddTrail)
			{
				trailRenderer = gameObject.AddComponent<TrailRenderer>();
				trailRenderer.time = 0.08f;
				trailRenderer.startWidth = 0.09f;
				trailRenderer.endWidth = 0f;
				trailRenderer.startColor = DefaultVisualColor;
				trailRenderer.endColor = new Color(DefaultVisualColor.r, DefaultVisualColor.g, DefaultVisualColor.b, 0f);
				trailRenderer.sharedMaterial = ProceduralVisualFactory.SharedMaterial;
				trailRenderer.sortingLayerName = ProceduralVisualFactory.InteractiveSortingLayer;
				trailRenderer.sortingOrder = 9;
			}

			if (AddParticles)
			{
				particleSystem = gameObject.AddComponent<ParticleSystem>();
				particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
				ParticleSystem.MainModule main = particleSystem.main;
				main.playOnAwake = false;
				main.loop = ParticleBurstCount <= 0;
				main.startLifetime = new ParticleSystem.MinMaxCurve(ParticleLifetimeMin, ParticleLifetimeMax);
				main.startSpeed = new ParticleSystem.MinMaxCurve(ParticleSpeedMin, ParticleSpeedMax);
				main.startSize = new ParticleSystem.MinMaxCurve(ParticleSizeMin, ParticleSizeMax);
				main.startColor = new ParticleSystem.MinMaxGradient(DefaultVisualColor, DefaultAccentColor);
				main.simulationSpace = ParticleSystemSimulationSpace.Local;
				main.maxParticles = 36;
				ParticleSystem.EmissionModule emission = particleSystem.emission;
				emission.rateOverTime = ParticleBurstCount > 0 ? 0f : ParticleEmissionRate;
				ParticleSystem.ShapeModule shape = particleSystem.shape;
				shape.shapeType = ParticleShape;
				shape.radius = ParticleRadius;
				ParticleSystemRenderer renderer = particleSystem.GetComponent<ParticleSystemRenderer>();
				renderer.sharedMaterial = ProceduralVisualFactory.SharedMaterial;
				renderer.sortingLayerName = ProceduralVisualFactory.InteractiveSortingLayer;
				renderer.sortingOrder = ParticleSortingOrder;
				particleSystem.Play();
				if (ParticleBurstCount > 0)
				{
					particleSystem.Emit(ParticleBurstCount);
				}
			}
		}

		public SpriteRenderer GetPrimaryRenderer()
		{
			return proceduralVisual != null ? proceduralVisual.FillRenderer : GetComponentInChildren<SpriteRenderer>(true);
		}

		public void ApplyVisualColor(Color color)
		{
			ApplyVisualStyle(color, ProceduralPalette.SpellAccent(color));
		}

		public virtual void ApplyVisualStyle(Color color, Color accentColor)
		{
			Color resolvedColor = ProceduralPalette.SpellColor(color);
			Color resolvedAccent = ProceduralPalette.SpellColor(accentColor);
			proceduralVisual?.SetColor(resolvedColor);
			accentVisual?.SetColor(resolvedAccent);
			if (trailRenderer != null)
			{
				trailRenderer.startColor = resolvedColor;
				trailRenderer.endColor = new Color(resolvedColor.r, resolvedColor.g, resolvedColor.b, 0f);
			}
			if (particleSystem != null)
			{
				ParticleSystem.MainModule main = particleSystem.main;
				main.startColor = new ParticleSystem.MinMaxGradient(resolvedColor, resolvedAccent);
			}
		}

		public virtual void DestroyView()
		{
			proceduralVisual?.PlayExit(0.12f);
			Destroy(gameObject);
		}
	}
}
