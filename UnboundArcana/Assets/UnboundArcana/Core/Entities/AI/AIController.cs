using UnityEngine;
using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Rooms;
using System;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Expedition;

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

		[SerializeField] private AIBehaviorDefinition behaviorDefinition;
		[SerializeField] private float targetMemoryDuration = 3f;
		[SerializeField] private float movementAcceleration = 8f;
		[SerializeField] private float movementDeceleration = 12f;

		private bool behaviorActive;
		private bool targetVisible;
		private float targetMemoryTimer;
		private Vector2 lastKnownTargetPosition;

		public bool TargetVisible => targetVisible;

		protected override void Awake()
		{
			base.Awake();

			sensor = GetComponentInChildren<EntitySensor>();
			Movement.SetMovementSmoothing(movementAcceleration, movementDeceleration);
			behavior = behaviorDefinition.CreateBehavior();
			behavior.Initialize(this);
		}

		private void OnEnable()
		{
			sensor.EntityDetected += OnEntityDetected;
			sensor.EntityLost += OnEntityLost;
			GameRuntimeManager.Instance.Events.Subscribe<BehaviorActivationEvent>(OnEncounterStarted);
		}

		private void OnDisable()
		{
			sensor.EntityDetected -= OnEntityDetected;
			sensor.EntityLost -= OnEntityLost;
			GameRuntimeManager.Instance.Events.Unsubscribe<BehaviorActivationEvent>(OnEncounterStarted);
			Movement.SetMovementIntent(Vector2.zero);
			Movement.SetSpeedMultiplier(1f);
		}

		private void OnEncounterStarted(BehaviorActivationEvent @event)
		{
			behaviorActive = true;
		}

		private void OnEntityDetected(Entity entity)
		{
			if (!entity.CompareTag("Player"))
			{
				return;
			}

			Targeting.SetTarget(entity);
			targetVisible = true;
			targetMemoryTimer = targetMemoryDuration;
			lastKnownTargetPosition = entity.transform.position;
		}

		private void OnEntityLost(Entity entity)
		{
			if (Targeting.CurrentTarget != entity)
			{
				return;
			}

			targetVisible = false;
			targetMemoryTimer = targetMemoryDuration;
			lastKnownTargetPosition = entity.transform.position;
		}

		private void Update()
		{
			UpdatePerception();

			if (behaviorActive)
			{
				behavior?.Tick();
			}
		}

		public bool TryGetPerceivedTargetPosition(out Vector2 position)
		{
			Entity target = Targeting.CurrentTarget;

			if (target == null)
			{
				position = default;
				return false;
			}

			position = targetVisible ? target.transform.position : lastKnownTargetPosition;
			return true;
		}

		private void UpdatePerception()
		{
			Entity target = Targeting.CurrentTarget;

			if (target == null)
			{
				targetVisible = false;
				return;
			}

			if (targetVisible)
			{
				lastKnownTargetPosition = target.transform.position;
				targetMemoryTimer = targetMemoryDuration;
				return;
			}

			targetMemoryTimer -= Time.deltaTime;

			if (targetMemoryTimer <= 0f)
			{
				Targeting.ClearTarget();
			}
		}
	}
}
