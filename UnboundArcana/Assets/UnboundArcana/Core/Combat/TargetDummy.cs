using UnboundArcana.Core.Combat;
using UnboundArcana.Core.Events;
using UnityEngine;

namespace UnboundArcana.Sandbox
{
	public class TargetDummy : MonoBehaviour, IDamageable
	{
		[SerializeField] private float health = 100f;
		[SerializeField] private float moveSpeed = 2f;

		[SerializeField] private float contactRange = 0.8f;
		[SerializeField] private float contactDamage = 10f;
		[SerializeField] private float damageCooldown = 1f;

		private float damageTimer;

		private bool isDead;
		[SerializeField] private Transform target;

		public System.Action OnDeath;

		private GameEventBus gameEvents;

		public void Initialize(
	Transform target,
	GameEventBus gameEvents,
	float healthMultiplier,
	float speedMultiplier)
		{
			this.target = target;
			this.gameEvents = gameEvents;

			health *= healthMultiplier;
			moveSpeed *= speedMultiplier;
		}

		private void Update()
		{
			if (isDead || target == null)
			{
				return;
			}

			damageTimer -= Time.deltaTime;

			Vector3 direction = target.position - transform.position;
			direction.z = 0f;

			if (direction.sqrMagnitude > contactRange * contactRange)
			{
				transform.position +=
					direction.normalized *
					moveSpeed *
					Time.deltaTime;
			}
			else
			{
				TryDamagePlayer();
			}
		}

		private void TryDamagePlayer()
		{
			if (damageTimer > 0f)
			{
				return;
			}

			damageTimer = damageCooldown;

			gameEvents?.Publish(
				new DamageEvent(
					gameObject,
					target.gameObject,
					contactDamage,
					DamageType.Physical
				)
			);
		}

		public void TakeDamage(DamageInfo damage)
		{
			if (isDead)
			{
				return;
			}

			health -= damage.Amount;

			if (health <= 0)
			{
				Die();
			}
		}

		private void Die()
		{
			isDead = true;

			OnDeath?.Invoke();

			gameEvents?.Publish(
				new EnemyKilledEvent(gameObject)
			);

			Destroy(gameObject);
		}
	}
}