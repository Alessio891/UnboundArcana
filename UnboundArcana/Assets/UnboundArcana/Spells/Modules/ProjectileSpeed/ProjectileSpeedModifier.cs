using UnityEngine;
using UnboundArcana.Core.Events;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Runtime.Objects;

namespace UnboundArcana.Spells.Modules.ProjectileSpeed
{
	public class ProjectileSpeedModifier : IRuntimeObjectModifier
	{
		private readonly float acceleration;
		private ProjectileRuntimeObject projectile;

		public bool ControlsMovement => false;

		public ProjectileSpeedModifier(
			float acceleration)
		{
			this.acceleration = acceleration;
		}

		public void Initialize(
			SpellRuntimeObject runtimeObject)
		{
			projectile =
				runtimeObject as ProjectileRuntimeObject;
		}

		public void Update(float deltaTime)
		{
			if (projectile == null)
			{
				return;
			}
			projectile.AddSpeed(acceleration * deltaTime);
		}

		public void OnHit(HitEvent hitEvent)
		{
		}

		public void Destroy()
		{
		}
	}
}
