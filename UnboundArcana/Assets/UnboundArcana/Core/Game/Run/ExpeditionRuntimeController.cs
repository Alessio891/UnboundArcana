using System.Collections;
using System.Collections.Generic;
using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Entities.Events;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Research;
using UnboundArcana.Core.Rooms;
using UnboundArcana.Core.Runtime;
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


		private readonly ResearchSystem researchSystem = new();


		[SerializeField]
		private List<ResearchDefinition> availableResearch = new();

		[SerializeField]
		private ResearchPickup researchPickupPrefab;

		[SerializeField]
		private int rewardCount = 3;

		private ExpeditionPlayerCoordinator playerCoordinator;
		private ResearchRewardSpawner rewardSpawner;

		public Entity Player => playerCoordinator?.Player;


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
			playerCoordinator = new ExpeditionPlayerCoordinator();
			rewardSpawner = new ResearchRewardSpawner(availableResearch, researchPickupPrefab, rewardCount);
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
			playerCoordinator.SetInputEnabled(false);
			rewardSpawner.Clear();


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


			playerCoordinator.MoveToRoom(nextRoom);


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


			if (!playerCoordinator.Spawn(room))
			{
				SetState(
					ExpeditionState.Failed);

				return;
			}


			playerCoordinator.SetRevealProgress(0f);
			playerCoordinator.SetInputEnabled(false);
		}


		private void EnsureSession()
		{
			if (GameSession.Instance.Player != null)
				return;


			GameSession.Instance.CreatePlayer(
				playerDefinition);
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
				yield return playerCoordinator.Reveal(1.5f);


				GameRuntimeManager.Instance.Events.Publish(
					new ShowDialogueEvent(
						"Am I inside the tower? Nice, let's focus on experimentations then!",
						null));


				yield return new WaitForSeconds(2.5f);


				isFirstRoom = false;
			}


			SetState(
				ExpeditionState.RoomActive);


			playerCoordinator.SetInputEnabled(true);
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
			playerCoordinator.SetInputEnabled(false);
			yield return rewardSpawner.Spawn(currentRoom, Player);

			if (!rewardSpawner.HasSpawnedRewards)
			{
				Debug.LogError("Research rewards could not be spawned. Advancing to the next room to avoid blocking the expedition.");
				yield return AdvanceToNextRoom();
				yield break;
			}

			playerCoordinator.FollowPlayer();
			playerCoordinator.SetInputEnabled(true);
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
