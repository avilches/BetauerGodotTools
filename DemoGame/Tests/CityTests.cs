using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;
using Veronenger.Game.Dungeon.GenCity;

namespace Veronenger.Tests;

[TestFixture]
public class CityTests {
    private City _city;

    [SetUp]
    public void Setup() {
        _city = new City(30, 30);
    }

    [Test]
    public void AddIntersection_ShouldCreateIntersection() {
        // Act
        var position = new Vector2I(5, 5);
        var intersection = _city.GetOrCreateIntersectionAt(position);

        // Assert
        Assert.IsNotNull(intersection);
        Assert.AreEqual(position, intersection.Position);
        Assert.Contains(intersection, _city.Intersections);
        Assert.AreEqual(intersection, _city.Data[position]);
    }

    [Test]
    public void CreatePath_ShouldCreatePathBetweenIntersections() {
        // Arrange
        var startPos = new Vector2I(5, 5);
        var endPos = new Vector2I(5, 10);

        // Act
        var paths = _city.CreatePath(startPos, endPos);

        // Assert
        Assert.AreEqual(1, paths.Count);
        var path = paths[0];
        Assert.AreEqual(Vector2I.Down, path.Direction);
        Assert.AreEqual(5, path.GetLength());

        var startIntersection = _city.Data[startPos] as Intersection;
        var endIntersection = _city.Data[endPos] as Intersection;

        Assert.IsNotNull(startIntersection);
        Assert.IsNotNull(endIntersection);
        Assert.AreEqual(startIntersection, path.Start);
        Assert.AreEqual(endIntersection, path.End);

        // Check that all path positions are correctly marked in the city Data
        for (int y = startPos.Y + 1; y < endPos.Y; y++) {
            Assert.AreEqual(path, _city.Data[new Vector2I(startPos.X, y)]);
        }
    }

    [Test]
    public void GetOrCreateIntersectionAt_ShouldSplitExistingPath() {
        // Arrange
        // Create a path from (5,10) to (15,10)
        var startPos = new Vector2I(5, 10);
        var endPos = new Vector2I(15, 10);
        var paths = _city.CreatePath(startPos, endPos);

        Assert.AreEqual(1, paths.Count);
        var originalPath = paths[0];

        // Verify we have 2 intersections before splitting
        Assert.AreEqual(2, _city.Intersections.Count);

        // Point in the middle of the path where we'll create an intersection
        var midPos = new Vector2I(10, 10);
        Assert.IsInstanceOf<Path>(_city.Data[midPos]);

        // Act
        var midIntersection = _city.GetOrCreateIntersectionAt(midPos);

        // Assert
        // Should have 3 intersections now
        Assert.AreEqual(3, _city.Intersections.Count);

        // The original path should still exist (but now it's shorter)
        var allPaths = _city.GetAllPaths().ToList();
        Assert.IsTrue(allPaths.Contains(originalPath));

        // Should now have 2 paths
        Assert.AreEqual(2, allPaths.Count);

        // The middle intersection should be at the specified position
        Assert.AreEqual(midPos, midIntersection.Position);
        Assert.IsNotNull(_city.Data[midPos]);
        Assert.IsInstanceOf<Intersection>(_city.Data[midPos]);

        // The middle intersection should connect to both paths
        Assert.AreEqual(2, midIntersection.GetAllPaths().Count);

        // First path: from start to middle
        var path1 = _city.Data[startPos] as Intersection;
        Assert.IsNotNull(path1);
        var pathFromStart = path1.GetOutputPaths()[0];
        Assert.AreEqual(startPos, pathFromStart.Start.Position);
        Assert.AreEqual(midPos, pathFromStart.End.Position);
        Assert.AreEqual(Vector2I.Right, pathFromStart.Direction);

        // Second path: from middle to end
        var path2 = midIntersection.GetOutputPaths()[0];
        Assert.AreEqual(midPos, path2.Start.Position);
        Assert.AreEqual(endPos, path2.End.Position);
        Assert.AreEqual(Vector2I.Right, path2.Direction);

        // Verify all positions in the grid contain the correct objects
        // Start position should be an intersection
        Assert.IsInstanceOf<Intersection>(_city.Data[startPos]);
        // Middle position should be an intersection
        Assert.IsInstanceOf<Intersection>(_city.Data[midPos]);
        // End position should be an intersection
        Assert.IsInstanceOf<Intersection>(_city.Data[endPos]);
        // Positions between start and middle should be the first path
        Assert.AreEqual(pathFromStart, _city.Data[new Vector2I(7, 10)]);
        // Positions between middle and end should be the second path
        Assert.AreEqual(path2, _city.Data[new Vector2I(12, 10)]);
    }

