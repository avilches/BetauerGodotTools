using System;
using System.Linq;
using Betauer.Core.PCG.Graph;
using Betauer.Core.PCG.Maze;
using Godot;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class MazeNodeTests {
    private MazeGraph _graph = null!;
    private MazeNode _node = null!;

    [SetUp]
    public void Setup() {
        _graph = MazeGraph.Create(10, 10);
        _node = _graph.CreateNode(new Vector2I(1, 1));
    }

    [Test]
    public void Constructor_SetsPropertiesCorrectly() {
        Assert.Multiple(() => {
            Assert.That(_node.Id, Is.EqualTo(0));
            Assert.That(_node.Position, Is.EqualTo(new Vector2I(1, 1)));
            Assert.That(_node.Parent, Is.Null);
            Assert.That(_node.ZoneId, Is.EqualTo(0));
            Assert.That(_node.OutDegree, Is.EqualTo(0));
            Assert.That(_node.InDegree, Is.EqualTo(0));
            Assert.That(_node.Degree, Is.EqualTo(0));
        });
    }

    [Test]
    public void ConnectTo_CreatesOneDirectionalConnection() {
        var other = _graph.CreateNode(new Vector2I(1, 0));
        var edge = _node.ConnectTo(other);

        Assert.Multiple(() => {
            Assert.That(edge.From, Is.EqualTo(_node));
            Assert.That(edge.To, Is.EqualTo(other));
            Assert.That(_node.HasEdgeTo(other), Is.True);
            Assert.That(other.HasEdgeFrom(_node), Is.True);
            Assert.That(other.HasEdgeTo(_node), Is.False);
            Assert.That(_node.HasEdgeFrom(other), Is.False);
            Assert.That(_node.OutDegree, Is.EqualTo(1));
            Assert.That(_node.InDegree, Is.EqualTo(0));
            Assert.That(other.OutDegree, Is.EqualTo(0));
            Assert.That(other.InDegree, Is.EqualTo(1));
        });
    }

    [Test]
    public void ConnectTo_WhenEdgeExists_ReturnsSameEdge() {
        var other = _graph.CreateNode(new Vector2I(1, 0));
        var edge1 = _node.ConnectTo(other);
        var edge2 = _node.ConnectTo(other);

        Assert.That(edge1, Is.EqualTo(edge2));
    }

    [Test]
    public void RemoveEdge_RemovesBothConnections() {
        var other = _graph.CreateNode(new Vector2I(1, 0));
        var edge = _node.ConnectTo(other);

        _node.RemoveEdge(edge);

        Assert.Multiple(() => {
            Assert.That(_node.HasEdgeTo(other), Is.False);
            Assert.That(other.HasEdgeFrom(_node), Is.False);
            Assert.That(_node.OutDegree, Is.EqualTo(0));
            Assert.That(other.InDegree, Is.EqualTo(0));
        });
    }

    [Test]
    public void GetEdges_ReturnsOutgoingEdges() {
        var up = _graph.CreateNode(new Vector2I(1, 0));
        var right = _graph.CreateNode(new Vector2I(2, 1));
        var edgeUp = _node.ConnectTo(up);
        var edgeRight = _node.ConnectTo(right);

        var edges = _node.GetOutEdges().ToList();

        Assert.Multiple(() => {
            Assert.That(edges, Has.Count.EqualTo(2));
            Assert.That(edges, Contains.Item(edgeUp));
            Assert.That(edges, Contains.Item(edgeRight));
        });
    }

    [Test]
    public void GetInEdges_ReturnsIncomingEdges() {
        _graph.Clear();
        var up = _graph.CreateNode(new Vector2I(1, 0));
        var right = _graph.CreateNode(new Vector2I(2, 1));
        up.ConnectTo(_node);
        right.ConnectTo(_node);

        var edges = _node.GetInEdges().ToList();

        Assert.Multiple(() => {
            Assert.That(edges, Has.Count.EqualTo(2));
            Assert.That(edges.All(e => e.To == _node), Is.True);
            Assert.That(edges.Select(e => e.From), Contains.Item(up));
            Assert.That(edges.Select(e => e.From), Contains.Item(right));
        });
    }

    [Test]
    public void GetAllEdges_ReturnsAllConnections() {
        var other = _graph.CreateNode(new Vector2I(1, 0));
        var third = _graph.CreateNode(new Vector2I(2, 1));

        // _node -> other, third -> _node
        var outEdge = _node.ConnectTo(other);
        var inEdge = third.ConnectTo(_node);

        var allEdges = _node.GetAllEdges().ToList();

        Assert.Multiple(() => {
            Assert.That(allEdges, Has.Count.EqualTo(2));
            Assert.That(allEdges, Contains.Item(outEdge));
            Assert.That(allEdges, Contains.Item(inEdge));
            Assert.That(_node.Degree, Is.EqualTo(2));
        });
    }

    [Test]
    public void Degree_ReturnsSumOfInAndOutDegrees() {
        var node1 = _graph.CreateNode(new Vector2I(1, 0));
        var node2 = _graph.CreateNode(new Vector2I(2, 0));
        var node3 = _graph.CreateNode(new Vector2I(0, 1));

        // Dos conexiones salientes y una entrante
        _node.ConnectTo(node1);
        _node.ConnectTo(node2);
        node3.ConnectTo(_node);

        Assert.Multiple(() => {
            Assert.That(_node.OutDegree, Is.EqualTo(2));
            Assert.That(_node.InDegree, Is.EqualTo(1));
            Assert.That(_node.Degree, Is.EqualTo(3));
        });
    }

    [Test]
    public void GetEdgeFrom_ReturnsCorrectEdge() {
        var other = _graph.CreateNode(new Vector2I(1, 0));
        var edge = other.ConnectTo(_node);

        var foundEdge = _node.GetEdgeFrom(other);

        Assert.Multiple(() => {
            Assert.That(foundEdge, Is.Not.Null);
            Assert.That(foundEdge, Is.EqualTo(edge));
            Assert.That(foundEdge!.From, Is.EqualTo(other));
            Assert.That(foundEdge.To, Is.EqualTo(_node));
        });
    }

    [Test]
    public void HasEdgeFromDirection_ReturnsCorrectResult() {
        var up = _graph.CreateNode(new Vector2I(1, 0));
        up.ConnectTo(_node); // Conecta desde arriba hacia el nodo

        Assert.Multiple(() => {
            Assert.That(_node.HasEdgeFromDirection(Vector2I.Up), Is.True);
            Assert.That(_node.HasEdgeFromDirection(Vector2I.Down), Is.False);
            Assert.That(_node.HasEdgeFromDirection(Vector2I.Left), Is.False);
            Assert.That(_node.HasEdgeFromDirection(Vector2I.Right), Is.False);
        });
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
    public void GetPathToRoot_ReturnsCorrectPath() {
        var root = _graph.CreateNode(new Vector2I(0, 0));
        var middle = _graph.CreateNode(new Vector2I(1, 0));
        var leaf = _graph.CreateNode(new Vector2I(2, 0));

        middle.Parent = root;
        leaf.Parent = middle;

        var path = leaf.FindTreePathToRoot();

        Assert.Multiple(() => {
            Assert.That(path, Has.Count.EqualTo(3));
            Assert.That(path[0], Is.EqualTo(leaf));
            Assert.That(path[1], Is.EqualTo(middle));
            Assert.That(path[2], Is.EqualTo(root));
        });
    }

    [Test]
    public void GetPathToNode_WhenSameNode_ReturnsSingleNodePath() {
        var path = _node.FindTreePathToNode(_node);

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

        var pathToChild = _node.FindTreePathToNode(child);
        var pathFromChild = child.FindTreePathToNode(_node);

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

        var path = nodeB.FindTreePathToNode(root);

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
        var path = nodeA.FindTreePathToNode(nodeB);

        Assert.That(path, Is.Empty);
    }

    [Test]
    public void Depth_RootNode_ReturnsZero() {
        // Root node should have depth 0
        Assert.That(_node.Depth, Is.EqualTo(0));
    }

    [Test]
    public void Depth_ChildNode_ReturnsOne() {
        var child = _graph.CreateNode(new Vector2I(2, 1));
        child.Parent = _node;

        Assert.That(child.Depth, Is.EqualTo(1));
    }

    [Test]
    public void Depth_GrandchildNode_ReturnsTwo() {
        var child = _graph.CreateNode(new Vector2I(2, 1));
        var grandchild = _graph.CreateNode(new Vector2I(3, 1));
        child.Parent = _node;
        grandchild.Parent = child;

        Assert.Multiple(() => {
            Assert.That(_node.Depth, Is.EqualTo(0));
            Assert.That(child.Depth, Is.EqualTo(1));
            Assert.That(grandchild.Depth, Is.EqualTo(2));
        });
    }

    [Test]
    public void Depth_ChangingParent_UpdatesDepth() {
        var nodeA = _graph.CreateNode(new Vector2I(2, 1));
        var nodeB = _graph.CreateNode(new Vector2I(3, 1));
        var nodeC = _graph.CreateNode(new Vector2I(4, 1));

        // Create initial structure:
        // _node -> nodeA -> nodeB -> nodeC (depths: 0, 1, 2, 3)
        nodeA.Parent = _node;
        nodeB.Parent = nodeA;
        nodeC.Parent = nodeB;

        Assert.Multiple(() => {
            Assert.That(nodeA.Depth, Is.EqualTo(1));
            Assert.That(nodeB.Depth, Is.EqualTo(2));
            Assert.That(nodeC.Depth, Is.EqualTo(3));
        });

        // Change structure to:
        // _node -> nodeB -> nodeC
        //      \-> nodeA
        // (depths: 0, 1, 2, 1)
        nodeB.Parent = _node;

        Assert.Multiple(() => {
            Assert.That(nodeA.Depth, Is.EqualTo(1));
            Assert.That(nodeB.Depth, Is.EqualTo(1));
            Assert.That(nodeC.Depth, Is.EqualTo(2));
        });
    }

    [Test]
    public void Depth_CacheInvalidation_UpdatesDescendants() {
        var nodeA = _graph.CreateNode(new Vector2I(2, 1));
        var nodeB = _graph.CreateNode(new Vector2I(3, 1));
        var nodeC = _graph.CreateNode(new Vector2I(4, 1));

        // Initial structure: _node -> nodeA -> nodeB -> nodeC
        nodeA.Parent = _node;
        nodeB.Parent = nodeA;
        nodeC.Parent = nodeB;

        // Force cache calculation
        var originalDepths = new[] {
            nodeA.Depth,
            nodeB.Depth,
            nodeC.Depth
        };

        // Change nodeA's parent to null, making it a root
        nodeA.Parent = null;

        // Verify all descendants' depths have been updated
        Assert.Multiple(() => {
            Assert.That(nodeA.Depth, Is.EqualTo(0));
            Assert.That(nodeB.Depth, Is.EqualTo(1));
            Assert.That(nodeC.Depth, Is.EqualTo(2));
            // Verify depths have actually changed from original values
            Assert.That(nodeA.Depth, Is.Not.EqualTo(originalDepths[0]));
            Assert.That(nodeB.Depth, Is.Not.EqualTo(originalDepths[1]));
            Assert.That(nodeC.Depth, Is.Not.EqualTo(originalDepths[2]));
        });
    }

    [Test]
    public void Depth_RemovingNode_UpdatesDepths() {
        var nodeA = _graph.CreateNode(new Vector2I(2, 1));
        var nodeB = _graph.CreateNode(new Vector2I(3, 1));

        // Initial structure: _node -> nodeA -> nodeB
        nodeA.Parent = _node;
        nodeB.Parent = nodeA;

        // Force cache calculation
        Assert.That(nodeB.Depth, Is.EqualTo(2));

        // Remove nodeA
        nodeA.Parent = null;

        // Check nodeB's depth is updated
        Assert.That(nodeB.Depth, Is.EqualTo(1));
    }

    [Test]
    public void Depth_CircularReference_ThrowsException() {
        var nodeA = _graph.CreateNode(new Vector2I(2, 1));
        var nodeB = _graph.CreateNode(new Vector2I(3, 1));

        nodeA.Parent = _node;
        nodeB.Parent = nodeA;

        // Try to create a circular reference
        Assert.Throws<InvalidOperationException>(() => _node.Parent = nodeB);
    }

    [Test]
    public void GetDistanceToNode_ValidPath_ReturnsCorrectDistance() {
        var root = _graph.CreateNode(new Vector2I(0, 0));
        var nodeA = _graph.CreateNode(new Vector2I(1, 0));
        var nodeB = _graph.CreateNode(new Vector2I(2, 0));

        nodeA.Parent = root;
        nodeB.Parent = nodeA;

        Assert.Multiple(() => {
            Assert.That(nodeB.GetTreeDistanceToNode(root), Is.EqualTo(2));
            Assert.That(nodeB.GetTreeDistanceToNode(nodeA), Is.EqualTo(1));
            Assert.That(nodeB.GetTreeDistanceToNode(nodeB), Is.EqualTo(0));
        });
    }

    [Test]
    public void GetDistanceToNode_NoPath_ReturnsMinusOne() {
        var nodeA = _graph.CreateNode(new Vector2I(0, 0));
        var nodeB = _graph.CreateNode(new Vector2I(1, 0));

        Assert.That(nodeA.GetTreeDistanceToNode(nodeB), Is.EqualTo(-1));
    }

    [Test]
    public void FindShortestPath_DirectionalEdges_RespectsEdgeDirection() {
        var nodeA = _graph.CreateNode(new Vector2I(0, 0));
        var nodeB = _graph.CreateNode(new Vector2I(1, 0));
        var nodeC = _graph.CreateNode(new Vector2I(2, 0));

        // Crear un camino direccional: A -> B -> C
        nodeA.ConnectTo(nodeB);
        nodeB.ConnectTo(nodeC);

        Assert.Multiple(() => {
            // Camino hacia adelante debe existir
            var forwardPath = nodeA.FindShortestPath(nodeC);
            Assert.That(forwardPath, Is.Not.Null);
            Assert.That(forwardPath, Has.Count.EqualTo(3));

            // Camino hacia atrás no debe existir
            var backwardPath = nodeC.FindShortestPath(nodeA);
            Assert.That(backwardPath, Is.Empty);
        });
    }

    [Test]
    public void GetDistanceToNodeByEdges_DirectionalEdges_RespectsEdgeDirection() {
        var nodeA = _graph.CreateNode(new Vector2I(0, 0));
        var nodeB = _graph.CreateNode(new Vector2I(1, 0));
        var nodeC = _graph.CreateNode(new Vector2I(2, 0));

        // Crear un camino direccional: A -> B -> C
        nodeA.ConnectTo(nodeB);
        nodeB.ConnectTo(nodeC);

        Assert.Multiple(() => {
            // Distancia hacia adelante debe ser 2
            Assert.That(nodeA.GetGraphDistanceToNode(nodeC), Is.EqualTo(2));

            // Distancia hacia atrás debe ser -1 (no hay camino)
            Assert.That(nodeC.GetGraphDistanceToNode(nodeA), Is.EqualTo(-1));
        });
    }

    [Test]
    public void GetReachableNodes_DirectionalEdges_RespectsEdgeDirection() {
        var nodeA = _graph.CreateNode(new Vector2I(0, 0));
        var nodeB = _graph.CreateNode(new Vector2I(1, 0));
        var nodeC = _graph.CreateNode(new Vector2I(2, 0));

        // Crear un camino direccional: A -> B -> C
        nodeA.ConnectTo(nodeB);
        nodeB.ConnectTo(nodeC);

        var reachableFromA = nodeA.GetReachableNodes();
        var reachableFromC = nodeC.GetReachableNodes();

        Assert.Multiple(() => {
            // Desde A se pueden alcanzar todos los nodos
            Assert.That(reachableFromA, Has.Count.EqualTo(3));
            Assert.That(reachableFromA, Contains.Item(nodeA));
            Assert.That(reachableFromA, Contains.Item(nodeB));
            Assert.That(reachableFromA, Contains.Item(nodeC));

            // Desde C solo se puede alcanzar C
            Assert.That(reachableFromC, Has.Count.EqualTo(1));
            Assert.That(reachableFromC, Contains.Item(nodeC));
        });
    }

    [Test]
    public void FindShortestPath_WithPredicate_RespectsPredicateConditions() {
        _graph.Clear();
        var nodeA = _graph.CreateNode(new Vector2I(0, 0));
        var nodeB = _graph.CreateNode(new Vector2I(1, 0));
        var nodeC = _graph.CreateNode(new Vector2I(2, 0));
        var nodeD = _graph.CreateNode(new Vector2I(1, 1));

        // Crear camino: A -> B -> C
        nodeA.ConnectTo(nodeB);
        nodeB.ConnectTo(nodeC);

        // Camino alternativo: A -> D -> C
        nodeA.ConnectTo(nodeD);
        nodeD.ConnectTo(nodeC);

        // Crear un predicado que no permite pasar por B
        var predicate = new Func<MazeNode, bool>(node => node != nodeB);
        var path = nodeA.FindShortestPath(nodeC, PathWeightMode.Both, predicate);

        Assert.Multiple(() => {
            Assert.That(path, Has.Count.EqualTo(3));
            Assert.That(path[0], Is.EqualTo(nodeA));
            Assert.That(path[1], Is.EqualTo(nodeD)); // Debe usar el camino alternativo
            Assert.That(path[2], Is.EqualTo(nodeC));
        });
    }

    [Test]
    public void GetReachableNodes_WithPredicate_OnlyReturnsValidNodes() {
        var nodeA = _graph.CreateNode(new Vector2I(0, 0));
        var nodeB = _graph.CreateNode(new Vector2I(1, 0));
        var nodeC = _graph.CreateNode(new Vector2I(2, 0));

        // Crear camino: A -> B -> C
        nodeA.ConnectTo(nodeB);
        nodeB.ConnectTo(nodeC);

        // Solo permitir nodos con coordenada x < 2
        var predicate = new Func<MazeNode, bool>(node => node.Position.X < 2);
        var reachableNodes = nodeA.GetReachableNodes(predicate);

        Assert.Multiple(() => {
            Assert.That(reachableNodes, Has.Count.EqualTo(2));
            Assert.That(reachableNodes, Contains.Item(nodeA));
            Assert.That(reachableNodes, Contains.Item(nodeB));
            Assert.That(reachableNodes, Does.Not.Contain(nodeC));
        });
    }
}