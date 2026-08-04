using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnboundArcana.Core.Rooms.Editor
{
	public enum RoomSectionShapePreset
	{
		Rectangle,
		SideBay,
		EndBay,
		LShape,
		Random
	}

	public enum GeneratedBoundaryTopology
	{
		Straight,
		OuterTurn,
		InnerTurn,
		Cap
	}

	public enum GeneratedBoundaryHandedness
	{
		None,
		Left,
		Right
	}

	public enum GeneratedBoundaryCornerKind
	{
		Outer,
		Inner
	}

	public enum GeneratedBoundaryCornerOrientation
	{
		NorthWest,
		NorthEast,
		SouthEast,
		SouthWest
	}

	public enum RoomSectionDiagnosticSeverity
	{
		Error,
		Warning
	}

	[Serializable]
	public sealed class RoomSectionGenerationRequest
	{
		public int Seed = 12345;
		public RoomSectionShapePreset Preset = RoomSectionShapePreset.Random;
		public Vector2Int Size = new(10, 8);
		public Vector2Int ExtensionSize = new(2, 2);
		public int Extensions = 2;
		public bool Symmetric;

		public RoomSectionGenerationRequest Normalize()
		{
			Size = new Vector2Int(Mathf.Max(3, Size.x), Mathf.Max(3, Size.y));
			ExtensionSize = new Vector2Int(Mathf.Max(1, ExtensionSize.x), Mathf.Max(1, ExtensionSize.y));
			Extensions = Mathf.Max(0, Extensions);
			return this;
		}
	}

	public sealed class RoomSectionDiagnostic
	{
		public readonly RoomSectionDiagnosticSeverity Severity;
		public readonly string Message;
		public readonly Vector2Int? Cell;
		public readonly GeneratedWallDirection? Direction;

		public RoomSectionDiagnostic(RoomSectionDiagnosticSeverity severity, string message, Vector2Int? cell = null, GeneratedWallDirection? direction = null)
		{
			Severity = severity;
			Message = message;
			Cell = cell;
			Direction = direction;
		}

		public override string ToString()
		{
			string location = Cell.HasValue ? $" at {Cell.Value}" : string.Empty;
			string direction = Direction.HasValue ? $" {Direction.Value}" : string.Empty;
			return $"{Message}{direction}{location}";
		}
	}

	public sealed class GeneratedBoundaryVertex
	{
		public readonly Vector2Int Position;
		public readonly GeneratedBoundaryTopology Topology;

		public GeneratedBoundaryVertex(Vector2Int position, GeneratedBoundaryTopology topology)
		{
			Position = position;
			Topology = topology;
		}
	}

	public sealed class GeneratedBoundaryEdge
	{
		public readonly Vector2Int Cell;
		public readonly GeneratedWallDirection Direction;
		public readonly Vector2Int StartVertex;
		public readonly Vector2Int EndVertex;
		public readonly GeneratedBoundaryTopology Topology;
		public readonly GeneratedBoundaryHandedness Handedness;
		public readonly bool HasMultipleFeatures;

		public GeneratedBoundaryEdge(Vector2Int cell, GeneratedWallDirection direction, Vector2Int startVertex, Vector2Int endVertex, GeneratedBoundaryTopology topology, GeneratedBoundaryHandedness handedness, bool hasMultipleFeatures)
		{
			Cell = cell;
			Direction = direction;
			StartVertex = startVertex;
			EndVertex = endVertex;
			Topology = topology;
			Handedness = handedness;
			HasMultipleFeatures = hasMultipleFeatures;
		}
	}

	public sealed class GeneratedWallRun
	{
		public readonly GeneratedWallDirection Direction;
		public readonly Vector2Int StartVertex;
		public readonly Vector2Int EndVertex;
		public readonly int Length;

		public GeneratedWallDirection ExposedWallSide => Direction;

		public GeneratedWallDirection Side => Direction;

		public GeneratedWallRun(GeneratedWallDirection direction, Vector2Int startVertex, Vector2Int endVertex, int length)
		{
			Direction = direction;
			StartVertex = startVertex;
			EndVertex = endVertex;
			Length = length;
		}
	}

	public sealed class GeneratedBoundaryCorner
	{
		public readonly Vector2Int Position;
		public readonly GeneratedBoundaryCornerKind Kind;
		public readonly GeneratedBoundaryCornerOrientation Orientation;
		public readonly GeneratedWallDirection IncomingWallSide;
		public readonly GeneratedWallDirection OutgoingWallSide;

		public GeneratedWallDirection IncomingDirection => IncomingWallSide;

		public GeneratedWallDirection OutgoingDirection => OutgoingWallSide;

		public GeneratedBoundaryCorner(Vector2Int position, GeneratedBoundaryCornerKind kind, GeneratedBoundaryCornerOrientation orientation, GeneratedWallDirection incomingWallSide, GeneratedWallDirection outgoingWallSide)
		{
			Position = position;
			Kind = kind;
			Orientation = orientation;
			IncomingWallSide = incomingWallSide;
			OutgoingWallSide = outgoingWallSide;
		}
	}

	public sealed class GeneratedBoundaryLoop
	{
		public readonly List<Vector2Int> Vertices = new();
		public readonly List<GeneratedWallRun> WallRuns = new();
		public readonly List<GeneratedBoundaryCorner> Corners = new();
		public bool IsClockwise { get; internal set; }
		public bool IsClosed => Vertices.Count > 1 && Vertices[0] == Vertices[Vertices.Count - 1];

		public IReadOnlyList<GeneratedWallRun> Runs => WallRuns;
	}

	public sealed class RoomSectionShape
	{
		public readonly HashSet<Vector2Int> Cells = new();
		public readonly List<GeneratedBoundaryEdge> Boundaries = new();
		public readonly List<GeneratedBoundaryVertex> BoundaryVertices = new();
		public readonly List<GeneratedBoundaryLoop> BoundaryLoops = new();
		public readonly List<RoomSectionDiagnostic> Diagnostics = new();

		public IReadOnlyList<GeneratedBoundaryLoop> Loops => BoundaryLoops;

		public IEnumerable<RoomSectionDiagnostic> Errors => Diagnostics.Where(diagnostic => diagnostic.Severity == RoomSectionDiagnosticSeverity.Error);
		public IEnumerable<RoomSectionDiagnostic> Warnings => Diagnostics.Where(diagnostic => diagnostic.Severity == RoomSectionDiagnosticSeverity.Warning);
		public bool IsValid => !Errors.Any();
	}

	public static class RoomSectionShapeGenerationLogic
	{
		private readonly struct BoundaryKey : IEquatable<BoundaryKey>
		{
			public readonly Vector2Int Cell;
			public readonly GeneratedWallDirection Direction;

			public BoundaryKey(Vector2Int cell, GeneratedWallDirection direction)
			{
				Cell = cell;
				Direction = direction;
			}

			public bool Equals(BoundaryKey other)
			{
				return Cell == other.Cell && Direction == other.Direction;
			}

			public override bool Equals(object obj)
			{
				return obj is BoundaryKey other && Equals(other);
			}

			public override int GetHashCode()
			{
				return HashCode.Combine(Cell, Direction);
			}
		}

		private sealed class BoundaryVertexFeature
		{
			public GeneratedBoundaryTopology Topology;
		}

		public static RoomSectionShape Generate(RoomSectionGenerationRequest request)
		{
			request = request ?? new RoomSectionGenerationRequest();
			request.Normalize();
			RoomSectionShape result = new();
			System.Random random = new(request.Seed);
			CreateBaseShape(result.Cells, request, random);
			FinalizeShape(result);
			return result;
		}

		public static RoomSectionShape GenerateFromCells(IEnumerable<Vector2Int> cells)
		{
			RoomSectionShape result = new();
			if (cells != null) foreach (Vector2Int cell in cells) result.Cells.Add(cell);
			FinalizeShape(result);
			return result;
		}

		private static void FinalizeShape(RoomSectionShape result)
		{
			BuildBoundaries(result);
			BuildBoundaryLoops(result);
			Validate(result);
		}

		private static void CreateBaseShape(HashSet<Vector2Int> cells, RoomSectionGenerationRequest request, System.Random random)
		{
			switch (request.Preset)
			{
				case RoomSectionShapePreset.SideBay:
					AddRectangle(cells, Vector2Int.zero, request.Size);
					AddBay(cells, request.Size, request.ExtensionSize, random.Next(4), random);
					break;
				case RoomSectionShapePreset.EndBay:
					AddRectangle(cells, Vector2Int.zero, request.Size);
					AddBay(cells, request.Size, request.ExtensionSize, random.Next(2) * 2, random);
					break;
				case RoomSectionShapePreset.LShape:
					AddLShape(cells, request.Size);
					break;
				case RoomSectionShapePreset.Random:
					AddRectangle(cells, Vector2Int.zero, request.Size);
					AddRandomBays(cells, request, random);
					break;
				default:
					AddRectangle(cells, Vector2Int.zero, request.Size);
					break;
			}
		}

		private static void AddRandomBays(HashSet<Vector2Int> cells, RoomSectionGenerationRequest request, System.Random random)
		{
			if (request.Symmetric)
			{
				int pairCount = (request.Extensions + 1) / 2;
				for (int i = 0; i < pairCount; i++)
				{
					int side = random.Next(2) == 0 ? 0 : 1;
					int maximumOffset = side == 0 ? request.Size.x - Mathf.Clamp(request.ExtensionSize.x, 1, request.Size.x) : request.Size.y - Mathf.Clamp(request.ExtensionSize.y, 1, request.Size.y);
					int offset = random.Next(0, Mathf.Max(1, maximumOffset + 1));
					AddBay(cells, request.Size, request.ExtensionSize, side, random, offset);
					AddBay(cells, request.Size, request.ExtensionSize, side == 0 ? 2 : 3, random, offset);
				}
				return;
			}
			for (int i = 0; i < request.Extensions; i++) AddBay(cells, request.Size, request.ExtensionSize, random.Next(4), random);
		}

		private static void AddBay(HashSet<Vector2Int> cells, Vector2Int baseSize, Vector2Int extensionSize, int side, System.Random random, int? requestedOffset = null)
		{
			int bayWidth = Mathf.Clamp(extensionSize.x, 1, baseSize.x);
			int bayHeight = Mathf.Clamp(extensionSize.y, 1, baseSize.y);
			int x = requestedOffset ?? random.Next(0, Mathf.Max(1, baseSize.x - bayWidth + 1));
			int y = requestedOffset ?? random.Next(0, Mathf.Max(1, baseSize.y - bayHeight + 1));
			switch (side)
			{
				case 0:
					AddRectangle(cells, new Vector2Int(x, baseSize.y), new Vector2Int(bayWidth, extensionSize.y));
					break;
				case 1:
					AddRectangle(cells, new Vector2Int(baseSize.x, y), new Vector2Int(extensionSize.x, bayHeight));
					break;
				case 2:
					AddRectangle(cells, new Vector2Int(x, -extensionSize.y), new Vector2Int(bayWidth, extensionSize.y));
					break;
				default:
					AddRectangle(cells, new Vector2Int(-extensionSize.x, y), new Vector2Int(extensionSize.x, bayHeight));
					break;
			}
		}

		private static void AddLShape(HashSet<Vector2Int> cells, Vector2Int size)
		{
			int splitX = Mathf.Max(1, size.x / 2);
			int splitY = Mathf.Max(1, size.y / 2);
			AddRectangle(cells, Vector2Int.zero, new Vector2Int(size.x, splitY));
			AddRectangle(cells, Vector2Int.zero, new Vector2Int(splitX, size.y));
		}

		private static void AddRectangle(HashSet<Vector2Int> cells, Vector2Int origin, Vector2Int size)
		{
			for (int x = 0; x < size.x; x++)
				for (int y = 0; y < size.y; y++)
					cells.Add(origin + new Vector2Int(x, y));
		}

		private static void BuildBoundaries(RoomSectionShape result)
		{
			HashSet<BoundaryKey> boundaryKeys = new();
			Dictionary<Vector2Int, List<BoundaryKey>> vertexEdges = new();
			foreach (Vector2Int cell in result.Cells)
				foreach ((Vector2Int offset, GeneratedWallDirection direction) side in GetSides())
				{
					if (result.Cells.Contains(cell + side.offset)) continue;
					BoundaryKey key = new(cell, side.direction);
					boundaryKeys.Add(key);
					foreach (Vector2Int vertex in GetVertices(cell, side.direction))
					{
						if (!vertexEdges.TryGetValue(vertex, out List<BoundaryKey> edges)) vertexEdges.Add(vertex, edges = new List<BoundaryKey>());
						edges.Add(key);
					}
				}
			Dictionary<Vector2Int, BoundaryVertexFeature> features = GetVertexFeatures(vertexEdges, result.Cells);
			foreach (KeyValuePair<Vector2Int, List<BoundaryKey>> vertex in vertexEdges.OrderBy(item => item.Key.x).ThenBy(item => item.Key.y))
				result.BoundaryVertices.Add(new GeneratedBoundaryVertex(vertex.Key, features.TryGetValue(vertex.Key, out BoundaryVertexFeature feature) ? feature.Topology : GeneratedBoundaryTopology.Straight));
			foreach (BoundaryKey key in boundaryKeys.OrderBy(item => item.Cell.x).ThenBy(item => item.Cell.y).ThenBy(item => item.Direction))
			{
				Vector2Int[] vertices = GetVertices(key.Cell, key.Direction);
				List<(Vector2Int Vertex, BoundaryVertexFeature Feature)> edgeFeatures = new();
				foreach (Vector2Int vertex in vertices) if (features.TryGetValue(vertex, out BoundaryVertexFeature feature)) edgeFeatures.Add((vertex, feature));
				GeneratedBoundaryTopology topology = GeneratedBoundaryTopology.Straight;
				GeneratedBoundaryHandedness handedness = GeneratedBoundaryHandedness.None;
				bool multipleFeatures = edgeFeatures.Count > 1;
				if (multipleFeatures)
				{
					topology = GeneratedBoundaryTopology.Cap;
				}
				else if (edgeFeatures.Count == 1)
				{
					topology = edgeFeatures[0].Feature.Topology;
					handedness = GetHandedness(key.Direction, vertices, edgeFeatures[0].Vertex);
				}
				result.Boundaries.Add(new GeneratedBoundaryEdge(key.Cell, key.Direction, vertices[0], vertices[1], topology, handedness, multipleFeatures));
			}
		}

		private sealed class OrderedBoundaryLoopData
		{
			public readonly List<GeneratedBoundaryEdge> Edges = new();
			public long SignedAreaTwice;
		}

		private static void BuildBoundaryLoops(RoomSectionShape result)
		{
			result.BoundaryLoops.Clear();
			if (result.Boundaries.Count == 0)
			{
				if (result.Cells.Count > 0) result.Diagnostics.Add(new RoomSectionDiagnostic(RoomSectionDiagnosticSeverity.Error, "Cannot build boundary loops because no exposed boundary segments were generated."));
				return;
			}

			Dictionary<Vector2Int, List<int>> segmentsByStart = new();
			for (int i = 0; i < result.Boundaries.Count; i++)
			{
				Vector2Int start = result.Boundaries[i].StartVertex;
				if (!segmentsByStart.TryGetValue(start, out List<int> segments)) segmentsByStart.Add(start, segments = new List<int>());
				segments.Add(i);
			}

			HashSet<int> remaining = Enumerable.Range(0, result.Boundaries.Count).ToHashSet();
			List<OrderedBoundaryLoopData> loops = new();
			while (remaining.Count > 0)
			{
				int firstIndex = remaining.Min();
				if (!TryStitchLoop(result, segmentsByStart, remaining, firstIndex, out OrderedBoundaryLoopData loop))
				{
					result.BoundaryLoops.Clear();
					return;
				}
				loops.Add(loop);
			}

			if (loops.Sum(loop => loop.Edges.Count) != result.Boundaries.Count)
			{
				result.BoundaryLoops.Clear();
				result.Diagnostics.Add(new RoomSectionDiagnostic(RoomSectionDiagnosticSeverity.Error, "Boundary loop extraction lost or duplicated exposed boundary segments."));
				return;
			}

			int exteriorIndex = 0;
			for (int i = 1; i < loops.Count; i++) if (Math.Abs(loops[i].SignedAreaTwice) > Math.Abs(loops[exteriorIndex].SignedAreaTwice)) exteriorIndex = i;
			if (loops[exteriorIndex].SignedAreaTwice > 0) ReverseLoop(loops[exteriorIndex]);
			foreach (OrderedBoundaryLoopData loop in loops) if (!TryCreateBoundaryLoop(result, loop, out GeneratedBoundaryLoop generatedLoop))
			{
				result.BoundaryLoops.Clear();
				return;
			}
			else result.BoundaryLoops.Add(generatedLoop);

			result.BoundaryLoops.Sort(CompareLoops);
		}

		private static bool TryStitchLoop(RoomSectionShape result, Dictionary<Vector2Int, List<int>> segmentsByStart, HashSet<int> remaining, int firstIndex, out OrderedBoundaryLoopData loop)
		{
			loop = new OrderedBoundaryLoopData();
			GeneratedBoundaryEdge first = result.Boundaries[firstIndex];
			Vector2Int startVertex = first.StartVertex;
			Vector2Int currentVertex = startVertex;
			int currentIndex = firstIndex;
			HashSet<Vector2Int> visitedVertices = new() { startVertex };
			while (true)
			{
				if (!remaining.Remove(currentIndex))
				{
					AddLoopError(result, "Boundary loop extraction revisited an exposed boundary segment.", currentVertex);
					return false;
				}
				GeneratedBoundaryEdge edge = result.Boundaries[currentIndex];
				if (edge.StartVertex != currentVertex)
				{
					AddLoopError(result, "Boundary segments do not connect in order.", currentVertex);
					return false;
				}
				loop.Edges.Add(edge);
				currentVertex = edge.EndVertex;
				if (currentVertex == startVertex) break;
				if (!visitedVertices.Add(currentVertex))
				{
					AddLoopError(result, "Boundary segments form a self-intersecting or partial loop.", currentVertex);
					return false;
				}
				if (!segmentsByStart.TryGetValue(currentVertex, out List<int> candidates))
				{
					AddLoopError(result, "Boundary segments cannot form a closed loop.", currentVertex);
					return false;
				}
				List<int> available = candidates.Where(remaining.Contains).ToList();
				if (available.Count != 1)
				{
					AddLoopError(result, available.Count == 0 ? "Boundary segments cannot form a closed loop." : "Boundary vertex has multiple possible following boundary segments.", currentVertex);
					return false;
				}
				currentIndex = available[0];
			}

			loop.SignedAreaTwice = GetSignedAreaTwice(loop.Edges);
			CanonicalizeLoop(loop);
			return loop.Edges.Count > 0;
		}

		private static void ReverseLoop(OrderedBoundaryLoopData loop)
		{
			List<GeneratedBoundaryEdge> reversed = new();
			for (int i = loop.Edges.Count - 1; i >= 0; i--)
			{
				GeneratedBoundaryEdge edge = loop.Edges[i];
				reversed.Add(new GeneratedBoundaryEdge(edge.Cell, edge.Direction, edge.EndVertex, edge.StartVertex, edge.Topology, edge.Handedness, edge.HasMultipleFeatures));
			}
			loop.Edges.Clear();
			loop.Edges.AddRange(reversed);
			loop.SignedAreaTwice = -loop.SignedAreaTwice;
			CanonicalizeLoop(loop);
		}

		private static void CanonicalizeLoop(OrderedBoundaryLoopData loop)
		{
			int startIndex = -1;
			Vector2Int startVertex = default;
			for (int i = 0; i < loop.Edges.Count; i++)
			{
				GeneratedBoundaryEdge current = loop.Edges[i];
				GeneratedBoundaryEdge next = loop.Edges[(i + 1) % loop.Edges.Count];
				if (current.Direction == next.Direction) continue;
				Vector2Int candidate = current.EndVertex;
				if (startIndex < 0 || candidate.x < startVertex.x || candidate.x == startVertex.x && candidate.y < startVertex.y)
				{
					startIndex = (i + 1) % loop.Edges.Count;
					startVertex = candidate;
				}
			}
			if (startIndex <= 0) return;
			List<GeneratedBoundaryEdge> rotated = new();
			for (int i = 0; i < loop.Edges.Count; i++) rotated.Add(loop.Edges[(startIndex + i) % loop.Edges.Count]);
			loop.Edges.Clear();
			loop.Edges.AddRange(rotated);
		}

		private static bool TryCreateBoundaryLoop(RoomSectionShape result, OrderedBoundaryLoopData data, out GeneratedBoundaryLoop loop)
		{
			loop = new GeneratedBoundaryLoop();
			if (data.Edges.Count == 0)
			{
				result.Diagnostics.Add(new RoomSectionDiagnostic(RoomSectionDiagnosticSeverity.Error, "Boundary loop contains no segments."));
				return false;
			}

			int runStart = 0;
			for (int i = 1; i <= data.Edges.Count; i++)
			{
				if (i < data.Edges.Count && data.Edges[i].Direction == data.Edges[runStart].Direction) continue;
				GeneratedBoundaryEdge first = data.Edges[runStart];
				GeneratedBoundaryEdge last = data.Edges[i - 1];
				loop.WallRuns.Add(new GeneratedWallRun(first.Direction, first.StartVertex, last.EndVertex, i - runStart));
				runStart = i;
			}

			if (loop.WallRuns.Count == 0)
			{
				result.Diagnostics.Add(new RoomSectionDiagnostic(RoomSectionDiagnosticSeverity.Error, "Boundary loop contains no wall runs."));
				return false;
			}
			for (int i = 0; i < loop.WallRuns.Count; i++)
			{
				GeneratedWallRun incoming = loop.WallRuns[i];
				GeneratedWallRun outgoing = loop.WallRuns[(i + 1) % loop.WallRuns.Count];
				if (incoming.EndVertex != outgoing.StartVertex || incoming.Length <= 0)
				{
					AddLoopError(result, "Boundary wall runs are not connected or have a non-positive length.", incoming.EndVertex);
					return false;
				}
				if (incoming.Direction == outgoing.Direction)
				{
					AddLoopError(result, "Boundary wall runs were not merged at a collinear transition.", incoming.EndVertex);
					return false;
				}
				if (!TryClassifyCorner(result, incoming.EndVertex, incoming.Direction, outgoing.Direction, out GeneratedBoundaryCorner corner)) return false;
				loop.Corners.Add(corner);
			}
			foreach (GeneratedWallRun run in loop.WallRuns) loop.Vertices.Add(run.StartVertex);
			loop.Vertices.Add(loop.WallRuns[0].StartVertex);
			if (!loop.IsClosed)
			{
				result.Diagnostics.Add(new RoomSectionDiagnostic(RoomSectionDiagnosticSeverity.Error, "Boundary loop is not closed."));
				return false;
			}

			if (loop.WallRuns.Sum(run => run.Length) != data.Edges.Count || loop.Corners.Count != loop.WallRuns.Count)
			{
				result.Diagnostics.Add(new RoomSectionDiagnostic(RoomSectionDiagnosticSeverity.Error, "Boundary loop wall runs do not cover each exposed boundary segment exactly once."));
				return false;
			}
			long signedAreaTwice = GetSignedAreaTwice(data.Edges);
			if (signedAreaTwice == 0)
			{
				result.Diagnostics.Add(new RoomSectionDiagnostic(RoomSectionDiagnosticSeverity.Error, "Boundary loop has zero signed area."));
				return false;
			}
			loop.IsClockwise = signedAreaTwice < 0;
			return true;
		}

		private static bool TryClassifyCorner(RoomSectionShape result, Vector2Int position, GeneratedWallDirection incoming, GeneratedWallDirection outgoing, out GeneratedBoundaryCorner corner)
		{
			int occupiedCount = 0;
			bool northWest = result.Cells.Contains(position + new Vector2Int(-1, 0));
			bool northEast = result.Cells.Contains(position);
			bool southWest = result.Cells.Contains(position + new Vector2Int(-1, -1));
			bool southEast = result.Cells.Contains(position + Vector2Int.down);
			if (northWest) occupiedCount++;
			if (northEast) occupiedCount++;
			if (southWest) occupiedCount++;
			if (southEast) occupiedCount++;
			GeneratedBoundaryCornerKind kind;
			GeneratedBoundaryCornerOrientation orientation;
			if (occupiedCount == 1)
			{
				kind = GeneratedBoundaryCornerKind.Outer;
				if (southEast) orientation = GeneratedBoundaryCornerOrientation.NorthWest;
				else if (southWest) orientation = GeneratedBoundaryCornerOrientation.NorthEast;
				else if (northWest) orientation = GeneratedBoundaryCornerOrientation.SouthEast;
				else orientation = GeneratedBoundaryCornerOrientation.SouthWest;
			}
			else if (occupiedCount == 3)
			{
				kind = GeneratedBoundaryCornerKind.Inner;
				if (!northWest) orientation = GeneratedBoundaryCornerOrientation.NorthWest;
				else if (!northEast) orientation = GeneratedBoundaryCornerOrientation.NorthEast;
				else if (!southEast) orientation = GeneratedBoundaryCornerOrientation.SouthEast;
				else orientation = GeneratedBoundaryCornerOrientation.SouthWest;
			}
			else
			{
				corner = null;
				AddLoopError(result, "Boundary turn does not have a valid outer or inner corner quadrant.", position);
				return false;
			}
			corner = new GeneratedBoundaryCorner(position, kind, orientation, incoming, outgoing);
			return true;
		}

		private static long GetSignedAreaTwice(IEnumerable<GeneratedBoundaryEdge> edges)
		{
			long area = 0;
			foreach (GeneratedBoundaryEdge edge in edges) area += (long)edge.StartVertex.x * edge.EndVertex.y - (long)edge.EndVertex.x * edge.StartVertex.y;
			return area;
		}

		private static int CompareLoops(GeneratedBoundaryLoop first, GeneratedBoundaryLoop second)
		{
			int firstArea = first.WallRuns.Sum(run => run.Length);
			int secondArea = second.WallRuns.Sum(run => run.Length);
			int comparison = secondArea.CompareTo(firstArea);
			if (comparison != 0) return comparison;
			Vector2Int firstVertex = first.Vertices[0];
			Vector2Int secondVertex = second.Vertices[0];
			comparison = firstVertex.x.CompareTo(secondVertex.x);
			return comparison != 0 ? comparison : firstVertex.y.CompareTo(secondVertex.y);
		}

		private static void AddLoopError(RoomSectionShape result, string message, Vector2Int vertex)
		{
			result.Diagnostics.Add(new RoomSectionDiagnostic(RoomSectionDiagnosticSeverity.Error, message, vertex));
		}

		private static Dictionary<Vector2Int, BoundaryVertexFeature> GetVertexFeatures(Dictionary<Vector2Int, List<BoundaryKey>> vertexEdges, HashSet<Vector2Int> cells)
		{
			Dictionary<Vector2Int, BoundaryVertexFeature> result = new();
			foreach (KeyValuePair<Vector2Int, List<BoundaryKey>> vertex in vertexEdges)
			{
				List<GeneratedWallDirection> directions = vertex.Value.Select(edge => edge.Direction).Distinct().ToList();
				if (directions.Count != 2 || !IsCornerPair(directions[0], directions[1])) continue;
				GeneratedBoundaryTopology topology = GetCornerTopology(vertex.Key, cells);
				if (topology == GeneratedBoundaryTopology.Straight) continue;
				result[vertex.Key] = new BoundaryVertexFeature
				{
					Topology = topology
				};
			}
			return result;
		}

		private static void Validate(RoomSectionShape result)
		{
			if (result.Cells.Count == 0) result.Diagnostics.Add(new RoomSectionDiagnostic(RoomSectionDiagnosticSeverity.Error, "Generated shape is empty."));
			if (result.Boundaries.Count == 0) result.Diagnostics.Add(new RoomSectionDiagnostic(RoomSectionDiagnosticSeverity.Error, "Generated shape has no wall boundary."));
			if (result.Cells.Count == 0) return;
			HashSet<Vector2Int> visited = FloodFill(result.Cells.First(), result.Cells);
			if (visited.Count != result.Cells.Count) result.Diagnostics.Add(new RoomSectionDiagnostic(RoomSectionDiagnosticSeverity.Error, "Generated shape is disconnected."));
			if (HasHole(result.Cells)) result.Diagnostics.Add(new RoomSectionDiagnostic(RoomSectionDiagnosticSeverity.Error, "Generated shape contains a hole."));
			if (HasDiagonalOnlyContact(result.Cells)) result.Diagnostics.Add(new RoomSectionDiagnostic(RoomSectionDiagnosticSeverity.Error, "Generated shape contains diagonal-only contact."));
		}

		private static HashSet<Vector2Int> FloodFill(Vector2Int start, HashSet<Vector2Int> cells = null)
		{
			cells ??= new HashSet<Vector2Int>();
			HashSet<Vector2Int> visited = new();
			Queue<Vector2Int> queue = new();
			queue.Enqueue(start);
			while (queue.Count > 0)
			{
				Vector2Int cell = queue.Dequeue();
				if (!visited.Add(cell)) continue;
				foreach (Vector2Int offset in GetOffsets()) if (cells.Contains(cell + offset)) queue.Enqueue(cell + offset);
			}
			return visited;
		}

		private static bool HasHole(HashSet<Vector2Int> cells)
		{
			int minX = cells.Min(cell => cell.x) - 1;
			int maxX = cells.Max(cell => cell.x) + 1;
			int minY = cells.Min(cell => cell.y) - 1;
			int maxY = cells.Max(cell => cell.y) + 1;
			HashSet<Vector2Int> exterior = new();
			Queue<Vector2Int> queue = new();
			queue.Enqueue(new Vector2Int(minX, minY));
			while (queue.Count > 0)
			{
				Vector2Int cell = queue.Dequeue();
				if (!exterior.Add(cell) || cells.Contains(cell) || cell.x < minX || cell.x > maxX || cell.y < minY || cell.y > maxY) continue;
				foreach (Vector2Int offset in GetOffsets()) queue.Enqueue(cell + offset);
			}
			for (int x = minX + 1; x < maxX; x++)
				for (int y = minY + 1; y < maxY; y++)
					if (!cells.Contains(new Vector2Int(x, y)) && !exterior.Contains(new Vector2Int(x, y))) return true;
			return false;
		}

		private static bool HasDiagonalOnlyContact(HashSet<Vector2Int> cells)
		{
			foreach (Vector2Int cell in cells)
				foreach (Vector2Int diagonal in new[] { new Vector2Int(1, 1), new Vector2Int(1, -1) })
				{
					Vector2Int other = cell + diagonal;
					if (!cells.Contains(other)) continue;
					if (!cells.Contains(cell + new Vector2Int(diagonal.x, 0)) && !cells.Contains(cell + new Vector2Int(0, diagonal.y))) return true;
				}
			return false;
		}

		private static IEnumerable<Vector2Int> GetOffsets()
		{
			yield return Vector2Int.up;
			yield return Vector2Int.right;
			yield return Vector2Int.down;
			yield return Vector2Int.left;
		}

		private static IEnumerable<(Vector2Int Offset, GeneratedWallDirection Direction)> GetSides()
		{
			yield return (Vector2Int.up, GeneratedWallDirection.North);
			yield return (Vector2Int.right, GeneratedWallDirection.East);
			yield return (Vector2Int.down, GeneratedWallDirection.South);
			yield return (Vector2Int.left, GeneratedWallDirection.West);
		}

		private static Vector2Int[] GetVertices(Vector2Int cell, GeneratedWallDirection direction)
		{
			return direction switch
			{
				GeneratedWallDirection.North => new[] { cell + Vector2Int.up, cell + Vector2Int.up + Vector2Int.right },
				GeneratedWallDirection.East => new[] { cell + Vector2Int.up + Vector2Int.right, cell + Vector2Int.right },
				GeneratedWallDirection.South => new[] { cell + Vector2Int.right, cell },
				_ => new[] { cell, cell + Vector2Int.up }
			};
		}

		private static GeneratedBoundaryHandedness GetHandedness(GeneratedWallDirection direction, Vector2Int[] vertices, Vector2Int featureVertex)
		{
			bool horizontal = direction == GeneratedWallDirection.North || direction == GeneratedWallDirection.South;
			int minimum = horizontal ? Mathf.Min(vertices[0].x, vertices[1].x) : Mathf.Min(vertices[0].y, vertices[1].y);
			bool left = horizontal ? featureVertex.x == minimum : featureVertex.y == minimum;
			return left ? GeneratedBoundaryHandedness.Left : GeneratedBoundaryHandedness.Right;
		}

		private static GeneratedBoundaryTopology GetCornerTopology(Vector2Int vertex, HashSet<Vector2Int> cells)
		{
			int occupied = 0;
			foreach (Vector2Int cell in new[] { vertex + new Vector2Int(-1, -1), vertex + Vector2Int.down, vertex + Vector2Int.left, vertex }) if (cells.Contains(cell)) occupied++;
			if (occupied == 1) return GeneratedBoundaryTopology.OuterTurn;
			if (occupied == 3) return GeneratedBoundaryTopology.InnerTurn;
			return GeneratedBoundaryTopology.Straight;
		}

		private static bool IsCornerPair(GeneratedWallDirection first, GeneratedWallDirection second)
		{
			return first != second && ((int)first + (int)second) % 2 == 1;
		}

	}
}