    [Test]
    public void CreatePath_ShouldReuseExistingIntersection_WhenPathPassesThroughIt() {
        // Arrange
        // Create an intersection at position (10, 10)
        var intersectionPos = new Vector2I(10, 10);
        var existingIntersection = _city.GetOrCreateIntersectionAt(intersectionPos);

        // Remember the ID to verify it's the same intersection later
        int originalIntersectionId = existingIntersection.Id;

        // Define start and end points for a path that passes through the existing intersection
        var startPos = new Vector2I(5, 10); // Left of the intersection
        var endPos = new Vector2I(15, 10); // Right of the intersection

        // Act
        var paths = _city.CreatePath(startPos, endPos);

        // Assert
        // Should create two paths (one from start to intersection, one from intersection to end)
        Assert.AreEqual(2, paths.Count);

        // Verify the intersection at (10, 10) is still the same object (reused)
        var intersectionAfter = _city.Data[intersectionPos] as Intersection;
        Assert.IsNotNull(intersectionAfter);
        Assert.AreEqual(originalIntersectionId, intersectionAfter.Id);

        // Verify the paths are correctly connected to the intersection
        var pathFromStart = paths[0];
        var pathToEnd = paths[1];

        // First path: from start to intersection
        Assert.AreEqual(startPos, pathFromStart.Start.Position);
        Assert.AreEqual(intersectionPos, pathFromStart.End.Position);
        Assert.AreEqual(Vector2I.Right, pathFromStart.Direction);

        // Second path: from intersection to end
        Assert.AreEqual(intersectionPos, pathToEnd.Start.Position);
        Assert.AreEqual(endPos, pathToEnd.End.Position);
        Assert.AreEqual(Vector2I.Right, pathToEnd.Direction);

        // Verify the intersection has the correct connections
        Assert.AreEqual(2, intersectionAfter.GetAllPaths().Count); // Should have 3 connected paths
        Assert.Contains(pathFromStart, intersectionAfter.GetInputPaths());
        Assert.Contains(pathToEnd, intersectionAfter.GetOutputPaths());
    }

    [Test]
    public void CreatePath_ShouldCreateIntersection_WhenPassingThroughExistingPath() {
        // Arrange
        // First create a vertical path
        var verticalStart = new Vector2I(10, 5);
        var verticalEnd = new Vector2I(10, 15);
        var verticalPaths = _city.CreatePath(verticalStart, verticalEnd);
        Assert.AreEqual(1, verticalPaths.Count);
        var verticalPath = verticalPaths[0];

        // The point where the horizontal path will cross the vertical path
        var crossingPoint = new Vector2I(10, 10);

        // Define start and end points for a horizontal path that crosses the vertical one
        var horizStart = new Vector2I(5, 10);
        var horizEnd = new Vector2I(15, 10);

        // Act
        var horizPaths = _city.CreatePath(horizStart, horizEnd);

        // Assert
        // A new intersection should have been created at the crossing point
        // 2 intersections per path (start/end) + 1 in the cross fork = 6 total
        Assert.AreEqual(5, _city.Intersections.Count);

        // Verify there's an intersection at the crossing point
        var intersection = _city.Data[crossingPoint] as Intersection;
        Assert.IsNotNull(intersection);

        // The original vertical path still remains, but now it's shorter
        Assert.IsTrue(_city.GetAllPaths().Contains(verticalPath));
        Assert.AreEqual(verticalPath.End.Position, crossingPoint);

        // there should now be 4 paths (2 vertical + 2 horizontal)
        var allPaths = _city.GetAllPaths().ToList();
        Assert.AreEqual(4, allPaths.Count);

        // The horizontal path should have been split into two
        Assert.AreEqual(2, horizPaths.Count);

        // The intersection should have 4 connected paths
        Assert.AreEqual(4, intersection.GetAllPaths().Count);

        Assert.IsTrue(intersection.Up != null);
        Assert.IsTrue(intersection.Down != null);
        Assert.IsTrue(intersection.Right != null);
        Assert.IsTrue(intersection.Left != null);

        // Verify data cells contain the correct path segments
        // One cell to the left of crossing
        var pathLeft = _city.Data[new Vector2I(9, 10)] as Path;
        Assert.IsNotNull(pathLeft);
        Assert.AreEqual(Vector2I.Right, pathLeft.Direction);

        // One cell to the right of crossing
        var pathRight = _city.Data[new Vector2I(11, 10)] as Path;
        Assert.IsNotNull(pathRight);
        Assert.AreEqual(Vector2I.Right, pathRight.Direction);

        // One cell above crossing
        var pathUp = _city.Data[new Vector2I(10, 9)] as Path;
        Assert.IsNotNull(pathUp);
        Assert.AreEqual(Vector2I.Down, pathUp.Direction);

        // One cell below crossing
        var pathDown = _city.Data[new Vector2I(10, 11)] as Path;
        Assert.IsNotNull(pathDown);
        Assert.AreEqual(Vector2I.Down, pathDown.Direction);
    }

