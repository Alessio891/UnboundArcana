using UnboundArcana.Core.Entities.Statuses;
using UnityEngine;

namespace UnboundArcana.Spells.Modules.Principles
{
	[CreateAssetMenu(menuName = "Unbound Arcana/Statuses/Drenched")]
	public class DrenchedStatusDefinition : StatusDefinition
	{
		[Range(0f, 1f)]
		public float slow = 0.2f;

		public override StatusInstance CreateRuntime()
		{
			return new DrenchedStatus(this);
		}
	}
}
