using System;
using Betauer.Core.PCG.Maze;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class MazeEdgeTests {
    private MazeGraph _graph = null!;
    private MazeNode _nodeFrom = null!;
    private MazeNode _nodeTo = null!;

    [SetUp]
    public void Setup() {
        _graph = new MazeGraph(10, 10);
        _nodeFrom = _graph.CreateNode(new Vector2I(0, 0));
        _nodeTo = _graph.CreateNode(new Vector2I(1, 0));
    }

    [Test]
    public void Constructor_WithValidParams_CreatesEdge() {
        var edge = new MazeEdge(_nodeFrom, _nodeTo);

        Assert.Multiple(() => {
            Assert.That(edge.From, Is.EqualTo(_nodeFrom));
            Assert.That(edge.To, Is.EqualTo(_nodeTo));
            Assert.That(edge.Direction, Is.EqualTo(Vector2I.Right));
        });
    }

    [Test]
    public void Constructor_WithNullFrom_ThrowsArgumentNullException() {
        Assert.Throws<ArgumentNullException>(() => new MazeEdge(null!, _nodeTo));
    }

    [Test]
    public void Constructor_WithNullTo_ThrowsArgumentNullException() {
        Assert.Throws<ArgumentNullException>(() => new MazeEdge(_nodeFrom, null!));
    }

    [Test]
    public void Direction_CalculatesCorrectly() {
        var diagonal = _graph.CreateNode(new Vector2I(1, 1));
        var edgeDiagonal = new MazeEdge(_nodeFrom, diagonal);
        
        Assert.That(edgeDiagonal.Direction, Is.EqualTo(new Vector2I(1, 1)));
    }
}