    [Test]
    public void RemovePath_ShouldRemovePathAndIntersectionsWhenEmpty() {
        // Arrange
        var startPos = new Vector2I(5, 5);
        var endPos = new Vector2I(5, 10);
        var paths = _city.CreatePath(startPos, endPos);
        var path = paths[0];

        // Act
        _city.RemovePath(path);

        // Assert
        // Both intersections should be removed since they have no more paths
        Assert.IsNull(_city.Data[startPos]);
        Assert.IsNull(_city.Data[endPos]);

        // No intersections should remain in the city
        Assert.IsEmpty(_city.Intersections);

        // All path positions should be cleared in the city Data
        for (int y = startPos.Y; y <= endPos.Y; y++) {
            Assert.IsNull(_city.Data[new Vector2I(startPos.X, y)]);
        }
    }

    [Test]
    public void RemovePath_ShouldOnlyRemoveEmptyIntersections() {
        // Arrange - Create a T-junction with three paths
        var centerPos = new Vector2I(10, 10);
        var leftPos = new Vector2I(5, 10);
        var rightPos = new Vector2I(15, 10);
        var bottomPos = new Vector2I(10, 15);

        _city.CreatePath(leftPos, centerPos); // Path 1
        _city.CreatePath(centerPos, rightPos); // Path 2
        _city.CreatePath(centerPos, bottomPos); // Path 3

        var centerIntersection = _city.Data[centerPos] as Intersection;
        Assert.AreEqual(3, centerIntersection.GetAllPaths().Count);

        // Find the path from center to right
        var pathToRight = centerIntersection.FindPathTo(Vector2I.Right);
        Assert.IsNotNull(pathToRight);

        // Act
        _city.RemovePath(pathToRight);

        // Assert
        // Center intersection should remain as it still has 2 paths
        Assert.IsNotNull(_city.Data[centerPos]);
        Assert.IsTrue(_city.Intersections.Contains(centerIntersection));
        Assert.AreEqual(2, centerIntersection.GetAllPaths().Count);

        // Right intersection should be removed as it has no paths
        Assert.IsNull(_city.Data[rightPos]);
        Assert.IsFalse(_city.Intersections.Any(i => i.Position == rightPos));

        // Left and bottom intersections should remain
        Assert.IsNotNull(_city.Data[leftPos]);
        Assert.IsNotNull(_city.Data[bottomPos]);

        // Path positions should be updated
        for (int x = centerPos.X + 1; x < rightPos.X; x++) {
            Assert.IsNull(_city.Data[new Vector2I(x, centerPos.Y)]);
        }
    }

