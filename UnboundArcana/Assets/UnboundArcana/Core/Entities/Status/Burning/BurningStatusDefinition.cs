using UnityEngine;
using UnboundArcana.Core.Entities.Statuses;

namespace UnboundArcana.Core.Entities.Statuses
{
	[CreateAssetMenu(menuName = "Unbound Arcana/Statuses/Burning")]
	public class BurningStatusDefinition : StatusDefinition
	{
		public float secondsBetweenTicks = 1.0f;
		public float magnitude = 1.0f;
		public override StatusInstance CreateRuntime()
		{
			return new BurningStatus(this);
		}
	}
}