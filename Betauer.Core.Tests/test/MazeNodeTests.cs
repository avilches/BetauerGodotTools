using System.Linq;
using Betauer.Core.PCG.Maze;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
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
    public void GetPathToNodeByEdges_WhenSameNode_ReturnsSingleNodePath() {
        var path = _node.GetPathToNodeByEdges(_node);

        Assert.Multiple(() => {
            Assert.That(path, Is.Not.Null);
            Assert.That(path, Has.Count.EqualTo(1));
            Assert.That(path![0], Is.EqualTo(_node));
        });
    }

    [Test]
    public void GetPathToNodeByEdges_DirectConnection_ReturnsCorrectPath() {
        var nodeA = _graph.CreateNode(new Vector2I(0, 0));
        var nodeB = _graph.CreateNode(new Vector2I(1, 0));

        nodeA.AddEdgeTo(nodeB);

        var path = nodeA.GetPathToNodeByEdges(nodeB);

        Assert.Multiple(() => {
            Assert.That(path, Is.Not.Null);
            Assert.That(path, Has.Count.EqualTo(2));
            Assert.That(path![0], Is.EqualTo(nodeA));
            Assert.That(path![1], Is.EqualTo(nodeB));
        });
    }

    [Test]
    public void GetPathToNodeByEdges_ComplexPath_ReturnsShortestPath() {
        var nodeA = _graph.GetOrCreateNode(new Vector2I(0, 0));
        var nodeB = _graph.GetOrCreateNode(new Vector2I(1, 0));
        var nodeC = _graph.GetOrCreateNode(new Vector2I(2, 0));
        var nodeD = _graph.GetOrCreateNode(new Vector2I(1, 1));

        // Create a diamond shape: A -> B -> C and A -> D -> C
        nodeA.AddEdgeTo(nodeB);
        nodeB.AddEdgeTo(nodeC);
        nodeA.AddEdgeTo(nodeD);
        nodeD.AddEdgeTo(nodeC);

        var path = nodeA.GetPathToNodeByEdges(nodeC);

        Assert.Multiple(() => {
            Assert.That(path, Is.Not.Null);
            Assert.That(path, Has.Count.EqualTo(3));
            Assert.That(path![0], Is.EqualTo(nodeA));
            // No importa qué camino tome (por B o por D) mientras la longitud sea 3
            Assert.That(path![2], Is.EqualTo(nodeC));
        });
    }

    [Test]
    public void GetPathToNodeByEdges_NoPath_ReturnsNull() {
        var nodeA = _graph.CreateNode(new Vector2I(0, 0));
        var nodeB = _graph.CreateNode(new Vector2I(1, 0));

        // No edges between nodes
        var path = nodeA.GetPathToNodeByEdges(nodeB);

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

    [Test]
    public void GetShortestPathByEdges_WithWeights_ReturnsShortestPath() {
        var nodeA = _graph.GetOrCreateNode(new Vector2I(0, 0));
        var nodeB = _graph.GetOrCreateNode(new Vector2I(1, 0));
        var nodeC = _graph.GetOrCreateNode(new Vector2I(2, 0));
        var nodeD = _graph.GetOrCreateNode(new Vector2I(1, 1));

        // Crear un grafo con pesos:
        //      B(5)--C
        //     /       |
        // A--+        |
        //     \       |
        //      D(2)---+(1)

        // Ruta larga con peso total 6
        nodeA.AddEdgeTo(nodeB).Weight = 5;
        nodeB.AddEdgeTo(nodeC).Weight = 1;

        // Ruta corta con peso total 3
        nodeA.AddEdgeTo(nodeD).Weight = 2;
        nodeD.AddEdgeTo(nodeC).Weight = 1;

        var result = nodeA.GetShortestPathByEdges(nodeC);

        Assert.Multiple(() => {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Path, Has.Count.EqualTo(3));
            Assert.That(result.Path[0], Is.EqualTo(nodeA));
            Assert.That(result.Path[1], Is.EqualTo(nodeD)); // Toma el camino por D que es más corto
            Assert.That(result.Path[2], Is.EqualTo(nodeC));
            Assert.That(result.TotalCost, Is.EqualTo(3));
        });
    }

    [Test]
    public void GetShortestPathByEdges_WithEqualWeights_BehavesLikeBFS() {
        var nodeA = _graph.CreateNode(new Vector2I(0, 0));
        var nodeB = _graph.CreateNode(new Vector2I(1, 0));
        var nodeC = _graph.CreateNode(new Vector2I(2, 0));

        // Todos los edges tienen peso 1 (explícitamente)
        nodeA.AddEdgeTo(nodeB).Weight = 1;
        nodeB.AddEdgeTo(nodeC).Weight = 1;

        var result = nodeA.GetShortestPathByEdges(nodeC);

        Assert.Multiple(() => {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Path, Has.Count.EqualTo(3));
            Assert.That(result.Path[0], Is.EqualTo(nodeA));
            Assert.That(result.Path[1], Is.EqualTo(nodeB));
            Assert.That(result.Path[2], Is.EqualTo(nodeC));
            Assert.That(result.TotalCost, Is.EqualTo(2)); // 2 edges de peso 1
        });
    }

    [Test]
    public void GetShortestPathByEdges_NoPath_ReturnsNull() {
        var nodeA = _graph.CreateNode(new Vector2I(0, 0));
        var nodeB = _graph.CreateNode(new Vector2I(1, 0));

        var result = nodeA.GetShortestPathByEdges(nodeB);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetShortestPathByEdges_ToSelf_ReturnsZeroCostPath() {
        var nodeA = _graph.CreateNode(new Vector2I(0, 0));

        var result = nodeA.GetShortestPathByEdges(nodeA);

        Assert.Multiple(() => {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Path, Has.Count.EqualTo(1));
            Assert.That(result.Path[0], Is.EqualTo(nodeA));
            Assert.That(result.TotalCost, Is.EqualTo(0));
        });
    }

    [Test]
    public void GetShortestPath_ThreeModesProduceDifferentResults() {
        var nodeA = _graph.GetOrCreateNode(new Vector2I(0, 0));
        var nodeB = _graph.GetOrCreateNode(new Vector2I(1, 0));
        var nodeC = _graph.GetOrCreateNode(new Vector2I(2, 0));
        var nodeD = _graph.GetOrCreateNode(new Vector2I(1, 1));

        // Configuración:
        // Ruta superior: A -> B -> C
        // - Edge AB = 1, B.Weight = 5, Edge BC = 1, C.Weight = 1
        // Ruta inferior: A -> D -> C
        // - Edge AD = 2, D.Weight = 1, Edge DC = 2, C.Weight = 1

        nodeA.AddEdgeTo(nodeB).Weight = 1;
        nodeB.Weight = 5;
        nodeB.AddEdgeTo(nodeC).Weight = 1;
        nodeC.Weight = 1;

        nodeA.AddEdgeTo(nodeD).Weight = 2;
        nodeD.Weight = 1;
        nodeD.AddEdgeTo(nodeC).Weight = 2;

        var resultNodesOnly = nodeA.GetShortestPathByNodeWeights(nodeC);
        var resultEdgesOnly = nodeA.GetShortestPathByEdges(nodeC);
        var resultBoth = nodeA.GetShortestPathByWeights(nodeC);

        Assert.Multiple(() => {
            // NodesOnly: Preferirá la ruta inferior A->D->C (menor peso de nodos)
            Assert.That(resultNodesOnly!.Path[1], Is.EqualTo(nodeD));
            Assert.That(resultNodesOnly.GetNodesCost(), Is.EqualTo(2)); // D(1) + C(1)

            // EdgesOnly: Preferirá la ruta superior A->B->C (edges más baratos)
            Assert.That(resultEdgesOnly!.Path[1], Is.EqualTo(nodeB));
            Assert.That(resultEdgesOnly.GetEdgesCost(), Is.EqualTo(2)); // AB(1) + BC(1)

            // Both: Preferirá la ruta inferior A->D->C (mejor balance)
            Assert.That(resultBoth!.Path[1], Is.EqualTo(nodeD));
            Assert.That(resultBoth.TotalCost, Is.EqualTo(6)); // Edges(4) + Nodes(2)
        });
    }

    [Test]
    public void GetShortestPath_StartNodeWeight_HandlesDifferentlyForEachMode() {
        var nodeA = _graph.GetOrCreateNode(new Vector2I(0, 0));
        var nodeB = _graph.GetOrCreateNode(new Vector2I(1, 0));

        nodeA.Weight = 5;
        var edge = nodeA.AddEdgeTo(nodeB);
        edge.Weight = 1;
        nodeB.Weight = 1;

        var resultNodesOnly = nodeA.GetShortestPathByNodeWeights(nodeB);
        var resultEdgesOnly = nodeA.GetShortestPathByEdges(nodeB);
        var resultBoth = nodeA.GetShortestPathByWeights(nodeB);

        Assert.Multiple(() => {
            // NodesOnly: Incluye los pesos de A y B
            Assert.That(resultNodesOnly!.TotalCost, Is.EqualTo(6)); // A(5) + B(1)

            // EdgesOnly: Solo incluye el peso del edge
            Assert.That(resultEdgesOnly!.TotalCost, Is.EqualTo(1)); // Edge(1)

            // Both: Incluye todos los pesos
            Assert.That(resultBoth!.TotalCost, Is.EqualTo(7)); // A(5) + Edge(1) + B(1)
        });
    }

    [Test]
    public void GetShortestPath_WhenAllWeightsAreZero_PrefersShorterPaths() {
        var nodeA = _graph.GetOrCreateNode(new Vector2I(0, 0));
        var nodeB = _graph.GetOrCreateNode(new Vector2I(1, 0));
        var nodeC = _graph.GetOrCreateNode(new Vector2I(2, 0));
        var nodeD = _graph.GetOrCreateNode(new Vector2I(1, 1));
        var nodeE = _graph.GetOrCreateNode(new Vector2I(1, 2));

        // Ruta corta: A -> B -> C
        nodeA.AddEdgeTo(nodeB);
        nodeB.AddEdgeTo(nodeC);

        // Ruta larga: A -> D -> E -> C
        nodeA.AddEdgeTo(nodeD);
        nodeD.AddEdgeTo(nodeE);
        nodeE.AddEdgeTo(nodeC);

        var resultNodesOnly = nodeA.GetShortestPathByNodeWeights(nodeC);
        var resultEdgesOnly = nodeA.GetShortestPathByEdges(nodeC);
        var resultBoth = nodeA.GetShortestPathByWeights(nodeC);

        Assert.Multiple(() => {
            // Todos los modos deberían preferir el camino más corto
            Assert.That(resultNodesOnly!.Path, Has.Count.EqualTo(3));
            Assert.That(resultEdgesOnly!.Path, Has.Count.EqualTo(3));
            Assert.That(resultBoth!.Path, Has.Count.EqualTo(3));

            // Y todos deberían tener coste 0
            Assert.That(resultNodesOnly.TotalCost, Is.EqualTo(0));
            Assert.That(resultEdgesOnly.TotalCost, Is.EqualTo(0));
            Assert.That(resultBoth.TotalCost, Is.EqualTo(0));
        });
    }
    
    [Test]
    public void EdgeWeight_DefaultIsZero() {
        var nodeA = _graph.GetOrCreateNode(new Vector2I(0, 0));
        var nodeB = _graph.GetOrCreateNode(new Vector2I(1, 0));
    
        var edge = nodeA.AddEdgeTo(nodeB);
    
        Assert.That(edge.Weight, Is.EqualTo(0f));
    }

    [Test]
    public void GetShortestPath_UnweightedEdgesHaveZeroCost() {
        var nodeA = _graph.GetOrCreateNode(new Vector2I(0, 0));
        var nodeB = _graph.GetOrCreateNode(new Vector2I(1, 0));
        var nodeC = _graph.GetOrCreateNode(new Vector2I(2, 0));

        // No establecemos pesos explícitamente
        nodeA.AddEdgeTo(nodeB);
        nodeB.AddEdgeTo(nodeC);

        var result = nodeA.GetShortestPathByEdges(nodeC);

        Assert.Multiple(() => {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.TotalCost, Is.EqualTo(0));
            Assert.That(result.GetEdgesCost(), Is.EqualTo(0));
        });
    }
}