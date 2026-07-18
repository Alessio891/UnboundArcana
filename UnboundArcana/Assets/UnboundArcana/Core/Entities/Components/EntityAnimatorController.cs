using UnityEngine;
using UnboundArcana.Core.Entities.Events;

namespace UnboundArcana.Core.Entities
{
	public class EntityAnimatorController : MonoBehaviour
	{
		private Entity entity;
		private Animator animator;

		private static readonly int MoveSpeed =
			Animator.StringToHash("MoveSpeed");

		private static readonly int Hit =
			Animator.StringToHash("Hit");

		private static readonly int Death =
			Animator.StringToHash("Death");

		private void Awake()
		{
			entity = GetComponent<Entity>();
			animator =
				GetComponentInChildren<Animator>();
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
		}

		private void OnFacingChanged(EntityFacingChangedEvent ev) {
			GetComponentInChildren<SpriteRenderer>().flipX = ev.Direction.x < 0;
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
		}

		private void OnDeath(
			EntityDeathEvent evt)
		{
			animator.SetTrigger(Death);
		}
	}
}