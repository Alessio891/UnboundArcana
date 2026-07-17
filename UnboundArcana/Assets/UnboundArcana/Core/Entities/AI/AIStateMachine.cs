namespace UnboundArcana.Core.Entities.AI
{
	public class AIStateMachine
	{
		private AIState currentState;

		public AIState CurrentState => currentState;


		public void ChangeState(AIState newState)
		{
			if (currentState == newState)
				return;

			currentState?.Exit();

			currentState = newState;

			currentState?.Enter();
		}


		public void Tick()
		{
			currentState?.Tick();
		}
	}
}