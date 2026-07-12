using UnboundArcana.Core.Events;
using UnboundArcana.Core.Runtime;
using UnityEngine;

namespace UnboundArcana.Sandbox
{
	public class EnemyWaveSpawner : MonoBehaviour
	{
		[SerializeField] private GameObject enemyPrefab;
		[SerializeField] private Transform player;

		[SerializeField] private int enemiesPerWave = 10;
		[SerializeField] private float waveDelay = 5f;
		[SerializeField] private float spawnInterval = 0.5f;
		[SerializeField] private float spawnRadius = 8f;

		private float spawnTimer;
		private float waveTimer;

		private int enemiesRemaining;
		private int enemiesToSpawn;

		private bool spawningWave;
		private bool waitingForNextWave;

		public SpellRuntimeManager Runtime;
		public GameEventBus GameEvents => Runtime?.GameEvents;

		private void Start()
		{
			StartWave();
			Runtime.GameEvents.Subscribe<RewardSelectedEvent>(
				OnRewardSelected
			);
		}

		private void Update()
		{
			if (waitingForNextWave)
			{
				return;
			}

			if (!spawningWave)
			{
				return;
			}

			spawnTimer -= Time.deltaTime;

			if (spawnTimer <= 0f && enemiesToSpawn > 0)
			{
				spawnTimer = spawnInterval;
				SpawnEnemy();

				enemiesToSpawn--;

				if (enemiesToSpawn == 0)
				{
					spawningWave = false;
				}
			}
		}

		private void StartWave()
		{
			Debug.Log("Starting wave");
			enemiesToSpawn = enemiesPerWave;
			spawningWave = true;
			waitingForNextWave = false;
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

			if (dummy != null)
			{
				dummy.Initialize(player);

				dummy.OnDeath += OnEnemyDeath;
			}

			enemiesRemaining++;
		}
		private void OnRewardSelected(
			RewardSelectedEvent eventData)
		{
			if (!waitingForNextWave)
			{
				return;
			}

			waitingForNextWave = false;

			StartWave();
		}
		private void OnEnemyDeath()
		{
			enemiesRemaining--;

			if (enemiesRemaining <= 0 && !spawningWave)
			{
				CompleteWave();
			}
		}

		private void CompleteWave()
		{
			waitingForNextWave = true;

			GameEvents?.Publish(
				new EncounterCompletedEvent()
			);

			Debug.Log("Wave completed");
		}
	}
}