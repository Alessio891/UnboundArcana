using UnityEngine;
using UnboundArcana.Spells.Runtime.Objects;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Core.Events;

namespace UnboundArcana.Spells.Modules.Orbit
{
	public class OrbitModifier : IRuntimeObjectModifier
	{
		private readonly Transform owner;
		private readonly float radius;
		private readonly float angularSpeed;

		private ProjectileRuntimeObject projectile;
		private float angle;

		public bool ControlsMovement => true;

		public OrbitModifier(
			Transform owner,
			float radius,
			float angularSpeed)
		{
			this.owner = owner;
			this.radius = radius;
			this.angularSpeed = angularSpeed;
		}

		public void Initialize(
			SpellRuntimeObject runtimeObject)
		{
			projectile =
				runtimeObject as ProjectileRuntimeObject;

			angle = 0;
		}

		public void Update(float deltaTime)
		{
			if (projectile == null)
			{
				return;
			}

			angle += angularSpeed * deltaTime;

			Vector3 offset =
				Quaternion.Euler(
					0,
					0,
					angle
				) * Vector3.right * radius;

			projectile.SetProjectilePosition(
				owner.position + offset
			);
		}

		public void OnHit(HitEvent hitEvent)
		{
		}

		public void Destroy()
		{
		}
	}
}