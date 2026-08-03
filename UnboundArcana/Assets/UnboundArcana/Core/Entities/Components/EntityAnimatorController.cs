using UnityEngine;
using UnboundArcana.Core.Entities.Events;
using System.Collections;

namespace UnboundArcana.Core.Entities
{
	public class EntityAnimatorController : MonoBehaviour
	{
		private Entity entity;
		private Animator animator;
		private SpriteRenderer spriteRenderer;
		private Coroutine hitFeedbackRoutine;
		private Coroutine deathFeedbackRoutine;
		private Color baseSpriteColor;
		private Vector3 baseSpritePosition;
		private Quaternion baseSpriteRotation;
		private Vector3 baseSpriteScale;
		private Vector2 lastDamageReactionDirection;
		private bool hasDamageReactionDirection;
		private bool deathSequenceStarted;
		private static Material deathParticleMaterial;

		[SerializeField] private Color hitFlashColor = new(1f, 0.3f, 0.2f, 1f);
		[SerializeField] private float hitFlashDuration = 0.08f;
		[SerializeField] private float hitPunchScale = 1.12f;
		[SerializeField] private float deathReactionDuration = 0.14f;
		[SerializeField] private float deathHoldDuration = 0.14f;
		[SerializeField] private float deathFadeDuration = 0.22f;
		[SerializeField] private float deathDisplacement = 0.12f;
		[SerializeField] private float deathRotation = 8f;
		[SerializeField] private Vector2 deathScaleDeformation = new(1.08f, 0.9f);

		private static readonly int MoveSpeed =
			Animator.StringToHash("MoveSpeed");

		private static readonly int Hit =
			Animator.StringToHash("Hit");

		private static readonly int Death =
			Animator.StringToHash("Death");

		private static readonly int Melee =
			Animator.StringToHash("Melee");

		private void Awake()
		{
			entity = GetComponent<Entity>();
			animator =
				GetComponentInChildren<Animator>();
			spriteRenderer = GetComponentInChildren<SpriteRenderer>();
			baseSpriteColor = spriteRenderer.color;
			baseSpritePosition = spriteRenderer.transform.localPosition;
			baseSpriteRotation = spriteRenderer.transform.localRotation;
			baseSpriteScale = spriteRenderer.transform.localScale;
		}

		private void OnEnable()
		{
			deathSequenceStarted = false;
			animator.enabled = true;
			entity.Events.Subscribe<EntityMovementEvent>(
				OnMovement);

			entity.Events.Subscribe<EntityDamagedEvent>(
				OnDamaged);

			entity.Events.Subscribe<EntityDeathEvent>(
				OnDeath);

			entity.Events.Subscribe<EntityFacingChangedEvent>(OnFacingChanged);
			entity.Events.Subscribe<EntityMeleeAttackEvent>(OnMeleeAttack);
		}

		private void OnMeleeAttack(EntityMeleeAttackEvent evt)
		{
			animator.SetTrigger(Melee);
		}

		private void OnDisable()
		{
			entity.Events.Unsubscribe<EntityMovementEvent>(
				OnMovement);

			entity.Events.Unsubscribe<EntityDamagedEvent>(
				OnDamaged);

			entity.Events.Unsubscribe<EntityDeathEvent>(
				OnDeath);
			entity.Events.Unsubscribe<EntityFacingChangedEvent>(OnFacingChanged);
			entity.Events.Unsubscribe<EntityMeleeAttackEvent>(OnMeleeAttack);

			if (hitFeedbackRoutine != null)
			{
				StopCoroutine(hitFeedbackRoutine);
				hitFeedbackRoutine = null;
			}

			if (deathFeedbackRoutine != null)
			{
				StopCoroutine(deathFeedbackRoutine);
				deathFeedbackRoutine = null;
			}

			spriteRenderer.color = baseSpriteColor;
			spriteRenderer.transform.localPosition = baseSpritePosition;
			spriteRenderer.transform.localRotation = baseSpriteRotation;
			spriteRenderer.transform.localScale = baseSpriteScale;
		}

		private void OnFacingChanged(EntityFacingChangedEvent ev) {
			spriteRenderer.flipX = ev.Direction.x < 0;
		}

		private void OnMovement(
			EntityMovementEvent evt)
		{
			animator.SetFloat(
				MoveSpeed,
				evt.Velocity.magnitude);
			//bool flip = evt.Velocity.x < 0;
   //         GetComponentInChildren<SpriteRenderer>().flipX = flip;
        }

		private void OnDamaged(
			EntityDamagedEvent evt)
		{
			if (evt.Damage.Source != null)
			{
				Vector2 reactionDirection = (Vector2)entity.transform.position - (Vector2)evt.Damage.Source.transform.position;

				if (reactionDirection.sqrMagnitude > 0.0001f)
				{
					lastDamageReactionDirection = reactionDirection.normalized;
					hasDamageReactionDirection = true;
				}
			}

			animator.SetTrigger(Hit);

			Entity source = evt.Damage.Source != null ? evt.Damage.Source.GetComponentInParent<Entity>() : null;

			if (source == null || source == entity || !source.CompareTag("Player"))
			{
				return;
			}

			if (hitFeedbackRoutine != null)
			{
				StopCoroutine(hitFeedbackRoutine);
			}

			hitFeedbackRoutine = StartCoroutine(PlayHitFeedback());
		}

