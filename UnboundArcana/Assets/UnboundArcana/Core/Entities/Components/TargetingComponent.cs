using System;
using UnityEngine;

namespace UnboundArcana.Core.Entities
{
	public class TargetingComponent : MonoBehaviour
	{
		public event Action<Entity> TargetChanged;

		public Entity CurrentTarget { get; private set; }


		public void SetTarget(Entity target)
		{
			if (CurrentTarget == target)
				return;

			CurrentTarget = target;

			TargetChanged?.Invoke(CurrentTarget);
		}


		public void ClearTarget()
		{
			if (CurrentTarget == null)
				return;

			CurrentTarget = null;

			TargetChanged?.Invoke(null);
		}
	}
}