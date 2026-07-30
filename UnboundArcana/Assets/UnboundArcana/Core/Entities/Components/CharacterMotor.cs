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
		private float speedMultiplier = 1f;
		private float acceleration = float.PositiveInfinity;
		private float deceleration = float.PositiveInfinity;

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

		public void SetSpeedMultiplier(float multiplier)
		{
			speedMultiplier = Mathf.Max(0f, multiplier);
		}

		public void SetMovementSmoothing(float accelerationRate, float decelerationRate)
		{
			acceleration = accelerationRate > 0f ? accelerationRate : float.PositiveInfinity;
			deceleration = decelerationRate > 0f ? decelerationRate : float.PositiveInfinity;
		}

		public void StopImmediately()
		{
			moveInput = Vector2.zero;
			velocity = Vector2.zero;
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

			Vector2 desiredVelocity = moveInput * moveSpeed * speedMultiplier;
			float changeRate = moveInput.sqrMagnitude > 0f ? acceleration : deceleration;
			velocity = Vector2.MoveTowards(velocity, desiredVelocity, changeRate * Time.fixedDeltaTime);

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
