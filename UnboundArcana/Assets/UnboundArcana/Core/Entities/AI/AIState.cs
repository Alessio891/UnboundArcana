using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace UnboundArcana.Core.Entities.AI
{
	public abstract class AIState
	{
		protected AIController controller;

		public virtual void Initialize(AIController controller)
		{
			this.controller = controller;
		}

		public virtual void Enter()
		{
		}

		public virtual void Exit()
		{
		}

		public virtual void Tick()
		{
		}
	}

	public class IdleState : AIState
	{
		public override void Tick()
		{
			if (controller.Target.CurrentTarget != null)
			{
				controller.SetState(
					new ChaseState()
				);
			}
		}
	}

	public class ChaseState : AIState
	{
		public override void Enter()
		{
		}


		public override void Tick()
		{
			var target = controller.Target.CurrentTarget;

			if (target == null)
			{
				controller.SetState(new IdleState());
				return;
			}

			Vector2 direction =
				target.transform.position -
				controller.transform.position;

			float distance = direction.magnitude;

			if (distance <= controller.Profile.AttackRange)
			{
				controller.SetState(new AttackState());
				return;
			}

			controller.Movement.SetMovementIntent(
				direction.normalized
			);
		}

		public override void Exit()
		{
			controller.Movement.SetMovementIntent(Vector2.zero);
		}
	}

	public class AttackState : AIState
	{
		public override void Enter()
		{
			controller.Movement.SetMovementIntent(
				Vector2.zero
			);
		}


		public override void Tick()
		{
			var target = controller.Target.CurrentTarget;

			if (target == null)
			{
				controller.SetState(new IdleState());
				return;
			}


			Vector2 direction =
				target.transform.position -
				controller.transform.position;


			float distance = direction.magnitude;


			if (distance > controller.Profile.AttackRange)
			{
				controller.SetState(new ChaseState());
				return;
			}


			controller.FacingDirection.SetDirection(
				direction
			);

			controller.Caster.SetAimDirection(
				direction
			);

			controller.Caster.BeginCast();
		}


		public override void Exit()
		{
			controller.Caster.EndCast();
		}
	}
}