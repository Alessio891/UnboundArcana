using UnityEngine;

namespace UnboundArcana.Core.Rooms
{
	public static class ConnectorDirectionExtensions
	{
		public static ConnectorDirection Opposite(this ConnectorDirection direction)
		{
			return direction switch
			{
				ConnectorDirection.North => ConnectorDirection.South,
				ConnectorDirection.East => ConnectorDirection.West,
				ConnectorDirection.South => ConnectorDirection.North,
				ConnectorDirection.West => ConnectorDirection.East,
				_ => ConnectorDirection.North
			};
		}

		public static Quaternion ToRotation(this ConnectorDirection direction)
		{
			return direction switch
			{
				ConnectorDirection.North => Quaternion.identity,
				ConnectorDirection.East => Quaternion.Euler(0, 0, -90),
				ConnectorDirection.South => Quaternion.Euler(0, 0, 180),
				ConnectorDirection.West => Quaternion.Euler(0, 0, 90),
				_ => Quaternion.identity
			};
		}
	}
}