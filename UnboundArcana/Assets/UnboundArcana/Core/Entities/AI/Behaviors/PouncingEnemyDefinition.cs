using UnityEngine;

namespace UnboundArcana.Core.Entities.AI
{
	[CreateAssetMenu(menuName = "Unbound Arcana/AI/Pouncing Enemy")]
	public class PouncingEnemyDefinition : AIBehaviorDefinition
	{
		[SerializeField] private float preferredRange = 2.25f;
		[SerializeField] private float retreatRange = 0.8f;
		[SerializeField] private float telegraphDuration = 0.45f;
		[SerializeField] private float lungeDuration = 0.35f;
		[SerializeField] private float lungeSpeedMultiplier = 3f;
		[SerializeField] private float landingRange = 0.85f;
		[SerializeField] private float impactRange = 1.1f;
		[SerializeField] private float recoveryDuration = 0.9f;
		public float PreferredRange => preferredRange;
		public float RetreatRange => retreatRange;
		public float TelegraphDuration => telegraphDuration;
		public float LungeDuration => lungeDuration;
		public float LungeSpeedMultiplier => lungeSpeedMultiplier;
		public float LandingRange => landingRange;
		public float ImpactRange => Mathf.Max(impactRange, landingRange);
		public float RecoveryDuration => recoveryDuration;

		public override AIBehavior CreateBehavior()
		{
			return new PouncingEnemyBehavior(this);
		}
	}

	public class PouncingEnemyBehavior : AIBehavior
	{
		private enum State
		{
			Approach,
			Telegraph,
			Lunge,
			Recover
		}

		private readonly PouncingEnemyDefinition definition;
		private MeleeAttacker attacker;
		private State state;
		private float stateTimer;
		private Vector2 lungeDirection;

		public PouncingEnemyBehavior(PouncingEnemyDefinition definition)
		{
			this.definition = definition;
		}

		protected override void OnInitialize()
		{
			attacker = Controller.GetComponent<MeleeAttacker>();
			EnterState(State.Approach, 0f);
		}

		protected override void OnTick()
		{
			if (!Controller.TryGetPerceivedTargetPosition(out Vector2 targetPosition))
			{
				ResetMovement();
				EnterState(State.Approach, 0f);
				return;
			}

			Vector2 targetDirection = targetPosition - (Vector2)Controller.transform.position;
			float distance = targetDirection.magnitude;
			Controller.FacingDirection.SetDirection(state == State.Lunge ? lungeDirection : targetDirection);

			switch (state)
			{
				case State.Approach:
					TickApproach(targetDirection, distance);
					break;
				case State.Telegraph:
					TickTelegraph(targetDirection);
					break;
				case State.Lunge:
					TickLunge(distance);
					break;
				case State.Recover:
					TickRecovery(targetDirection, distance);
					break;
			}
		}

		private void TickApproach(Vector2 targetDirection, float distance)
		{
			Controller.Movement.SetSpeedMultiplier(1f);

			if (!Controller.TargetVisible && distance <= definition.RetreatRange)
			{
				Controller.SetMovementIntent(Vector2.zero);
				return;
			}

			if (distance > definition.PreferredRange)
			{
				Controller.SetMovementIntent(targetDirection.normalized);
				return;
			}

			if (distance < definition.RetreatRange)
			{
				Controller.SetMovementIntent(-targetDirection.normalized);
				return;
			}

			Controller.SetMovementIntent(Vector2.zero);
			lungeDirection = targetDirection.normalized;
			EnterState(State.Telegraph, definition.TelegraphDuration);
		}

		private void TickTelegraph(Vector2 targetDirection)
		{
			if (!Controller.TargetVisible)
			{
				ResetMovement();
				EnterState(State.Approach, 0f);
				return;
			}

			Controller.SetMovementIntent(Vector2.zero);
			lungeDirection = targetDirection.normalized;

			if (!TimerFinished())
			{
				return;
			}

			EnterState(State.Lunge, definition.LungeDuration);
			Controller.Movement.SetSpeedMultiplier(definition.LungeSpeedMultiplier);
		}

		private void TickLunge(float distance)
		{
			Controller.SetMovementIntent(lungeDirection, false);

			if (distance <= definition.LandingRange)
			{
				Land(distance);
				return;
			}

			if (!TimerFinished())
			{
				return;
			}

			Land(distance);
		}

		private void Land(float distance)
		{
			Controller.Movement.StopImmediately();
			Controller.Movement.SetSpeedMultiplier(1f);

			if (Controller.TargetVisible && distance <= definition.ImpactRange && attacker != null)
			{
				attacker.PerformImmediateMeleeAttack(definition.ImpactRange);
			}

			EnterState(State.Recover, definition.RecoveryDuration);
		}

		private void TickRecovery(Vector2 targetDirection, float distance)
		{
			Controller.Movement.SetSpeedMultiplier(1f);
			Controller.SetMovementIntent(distance < definition.RetreatRange ? -targetDirection.normalized : Vector2.zero);

			if (TimerFinished())
			{
				EnterState(State.Approach, 0f);
			}
		}

		private bool TimerFinished()
		{
			stateTimer -= Time.deltaTime;
			return stateTimer <= 0f;
		}

		private void EnterState(State nextState, float duration)
		{
			state = nextState;
			stateTimer = duration;
		}

		private void ResetMovement()
		{
			Controller.SetMovementIntent(Vector2.zero);
			Controller.Movement.SetSpeedMultiplier(1f);
		}
	}
}
