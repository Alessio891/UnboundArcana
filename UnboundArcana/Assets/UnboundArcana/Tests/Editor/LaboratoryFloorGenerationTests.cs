using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnboundArcana.Core.Expedition;
using UnboundArcana.Core.Rooms;
using UnityEngine;

namespace UnboundArcana.Tests
{
	public class LaboratoryFloorGenerationTests
	{
		private readonly List<Object> createdObjects = new();

		[TearDown]
		public void TearDown()
		{
			foreach (Object createdObject in createdObjects) { Object.DestroyImmediate(createdObject); }
		}

		[Test]
		public void LaboratoryIsInsertedAfterSecondCombatWithTwoStandardCombatsRemaining()
		{
			RoomDefinition combat = CreateRoom(RoomType.Combat);
			RoomDefinition laboratory = CreateRoom(RoomType.Laboratory);
			RoomDefinition finalEncounter = CreateRoom(RoomType.Combat);
			FloorDefinition definition = CreateFloor(4, combat, laboratory, finalEncounter);

			FloorInstance floor = new FloorGenerator().Generate(definition);

			Assert.That(floor.Rooms, Has.Count.EqualTo(7));
			Assert.That(floor.Rooms[0], Is.SameAs(combat));
			Assert.That(floor.Rooms[1], Is.SameAs(combat));
			Assert.That(floor.Rooms[2], Is.SameAs(laboratory));
			Assert.That(floor.Rooms[3], Is.SameAs(combat));
			Assert.That(floor.Rooms[4], Is.SameAs(combat));
			Assert.That(floor.Rooms[5], Is.SameAs(laboratory));
			Assert.That(floor.Rooms[6], Is.SameAs(finalEncounter));
		}

		[Test]
		public void FloorWithoutLaboratoryPreservesExistingSequenceLength()
		{
			RoomDefinition combat = CreateRoom(RoomType.Combat);
			RoomDefinition finalEncounter = CreateRoom(RoomType.Combat);
			FloorDefinition definition = CreateFloor(4, combat, null, finalEncounter);

			FloorInstance floor = new FloorGenerator().Generate(definition);

			Assert.That(floor.Rooms, Has.Count.EqualTo(5));
			Assert.That(floor.Rooms, Does.Not.Contain(null));
		}

		private RoomDefinition CreateRoom(RoomType type)
		{
			RoomDefinition room = ScriptableObject.CreateInstance<RoomDefinition>();
			createdObjects.Add(room);
			SetField(room, "type", type);
			return room;
		}

		private FloorDefinition CreateFloor(int roomCount, RoomDefinition availableRoom, RoomDefinition laboratory, RoomDefinition finalEncounter)
		{
			FloorDefinition floor = ScriptableObject.CreateInstance<FloorDefinition>();
			createdObjects.Add(floor);
			SetField(floor, "roomCount", roomCount);
			SetField(floor, "availableRooms", new List<RoomDefinition> { availableRoom });
			SetField(floor, "laboratoryRoom", laboratory);
			SetField(floor, "bossRoom", finalEncounter);
			return floor;
		}

		private void SetField<T>(Object target, string fieldName, T value)
		{
			target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(target, value);
		}
	}
}
