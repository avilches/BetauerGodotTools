using System;
using System.Linq;
using Betauer.Core.PCG.Maze;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class MazeGraphTests {
    private MazeGraph _graph = null!;

    [SetUp]
    public void Setup() {
        _graph = MazeGraph.Create(10, 10);
    }

    [Test]
    public void Constructor_InitializesCorrectly() {
        Assert.Multiple(() => { Assert.That(_graph.GetNodes(), Is.Empty); });
    }

    [Test]
    public void GetOrCreateNode_CreatesNewNode() {
        var node = _graph.GetOrCreateNode(new Vector2I(1, 1));

        Assert.Multiple(() => {
            Assert.That(node, Is.Not.Null);
            Assert.That(_graph.GetNodeAt(new Vector2I(1, 1)), Is.EqualTo(node));
            Assert.That(_graph.HasNodeAt(new Vector2I(1, 1)), Is.True);
            Assert.That(_graph.HasNode(node.Id), Is.True);
        });
    }

    [Test]
    public void GetOrCreateNode_ReturnsExistingNode() {
        var node1 = _graph.GetOrCreateNode(new Vector2I(1, 1));
        var node2 = _graph.GetOrCreateNode(new Vector2I(1, 1));

        Assert.That(node1, Is.EqualTo(node2));
    }

    [Test]
    public void CreateNode_CreatesNewNode() {
        var node = _graph.CreateNode(new Vector2I(1, 1));

        Assert.Multiple(() => {
            Assert.That(node.Position, Is.EqualTo(new Vector2I(1, 1)));
            Assert.That(_graph.GetNodeAt(node.Position), Is.EqualTo(node));
            Assert.That(_graph.GetNode(node.Id), Is.EqualTo(node));
        });
    }

    [Test]
    public void CreateNode_WithExistingPosition_ThrowsArgumentException() {
        _graph.CreateNode(new Vector2I(1, 1));

        Assert.Throws<InvalidOperationException>(() =>
            _graph.CreateNode(new Vector2I(1, 1)));
    }

    [Test]
    public void CreateNode_WithCustomId_CreatesNodeWithSpecifiedId() {
        var node = _graph.CreateNode(new Vector2I(1, 1), id: 42);

        Assert.Multiple(() => {
            Assert.That(node.Id, Is.EqualTo(42));
            Assert.That(_graph.GetNode(42), Is.EqualTo(node));
            Assert.That(_graph.HasNode(42), Is.True);
        });
    }

    [Test]
    public void CreateNode_WithDuplicateId_ThrowsInvalidOperationException() {
        _graph.CreateNode(new Vector2I(1, 1), id: 42);

        Assert.Throws<InvalidOperationException>(() =>
            _graph.CreateNode(new Vector2I(2, 2), id: 42));
    }

    [Test]
    public void CreateNode_WithCustomId_UpdatesLastId() {
        _graph.CreateNode(new Vector2I(1, 1), id: 42);
        var nextNode = _graph.CreateNode(new Vector2I(2, 2)); // Auto-generated ID

        Assert.That(nextNode.Id, Is.EqualTo(43));
    }

    [Test]
    public void CreateNode_MixingCustomAndAutoIds_MaintainsUniqueSequence() {
        var node1 = _graph.CreateNode(new Vector2I(0, 0)); // Auto ID: 0
        var node2 = _graph.CreateNode(new Vector2I(1, 0), id: 5);
        var node3 = _graph.CreateNode(new Vector2I(2, 0)); // Should get ID: 6

        Assert.Multiple(() => {
            Assert.That(node1.Id, Is.EqualTo(0));
            Assert.That(node2.Id, Is.EqualTo(5));
            Assert.That(node3.Id, Is.EqualTo(6));
            Assert.That(_graph.GetNodes().Count, Is.EqualTo(3));
        });
    }
    
    [Test]
    public void CreateNode_WithCustomIdLessThanLastId_Works() {
        var node1 = _graph.CreateNode(new Vector2I(0, 0)); // Auto ID: 0, LastId = 1
        var node2 = _graph.CreateNode(new Vector2I(1, 0)); // Auto ID: 1, LastId = 2
        node2.RemoveNode();
        var node3 = _graph.CreateNode(new Vector2I(2, 0)); // Auto ID: 2, LastId = 3
        var node4 = _graph.CreateNode(new Vector2I(1, 0), id: 1); // Should work with old id
        Assert.Multiple(() => {
            Assert.That(node1.Id, Is.EqualTo(0));
            Assert.That(node2.Id, Is.EqualTo(1));
            Assert.That(node3.Id, Is.EqualTo(2));
            Assert.That(node4.Id, Is.EqualTo(1));
        });
    }

    [Test]
    public void RemoveNode_RemovesNodeAndConnections() {
        var node = _graph.CreateNode(new Vector2I(1, 1));
        var other = _graph.CreateNode(new Vector2I(1, 0));
        _graph.ConnectNodes(node, other);
        _graph.ConnectNodes(other, node);
        other.Parent = node;

        node.RemoveNode();

        Assert.Multiple(() => {
            Assert.That(_graph.GetNodeAtOrNull(node.Position), Is.Null);
            Assert.That(_graph.HasNodeAt(node.Position), Is.False);
            Assert.That(_graph.HasNode(node.Id), Is.False);
            Assert.That(other.Parent, Is.Null);
        });
    }

    [Test]
    public void ConnectNode_CreatesBidirectionalConnection() {
        var node1 = _graph.CreateNode(new Vector2I(0, 0));
        var node2 = _graph.CreateNode(new Vector2I(1, 0));

        _graph.ConnectNodes(node1, node2);
        _graph.ConnectNodes(node2, node1);

        Assert.Multiple(() => {
            Assert.That(node1.HasEdgeTo(node2), Is.True);
            Assert.That(node2.HasEdgeTo(node1), Is.True);
        });
    }

    [Test]
    public void GetValidFreeAdjacentPositions_ReturnsCorrectPositions() {
        var node = _graph.CreateNode(new Vector2I(1, 1));
        var positions = _graph.GetAvailableAdjacentPositions(node.Position).ToList();

        Assert.Multiple(() => {
            Assert.That(positions, Has.Count.EqualTo(4));
            Assert.That(positions, Contains.Item(new Vector2I(1, 0)));
            Assert.That(positions, Contains.Item(new Vector2I(2, 1)));
            Assert.That(positions, Contains.Item(new Vector2I(1, 2)));
            Assert.That(positions, Contains.Item(new Vector2I(0, 1)));
        });
    }

    [Test]
    public void Create_FromBooleanTemplate_CreatesValidGraph() {
        var template = new bool[3, 3] {
            { true, false, true },
            { true, true, true },
            { false, true, false }
        };

        var graph = MazeGraph.Create(template);

        Assert.Multiple(() => {
            Assert.That(graph.IsValidPosition(new Vector2I(0, 0)), Is.True);
            Assert.That(graph.IsValidPosition(new Vector2I(1, 0)), Is.False);
            Assert.That(graph.IsValidPosition(new Vector2I(2, 2)), Is.False);
        });
    }

    [Test]
    public void Grow_CreatesValidMaze() {
        var constraints = new BacktrackConstraints {
            MaxTotalCells = 10,
            DirectionSelector = (current, available) => available[0]
        };

        _graph.Grow(new Vector2I(0, 0), constraints);

        Assert.Multiple(() => {
            Assert.That(_graph.GetRoots().Count(), Is.EqualTo(1));
            Assert.That(_graph.GetNodes(), Is.Not.Empty);
            Assert.That(_graph.GetNodes().Count, Is.LessThanOrEqualTo(10));
        });
    }

    [Test]
    public void GrowRandom_CreatesValidMaze() {
        _graph.GrowRandom(new Vector2I(0, 0), 10);

        Assert.Multiple(() => {
            Assert.That(_graph.GetRoots().Count(), Is.EqualTo(1));
            Assert.That(_graph.GetNodes(), Is.Not.Empty);
            Assert.That(_graph.GetNodes().Count, Is.LessThanOrEqualTo(10));
        });
    }

    [Test]
    public void ParseAndExportSimpleMaze() {
        var input = """
                    #-#
                    | |
                    #-#
                    """;
        var maze = MazeGraph.Parse(input);

        // Verify nodes
        Assert.That(maze.GetNodes().Count, Is.EqualTo(4));

        // Verify connections
        var topLeft = maze.GetNodeAt(new Vector2I(0, 0));
        var topRight = maze.GetNodeAt(new Vector2I(1, 0));
        var bottomLeft = maze.GetNodeAt(new Vector2I(0, 1));
        var bottomRight = maze.GetNodeAt(new Vector2I(1, 1));

        Assert.That(topLeft.Right, Is.EqualTo(topRight));
        Assert.That(topLeft.Down, Is.EqualTo(bottomLeft));
        Assert.That(topRight.Down, Is.EqualTo(bottomRight));
        Assert.That(bottomLeft.Right, Is.EqualTo(bottomRight));

        // Verify export matches input
        var output = maze.Export();
        Assert.That(NormalizeMazeString(output), Is.EqualTo(NormalizeMazeString(input)));
    }

    [Test]
    public void ParseAndExportComplexMaze() {
        var input =
            """
                                   #
                                   |
                               #-#-#   #
                               | |     |
                               #-#-#-#-#
                                 |   | |
                               #-#   # #
            """;
        var maze = MazeGraph.Parse(input);

        // Verify node count
        Assert.That(maze.GetNodes().Count, Is.EqualTo(14));

        // Verify export matches input
        var output = maze.Export();
        Console.WriteLine(input);
        Console.WriteLine(NormalizeMazeString(input));
        Console.WriteLine(output);
        Console.WriteLine(NormalizeMazeString(output));
        Assert.That(NormalizeMazeString(output), Is.EqualTo(NormalizeMazeString(input)));
    }

    [Test]
    public void ParseEmptyMaze() {
        var maze = MazeGraph.Parse("   \n ");
        Assert.That(maze.GetNodes().Count, Is.EqualTo(0));
        Assert.That(maze.Export(), Is.EqualTo(""));
        var maze2 = MazeGraph.Parse(null);
        Assert.That(maze2.GetNodes().Count, Is.EqualTo(0));
        Assert.That(maze2.Export(), Is.EqualTo(""));
    }

    private static string NormalizeMazeString(string input) {
        var lines = input.Split('\n');

        // Remove empty lines at start and end
        while (lines.Length > 0 && string.IsNullOrWhiteSpace(lines[0])) lines = lines.Skip(1).ToArray();
        while (lines.Length > 0 && string.IsNullOrWhiteSpace(lines[^1])) lines = lines.SkipLast(1).ToArray();

        // Remove minimum common left padding
        var minPadding = lines.Where(l => !string.IsNullOrWhiteSpace(l))
            .Min(l => l.TakeWhile(c => c == ' ').Count());
        return string.Join('\n', lines.Select(line => (line.Length >= minPadding ? line[minPadding..] : line).TrimEnd()));
    }
}