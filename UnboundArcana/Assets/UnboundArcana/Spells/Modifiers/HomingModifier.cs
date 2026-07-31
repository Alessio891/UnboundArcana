using UnityEngine;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Runtime.Objects;
using UnboundArcana.Core.Combat;
using UnboundArcana.Core.Events;

namespace UnboundArcana.Spells.Modifiers
{
	public class HomingModifier : IRuntimeObjectModifier
	{
		private ProjectileRuntimeObject projectile;
		private readonly Collider2D[] targetBuffer = new Collider2D[32];

		private float strength;

		public bool ControlsMovement => false;

		public HomingModifier(float strength)
		{
			this.strength = strength;
		}

		public void Initialize(
			SpellRuntimeObject runtimeObject)
		{
			projectile = runtimeObject as ProjectileRuntimeObject;
		}

		public void Update(float deltaTime)
		{
			if (projectile == null)
			{
				return;
			}

			GameObject target = FindTarget();

			if (target == null)
			{
				return;
			}

			Vector3 direction =
				target.transform.position -
				projectile.Position;

			Vector3 newDirection =
				Vector3.Lerp(
					projectile.Direction,
					direction.normalized,
					strength * deltaTime
				);

			projectile.SetProjectileDirection(newDirection);
		}

		private GameObject FindTarget()
		{
			int hitCount = Physics2D.OverlapCircle(projectile.Position, 1.5f, ContactFilter2D.noFilter, targetBuffer);
			GameObject closestTarget = null;
			float closestDistance = float.PositiveInfinity;

			for (int i = 0; i < hitCount; i++)
			{
				Collider2D hit = targetBuffer[i];
				if (hit.GetComponent<IDamageable>() == null || hit.gameObject == projectile.Spell.Owner || projectile.HitHistory.HasHit(hit.gameObject)) { continue; }

				float distance = (hit.transform.position - projectile.Position).sqrMagnitude;
				if (distance >= closestDistance) { continue; }

				closestDistance = distance;
				closestTarget = hit.gameObject;
			}

			return closestTarget;
		}

		public void Destroy()
		{
		}

		public void OnHit(HitEvent hitEvent)
		{
		}
	}
}
