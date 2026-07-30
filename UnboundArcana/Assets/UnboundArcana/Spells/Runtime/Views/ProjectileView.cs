using UnityEngine;
using UnboundArcana.Spells.Runtime.Objects;
using System.Collections;

namespace UnboundArcana.Spells.Runtime.Views
{
	public class ProjectileView : SpellRuntimeView
	{
		private ProjectileRuntimeObject runtimeObject;

		public void Initialize(ProjectileRuntimeObject runtimeObject)
		{
			this.runtimeObject = runtimeObject;
			runtimeObject.SetView(this);
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			runtimeObject?.Hit(collision.gameObject);
		}

		public override void DestroyView() {
			GetComponentInChildren<Animator>()?.SetTrigger("End");
			StartCoroutine(waitAndDestroy());
		}
		IEnumerator waitAndDestroy() {
			yield return new WaitForSeconds(0.5f);
			Destroy(gameObject);
		}
	}
}
