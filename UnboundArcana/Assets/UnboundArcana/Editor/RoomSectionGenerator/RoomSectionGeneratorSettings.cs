using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnboundArcana.Core.Rooms.Editor
{
	[CreateAssetMenu(menuName = "Unbound Arcana/Rooms/Section Generator Settings")]
	public class RoomSectionGeneratorSettings : ScriptableObject
	{
		public Vector3 CellSize = new(0.3f, 0.3f, 0.3f);
		public bool RequireSquareCells = true;
		public int SouthWallSortingOrder = 1;
		public TileBase FloorTile;
		public TileBase SouthWallTile;
		public TileBase EastWallTile;
		public TileBase WestWallTile;
		public List<TileBase> NorthWallLayers = new();
		public List<TileBase> NorthWestCornerLayers = new();
		public List<TileBase> NorthEastCornerLayers = new();
		public List<TileBase> InnerNorthWestCornerLayers = new();
		public List<TileBase> InnerNorthEastCornerLayers = new();
		public TileBase SouthWestCornerTile;
		public TileBase SouthEastCornerTile;
		public TileBase InnerSouthWestCornerTile;
		public TileBase InnerSouthEastCornerTile;
		public TileBase SouthWestCapTile;
		public TileBase SouthEastCapTile;
		public ConnectorShapeDefinition ConnectorShape;
		public ConnectorType ConnectorType = ConnectorType.Stone;
		public bool NorthConnector;
		public bool EastConnector;
		public bool SouthConnector;
		public bool WestConnector;
		public List<RoomSectionGeneratorProp> Props = new();
	}

	[System.Serializable]
	public class RoomSectionGeneratorProp
	{
		public GameObject Prefab;
		public Vector2Int Footprint = Vector2Int.one;
		[Range(0f, 1f)] public float Weight = 1f;
		public bool AvoidCenter = true;
	}
}
