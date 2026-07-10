using UnityEngine;
using UnboundArcana.Spells.Runtime.Objects;

namespace UnboundArcana.Spells.Runtime.Views
{
	public class AuraView : MonoBehaviour
	{
		private AuraRuntimeObject runtimeObject;

		public void Initialize(AuraRuntimeObject runtimeObject)
		{
			this.runtimeObject = runtimeObject;
			runtimeObject.SetView(gameObject);
		}
	}
}