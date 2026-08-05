using UnityEngine;
using UnboundArcana.Spells.Runtime.Objects;
using System.Collections;
using UnboundArcana.Core.Visuals;

namespace UnboundArcana.Spells.Runtime.Views
{
	public class ProjectileView : SpellRuntimeView
	{
		protected override ProceduralShape VisualShape => ProceduralShape.Diamond;
		protected override ProceduralShape AccentShape => ProceduralShape.Circle;
		protected override float AccentScale => 0.3f;
		protected override bool AddTrail => true;
		protected override bool AddHalo => false;

		private ProjectileRuntimeObject runtimeObject;

		public void Initialize(ProjectileRuntimeObject runtimeObject)
		{
			this.runtimeObject = runtimeObject;
			runtimeObject.SetView(this);
			runtimeObject.SyncView();
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (runtimeObject == null)
			{
				return;
			}

			ProceduralVfx.SpawnImpact(transform.position, ProceduralVisual != null ? ProceduralVisual.CurrentColor : ProceduralPalette.Arcane, 0.7f);
			runtimeObject.Hit(collision.gameObject);
		}

		public override void DestroyView() {
			ProceduralVisual?.PlayExit(0.14f);
			GetComponentInChildren<Animator>()?.SetTrigger("End");
			StartCoroutine(waitAndDestroy());
		}
		IEnumerator waitAndDestroy() {
			yield return new WaitForSeconds(0.5f);
			Destroy(gameObject);
		}
	}
}
