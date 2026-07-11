using UnityEngine;
using UnboundArcana.Spells.Runtime;

namespace UnboundArcana.Spells.Behaviors.Beam
{
	[CreateAssetMenu(menuName = "Unbound Arcana/Behaviors/Beam")]
	public class BeamBehaviorDefinition : SpellBehaviorDefinition
	{
		public GameObject beamPrefab;

		public override SpellBehavior CreateRuntime()
		{
			BeamBehavior behavior = new();
			behavior.InitializeDefinition(this);

			return behavior;
		}
	}
}