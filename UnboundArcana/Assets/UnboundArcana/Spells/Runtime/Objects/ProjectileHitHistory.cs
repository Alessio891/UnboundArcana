using System.Collections.Generic;
using UnityEngine;

namespace UnboundArcana.Spells.Runtime.Objects
{
	public class ProjectileHitHistory
	{
		private readonly HashSet<GameObject> targets = new();

		public bool HasHit(GameObject target)
		{
			return targets.Contains(target);
		}

		public void Add(GameObject target)
		{
			targets.Add(target);
		}
	}
}