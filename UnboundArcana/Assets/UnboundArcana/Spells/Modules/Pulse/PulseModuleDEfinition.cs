using UnityEngine;
using UnboundArcana.Spells.Runtime;

namespace UnboundArcana.Spells.Modules.Pulse
{
	[CreateAssetMenu(menuName = "Spells/Modules/Pulse")]
	public class PulseModuleDefinition : SpellModuleDefinition
	{
		public float interval = 1f;

		public override SpellModule CreateRuntime()
		{
			return new PulseModule(this);
		}
	}
}