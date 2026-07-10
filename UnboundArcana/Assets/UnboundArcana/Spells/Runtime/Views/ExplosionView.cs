using UnityEngine;
using UnboundArcana.Spells.Runtime.Objects;

namespace UnboundArcana.Spells.Runtime.Views
{
	public class ExplosionView : MonoBehaviour
	{
		private ExplosionRuntimeObject runtimeObject;

		public void Initialize(ExplosionRuntimeObject runtimeObject)
		{
			this.runtimeObject = runtimeObject;

			runtimeObject.SetView(gameObject);
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