    [Test]
    public void CreateCrossingPaths_ShouldCreateFourPathsWithIntersection() {
        // Arrange
        var horizStartPos = new Vector2I(3, 7);
        var horizEndPos = new Vector2I(15, 7);
        var vertStartPos = new Vector2I(10, 3);
        var vertEndPos = new Vector2I(10, 15);
        var crossingPoint = new Vector2I(10, 7);

        // Act - Create first path
        var horizPaths = _city.CreatePath(horizStartPos, horizEndPos);

        // Assert - First path should be created correctly
        Assert.AreEqual(1, horizPaths.Count);
        var horizPath = horizPaths[0];
        Assert.AreEqual(horizPath, _city.Data[new Vector2I(7, 7)]);

        // Act - Create second path that crosses the first
        var vertPaths = _city.CreatePath(vertStartPos, vertEndPos);

        // Assert - Should now have 4 paths total and an intersection at the crossing point
        var allPaths = _city.GetAllPaths().ToList();
        Assert.AreEqual(4, allPaths.Count);

        // Check intersection at crossing point
        var intersection = _city.Data[crossingPoint] as Intersection;
        Assert.IsNotNull(intersection);
        Assert.AreEqual(4, intersection.GetAllPaths().Count);
        Assert.AreEqual(2, intersection.GetInputPaths().Count);
        Assert.AreEqual(2, intersection.GetOutputPaths().Count);

        // Verify paths connect to the intersection
        var pathsFromIntersection = intersection.GetAllPaths();
        Assert.AreEqual(4, pathsFromIntersection.Count);

        // Verify directions
        Assert.IsTrue(intersection.Up != null);
        Assert.IsTrue(intersection.Down != null);
        Assert.IsTrue(intersection.Right != null);
        Assert.IsTrue(intersection.Left != null);
    }

    [Test]
    public void FlatIntersection_WithTwoIncomingPaths_ShouldFlattenCorrectly() {
        // Arrange - Create a horizontal path with an intersection in the middle
        var leftPos = new Vector2I(5, 10);
        var middlePos = new Vector2I(10, 10);
        var rightPos = new Vector2I(15, 10);

        _city.CreatePath(leftPos, middlePos);
        _city.CreatePath(rightPos, middlePos);

        var middleIntersection = _city.Data[middlePos] as Intersection;
        Assert.IsNotNull(middleIntersection);
        Assert.AreEqual(2, middleIntersection.GetInputPaths().Count);
        Assert.AreEqual(0, middleIntersection.GetOutputPaths().Count);

        // Act
        bool result = _city.FlatIntersection(middleIntersection);

        // Assert
        Assert.IsTrue(result);
        Assert.IsFalse(_city.Intersections.Contains(middleIntersection));

        // The intersection is removed but the position now contains the path
        Assert.IsInstanceOf<Path>(_city.Data[middlePos]);

        // Should now have a direct path between left and right
        var leftIntersection = _city.Data[leftPos] as Intersection;
        var rightIntersection = _city.Data[rightPos] as Intersection;

        Assert.IsNotNull(leftIntersection.FindPathTo(Vector2I.Right));
        Assert.IsNotNull(rightIntersection.FindPathTo(Vector2I.Left));

        // Path tiles should be filled
        for (int x = leftPos.X + 1; x < rightPos.X; x++) {
            Assert.IsInstanceOf<Path>(_city.Data[new Vector2I(x, leftPos.Y)]);
        }
    }

    [Test]
    public void FlatIntersection_WithTwoOutgoingPaths_ShouldFlattenCorrectly() {
        // Arrange - Create a horizontal path with an intersection in the middle
        var leftPos = new Vector2I(5, 10);
        var middlePos = new Vector2I(10, 10);
        var rightPos = new Vector2I(15, 10);

        // Create outgoing paths from middle to left and right
        var middleIntersection = _city.GetOrCreateIntersectionAt(middlePos);
        var pathToLeft = middleIntersection.CreatePathTo(Vector2I.Left);
        pathToLeft.SetEnd(_city.GetOrCreateIntersectionAt(leftPos));

        var pathToRight = middleIntersection.CreatePathTo(Vector2I.Right);
        pathToRight.SetEnd(_city.GetOrCreateIntersectionAt(rightPos));

        Assert.AreEqual(0, middleIntersection.GetInputPaths().Count);
        Assert.AreEqual(2, middleIntersection.GetOutputPaths().Count);

        // Act
        bool result = _city.FlatIntersection(middleIntersection);

        // Assert
        Assert.IsTrue(result);
        Assert.IsFalse(_city.Intersections.Contains(middleIntersection));

        // The intersection is removed but the position now contains the path
        Assert.IsInstanceOf<Path>(_city.Data[middlePos]);

        // Should now have a direct path between left and right
        var leftIntersection = _city.Data[leftPos] as Intersection;
        var rightIntersection = _city.Data[rightPos] as Intersection;

        Assert.IsNotNull(leftIntersection.FindPathTo(Vector2I.Right));
        Assert.IsNotNull(rightIntersection.FindPathTo(Vector2I.Left));
    }

