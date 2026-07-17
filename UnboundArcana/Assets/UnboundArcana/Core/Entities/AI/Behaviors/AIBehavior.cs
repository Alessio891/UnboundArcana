using UnboundArcana.Core.Entities.AI.Attacks;
using UnboundArcana.Core.Entities.AI.Steering;
using UnityEngine;

namespace UnboundArcana.Core.Entities.AI
{
	public abstract class AIBehavior
	{
		protected AIController Controller { get; }

		protected SteeringStrategy Steering { get; }
		protected AttackStrategy Attack { get; }

		private readonly AIStateMachine stateMachine = new();


		protected AIBehavior(
			AIController controller,
			SteeringStrategy steering,
			AttackStrategy attack)
		{
			Controller = controller;
			Steering = steering;
			Attack = attack;
		}
		public void ExecuteAttack(Entity target)
		{
			if (Attack.CanAttack(target))
			{
				Attack.Execute(target);
			}
		}

		public Vector2 GetMovementDirection(Entity target)
		{
			return Steering.CalculateDirection(target);
		}


		public void Initialize()
		{
			ChangeState(CreateInitialState());
		}


		public void Tick()
		{
			stateMachine.Tick();
		}


		public void ChangeState(AIState state)
		{
			state.Initialize(
				Controller,
				this
			);

			stateMachine.ChangeState(state);
		}


		protected abstract AIState CreateInitialState();
	}
}