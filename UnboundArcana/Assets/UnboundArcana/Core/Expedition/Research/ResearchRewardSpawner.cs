using System.Collections;
using System.Collections.Generic;
using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Rooms;
using UnityEngine;

namespace UnboundArcana.Core.Research
{
	public class ResearchRewardSpawner
	{
		private readonly IReadOnlyList<ResearchDefinition> availableResearch;
		private readonly ResearchPickup pickupPrefab;
		private readonly int rewardCount;
		private readonly List<ResearchPickup> spawnedPickups = new();

		public ResearchRewardSpawner(IReadOnlyList<ResearchDefinition> availableResearch, ResearchPickup pickupPrefab, int rewardCount)
		{
			this.availableResearch = availableResearch;
			this.pickupPrefab = pickupPrefab;
			this.rewardCount = rewardCount;
		}

		public IEnumerator Spawn(RoomInstance room, Entity player)
		{
			Clear();

			if (availableResearch == null || availableResearch.Count == 0 || pickupPrefab == null || room == null || player == null)
			{
				yield break;
			}

			RoomSection section = room.GetSectionAtWorldPosition(player.transform.position);

			if (section == null)
			{
				Debug.LogWarning("No section found for research reward");
				yield break;
			}

			RoomMarker marker = section.GetComponentInChildren<RoomMarker>();

			if (marker == null)
			{
				Debug.LogWarning("No research reward marker found");
				yield break;
			}

			for (int i = 0; i < rewardCount; i++)
			{
				ResearchDefinition definition = availableResearch[Random.Range(0, availableResearch.Count)];
				Vector3 spawnPosition = FindSpawnPosition(section, marker.transform.position);
				ResearchPickup pickup = Object.Instantiate(pickupPrefab, spawnPosition, Quaternion.identity);

				pickup.Initialize(definition);
				pickup.transform.localScale = Vector3.zero;
				spawnedPickups.Add(pickup);

				yield return new WaitForSeconds(0.2f);

				iTween.ScaleTo(pickup.gameObject, iTween.Hash("scale", Vector3.one, "time", 0.7f, "easeType", "easeOutElastic"));

				yield return new WaitForSeconds(0.5f);
			}

			yield return new WaitForSeconds(1f);
		}

		public void Clear()
		{
			foreach (ResearchPickup pickup in spawnedPickups)
			{
				if (pickup != null)
				{
					Object.Destroy(pickup.gameObject);
				}
			}

			spawnedPickups.Clear();
		}

		private Vector3 FindSpawnPosition(RoomSection section, Vector3 origin)
		{
			Vector3 spawnPosition = origin;

			for (int attempt = 0; attempt < 100; attempt++)
			{
				Vector2 randomOffset = Random.onUnitCircle * 1.2f;
				spawnPosition = origin + new Vector3(randomOffset.x, randomOffset.y, 0f);

				if (section.ContainsWorldPosition(spawnPosition))
				{
					break;
				}
			}

			return spawnPosition;
		}
	}
}
