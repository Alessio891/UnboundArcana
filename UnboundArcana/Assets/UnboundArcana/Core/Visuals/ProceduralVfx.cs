using System.Collections.Generic;
using UnityEngine;

namespace UnboundArcana.Core.Visuals
{
	public static class ProceduralVfx
	{
		public const int MaxActiveEffects = 48;
		public const int MaxParticlesPerBurst = 24;
		private static readonly List<ProceduralVfxEffect> activeEffects = new();

		public static ProceduralImpactEffect SpawnImpact(Vector3 position, Color color, float intensity = 1f)
		{
			TrimEffects();
			GameObject instance = new("Procedural Impact");
			instance.transform.position = position;
			ProceduralImpactEffect effect = instance.AddComponent<ProceduralImpactEffect>();
			effect.Initialize(color, intensity);
			Register(effect);
			return effect;
		}

		public static ProceduralTelegraphEffect SpawnTelegraph(Transform target, Color color, float duration)
		{
			TrimEffects();
			GameObject instance = new("Procedural Telegraph");
			ProceduralTelegraphEffect effect = instance.AddComponent<ProceduralTelegraphEffect>();
			effect.Initialize(target, color, duration);
			Register(effect);
			return effect;
		}

		private static void Register(ProceduralVfxEffect effect)
		{
			activeEffects.Add(effect);
		}

		internal static void Unregister(ProceduralVfxEffect effect)
		{
			activeEffects.Remove(effect);
		}

		private static void TrimEffects()
		{
			for (int i = activeEffects.Count - 1; i >= 0; i--)
			{
				if (activeEffects[i] == null)
				{
					activeEffects.RemoveAt(i);
				}
			}

			while (activeEffects.Count >= MaxActiveEffects)
			{
				ProceduralVfxEffect effect = activeEffects[0];
				activeEffects.RemoveAt(0);
				if (effect != null)
				{
					effect.StopEffect();
				}
			}
		}
	}

	public abstract class ProceduralVfxEffect : MonoBehaviour
	{
		private bool isStopping;

		internal abstract void StopEffect();

		protected void DestroyEffect()
		{
			if (!this || isStopping)
			{
				return;
			}

			isStopping = true;
			gameObject.SetActive(false);
			Destroy(gameObject);
		}

		protected virtual void OnDestroy()
		{
			ProceduralVfx.Unregister(this);
		}
	}

	public sealed class ProceduralImpactEffect : ProceduralVfxEffect
	{
		private ProceduralShapeVisual ring;
		private ProceduralShapeVisual core;
		private float elapsed;
		private float duration;

		internal void Initialize(Color color, float intensity)
		{
			float safeIntensity = Mathf.Clamp(intensity, 0.5f, 2f);
			duration = 0.18f + safeIntensity * 0.06f;
			ring = ProceduralShapeVisual.Create(transform, "Ring", ProceduralShape.Ring, color, 0.16f, 30, true);
			core = ProceduralShapeVisual.Create(transform, "Core", ProceduralShape.Diamond, ProceduralPalette.Damage, 0.12f, 32);
			ring.Pulse(duration, 0.08f);
			ParticleSystem particles = gameObject.AddComponent<ParticleSystem>();
			particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
			ParticleSystem.MainModule main = particles.main;
			main.playOnAwake = false;
			main.duration = duration;
			main.loop = false;
			main.startLifetime = new ParticleSystem.MinMaxCurve(0.12f, 0.24f);
			main.startSpeed = new ParticleSystem.MinMaxCurve(0.6f, 1.4f * safeIntensity);
			main.startSize = new ParticleSystem.MinMaxCurve(0.018f, 0.04f * safeIntensity);
			main.startColor = color;
			main.simulationSpace = ParticleSystemSimulationSpace.World;
			main.maxParticles = ProceduralVfx.MaxParticlesPerBurst;
			ParticleSystem.EmissionModule emission = particles.emission;
			emission.rateOverTime = 0f;
			ParticleSystem.ShapeModule shape = particles.shape;
			shape.shapeType = ParticleSystemShapeType.Circle;
			shape.radius = 0.06f * safeIntensity;
			ParticleSystemRenderer renderer = particles.GetComponent<ParticleSystemRenderer>();
			renderer.sharedMaterial = ProceduralVisualFactory.SharedMaterial;
			renderer.sortingLayerName = ProceduralVisualFactory.InteractiveSortingLayer;
			renderer.sortingOrder = 31;
			particles.Play();
			particles.Emit(Mathf.Clamp(Mathf.RoundToInt(10f * safeIntensity), 6, ProceduralVfx.MaxParticlesPerBurst));
		}

		private void Update()
		{
			elapsed += Time.unscaledDeltaTime;
			float progress = Mathf.Clamp01(elapsed / duration);
			ring.SetScale(Mathf.Lerp(0.16f, 0.72f, progress));
			ring.SetAlpha(1f - progress);
			core.SetScale(Mathf.Lerp(0.12f, 0.02f, progress));
			core.SetAlpha(1f - progress);
			if (progress >= 1f)
			{
				DestroyEffect();
			}
		}

		internal override void StopEffect()
		{
			DestroyEffect();
		}
	}

	public sealed class ProceduralTelegraphEffect : ProceduralVfxEffect
	{
		private ProceduralShapeVisual ring;
		private Transform target;
		private float duration;
		private float elapsed;
		private float chargeProgress;
		private bool externallyDriven;

		internal void Initialize(Transform target, Color color, float duration)
		{
			this.target = target;
			this.duration = Mathf.Max(0.05f, duration);
			ring = ProceduralShapeVisual.Create(transform, "Charge Ring", ProceduralShape.Ring, color, 0.42f, 25, true);
		}

		public void SetChargeProgress(float progress)
		{
			externallyDriven = true;
			chargeProgress = Mathf.Clamp01(progress);
		}

		public void Cancel()
		{
			DestroyEffect();
		}

		private void Update()
		{
			if (target == null)
			{
				DestroyEffect();
				return;
			}

			transform.position = target.position;
			elapsed += Time.unscaledDeltaTime;
			float progress = Mathf.Clamp01(elapsed / duration);
			float wave = 0.5f + Mathf.Sin(elapsed * 12f) * 0.5f;
			float chargeScale = Mathf.Lerp(0.3f, 0.58f, chargeProgress);
			ring.SetScale(chargeScale * Mathf.Lerp(0.88f, 1.12f, wave));
			ring.SetAlpha(Mathf.Lerp(0.18f, 0.78f, chargeProgress) * Mathf.Lerp(0.78f, 1.1f, wave));

			if (!externallyDriven && progress >= 1f)
			{
				DestroyEffect();
			}
		}

		internal override void StopEffect()
		{
			DestroyEffect();
		}
	}
}
