using UnityEngine;
using UnboundArcana.Spells.Runtime;

namespace UnboundArcana.Spells.Modules.Orbit
{
	[CreateAssetMenu(menuName = "Spells/Modules/Orbit")]
	public class OrbitModuleDefinition : SpellModuleDefinition
	{
		public float radius = 2f;
		public float angularSpeed = 180f;

		public override SpellModule CreateRuntime()
		{
			return new OrbitModule(this);
		}
	}
}