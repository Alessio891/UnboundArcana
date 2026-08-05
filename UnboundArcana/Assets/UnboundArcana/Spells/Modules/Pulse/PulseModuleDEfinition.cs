using UnityEngine;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Data;
using UnboundArcana.Spells.Modules.Explosion;
using UnboundArcana.Spells.Modules.ExplosionOnDestroy;

namespace UnboundArcana.Spells.Modules.Pulse
{
	[CreateAssetMenu(menuName = "Spells/Modules/Pulse")]
	public class PulseModuleDefinition : SpellModuleDefinition
	{
		public float interval = 1f;

		public override bool CanAddTo(SpellConfiguration configuration)
		{
			if (!base.CanAddTo(configuration)) { return false; }

			foreach (SpellModuleDefinition module in configuration.Modules)
			{
				if (module is ExplosionModuleDefinition || module is ExplosionOnDestroyModuleDefinition) { return true; }
			}

			return false;
		}

		public override SpellModule CreateRuntime()
		{
			return new PulseModule(this);
		}
	}
}
