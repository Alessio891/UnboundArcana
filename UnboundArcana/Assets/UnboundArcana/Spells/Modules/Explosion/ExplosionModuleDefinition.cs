using UnboundArcana.Spells.Runtime;
using UnityEngine;

namespace UnboundArcana.Spells.Modules.Explosion
{
	[CreateAssetMenu(menuName = "Spells/Modules/Explosion")]
	public class ExplosionModuleDefinition : SpellModuleDefinition
	{
		public float radius;
		public float damage;
		public float duration;
		public GameObject explosionPrefab;
		public override SpellModule CreateRuntime()
		{
			return new ExplosionModule(this);
		}
	}
}