using System;
using System.Linq;
using Betauer.Core.PCG.Maze;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class MazeNodeEdgeTests {
    private MazeGraph _graph = null!;
    private MazeNode _nodeFrom = null!;
    private MazeNode _nodeTo = null!;

    [SetUp]
    public void Setup() {
        _graph = new MazeGraph(10, 10);
        _nodeFrom =_graph.GetOrCreateNode(new Vector2I(0, 0));
        _nodeTo =_graph.GetOrCreateNode(new Vector2I(1, 0));
    }

    [Test]
    public void Constructor_WithValidParams_CreatesEdge() {
        var direction = Vector2I.Right;
        var edge = new MazeNodeEdge(_nodeFrom, _nodeTo, direction);

        Assert.That(edge.From, Is.EqualTo(_nodeFrom));
        Assert.That(edge.To, Is.EqualTo(_nodeTo));
        Assert.That(edge.Direction, Is.EqualTo(direction));
    }

    [Test]
    public void Constructor_WithNullFrom_ThrowsArgumentNullException() {
        Assert.Throws<ArgumentNullException>(() =>
            new MazeNodeEdge(null!, _nodeTo, Vector2I.Right));
    }

    [Test]
    public void Constructor_WithNullTo_ThrowsArgumentNullException() {
        Assert.Throws<ArgumentNullException>(() =>
            new MazeNodeEdge(_nodeFrom, null!, Vector2I.Right));
    }

    [Test]
    public void Metadata_CanBeSetAndRetrieved() {
        var edge = new MazeNodeEdge(_nodeFrom, _nodeTo, Vector2I.Right);
        var metadata = new object();

        edge.Metadata = metadata;

        Assert.That(edge.Metadata, Is.EqualTo(metadata));
    }
}

[TestFixture]
[Only]
public class MazeNodeTests {
    private MazeGraph _graph = null!;
    private MazeNode _node = null!;

    [SetUp]
    public void Setup() {
        _graph = new MazeGraph(10, 10);
        _node = _graph.GetOrCreateNode(new Vector2I(1, 1));
    }

    [Test]
    public void Constructor_SetsProperties() {
        Assert.Multiple(() => {
            Assert.That(_node.Id, Is.EqualTo(0));
            Assert.That(_node.Position, Is.EqualTo(new Vector2I(1, 1)));
            Assert.That(_node.MazeGraph, Is.EqualTo(_graph));
            Assert.That(_node.Parent, Is.Null);
        });
    }

    [Test]
    public void Connect_CreatesEdge() {
        var other = _graph.GetOrCreateNode(new Vector2I(1, 0));
        var edge = _node.SetEdge(Vector2I.Up, other);

        Assert.Multiple(() => {
            Assert.That(edge.From, Is.EqualTo(_node));
            Assert.That(edge.To, Is.EqualTo(other));
            Assert.That(edge.Direction, Is.EqualTo(Vector2I.Up));
            Assert.That(_node.Up, Is.EqualTo(edge));
        });
    }

    [Test]
    public void Connect_WithNullNode_ThrowsArgumentNullException() {
        Assert.Throws<ArgumentNullException>(() =>
            _node.SetEdge(Vector2I.Up, null!));
    }

    [Test]
    public void RemoveEdge_RemovesConnection() {
        var other = _graph.GetOrCreateNode(new Vector2I(1, 0));
        _node.SetEdge(Vector2I.Up, other);

        _node.RemoveEdge(Vector2I.Up);

        Assert.That(_node.Up, Is.Null);
    }

    [Test]
    public void HasEdge_ReturnsCorrectValue() {
        var other = _graph.GetOrCreateNode(new Vector2I(1, 0));
        _node.SetEdge(Vector2I.Up, other);

        Assert.Multiple(() => {
            Assert.That(_node.HasEdge(Vector2I.Up), Is.True);
            Assert.That(_node.HasEdge(Vector2I.Down), Is.False);
        });
    }

    [Test]
    public void GetEdge_ReturnsCorrectEdge() {
        var other = _graph.GetOrCreateNode(new Vector2I(1, 0));
        var edge = _node.SetEdge(Vector2I.Up, other);

        Assert.That(_node.GetEdge(Vector2I.Up), Is.EqualTo(edge));
    }

