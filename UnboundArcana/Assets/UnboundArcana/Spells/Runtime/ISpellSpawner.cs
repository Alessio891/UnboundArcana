using UnityEngine;

namespace UnboundArcana.Spells.Runtime
{
	public interface ISpellSpawner
	{
		void SpawnProjectile(
			Vector3 position,
			Vector3 direction
		);
	}
}