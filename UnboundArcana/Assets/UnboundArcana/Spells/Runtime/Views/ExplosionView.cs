using UnityEngine;
using UnboundArcana.Spells.Runtime.Objects;

namespace UnboundArcana.Spells.Runtime.Views
{
	public class ExplosionView : SpellRuntimeView
	{
		private ExplosionRuntimeObject runtimeObject;
		public AnimationClip explosionClip;

		public void Initialize(ExplosionRuntimeObject runtimeObject)
		{
			this.runtimeObject = runtimeObject;

			runtimeObject.SetView(this);
			Animator animator = GetComponentInChildren<Animator>();

			AnimationClip clip = animator.runtimeAnimatorController.animationClips[0];

			animator.speed = clip.length / runtimeObject.Duration;
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
