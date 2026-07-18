using UnityEngine;

namespace UnboundArcana.Core.Entities.AI
{
	[CreateAssetMenu(menuName = "SUnbound Arcana/AI/TargetDummyAI")]
	public class TargetDummyAIDefinition : AIBehaviorDefinition
	{
		public override AIBehavior CreateBehavior()
		{
			return new TargetDummyAI();
		}
	}
}