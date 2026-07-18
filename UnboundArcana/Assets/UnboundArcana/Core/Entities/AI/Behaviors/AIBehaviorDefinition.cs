using UnityEngine;

namespace UnboundArcana.Core.Entities.AI
{
	public abstract class AIBehaviorDefinition
		: ScriptableObject
	{
		public abstract AIBehavior CreateBehavior();
	}
}