using UnboundArcana.Spells.Runtime;
using UnityEngine;

namespace UnboundArcana.Spells.Modules.Piercing
{
	[CreateAssetMenu(menuName = "Spells/Modules/Piercing")]
	public class PiercingModuleDefinition : SpellModuleDefinition
	{
		public int additionalHits = 1;

		public override SpellModule CreateRuntime()
		{
			return new PiercingModule(this);
		}
	}
}