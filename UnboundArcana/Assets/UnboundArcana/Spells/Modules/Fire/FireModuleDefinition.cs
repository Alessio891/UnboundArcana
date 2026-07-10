using UnboundArcana.Spells.Runtime;
using UnityEngine;

namespace UnboundArcana.Spells.Modules.Fire
{
	[CreateAssetMenu(menuName = "Spells/Modules/Fire")]
	public class FireModuleDefinition : SpellModuleDefinition
	{
		public float damage;

		public override SpellModule CreateRuntime()
		{
			return new FireModule(this);
		}
	}
}