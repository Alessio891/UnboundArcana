using UnityEngine;
using UnboundArcana.Core.Combat;

namespace UnboundArcana.Sandbox
{
	public class EnemyWaveSpawner : MonoBehaviour
	{
		[SerializeField] private GameObject enemyPrefab;
		[SerializeField] private Transform player;

		[SerializeField] private int maxEnemies = 20;
		[SerializeField] private float spawnInterval = 2f;
		[SerializeField] private float spawnRadius = 8f;

		private float timer;
		private int activeEnemies;

		private void Update()
		{
			timer += Time.deltaTime;

			if (timer >= spawnInterval)
			{
				timer = 0f;

				if (activeEnemies < maxEnemies)
				{
					SpawnEnemy();
				}
			}
		}

		private void SpawnEnemy()
		{
			Vector2 direction = Random.insideUnitCircle.normalized;

			Vector3 position =
				player.position +
				new Vector3(
					direction.x,
					direction.y,
					0f
				) *
				spawnRadius;

			GameObject instance = Instantiate(
				enemyPrefab,
				position,
				Quaternion.identity
			);

			TargetDummy dummy =
				instance.GetComponent<TargetDummy>();
			dummy.OnDeath += () =>
			{
				activeEnemies--;
			};
			if (dummy != null)
			{
				dummy.Initialize(player);
			}

			activeEnemies++;
		}
	}
}