using UnboundArcana.Spells.Runtime;
using UnityEngine;

namespace UnboundArcana.Spells.Modules.Split
{
	[CreateAssetMenu(menuName = "Spells/Modules/Split On Destroy")]
	public class SplitOnDestroyModuleDefinition : SpellModuleDefinition
	{
		public int count = 2;

		[Range(0, 180)]
		public float spreadAngle = 45f;

		public override SpellModule CreateRuntime()
		{
			return new SplitOnDestroyModule(this);
		}
	}
}