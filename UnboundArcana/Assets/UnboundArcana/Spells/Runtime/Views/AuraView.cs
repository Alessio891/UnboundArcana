using UnityEngine;
using UnboundArcana.Spells.Runtime.Objects;

namespace UnboundArcana.Spells.Runtime.Views
{
	public class AuraView : SpellRuntimeView
	{
		private AuraRuntimeObject runtimeObject;

		public void Initialize(AuraRuntimeObject runtimeObject)
		{
			this.runtimeObject = runtimeObject;
			runtimeObject.SetView(this);
		}
	}
}
