using System.Collections;
using System.Collections.Generic;
using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Entities.Events;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Research;
using UnboundArcana.Core.Rooms;
using UnboundArcana.Core.Runtime;
using UnboundArcana.Core.Stats;
using UnboundArcana.Player;
using UnityEngine;

namespace UnboundArcana.Core.Expedition
{
	public class BehaviorActivationEvent
	{
		public BehaviorActivationEvent() { }
	}

	public class ExpeditionRuntimeController : MonoBehaviour
	{
		private const int EnemyKillKnowledge = 50;

		[SerializeField]
		private EntityDefinition playerDefinition;

		[SerializeField]
		private FloorDefinition startingFloor;

		[SerializeField]
		private Transform roomParent;


		[SerializeField]
		private List<ResearchDefinition> availableResearch = new();

		[SerializeField]
		private ResearchPickup researchPickupPrefab;

		[SerializeField]
		private int rewardCount = 3;

		private ExpeditionPlayerCoordinator playerCoordinator;
		private ResearchRewardSpawner rewardSpawner;
		private readonly object reactiveWardSpeedSource = new();
		private Coroutine reactiveWardRoutine;
		private bool reactiveWardTriggered;
		private LaboratoryMajorRewardPresenter laboratoryPresenter;
		private RoomInstance laboratorySessionRoom;
		private bool laboratoryCompletionRequested;

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
			laboratoryPresenter = GetComponent<LaboratoryMajorRewardPresenter>();
			if (laboratoryPresenter == null) { laboratoryPresenter = gameObject.AddComponent<LaboratoryMajorRewardPresenter>(); }
		}


		private void OnEnable()
		{
			if (laboratoryPresenter != null) { laboratoryPresenter.SelectionSucceeded += OnLaboratorySelectionSucceeded; }
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

			GameRuntimeManager.Instance.Events.Subscribe<EntityDamagedEvent>(
				OnEntityDamaged);
		}


		private void OnDisable()
		{
			if (laboratoryPresenter != null) { laboratoryPresenter.SelectionSucceeded -= OnLaboratorySelectionSucceeded; }
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

			GameRuntimeManager.Instance.Events.Unsubscribe<EntityDamagedEvent>(
				OnEntityDamaged);
		}

		private void OnEntityDamaged(EntityDamagedEvent evt)
		{
			if (Result != null || State != ExpeditionState.RoomActive || evt.Entity != Player || evt.Damage.Amount <= 0f || currentRoom?.Definition?.Type != RoomType.Combat || reactiveWardTriggered)
				return;

			RunModifier modifier = GameSession.Instance.Player.Modifiers.Find(x => x.Stat == RunModifierStat.ReactiveWard);

			if (modifier == null)
				return;

			reactiveWardTriggered = true;
			reactiveWardRoutine = StartCoroutine(ApplyReactiveWard(modifier.Value));
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

			PlayerState player = GameSession.Instance.Player;
			float multiplier = 1f + CountModifiers(RunModifierStat.DangerousStudy);
			EntityHealth health = Player?.GetComponent<EntityHealth>();

			if (health != null && health.currentHealth < Player.Stats.Get(StatKeys.Entity.MaxHealth) * 0.4f)
				multiplier += CountModifiers(RunModifierStat.BloodResearch);

			player.AddKnowledge(Mathf.RoundToInt(EnemyKillKnowledge * multiplier));
		}


		private void OnResearchCollected(
			ResearchCollectedEvent evt)
		{
			if (Result != null || State != ExpeditionState.Reward)
				return;

			playerCoordinator.SetInputEnabled(false);
			rewardSpawner.Clear();


			RunModifier modifier = GameSession.Instance.Player.AddMinorReward(evt.Research);
			ApplySelectedMinorReward(modifier);


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

			ReleaseCurrentRoom();


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
			laboratorySessionRoom = null;
			laboratoryCompletionRequested = false;


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

			if (currentRoom.Definition.Type == RoomType.Combat)
			{
				reactiveWardTriggered = false;
				ClearReactiveWardSpeedBonus();

				SpellCaster caster = Player?.GetComponent<SpellCaster>();

				if (CountModifiers(RunModifierStat.ArcaneReserves) > 0)
					caster?.ArmNextCooldownBypass();
				else
					caster?.ClearCooldownBypass();
			}

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

			if (currentRoom.Definition.Type == RoomType.Laboratory) { StartLaboratoryReward(); }
		}


