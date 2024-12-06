using System.Linq;
using Betauer.Core.PCG.Maze;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class MazeNodeWeightedPathTests {
    private MazeGraph<object> _graph = null!;
    private MazeNode<object> _node = null!;

    [SetUp]
    public void Setup() {
        _graph = new MazeGraph<object>(10, 10);
        _node = _graph.CreateNode(new Vector2I(1, 1));
    }

    [Test]
    public void FindWeightedPath_WithEdgeWeights_ReturnsShortestPath() {
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
        nodeA.ConnectTo(nodeB).Weight = 5;
        nodeB.ConnectTo(nodeC).Weight = 1;

        // Ruta corta con peso total 3
        nodeA.ConnectTo(nodeD).Weight = 2;
        nodeD.ConnectTo(nodeC).Weight = 1;

        var result = nodeA.FindWeightedPath(nodeC, PathWeightMode.EdgesOnly);

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
    public void FindWeightedPath_WithEqualEdgeWeights_BehavesLikeBFS() {
        var nodeA = _graph.CreateNode(new Vector2I(0, 0));
        var nodeB = _graph.CreateNode(new Vector2I(1, 0));
        var nodeC = _graph.CreateNode(new Vector2I(2, 0));

        // Todos los edges tienen peso 1 (explícitamente)
        nodeA.ConnectTo(nodeB).Weight = 1;
        nodeB.ConnectTo(nodeC).Weight = 1;

        var result = nodeA.FindWeightedPath(nodeC, PathWeightMode.EdgesOnly);

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
    public void FindWeightedPath_NoPath_ReturnsNull() {
        var nodeA = _graph.CreateNode(new Vector2I(0, 0));
        var nodeB = _graph.CreateNode(new Vector2I(1, 0));

        var result = nodeA.FindWeightedPath(nodeB, PathWeightMode.EdgesOnly);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void FindWeightedPath_ToSelf_ReturnsZeroCostPath() {
        var nodeA = _graph.CreateNode(new Vector2I(0, 0));

        var result = nodeA.FindWeightedPath(nodeA, PathWeightMode.EdgesOnly);

        Assert.Multiple(() => {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Path, Has.Count.EqualTo(1));
            Assert.That(result.Path[0], Is.EqualTo(nodeA));
            Assert.That(result.TotalCost, Is.EqualTo(0));
        });
    }

    [Test]
    public void FindWeightedPath_ThreeModesProduceDifferentResults() {
        var nodeA = _graph.GetOrCreateNode(new Vector2I(0, 0));
        var nodeB = _graph.GetOrCreateNode(new Vector2I(1, 0));
        var nodeC = _graph.GetOrCreateNode(new Vector2I(2, 0));
        var nodeD = _graph.GetOrCreateNode(new Vector2I(1, 1));

        // Configuración:
        // Ruta superior: A -> B -> C
        // - Edge AB = 1, B.Weight = 5, Edge BC = 1, C.Weight = 1
        // Ruta inferior: A -> D -> C
        // - Edge AD = 2, D.Weight = 1, Edge DC = 2, C.Weight = 1

        nodeA.ConnectTo(nodeB).Weight = 1;
        nodeB.Weight = 5;
        nodeB.ConnectTo(nodeC).Weight = 1;
        nodeC.Weight = 1;

        nodeA.ConnectTo(nodeD).Weight = 2;
        nodeD.Weight = 1;
        nodeD.ConnectTo(nodeC).Weight = 2;

        var resultNodesOnly = nodeA.FindWeightedPath(nodeC, PathWeightMode.NodesOnly);
        var resultEdgesOnly = nodeA.FindWeightedPath(nodeC, PathWeightMode.EdgesOnly);
        var resultBoth = nodeA.FindWeightedPath(nodeC);

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
    public void FindWeightedPath_StartNodeWeight_HandlesDifferentlyForEachMode() {
        var nodeA = _graph.GetOrCreateNode(new Vector2I(0, 0));
        var nodeB = _graph.GetOrCreateNode(new Vector2I(1, 0));

        nodeA.Weight = 5;
        var edge = nodeA.ConnectTo(nodeB);
        edge.Weight = 1;
        nodeB.Weight = 1;

        var resultNodesOnly = nodeA.FindWeightedPath(nodeB, PathWeightMode.NodesOnly);
        var resultEdgesOnly = nodeA.FindWeightedPath(nodeB, PathWeightMode.EdgesOnly);
        var resultBoth = nodeA.FindWeightedPath(nodeB);

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
    public void FindWeightedPath_WhenAllWeightsAreZero_PrefersShorterPaths() {
        var nodeA = _graph.GetOrCreateNode(new Vector2I(0, 0));
        var nodeB = _graph.GetOrCreateNode(new Vector2I(1, 0));
        var nodeC = _graph.GetOrCreateNode(new Vector2I(2, 0));
        var nodeD = _graph.GetOrCreateNode(new Vector2I(1, 1));
        var nodeE = _graph.GetOrCreateNode(new Vector2I(1, 2));

        // Ruta corta: A -> B -> C
        nodeA.ConnectTo(nodeB);
        nodeB.ConnectTo(nodeC);

        // Ruta larga: A -> D -> E -> C
        nodeA.ConnectTo(nodeD);
        nodeD.ConnectTo(nodeE);
        nodeE.ConnectTo(nodeC);

        var resultNodesOnly = nodeA.FindWeightedPath(nodeC, PathWeightMode.NodesOnly);
        var resultEdgesOnly = nodeA.FindWeightedPath(nodeC, PathWeightMode.EdgesOnly);
        var resultBoth = nodeA.FindWeightedPath(nodeC);

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
    
        var edge = nodeA.ConnectTo(nodeB);
    
        Assert.That(edge.Weight, Is.EqualTo(0f));
    }

    [Test]
    public void FindWeightedPath_UnweightedEdgesHaveZeroCost() {
        var nodeA = _graph.GetOrCreateNode(new Vector2I(0, 0));
        var nodeB = _graph.GetOrCreateNode(new Vector2I(1, 0));
        var nodeC = _graph.GetOrCreateNode(new Vector2I(2, 0));

        // No establecemos pesos explícitamente
        nodeA.ConnectTo(nodeB);
        nodeB.ConnectTo(nodeC);

        var result = nodeA.FindWeightedPath(nodeC, PathWeightMode.EdgesOnly);

        Assert.Multiple(() => {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.TotalCost, Is.EqualTo(0));
            Assert.That(result.GetEdgesCost(), Is.EqualTo(0));
        });
    }
    
    [Test]
    public void FindWeightedPath_DirectionalEdges_RespectsEdgeDirection() {
        var nodeA = _graph.CreateNode(new Vector2I(0, 0));
        var nodeB = _graph.CreateNode(new Vector2I(1, 0));
        var nodeC = _graph.CreateNode(new Vector2I(2, 0));

        // Crear un camino direccional con pesos: A -> B -> C
        nodeA.ConnectTo(nodeB, weight: 1);
        nodeB.ConnectTo(nodeC, weight: 2);

        var forwardPath = nodeA.FindWeightedPath(nodeC, PathWeightMode.EdgesOnly);
        var backwardPath = nodeC.FindWeightedPath(nodeA, PathWeightMode.EdgesOnly);

        Assert.Multiple(() => {
            // Camino hacia adelante debe existir y tener coste 3
            Assert.That(forwardPath, Is.Not.Null);
            Assert.That(forwardPath!.Path, Has.Count.EqualTo(3));
            Assert.That(forwardPath.TotalCost, Is.EqualTo(3));

            // Camino hacia atrás no debe existir
            Assert.That(backwardPath, Is.Null);
        });
    }
}