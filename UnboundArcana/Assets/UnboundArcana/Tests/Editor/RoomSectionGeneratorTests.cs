using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnboundArcana.Core.Rooms.Editor.Tests
{
	public class RoomSectionGeneratorTests
	{
		[Test]
		public void ShapeRequestsAreDeterministic()
		{
			RoomSectionGenerationRequest request = new() { Seed = 91, Preset = RoomSectionShapePreset.Random, Size = new Vector2Int(10, 8), Extensions = 3 };
			RoomSectionShape first = RoomSectionShapeGenerationLogic.Generate(request);
			RoomSectionShape second = RoomSectionShapeGenerationLogic.Generate(new RoomSectionGenerationRequest { Seed = 91, Preset = RoomSectionShapePreset.Random, Size = new Vector2Int(10, 8), Extensions = 3 });
			Assert.That(first.Cells, Is.EquivalentTo(second.Cells));
			Assert.That(first.Boundaries.Count, Is.EqualTo(second.Boundaries.Count));
			Assert.That(first.BoundaryVertices.Count, Is.EqualTo(second.BoundaryVertices.Count));
			Assert.That(first.BoundaryLoops.Count, Is.EqualTo(second.BoundaryLoops.Count));
			AssertLoopTopology(first.BoundaryLoops[0], second.BoundaryLoops[0]);
		}

		[Test]
		public void ShapePresetsAreConnectedHoleFreeAndValid()
		{
			foreach (RoomSectionShapePreset preset in System.Enum.GetValues(typeof(RoomSectionShapePreset)))
			{
				RoomSectionShape shape = RoomSectionShapeGenerationLogic.Generate(new RoomSectionGenerationRequest { Seed = 17, Preset = preset, Size = new Vector2Int(10, 8), Extensions = 3 });
				Assert.That(shape.IsValid, Is.True, $"{preset}: {string.Join("; ", shape.Errors.Select(diagnostic => diagnostic.Message))}");
				Assert.That(shape.BoundaryVertices, Is.Not.Empty, $"{preset}: boundary vertices are missing");
				Assert.That(shape.BoundaryLoops, Has.Count.EqualTo(1), $"{preset}: boundary loop count");
			}
		}

		[Test]
		public void RectangleProducesClockwiseClosedRunsAndOuterCorners()
		{
			RoomSectionShape shape = GenerateRectangle(4, 3);
			GeneratedBoundaryLoop loop = shape.BoundaryLoops.Single();
			Assert.That(shape.IsValid, Is.True, string.Join("; ", shape.Errors.Select(diagnostic => diagnostic.Message)));
			Assert.That(loop.IsClosed, Is.True);
			Assert.That(loop.IsClockwise, Is.True);
			Assert.That(loop.Vertices, Is.EqualTo(new[] { new Vector2Int(0, 0), new Vector2Int(0, 3), new Vector2Int(4, 3), new Vector2Int(4, 0), new Vector2Int(0, 0) }));
			Assert.That(loop.WallRuns.Select(run => run.Direction), Is.EqualTo(new[] { GeneratedWallDirection.West, GeneratedWallDirection.North, GeneratedWallDirection.East, GeneratedWallDirection.South }));
			Assert.That(loop.WallRuns.Select(run => run.Length), Is.EqualTo(new[] { 3, 4, 3, 4 }));
			Assert.That(loop.WallRuns.Select(run => run.StartVertex), Is.EqualTo(new[] { new Vector2Int(0, 0), new Vector2Int(0, 3), new Vector2Int(4, 3), new Vector2Int(4, 0) }));
			Assert.That(loop.WallRuns.Select(run => run.EndVertex), Is.EqualTo(new[] { new Vector2Int(0, 3), new Vector2Int(4, 3), new Vector2Int(4, 0), new Vector2Int(0, 0) }));
			Assert.That(loop.Corners.Select(corner => corner.Kind), Is.All.EqualTo(GeneratedBoundaryCornerKind.Outer));
			Assert.That(loop.Corners.Select(corner => corner.Orientation), Is.EqualTo(new[] { GeneratedBoundaryCornerOrientation.NorthWest, GeneratedBoundaryCornerOrientation.NorthEast, GeneratedBoundaryCornerOrientation.SouthEast, GeneratedBoundaryCornerOrientation.SouthWest }));
			Assert.That(loop.Corners.Select(corner => corner.IncomingWallSide), Is.EqualTo(loop.WallRuns.Select(run => run.Direction)));
			Assert.That(loop.Corners.Select(corner => corner.OutgoingWallSide), Is.EqualTo(loop.WallRuns.Skip(1).Select(run => run.Direction).Append(loop.WallRuns[0].Direction)));
		}

		[Test]
		public void LShapeProducesOneInnerCornerAndFiveOuterCorners()
		{
			RoomSectionShape shape = GenerateFixture(new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) });
			GeneratedBoundaryLoop loop = shape.BoundaryLoops.Single();
			Assert.That(shape.IsValid, Is.True, string.Join("; ", shape.Errors.Select(diagnostic => diagnostic.Message)));
			Assert.That(loop.WallRuns, Has.Count.EqualTo(6));
			Assert.That(loop.Corners, Has.Count.EqualTo(6));
			Assert.That(loop.Corners.Count(corner => corner.Kind == GeneratedBoundaryCornerKind.Inner), Is.EqualTo(1));
			Assert.That(loop.Corners.Count(corner => corner.Kind == GeneratedBoundaryCornerKind.Outer), Is.EqualTo(5));
			GeneratedBoundaryCorner inner = loop.Corners.Single(corner => corner.Kind == GeneratedBoundaryCornerKind.Inner);
			Assert.That(inner.Orientation, Is.EqualTo(GeneratedBoundaryCornerOrientation.NorthEast));
			Assert.That(inner.IncomingWallSide, Is.EqualTo(GeneratedWallDirection.East));
			Assert.That(inner.OutgoingWallSide, Is.EqualTo(GeneratedWallDirection.North));
		}

		[Test]
		public void BaysOnAllFourSidesProduceExpectedInnerAndOuterOrientations()
		{
			var expectedInner = new Dictionary<int, GeneratedBoundaryCornerOrientation[]>
			{
				{ 0, new[] { GeneratedBoundaryCornerOrientation.NorthWest, GeneratedBoundaryCornerOrientation.NorthEast } },
				{ 1, new[] { GeneratedBoundaryCornerOrientation.NorthEast, GeneratedBoundaryCornerOrientation.SouthEast } },
				{ 2, new[] { GeneratedBoundaryCornerOrientation.SouthEast, GeneratedBoundaryCornerOrientation.SouthWest } },
				{ 3, new[] { GeneratedBoundaryCornerOrientation.NorthWest, GeneratedBoundaryCornerOrientation.SouthWest } }
			};
			for (int side = 0; side < 4; side++)
			{
				RoomSectionShape shape = GenerateBay(side);
				GeneratedBoundaryLoop loop = shape.BoundaryLoops.Single();
				Assert.That(shape.IsValid, Is.True, $"side {side}: {string.Join("; ", shape.Errors.Select(diagnostic => diagnostic.Message))}");
				Assert.That(loop.WallRuns, Has.Count.EqualTo(8), $"side {side}: runs");
				Assert.That(loop.Corners, Has.Count.EqualTo(8), $"side {side}: corners");
				Assert.That(loop.Corners.Count(corner => corner.Kind == GeneratedBoundaryCornerKind.Inner), Is.EqualTo(2), $"side {side}: inner corners");
				Assert.That(loop.Corners.Count(corner => corner.Kind == GeneratedBoundaryCornerKind.Outer), Is.EqualTo(6), $"side {side}: outer corners");
				Assert.That(loop.Corners.Where(corner => corner.Kind == GeneratedBoundaryCornerKind.Inner).Select(corner => corner.Orientation).OrderBy(value => value), Is.EqualTo(expectedInner[side].OrderBy(value => value)), $"side {side}: inner orientations");
			}
		}

		[Test]
		public void TranslatedNegativeCoordinatesPreserveTopology()
		{
			Vector2Int[] cells = { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) };
			Vector2Int translation = new(-10, -20);
			RoomSectionShape positive = GenerateFixture(cells);
			RoomSectionShape negative = GenerateFixture(cells.Select(cell => cell + translation));
			AssertLoopTopology(positive.BoundaryLoops.Single(), negative.BoundaryLoops.Single(), translation);
		}

		[Test]
		public void BoundaryLoopsAreClosedAndCoverEveryGeneratedBoundaryEdge()
		{
			foreach (RoomSectionShapePreset preset in System.Enum.GetValues(typeof(RoomSectionShapePreset)))
			{
				RoomSectionShape shape = RoomSectionShapeGenerationLogic.Generate(new RoomSectionGenerationRequest { Seed = 17, Preset = preset, Size = new Vector2Int(10, 8), Extensions = 3 });
				Assert.That(shape.IsValid, Is.True, $"{preset}: {string.Join("; ", shape.Errors.Select(diagnostic => diagnostic.Message))}");
				Assert.That(shape.Boundaries, Is.Not.Empty, $"{preset}: generated boundary compatibility data");
				Assert.That(shape.BoundaryLoops.Sum(loop => loop.WallRuns.Sum(run => run.Length)), Is.EqualTo(shape.Boundaries.Count), $"{preset}: boundary coverage");
				HashSet<string> expectedSegments = shape.Boundaries.Select(edge => BoundarySegmentKey(edge.Direction, edge.StartVertex, edge.EndVertex)).ToHashSet();
				HashSet<string> actualSegments = new();
				foreach (GeneratedBoundaryLoop loop in shape.BoundaryLoops)
				{
					Assert.That(loop.IsClosed, Is.True, $"{preset}: loop closure");
					Assert.That(loop.Vertices.Count, Is.EqualTo(loop.WallRuns.Count + 1), $"{preset}: compressed loop vertices");
					Assert.That(loop.Vertices[0], Is.EqualTo(loop.Vertices[loop.Vertices.Count - 1]), $"{preset}: repeated closing vertex");
					Assert.That(loop.WallRuns.All(run => run.Length > 0), Is.True, $"{preset}: positive run lengths");
					for (int i = 0; i < loop.WallRuns.Count; i++)
					{
						GeneratedWallRun current = loop.WallRuns[i];
						GeneratedWallRun next = loop.WallRuns[(i + 1) % loop.WallRuns.Count];
						Assert.That(loop.Vertices[i], Is.EqualTo(current.StartVertex), $"{preset}: run start vertex {i}");
						Assert.That(loop.Vertices[i + 1], Is.EqualTo(current.EndVertex), $"{preset}: run end vertex {i}");
						Assert.That(current.EndVertex, Is.EqualTo(next.StartVertex), $"{preset}: run connection {i}");
						Vector2Int start = current.StartVertex;
						Vector2Int step = current.Direction switch
						{
							GeneratedWallDirection.North => Vector2Int.right,
							GeneratedWallDirection.East => Vector2Int.down,
							GeneratedWallDirection.South => Vector2Int.left,
							_ => Vector2Int.up
						};
						for (int segment = 0; segment < current.Length; segment++)
						{
							Vector2Int end = start + step;
							actualSegments.Add(BoundarySegmentKey(current.Direction, start, end));
							start = end;
						}
					}
				}
				Assert.That(actualSegments.SetEquals(expectedSegments), Is.True, $"{preset}: boundary segment identity");
			}
		}

		[Test]
		public void StampPreviewBuildsWithoutConnectors()
		{
			Tile tile = ScriptableObject.CreateInstance<Tile>();
			RoomSectionStyleProfile profile = CreateValidStampProfile(tile);
			RoomSectionShape shape = RoomSectionShapeGenerationLogic.Generate(new RoomSectionGenerationRequest { Seed = 3, Preset = RoomSectionShapePreset.Rectangle, Size = new Vector2Int(6, 5) });
			RoomSectionPreviewResult result = RoomSectionPreviewBuilder.Build(shape, profile, "StampPreview");
			Assert.That(result.Root, Is.Not.Null);
			Assert.That(result.Errors, Is.Empty);
			RoomSection section = result.Root.GetComponent<RoomSection>();
			Assert.That(section, Is.Not.Null);
			Assert.That(section.Connectors, Is.Empty);
			Assert.That(section.GetFootprintCells(), Is.EquivalentTo(shape.Cells));
			Object.DestroyImmediate(result.Root);
			Object.DestroyImmediate(profile);
			Object.DestroyImmediate(tile);
		}

		[Test]
		public void StampPreviewDoesNotRequireLegacyMappings()
		{
			Tile tile = ScriptableObject.CreateInstance<Tile>();
			RoomSectionStyleProfile profile = CreateValidStampProfile(tile);
			RoomSectionShape shape = RoomSectionShapeGenerationLogic.Generate(new RoomSectionGenerationRequest { Seed = 4, Preset = RoomSectionShapePreset.Rectangle, Size = new Vector2Int(5, 5) });
			RoomSectionPreviewResult result = RoomSectionPreviewBuilder.Build(shape, profile, "NoLegacyMappings");
			Assert.That(result.Root, Is.Not.Null);
			Assert.That(result.Errors, Is.Empty);
			Assert.That(result.Warnings, Is.Empty);
			Object.DestroyImmediate(result.Root);
			Object.DestroyImmediate(profile);
			Object.DestroyImmediate(tile);
		}

		[Test]
		public void StampPreviewUsesStraightDirection()
		{
			Tile northTile = ScriptableObject.CreateInstance<Tile>();
			RoomSectionStyleProfile profile = CreateValidStampProfile(northTile);
			RoomSectionShape shape = RoomSectionShapeGenerationLogic.Generate(new RoomSectionGenerationRequest { Seed = 5, Preset = RoomSectionShapePreset.Rectangle, Size = new Vector2Int(4, 4) });
			RoomSectionPreviewResult result = RoomSectionPreviewBuilder.Build(shape, profile, "MappedProfile");
			Transform wallsTransform = result.Root.transform.Find("Walls_Sides");
			Assert.That(wallsTransform, Is.Not.Null);
			Tilemap walls = wallsTransform.GetComponent<Tilemap>();
			Assert.That(walls, Is.Not.Null);
			Assert.That(walls.GetTile(new Vector3Int(1, 2, 0)), Is.EqualTo(northTile));
			Object.DestroyImmediate(result.Root);
			Object.DestroyImmediate(northTile);
			Object.DestroyImmediate(profile);
		}

		[Test]
		public void StampPreviewUsesCornerOrientation()
		{
			Tile turnTile = ScriptableObject.CreateInstance<Tile>();
			Tile baseTile = ScriptableObject.CreateInstance<Tile>();
			RoomSectionStyleProfile profile = CreateValidStampProfile(baseTile);
			RoomSectionShape shape = RoomSectionShapeGenerationLogic.Generate(new RoomSectionGenerationRequest { Seed = 8, Preset = RoomSectionShapePreset.LShape, Size = new Vector2Int(6, 6) });
			GeneratedBoundaryCorner turn = shape.BoundaryLoops.Single().Corners.First(corner => corner.Kind == GeneratedBoundaryCornerKind.Outer);
			RoomSectionCornerWallStamp turnStamp = profile.CornerWallStamps.Single(stamp => stamp.Kind == turn.Kind && stamp.Orientation == turn.Orientation);
			turnStamp.Placements[0].Tile = turnTile;
			RoomSectionPreviewResult result = RoomSectionPreviewBuilder.Build(shape, profile, "CornerProfile");
			Transform wallsTransform = result.Root.transform.Find("Walls_Sides");
			Assert.That(wallsTransform, Is.Not.Null);
			Tilemap walls = wallsTransform.GetComponent<Tilemap>();
			Assert.That(walls, Is.Not.Null);
			Assert.That(walls.GetTile(new Vector3Int(turn.Position.x, turn.Position.y, 0)), Is.EqualTo(turnTile));
			Object.DestroyImmediate(result.Root);
			Object.DestroyImmediate(turnTile);
			Object.DestroyImmediate(baseTile);
			Object.DestroyImmediate(profile);
		}

		[Test]
		public void StampPreviewUsesTargetRenderPass()
		{
			Tile southTile = ScriptableObject.CreateInstance<Tile>();
			RoomSectionStyleProfile profile = CreateValidStampProfile(southTile);
			RoomSectionStraightWallStamp southStamp = profile.StraightWallStamps.Single(stamp => stamp.Direction == GeneratedWallDirection.South);
			southStamp.Placements[0].RenderPass = RoomSectionStampRenderPass.Front;
			RoomSectionShape shape = RoomSectionShapeGenerationLogic.Generate(new RoomSectionGenerationRequest { Seed = 6, Preset = RoomSectionShapePreset.Rectangle, Size = new Vector2Int(4, 4) });
			RoomSectionPreviewResult result = RoomSectionPreviewBuilder.Build(shape, profile, "FrontProfile");
			Transform wallsTransform = result.Root.transform.Find("Walls_Front");
			Assert.That(wallsTransform, Is.Not.Null);
			Tilemap walls = wallsTransform.GetComponent<Tilemap>();
			Assert.That(walls, Is.Not.Null);
			Assert.That(walls.GetTile(new Vector3Int(2, 0, 0)), Is.EqualTo(southTile));
			Object.DestroyImmediate(result.Root);
			Object.DestroyImmediate(southTile);
			Object.DestroyImmediate(profile);
		}

		[Test]
		public void RectangleStampRenderingUsesCornersAndUsableStraightUnits()
		{
			Tile floorTile = ScriptableObject.CreateInstance<Tile>();
			Tile straightTile = ScriptableObject.CreateInstance<Tile>();
			Dictionary<GeneratedBoundaryCornerOrientation, Tile> cornerTiles = new();
			foreach (GeneratedBoundaryCornerOrientation orientation in StampOrientations) cornerTiles.Add(orientation, ScriptableObject.CreateInstance<Tile>());
			RoomSectionStyleProfile profile = CreateValidStampProfile(floorTile);
			SetAllCornerPass(profile, RoomSectionStampRenderPass.Overlay);
			foreach (RoomSectionCornerWallStamp stamp in profile.CornerWallStamps) if (stamp.Kind == GeneratedBoundaryCornerKind.Outer) { stamp.Placements[0].Tile = cornerTiles[stamp.Orientation]; stamp.Placements[0].RenderPass = RoomSectionStampRenderPass.Overlay; }
			foreach (RoomSectionStraightWallStamp stamp in profile.StraightWallStamps) { stamp.Placements[0].Tile = straightTile; stamp.Placements[0].RenderPass = RoomSectionStampRenderPass.Sides; }
			RoomSectionPreviewResult result = RoomSectionPreviewBuilder.Build(GenerateRectangle(4, 3), profile, "RectangleStamps");
			Tilemap sides = GetPreviewTilemap(result.Root, "Walls_Sides");
			Tilemap overlay = GetPreviewTilemap(result.Root, "Walls_Overlay");
			Assert.That(result.Errors, Is.Empty);
			Assert.That(CountTile(sides, straightTile), Is.EqualTo(6));
			Assert.That(sides.GetTile(new Vector3Int(0, 1, 0)), Is.EqualTo(straightTile));
			Assert.That(sides.GetTile(new Vector3Int(1, 2, 0)), Is.EqualTo(straightTile));
			Assert.That(sides.GetTile(new Vector3Int(2, 2, 0)), Is.EqualTo(straightTile));
			Assert.That(sides.GetTile(new Vector3Int(3, 1, 0)), Is.EqualTo(straightTile));
			Assert.That(sides.GetTile(new Vector3Int(2, 0, 0)), Is.EqualTo(straightTile));
			Assert.That(sides.GetTile(new Vector3Int(1, 0, 0)), Is.EqualTo(straightTile));
			foreach (GeneratedBoundaryCorner corner in GenerateRectangle(4, 3).BoundaryLoops.Single().Corners) Assert.That(CountTile(overlay, cornerTiles[corner.Orientation]), Is.EqualTo(1));
			Object.DestroyImmediate(result.Root);
			Object.DestroyImmediate(profile);
			Object.DestroyImmediate(floorTile);
			Object.DestroyImmediate(straightTile);
			foreach (Tile tile in cornerTiles.Values) Object.DestroyImmediate(tile);
		}

		[Test]
		public void LShapeStampRenderingUsesInnerAndOuterOrientations()
		{
			Tile floorTile = ScriptableObject.CreateInstance<Tile>();
			Tile outerTile = ScriptableObject.CreateInstance<Tile>();
			Tile innerTile = ScriptableObject.CreateInstance<Tile>();
			RoomSectionStyleProfile profile = CreateValidStampProfile(floorTile);
			SetAllCornerPass(profile, RoomSectionStampRenderPass.Overlay);
			foreach (RoomSectionCornerWallStamp stamp in profile.CornerWallStamps) stamp.Placements[0].Tile = stamp.Kind == GeneratedBoundaryCornerKind.Inner ? innerTile : outerTile;
			RoomSectionShape shape = GenerateFixture(new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) });
			RoomSectionPreviewResult result = RoomSectionPreviewBuilder.Build(shape, profile, "LShapeStamps");
			Tilemap overlay = GetPreviewTilemap(result.Root, "Walls_Overlay");
			GeneratedBoundaryCorner inner = shape.BoundaryLoops.Single().Corners.Single(corner => corner.Kind == GeneratedBoundaryCornerKind.Inner);
			Assert.That(result.Errors, Is.Empty);
			Assert.That(inner.Orientation, Is.EqualTo(GeneratedBoundaryCornerOrientation.NorthEast));
			Assert.That(overlay.GetTile(new Vector3Int(inner.Position.x, inner.Position.y, 0)), Is.EqualTo(innerTile));
			Assert.That(CountTile(overlay, innerTile), Is.EqualTo(1));
			Assert.That(CountTile(overlay, outerTile), Is.EqualTo(5));
			Object.DestroyImmediate(result.Root);
			Object.DestroyImmediate(profile);
			Object.DestroyImmediate(floorTile);
			Object.DestroyImmediate(outerTile);
			Object.DestroyImmediate(innerTile);
		}

		[Test]
		public void CornerConsumptionRemovesUnitsFromBothRunEnds()
		{
			Tile tile = ScriptableObject.CreateInstance<Tile>();
			RoomSectionStyleProfile profile = CreateValidStampProfile(tile);
			SetAllCornerPass(profile, RoomSectionStampRenderPass.Overlay);
			SetAllStraightPass(profile, RoomSectionStampRenderPass.Sides);
			RoomSectionShape shape = GenerateRectangle(5, 4);
			GeneratedBoundaryLoop loop = shape.BoundaryLoops.Single();
			GeneratedBoundaryCorner start = loop.Corners.Single(corner => corner.Position == new Vector2Int(0, 4));
			GeneratedBoundaryCorner end = loop.Corners.Single(corner => corner.Position == new Vector2Int(5, 4));
			profile.CornerWallStamps.Single(stamp => stamp.Kind == start.Kind && stamp.Orientation == start.Orientation).OutgoingRunConsumption = 2;
			profile.CornerWallStamps.Single(stamp => stamp.Kind == end.Kind && stamp.Orientation == end.Orientation).IncomingRunConsumption = 2;
			RoomSectionPreviewResult result = RoomSectionPreviewBuilder.Build(shape, profile, "ConsumptionStamps");
			Tilemap sides = GetPreviewTilemap(result.Root, "Walls_Sides");
			Assert.That(result.Errors, Is.Empty);
			Assert.That(sides.GetTile(new Vector3Int(1, 3, 0)), Is.Null);
			Assert.That(sides.GetTile(new Vector3Int(2, 3, 0)), Is.EqualTo(tile));
			Assert.That(sides.GetTile(new Vector3Int(3, 3, 0)), Is.Null);
			Object.DestroyImmediate(result.Root);
			Object.DestroyImmediate(profile);
			Object.DestroyImmediate(tile);
		}

		[Test]
		public void ZeroUsableStraightLengthIsValidAndPlacesNothingForThatRun()
		{
			Tile tile = ScriptableObject.CreateInstance<Tile>();
			RoomSectionStyleProfile profile = CreateValidStampProfile(tile);
			SetAllCornerPass(profile, RoomSectionStampRenderPass.Overlay);
			SetAllStraightPass(profile, RoomSectionStampRenderPass.Sides);
			RoomSectionShape shape = GenerateRectangle(4, 3);
			GeneratedBoundaryLoop loop = shape.BoundaryLoops.Single();
			GeneratedBoundaryCorner start = loop.Corners.Single(corner => corner.Position == new Vector2Int(0, 0));
			GeneratedBoundaryCorner end = loop.Corners.Single(corner => corner.Position == new Vector2Int(0, 3));
			profile.CornerWallStamps.Single(stamp => stamp.Kind == start.Kind && stamp.Orientation == start.Orientation).OutgoingRunConsumption = 2;
			profile.CornerWallStamps.Single(stamp => stamp.Kind == end.Kind && stamp.Orientation == end.Orientation).IncomingRunConsumption = 1;
			RoomSectionPreviewResult result = RoomSectionPreviewBuilder.Build(shape, profile, "ZeroLengthStamps");
			Assert.That(result.Errors, Is.Empty);
			Assert.That(GetPreviewTilemap(result.Root, "Walls_Sides").GetTile(new Vector3Int(0, 1, 0)), Is.Null);
			Object.DestroyImmediate(result.Root);
			Object.DestroyImmediate(profile);
			Object.DestroyImmediate(tile);
		}

		[Test]
		public void NegativeUsableStraightLengthIsAnErrorAndIsNotClamped()
		{
			Tile tile = ScriptableObject.CreateInstance<Tile>();
			RoomSectionStyleProfile profile = CreateValidStampProfile(tile);
			SetAllCornerPass(profile, RoomSectionStampRenderPass.Overlay);
			SetAllStraightPass(profile, RoomSectionStampRenderPass.Sides);
			RoomSectionShape shape = GenerateRectangle(4, 3);
			GeneratedBoundaryLoop loop = shape.BoundaryLoops.Single();
			GeneratedBoundaryCorner start = loop.Corners.Single(corner => corner.Position == new Vector2Int(0, 0));
			GeneratedBoundaryCorner end = loop.Corners.Single(corner => corner.Position == new Vector2Int(0, 3));
			profile.CornerWallStamps.Single(stamp => stamp.Kind == start.Kind && stamp.Orientation == start.Orientation).OutgoingRunConsumption = 3;
			profile.CornerWallStamps.Single(stamp => stamp.Kind == end.Kind && stamp.Orientation == end.Orientation).IncomingRunConsumption = 1;
			RoomSectionPreviewResult result = RoomSectionPreviewBuilder.Build(shape, profile, "NegativeLengthStamps");
			AssertDiagnosticContains(result.Errors.Select(diagnostic => diagnostic.Message).ToList(), "negative usable length");
			Assert.That(GetPreviewTilemap(result.Root, "Walls_Sides").GetTile(new Vector3Int(0, 1, 0)), Is.Null);
			Object.DestroyImmediate(result.Root);
			Object.DestroyImmediate(profile);
			Object.DestroyImmediate(tile);
		}

		[Test]
		public void MultiTileStraightStampUsesEveryOffsetFromItsAnchor()
		{
			Tile floorTile = ScriptableObject.CreateInstance<Tile>();
			Tile firstTile = ScriptableObject.CreateInstance<Tile>();
			Tile secondTile = ScriptableObject.CreateInstance<Tile>();
			RoomSectionStyleProfile profile = CreateValidStampProfile(floorTile);
			RoomSectionStraightWallStamp north = profile.StraightWallStamps.Single(stamp => stamp.Direction == GeneratedWallDirection.North);
			north.Placements.Clear();
			north.Placements.Add(CreatePlacement(firstTile, Vector2Int.zero, RoomSectionStampRenderPass.Sides));
			north.Placements.Add(CreatePlacement(secondTile, Vector2Int.up, RoomSectionStampRenderPass.Sides));
			RoomSectionPreviewResult result = RoomSectionPreviewBuilder.Build(GenerateRectangle(4, 3), profile, "MultiTileStamps");
			Tilemap sides = GetPreviewTilemap(result.Root, "Walls_Sides");
			Assert.That(result.Errors, Is.Empty);
			Assert.That(sides.GetTile(new Vector3Int(1, 2, 0)), Is.EqualTo(firstTile));
			Assert.That(sides.GetTile(new Vector3Int(1, 3, 0)), Is.EqualTo(secondTile));
			Object.DestroyImmediate(result.Root);
			Object.DestroyImmediate(profile);
			Object.DestroyImmediate(floorTile);
			Object.DestroyImmediate(firstTile);
			Object.DestroyImmediate(secondTile);
		}

		[Test]
		public void MultiPassStampUsesTheSameOffsetOnDifferentTilemaps()
		{
			Tile floorTile = ScriptableObject.CreateInstance<Tile>();
			Tile backTile = ScriptableObject.CreateInstance<Tile>();
			Tile frontTile = ScriptableObject.CreateInstance<Tile>();
			RoomSectionStyleProfile profile = CreateValidStampProfile(floorTile);
			RoomSectionStraightWallStamp north = profile.StraightWallStamps.Single(stamp => stamp.Direction == GeneratedWallDirection.North);
			north.Placements.Clear();
			north.Placements.Add(CreatePlacement(backTile, Vector2Int.zero, RoomSectionStampRenderPass.Back));
			north.Placements.Add(CreatePlacement(frontTile, Vector2Int.zero, RoomSectionStampRenderPass.Front));
			RoomSectionPreviewResult result = RoomSectionPreviewBuilder.Build(GenerateRectangle(4, 3), profile, "MultiPassStamps");
			Assert.That(result.Errors, Is.Empty);
			Assert.That(GetPreviewTilemap(result.Root, "Walls_Back").GetTile(new Vector3Int(1, 2, 0)), Is.EqualTo(backTile));
			Assert.That(GetPreviewTilemap(result.Root, "Walls_Front").GetTile(new Vector3Int(1, 2, 0)), Is.EqualTo(frontTile));
			Object.DestroyImmediate(result.Root);
			Object.DestroyImmediate(profile);
			Object.DestroyImmediate(floorTile);
			Object.DestroyImmediate(backTile);
			Object.DestroyImmediate(frontTile);
		}

		[Test]
		public void CrossInstanceConflictKeepsTheFirstPlacement()
		{
			Tile floorTile = ScriptableObject.CreateInstance<Tile>();
			Tile cornerTile = ScriptableObject.CreateInstance<Tile>();
			Tile straightTile = ScriptableObject.CreateInstance<Tile>();
			RoomSectionStyleProfile profile = CreateValidStampProfile(floorTile);
			RoomSectionCornerWallStamp corner = profile.CornerWallStamps.Single(stamp => stamp.Kind == GeneratedBoundaryCornerKind.Outer && stamp.Orientation == GeneratedBoundaryCornerOrientation.NorthWest);
			corner.Placements.Clear();
			corner.Placements.Add(CreatePlacement(cornerTile, new Vector2Int(1, -1), RoomSectionStampRenderPass.Sides));
			profile.StraightWallStamps.Single(stamp => stamp.Direction == GeneratedWallDirection.North).Placements[0].Tile = straightTile;
			RoomSectionPreviewResult result = RoomSectionPreviewBuilder.Build(GenerateRectangle(4, 3), profile, "ConflictStamps");
			AssertDiagnosticContains(result.Errors.Select(diagnostic => diagnostic.Message).ToList(), "Placement conflict on Sides");
			Assert.That(GetPreviewTilemap(result.Root, "Walls_Sides").GetTile(new Vector3Int(1, 2, 0)), Is.EqualTo(cornerTile));
			Assert.That(GetPreviewTilemap(result.Root, "Walls_Sides").GetTile(new Vector3Int(1, 2, 0)), Is.Not.EqualTo(straightTile));
			Object.DestroyImmediate(result.Root);
			Object.DestroyImmediate(profile);
			Object.DestroyImmediate(floorTile);
			Object.DestroyImmediate(cornerTile);
			Object.DestroyImmediate(straightTile);
		}

		[Test]
		public void DifferentPassOverlapIsValid()
		{
			Tile floorTile = ScriptableObject.CreateInstance<Tile>();
			Tile cornerTile = ScriptableObject.CreateInstance<Tile>();
			Tile straightTile = ScriptableObject.CreateInstance<Tile>();
			RoomSectionStyleProfile profile = CreateValidStampProfile(floorTile);
			RoomSectionCornerWallStamp corner = profile.CornerWallStamps.Single(stamp => stamp.Kind == GeneratedBoundaryCornerKind.Outer && stamp.Orientation == GeneratedBoundaryCornerOrientation.NorthWest);
			corner.Placements.Clear();
			corner.Placements.Add(CreatePlacement(cornerTile, new Vector2Int(1, -1), RoomSectionStampRenderPass.Overlay));
			profile.StraightWallStamps.Single(stamp => stamp.Direction == GeneratedWallDirection.North).Placements[0].Tile = straightTile;
			RoomSectionPreviewResult result = RoomSectionPreviewBuilder.Build(GenerateRectangle(4, 3), profile, "DifferentPassStamps");
			Assert.That(result.Errors, Is.Empty);
			Assert.That(GetPreviewTilemap(result.Root, "Walls_Overlay").GetTile(new Vector3Int(1, 2, 0)), Is.EqualTo(cornerTile));
			Assert.That(GetPreviewTilemap(result.Root, "Walls_Sides").GetTile(new Vector3Int(1, 2, 0)), Is.EqualTo(straightTile));
			Object.DestroyImmediate(result.Root);
			Object.DestroyImmediate(profile);
			Object.DestroyImmediate(floorTile);
			Object.DestroyImmediate(cornerTile);
			Object.DestroyImmediate(straightTile);
		}

		[Test]
		public void MissingStraightStampDoesNotFallBackToLegacyMapping()
		{
			Tile floorTile = ScriptableObject.CreateInstance<Tile>();
			Tile legacyTile = ScriptableObject.CreateInstance<Tile>();
			RoomSectionStyleProfile profile = CreateValidStampProfile(floorTile);
			profile.StraightWallStamps.RemoveAll(stamp => stamp.Direction == GeneratedWallDirection.North);
			profile.WallMappings.Add(new RoomSectionWallVisualMapping { Direction = GeneratedWallDirection.North, Topology = GeneratedBoundaryTopology.Straight, Handedness = GeneratedBoundaryHandedness.None, Layer = 0, Tile = legacyTile });
			RoomSectionPreviewResult result = RoomSectionPreviewBuilder.Build(GenerateRectangle(4, 3), profile, "MissingStraightStamp");
			AssertDiagnosticContains(result.Errors.Select(diagnostic => diagnostic.Message).ToList(), "No unique straight stamp lookup for North");
			Assert.That(CountTile(GetPreviewTilemap(result.Root, "Walls_Sides"), legacyTile), Is.EqualTo(0));
			Object.DestroyImmediate(result.Root);
			Object.DestroyImmediate(profile);
			Object.DestroyImmediate(floorTile);
			Object.DestroyImmediate(legacyTile);
		}

		[Test]
		public void MissingCornerStampDoesNotFallBackToLegacyMapping()
		{
			Tile floorTile = ScriptableObject.CreateInstance<Tile>();
			Tile legacyTile = ScriptableObject.CreateInstance<Tile>();
			RoomSectionStyleProfile profile = CreateValidStampProfile(floorTile);
			profile.CornerWallStamps.RemoveAll(stamp => stamp.Kind == GeneratedBoundaryCornerKind.Outer && stamp.Orientation == GeneratedBoundaryCornerOrientation.NorthWest);
			profile.WallMappings.Add(new RoomSectionWallVisualMapping { Direction = GeneratedWallDirection.North, Topology = GeneratedBoundaryTopology.OuterTurn, Handedness = GeneratedBoundaryHandedness.Left, Layer = 0, Tile = legacyTile });
			RoomSectionPreviewResult result = RoomSectionPreviewBuilder.Build(GenerateRectangle(4, 3), profile, "MissingCornerStamp");
			AssertDiagnosticContains(result.Errors.Select(diagnostic => diagnostic.Message).ToList(), "No unique corner stamp lookup for Outer NorthWest");
			Assert.That(CountTile(GetPreviewTilemap(result.Root, "Walls_Sides"), legacyTile), Is.EqualTo(0));
			Assert.That(CountTile(GetPreviewTilemap(result.Root, "Walls_Overlay"), legacyTile), Is.EqualTo(0));
			Object.DestroyImmediate(result.Root);
			Object.DestroyImmediate(profile);
			Object.DestroyImmediate(floorTile);
			Object.DestroyImmediate(legacyTile);
		}

		[Test]
		public void StampPreviewCreatesRequiredHierarchyAndSorting()
		{
			Tile tile = ScriptableObject.CreateInstance<Tile>();
			RoomSectionStyleProfile profile = CreateValidStampProfile(tile);
			RoomSectionPreviewResult result = RoomSectionPreviewBuilder.Build(GenerateRectangle(4, 3), profile, "HierarchyStamps");
			Assert.That(result.Root.transform.Find("Floor"), Is.Not.Null);
			foreach (string pass in new[] { "Walls_Back", "Walls_Sides", "Walls_Front", "Walls_Overlay" })
			{
				Transform child = result.Root.transform.Find(pass);
				Assert.That(child, Is.Not.Null);
				TilemapRenderer renderer = child.GetComponent<TilemapRenderer>();
				Assert.That(renderer.sortingLayerName, Is.EqualTo(pass == "Walls_Overlay" ? "Interactives" : "Interactives"));
				Assert.That(renderer.sortingOrder, Is.EqualTo(pass == "Walls_Overlay" ? 2 : 1));
				Assert.That(renderer.mode, Is.EqualTo(TilemapRenderer.Mode.Individual));
			}
			Assert.That(result.Root.transform.Find("Walls_South"), Is.Null);
			Assert.That(result.Root.transform.Find("Walls_Other"), Is.Null);
			Assert.That(result.Root.transform.Find("Floor").GetComponent<TilemapRenderer>().sortingLayerName, Is.EqualTo("Background"));
			Assert.That(result.Root.transform.Find("Floor").GetComponent<TilemapRenderer>().sortingOrder, Is.EqualTo(0));
			Assert.That(result.Root.transform.Find("Walls_Back").GetComponent<TilemapCollider2D>(), Is.Not.Null);
			Assert.That(result.Root.transform.Find("Walls_Sides").GetComponent<TilemapCollider2D>(), Is.Not.Null);
			Assert.That(result.Root.transform.Find("Walls_Front").GetComponent<TilemapCollider2D>(), Is.Not.Null);
			Assert.That(result.Root.transform.Find("Walls_Overlay").GetComponent<TilemapCollider2D>(), Is.Null);
			Object.DestroyImmediate(result.Root);
			Object.DestroyImmediate(profile);
			Object.DestroyImmediate(tile);
		}

		[Test]
		public void StampRenderingIsDeterministic()
		{
			Tile tile = ScriptableObject.CreateInstance<Tile>();
			RoomSectionStyleProfile profile = CreateValidStampProfile(tile);
			RoomSectionShape shape = GenerateFixture(new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) });
			RoomSectionPreviewResult first = RoomSectionPreviewBuilder.Build(shape, profile, "DeterministicFirst");
			RoomSectionPreviewResult second = RoomSectionPreviewBuilder.Build(shape, profile, "DeterministicSecond");
			Assert.That(first.Errors, Is.Empty);
			Assert.That(second.Errors, Is.Empty);
			foreach (string pass in new[] { "Walls_Back", "Walls_Sides", "Walls_Front", "Walls_Overlay" }) AssertTilemapsEqual(GetPreviewTilemap(first.Root, pass), GetPreviewTilemap(second.Root, pass));
			Object.DestroyImmediate(first.Root);
			Object.DestroyImmediate(second.Root);
			Object.DestroyImmediate(profile);
			Object.DestroyImmediate(tile);
		}

		[Test]
		public void CanonicalStampShapeContainsAllRequiredDirectionsAndCorners()
		{
			RoomSectionShape shape = CreateCanonicalStampShape();
			Assert.That(shape.IsValid, Is.True, string.Join("; ", shape.Errors.Select(diagnostic => diagnostic.Message)));
			GeneratedBoundaryLoop loop = shape.BoundaryLoops.Single();
			Assert.That(loop.WallRuns.Select(run => run.Direction).Distinct().Count(), Is.EqualTo(4));
			foreach (GeneratedBoundaryCornerKind kind in StampKinds)
				foreach (GeneratedBoundaryCornerOrientation orientation in StampOrientations)
					Assert.That(loop.Corners.Count(corner => corner.Kind == kind && corner.Orientation == orientation), Is.EqualTo(1), $"{kind} {orientation}");
		}

		[Test]
		public void LegacyStyleProfileMappingLookupRemainsAvailable()
		{
			Tile tile = ScriptableObject.CreateInstance<Tile>();
			RoomSectionStyleProfile profile = ScriptableObject.CreateInstance<RoomSectionStyleProfile>();
			GeneratedBoundaryEdge edge = GenerateRectangle(4, 3).Boundaries.First(boundary => boundary.Direction == GeneratedWallDirection.North && boundary.Topology == GeneratedBoundaryTopology.Straight);
			profile.WallMappings.Add(new RoomSectionWallVisualMapping { Direction = edge.Direction, Topology = edge.Topology, Handedness = edge.Handedness, Layer = 0, Tile = tile });
			Assert.That(profile.TryGetTile(edge, 0, out TileBase actual), Is.True);
			Assert.That(actual, Is.SameAs(tile));
			Object.DestroyImmediate(profile);
			Object.DestroyImmediate(tile);
		}

		[Test]
		public void ValidStampStructurePassesValidation()
		{
			Tile tile = ScriptableObject.CreateInstance<Tile>();
			RoomSectionStyleProfile profile = CreateValidStampProfile(tile);
			Assert.That(profile.ValidateProfile(), Is.Empty);
			Object.DestroyImmediate(profile);
			Object.DestroyImmediate(tile);
		}

		[Test]
		public void MissingStraightStampIsDetectedForEveryDirection()
		{
			foreach (GeneratedWallDirection direction in StampDirections)
			{
				Tile tile = ScriptableObject.CreateInstance<Tile>();
				RoomSectionStyleProfile profile = CreateValidStampProfile(tile);
				profile.StraightWallStamps.RemoveAll(stamp => stamp.Direction == direction);
				AssertDiagnosticContains(profile.ValidateProfile(), $"Missing Straight {direction} stamp");
				Object.DestroyImmediate(profile);
				Object.DestroyImmediate(tile);
			}
		}

		[Test]
		public void MissingCornerStampIsDetectedForEveryRequiredPair()
		{
			foreach (GeneratedBoundaryCornerKind kind in StampKinds)
				foreach (GeneratedBoundaryCornerOrientation orientation in StampOrientations)
				{
					Tile tile = ScriptableObject.CreateInstance<Tile>();
					RoomSectionStyleProfile profile = CreateValidStampProfile(tile);
					profile.CornerWallStamps.RemoveAll(stamp => stamp.Kind == kind && stamp.Orientation == orientation);
					AssertDiagnosticContains(profile.ValidateProfile(), $"Missing {kind} {orientation} corner stamp");
					Object.DestroyImmediate(profile);
					Object.DestroyImmediate(tile);
				}
		}

		[Test]
		public void DuplicateStraightStampIsReported()
		{
			Tile tile = ScriptableObject.CreateInstance<Tile>();
			RoomSectionStyleProfile profile = CreateValidStampProfile(tile);
			profile.StraightWallStamps.Add(new RoomSectionStraightWallStamp { Direction = GeneratedWallDirection.North, Placements = new List<RoomSectionTilePlacement> { CreatePlacement(tile) } });
			AssertDiagnosticContains(profile.ValidateProfile(), "Duplicate Straight North stamps");
			Object.DestroyImmediate(profile);
			Object.DestroyImmediate(tile);
		}

		[Test]
		public void DuplicateCornerStampIsReported()
		{
			Tile tile = ScriptableObject.CreateInstance<Tile>();
			RoomSectionStyleProfile profile = CreateValidStampProfile(tile);
			profile.CornerWallStamps.Add(new RoomSectionCornerWallStamp { Kind = GeneratedBoundaryCornerKind.Outer, Orientation = GeneratedBoundaryCornerOrientation.NorthWest, Placements = new List<RoomSectionTilePlacement> { CreatePlacement(tile) } });
			AssertDiagnosticContains(profile.ValidateProfile(), "Duplicate Outer NorthWest corner stamps");
			Object.DestroyImmediate(profile);
			Object.DestroyImmediate(tile);
		}

		[Test]
		public void EmptyStampPlacementsAreReported()
		{
			Tile tile = ScriptableObject.CreateInstance<Tile>();
			RoomSectionStyleProfile profile = CreateValidStampProfile(tile);
			profile.StraightWallStamps.Single(stamp => stamp.Direction == GeneratedWallDirection.North).Placements.Clear();
			AssertDiagnosticContains(profile.ValidateProfile(), "Straight North stamp has no tile placements");
			Object.DestroyImmediate(profile);
			Object.DestroyImmediate(tile);
		}

		[Test]
		public void NullStampTileIsReported()
		{
			Tile tile = ScriptableObject.CreateInstance<Tile>();
			RoomSectionStyleProfile profile = CreateValidStampProfile(tile);
			profile.StraightWallStamps.Single(stamp => stamp.Direction == GeneratedWallDirection.North).Placements[0].Tile = null;
			AssertDiagnosticContains(profile.ValidateProfile(), "Straight North stamp has a null tile reference");
			Object.DestroyImmediate(profile);
			Object.DestroyImmediate(tile);
		}

		[Test]
		public void ConflictingStampPlacementsAreReported()
		{
			Tile tile = ScriptableObject.CreateInstance<Tile>();
			RoomSectionStyleProfile profile = CreateValidStampProfile(tile);
			profile.StraightWallStamps.Single(stamp => stamp.Direction == GeneratedWallDirection.North).Placements.Add(CreatePlacement(tile));
			AssertDiagnosticContains(profile.ValidateProfile(), "Straight North stamp has conflicting tile placements");
			Object.DestroyImmediate(profile);
			Object.DestroyImmediate(tile);
		}

		[Test]
		public void SameStampOffsetOnDifferentPassesIsValid()
		{
			Tile tile = ScriptableObject.CreateInstance<Tile>();
			RoomSectionStyleProfile profile = CreateValidStampProfile(tile);
			profile.StraightWallStamps.Single(stamp => stamp.Direction == GeneratedWallDirection.North).Placements.Add(CreatePlacement(tile, default, RoomSectionStampRenderPass.Back));
			Assert.That(profile.ValidateProfile().Any(diagnostic => diagnostic.Contains("conflicting tile placements")), Is.False);
			Object.DestroyImmediate(profile);
			Object.DestroyImmediate(tile);
		}

		[Test]
		public void NegativeIncomingCornerConsumptionIsRejected()
		{
			Tile tile = ScriptableObject.CreateInstance<Tile>();
			RoomSectionStyleProfile profile = CreateValidStampProfile(tile);
			profile.CornerWallStamps[0].IncomingRunConsumption = -1;
			AssertDiagnosticContains(profile.ValidateProfile(), "negative incoming consumption");
			Object.DestroyImmediate(profile);
			Object.DestroyImmediate(tile);
		}

		[Test]
		public void NegativeOutgoingCornerConsumptionIsRejected()
		{
			Tile tile = ScriptableObject.CreateInstance<Tile>();
			RoomSectionStyleProfile profile = CreateValidStampProfile(tile);
			profile.CornerWallStamps[0].OutgoingRunConsumption = -1;
			AssertDiagnosticContains(profile.ValidateProfile(), "negative outgoing consumption");
			Object.DestroyImmediate(profile);
			Object.DestroyImmediate(tile);
		}

		[Test]
		public void ZeroCornerConsumptionIsValid()
		{
			Tile tile = ScriptableObject.CreateInstance<Tile>();
			RoomSectionStyleProfile profile = CreateValidStampProfile(tile);
			profile.CornerWallStamps[0].IncomingRunConsumption = 0;
			profile.CornerWallStamps[0].OutgoingRunConsumption = 0;
			Assert.That(profile.ValidateProfile(), Is.Empty);
			Object.DestroyImmediate(profile);
			Object.DestroyImmediate(tile);
		}

		[Test]
		public void StraightStampLookupReturnsTheUniqueDefinition()
		{
			Tile tile = ScriptableObject.CreateInstance<Tile>();
			RoomSectionStyleProfile profile = CreateValidStampProfile(tile);
			RoomSectionStraightWallStamp expected = profile.StraightWallStamps.Single(stamp => stamp.Direction == GeneratedWallDirection.East);
			Assert.That(profile.TryGetStraightStamp(GeneratedWallDirection.East, out RoomSectionStraightWallStamp actual), Is.True);
			Assert.That(actual, Is.SameAs(expected));
			Object.DestroyImmediate(profile);
			Object.DestroyImmediate(tile);
		}

		[Test]
		public void CornerStampLookupReturnsTheUniqueDefinition()
		{
			Tile tile = ScriptableObject.CreateInstance<Tile>();
			RoomSectionStyleProfile profile = CreateValidStampProfile(tile);
			RoomSectionCornerWallStamp expected = profile.CornerWallStamps.Single(stamp => stamp.Kind == GeneratedBoundaryCornerKind.Inner && stamp.Orientation == GeneratedBoundaryCornerOrientation.SouthEast);
			Assert.That(profile.TryGetCornerStamp(GeneratedBoundaryCornerKind.Inner, GeneratedBoundaryCornerOrientation.SouthEast, out RoomSectionCornerWallStamp actual), Is.True);
			Assert.That(actual, Is.SameAs(expected));
			Object.DestroyImmediate(profile);
			Object.DestroyImmediate(tile);
		}

		[Test]
		public void NonSquareCellSizeIsRejected()
		{
			Tile tile = ScriptableObject.CreateInstance<Tile>();
			RoomSectionStyleProfile profile = CreateValidStampProfile(tile);
			profile.CellSize = new Vector3(0.3f, 0.4f, 0.3f);
			AssertDiagnosticContains(profile.ValidateProfile(), "Grid X and Y cell size must match");
			Object.DestroyImmediate(profile);
			Object.DestroyImmediate(tile);
		}

		[Test]
		public void NonContractSquareCellSizeIsRejected()
		{
			Tile tile = ScriptableObject.CreateInstance<Tile>();
			RoomSectionStyleProfile profile = CreateValidStampProfile(tile);
			profile.CellSize = new Vector3(0.5f, 0.5f, 0.3f);
			AssertDiagnosticContains(profile.ValidateProfile(), "RoomGenerator contract of 0.3 x 0.3");
			Object.DestroyImmediate(profile);
			Object.DestroyImmediate(tile);
		}

		[Test]
		public void RuntimeContractCellSizeIsAccepted()
		{
			Tile tile = ScriptableObject.CreateInstance<Tile>();
			RoomSectionStyleProfile profile = CreateValidStampProfile(tile);
			profile.CellSize = new Vector3(0.3f, 0.3f, 0.3f);
			Assert.That(profile.ValidateProfile(), Is.Empty);
			Object.DestroyImmediate(profile);
			Object.DestroyImmediate(tile);
		}

		[Test]
		public void ChangingStyleProfileDoesNotChangeFootprint()
		{
			RoomSectionShape shape = RoomSectionShapeGenerationLogic.Generate(new RoomSectionGenerationRequest { Seed = 12, Preset = RoomSectionShapePreset.Random, Size = new Vector2Int(9, 7), Extensions = 2 });
			Tile firstFloorTile = ScriptableObject.CreateInstance<Tile>();
			Tile secondFloorTile = ScriptableObject.CreateInstance<Tile>();
			RoomSectionStyleProfile firstProfile = CreateValidStampProfile(firstFloorTile);
			RoomSectionStyleProfile secondProfile = CreateValidStampProfile(secondFloorTile);
			RoomSectionPreviewResult firstResult = RoomSectionPreviewBuilder.Build(shape, firstProfile, "FirstProfile");
			RoomSectionPreviewResult secondResult = RoomSectionPreviewBuilder.Build(shape, secondProfile, "SecondProfile");
			RoomSection firstSection = firstResult.Root.GetComponent<RoomSection>();
			RoomSection secondSection = secondResult.Root.GetComponent<RoomSection>();
			Assert.That(firstSection, Is.Not.Null);
			Assert.That(secondSection, Is.Not.Null);
			Assert.That(secondSection.GetFootprintCells(), Is.EquivalentTo(firstSection.GetFootprintCells()));
			Object.DestroyImmediate(firstResult.Root);
			Object.DestroyImmediate(secondResult.Root);
			Object.DestroyImmediate(firstFloorTile);
			Object.DestroyImmediate(secondFloorTile);
			Object.DestroyImmediate(firstProfile);
			Object.DestroyImmediate(secondProfile);
		}

		[Test]
		public void SavedPreviewRemainsAValidRoomSectionPrefab()
		{
			RoomSectionShape shape = RoomSectionShapeGenerationLogic.Generate(new RoomSectionGenerationRequest { Seed = 14, Preset = RoomSectionShapePreset.LShape, Size = new Vector2Int(7, 6) });
			Tile tile = ScriptableObject.CreateInstance<Tile>();
			RoomSectionStyleProfile profile = CreateValidStampProfile(tile);
			RoomSectionPreviewResult result = RoomSectionPreviewBuilder.Build(shape, profile, "SavedPreview");
			string path = AssetDatabase.GenerateUniqueAssetPath("Assets/RoomSectionGeneratorTest.prefab");
			try
			{
				GameObject prefab = PrefabUtility.SaveAsPrefabAsset(result.Root, path);
				Assert.That(prefab, Is.Not.Null);
				RoomSection section = prefab.GetComponent<RoomSection>();
				Assert.That(section, Is.Not.Null);
				Assert.That(section.GetFootprintCells(), Is.EquivalentTo(shape.Cells));
			}
			finally
			{
				Object.DestroyImmediate(result.Root);
				Object.DestroyImmediate(profile);
				Object.DestroyImmediate(tile);
				AssetDatabase.DeleteAsset(path);
			}
		}

		[Test]
		public void LegacySettingsCanCreateStyleProfile()
		{
			RoomSectionGeneratorSettings legacy = ScriptableObject.CreateInstance<RoomSectionGeneratorSettings>();
			Tile northTile = ScriptableObject.CreateInstance<Tile>();
			legacy.NorthWallLayers.Add(northTile);
			RoomSectionStyleProfile profile = RoomSectionStyleProfileMigration.CreateFromLegacy(legacy);
			Assert.That(profile.NorthWallLayerCount, Is.EqualTo(1));
			Assert.That(profile.WallMappings.Exists(mapping => mapping.Direction == GeneratedWallDirection.North && mapping.Topology == GeneratedBoundaryTopology.Straight && mapping.Tile == northTile), Is.True);
			Object.DestroyImmediate(profile);
			Object.DestroyImmediate(northTile);
			Object.DestroyImmediate(legacy);
		}

		[Test]
		public void NorthWallLayerCountComesFromSettings()
		{
			RoomSectionGeneratorSettings settings = ScriptableObject.CreateInstance<RoomSectionGeneratorSettings>();
			settings.NorthWallLayers.Add(null);
			settings.NorthWallLayers.Add(null);
			settings.NorthWallLayers.Add(null);
			GeneratedRoomSection result = RoomSectionGenerationLogic.Generate(1, 6, 6, 0, false, settings);
			Assert.That(result.Walls.FindAll(wall => wall.Direction == GeneratedWallDirection.North && wall.Corner == GeneratedWallCorner.None).Count, Is.EqualTo(12));
			Object.DestroyImmediate(settings);
		}

		[Test]
		public void NorthCornersUseConfiguredLayerCount()
		{
			RoomSectionGeneratorSettings settings = ScriptableObject.CreateInstance<RoomSectionGeneratorSettings>();
			for (int i = 0; i < 3; i++) settings.NorthWallLayers.Add(null);
			for (int i = 0; i < 3; i++) settings.NorthWestCornerLayers.Add(null);
			for (int i = 0; i < 3; i++) settings.NorthEastCornerLayers.Add(null);
			GeneratedRoomSection result = RoomSectionGenerationLogic.Generate(2, 6, 6, 0, false, settings);
			Assert.That(result.Walls.FindAll(wall => wall.Direction == GeneratedWallDirection.North && wall.Corner == GeneratedWallCorner.NorthWest).Count, Is.EqualTo(3));
			Assert.That(result.Walls.FindAll(wall => wall.Direction == GeneratedWallDirection.North && wall.Corner == GeneratedWallCorner.NorthEast).Count, Is.EqualTo(3));
			Object.DestroyImmediate(settings);
		}

		[Test]
		public void BuilderUsesConfiguredGridCellSize()
		{
			RoomSectionGeneratorSettings settings = ScriptableObject.CreateInstance<RoomSectionGeneratorSettings>();
			settings.CellSize = new Vector3(0.3f, 0.3f, 0.3f);
			settings.NorthWallLayers.Add(null);
			GeneratedRoomSection result = RoomSectionGenerationLogic.Generate(3, 4, 4, 0, false, settings);
			GameObject root = RoomSectionGeneratorBuilder.Build(result, settings, "Test");
			Grid grid = root.GetComponent<Grid>();
			Assert.That(grid, Is.Not.Null);
			Assert.That(grid.cellSize, Is.EqualTo(settings.CellSize));
			Object.DestroyImmediate(root);
			Object.DestroyImmediate(settings);
		}

		[Test]
		public void ConfiguredBuilderCreatesSeparateSouthAndOtherWallTilemaps()
		{
			RoomSectionGeneratorSettings settings = ScriptableObject.CreateInstance<RoomSectionGeneratorSettings>();
			for (int i = 0; i < 3; i++) settings.NorthWallLayers.Add(null);
			GeneratedRoomSection data = RoomSectionGenerationLogic.Generate(4, 6, 5, 0, false, settings);
			GameObject root = RoomSectionGeneratorBuilder.Build(data, settings, "TemplateTest");
			Assert.That(root.transform.Find("Walls_South"), Is.Not.Null);
			Assert.That(root.transform.Find("Walls_Other"), Is.Not.Null);
			Object.DestroyImmediate(root);
			Object.DestroyImmediate(settings);
		}

		[Test]
		public void NorthTurnUsesCornerHandednessOfTheTouchedVertex()
		{
			RoomSectionGeneratorSettings settings = ScriptableObject.CreateInstance<RoomSectionGeneratorSettings>();
			for (int i = 0; i < 3; i++) settings.NorthWallLayers.Add(null);
			GeneratedRoomSection result = RoomSectionGenerationLogic.Generate(1, 10, 8, 3, false, settings);
			System.Collections.Generic.List<GeneratedWall> walls = result.Walls.FindAll(wall => wall.Direction == GeneratedWallDirection.North && wall.Cell == new Vector2Int(5, 7));
			Assert.That(walls.Count, Is.EqualTo(3));
			foreach (GeneratedWall wall in walls) Assert.That(wall.Corner, Is.EqualTo(GeneratedWallCorner.InnerNorthEast));
			Object.DestroyImmediate(settings);
		}

		[Test]
		public void VerticalWallsDoNotRetainHorizontalCornerRoles()
		{
			RoomSectionGeneratorSettings settings = ScriptableObject.CreateInstance<RoomSectionGeneratorSettings>();
			for (int i = 0; i < 3; i++) settings.NorthWallLayers.Add(null);
			GeneratedRoomSection result = RoomSectionGenerationLogic.Generate(12345, 10, 8, 2, false, settings);
			GeneratedWall wall = result.Walls.Find(item => item.Direction == GeneratedWallDirection.West && item.Cell == new Vector2Int(0, 0));
			Assert.That(wall, Is.Not.Null);
			Assert.That(wall.Corner, Is.EqualTo(GeneratedWallCorner.None));
			Object.DestroyImmediate(settings);
		}

		private static readonly GeneratedWallDirection[] StampDirections = { GeneratedWallDirection.North, GeneratedWallDirection.East, GeneratedWallDirection.South, GeneratedWallDirection.West };
		private static readonly GeneratedBoundaryCornerKind[] StampKinds = { GeneratedBoundaryCornerKind.Outer, GeneratedBoundaryCornerKind.Inner };
		private static readonly GeneratedBoundaryCornerOrientation[] StampOrientations = { GeneratedBoundaryCornerOrientation.NorthWest, GeneratedBoundaryCornerOrientation.NorthEast, GeneratedBoundaryCornerOrientation.SouthEast, GeneratedBoundaryCornerOrientation.SouthWest };

		private static RoomSectionShape CreateCanonicalStampShape()
		{
			List<Vector2Int> cells = new();
			for (int x = 0; x < 7; x++)
				for (int y = 0; y < 7; y++)
					if (x >= 2 && x <= 4 || y >= 2 && y <= 4) cells.Add(new Vector2Int(x, y));
			return GenerateFixture(cells);
		}

		private static RoomSectionStyleProfile CreateValidStampProfile(Tile tile)
		{
			RoomSectionStyleProfile profile = ScriptableObject.CreateInstance<RoomSectionStyleProfile>();
			profile.FloorTile = tile;
			foreach (GeneratedWallDirection direction in StampDirections) profile.StraightWallStamps.Add(new RoomSectionStraightWallStamp { Direction = direction, Placements = new List<RoomSectionTilePlacement> { CreatePlacement(tile) } });
			foreach (GeneratedBoundaryCornerKind kind in StampKinds)
				foreach (GeneratedBoundaryCornerOrientation orientation in StampOrientations)
					profile.CornerWallStamps.Add(new RoomSectionCornerWallStamp { Kind = kind, Orientation = orientation, Placements = new List<RoomSectionTilePlacement> { CreatePlacement(tile) } });
			return profile;
		}

		private static RoomSectionTilePlacement CreatePlacement(Tile tile, Vector2Int offset = default, RoomSectionStampRenderPass renderPass = RoomSectionStampRenderPass.Sides)
		{
			return new RoomSectionTilePlacement { Offset = offset, Tile = tile, RenderPass = renderPass };
		}

		private static void AssertDiagnosticContains(List<string> diagnostics, string expected)
		{
			Assert.That(diagnostics.Any(diagnostic => diagnostic.Contains(expected)), Is.True, $"Expected diagnostic containing '{expected}', got: {string.Join("; ", diagnostics)}");
		}

		private static void SetAllCornerPass(RoomSectionStyleProfile profile, RoomSectionStampRenderPass renderPass)
		{
			foreach (RoomSectionCornerWallStamp stamp in profile.CornerWallStamps) foreach (RoomSectionTilePlacement placement in stamp.Placements) placement.RenderPass = renderPass;
		}

		private static void SetAllStraightPass(RoomSectionStyleProfile profile, RoomSectionStampRenderPass renderPass)
		{
			foreach (RoomSectionStraightWallStamp stamp in profile.StraightWallStamps) foreach (RoomSectionTilePlacement placement in stamp.Placements) placement.RenderPass = renderPass;
		}

		private static Tilemap GetPreviewTilemap(GameObject root, string name)
		{
			return root.transform.Find(name).GetComponent<Tilemap>();
		}

		private static int CountTile(Tilemap tilemap, TileBase tile)
		{
			BoundsInt bounds = tilemap.cellBounds;
			if (bounds.size.x <= 0 || bounds.size.y <= 0) return 0;
			return tilemap.GetTilesBlock(bounds).Count(candidate => candidate == tile);
		}

		private static void AssertTilemapsEqual(Tilemap first, Tilemap second)
		{
			BoundsInt firstBounds = first.cellBounds;
			BoundsInt secondBounds = second.cellBounds;
			int minX = Mathf.Min(firstBounds.xMin, secondBounds.xMin);
			int maxX = Mathf.Max(firstBounds.xMax, secondBounds.xMax);
			int minY = Mathf.Min(firstBounds.yMin, secondBounds.yMin);
			int maxY = Mathf.Max(firstBounds.yMax, secondBounds.yMax);
			for (int x = minX; x < maxX; x++)
				for (int y = minY; y < maxY; y++)
					Assert.That(first.GetTile(new Vector3Int(x, y, 0)), Is.EqualTo(second.GetTile(new Vector3Int(x, y, 0))), $"Cell {x},{y}");
		}

		private static RoomSectionShape GenerateRectangle(int width, int height)
		{
			List<Vector2Int> cells = new();
			for (int x = 0; x < width; x++) for (int y = 0; y < height; y++) cells.Add(new Vector2Int(x, y));
			return GenerateFixture(cells);
		}

		private static RoomSectionShape GenerateBay(int side)
		{
			List<Vector2Int> cells = new();
			for (int x = 0; x < 4; x++) for (int y = 0; y < 4; y++) cells.Add(new Vector2Int(x, y));
			for (int x = 0; x < 2; x++) for (int y = 0; y < 2; y++)
			{
				Vector2Int cell = side switch
				{
					0 => new Vector2Int(x + 1, y + 4),
					1 => new Vector2Int(x + 4, y + 1),
					2 => new Vector2Int(x + 1, y - 2),
					_ => new Vector2Int(x - 2, y + 1)
				};
				cells.Add(cell);
			}
			return GenerateFixture(cells);
		}

		private static RoomSectionShape GenerateFixture(IEnumerable<Vector2Int> cells)
		{
			return RoomSectionShapeGenerationLogic.GenerateFromCells(cells);
		}

		private static string BoundarySegmentKey(GeneratedWallDirection direction, Vector2Int start, Vector2Int end)
		{
			return $"{direction}:{start.x},{start.y}:{end.x},{end.y}";
		}

		private static void AssertLoopTopology(GeneratedBoundaryLoop expected, GeneratedBoundaryLoop actual, Vector2Int translation = default)
		{
			Assert.That(actual.IsClosed, Is.EqualTo(expected.IsClosed));
			Assert.That(actual.IsClockwise, Is.EqualTo(expected.IsClockwise));
			Assert.That(actual.Vertices.Count, Is.EqualTo(expected.Vertices.Count));
			Assert.That(actual.WallRuns.Count, Is.EqualTo(expected.WallRuns.Count));
			Assert.That(actual.Corners.Count, Is.EqualTo(expected.Corners.Count));
			for (int i = 0; i < expected.Vertices.Count; i++) Assert.That(actual.Vertices[i], Is.EqualTo(expected.Vertices[i] + translation));
			for (int i = 0; i < expected.WallRuns.Count; i++)
			{
				GeneratedWallRun expectedRun = expected.WallRuns[i];
				GeneratedWallRun actualRun = actual.WallRuns[i];
				Assert.That(actualRun.Direction, Is.EqualTo(expectedRun.Direction));
				Assert.That(actualRun.StartVertex, Is.EqualTo(expectedRun.StartVertex + translation));
				Assert.That(actualRun.EndVertex, Is.EqualTo(expectedRun.EndVertex + translation));
				Assert.That(actualRun.Length, Is.EqualTo(expectedRun.Length));
			}
			for (int i = 0; i < expected.Corners.Count; i++)
			{
				GeneratedBoundaryCorner expectedCorner = expected.Corners[i];
				GeneratedBoundaryCorner actualCorner = actual.Corners[i];
				Assert.That(actualCorner.Position, Is.EqualTo(expectedCorner.Position + translation));
				Assert.That(actualCorner.Kind, Is.EqualTo(expectedCorner.Kind));
				Assert.That(actualCorner.Orientation, Is.EqualTo(expectedCorner.Orientation));
				Assert.That(actualCorner.IncomingWallSide, Is.EqualTo(expectedCorner.IncomingWallSide));
				Assert.That(actualCorner.OutgoingWallSide, Is.EqualTo(expectedCorner.OutgoingWallSide));
			}
		}
	}
}
