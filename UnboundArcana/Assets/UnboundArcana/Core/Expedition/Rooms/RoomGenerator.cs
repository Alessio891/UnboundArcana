using System.Collections.Generic;
using UnityEngine;

namespace UnboundArcana.Core.Rooms
{
	public class RoomGenerator
	{
		private readonly float cellSize;

		public RoomGenerator(float cellSize)
		{
			this.cellSize = cellSize;
		}

		public RoomInstance Generate(
			RoomDefinition definition,
			Transform parent)
		{
			if (definition == null)
			{
				Debug.LogError(
					"Cannot generate room. Definition is null.");

				return null;
			}

			var assembler = new RoomAssembler(
				new List<RoomSection>(
					definition.AvailableSections));

			GeneratedRoomLayout layout =
				assembler.Generate(
					definition.GetSectionCount());

			var roomObject =
				new GameObject(definition.RoomId);

			roomObject.transform.SetParent(parent);

			var roomInstance =
				roomObject.AddComponent<RoomInstance>();

			foreach (var section in layout.Sections)
			{
				RoomSection instance =
					Object.Instantiate(
						section.Template,
						new Vector3(
							section.CellPosition.x * cellSize,
							section.CellPosition.y * cellSize,
							0),
						Quaternion.identity,
						roomObject.transform);

				section.Instance = instance;
			}

			roomInstance.Initialize(definition, layout);
			var debug = roomObject.AddComponent<RoomDebugView>();

			debug.Initialize(roomInstance);
			return roomInstance;
		}
	}
}