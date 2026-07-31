using UnboundArcana.Core.Entities.Statuses;
using UnityEngine;

namespace UnboundArcana.Spells.Modules.Principles
{
	[CreateAssetMenu(menuName = "Unbound Arcana/Statuses/Corroded")]
	public class CorrodedStatusDefinition : StatusDefinition
	{
		public float damagePerStack = 0.5f;
		public float damageInterval = 0.75f;

		public override StatusInstance CreateRuntime()
		{
			return new CorrodedStatus(this);
		}
	}
}
