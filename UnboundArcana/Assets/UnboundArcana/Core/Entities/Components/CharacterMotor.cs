using UnityEngine;
using UnboundArcana.Core.Entities.Events;

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

		private void Awake()
		{
			entity = GetComponent<Entity>();
			rb = GetComponent<Rigidbody2D>();
		}

		public void SetMovementIntent(Vector2 input)
		{
			moveInput = Vector2.ClampMagnitude(input, 1f);
		}

		private void FixedUpdate()
		{
			float moveSpeed = entity.Stats.Get(
				EntityStatId.MoveSpeed);

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