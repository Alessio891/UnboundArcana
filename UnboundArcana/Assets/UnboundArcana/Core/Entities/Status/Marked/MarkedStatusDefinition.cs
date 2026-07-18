using UnityEngine;

namespace UnboundArcana.Core.Entities.Statuses
{
	[CreateAssetMenu(menuName = "Unbound Arcana/Statuses/Marked")]
	public class MarkedStatusDefinition : StatusDefinition
	{
		public override StatusInstance CreateRuntime()
		{
			return new MarkedStatus(this);
		}
	}
}