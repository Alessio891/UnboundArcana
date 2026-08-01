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
		public ExpeditionResult Result { get; private set; }


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
			if (Result != null)
				return;

			if (@event.Entity == Player)
			{
				TryEndExpedition(ExpeditionOutcome.Failed, "Expedition player died.");
				return;
			}

			GameSession.Instance.Player.AddKnowledge(50);
		}


		private void OnResearchCollected(
			ResearchCollectedEvent evt)
		{
			if (Result != null || State != ExpeditionState.Reward)
				return;

			playerCoordinator.SetInputEnabled(false);
			rewardSpawner.Clear();


			GameSession.Instance.Player.AddResearch(
				evt.Research);


			StartCoroutine(
				AdvanceToNextRoom());
		}


		public IEnumerator AdvanceToNextRoom()
		{
			if (Result != null || State != ExpeditionState.Reward)
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

			if (Result != null || State != ExpeditionState.Reward)
				yield break;


			bool hasNextRoom =
				currentFloor.Advance();


			if (!hasNextRoom)
			{
				Debug.Log(
					"Floor completed.");

				TryEndExpedition(ExpeditionOutcome.Completed, "Final floor completed.");

				yield break;
			}


			RoomDefinition nextRoomDefinition =
				currentFloor.GetCurrentRoom();


			RoomInstance nextRoom =
				GenerateRoom(
					nextRoomDefinition);


			if (nextRoom == null)
			{
				TryEndExpedition(ExpeditionOutcome.Failed, $"Next room generation failed at room index {currentFloor.CurrentRoomIndex}.");

				yield break;
			}

			if (Result != null)
				yield break;


			researchSystem.ActivateCompletedResearches(
				GameSession.Instance.Player);


			currentRoom = nextRoom;


			if (!playerCoordinator.MoveToRoom(nextRoom))
			{
				TryEndExpedition(ExpeditionOutcome.Failed, $"Player placement failed for room index {currentFloor.CurrentRoomIndex}.");
				yield break;
			}


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
				TryEndExpedition(ExpeditionOutcome.Failed, "Starting floor generation failed.");

				return;
			}


			RoomDefinition firstRoom =
				currentFloor.GetCurrentRoom();


			RoomInstance room =
				GenerateRoom(
					firstRoom);


			if (room == null)
			{
				TryEndExpedition(ExpeditionOutcome.Failed, "Initial room generation failed.");

				return;
			}


			SetState(
				ExpeditionState.EnteringRoom);


			currentRoom = room;


			GameRuntimeManager.Instance.Rooms.StartRoom();


			if (!playerCoordinator.Spawn(room))
			{
				TryEndExpedition(ExpeditionOutcome.Failed, "Player spawn failed in the initial room.");

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
			if (Result != null || State != ExpeditionState.EnteringRoom || evt.Room != currentRoom)
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

			if (Result != null || State != ExpeditionState.EnteringRoom)
				yield break;


			SetState(
				ExpeditionState.RoomActive);


			playerCoordinator.SetInputEnabled(true);
			GameRuntimeManager.Instance.Events.Publish(new BehaviorActivationEvent());
		}


		private void OnRoomCompleted(
			RoomCompletedEvent evt)
		{
			if (Result != null || State != ExpeditionState.RoomActive || evt.Room != currentRoom)
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

			if (Result != null || State != ExpeditionState.Reward)
			{
				rewardSpawner.Clear();
				yield break;
			}

			if (!rewardSpawner.HasSpawnedRewards)
			{
				Debug.LogError("Research rewards could not be spawned. Advancing to the next room to avoid blocking the expedition.");
				yield return AdvanceToNextRoom();
				yield break;
			}

			playerCoordinator.FollowPlayer();
			playerCoordinator.SetInputEnabled(true);
		}


		private bool TryEndExpedition(ExpeditionOutcome outcome, string reason)
		{
			if (Result != null)
				return false;

			Result = new ExpeditionResult(outcome, reason);
			SetState(outcome == ExpeditionOutcome.Completed ? ExpeditionState.Completed : ExpeditionState.Failed);
			StopAllCoroutines();
			playerCoordinator.SetInputEnabled(false);
			rewardSpawner.Clear();
			currentRoom?.StopRoom();

			if (outcome == ExpeditionOutcome.Failed)
				Debug.LogError($"Expedition failed: {reason}");
			else
				Debug.Log($"Expedition completed: {reason}");

			GameRuntimeManager.Instance.Events.Publish(new ExpeditionEndedEvent(Result));
			return true;
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
