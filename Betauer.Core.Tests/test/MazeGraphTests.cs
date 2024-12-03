using System;
using System.Linq;
using Betauer.Core.PCG.Maze;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
[Only]
public class MazeGraphTests {
    private MazeGraph _graph = null!;

    [SetUp]
    public void Setup() {
        _graph = new MazeGraph(10, 10);
    }

    [Test]
    public void Constructor_InitializesCorrectly() {
        Assert.Multiple(() => {
            Assert.That(_graph.Width, Is.EqualTo(10));
            Assert.That(_graph.Height, Is.EqualTo(10));
            Assert.That(_graph.NodeGrid, Is.Not.Null);
            Assert.That(_graph.Nodes, Is.Empty);
            Assert.That(_graph.Root, Is.Null);
        });
    }

    [Test]
    public void GetOrCreateNode_CreatesNewNode() {
        var node = _graph.GetOrCreateNode(new Vector2I(1, 1));
        
        Assert.Multiple(() => {
            Assert.That(node, Is.Not.Null);
            Assert.That(_graph.NodeGrid[new Vector2I(1, 1)], Is.EqualTo(node));
            Assert.That(_graph.Nodes.ContainsKey(node.Id), Is.True);
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
            Assert.That(_graph.NodeGrid[node.Position], Is.EqualTo(node));
            Assert.That(_graph.Nodes[node.Id], Is.EqualTo(node));
        });
    }

    [Test]
    public void CreateNode_WithExistingPosition_ThrowsArgumentException() {
        _graph.CreateNode(new Vector2I(1, 1));
        
        Assert.Throws<ArgumentException>(() => 
            _graph.CreateNode(new Vector2I(1, 1)));
    }

    [Test]
    public void RemoveNode_RemovesNodeAndConnections() {
        var node = _graph.CreateNode(new Vector2I(1, 1));
        var other = _graph.CreateNode(new Vector2I(1, 0));
        _graph.ConnectNode(node, other, true);
        other.Parent = node;

        _graph.RemoveNode(node);

        Assert.Multiple(() => {
            Assert.That(_graph.NodeGrid[node.Position], Is.Null);
            Assert.That(_graph.Nodes.ContainsKey(node.Id), Is.False);
            Assert.That(other.Parent, Is.Null);
        });
    }

    [Test]
    public void ConnectNode_CreatesBidirectionalConnection() {
        var node1 = _graph.CreateNode(new Vector2I(0, 0));
        var node2 = _graph.CreateNode(new Vector2I(1, 0));
        
        _graph.ConnectNode(node1, node2, true);

        Assert.Multiple(() => {
            Assert.That(node1.HasEdgeTo(node2), Is.True);
            Assert.That(node2.HasEdgeTo(node1), Is.True);
        });
    }

    [Test]
    public void GetValidFreeAdjacentPositions_ReturnsCorrectPositions() {
        var node = _graph.CreateNode(new Vector2I(1, 1));
        var positions = _graph.GetValidFreeAdjacentPositions(node.Position).ToList();

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
            {true, false, true},
            {true, true, true},
            {false, true, false}
        };

        var graph = MazeGraph.Create(template);
        
        Assert.Multiple(() => {
            Assert.That(graph.Width, Is.EqualTo(3));
            Assert.That(graph.Height, Is.EqualTo(3));
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
            Assert.That(_graph.Root, Is.Not.Null);
            Assert.That(_graph.Nodes, Is.Not.Empty);
            Assert.That(_graph.Nodes.Count, Is.LessThanOrEqualTo(10));
        });
    }

    [Test]
    public void GrowRandom_CreatesValidMaze() {
        _graph.GrowRandom(new Vector2I(0, 0), 10);

        Assert.Multiple(() => {
            Assert.That(_graph.Root, Is.Not.Null);
            Assert.That(_graph.Nodes, Is.Not.Empty);
            Assert.That(_graph.Nodes.Count, Is.LessThanOrEqualTo(10));
        });
    }
}