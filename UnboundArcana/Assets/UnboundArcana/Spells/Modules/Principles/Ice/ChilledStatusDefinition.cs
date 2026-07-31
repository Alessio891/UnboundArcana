using UnityEngine;
using UnboundArcana.Core.Entities.Statuses;

namespace UnboundArcana.Core.Entities.Statuses
{
	[CreateAssetMenu(menuName = "Unbound Arcana/Statuses/Chilled")]
	public class ChilledStatusDefinition : StatusDefinition
	{
		public FrozenStatusDefinition frozenStatus;
		[Range(0f, 0.5f)] public float slowPerStack = 0.08f;
		public int stacksToFreeze = 5;
		public override StatusInstance CreateRuntime()
		{
			return new ChilledStatus(this);
		}
	}
}