    [Test]
    public void FlatIntersection_WithOneIncomingAndOneOutgoingPath_ShouldFlattenCorrectly() {
        // Arrange - Create a horizontal path with an intersection in the middle
        var leftPos = new Vector2I(5, 10);
        var middlePos = new Vector2I(10, 10);
        var rightPos = new Vector2I(15, 10);

        // Create incoming path from left and outgoing path to right
        _city.CreatePath(leftPos, middlePos);

        var middleIntersection = _city.Data[middlePos] as Intersection;
        var pathToRight = middleIntersection.CreatePathTo(Vector2I.Right);
        pathToRight.SetEnd(_city.GetOrCreateIntersectionAt(rightPos));

        Assert.AreEqual(1, middleIntersection.GetInputPaths().Count);
        Assert.AreEqual(1, middleIntersection.GetOutputPaths().Count);

        // Act
        bool result = _city.FlatIntersection(middleIntersection);

        // Assert
        Assert.IsTrue(result);
        Assert.IsFalse(_city.Intersections.Contains(middleIntersection));

        // The intersection is removed but the position now contains the path
        Assert.IsInstanceOf<Path>(_city.Data[middlePos]);

        // Should now have a direct path between left and right
        var leftIntersection = _city.Data[leftPos] as Intersection;
        var rightIntersection = _city.Data[rightPos] as Intersection;

        Assert.IsNotNull(leftIntersection.FindPathTo(Vector2I.Right));
        Assert.IsNotNull(rightIntersection.FindPathTo(Vector2I.Left));

        // One single path should connect all points
        var path = leftIntersection.GetOutputPaths()[0];
        Assert.AreEqual(rightIntersection, path.End);
        Assert.AreEqual(Vector2I.Right, path.Direction);
    }

    [Test]
    public void FlatIntersection_WithNonOppositeDirectionPaths_ShouldNotFlatten() {
        // Arrange - Create L-shaped paths with an intersection in the middle
        var middlePos = new Vector2I(10, 10);
        var leftPos = new Vector2I(5, 10);
        var bottomPos = new Vector2I(10, 15);

        _city.CreatePath(leftPos, middlePos);
        _city.CreatePath(bottomPos, middlePos);

        var middleIntersection = _city.Data[middlePos] as Intersection;

        // Act
        bool result = _city.FlatIntersection(middleIntersection);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNotNull(_city.Data[middlePos]);
        Assert.IsTrue(_city.Intersections.Contains(middleIntersection));

        // The original two paths should still exist
        Assert.AreEqual(2, middleIntersection.GetAllPaths().Count);
    }

    [Test]
    public void CreatePath() {
        _city = new City(20, 1);
        var render = new CityRender(_city);

        var paths = _city.CreatePath(new Vector2I(2, 0), new Vector2I(10, 0));
        Console.WriteLine(render.ToString());
        Assert.AreEqual("  ╞═══════╡         ", render.ToString());
        Assert.AreEqual(1, paths.Count);
        Assert.AreEqual(8, paths[0].GetLength());
        Assert.AreEqual(2, _city.Intersections.Count);
        Assert.AreEqual(1, _city.GetAllPaths().Count());
    }

    [Test]
    public void CreatePath_OnPoint() {
        _city = new City(20, 1);
        var render = new CityRender(_city);

        var paths = _city.CreatePath(new Vector2I(2, 0), new Vector2I(3, 0));
        Console.WriteLine(render.ToString());
        Assert.AreEqual("  ╞╡                ", render.ToString());
        Assert.AreEqual(1, paths.Count);
        Assert.AreEqual(1, paths[0].GetLength());
        Assert.AreEqual(2, _city.Intersections.Count);
        Assert.AreEqual(1, _city.GetAllPaths().Count());
    }

