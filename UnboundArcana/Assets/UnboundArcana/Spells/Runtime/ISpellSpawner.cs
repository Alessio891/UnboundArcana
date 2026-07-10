using UnityEngine;

namespace UnboundArcana.Spells.Runtime
{
	public interface ISpellSpawner
	{
		void SpawnProjectile(
			SpawnContext context
		);
	}
}