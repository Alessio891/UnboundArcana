using System.Collections;
using System.Collections.Generic;
using UnboundArcana.Core.Camera;
using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Entities.Events;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Research;
using UnboundArcana.Core.Rooms;
using UnboundArcana.Core.Runtime;
using UnboundArcana.Player;
using UnboundArcana.Spells.Modules;
using UnityEngine;

namespace UnboundArcana.Core.Expedition
{
	public class BehaviorActivationEvent
	{
		public BehaviorActivationEvent() { }
	}

	public class ExpeditionRuntimeController : MonoBehaviour
	{
		[SerializeField]
		private EntityDefinition playerDefinition;

		[SerializeField]
		private FloorDefinition startingFloor;

		[SerializeField]
		private Transform roomParent;


		[SerializeField]
		private List<SpellModuleDefinition> availableRewardModules = new();

		private readonly ResearchSystem researchSystem = new();


		[SerializeField]
		private List<ResearchDefinition> availableResearch = new();

		[SerializeField]
		private ResearchPickup researchPickupPrefab;

		private readonly List<ResearchPickup> currentSpawnedResearches = new();


		[SerializeField]
		private int rewardCount = 3;


		private Entity player;
		public Entity Player => player;


		private readonly List<SpellModuleDefinition> currentRewards = new();

		public IReadOnlyList<SpellModuleDefinition> CurrentRewards =>
			currentRewards;


		private RoomInstance currentRoom;

		private FloorInstance currentFloor;


		public ExpeditionState State { get; private set; }


		public static ExpeditionRuntimeController instance;
		public static ExpeditionRuntimeController Instance => instance;


		private bool isFirstRoom = true;


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

			GameRuntimeManager.Instance.Events.Subscribe<ResearchCollectedEvent>(
				OnResearchCollected);

			GameRuntimeManager.Instance.Events.Subscribe<EntityDeathEvent>(
				OnEntityDied);
		}


		private void OnDisable()
		{
			if (GameRuntimeManager.Instance == null)
				return;

			GameRuntimeManager.Instance.Events.Unsubscribe<RoomStartedEvent>(
				OnRoomStarted);

			GameRuntimeManager.Instance.Events.Unsubscribe<RoomCompletedEvent>(
				OnRoomCompleted);

			GameRuntimeManager.Instance.Events.Unsubscribe<ResearchCollectedEvent>(
				OnResearchCollected);

			GameRuntimeManager.Instance.Events.Unsubscribe<EntityDeathEvent>(
				OnEntityDied);
		}


		private void OnEntityDied(EntityDeathEvent @event)
		{
			GameSession.Instance.Player.AddKnowledge(50);
		}


		private void OnResearchCollected(
			ResearchCollectedEvent evt)
		{
			player.GetComponent<PlayerInput>()
				.SetInputEnabled(false);


			foreach (var r in currentSpawnedResearches)
			{
				Destroy(r.gameObject);
			}


			currentSpawnedResearches.Clear();


			GameSession.Instance.Player.AddResearch(
				evt.Research);


			StartCoroutine(
				AdvanceToNextRoom());
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


			yield return StartCoroutine(
				currentRoom.StartDeconstructEffect());


			bool hasNextRoom =
				currentFloor.Advance();


			if (!hasNextRoom)
			{
				Debug.Log(
					"Floor completed.");

				SetState(
					ExpeditionState.Completed);

				yield break;
			}


			RoomDefinition nextRoomDefinition =
				currentFloor.GetCurrentRoom();


			RoomInstance nextRoom =
				GenerateRoom(
					nextRoomDefinition);


			if (nextRoom == null)
			{
				Debug.LogError(
					"Failed to generate next room.");

				yield break;
			}


			researchSystem.ActivateCompletedResearches(
				GameSession.Instance.Player);


			currentRoom = nextRoom;


			MovePlayerToRoom(
				nextRoom);


			SetState(
				ExpeditionState.EnteringRoom);


			GameRuntimeManager.Instance.Rooms.StartRoom();
		}


