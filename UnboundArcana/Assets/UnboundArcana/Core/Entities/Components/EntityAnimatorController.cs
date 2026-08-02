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
		private Color baseSpriteColor;
		private Vector3 baseSpriteScale;

		[SerializeField] private Color hitFlashColor = new(1f, 0.3f, 0.2f, 1f);
		[SerializeField] private float hitFlashDuration = 0.08f;
		[SerializeField] private float hitPunchScale = 1.12f;

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
			baseSpriteScale = spriteRenderer.transform.localScale;
		}

		private void OnEnable()
		{
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

			spriteRenderer.color = baseSpriteColor;
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
			animator.SetTrigger(Death);
		}
	}
}
