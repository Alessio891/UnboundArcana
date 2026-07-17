using UnboundArcana.Core.Entities.AI;
using UnboundArcana.Core.Entities.AI.Attacks;
using UnboundArcana.Core.Entities.AI.Steering;
using UnityEngine;

namespace UnboundArcana.Core.Entities.AI
{
	public abstract class AIBehaviorDefinition : ScriptableObject
	{
		public abstract AIBehavior CreateBehavior(
			AIController controller);
	}
}
[CreateAssetMenu(
	menuName = "Unbound Arcana/AI/Behaviors/Chase Attack"
)]
public class ChaseAttackBehaviorDefinition
	: AIBehaviorDefinition
{
	public override AIBehavior CreateBehavior(
		AIController controller)
	{
		return new ChaseAttackBehavior(
			controller,
			new DirectChaseSteering(controller),
			new ContactDamageAttack(controller)
		);
	}
}
public class ChaseAttackBehavior
	: AIBehavior
{
	public ChaseAttackBehavior(
			AIController controller,
			SteeringStrategy steering,
			AttackStrategy attack)
			: base(
				controller,
				steering,
				attack)
	{
	}

	protected override AIState CreateInitialState()
	{
		return new IdleState();
	}
}