		private void OnRoomCompleted(
			RoomCompletedEvent evt)
		{
			if (Result != null || State != ExpeditionState.RoomActive || evt.Room != currentRoom)
				return;

			Player?.GetComponent<SpellCaster>()?.ClearCooldownBypass();

			if (evt.Room.Definition.Type == RoomType.Combat)
				ApplyCombatRoomCompletedRewards();


			SetState(
				ExpeditionState.Reward);

			if (currentFloor.CurrentRoomIndex >= currentFloor.Rooms.Count - 1)
			{
				StartCoroutine(
					AdvanceToNextRoom());
				return;
			}

			if (evt.Room.Definition.Type == RoomType.Laboratory || currentFloor.GetNextRoom()?.Type == RoomType.Laboratory)
			{
				playerCoordinator.SetInputEnabled(false);
				StartCoroutine(AdvanceToNextRoom());
				return;
			}


			StartCoroutine(
				SpawnResearchRewards());
		}

		private void StartLaboratoryReward()
		{
			if (laboratorySessionRoom == currentRoom)
			{
				Debug.LogError("The Laboratory Major Reward session was already opened for this expedition.");
				return;
			}

			laboratorySessionRoom = currentRoom;
			laboratoryCompletionRequested = false;
			SpellCaster spellCaster = Player?.GetComponent<SpellCaster>();
			PlayerInput input = Player?.GetComponent<PlayerInput>();
			LaboratoryMajorRewardSession session = LaboratoryMajorRewardSession.CreateForPlayer(spellCaster);
			LaboratoryOfferStatus status = laboratoryPresenter.Open(session, input);

			if (status == LaboratoryOfferStatus.Success) { return; }

			Debug.LogError($"Laboratory Major Reward could not open ({status}): {laboratoryPresenter.FailureMessage} Continuing without a Major Reward to avoid blocking the expedition.");
			laboratoryCompletionRequested = true;
			playerCoordinator.SetInputEnabled(false);
			GameRuntimeManager.Instance.Rooms.CompleteRoom();
		}

		private void OnLaboratorySelectionSucceeded(LaboratorySelectionResult result)
		{
			if (!result.Success || laboratoryCompletionRequested || Result != null || State != ExpeditionState.RoomActive || currentRoom?.Definition?.Type != RoomType.Laboratory) { return; }
			laboratoryCompletionRequested = true;
			StartCoroutine(CompleteLaboratoryAfterConfirmation());
		}

		private IEnumerator CompleteLaboratoryAfterConfirmation()
		{
			while (laboratoryPresenter != null && laboratoryPresenter.IsOpen) { yield return null; }
			if (Result != null || State != ExpeditionState.RoomActive || currentRoom?.Definition?.Type != RoomType.Laboratory) { yield break; }
			playerCoordinator.SetInputEnabled(false);
			GameRuntimeManager.Instance.Rooms.CompleteRoom();
		}

		private void ApplySelectedMinorReward(RunModifier modifier)
		{
			if (modifier == null || Player == null)
				return;

			if (modifier.Stat == RunModifierStat.MaxHealth)
			{
				float previousMaxHealth = Player.Stats.Get(StatKeys.Entity.MaxHealth);
				Player.Stats.AddModifier(new StatModifier(StatKeys.Entity.MaxHealth, modifier.Value, ConvertOperation(modifier.Operation), modifier));
				float additionalHealth = Player.Stats.Get(StatKeys.Entity.MaxHealth) - previousMaxHealth;
				Player.GetComponent<EntityHealth>()?.RestoreHealth(additionalHealth);
			}
			else if (modifier.Stat == RunModifierStat.DangerousStudy)
			{
				Player.Stats.AddModifier(new StatModifier(StatKeys.Entity.DamageTakenFromEnemies, modifier.Value, ModifierOperation.Percent, modifier));
			}
		}

