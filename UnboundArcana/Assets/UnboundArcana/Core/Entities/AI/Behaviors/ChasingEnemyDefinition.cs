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
		public override AIBehavior CreateBehavior()
		{
			return new ChasingEnemyBehavior();
		}
	}

	public class ChasingEnemyBehavior : AIBehavior
	{
		private Entity target;


		protected override void OnInitialize()
		{
			target =
				GameObject.FindGameObjectWithTag("Player")
				.GetComponent<Entity>();
		}


		protected override void OnTick()
		{
			if (Controller == null) {
				Debug.Log("controller null");
				return;
			}
			if (Controller.Target == null) {
				Debug.Log("Target is null");
			}
			if (Controller.Target?.CurrentTarget == null)
			{
				return;
			}
			Debug.Log("tick behav");


			Vector2 direction =
				Controller.Target.CurrentTarget.transform.position -
				Controller.transform.position;


			Controller.Movement.SetMovementIntent(
				direction.normalized
			);
		}
	}
}