		private RoomInstance GenerateRoom(
			RoomDefinition definition)
		{
			if (definition == null)
				return null;


			return GameRuntimeManager.Instance.Rooms.GenerateRoom(
				definition,
				roomParent);
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


			SetState(
				ExpeditionState.Preparing);


			EnsureSession();


			currentFloor =
				GameRuntimeManager.Instance.Floors.GenerateFloor(
					startingFloor);


			if (currentFloor == null)
			{
				SetState(
					ExpeditionState.Failed);

				return;
			}


			RoomDefinition firstRoom =
				currentFloor.GetCurrentRoom();


			RoomInstance room =
				GenerateRoom(
					firstRoom);


			if (room == null)
			{
				SetState(
					ExpeditionState.Failed);

				return;
			}


			SetState(
				ExpeditionState.EnteringRoom);


			currentRoom = room;


			GameRuntimeManager.Instance.Rooms.StartRoom();


			SpawnPlayer(
				room);


			if (player == null)
			{
				SetState(
					ExpeditionState.Failed);

				return;
			}


			player.GetComponentInChildren<SpriteRenderer>()
				.material.SetFloat(
					"_Progress",
					0.0f);


			player.GetComponent<PlayerInput>()
				.SetInputEnabled(false);
		}


		private void EnsureSession()
		{
			if (GameSession.Instance.Player != null)
				return;


			GameSession.Instance.CreatePlayer(
				playerDefinition);
		}


		private void SpawnPlayer(
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

				MainCameraManager.Instance.SetFollowTarget(
					player.transform);
			}
		}


		private void OnRoomStarted(
			RoomStartedEvent evt)
		{
			if (State != ExpeditionState.EnteringRoom)
				return;


			currentRoom = evt.Room;

			StartCoroutine(
				RoomStartRoutine());
		}


		private IEnumerator RoomStartRoutine()
		{
			if (isFirstRoom)
			{
				float value = 0.0f;

				while (true)
				{
					player.GetComponentInChildren<SpriteRenderer>()
						.material.SetFloat(
							"_Progress",
							value);

					value += Time.deltaTime * 1.5f;


					if (value >= 1.0f)
						break;


					yield return null;
				}


				GameRuntimeManager.Instance.Events.Publish(
					new ShowDialogueEvent(
						"Am I inside the tower? Nice, let's focus on experimentations then!",
						null));


				yield return new WaitForSeconds(2.5f);


				isFirstRoom = false;
			}


			SetState(
				ExpeditionState.RoomActive);


			player.GetComponent<PlayerInput>()
				.SetInputEnabled(true);
			GameRuntimeManager.Instance.Events.Publish(new BehaviorActivationEvent());
		}


		private void OnRoomCompleted(
			RoomCompletedEvent evt)
		{
			if (State != ExpeditionState.RoomActive)
				return;


			SetState(
				ExpeditionState.Reward);


			StartCoroutine(
				SpawnResearchRewards());


			GameRuntimeManager.Instance.Events.Publish(
				new ExpeditionRewardStartedEvent());
		}


		private IEnumerator SpawnResearchRewards()
		{
			foreach (var r in currentSpawnedResearches)
			{
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


			player.GetComponent<PlayerInput>()
				.SetInputEnabled(false);


			for (int i = 0; i < rewardCount; i++)
			{
				ResearchDefinition definition =
					availableResearch[
						Random.Range(
							0,
							availableResearch.Count)];


				Vector3 spawnPos =
					marker.transform.position;


				int safeGuard = 0;


				while (true)
				{
					Vector2 rndOffset =
						Random.onUnitCircle * 1.2f;


					Vector3 offset =
						new Vector3(
							rndOffset.x,
							rndOffset.y,
							0);


					spawnPos =
						marker.transform.position +
						offset;


					if (section.ContainsWorldPosition(spawnPos))
						break;


					safeGuard++;


					if (safeGuard > 100)
						break;
				}


				ResearchPickup pickup =
					Instantiate(
						researchPickupPrefab,
						spawnPos,
						Quaternion.identity);


				pickup.Initialize(
					definition);


				currentSpawnedResearches.Add(
					pickup);


				pickup.transform.localScale =
					Vector3.zero;


				yield return new WaitForSeconds(0.2f);


				iTween.ScaleTo(
					pickup.gameObject,
					iTween.Hash(
						"scale",
						Vector3.one,
						"time",
						0.7f,
						"easeType",
						"easeOutElastic"));


				yield return new WaitForSeconds(0.5f);
			}


			yield return new WaitForSeconds(1.0f);


			MainCameraManager.Instance.SetFollowTarget(
				player.transform);


			player.GetComponent<PlayerInput>()
				.SetInputEnabled(true);
		}


		private void GenerateRewards()
		{
			currentRewards.Clear();

			currentRewards.AddRange(
				GameRuntimeManager.Instance.Rewards
					.GenerateModuleRewards(
						availableRewardModules,
						rewardCount));
		}


		private void SetState(
			ExpeditionState state)
		{
			if (State == state)
				return;


			State = state;


			//Debug.Log($"Expedition state: {State}");
		}
	}
}