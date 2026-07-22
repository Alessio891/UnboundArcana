using UnityEngine;
using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Runtime;

namespace UnboundArcana.Core.Rooms
{
	public class TowerSceneController : MonoBehaviour
	{
		[SerializeField]
		private EntityDefinition debugPlayer;

		[SerializeField]
		private Transform playerSpawn;

		[SerializeField]
		private RoomDefinition startingRoom;

		[SerializeField]
		private Transform roomParent;

		private Entity player;

		private void Start()
		{
			EnsureSession();

			SpawnPlayer();

			GenerateRoom();
		}

		private void EnsureSession()
		{
			if (GameSession.Instance.Player != null)
				return;

			GameSession.Instance.CreatePlayer(
				debugPlayer);
		}

		private void SpawnPlayer()
		{
			player =
				GameRuntimeManager.Instance.PlayerSpawner.Spawn(
					GameSession.Instance.Player,
					playerSpawn.position,
					null);
		}

		private void GenerateRoom()
		{
			if (startingRoom == null)
			{
				Debug.LogWarning(
					"No starting room assigned.");

				return;
			}

			RoomInstance room =
				GameRuntimeManager.Instance.Rooms.GenerateRoom(
					startingRoom,
					roomParent);

			if (room != null)
			{
				GameRuntimeManager.Instance.Rooms.StartRoom();
			}
		}
	}
}