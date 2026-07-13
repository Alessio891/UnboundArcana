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
			Collider2D[] hits =
				Physics2D.OverlapCircleAll(
					projectile.Position,
					5f
				);

			foreach (Collider2D hit in hits)
			{
				if (hit.GetComponent<IDamageable>() != null && hit.gameObject.gameObject != projectile.Spell.Owner && !projectile.HitHistory.HasHit(hit.gameObject))
				{
					return hit.gameObject;
				}
			}

			return null;
		}

		public void Destroy()
		{
		}

		public void OnHit(HitEvent hitEvent)
		{
		}
	}
}