using UnityEngine;

namespace UnboundArcana.Core.Entities.AI
{
	[CreateAssetMenu(menuName = "Unbound Arcana/AI/Kiting Caster")]
	public class KitingCasterDefinition : AIBehaviorDefinition
	{
		[SerializeField] private float minimumRange = 2.5f;
		[SerializeField] private float preferredRange = 4f;
		[SerializeField] private float maximumRange = 5.5f;
		[SerializeField] private float directionChangeInterval = 1.5f;
		public float MinimumRange => minimumRange;
		public float PreferredRange => Mathf.Max(preferredRange, minimumRange);
		public float MaximumRange => Mathf.Max(maximumRange, PreferredRange);
		public float DirectionChangeInterval => directionChangeInterval;

		public override AIBehavior CreateBehavior()
		{
			return new KitingCasterBehavior(this);
		}
	}

	public class KitingCasterBehavior : AIBehavior
	{
		private readonly KitingCasterDefinition definition;
		private float directionTimer;
		private float orbitSign = 1f;

		public KitingCasterBehavior(KitingCasterDefinition definition)
		{
			this.definition = definition;
		}

		protected override void OnTick()
		{
			if (!Controller.TryGetPerceivedTargetPosition(out Vector2 targetPosition))
			{
				Controller.SetMovementIntent(Vector2.zero);
				return;
			}

			Vector2 toTarget = targetPosition - (Vector2)Controller.transform.position;
			float distance = toTarget.magnitude;
			Vector2 direction = toTarget.normalized;
			Controller.FacingDirection.SetDirection(direction);
			UpdateOrbitDirection();

			if (!Controller.TargetVisible)
			{
				Controller.SetMovementIntent(distance > 0.1f ? direction : Vector2.zero);
				return;
			}

			Controller.Caster.SetAimDirection(direction);

			if (distance < definition.MinimumRange)
			{
				Controller.SetMovementIntent(-direction);
				return;
			}

			if (distance > definition.MaximumRange)
			{
				Controller.SetMovementIntent(direction);
				return;
			}

			Vector2 tangent = new Vector2(-direction.y, direction.x) * orbitSign;
			float radialCorrection = Mathf.Clamp((distance - definition.PreferredRange) / definition.PreferredRange, -0.5f, 0.5f);
			Controller.SetMovementIntent((tangent - direction * radialCorrection).normalized);
			Controller.Caster.BeginCast();
		}

		private void UpdateOrbitDirection()
		{
			directionTimer -= Time.deltaTime;

			if (directionTimer > 0f)
			{
				return;
			}

			orbitSign = Random.value < 0.5f ? -1f : 1f;
			directionTimer = definition.DirectionChangeInterval;
		}
	}
}
