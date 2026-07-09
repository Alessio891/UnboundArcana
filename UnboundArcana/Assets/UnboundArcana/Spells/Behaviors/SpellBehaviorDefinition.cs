using UnityEngine;
using UnboundArcana.Spells.Runtime;

namespace UnboundArcana.Spells.Behaviors
{
	public abstract class SpellBehaviorDefinition : ScriptableObject
	{
		public abstract SpellBehavior CreateRuntime();
	}
}