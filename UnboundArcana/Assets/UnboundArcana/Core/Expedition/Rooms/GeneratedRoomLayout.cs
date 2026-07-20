using System.Collections.Generic;
using UnityEngine;

namespace UnboundArcana.Core.Rooms
{
	public class GeneratedRoomLayout
	{
		public List<GeneratedSection> Sections { get; } = new();
		public List<RoomConnection> Connections { get; } = new();
	}

	public class GeneratedSection
	{
		public string SectionId;
		public RoomSection Template;

		public Vector2Int CellPosition;

		public List<int> UsedConnectorIndices { get; } = new();
	}

	public class RoomConnection
	{
		public GeneratedSection A;
		public int ConnectorAIndex;

		public GeneratedSection B;
		public int ConnectorBIndex;
	}
}