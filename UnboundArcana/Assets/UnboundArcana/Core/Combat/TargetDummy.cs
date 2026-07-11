using UnboundArcana.Core.Combat;
using UnityEngine;

namespace UnboundArcana.Sandbox
{
	public class TargetDummy : MonoBehaviour, IDamageable
	{
		[SerializeField] private float health = 100f;

		private bool isDead;

		public void TakeDamage(DamageInfo damage)
		{
			if (isDead)
			{
				return;
			}

			health -= damage.Amount;

			Debug.Log(
				$"{name} took {damage.Amount} {damage.Type} damage from {damage.Source.name}. HP: {health}"
			);

			if (health <= 0)
			{
				Die();
			}
		}

		private void Die()
		{
			isDead = true;

			Debug.Log($"{name} died");

			Destroy(gameObject);
		}
	}
}