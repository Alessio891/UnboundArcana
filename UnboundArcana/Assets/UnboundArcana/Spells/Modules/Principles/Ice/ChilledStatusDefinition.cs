using UnityEngine;
using UnboundArcana.Core.Entities.Statuses;

namespace UnboundArcana.Core.Entities.Statuses
{
	[CreateAssetMenu(menuName = "Unbound Arcana/Statuses/Chilled")]
	public class ChilledStatusDefinition : StatusDefinition
	{
		public override StatusInstance CreateRuntime()
		{
			return new ChilledStatus(this);
		}
	}
}