using System;
using Betauer.Core.PCG.Maze;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class MazeEdgeTests {
    private MazeGraph<object> _graph = null!;
    private MazeNode<object> _nodeFrom = null!;
    private MazeNode<object> _nodeTo = null!;

    [SetUp]
    public void Setup() {
        _graph = new MazeGraph<object>(10, 10);
        _nodeFrom = _graph.CreateNode(new Vector2I(0, 0));
        _nodeTo = _graph.CreateNode(new Vector2I(1, 0));
    }

    [Test]
    public void Constructor_WithValidParams_CreatesEdge() {
        var edge = new MazeEdge<object>(_nodeFrom, _nodeTo);

        Assert.Multiple(() => {
            Assert.That(edge.From, Is.EqualTo(_nodeFrom));
            Assert.That(edge.To, Is.EqualTo(_nodeTo));
            Assert.That(edge.Direction, Is.EqualTo(Vector2I.Right));
        });
    }

    [Test]
    public void Constructor_WithNullFrom_ThrowsArgumentNullException() {
        Assert.Throws<ArgumentNullException>(() => new MazeEdge<object>(null!, _nodeTo));
    }

    [Test]
    public void Constructor_WithNullTo_ThrowsArgumentNullException() {
        Assert.Throws<ArgumentNullException>(() => new MazeEdge<object>(_nodeFrom, null!));
    }

    [Test]
    public void Direction_CalculatesCorrectly() {
        var diagonal = _graph.CreateNode(new Vector2I(1, 1));
        var edgeDiagonal = new MazeEdge<object>(_nodeFrom, diagonal);
        
        Assert.That(edgeDiagonal.Direction, Is.EqualTo(new Vector2I(1, 1)));
    }
}