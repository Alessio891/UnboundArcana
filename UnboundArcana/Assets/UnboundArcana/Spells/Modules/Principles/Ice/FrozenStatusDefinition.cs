using UnityEngine;
using UnboundArcana.Core.Entities.Statuses;

namespace UnboundArcana.Core.Entities.Statuses
{
	[CreateAssetMenu(menuName = "Unbound Arcana/Statuses/Frozen")]
	public class FrozenStatusDefinition : StatusDefinition
	{
		public override StatusInstance CreateRuntime()
		{
			return new FrozenStatus(this);
		}
	}
}