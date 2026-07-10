using UnityEngine;
using UnboundArcana.Spells.Runtime;

namespace UnboundArcana.Spells.Behaviors.Aura
{
	[CreateAssetMenu(menuName = "Unbound Arcana/Behaviors/Aura")]
	public class AuraBehaviorDefinition : SpellBehaviorDefinition
	{
		public GameObject auraPrefab;
		public float duration = 5f;

		public override SpellBehavior CreateRuntime()
		{
			AuraBehavior behavior = new();
			behavior.InitializeDefinition(this);

			return behavior;
		}
	}
}