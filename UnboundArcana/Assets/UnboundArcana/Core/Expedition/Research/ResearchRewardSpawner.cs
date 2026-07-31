using System.Collections;
using System.Collections.Generic;
using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Rooms;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnboundArcana.Core.Research
{
	public class ResearchRewardSpawner
	{
		private const float MinimumRewardSpacing = 1.1f;
		private const float PlacementClearance = 0.1f;
		private const int PlacementSearchRings = 12;

		private readonly IReadOnlyList<ResearchDefinition> availableResearch;
		private readonly ResearchPickup pickupPrefab;
		private readonly int rewardCount;
		private readonly List<ResearchPickup> spawnedPickups = new();

		public bool HasSpawnedRewards => spawnedPickups.Count > 0;

		public ResearchRewardSpawner(IReadOnlyList<ResearchDefinition> availableResearch, ResearchPickup pickupPrefab, int rewardCount)
		{
			this.availableResearch = availableResearch;
			this.pickupPrefab = pickupPrefab;
			this.rewardCount = rewardCount;
		}

		public IEnumerator Spawn(RoomInstance room, Entity player)
		{
			Clear();

			if (availableResearch == null || availableResearch.Count == 0 || pickupPrefab == null || rewardCount <= 0 || room == null || player == null)
			{
				yield break;
			}

			Vector3 playerPosition = player.transform.position;
			float placementRadius = GetPlacementRadius();
			RoomSection section = room.GetSectionAtWorldPosition(playerPosition);
			List<Vector3> spawnPositions;

			if (section == null)
			{
				Debug.LogWarning("No section found for research rewards. Spawning a fallback formation around the player.");
				spawnPositions = BuildFormationPositions(playerPosition, placementRadius, 0f);
			}
			else
			{
				Vector3 rewardOrigin = FindRewardOrigin(section, playerPosition);

				if (!TryFindSpawnPositions(section, playerPosition, rewardOrigin, placementRadius, out spawnPositions))
				{
					Debug.LogWarning("No fully valid formation found for research rewards. Spawning a fallback formation around the player.");
					spawnPositions = BuildFormationPositions(playerPosition, placementRadius, 0f);
				}
			}

			for (int i = 0; i < spawnPositions.Count; i++)
			{
				ResearchDefinition definition = availableResearch[Random.Range(0, availableResearch.Count)];
				ResearchPickup pickup = Object.Instantiate(pickupPrefab, spawnPositions[i], Quaternion.identity);

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

		private Vector3 FindRewardOrigin(RoomSection section, Vector3 playerPosition)
		{
			RoomMarker[] markers = section.GetComponentsInChildren<RoomMarker>();

			foreach (RoomMarker marker in markers)
			{
				if (marker.Type == RoomMarkerType.RewardSpawn)
				{
					return marker.transform.position;
				}
			}

			Debug.LogWarning($"Room section '{section.SectionId}' has no reward marker. Using the player's position.");
			return playerPosition;
		}

		private bool TryFindSpawnPositions(RoomSection section, Vector3 pathOrigin, Vector3 searchOrigin, float placementRadius, out List<Vector3> positions)
		{
			positions = new List<Vector3>(rewardCount);

			if (section.SectionGrid == null)
			{
				return false;
			}

			float searchStep = Mathf.Max(Mathf.Min(section.SectionGrid.cellSize.x, section.SectionGrid.cellSize.y), placementRadius);

			foreach (Vector2 centerOffset in GetSearchOffsets(searchStep))
			{
				Vector3 center = searchOrigin + new Vector3(centerOffset.x, centerOffset.y, 0f);

				if (TryBuildFormation(section, pathOrigin, center, placementRadius, 0f, positions))
				{
					return true;
				}

				if (TryBuildFormation(section, pathOrigin, center, placementRadius, Mathf.PI / rewardCount, positions))
				{
					return true;
				}
			}

			positions.Clear();
			return false;
		}

		private bool TryBuildFormation(RoomSection section, Vector3 pathOrigin, Vector3 center, float placementRadius, float rotation, List<Vector3> positions)
		{
			positions.Clear();
			List<Vector3> candidates = BuildFormationPositions(center, placementRadius, rotation);

			foreach (Vector3 position in candidates)
			{
				if (!IsValidPosition(section, pathOrigin, position, placementRadius))
				{
					positions.Clear();
					return false;
				}

				positions.Add(position);
			}

			return true;
		}

		private List<Vector3> BuildFormationPositions(Vector3 center, float placementRadius, float rotation)
		{
			List<Vector3> positions = new(rewardCount);

			if (rewardCount == 1)
			{
				positions.Add(center);
				return positions;
			}

			float spacing = Mathf.Max(MinimumRewardSpacing, placementRadius * 2f);
			float formationRadius = spacing / (2f * Mathf.Sin(Mathf.PI / rewardCount));
			float angleStep = Mathf.PI * 2f / rewardCount;
			float startAngle = Mathf.PI * 0.5f + rotation;

			for (int i = 0; i < rewardCount; i++)
			{
				float angle = startAngle + angleStep * i;
				positions.Add(center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * formationRadius);
			}

			return positions;
		}

		private bool IsValidPosition(RoomSection section, Vector3 origin, Vector3 position, float placementRadius)
		{
			if (!ContainsCircle(section, position, placementRadius))
			{
				return false;
			}

			Collider2D[] overlaps = Physics2D.OverlapCircleAll(position, placementRadius);

			foreach (Collider2D overlap in overlaps)
			{
				if (IsBlockingCollider(overlap))
				{
					return false;
				}
			}

			Vector2 path = position - origin;
			RaycastHit2D[] pathHits = Physics2D.CircleCastAll(origin, placementRadius, path.normalized, path.magnitude);

			foreach (RaycastHit2D hit in pathHits)
			{
				if (IsBlockingCollider(hit.collider))
				{
					return false;
				}
			}

			return true;
		}

		private bool IsBlockingCollider(Collider2D collider)
		{
			return collider != null && !collider.isTrigger && (collider is TilemapCollider2D || collider.attachedRigidbody == null);
		}

		private bool ContainsCircle(RoomSection section, Vector3 center, float radius)
		{
			if (!section.ContainsWorldPosition(center))
			{
				return false;
			}

			for (int i = 0; i < 8; i++)
			{
				float angle = i * Mathf.PI * 0.25f;
				Vector3 edge = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;

				if (!section.ContainsWorldPosition(edge))
				{
					return false;
				}
			}

			return true;
		}

		private float GetPlacementRadius()
		{
			CircleCollider2D circle = pickupPrefab.GetComponent<CircleCollider2D>();

			if (circle == null)
			{
				return 0.5f;
			}

			float scale = Mathf.Max(Mathf.Abs(pickupPrefab.transform.lossyScale.x), Mathf.Abs(pickupPrefab.transform.lossyScale.y));
			return (circle.radius + circle.offset.magnitude) * scale + PlacementClearance;
		}

		private IEnumerable<Vector2> GetSearchOffsets(float step)
		{
			yield return Vector2.zero;

			for (int ring = 1; ring <= PlacementSearchRings; ring++)
			{
				for (int x = -ring; x <= ring; x++)
				{
					yield return new Vector2(x, ring) * step;
					yield return new Vector2(x, -ring) * step;
				}

				for (int y = ring - 1; y >= -ring + 1; y--)
				{
					yield return new Vector2(ring, y) * step;
					yield return new Vector2(-ring, y) * step;
				}
			}
		}
	}
}
