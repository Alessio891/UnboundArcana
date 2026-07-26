using System.Collections;
using System.Collections.Generic;
using UnboundArcana.Core.Camera;
using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Research;
using UnboundArcana.Core.Rooms;
using UnboundArcana.Core.Runtime;
using UnboundArcana.Player;
using UnboundArcana.Spells.Modules;
using UnityEngine;

namespace UnboundArcana.Core.Expedition
{
	public class ExpeditionRuntimeController : MonoBehaviour
	{
		[SerializeField]
		private EntityDefinition playerDefinition;

		[SerializeField]
		private RoomDefinition startingRoom;

		[SerializeField]
		private Transform roomParent;

		[SerializeField]
		private List<SpellModuleDefinition> availableRewardModules = new();
		private readonly ResearchSystem researchSystem = new();
		[SerializeField]
		private List<ResearchDefinition> availableResearch = new();
		[SerializeField]
		private ResearchPickup researchPickupPrefab;
		List<ResearchPickup> currentSpawnedResearches = new();

		[SerializeField]
		private int rewardCount = 3;


		private Entity player;
		public Entity Player => player;

		private List<SpellModuleDefinition> currentRewards = new();

		public ExpeditionState State { get; private set; }

		public IReadOnlyList<SpellModuleDefinition> CurrentRewards =>
			currentRewards;

		private RoomInstance currentRoom;

		public static ExpeditionRuntimeController instance;
		public static ExpeditionRuntimeController Instance => instance;
		private void Awake()
		{
			if (instance != null && instance != this)
			{
				Destroy(gameObject);
				return;
			}
			instance = this;
		}
		private void OnEnable()
		{
			if (GameRuntimeManager.Instance == null)
				return;

			GameRuntimeManager.Instance.Events.Subscribe<RoomStartedEvent>(
				OnRoomStarted);

			GameRuntimeManager.Instance.Events.Subscribe<RoomCompletedEvent>(
				OnRoomCompleted);

			GameRuntimeManager.Instance.Events.Subscribe<ResearchCollectedEvent>(OnResearchCollected);
		}

		private void OnResearchCollected(ResearchCollectedEvent evt)
		{
			foreach(var r in currentSpawnedResearches) {
				Destroy(r.gameObject);
			}
			currentSpawnedResearches.Clear();
			GameSession.Instance.Player.AddResearch(evt.Research);
			StartCoroutine(AdvanceToNextRoom());
		}

		private void OnDisable()
		{
			if (GameRuntimeManager.Instance == null)
				return;

			GameRuntimeManager.Instance.Events.Unsubscribe<RoomStartedEvent>(
				OnRoomStarted);

			GameRuntimeManager.Instance.Events.Unsubscribe<RoomCompletedEvent>(
				OnRoomCompleted);
			GameRuntimeManager.Instance.Events.Unsubscribe<ResearchCollectedEvent>(OnResearchCollected);
		}
		public IEnumerator AdvanceToNextRoom()
		{
			if (State != ExpeditionState.Reward)
			{
				Debug.LogWarning(
					"Cannot advance room while expedition is not in Reward state.");

				yield break;
			}

			if (currentRoom == null)
			{
				Debug.LogWarning(
					"No current room.");

				yield break;
			}

			if (!currentRoom.IsCompleted)
			{
				Debug.LogWarning(
					"Current room is not completed.");

				yield break;
			}
			yield return StartCoroutine(currentRoom.StartDeconstructEffect());
			Debug.Log(
				$"Advancing from room {currentRoom.Definition.RoomId}");

			RoomInstance nextRoom =
				GameRuntimeManager.Instance.Rooms.GenerateRoom(
					startingRoom,
					roomParent);

			if (nextRoom == null)
			{
				Debug.LogError(
					"Failed to generate next room.");

				yield break;
			}
			researchSystem.ActivateCompletedResearches(GameSession.Instance.Player);
			MovePlayerToRoom(
				nextRoom);

			currentRoom = nextRoom;

			SetState(
				ExpeditionState.EnteringRoom);

			GameRuntimeManager.Instance.Rooms.StartRoom();
		}
		private void MovePlayerToRoom(
	RoomInstance room)
		{
			List<RoomMarker> markers =
				new(room.GetMarkers(
					RoomMarkerType.PlayerStart));

			if (markers.Count == 0)
			{
				Debug.LogWarning(
					"No PlayerStart marker found.");

				return;
			}

			player.transform.position =
				markers[0].transform.position;
			MainCameraManager.Instance.SnapToTarget();
		}
		public void StartExpedition()
		{
			if (State != ExpeditionState.None)
				return;

			SetState(ExpeditionState.Preparing);

			EnsureSession();

			RoomInstance room =
				GenerateStartingRoom();

			if (room == null)
			{
				SetState(ExpeditionState.Failed);
				return;
			}

			SetState(ExpeditionState.EnteringRoom);



			GameRuntimeManager.Instance.Rooms.StartRoom();
			SpawnPlayer(room);
			if (player == null)
			{
				SetState(ExpeditionState.Failed);
				return;
			}
		}


