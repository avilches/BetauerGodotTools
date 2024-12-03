using System.Linq;
using Betauer.Core.PCG.Maze;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
[Only]
public class MazeNodeTests {
    private MazeGraph _graph = null!;
    private MazeNode _node = null!;

    [SetUp]
    public void Setup() {
        _graph = new MazeGraph(10, 10);
        _node = _graph.CreateNode(new Vector2I(1, 1));
    }

    [Test]
    public void Constructor_SetsPropertiesCorrectly() {
        Assert.Multiple(() => {
            Assert.That(_node.Id, Is.EqualTo(0));
            Assert.That(_node.Position, Is.EqualTo(new Vector2I(1, 1)));
            Assert.That(_node.MazeGraph, Is.EqualTo(_graph));
            Assert.That(_node.Parent, Is.Null);
            Assert.That(_node.Zone, Is.EqualTo(0));
        });
    }

    [Test]
    public void AddEdgeTo_CreatesBidirectionalConnection() {
        var other = _graph.CreateNode(new Vector2I(1, 0));
        var edge = _node.AddEdgeTo(other);

        Assert.Multiple(() => {
            Assert.That(edge.From, Is.EqualTo(_node));
            Assert.That(edge.To, Is.EqualTo(other));
            Assert.That(_node.HasEdgeTo(other), Is.True);
        });
    }

    [Test]
    public void AddEdgeTo_WhenEdgeExists_ReturnsSameEdge() {
        var other = _graph.CreateNode(new Vector2I(1, 0));
        var edge1 = _node.AddEdgeTo(other);
        var edge2 = _node.AddEdgeTo(other);
        
        Assert.That(edge1, Is.EqualTo(edge2));
    }

    [Test]
    public void RemoveEdgeTo_RemovesConnection() {
        var other = _graph.CreateNode(new Vector2I(1, 0));
        var edge = _node.AddEdgeTo(other);
        
        _node.RemoveEdgeTo(other);
        
        Assert.Multiple(() => {
            Assert.That(_node.HasEdgeTo(other), Is.False);
            Assert.That(_node.GetEdges(), Does.Not.Contain(edge));
        });
    }

    [Test]
    public void GetEdges_ReturnsAllEdges() {
        var up = _graph.CreateNode(new Vector2I(1, 0));
        var right = _graph.CreateNode(new Vector2I(2, 1));
        var edgeUp = _node.AddEdgeTo(up);
        var edgeRight = _node.AddEdgeTo(right);

        var edges = _node.GetEdges().ToList();

        Assert.Multiple(() => {
            Assert.That(edges, Has.Count.EqualTo(2));
            Assert.That(edges, Contains.Item(edgeUp));
            Assert.That(edges, Contains.Item(edgeRight));
        });
    }

    [Test]
    public void GetDirectionToParent_ReturnsCorrectDirection() {
        var parent = _graph.CreateNode(new Vector2I(1, 0));
        _node.Parent = parent;

        Assert.That(_node.GetDirectionToParent(), Is.EqualTo(new Vector2I(0, 1)));
    }

    [Test]
    public void GetDirectionToParent_WithNoParent_ReturnsNull() {
        Assert.That(_node.GetDirectionToParent(), Is.Null);
    }

    [Test]
    public void GetChildren_ReturnsCorrectNodes() {
        var child1 = _graph.CreateNode(new Vector2I(1, 0));
        var child2 = _graph.CreateNode(new Vector2I(2, 1));
        var nonChild = _graph.CreateNode(new Vector2I(0, 1));
        
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
    public void GetEdgesToChildren_ReturnsCorrectEdges() {
        var child1 = _graph.CreateNode(new Vector2I(1, 0));
        var child2 = _graph.CreateNode(new Vector2I(2, 1));
        var nonChild = _graph.CreateNode(new Vector2I(0, 1));
        
        child1.Parent = _node;
        child2.Parent = _node;
        
        var edge1 = _node.AddEdgeTo(child1);
        var edge2 = _node.AddEdgeTo(child2);
        var edge3 = _node.AddEdgeTo(nonChild);

        var childEdges = _node.GetEdgesToChildren().ToList();

        Assert.Multiple(() => {
            Assert.That(childEdges, Has.Count.EqualTo(2));
            Assert.That(childEdges, Contains.Item(edge1));
            Assert.That(childEdges, Contains.Item(edge2));
            Assert.That(childEdges, Does.Not.Contain(edge3));
        });
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
}