using UnityEngine;
using UnboundArcana.Core.Entities.Events;
using UnboundArcana.Core.Stats;

namespace UnboundArcana.Core.Entities
{
	[RequireComponent(typeof(Entity))]
	[RequireComponent(typeof(Rigidbody2D))]
	public class CharacterMotor : MonoBehaviour
	{
		private Entity entity;
		private Rigidbody2D rb;

		private Vector2 moveInput;
		private Vector2 velocity;

		private bool movingToPosition = false;
		private Vector3 targetMoveToPosition;


		private void Awake()
		{
			entity = GetComponent<Entity>();
			rb = GetComponent<Rigidbody2D>();
		}

		public void MoveTo(Vector3 position) {
			movingToPosition = true;
			targetMoveToPosition = position;
			Debug.Log($"Moving to {position}");
		}
		
		public void SetMovementIntent(Vector2 input)
		{
			if (movingToPosition) { return; }

			moveInput = Vector2.ClampMagnitude(input, 1f);
		}

		private void Update()
		{
			if (movingToPosition)
			{
				Vector2 direction = (targetMoveToPosition - transform.position).normalized;
				moveInput = direction;

				if (Vector3.Distance(transform.position, targetMoveToPosition) < 0.1f)
				{
					movingToPosition = false;
					moveInput = Vector2.zero;
					entity.Events.Publish(new EntityMoveToCompleteEvent());
				}
			}
		}

		private void FixedUpdate()
		{
			float moveSpeed = entity.Stats.Get(
				StatKeys.Entity.MoveSpeed);

			velocity =
				moveInput * moveSpeed;

			Vector2 targetPosition =
				rb.position +
				velocity * Time.fixedDeltaTime;

			rb.MovePosition(targetPosition);

			entity.Events.Publish(
				new EntityMovementEvent(
					entity,
					velocity)
			);
		}

		public Vector2 Velocity => velocity;
	}
}