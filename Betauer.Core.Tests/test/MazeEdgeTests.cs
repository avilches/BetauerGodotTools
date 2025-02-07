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
    private MazeEdge _edge = null!;

    [SetUp]
    public void Setup() {
        _graph = MazeGraph.Create(10, 10);
        _nodeFrom = _graph.CreateNode(new Vector2I(0, 0));
        _nodeTo = _graph.CreateNode(new Vector2I(1, 0));
        _edge = new MazeEdge(_nodeFrom, _nodeTo);
    }

    [Test]
    public void Constructor_WithValidParams_CreatesEdge() {
        Assert.Multiple(() => {
            Assert.That(_edge.From, Is.EqualTo(_nodeFrom));
            Assert.That(_edge.To, Is.EqualTo(_nodeTo));
            Assert.That(_edge.Direction, Is.EqualTo(Vector2I.Right));
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
    public void Edge_WeightAndMetadata_CanBeModified() {
        var edge = new MazeEdge(_nodeFrom, _nodeTo, "metadata", 1.5f);
        
        Assert.Multiple(() => {
            Assert.That(edge.Weight, Is.EqualTo(1.5f));
            Assert.That(edge.Metadata, Is.EqualTo("metadata"));
        });

        edge.Weight = 2.5f;
        edge.Metadata = "new metadata";
        
        Assert.Multiple(() => {
            Assert.That(edge.Weight, Is.EqualTo(2.5f));
            Assert.That(edge.Metadata, Is.EqualTo("new metadata"));
        });
    }

    [Test]
    public void Edge_IsAddedCorrectly_ToNodesCollections() {
        var edge = _nodeFrom.ConnectTo(_nodeTo);
        
        Assert.Multiple(() => {
            Assert.That(_nodeFrom.OutDegree, Is.EqualTo(1));
            Assert.That(_nodeTo.InDegree, Is.EqualTo(1));
            Assert.That(_nodeFrom.GetOutEdges(), Does.Contain(edge));
            Assert.That(_nodeTo.GetInEdges(), Does.Contain(edge));
        });
    }

    [Test]
    public void Edge_IsRemovedCorrectly_FromNodesCollections() {
        _nodeFrom.ConnectTo(_nodeTo);
        var edge = _nodeFrom.GetEdgeTo(_nodeTo);
        Assert.That(edge, Is.Not.Null);
        
        bool removed = _nodeFrom.RemoveEdge(edge!);
        
        Assert.Multiple(() => {
            Assert.That(removed, Is.True);
            Assert.That(_nodeFrom.OutDegree, Is.EqualTo(0));
            Assert.That(_nodeTo.InDegree, Is.EqualTo(0));
            Assert.That(_nodeFrom.GetOutEdges(), Does.Not.Contain(edge));
            Assert.That(_nodeTo.GetInEdges(), Does.Not.Contain(edge));
        });
    }

    [Test]
    public void Edge_CannotBeAdded_BetweenNodesFromDifferentGraphs() {
        var otherGraph = MazeGraph.Create(10, 10);
        var otherNode = otherGraph.CreateNode(new Vector2I(2, 0));

        Assert.Throws<InvalidEdgeException>(() => _nodeFrom.ConnectTo(otherNode));
    }

    [Test]
    public void Edge_CannotBeAdded_ToSameNode() {
        Assert.Throws<InvalidEdgeException>(() => _nodeFrom.ConnectTo(_nodeFrom));
    }

    [Test]
    public void Edge_CanBeAddedAndRemoved_InBothDirections() {
        // Conexión bidireccional
        var edgeForward = _nodeFrom.ConnectTo(_nodeTo);
        var edgeBackward = _nodeTo.ConnectTo(_nodeFrom);

        Assert.Multiple(() => {
            // Verificar conexiones
            Assert.That(_nodeFrom.OutDegree, Is.EqualTo(1));
            Assert.That(_nodeFrom.InDegree, Is.EqualTo(1));
            Assert.That(_nodeTo.OutDegree, Is.EqualTo(1));
            Assert.That(_nodeTo.InDegree, Is.EqualTo(1));

            // Verificar que los edges están en las colecciones correctas
            Assert.That(_nodeFrom.GetOutEdges(), Does.Contain(edgeForward));
            Assert.That(_nodeFrom.GetInEdges(), Does.Contain(edgeBackward));
            Assert.That(_nodeTo.GetOutEdges(), Does.Contain(edgeBackward));
            Assert.That(_nodeTo.GetInEdges(), Does.Contain(edgeForward));
        });

        // Remover una dirección
        bool removed = _nodeFrom.RemoveEdge(edgeForward);
        Assert.That(removed, Is.True);

        Assert.Multiple(() => {
            // Verificar que solo se removió una dirección
            Assert.That(_nodeFrom.OutDegree, Is.EqualTo(0));
            Assert.That(_nodeFrom.InDegree, Is.EqualTo(1));
            Assert.That(_nodeTo.OutDegree, Is.EqualTo(1));
            Assert.That(_nodeTo.InDegree, Is.EqualTo(0));
        });
    }

    [Test]
    public void Edge_GetEdgeTowards_ReturnsCorrectEdge() {
        _nodeFrom.ConnectTo(_nodeTo);

        var edge = _nodeFrom.GetEdgeTowards(Vector2I.Right);
        Assert.That(edge, Is.Not.Null);
        Assert.That(edge!.To, Is.EqualTo(_nodeTo));

        edge = _nodeFrom.GetEdgeTowards(Vector2I.Left);
        Assert.That(edge, Is.Null);
    }

    [Test]
    public void Edge_GetEdgeFromDirection_ReturnsCorrectEdge() {
        _nodeTo.ConnectTo(_nodeFrom);  // Conectar en dirección opuesta

        var edge = _nodeFrom.GetEdgeFromDirection(Vector2I.Right);
        Assert.That(edge, Is.Not.Null);
        Assert.That(edge!.From, Is.EqualTo(_nodeTo));

        edge = _nodeFrom.GetEdgeFromDirection(Vector2I.Left);
        Assert.That(edge, Is.Null);
    }
}