		private void EnsureSession()
		{
			if (GameSession.Instance.Player != null)
				return;

			GameSession.Instance.CreatePlayer(
				playerDefinition);
		}


		private RoomInstance GenerateStartingRoom()
		{
			if (startingRoom == null)
			{
				Debug.LogWarning(
					"No starting room assigned.");

				return null;
			}

			return GameRuntimeManager.Instance.Rooms.GenerateRoom(
				startingRoom,
				roomParent);
		}


		private void SpawnPlayer(RoomInstance room)
		{
			List<RoomMarker> markers =
				new(room.GetMarkers(
					RoomMarkerType.PlayerStart));

			if (markers.Count == 0)
			{
				Debug.LogWarning(
					"No PlayerStart marker found.");

				return;
			}

			Vector3 position =
				markers[0].transform.position;

			player =
				GameRuntimeManager.Instance.PlayerSpawner.Spawn(
					GameSession.Instance.Player,
					position,
					null);

			if (player != null)
			{
				MovePlayerToRoom(room);
				MainCameraManager.Instance.SetFollowTarget(player.transform);
			}
		}


		private void OnRoomStarted(
			RoomStartedEvent evt)
		{
			if (State != ExpeditionState.EnteringRoom)
				return;
			
			currentRoom = evt.Room;
			SetState(ExpeditionState.RoomActive);
			
		}


		private void OnRoomCompleted(RoomCompletedEvent evt)
		{
			if (State != ExpeditionState.RoomActive)
				return;


			SetState(ExpeditionState.Reward);
			StartCoroutine(SpawnResearchRewards());

			GameRuntimeManager.Instance.Events.Publish(
				new ExpeditionRewardStartedEvent());
		}
		private IEnumerator SpawnResearchRewards()
		{
			foreach(var r in currentSpawnedResearches) {
				Destroy(r.gameObject);
			}
			currentSpawnedResearches.Clear();
			RoomSection section =
				currentRoom.GetSectionAtWorldPosition(
					player.transform.position);

			if (section == null)
			{
				Debug.LogWarning(
					"No section found for research reward");

				yield break;
			}

			RoomMarker marker =
				section.GetComponentInChildren<RoomMarker>();

			if (marker == null)
			{
				Debug.LogWarning(
					"No research reward marker found");

				yield break;
			}
			player.GetComponent<PlayerInput>().SetInputEnabled(false);
			for (int i = 0; i < rewardCount; i++)
			{
				ResearchDefinition definition =
					availableResearch[
						Random.Range(
							0,
							availableResearch.Count)];

				Vector2 rndOffset = Random.insideUnitCircle * 1.2f;
				Vector3 offset = new Vector3(rndOffset.x, rndOffset.y, 0);
				ResearchPickup pickup =
					Instantiate(
						researchPickupPrefab,
						marker.transform.position + offset,
						Quaternion.identity);

				pickup.Initialize(definition);
				currentSpawnedResearches.Add(pickup);
				pickup.transform.localScale = Vector3.zero;
				MainCameraManager.Instance.SetFollowTarget(pickup.transform);
				yield return new WaitForSeconds(0.4f);
				iTween.ScaleTo(pickup.gameObject, iTween.Hash("scale", Vector3.one, "time", 0.7f, "easeType", "easeOutElastic"));
				yield return new WaitForSeconds(0.9f);
				Debug.Log(
					$"Spawned research reward: {definition.DisplayName}");
			}
			yield return new WaitForSeconds(1.0f);
			MainCameraManager.Instance.SetFollowTarget(player.transform);
			player.GetComponent<PlayerInput>().SetInputEnabled(true);
		}

		private void GenerateRewards()
		{
			currentRewards =
				GameRuntimeManager.Instance.Rewards
					.GenerateModuleRewards(
						availableRewardModules,
						rewardCount);

			Debug.Log(
				$"Generated {currentRewards.Count} expedition rewards");

			foreach (var reward in currentRewards)
			{
				Debug.Log(
					$"Reward: {reward.name} ({reward.Rarity})");
			}
		}


		private void SetState(
			ExpeditionState state)
		{
			if (State == state)
				return;

			State = state;

			Debug.Log(
				$"Expedition state: {State}");
		}
	}
}