using UnboundArcana.Core.Events;
using UnboundArcana.Core.Runtime;
using UnityEngine;

namespace UnboundArcana.Sandbox
{
	public class EnemyWaveSpawner : MonoBehaviour
	{
		[SerializeField] private GameObject[] enemyPrefabs;
		[SerializeField] private Transform player;

		[SerializeField] private int enemiesPerWave = 10;
		[SerializeField] private float waveDelay = 5f;
		[SerializeField] private float spawnInterval = 0.5f;
		[SerializeField] private float spawnRadius = 8f;

		[SerializeField] private float healthScaling = 0.15f;
		[SerializeField] private float speedScaling = 0.05f;

		private float spawnTimer;
		private int waveIndex = 0;

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
			waveIndex++;

			enemiesToSpawn = enemiesPerWave;

			spawningWave = true;
			waitingForNextWave = false;

			GameEvents?.Publish(
				new WaveStartedEvent(waveIndex)
			);

			Debug.Log($"Starting wave {waveIndex}");
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

			GameObject prefab =
				enemyPrefabs[
					Random.Range(0, enemyPrefabs.Length)
				];

			GameObject instance = Instantiate(
				prefab,
				position,
				Quaternion.identity
			);

			TargetDummy dummy =
				instance.GetComponent<TargetDummy>();

			if (dummy != null)
			{
				(float healthMultiplier, float speedMultiplier) =
					GetEnemyModifiers();

				dummy.Initialize(
					player,
					GameEvents,
					healthMultiplier,
					speedMultiplier
				);

				dummy.OnDeath += OnEnemyDeath;
			}

			enemiesRemaining++;
		}

		private (float health, float speed) GetEnemyModifiers()
		{
			float healthMultiplier =
				1f + (waveIndex - 1) * healthScaling;

			float speedMultiplier =
				1f + (waveIndex - 1) * speedScaling;

			float roll = Random.value;

			if (roll < 0.15f)
			{
				healthMultiplier *= 0.5f;
				speedMultiplier *= 1.8f;
			}
			else if (roll > 0.85f)
			{
				healthMultiplier *= 2f;
				speedMultiplier *= 0.7f;
			}

			return (
				healthMultiplier,
				speedMultiplier
			);
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
				new EncounterCompletedEvent(waveIndex)
			);

			Debug.Log("Wave completed");
		}
	}
}