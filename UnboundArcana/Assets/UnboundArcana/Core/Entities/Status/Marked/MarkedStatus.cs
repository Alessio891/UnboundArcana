using UnityEngine;

namespace UnboundArcana.Core.Entities.Statuses
{
	public class MarkedStatus : StatusInstance
	{
		public MarkedStatus(
			StatusDefinition definition)
			: base(definition)
		{
		}

		public override void Tick(float deltaTime)
		{
			base.Tick(deltaTime);

			Debug.Log("Ticking marked status");
		}
	}
}