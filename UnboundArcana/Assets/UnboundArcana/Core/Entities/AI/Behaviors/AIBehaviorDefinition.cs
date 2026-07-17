using UnboundArcana.Core.Entities.AI;
using UnityEngine;

namespace UnboundArcana.Core.Entities.AI
{
	public abstract class AIBehaviorDefinition : ScriptableObject
	{
		public abstract AIState CreateInitialState();
	}
}

[CreateAssetMenu(
	menuName = "Unbound Arcana/AI/Chase Attack Behavior"
)]
public class ChaseAttackBehaviorDefinition : AIBehaviorDefinition
{
	public override AIState CreateInitialState()
	{
		return new IdleState();
	}
}