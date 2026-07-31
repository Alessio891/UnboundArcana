using UnboundArcana.Core.Combat;
using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Entities.Statuses;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Stats;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Runtime.Objects;
using UnityEngine;

namespace UnboundArcana.Spells.Modules.Principles
{
	public class ElementalPrincipleModule : SpellModule
	{
		private readonly ElementalPrincipleDefinition definition;
		private readonly Collider2D[] targetBuffer = new Collider2D[32];

		public ElementalPrincipleModule(ElementalPrincipleDefinition definition)
		{
			this.definition = definition;
		}

		public override void Initialize(SpellInstance spell)
		{
			base.Initialize(spell);
			Events.Subscribe<HitEvent>(OnHit);
			Events.Subscribe<RuntimeObjectSpawnedEvent>(OnRuntimeObjectSpawned);
		}

		public override void ApplyStats(StatCollection stats)
		{
			if (definition.principle == ElementalPrincipleType.Earth) { return; }
			stats.AddBase(StatKeys.Spell.Damage, definition.damage, this);
		}

		private void OnRuntimeObjectSpawned(RuntimeObjectSpawnedEvent eventData)
		{
			switch (eventData.RuntimeObject)
			{
				case ProjectileRuntimeObject:
					eventData.RuntimeObject.SetVisualAppearance(definition.projectileSprite, definition.projectileController, definition.fallbackColor);
					break;
				case BeamRuntimeObject:
					eventData.RuntimeObject.SetVisualAppearance(definition.beamSprite, definition.beamController, definition.fallbackColor);
					break;
				case AuraRuntimeObject:
					eventData.RuntimeObject.SetVisualAppearance(definition.auraSprite, definition.auraController, definition.fallbackColor);
					((AuraRuntimeObject)eventData.RuntimeObject).SetVisualOffset(definition.auraVisualOffset);
					if (definition.principle == ElementalPrincipleType.Air)
					{
						((AuraRuntimeObject)eventData.RuntimeObject).SetAnchor(null);
					}
					break;
			}
		}

		private void OnHit(HitEvent hitEvent)
		{
			switch (definition.principle)
			{
				case ElementalPrincipleType.Acid:
				ApplyStatus(hitEvent);
				break;
				case ElementalPrincipleType.Air:
					ApplyImpulse(hitEvent, definition.force);
					break;
				case ElementalPrincipleType.Earth:
					ApplyStagger(hitEvent);
					spell.Runtime.GameEvents.Publish(new DamageEvent(spell.Owner, hitEvent.Target, definition.damage, DamageType.Physical));
					break;
				case ElementalPrincipleType.Lightning:
					ArcToNearbyTarget(hitEvent);
					break;
				case ElementalPrincipleType.Water:
					ApplyWaterPressure(hitEvent);
					ApplyStatus(hitEvent);
					break;
			}
		}

		private void ApplyStatus(HitEvent hitEvent)
		{
			if (definition.status == null) { return; }

			Entity source = hitEvent.Owner.GetComponent<Entity>();
			hitEvent.Target.Status.Apply(definition.status, source);
		}

		private void ApplyImpulse(HitEvent hitEvent, float force)
		{
			CharacterMotor motor = hitEvent.Target.GetComponent<CharacterMotor>();
			if (motor == null) { return; }

			Vector3 sourcePosition = GetSourcePosition(hitEvent.Source);
			Vector2 direction = ((Vector2)hitEvent.Target.transform.position - (Vector2)sourcePosition).normalized;
			motor.ApplyImpulse(direction * force);
		}

		private void ApplyStagger(HitEvent hitEvent)
		{
			CharacterMotor motor = hitEvent.Target.GetComponent<CharacterMotor>();
			if (motor != null) { motor.ApplyMovementLock(definition.staggerDuration); }
		}

		private void ApplyWaterPressure(HitEvent hitEvent)
		{
			if (definition.status == null) { return; }

			StatusInstance current = hitEvent.Target.Status.Get(definition.status);
			if (current == null) { return; }

			float bonusDamage = definition.damage * current.Stacks * 0.25f;
			spell.Runtime.GameEvents.Publish(new DamageEvent(spell.Owner, hitEvent.Target, bonusDamage, DamageType.Water));
		}

		private void ArcToNearbyTarget(HitEvent hitEvent)
		{
			int hitCount = Physics2D.OverlapCircle(hitEvent.Target.transform.position, definition.arcRange, ContactFilter2D.noFilter, targetBuffer);
			Entity closestTarget = null;
			float closestDistance = float.PositiveInfinity;

			for (int i = 0; i < hitCount; i++)
			{
				Entity candidate = targetBuffer[i].GetComponent<Entity>();
				if (candidate == null || candidate == hitEvent.Target || candidate.gameObject == spell.Owner) { continue; }

				float distance = ((Vector2)candidate.transform.position - (Vector2)hitEvent.Target.transform.position).sqrMagnitude;
				if (distance >= closestDistance) { continue; }

				closestDistance = distance;
				closestTarget = candidate;
			}

			if (closestTarget == null) { return; }

			float damage = spell.Stats.Get(StatKeys.Spell.Damage) * definition.arcDamageMultiplier;
			spell.Runtime.GameEvents.Publish(new DamageEvent(spell.Owner, closestTarget, damage, DamageType.Lightning));
			LightningArcFeedback.Spawn(hitEvent.Target.transform.position, closestTarget.transform.position);
		}

		private Vector3 GetSourcePosition(SpellRuntimeObject source)
		{
			return source switch
			{
				ProjectileRuntimeObject projectile => projectile.Position,
				ExplosionRuntimeObject explosion => explosion.Position,
				AuraRuntimeObject aura => aura.Position,
				BeamRuntimeObject beam => beam.Position,
				_ => spell.Owner.transform.position
			};
		}

		public override void Destroy()
		{
			Events.Unsubscribe<HitEvent>(OnHit);
			Events.Unsubscribe<RuntimeObjectSpawnedEvent>(OnRuntimeObjectSpawned);
		}
	}
}
