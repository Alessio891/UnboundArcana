using UnityEngine;
using UnboundArcana.Spells.Runtime.Objects;

namespace UnboundArcana.Spells.Runtime.Views
{
	public class BeamView : SpellRuntimeView
	{
		private BeamRuntimeObject runtimeObject;

		public void Initialize(BeamRuntimeObject runtimeObject)
		{
			this.runtimeObject = runtimeObject;
			runtimeObject.SetView(this);
		}
	}
}
