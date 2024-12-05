using System.Linq;
using Betauer.Core.PCG.Maze;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class MazeNodeTests {
    private MazeGraph<object> _graph = null!;
    private MazeNode<object> _node = null!;

    [SetUp]
    public void Setup() {
        _graph = new MazeGraph<object>(10, 10);
        _node = _graph.CreateNode(new Vector2I(1, 1));
    }

    [Test]
    public void Constructor_SetsPropertiesCorrectly() {
        Assert.Multiple(() => {
            Assert.That(_node.Id, Is.EqualTo(0));
            Assert.That(_node.Position, Is.EqualTo(new Vector2I(1, 1)));
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
    public void GetChildren_ReturnsCorrectNodes() {
        var child1 = _graph.CreateNode(new Vector2I(1, 0));
        var child2 = _graph.CreateNode(new Vector2I(2, 1));
        var nonChild = _graph.CreateNode(new Vector2I(0, 1));

        child1.Parent = _node;
        child2.Parent = _node;

        var children = _graph.GetChildren(_node).ToList();

        Assert.Multiple(() => {
            Assert.That(children, Has.Count.EqualTo(2));
            Assert.That(children, Contains.Item(child1));
            Assert.That(children, Contains.Item(child2));
            Assert.That(children, Does.Not.Contain(nonChild));
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

    [Test]
    public void GetPathToNode_WhenSameNode_ReturnsSingleNodePath() {
        var path = _node.GetPathToNode(_node);

        Assert.Multiple(() => {
            Assert.That(path, Is.Not.Null);
            Assert.That(path, Has.Count.EqualTo(1));
            Assert.That(path![0], Is.EqualTo(_node));
        });
    }

    [Test]
    public void GetPathToNode_DirectParentChild_ReturnsCorrectPath() {
        var child = _graph.CreateNode(new Vector2I(2, 1));
        child.Parent = _node;

        var pathToChild = _node.GetPathToNode(child);
        var pathFromChild = child.GetPathToNode(_node);

        Assert.Multiple(() => {
            // Path from parent to child
            Assert.That(pathToChild, Is.Not.Null);
            Assert.That(pathToChild, Has.Count.EqualTo(2));
            Assert.That(pathToChild![0], Is.EqualTo(_node));
            Assert.That(pathToChild![1], Is.EqualTo(child));

            // Path from child to parent
            Assert.That(pathFromChild, Is.Not.Null);
            Assert.That(pathFromChild, Has.Count.EqualTo(2));
            Assert.That(pathFromChild![0], Is.EqualTo(child));
            Assert.That(pathFromChild![1], Is.EqualTo(_node));
        });
    }

    [Test]
    public void GetPathToNode_ComplexPath_ReturnsCorrectPath() {
        var root = _graph.CreateNode(new Vector2I(0, 0));
        var nodeA = _graph.CreateNode(new Vector2I(1, 0));
        var nodeB = _graph.CreateNode(new Vector2I(2, 0));

        // Create a tree: root -> nodeA -> nodeB
        nodeA.Parent = root;
        nodeB.Parent = nodeA;

        var path = nodeB.GetPathToNode(root);

        Assert.Multiple(() => {
            Assert.That(path, Is.Not.Null);
            Assert.That(path, Has.Count.EqualTo(3));
            Assert.That(path![0], Is.EqualTo(nodeB));
            Assert.That(path![1], Is.EqualTo(nodeA));
            Assert.That(path![2], Is.EqualTo(root));
        });
    }

    [Test]
    public void GetPathToNode_NoCommonAncestor_ReturnsNull() {
        var nodeA = _graph.CreateNode(new Vector2I(0, 0));
        var nodeB = _graph.CreateNode(new Vector2I(1, 0));

        // Nodes without common ancestor
        var path = nodeA.GetPathToNode(nodeB);

        Assert.That(path, Is.Null);
    }

    [Test]
    public void GetDistanceToNode_ValidPath_ReturnsCorrectDistance() {
        var root = _graph.CreateNode(new Vector2I(0, 0));
        var nodeA = _graph.CreateNode(new Vector2I(1, 0));
        var nodeB = _graph.CreateNode(new Vector2I(2, 0));

        nodeA.Parent = root;
        nodeB.Parent = nodeA;

        Assert.Multiple(() => {
            Assert.That(nodeB.GetDistanceToNode(root), Is.EqualTo(2));
            Assert.That(nodeB.GetDistanceToNode(nodeA), Is.EqualTo(1));
            Assert.That(nodeB.GetDistanceToNode(nodeB), Is.EqualTo(0));
        });
    }

    [Test]
    public void GetDistanceToNode_NoPath_ReturnsMinusOne() {
        var nodeA = _graph.CreateNode(new Vector2I(0, 0));
        var nodeB = _graph.CreateNode(new Vector2I(1, 0));

        Assert.That(nodeA.GetDistanceToNode(nodeB), Is.EqualTo(-1));
    }

    [Test]
    public void FindShortestPath_WhenSameNode_ReturnsSingleNodePath() {
        var path = _node.FindShortestPath(_node);

        Assert.Multiple(() => {
            Assert.That(path, Is.Not.Null);
            Assert.That(path, Has.Count.EqualTo(1));
            Assert.That(path![0], Is.EqualTo(_node));
        });
    }

    [Test]
    public void FindShortestPath_DirectConnection_ReturnsCorrectPath() {
        var nodeA = _graph.CreateNode(new Vector2I(0, 0));
        var nodeB = _graph.CreateNode(new Vector2I(1, 0));

        nodeA.AddEdgeTo(nodeB);

        var path = nodeA.FindShortestPath(nodeB);

        Assert.Multiple(() => {
            Assert.That(path, Is.Not.Null);
            Assert.That(path, Has.Count.EqualTo(2));
            Assert.That(path![0], Is.EqualTo(nodeA));
            Assert.That(path![1], Is.EqualTo(nodeB));
        });
    }

    [Test]
    public void FindShortestPath_ComplexPath_ReturnsShortestPath() {
        var nodeA = _graph.GetOrCreateNode(new Vector2I(0, 0));
        var nodeB = _graph.GetOrCreateNode(new Vector2I(1, 0));
        var nodeC = _graph.GetOrCreateNode(new Vector2I(2, 0));
        var nodeD = _graph.GetOrCreateNode(new Vector2I(1, 1));

        // Create a diamond shape: A -> B -> C and A -> D -> C
        nodeA.AddEdgeTo(nodeB);
        nodeB.AddEdgeTo(nodeC);
        nodeA.AddEdgeTo(nodeD);
        nodeD.AddEdgeTo(nodeC);

        var path = nodeA.FindShortestPath(nodeC);

        Assert.Multiple(() => {
            Assert.That(path, Is.Not.Null);
            Assert.That(path, Has.Count.EqualTo(3));
            Assert.That(path![0], Is.EqualTo(nodeA));
            // No importa qué camino tome (por B o por D) mientras la longitud sea 3
            Assert.That(path![2], Is.EqualTo(nodeC));
        });
    }

    [Test]
    public void FindShortestPath_NoPath_ReturnsNull() {
        var nodeA = _graph.CreateNode(new Vector2I(0, 0));
        var nodeB = _graph.CreateNode(new Vector2I(1, 0));

        // No edges between nodes
        var path = nodeA.FindShortestPath(nodeB);

        Assert.That(path, Is.Null);
    }

    [Test]
    public void GetDistanceToNodeByEdges_ValidPaths_ReturnsCorrectDistance() {
        var nodeA = _graph.CreateNode(new Vector2I(0, 0));
        var nodeB = _graph.CreateNode(new Vector2I(1, 0));
        var nodeC = _graph.CreateNode(new Vector2I(2, 0));

        nodeA.AddEdgeTo(nodeB);
        nodeB.AddEdgeTo(nodeC);

        Assert.Multiple(() => {
            Assert.That(nodeA.GetDistanceToNodeByEdges(nodeC), Is.EqualTo(2));
            Assert.That(nodeA.GetDistanceToNodeByEdges(nodeB), Is.EqualTo(1));
            Assert.That(nodeA.GetDistanceToNodeByEdges(nodeA), Is.EqualTo(0));
        });
    }

    [Test]
    public void GetDistanceToNodeByEdges_NoPath_ReturnsMinusOne() {
        var nodeA = _graph.CreateNode(new Vector2I(0, 0));
        var nodeB = _graph.CreateNode(new Vector2I(1, 0));

        Assert.That(nodeA.GetDistanceToNodeByEdges(nodeB), Is.EqualTo(-1));
    }

    [Test]
    public void GetDistanceToNodeByEdges_BidirectionalPath_SameDistanceBothWays() {
        var nodeA = _graph.CreateNode(new Vector2I(0, 0));
        var nodeB = _graph.CreateNode(new Vector2I(1, 0));

        // Create bidirectional connection
        nodeA.AddEdgeTo(nodeB);
        nodeB.AddEdgeTo(nodeA);

        Assert.Multiple(() => {
            Assert.That(nodeA.GetDistanceToNodeByEdges(nodeB), Is.EqualTo(1));
            Assert.That(nodeB.GetDistanceToNodeByEdges(nodeA), Is.EqualTo(1));
        });
    }

}