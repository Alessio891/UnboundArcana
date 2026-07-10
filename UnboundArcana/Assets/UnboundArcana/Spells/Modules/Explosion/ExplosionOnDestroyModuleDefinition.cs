using UnityEngine;
using UnboundArcana.Spells.Modules;
using UnboundArcana.Spells.Runtime;

namespace UnboundArcana.Spells.Modules.ExplosionOnDestroy
{
	[CreateAssetMenu(menuName = "Spells/Modules/Explosion On Destroy")]
	public class ExplosionOnDestroyModuleDefinition : SpellModuleDefinition
	{
		public GameObject explosionPrefab;
		public float radius = 2f;
		public float damage = 10f;
		public float duration = 0.5f;

		public override SpellModule CreateRuntime()
		{
			return new ExplosionOnDestroyModule(this);
		}
	}
}