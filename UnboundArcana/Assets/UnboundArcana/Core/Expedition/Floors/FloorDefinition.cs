using System.Collections.Generic;
using UnityEngine;
using UnboundArcana.Core.Rooms;

namespace UnboundArcana.Core.Expedition
{
	[CreateAssetMenu(
		menuName = "Unbound Arcana/Floors/Floor Definition")]
	public class FloorDefinition : ScriptableObject
	{
		[SerializeField]
		private string floorId;

		[SerializeField]
		private int roomCount = 5;

		[SerializeField]
		private List<RoomDefinition> availableRooms = new();

		[SerializeField]
		private RoomDefinition bossRoom;


		public string FloorId => floorId;

		public int RoomCount =>
			roomCount;

		public IReadOnlyList<RoomDefinition> AvailableRooms =>
			availableRooms;

		public RoomDefinition BossRoom =>
			bossRoom;
	}
}