    [Test]
    public void GetEdgeTo_ReturnsCorrectEdge() {
        var other = _graph.GetOrCreateNode(new Vector2I(1, 0));
        var edge = _node.SetEdge(Vector2I.Up, other);

        Assert.That(_node.GetEdgeTo(other), Is.EqualTo(edge));
    }

    [Test]
    public void GetChildren_ReturnsCorrectNodes() {
        var child1 = _graph.GetOrCreateNode(new Vector2I(1, 0));
        var child2 = _graph.GetOrCreateNode(new Vector2I(2, 1));
        var nonChild = _graph.GetOrCreateNode(new Vector2I(0, 1));
        
        child1.Parent = _node;
        child2.Parent = _node;

        var children = _node.GetChildren().ToList();

        Assert.Multiple(() => {
            Assert.That(children, Has.Count.EqualTo(2));
            Assert.That(children, Contains.Item(child1));
            Assert.That(children, Contains.Item(child2));
            Assert.That(children, Does.Not.Contain(nonChild));
        });
    }

    [Test]
    public void GetEdges_ReturnsAllConnectedEdges() {
        var up = _graph.GetOrCreateNode(new Vector2I(1, 0));
        var right = _graph.GetOrCreateNode(new Vector2I(2, 1));

        var edgeUp = _node.SetEdge(Vector2I.Up, up);
        var edgeRight = _node.SetEdge(Vector2I.Right, right);

        var edges = _node.GetEdges().ToList();

        Assert.Multiple(() => {
            Assert.That(edges, Has.Count.EqualTo(2));
            Assert.That(edges, Contains.Item(edgeUp));
            Assert.That(edges, Contains.Item(edgeRight));
        });
    }

    [Test]
    public void GetDirectionToParent_ReturnsCorrectDirection() {
        var parent = _graph.GetOrCreateNode(new Vector2I(1, 0));
        _node.Parent = parent;

        var direction = _node.GetDirectionToParent();

        Assert.That(direction, Is.EqualTo(Vector2I.Down));
    }

    [Test]
    public void GetDirectionToParent_WithNoParent_ReturnsNull() {
        Assert.That(_node.GetDirectionToParent(), Is.Null);
    }

    [Test]
    public void GetPathToRoot_ReturnsCorrectPath() {
        var root = _graph.GetOrCreateNode(new Vector2I(0, 0));
        var middle = _graph.GetOrCreateNode(new Vector2I(1, 0));
        var leaf = _graph.GetOrCreateNode(new Vector2I(2, 0));
        
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
        var up = _graph.GetOrCreateNode(new Vector2I(1, 0));
        var right = _graph.GetOrCreateNode(new Vector2I(2, 1));
        var down = _graph.GetOrCreateNode(new Vector2I(1, 2));

        _node.SetEdge(Vector2I.Up, up);
        _node.SetEdge(Vector2I.Right, right);
        down.SetEdge(Vector2I.Up, _node);

        down.Parent = _node;

        Assert.That(down.Up.To, Is.EqualTo(_node));
        Assert.That(_node.Right.To, Is.EqualTo(right));
        Assert.That(_node.Up.To, Is.EqualTo(up));

        _node.Remove();

        Assert.That(_graph.Nodes.ContainsKey(0), Is.False);
        Assert.That(_graph.NodeGrid[_node.Position], Is.Null);
        Assert.That(_node.Up, Is.Null);
        Assert.That(_node.Right, Is.Null);
        Assert.That(_node.Down, Is.Null);
        Assert.That(_node.Left, Is.Null);
        Assert.That(_node.Parent, Is.Null);
        Assert.That(up.Down, Is.Null);
        Assert.That(right.Left, Is.Null);
        Assert.That(down.Up, Is.Null);
        Assert.That(down.Parent, Is.Null);
    }
    
    [Test]
    public void GetNeighbors_CenterNode_ReturnsFourNeighbors() {
        // Create center node at (2,2)
        var center = _graph.GetOrCreateNode(new Vector2I(2, 2));
    
        // Create all surrounding nodes
        var up = _graph.GetOrCreateNode(new Vector2I(2, 1));
        var right = _graph.GetOrCreateNode(new Vector2I(3, 2));
        var down = _graph.GetOrCreateNode(new Vector2I(2, 3));
        var left = _graph.GetOrCreateNode(new Vector2I(1, 2));
    
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