using Betauer.Core.PCG.Maze;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class MazePathFinderTests {
    private MazeGraph graph;
    private MazeNode nodeA;
    private MazeNode nodeB;
    private MazeNode nodeC;
    private MazeNode nodeD;
    private MazeNode nodeE;
    private MazeNode nodeF;

    [SetUp]
    public void Setup() {
        graph = new MazeGraph();
            
        // Create nodes with positions and weights
        nodeA = graph.CreateNode(new Vector2I(0, 0), weight: 1);
        nodeB = graph.CreateNode(new Vector2I(1, 0), weight: 1);
        nodeC = graph.CreateNode(new Vector2I(2, 0), weight: 1);
        nodeD = graph.CreateNode(new Vector2I(0, 1), weight: 1);
        nodeE = graph.CreateNode(new Vector2I(1, 1), weight: 1);
        nodeF = graph.CreateNode(new Vector2I(2, 1), weight: 1);

        /*
         * Graph structure:
         * A --1--> B --1--> C
         * |        |
         * 10       1
         * |        |
         * D --1--> E --1--> F
         */

        // Create edges with weights
        graph.ConnectNodes(nodeA, nodeB, weight: 1);  // A -> B (weight 1)
        graph.ConnectNodes(nodeB, nodeC, weight: 1);  // B -> C (weight 1)
        graph.ConnectNodes(nodeA, nodeD, weight: 10); // A -> D (weight 10)
        graph.ConnectNodes(nodeB, nodeE, weight: 1);  // B -> E (weight 1)
        graph.ConnectNodes(nodeD, nodeE, weight: 1);  // D -> E (weight 1)
        graph.ConnectNodes(nodeE, nodeF, weight: 1);  // E -> F (weight 1)
    }

    [TearDown]
    public void TearDown() {
        graph.Clear();
    }

    [Test]
    public void FindWeightedPath_NoWeights_ShouldFindShortestPath() {
        var result = MazePathFinder.FindShortestPath(nodeA, nodeF, PathWeightMode.None);
            
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Path, Has.Count.EqualTo(4));
        Assert.That(result.Path, Is.EqualTo(new[] { nodeA, nodeB, nodeE, nodeF }));
        Assert.That(result.TotalCost, Is.EqualTo(3f)); // 3 edges
    }

    [Test]
    public void FindWeightedPath_EdgesOnly_ShouldFindPathWithLowestEdgeWeights() {
        var result = MazePathFinder.FindShortestPath(nodeA, nodeF, PathWeightMode.EdgesOnly);
            
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Path, Has.Count.EqualTo(4));
        Assert.That(result.Path, Is.EqualTo(new[] { nodeA, nodeB, nodeE, nodeF }));
        Assert.That(result.TotalCost, Is.EqualTo(3)); // 1 + 1 + 1 = 3
    }

    [Test]
    public void FindWeightedPath_NodesOnly_ShouldFindPathWithLowestNodeWeights() {
        // Set different node weights
        nodeB.Weight = 5;
        nodeE.Weight = 1;
            
        var result = MazePathFinder.FindShortestPath(nodeA, nodeF, PathWeightMode.NodesOnly);
            
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Path, Has.Count.EqualTo(4));
        Assert.That(result.Path, Is.EqualTo(new[] { nodeA, nodeD, nodeE, nodeF }));
        // Cost = nodeA(1) + nodeD(1) + nodeE(1) + nodeF(1) = 4
        Assert.That(result.TotalCost, Is.EqualTo(4));
    }

    [Test]
    public void FindWeightedPath_Both_ShouldConsiderBothWeights() {
        // Set weights to force path through D
        nodeB.Weight = 15; // Aumentamos el peso de B para que la ruta por D sea más favorable
    
        var result = MazePathFinder.FindShortestPath(nodeA, nodeF, PathWeightMode.Both);
    
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Path, Has.Count.EqualTo(4));
    
        // Use sequential asserts instead of collection equality
        Assert.Multiple(() => {
            Assert.That(result.Path[0], Is.EqualTo(nodeA), "First node should be A");
            Assert.That(result.Path[1], Is.EqualTo(nodeD), "Second node should be D");
            Assert.That(result.Path[2], Is.EqualTo(nodeE), "Third node should be E");
            Assert.That(result.Path[3], Is.EqualTo(nodeF), "Fourth node should be F");
        });

        // Ruta A->D->E->F
        // Cost = (A.Weight + edge_AD) + (D.Weight + edge_DE) + (E.Weight + edge_EF) + F.Weight
        // = (1 + 10) + (1 + 1) + (1 + 1) + 1 = 16
        Assert.That(result.TotalCost, Is.EqualTo(16));
    }

    [Test]
    public void FindWeightedPath_SameStartAndEnd_ShouldReturnSingleNodePath() {
        var result = MazePathFinder.FindShortestPath(nodeA, nodeA);
            
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Path, Has.Count.EqualTo(1));
        Assert.That(result.Path, Is.EqualTo(new[] { nodeA }));
        Assert.That(result.TotalCost, Is.EqualTo(1)); // Just the node weight
    }

    [Test]
    public void FindWeightedPath_SameStartAndEndNone_ShouldReturnSingleNodePath() {
        var result = MazePathFinder.FindShortestPath(nodeA, nodeA, PathWeightMode.None);
            
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Path, Has.Count.EqualTo(1));
        Assert.That(result.Path, Is.EqualTo(new[] { nodeA }));
        Assert.That(result.TotalCost, Is.EqualTo(0));
    }

    [Test]
    public void FindWeightedPath_SameStartAndEndEdgesOnly_ShouldReturnSingleNodePath() {
        var result = MazePathFinder.FindShortestPath(nodeA, nodeA, PathWeightMode.EdgesOnly);
            
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Path, Has.Count.EqualTo(1));
        Assert.That(result.Path, Is.EqualTo(new[] { nodeA }));
        Assert.That(result.TotalCost, Is.EqualTo(0));
    }

    [Test]
    public void FindWeightedPath_NoPathExists_ShouldReturnNull() {
        // Create an isolated node
        var isolatedNode = graph.CreateNode(new Vector2I(5, 5), weight: 1);
            
        var result = MazePathFinder.FindShortestPath(nodeA, isolatedNode);
            
        Assert.That(result.Path, Is.Empty);
        Assert.That(result.TotalCost, Is.EqualTo(-1));
    }

    [Test]
    public void FindWeightedPath_WithCanTraverse_ShouldRespectConstraint() {
        // Only allow traversal of nodes with even x coordinates
        bool CanTraverse(MazeNode node) => node.Position.X % 2 == 0;
            
        var result = MazePathFinder.FindShortestPath(nodeA, nodeF, PathWeightMode.None, CanTraverse);
            
        Assert.That(result.Path, Is.Empty);
        Assert.That(result.TotalCost, Is.EqualTo(-1));
    }
}