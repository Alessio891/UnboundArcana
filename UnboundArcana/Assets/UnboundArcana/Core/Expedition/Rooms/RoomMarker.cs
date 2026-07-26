using UnityEngine;

namespace UnboundArcana.Core.Rooms
{
	public enum RoomMarkerType
	{
		PlayerStart,
		EnemySpawn,
		Portal,
		Event,
		Decoration,
		RewardSpawn
	}
	public class RoomMarker : MonoBehaviour
	{
		[SerializeField]
		private RoomMarkerType type;

		public RoomMarkerType Type => type;
	}
}