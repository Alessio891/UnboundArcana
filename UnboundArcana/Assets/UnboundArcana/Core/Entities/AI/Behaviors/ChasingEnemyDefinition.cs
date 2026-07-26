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
		[SerializeField] private float stoppingRange = 0.1f;
		[SerializeField] private float attackCooldown = 0.8f;
		public float StoppingRange => stoppingRange;
		public float AttackCooldown => attackCooldown;


		public override AIBehavior CreateBehavior()
		{
			return new ChasingEnemyBehavior(this);
		}
	}

	public class ChasingEnemyBehavior : AIBehavior
	{
		private Entity target;
		ChasingEnemyDefinition definition;
		float attackTimer = 0f;

		public ChasingEnemyBehavior(ChasingEnemyDefinition def) {
			this.definition = def;
		}

		protected override void OnInitialize()
		{
		
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

			float dist = Vector3.Distance(Controller.Target.CurrentTarget.transform.position, Controller.transform.position);
			if (dist > definition.StoppingRange)
			{
				Vector2 direction =
					Controller.Target.CurrentTarget.transform.position -
					Controller.transform.position;


				Controller.Movement.SetMovementIntent(
					direction.normalized
				);
				Controller.FacingDirection.SetDirection(direction);
			} else {
				Controller.Movement.SetMovementIntent(
						Vector2.zero
					);
				if (attackTimer <= 0.0f)
				{
					var meleeComponent = Controller.GetComponent<MeleeAttacker>();
					if (meleeComponent != null)
					{
						meleeComponent.PerformMeleeAttack();
					}
					attackTimer = definition.AttackCooldown;
				} else {
					attackTimer -= Time.deltaTime;
				}
			}
		}
	}
}