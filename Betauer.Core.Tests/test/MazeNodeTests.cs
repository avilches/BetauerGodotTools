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
            Assert.That(backwardPath, Is.Null);
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
            Assert.That(nodeA.GetDistanceToNodeByEdges(nodeC), Is.EqualTo(2));
            
            // Distancia hacia atrás debe ser -1 (no hay camino)
            Assert.That(nodeC.GetDistanceToNodeByEdges(nodeA), Is.EqualTo(-1));
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


}