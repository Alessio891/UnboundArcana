using UnityEngine;
using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Rooms;
using System;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Expedition;
using UnityEngine.Tilemaps;

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
		[SerializeField] private float obstacleLookAhead = 0.8f;
		[SerializeField] private float obstacleRadius = 0.2f;
		[SerializeField] private float obstacleAvoidanceStrength = 1.25f;

		private bool behaviorActive;
		private bool targetVisible;
		private float targetMemoryTimer;
		private Vector2 lastKnownTargetPosition;
		private readonly RaycastHit2D[] physicsHits = new RaycastHit2D[16];
		private ContactFilter2D physicsQueryFilter;

		public bool TargetVisible => targetVisible;

		protected override void Awake()
		{
			base.Awake();

			sensor = GetComponentInChildren<EntitySensor>();
			physicsQueryFilter = new ContactFilter2D();
			physicsQueryFilter.useTriggers = false;
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
			targetVisible = HasLineOfSight(entity.transform.position);
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

		public void SetMovementIntent(Vector2 desiredDirection, bool avoidObstacles = true)
		{
			if (!avoidObstacles || desiredDirection.sqrMagnitude <= 0f)
			{
				Movement.SetMovementIntent(desiredDirection);
				return;
			}

			Movement.SetMovementIntent(CalculateAvoidedDirection(desiredDirection.normalized));
		}

		private void UpdatePerception()
		{
			Entity target = Targeting.CurrentTarget;

			if (target == null)
			{
				targetVisible = false;
				return;
			}

			bool canSeeTarget = sensor.IsDetected(target) && HasLineOfSight(target.transform.position);

			if (canSeeTarget)
			{
				targetVisible = true;
				lastKnownTargetPosition = target.transform.position;
				targetMemoryTimer = targetMemoryDuration;
				return;
			}

			targetVisible = false;
			targetMemoryTimer -= Time.deltaTime;

			if (targetMemoryTimer <= 0f)
			{
				Targeting.ClearTarget();
			}
		}

		private Vector2 CalculateAvoidedDirection(Vector2 desiredDirection)
		{
			int hitCount = Physics2D.CircleCast(transform.position, obstacleRadius, desiredDirection, physicsQueryFilter, physicsHits, obstacleLookAhead);
			RaycastHit2D closestHit = default;
			bool foundObstacle = false;

			for (int i = 0; i < hitCount; i++)
			{
				RaycastHit2D hit = physicsHits[i];

				if (!(hit.collider is TilemapCollider2D) || hit.distance <= 0.01f)
				{
					continue;
				}

				if (!foundObstacle || hit.distance < closestHit.distance)
				{
					closestHit = hit;
					foundObstacle = true;
				}
			}

			if (!foundObstacle)
			{
				return desiredDirection;
			}

			Vector2 normal = closestHit.normal.normalized;
			Vector2 tangentA = new Vector2(-normal.y, normal.x);
			Vector2 tangentB = -tangentA;
			Vector2 tangent = Vector2.Dot(tangentA, desiredDirection) >= Vector2.Dot(tangentB, desiredDirection) ? tangentA : tangentB;
			float proximity = 1f - Mathf.Clamp01(closestHit.distance / obstacleLookAhead);
			Vector2 avoidance = tangent * obstacleAvoidanceStrength + normal * proximity;
			return (desiredDirection + avoidance * proximity).normalized;
		}

		private bool HasLineOfSight(Vector2 targetPosition)
		{
			Vector2 origin = transform.position;
			Vector2 direction = targetPosition - origin;
			float distance = direction.magnitude;

			if (distance <= 0f)
			{
				return true;
			}

			int hitCount = Physics2D.Raycast(origin, direction.normalized, physicsQueryFilter, physicsHits, distance);

			for (int i = 0; i < hitCount; i++)
			{
				if (physicsHits[i].collider is TilemapCollider2D)
				{
					return false;
				}
			}

			return true;
		}
	}
}
