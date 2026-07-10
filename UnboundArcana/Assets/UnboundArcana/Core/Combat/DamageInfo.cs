using UnityEngine;

namespace UnboundArcana.Core.Combat
{
	public struct DamageInfo
	{
		public GameObject Source { get; }
		public float Amount { get; }
		public DamageType Type { get; }

		public DamageInfo(
			GameObject source,
			float amount,
			DamageType type)
		{
			Source = source;
			Amount = amount;
			Type = type;
		}
	}
}