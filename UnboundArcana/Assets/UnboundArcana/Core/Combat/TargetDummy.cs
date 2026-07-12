using UnboundArcana.Core.Combat;
using UnityEngine;

namespace UnboundArcana.Sandbox
{
	public class TargetDummy : MonoBehaviour, IDamageable
	{
		[SerializeField] private float health = 100f;
		[SerializeField] private float moveSpeed = 2f;

		private bool isDead;
		[SerializeField] private Transform target;
		public System.Action OnDeath;
		public void Initialize(Transform target)
		{
			this.target = target;
		}

		private void Update()
		{
			if (isDead || target == null)
			{
				return;
			}

			Vector3 direction = target.position - transform.position;
			direction.z = 0f;

			if (direction.sqrMagnitude > 0.01f)
			{
				transform.position +=
					direction.normalized *
					moveSpeed *
					Time.deltaTime;
			}
		}

		public void TakeDamage(DamageInfo damage)
		{
			if (isDead)
			{
				return;
			}

			health -= damage.Amount;

			//Debug.Log(
			//	$"{name} took {damage.Amount} {damage.Type} damage from {damage.Source.name}. HP: {health}"
			//);

			if (health <= 0)
			{
				Die();
			}
		}

		private void Die()
		{
			isDead = true;

			OnDeath?.Invoke();

			Destroy(gameObject);
		}
	}
}