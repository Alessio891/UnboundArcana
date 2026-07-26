using UnityEngine;
using UnboundArcana.Core.Entities;
using UnityEngine.InputSystem;

namespace UnboundArcana.Core.Entities.AI
{
	public class AIController : EntityController
	{
		private AIBehavior behavior;
		private EntitySensor sensor;
		public TargetingComponent Target => Targeting;

		public CharacterMotor Movement => Motor;

		public Entity CurrentEntity => Entity;

		public SpellCaster Caster => SpellCaster;

		public EntityFacing FacingDirection => Facing;
		
		[SerializeField]
		private AIBehaviorDefinition behaviorDefinition;

		protected override void Awake()
		{
			base.Awake();

			sensor = GetComponentInChildren<EntitySensor>();

			sensor.EntityDetected += OnEntityDetected;
			sensor.EntityLost += OnEntityLost;
			behavior =
				behaviorDefinition.CreateBehavior();

			behavior.Initialize(this);
		}

		private void OnEntityDetected(Entity entity)
		{
			if (entity.tag == "Player")
			{
				Targeting.SetTarget(entity);
				Debug.Log("Sensed target " + entity.name);
			}
		}


		private void OnEntityLost(Entity entity)
		{
			if (Targeting.CurrentTarget == entity)
			{
				Debug.Log("Lost target");
				Movement.SetMovementIntent(Vector2.zero);
				Targeting.ClearTarget();
			}
		}
		private void Update()
		{
			behavior?.Tick();
		}
	}
}