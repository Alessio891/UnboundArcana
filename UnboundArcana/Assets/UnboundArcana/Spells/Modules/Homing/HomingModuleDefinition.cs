using UnboundArcana.Spells.Runtime;
using UnityEngine;

namespace UnboundArcana.Spells.Modules.Homing
{
	[CreateAssetMenu(menuName = "Spells/Modules/Homing")]
	public class HomingModuleDefinition : SpellModuleDefinition
	{
		public float strength = 5f;

		public override SpellModule CreateRuntime()
		{
			return new HomingModule(this);
		}
	}
}