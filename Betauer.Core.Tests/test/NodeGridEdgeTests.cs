using System;
using Betauer.Core.PCG.Maze;
using Godot;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class NodeGridEdgeTests {
    private MazeGraph _graph = null!;
    private NodeGrid _nodeGridFrom = null!;
    private NodeGrid _nodeGridTo = null!;

    [SetUp]
    public void Setup() {
        _graph = new MazeGraph(10, 10);
        _nodeGridFrom =_graph.GetOrCreateNode(new Vector2I(0, 0));
        _nodeGridTo =_graph.GetOrCreateNode(new Vector2I(1, 0));
    }

    [Test]
    public void Constructor_WithValidParams_CreatesEdge() {
        var direction = Vector2I.Right;
        var edge = new NodeGridEdge(_nodeGridFrom, _nodeGridTo, direction);

        Assert.That(edge.From, Is.EqualTo(_nodeGridFrom));
        Assert.That(edge.To, Is.EqualTo(_nodeGridTo));
        Assert.That(edge.Direction, Is.EqualTo(direction));
    }

    [Test]
    public void Constructor_WithNullFrom_ThrowsArgumentNullException() {
        Assert.Throws<ArgumentNullException>(() =>
            new NodeGridEdge(null!, _nodeGridTo, Vector2I.Right));
    }

    [Test]
    public void Constructor_WithNullTo_ThrowsArgumentNullException() {
        Assert.Throws<ArgumentNullException>(() =>
            new NodeGridEdge(_nodeGridFrom, null!, Vector2I.Right));
    }

    [Test]
    public void Metadata_CanBeSetAndRetrieved() {
        var edge = new NodeGridEdge(_nodeGridFrom, _nodeGridTo, Vector2I.Right);
        var metadata = new object();

        edge.Metadata = metadata;

        Assert.That(edge.Metadata, Is.EqualTo(metadata));
    }
}