using System;
using System.Linq;
using Betauer.Core.PCG.Maze;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class NodeGridTests {
    private MazeGraph _graph = null!;
    private NodeGrid _nodeGrid = null!;

    [SetUp]
    public void Setup() {
        _graph = new MazeGraph(10, 10);
        _nodeGrid = _graph.CreateNode(new Vector2I(1, 1));
    }

    [Test]
    public void Constructor_SetsProperties() {
        Assert.Multiple(() => {
            Assert.That(_nodeGrid.Id, Is.EqualTo(0));
            Assert.That(_nodeGrid.Position, Is.EqualTo(new Vector2I(1, 1)));
            Assert.That(_nodeGrid.MazeGraph, Is.EqualTo(_graph));
            Assert.That(_nodeGrid.Parent, Is.Null);
        });
    }

    [Test]
    public void Connect_CreatesEdge() {
        var other = _graph.CreateNode(new Vector2I(1, 0));
        var edge = _nodeGrid.SetEdge(Vector2I.Up, other);

        Assert.Multiple(() => {
            Assert.That(edge.From, Is.EqualTo(_nodeGrid));
            Assert.That(edge.To, Is.EqualTo(other));
            Assert.That(edge.Direction, Is.EqualTo(Vector2I.Up));
            Assert.That(_nodeGrid.Up, Is.EqualTo(edge));
        });
    }

    [Test]
    public void Connect_WithNullNode_ThrowsArgumentNullException() {
        Assert.Throws<ArgumentNullException>(() =>
            _nodeGrid.SetEdge(Vector2I.Up, null!));
    }

    [Test]
    public void RemoveEdge_RemovesConnection() {
        var other = _graph.CreateNode(new Vector2I(1, 0));
        _nodeGrid.SetEdge(Vector2I.Up, other);

        _nodeGrid.RemoveEdge(Vector2I.Up);

        Assert.That(_nodeGrid.Up, Is.Null);
    }

    [Test]
    public void HasEdge_ReturnsCorrectValue() {
        var other = _graph.CreateNode(new Vector2I(1, 0));
        _nodeGrid.SetEdge(Vector2I.Up, other);

        Assert.Multiple(() => {
            Assert.That(_nodeGrid.HasEdge(Vector2I.Up), Is.True);
            Assert.That(_nodeGrid.HasEdge(Vector2I.Down), Is.False);
        });
    }

    [Test]
    public void GetEdge_ReturnsCorrectEdge() {
        var other = _graph.CreateNode(new Vector2I(1, 0));
        var edge = _nodeGrid.SetEdge(Vector2I.Up, other);

        Assert.That(_nodeGrid.GetEdge(Vector2I.Up), Is.EqualTo(edge));
    }

    [Test]
    public void GetEdgeTo_ReturnsCorrectEdge() {
        var other = _graph.CreateNode(new Vector2I(1, 0));
        var edge = _nodeGrid.SetEdge(Vector2I.Up, other);

        Assert.That(_nodeGrid.GetEdgeTo(other), Is.EqualTo(edge));
    }

    [Test]
    public void GetChildren_ReturnsCorrectNodes() {
        var child1 = _graph.CreateNode(new Vector2I(1, 0));
        var child2 = _graph.CreateNode(new Vector2I(2, 1));
        var nonChild = _graph.CreateNode(new Vector2I(0, 1));
        
        child1.Parent = _nodeGrid;
        child2.Parent = _nodeGrid;

        var children = _nodeGrid.GetChildren().ToList();

        Assert.Multiple(() => {
            Assert.That(children, Has.Count.EqualTo(2));
            Assert.That(children, Contains.Item(child1));
            Assert.That(children, Contains.Item(child2));
            Assert.That(children, Does.Not.Contain(nonChild));
        });
    }

    [Test]
    public void GetEdges_ReturnsAllConnectedEdges() {
        var up = _graph.CreateNode(new Vector2I(1, 0));
        var right = _graph.CreateNode(new Vector2I(2, 1));

        var edgeUp = _nodeGrid.SetEdge(Vector2I.Up, up);
        var edgeRight = _nodeGrid.SetEdge(Vector2I.Right, right);

        var edges = _nodeGrid.GetEdges().ToList();

        Assert.Multiple(() => {
            Assert.That(edges, Has.Count.EqualTo(2));
            Assert.That(edges, Contains.Item(edgeUp));
            Assert.That(edges, Contains.Item(edgeRight));
        });
    }

    [Test]
    public void GetDirectionToParent_ReturnsCorrectDirection() {
        var parent = _graph.CreateNode(new Vector2I(1, 0));
        _nodeGrid.Parent = parent;

        var direction = _nodeGrid.GetDirectionToParent();

        Assert.That(direction, Is.EqualTo(Vector2I.Down));
    }

    [Test]
    public void GetDirectionToParent_WithNoParent_ReturnsNull() {
        Assert.That(_nodeGrid.GetDirectionToParent(), Is.Null);
    }

    [Test]
    public void GetPathToRoot_ReturnsCorrectPath() {
        var root = _graph.CreateNode(new Vector2I(0, 0));
        var middle = _graph.CreateNode(new Vector2I(1, 0));
        var leaf = _graph.CreateNode(new Vector2I(2, 0));
        
        middle.Parent = root;
        leaf.Parent = middle;

        var path = leaf.GetPathToRoot();

        Assert.Multiple(() => {
            Assert.That(path, Has.Count.EqualTo(3));
            Assert.That(path[0], Is.EqualTo(leaf));
            Assert.That(path[1], Is.EqualTo(middle));
            Assert.That(path[2], Is.EqualTo(root));
        });
    }

    [Test]
    public void Remove_CleansUpNodeAndConnections() {
        var up = _graph.CreateNode(new Vector2I(1, 0));
        var right = _graph.CreateNode(new Vector2I(2, 1));
        var down = _graph.CreateNode(new Vector2I(1, 2));

        _nodeGrid.SetEdge(Vector2I.Up, up);
        _nodeGrid.SetEdge(Vector2I.Right, right);
        down.SetEdge(Vector2I.Up, _nodeGrid);

        down.Parent = _nodeGrid;

        Assert.That(down.Up.To, Is.EqualTo(_nodeGrid));
        Assert.That(_nodeGrid.Right.To, Is.EqualTo(right));
        Assert.That(_nodeGrid.Up.To, Is.EqualTo(up));

        _nodeGrid.Remove();

        Assert.That(_graph.Nodes.ContainsKey(0), Is.False);
        Assert.That(_graph.NodeGrid[_nodeGrid.Position], Is.Null);
        Assert.That(_nodeGrid.Up, Is.Null);
        Assert.That(_nodeGrid.Right, Is.Null);
        Assert.That(_nodeGrid.Down, Is.Null);
        Assert.That(_nodeGrid.Left, Is.Null);
        Assert.That(_nodeGrid.Parent, Is.Null);
        Assert.That(up.Down, Is.Null);
        Assert.That(right.Left, Is.Null);
        Assert.That(down.Up, Is.Null);
        Assert.That(down.Parent, Is.Null);
    }
    
    [Test]
    public void GetNeighbors_CenterNode_ReturnsFourNeighbors() {
        // Create center node at (2,2)
        var center = _graph.CreateNode(new Vector2I(2, 2));
    
        // Create all surrounding nodes
        var up = _graph.CreateNode(new Vector2I(2, 1));
        var right = _graph.CreateNode(new Vector2I(3, 2));
        var down = _graph.CreateNode(new Vector2I(2, 3));
        var left = _graph.CreateNode(new Vector2I(1, 2));
    
        var neighbors = center.GetNeighbors().ToList();
    
        Assert.Multiple(() => {
            Assert.That(neighbors, Has.Count.EqualTo(4));
            Assert.That(neighbors, Contains.Item(up));
            Assert.That(neighbors, Contains.Item(right));
            Assert.That(neighbors, Contains.Item(down));
            Assert.That(neighbors, Contains.Item(left));
        });
    }
}