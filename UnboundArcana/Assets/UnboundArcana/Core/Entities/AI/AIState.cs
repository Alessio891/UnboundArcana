using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace UnboundArcana.Core.Entities.AI
{
	public abstract class AIState
	{
		protected AIController controller;
		protected AIBehavior Behavior { get; private set; }
		public virtual void Initialize(
			AIController controller,
			AIBehavior behavior)
		{
			this.controller = controller;
			Behavior = behavior;
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
				Behavior.ChangeState(
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
				Behavior.ChangeState(new IdleState());
				return;
			}

			Vector2 direction =
				Behavior.GetMovementDirection(target);

			float distance = Vector3.Distance(controller.transform.position, target.transform.position);// direction.magnitude;

			if (distance <= controller.Profile.AttackRange)
			{
				Behavior.ChangeState(new AttackState());
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
				Behavior.ChangeState(new IdleState());
				return;
			}


			Vector2 direction = Behavior.GetMovementDirection(target);


			float distance = Vector3.Distance(controller.transform.position, target.transform.position);


			if (distance > controller.Profile.AttackRange)
			{
				Behavior.ChangeState(new ChaseState());
				return;
			}


			controller.FacingDirection.SetDirection(
				direction
			);

			controller.Caster?.SetAimDirection(
				direction
			);

			Behavior.ExecuteAttack(target);
		}


		public override void Exit()
		{
			controller.Caster.EndCast();
		}
	}
}