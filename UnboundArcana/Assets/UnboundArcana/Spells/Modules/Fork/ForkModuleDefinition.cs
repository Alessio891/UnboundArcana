using UnboundArcana.Spells.Modules.Fire;
using UnboundArcana.Spells.Runtime;
using UnityEngine;

namespace UnboundArcana.Spells.Modules.Fork
{
	[CreateAssetMenu(menuName = "Spells/Modules/Fork")]
	public class ForkModuleDefinition : SpellModuleDefinition
	{
		public int additionalProjectiles = 2;
		public float angle = 15f;

		public override SpellModule CreateRuntime()
		{
			return new ForkModule(this);
		}
	}
}