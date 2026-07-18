using UnityEngine;

namespace UnboundArcana.Core.Entities.Statuses
{
	public abstract class StatusDefinition : ScriptableObject
	{
		[SerializeField]
		private float duration = 5f;

		[SerializeField]
		private int maxStacks = 1;

		public float Duration => duration;

		public int MaxStacks => maxStacks;

		public abstract StatusInstance CreateRuntime();
	}
}