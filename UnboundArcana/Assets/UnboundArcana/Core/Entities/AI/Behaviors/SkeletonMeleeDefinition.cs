using UnityEngine;

namespace UnboundArcana.Core.Entities.AI
{
	[CreateAssetMenu(menuName = "Unbound Arcana/AI/Skeleton Melee")]
	public class SkeletonMeleeDefinition : AIBehaviorDefinition
	{
		[SerializeField] private float attackRange = 0.6f;
		[SerializeField] private float resumeChaseRange = 0.75f;
		[SerializeField] private float pursuitPredictionTime = 0.35f;
		[SerializeField] private float maximumPursuitLead = 0.65f;
		[SerializeField] private float attackAdvanceDuration = 0.22f;
		[SerializeField] private float attackAdvanceSpeedMultiplier = 1.35f;
		[SerializeField] private float recoveryDuration = 0.35f;

		public float AttackRange => attackRange;
		public float ResumeChaseRange => Mathf.Max(resumeChaseRange, attackRange);
		public float PursuitPredictionTime => pursuitPredictionTime;
		public float MaximumPursuitLead => maximumPursuitLead;
		public float AttackAdvanceDuration => attackAdvanceDuration;
		public float AttackAdvanceSpeedMultiplier => attackAdvanceSpeedMultiplier;
		public float RecoveryDuration => recoveryDuration;

		public override AIBehavior CreateBehavior()
		{
			return new SkeletonMeleeBehavior(this);
		}
	}

	public class SkeletonMeleeBehavior : AIBehavior
	{
		private readonly SkeletonMeleeDefinition definition;
		private MeleeAttacker attacker;
		private Vector2 previousTargetPosition;
		private Vector2 attackDirection;
		private float attackAdvanceTimer;
		private float recoveryTimer;
		private bool hasPreviousTargetPosition;
		private bool holdingAttackRange;
		private bool attackInProgress;

		public SkeletonMeleeBehavior(SkeletonMeleeDefinition definition)
		{
			this.definition = definition;
		}

		protected override void OnInitialize()
		{
			attacker = Controller.GetComponent<MeleeAttacker>();
		}

		protected override void OnTick()
		{
			if (!Controller.TryGetPerceivedTargetPosition(out Vector2 targetPosition))
			{
				ResetMovement();
				hasPreviousTargetPosition = false;
				return;
			}

			Vector2 toTarget = targetPosition - (Vector2)Controller.transform.position;
			Controller.FacingDirection.SetDirection(toTarget);
			UpdateTargetTracking(targetPosition);

			if (attacker != null && attacker.IsAttacking)
			{
				AdvanceCommittedAttack();
				return;
			}

			if (attackInProgress)
			{
				attackInProgress = false;
				attackAdvanceTimer = 0f;
				recoveryTimer = definition.RecoveryDuration;
			}

			if (recoveryTimer > 0f)
			{
				recoveryTimer -= Time.deltaTime;
				ResetMovement();
				return;
			}

			float chaseThreshold = holdingAttackRange ? definition.ResumeChaseRange : definition.AttackRange;

			if (toTarget.magnitude > chaseThreshold)
			{
				holdingAttackRange = false;
				Vector2 pursuitTarget = targetPosition + EstimateTargetLead(targetPosition);
				Controller.Movement.SetSpeedMultiplier(1f);
				Controller.SetMovementIntent((pursuitTarget - (Vector2)Controller.transform.position).normalized);
				return;
			}

			holdingAttackRange = true;
			ResetMovement();

			if (Controller.TargetVisible && attacker != null)
			{
				attackDirection = toTarget.normalized;
				attacker.PerformMeleeAttack();

				if (attacker.IsAttacking)
				{
					attackInProgress = true;
					attackAdvanceTimer = definition.AttackAdvanceDuration;
				}
			}
		}

		private void UpdateTargetTracking(Vector2 targetPosition)
		{
			if (!Controller.TargetVisible)
			{
				hasPreviousTargetPosition = false;
				return;
			}

			if (!hasPreviousTargetPosition)
			{
				previousTargetPosition = targetPosition;
				hasPreviousTargetPosition = true;
			}
		}

		private Vector2 EstimateTargetLead(Vector2 targetPosition)
		{
			if (!Controller.TargetVisible || !hasPreviousTargetPosition || Time.deltaTime <= 0f)
			{
				return Vector2.zero;
			}

			Vector2 targetVelocity = (targetPosition - previousTargetPosition) / Time.deltaTime;
			previousTargetPosition = targetPosition;
			return Vector2.ClampMagnitude(targetVelocity * definition.PursuitPredictionTime, definition.MaximumPursuitLead);
		}

		private void AdvanceCommittedAttack()
		{
			attackAdvanceTimer -= Time.deltaTime;
			Controller.Movement.SetSpeedMultiplier(definition.AttackAdvanceSpeedMultiplier);
			Controller.SetMovementIntent(attackAdvanceTimer > 0f ? attackDirection : Vector2.zero, false);
		}

		private void ResetMovement()
		{
			Controller.Movement.SetSpeedMultiplier(1f);
			Controller.SetMovementIntent(Vector2.zero);
		}
	}
}