    [Test]
    public void CreatePath_OverlapsWithBiggerPath() {
        _city = new City(20, 1);
        var render = new CityRender(_city);

        _city.CreatePath(new Vector2I(2, 0), new Vector2I(10, 0));
        Console.WriteLine(render.ToString());
        Assert.AreEqual("  ╞═══════╡         ", render.ToString());
        var paths = _city.CreatePath(new Vector2I(0, 0), new Vector2I(12, 0));
        Console.WriteLine(render.ToString());
        Assert.AreEqual("╞═-═══════-═╡       ", render.ToString());
        Assert.AreEqual(2, paths.Count);
        Assert.AreEqual(4, _city.Intersections.Count);
        Assert.AreEqual(3, _city.GetAllPaths().Count());
    }

    [Test]
    public void CreatePath_OverlapsTotal() {
        _city = new City(20, 1);
        var render = new CityRender(_city);

        _city.CreatePath(new Vector2I(2, 0), new Vector2I(10, 0));
        Console.WriteLine(render.ToString());
        Assert.AreEqual("  ╞═══════╡         ", render.ToString());
        var paths = _city.CreatePath(new Vector2I(2, 0), new Vector2I(10, 0));
        Console.WriteLine(render.ToString());
        Assert.AreEqual("  ╞═══════╡         ", render.ToString());
        Assert.AreEqual(0, paths.Count);
        Assert.AreEqual(2, _city.Intersections.Count);
        Assert.AreEqual(1, _city.GetAllPaths().Count());
    }

    [Test]
    public void CreatePath_OverlapsWithOffsetLeft() {
        _city = new City(20, 1);
        var render = new CityRender(_city);

        _city.CreatePath(new Vector2I(2, 0), new Vector2I(10, 0));
        Console.WriteLine(render.ToString());
        Assert.AreEqual("  ╞═══════╡         ", render.ToString());
        var paths = _city.CreatePath(new Vector2I(0, 0), new Vector2I(8, 0));
        Console.WriteLine(render.ToString());
        Assert.AreEqual("╞═-═══════╡         ", render.ToString());
        Assert.AreEqual(1, paths.Count);
        Assert.AreEqual(3, _city.Intersections.Count);
        Assert.AreEqual(2, _city.GetAllPaths().Count());
    }

    [Test]
    public void CreatePath_ShareLeft() {
        _city = new City(20, 1);
        var render = new CityRender(_city);

        _city.CreatePath(new Vector2I(2, 0), new Vector2I(10, 0));
        Console.WriteLine(render.ToString());
        Assert.AreEqual("  ╞═══════╡         ", render.ToString());
        var paths = _city.CreatePath(new Vector2I(0, 0), new Vector2I(2, 0));
        Console.WriteLine(render.ToString());
        Assert.AreEqual("╞═-═══════╡         ", render.ToString());
        Assert.AreEqual(1, paths.Count);
        Assert.AreEqual(3, _city.Intersections.Count);
        Assert.AreEqual(2, _city.GetAllPaths().Count());
    }

    [Test]
    public void CreatePath_OverlapsWithOffsetRight() {
        _city = new City(20, 1);
        var render = new CityRender(_city);

        _city.CreatePath(new Vector2I(2, 0), new Vector2I(10, 0));
        Console.WriteLine(render.ToString());
        Assert.AreEqual("  ╞═══════╡         ", render.ToString());
        var paths = _city.CreatePath(new Vector2I(5, 0), new Vector2I(12, 0));
        Console.WriteLine(render.ToString());
        Assert.AreEqual("  ╞═══════-═╡       ", render.ToString());
        Assert.AreEqual(1, paths.Count);
        Assert.AreEqual(3, _city.Intersections.Count);
        Assert.AreEqual(2, _city.GetAllPaths().Count());
    }

    [Test]
    public void CreatePath_OverlapsShareRight() {
        _city = new City(20, 1);
        var render = new CityRender(_city);

        _city.CreatePath(new Vector2I(2, 0), new Vector2I(10, 0));
        Console.WriteLine(render.ToString());
        Assert.AreEqual("  ╞═══════╡         ", render.ToString());
        var paths = _city.CreatePath(new Vector2I(10, 0), new Vector2I(12, 0));
        Console.WriteLine(render.ToString());
        Assert.AreEqual("  ╞═══════-═╡       ", render.ToString());
        Assert.AreEqual(1, paths.Count);
        Assert.AreEqual(3, _city.Intersections.Count);
        Assert.AreEqual(2, _city.GetAllPaths().Count());
    }

