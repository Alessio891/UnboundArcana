using UnityEngine;

namespace UnboundArcana.Core.Rooms
{
	public class RoomGenerationTester : MonoBehaviour
	{
		[SerializeField]
		private RoomDefinition roomDefinition;

		[SerializeField]
		private Transform roomParent;

		[SerializeField]
		private bool startRoomAfterGeneration = true;
		private void Start()
		{
		//	GenerateRoom();
		}
		[ContextMenu("Generate Room")]
		public void GenerateRoom()
		{
			if (roomDefinition == null)
			{
				Debug.LogWarning(
					"No RoomDefinition assigned.",
					this);

				return;
			}

			if (roomParent == null)
				roomParent = transform;
			if (GameRuntimeManager.Instance.Rooms == null) {
				Debug.LogError("Runtime null?");
				return;
			}
			RoomInstance room =	GameRuntimeManager.Instance.Rooms.GenerateRoom(roomDefinition, roomParent);

			if (room != null && startRoomAfterGeneration)
			{
				GameRuntimeManager.Instance.Rooms.StartRoom();
			}
		}

		[ContextMenu("Complete Room")]
		public void CompleteRoom()
		{
			GameRuntimeManager.Instance.Rooms.CompleteRoom();
		}

		[ContextMenu("Clear Room")]
		public void ClearRoom()
		{
			GameRuntimeManager.Instance.Rooms.ClearCurrentRoom();
		}
	}
}