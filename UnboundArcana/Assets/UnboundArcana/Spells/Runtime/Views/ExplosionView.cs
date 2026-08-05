using UnityEngine;
using UnboundArcana.Spells.Runtime.Objects;
using UnboundArcana.Core.Visuals;

namespace UnboundArcana.Spells.Runtime.Views
{
	public class ExplosionView : SpellRuntimeView
	{
		protected override ProceduralShape VisualShape => ProceduralShape.Ring;
		protected override ProceduralShape AccentShape => ProceduralShape.Circle;
		protected override float AccentScale => 0.24f;
		protected override bool AddTrail => false;
		protected override bool AddParticles => true;
		protected override int ParticleBurstCount => 18;

		private ExplosionRuntimeObject runtimeObject;
		public AnimationClip explosionClip;

		public void Initialize(ExplosionRuntimeObject runtimeObject)
		{
			this.runtimeObject = runtimeObject;

			runtimeObject.SetView(this);
			ProceduralVisual?.Pulse(runtimeObject.Duration, 0.14f);
			Animator animator = GetComponentInChildren<Animator>();

			if (animator != null && animator.runtimeAnimatorController != null && animator.runtimeAnimatorController.animationClips.Length > 0)
			{
				AnimationClip clip = animator.runtimeAnimatorController.animationClips[0];
				animator.speed = clip.length / runtimeObject.Duration;
			}
		}
		private void OnDrawGizmos()
		{
			if (runtimeObject != null)
			{
				Gizmos.DrawWireSphere(
					transform.position,
					runtimeObject.Radius
				);
			}
		}
	}
}