    [Test]
    public void CreatePath_OverlapsWithSmallerPath() {
        _city = new City(20, 1);
        var render = new CityRender(_city);

        _city.CreatePath(new Vector2I(0, 0), new Vector2I(12, 0));
        Console.WriteLine(render.ToString());
        Assert.AreEqual("╞═══════════╡       ", render.ToString());
        var paths = _city.CreatePath(new Vector2I(0, 0), new Vector2I(12, 0));
        Console.WriteLine(render.ToString());
        Assert.AreEqual("╞═══════════╡       ", render.ToString());
        Assert.AreEqual(0, paths.Count);
        Assert.AreEqual(2, _city.Intersections.Count);
        Assert.AreEqual(1, _city.GetAllPaths().Count());
    }

    [Test]
    public void CreatePath_OverlapsMultiple() {
        _city = new City(20, 1);
        var render = new CityRender(_city);

        _city.CreatePath(new Vector2I(1, 0), new Vector2I(2, 0));
        _city.CreatePath(new Vector2I(3, 0), new Vector2I(4, 0));
        _city.CreatePath(new Vector2I(8, 0), new Vector2I(10, 0));
        Console.WriteLine(render.ToString());
        Assert.AreEqual(" ╞╡╞╡   ╞═╡         ", render.ToString());
        var paths = _city.CreatePath(new Vector2I(0, 0), new Vector2I(12, 0));
        Console.WriteLine(render.ToString());
        Assert.AreEqual("╞----═══-═-═╡       ", render.ToString());
        Assert.AreEqual(4, paths.Count);
        Assert.AreEqual(8, _city.Intersections.Count);
        Assert.AreEqual(7, _city.GetAllPaths().Count());
    }

    [Test]
    public void FlatAll() {
        _city = new City(20, 3);
        var render = new CityRender(_city);

        _city.CreatePath(new Vector2I(1, 0), new Vector2I(2, 0));
        _city.CreatePath(new Vector2I(2, 0), new Vector2I(3, 0));
        _city.CreatePath(new Vector2I(3, 0), new Vector2I(4, 0));
        _city.CreatePath(new Vector2I(8, 0), new Vector2I(10, 0));
        _city.CreatePath(new Vector2I(0, 0), new Vector2I(12, 0));

        _city.CreatePath(new Vector2I(3, 0), new Vector2I(3, 1));
        _city.CreatePath(new Vector2I(3, 1), new Vector2I(3, 2));
        Console.WriteLine(render.ToString());
        Assert.AreEqual("╞--╦-═══-═-═╡       \n" +
                        "   |                \n" +
                        "   ╨                ", render.ToString());

        _city.FlatAllIntersections();
        Console.WriteLine(render.ToString());
        Assert.AreEqual("╞══╦════════╡       \n" +
                        "   ║                \n" +
                        "   ╨                ", render.ToString());
    }

    [Test]
    public void CreatePath_OverlapsPerpendicular() {
        _city = new City(20, 3);
        var render = new CityRender(_city);

        _city.CreatePath(new Vector2I(3, 1), new Vector2I(3, 2));
        _city.CreatePath(new Vector2I(5, 0), new Vector2I(5, 2));
        _city.CreatePath(new Vector2I(7, 0), new Vector2I(7, 1));
        Console.WriteLine(render.ToString());
        Assert.AreEqual("     ╥ ╥            \n" +
                        "   ╥ ║ ╨            \n" +
                        "   ╨ ╨              ", render.ToString());
        var paths = _city.CreatePath(new Vector2I(0, 1), new Vector2I(12, 1));
        Console.WriteLine(render.ToString());

        Assert.AreEqual("     ╥ ╥            \n" +
                        "╞══╦═╬═╩════╡       \n" +
                        "   ╨ ╨              ", render.ToString());

        Assert.AreEqual(4, paths.Count);
        Assert.AreEqual(9, _city.Intersections.Count);
        Assert.AreEqual(8, _city.GetAllPaths().Count());

        var paths2 = _city.CreatePath(new Vector2I(3, 2), new Vector2I(5, 2));
        Console.WriteLine(render.ToString());

        Assert.AreEqual("     ╥ ╥            \n" +
                        "╞══╦═╬═╩════╡       \n" +
                        "   ╚═╝              ", render.ToString());

        Assert.AreEqual(1, paths2.Count);
        Assert.AreEqual(9, _city.Intersections.Count);
        Assert.AreEqual(9, _city.GetAllPaths().Count());
    }

