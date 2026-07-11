using UnboundArcana.Spells.Runtime;
using UnboundArcana.Core.Stats;
using UnityEngine;

namespace UnboundArcana.Spells.Modules.Explosion
{
	[CreateAssetMenu(menuName = "Spells/Modules/Explosion")]
	public class ExplosionModuleDefinition : SpellModuleDefinition
	{
		public float radius = 2f;
		public float damage = 10f;
		public float duration = 1f;
		public GameObject explosionPrefab;

		public override SpellModule CreateRuntime()
		{
			return new ExplosionModule(this);
		}

		
	}
}