		private IEnumerator PlayHitFeedback()
		{
			spriteRenderer.color = hitFlashColor;
			spriteRenderer.transform.localScale = baseSpriteScale * hitPunchScale;
			yield return new WaitForSeconds(hitFlashDuration);
			spriteRenderer.color = baseSpriteColor;
			spriteRenderer.transform.localScale = baseSpriteScale;
			hitFeedbackRoutine = null;
		}

		private void OnDeath(
			EntityDeathEvent evt)
		{
			if (deathSequenceStarted)
			{
				return;
			}

			deathSequenceStarted = true;
			animator.enabled = false;

			if (hitFeedbackRoutine != null)
			{
				StopCoroutine(hitFeedbackRoutine);
				hitFeedbackRoutine = null;
			}

			spriteRenderer.color = baseSpriteColor;
			spriteRenderer.transform.localPosition = baseSpritePosition;
			spriteRenderer.transform.localRotation = baseSpriteRotation;
			spriteRenderer.transform.localScale = baseSpriteScale;
			SpawnDeathParticles();
			deathFeedbackRoutine = StartCoroutine(PlayDeathFeedback());
		}

		private void SpawnDeathParticles()
		{
			if (entity.CompareTag("Player"))
			{
				return;
			}

			GameObject effect = new("Enemy Death Burst");
			effect.transform.position = spriteRenderer.bounds.center;
			ParticleSystem particles = effect.AddComponent<ParticleSystem>();
			particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
			ParticleSystem.MainModule main = particles.main;
			main.duration = 0.25f;
			main.loop = false;
			main.startLifetime = new ParticleSystem.MinMaxCurve(0.28f, 0.5f);
			main.startSpeed = new ParticleSystem.MinMaxCurve(0.9f, 1.8f);
			main.startSize = new ParticleSystem.MinMaxCurve(0.015f, 0.045f);
			main.startRotation = new ParticleSystem.MinMaxCurve(0f, Mathf.PI * 2f);
			main.startColor = new ParticleSystem.MinMaxGradient(new Color(0.15f, 0.25f, 0.12f, 1f), new Color(0.85f, 0.85f, 0.85f, 1f));
			main.gravityModifier = 0.35f;
			main.simulationSpace = ParticleSystemSimulationSpace.World;
			main.stopAction = ParticleSystemStopAction.Destroy;

			ParticleSystem.EmissionModule emission = particles.emission;
			emission.rateOverTime = 0f;
			emission.enabled = false;

			ParticleSystem.ShapeModule shape = particles.shape;
			shape.shapeType = ParticleSystemShapeType.Circle;
			shape.radius = 0.12f;

			ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particles.colorOverLifetime;
			colorOverLifetime.enabled = true;
			Gradient fade = new();
			fade.SetKeys(new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) }, new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 0.55f), new GradientAlphaKey(0f, 1f) });
			colorOverLifetime.color = fade;

			ParticleSystemRenderer particleRenderer = effect.GetComponent<ParticleSystemRenderer>();
			particleRenderer.renderMode = ParticleSystemRenderMode.Stretch;
			particleRenderer.lengthScale = 0.4f;
			particleRenderer.velocityScale = 0.08f;
			particleRenderer.sortingLayerName = "Interactives";
			particleRenderer.sortingOrder = spriteRenderer.sortingOrder + 1;

			if (deathParticleMaterial == null)
			{
				deathParticleMaterial = new Material(Shader.Find("Sprites/Default"));
			}

			particleRenderer.sharedMaterial = deathParticleMaterial;

			particles.Play();
			particles.Emit(14);
		}

		private IEnumerator PlayDeathFeedback()
		{
			Vector3 targetPosition = baseSpritePosition;

			if (hasDamageReactionDirection)
			{
				Transform spriteParent = spriteRenderer.transform.parent;
				Vector3 worldDisplacement = lastDamageReactionDirection * deathDisplacement;
				targetPosition += spriteParent != null ? spriteParent.InverseTransformVector(worldDisplacement) : worldDisplacement;
			}

			float rotationDirection = hasDamageReactionDirection && lastDamageReactionDirection.x < 0f ? 1f : -1f;
			Quaternion targetRotation = baseSpriteRotation * Quaternion.Euler(0f, 0f, deathRotation * rotationDirection);
			Vector3 targetScale = new(baseSpriteScale.x * deathScaleDeformation.x, baseSpriteScale.y * deathScaleDeformation.y, baseSpriteScale.z);
			float reactionTimer = 0f;

			while (reactionTimer < deathReactionDuration)
			{
				reactionTimer += Time.deltaTime;
				float progress = deathReactionDuration > 0f ? Mathf.Clamp01(reactionTimer / deathReactionDuration) : 1f;
				float easedProgress = 1f - Mathf.Pow(1f - progress, 3f);
				spriteRenderer.transform.localPosition = Vector3.Lerp(baseSpritePosition, targetPosition, easedProgress);
				spriteRenderer.transform.localRotation = Quaternion.Lerp(baseSpriteRotation, targetRotation, easedProgress);
				spriteRenderer.transform.localScale = Vector3.Lerp(baseSpriteScale, targetScale, easedProgress);
				yield return null;
			}

			yield return new WaitForSeconds(deathHoldDuration);
			float timer = 0f;

			while (timer < deathFadeDuration)
			{
				timer += Time.deltaTime;
				float progress = deathFadeDuration > 0f ? Mathf.Clamp01(timer / deathFadeDuration) : 1f;
				Color color = baseSpriteColor;
				color.a = Mathf.Lerp(baseSpriteColor.a, 0f, progress);
				spriteRenderer.color = color;
				yield return null;
			}

			deathFeedbackRoutine = null;
		}
	}
}