    [Test]
    public void RemovePaths() {
        var options = new CityGenerationOptions {
            StreetMinLength = 3,

            ProbabilityIntersection = 0.20f,

            ProbabilityCross = 0.42f,
            ProbabilityFork = 0.42f,
            ProbabilityTurn = 0.12f,

            ProbabilityStreetEnd = 0.001f,

            BuildingMinSize = 2, //2,
            BuildingMaxSize = 5, //3,
            BuildingMinSpace = 1,
            BuildingMaxSpace = 2, // 2

            Seed = 1,
        };

        var city = new City(140, 25);
        city.OnUpdate = (_) => {
            city.ValidateIntersections(true);
            city.ValidateRoads();
            city.ValidateIntersectionPaths();
        };
        var generator = city.CreateGenerator(options);
        generator.Start();
        generator.Grow();
        generator.City.ValidateIntersections(true);
        generator.City.ValidateIntersectionPaths();
        generator.City.ValidateRoads();

        while (_city.GetAllPaths().Any()) {
            var path = _city.GetAllPaths().First();
            city.RemovePath(path);
            city.ValidateIntersections(true);
            city.ValidateRoads();
            city.ValidateIntersectionPaths();
        }

    }

    [Test]
    // [Only]
    // [Ignore("Slow tests")]
    public void Grow_ValidateOnUpdate() {
        List<Vector2I> startDirections = [Vector2I.Right, Vector2I.Down, Vector2I.Left, Vector2I.Up];

        var options = new CityGenerationOptions {
            StartDirections = startDirections,

            StreetMinLength = 3,

            ProbabilityIntersection = 0.20f,

            ProbabilityCross = 0.42f,
            ProbabilityFork = 0.42f,
            ProbabilityTurn = 0.12f,

            ProbabilityStreetEnd = 0.001f,

            BuildingMinSize = 2, //2,
            BuildingMaxSize = 5, //3,
            BuildingMinSpace = 1,
            BuildingMaxSpace = 2, // 2
        };

        var city = new City(140, 25);
        var generator = city.CreateGenerator(options);
        city.OnUpdate = (_) => {
            generator.City.ValidateIntersections();
            generator.City.ValidateIntersectionPaths();
            generator.City.ValidateRoads();
        };

        for (var i = 0; i < 2000; i++) {
            options.Seed = i;
            if (i % 100 == 0) {
                Console.WriteLine(options.Seed);
            }
            generator.Start();
            generator.Grow();
        }
    }

    [Test]
    // [Only]
    // [Ignore("Slow tests")]
    public void Grow_ValidateFinalState() {
        List<Vector2I> startDirections = [Vector2I.Right, Vector2I.Down, Vector2I.Left, Vector2I.Up];

        var options = new CityGenerationOptions {
            StartDirections = startDirections,

            StreetMinLength = 3,

            ProbabilityIntersection = 0.20f,

            ProbabilityCross = 0.42f,
            ProbabilityFork = 0.42f,
            ProbabilityTurn = 0.12f,

            ProbabilityStreetEnd = 0.001f,

            BuildingMinSize = 2, //2,
            BuildingMaxSize = 5, //3,
            BuildingMinSpace = 1,
            BuildingMaxSpace = 2, // 2
        };

        var city = new City(140, 25);
        var generator = city.CreateGenerator(options);
        for (var i = 0; i < 10000; i++) {
            options.Seed = i;
            if (i % 100 == 0) {
                Console.WriteLine(options.Seed);
            }
            generator.Start();
            generator.Grow();
            generator.City.ValidateIntersections(true);
            generator.City.ValidateIntersectionPaths();
            generator.City.ValidateRoads();
        }
    }
}