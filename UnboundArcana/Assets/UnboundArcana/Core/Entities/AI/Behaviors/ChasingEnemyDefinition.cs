using UnityEngine;

namespace UnboundArcana.Core.Entities.AI
{
	[CreateAssetMenu(
		menuName =
		"Unbound Arcana/AI/Chasing Enemy"
	)]
	public class ChasingEnemyDefinition
		: AIBehaviorDefinition
	{
		[SerializeField] private float stoppingRange = 0.45f;
		[SerializeField] private float resumeChaseRange = 0.65f;
		[SerializeField] private float recoveryDuration = 0.25f;
		public float StoppingRange => stoppingRange;
		public float ResumeChaseRange => Mathf.Max(resumeChaseRange, stoppingRange);
		public float RecoveryDuration => recoveryDuration;

		public override AIBehavior CreateBehavior()
		{
			return new ChasingEnemyBehavior(this);
		}
	}

	public class ChasingEnemyBehavior : AIBehavior
	{
		private readonly ChasingEnemyDefinition definition;
		private MeleeAttacker attacker;
		private float recoveryTimer;
		private bool holdingAttackRange;

		public ChasingEnemyBehavior(ChasingEnemyDefinition definition)
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
				Stop();
				return;
			}

			Vector2 direction = targetPosition - (Vector2)Controller.transform.position;
			float distance = direction.magnitude;
			Controller.FacingDirection.SetDirection(direction);

			if (attacker != null && attacker.IsAttacking)
			{
				Stop();
				return;
			}

			if (recoveryTimer > 0f)
			{
				recoveryTimer -= Time.deltaTime;
				Stop();
				return;
			}

			float chaseThreshold = holdingAttackRange ? definition.ResumeChaseRange : definition.StoppingRange;

			if (distance > chaseThreshold)
			{
				holdingAttackRange = false;
				Controller.SetMovementIntent(direction.normalized);
				return;
			}

			holdingAttackRange = true;
			Stop();

			if (Controller.TargetVisible && attacker != null)
			{
				attacker.PerformMeleeAttack();
				recoveryTimer = definition.RecoveryDuration;
			}
		}

		private void Stop()
		{
			Controller.SetMovementIntent(Vector2.zero);
		}
	}
}
