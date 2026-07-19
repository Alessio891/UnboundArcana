using UnityEngine;
using UnboundArcana.Core.Entities.Statuses;

namespace UnboundArcana.Core.Entities.Statuses
{
	[CreateAssetMenu(menuName = "Unbound Arcana/Statuses/Chilled")]
	public class ChilledStatusDefinition : StatusDefinition
	{
		public FrozenStatusDefinition frozenStatus;
		public override StatusInstance CreateRuntime()
		{
			return new ChilledStatus(this);
		}
	}
}