		private void ApplyCombatRoomCompletedRewards()
		{
			PlayerState player = GameSession.Instance.Player;
			EntityHealth health = Player?.GetComponent<EntityHealth>();
			int dangerousStudyCount = CountModifiers(RunModifierStat.DangerousStudy);

			if (dangerousStudyCount > 0)
				player.AddKnowledge(Mathf.RoundToInt(EnemyKillKnowledge * 0.5f * dangerousStudyCount));

			foreach (RunModifier modifier in player.Modifiers)
			{
				if (modifier.Stat == RunModifierStat.HealthRestoreOnCombatRoomCompleted && health != null)
				{
					float amount = modifier.Operation == RunModifierOperation.Percent ? Player.Stats.Get(StatKeys.Entity.MaxHealth) * modifier.Value : modifier.Value;
					health.RestoreHealth(amount);
				}
				else if (modifier.Stat == RunModifierStat.KnowledgeOnCombatRoomCompleted)
				{
					int amount = Mathf.RoundToInt(modifier.Value);
					player.AddKnowledge(amount);
				}
			}
		}

		private ModifierOperation ConvertOperation(RunModifierOperation operation)
		{
			return operation == RunModifierOperation.Percent ? ModifierOperation.Percent : ModifierOperation.Flat;
		}

		private int CountModifiers(RunModifierStat stat)
		{
			return GameSession.Instance.Player?.Modifiers.FindAll(x => x.Stat == stat).Count ?? 0;
		}

		private IEnumerator ApplyReactiveWard(float speedBonus)
		{
			ClearReactiveWardSpeedBonus();
			Player.Stats.AddModifier(new StatModifier(StatKeys.Entity.MoveSpeed, speedBonus, ModifierOperation.Percent, reactiveWardSpeedSource));
			yield return new WaitForSeconds(3f);
			Player?.Stats.RemoveModifiersFromSource(reactiveWardSpeedSource);
			reactiveWardRoutine = null;
		}

		private void ClearReactiveWardSpeedBonus()
		{
			if (reactiveWardRoutine != null)
			{
				StopCoroutine(reactiveWardRoutine);
				reactiveWardRoutine = null;
			}

			Player?.Stats.RemoveModifiersFromSource(reactiveWardSpeedSource);
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

			GameRuntimeManager.Instance.Events.Publish(new ExpeditionRewardStartedEvent());

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
			ClearExpeditionProgress();
			ReleaseCurrentRoom();

			if (outcome == ExpeditionOutcome.Failed)
				Debug.LogError($"Expedition failed: {reason}");
			else
				Debug.Log($"Expedition completed: {reason}");

			GameRuntimeManager.Instance.Events.Publish(new ExpeditionEndedEvent(Result));
			return true;
		}

		private void ClearExpeditionProgress()
		{
			PlayerState player = GameSession.Instance.Player;
			ClearReactiveWardSpeedBonus();
			reactiveWardTriggered = false;
			Player?.GetComponent<SpellCaster>()?.ClearCooldownBypass();

			if (player == null)
				return;

			if (Player != null)
			{
				foreach (RunModifier modifier in player.Modifiers)
					Player.Stats.RemoveModifiersFromSource(modifier);
			}

			player.ClearExpeditionProgress();
		}


		private void ReleaseCurrentRoom()
		{
			RoomInstance room = currentRoom;
			currentRoom = null;

			if (room == null)
				return;

			RoomService roomService = GameRuntimeManager.Instance.Rooms;

			if (roomService.CurrentRoom == room)
			{
				roomService.ClearCurrentRoom();
				return;
			}

			room.PrepareForDestruction();
			Destroy(room.gameObject);
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
