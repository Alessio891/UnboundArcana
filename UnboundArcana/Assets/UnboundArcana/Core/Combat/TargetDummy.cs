using UnboundArcana.Core.Combat;
using UnityEngine;

namespace UnboundArcana.Sandbox
{
	public class TargetDummy : MonoBehaviour, IDamageable
	{
		[SerializeField] private float health = 100f;

		public void TakeDamage(DamageInfo damage)
		{
			health -= damage.Amount;

			Debug.Log(
				$"{name} took {damage.Amount} {damage.Type} damage from {damage.Source.name}. HP: {health}"
			);

			if (health <= 0)
			{
				Debug.Log($"{name} died");
			}
		}
	}
}