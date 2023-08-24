using System.Linq;
using System.Threading.Tasks;
using Betauer.Core.Collision;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestRunner.Test]
[Only]
public class CollisionTests : Node2D {
    [TestRunner.SetUp]
    public void SetUp() {
        GetChildren().ForEach(n => n.Free());
    }

    [TestRunner.Test]
    public void TestIdentity() {
        var circle1 = new Circle { Position = new Vector2(5, 5), Radius = 4 };
        var circle2 = new Circle { Position = new Vector2(7, 7), Radius = 4 };
        var circle3 = new Circle { Position = new Vector2(17, 17), Radius = 4 };

        for (var i = 4; i < 100; i++) {
            var spatialGrid = new SpatialGrid(i);

            Assert.IsFalse(spatialGrid.Intersect(circle1));
            Assert.IsFalse(spatialGrid.Intersect(circle2));
            Assert.IsFalse(spatialGrid.Intersect(circle3));

            spatialGrid.Add(circle1);
            Assert.IsTrue(spatialGrid.Intersect(new Circle { Position = new Vector2(5, 5), Radius = 4 }));
            Assert.IsTrue(spatialGrid.Intersect(new Circle { Position = new Vector2(7, 7), Radius = 4 }));
            Assert.IsFalse(spatialGrid.Intersect(new Circle { Position = new Vector2(17, 17), Radius = 4 }));
            Assert.IsFalse(spatialGrid.Intersect(circle1));
            Assert.IsTrue(spatialGrid.Intersect(circle2));
            Assert.IsFalse(spatialGrid.Intersect(circle3));

            spatialGrid.Add(circle2);
            Assert.IsTrue(spatialGrid.Intersect(new Circle { Position = new Vector2(5, 5), Radius = 4 }));
            Assert.IsTrue(spatialGrid.Intersect(new Circle { Position = new Vector2(7, 7), Radius = 4 }));
            Assert.IsFalse(spatialGrid.Intersect(new Circle { Position = new Vector2(17, 17), Radius = 4 }));
            Assert.IsTrue(spatialGrid.Intersect(circle1));
            Assert.IsTrue(spatialGrid.Intersect(circle2));
            Assert.IsFalse(spatialGrid.Intersect(circle3));

            spatialGrid.Add(circle3);
            Assert.IsTrue(spatialGrid.Intersect(new Circle { Position = new Vector2(5, 5), Radius = 4 }));
            Assert.IsTrue(spatialGrid.Intersect(new Circle { Position = new Vector2(7, 7), Radius = 4 }));
            Assert.IsTrue(spatialGrid.Intersect(new Circle { Position = new Vector2(17, 17), Radius = 4 }));
            Assert.IsTrue(spatialGrid.Intersect(circle1));
            Assert.IsTrue(spatialGrid.Intersect(circle2));
            Assert.IsFalse(spatialGrid.Intersect(circle3));
        }
    }

    [TestRunner.Test]
    public async Task TestCircleCircleIntersect() {
        var circle1 = new Circle { Position = new Vector2(5, 5), Radius = 4 };
        var circle2 = new Circle { Position = new Vector2(7, 7), Radius = 4 };

        var circleArea1 = circle1.CreateArea2D();
        var circleArea2 = circle2.CreateArea2D();
        AddChild(circleArea1);
        AddChild(circleArea2);

        await this.AwaitPhysicsFrame();
        await this.AwaitPhysicsFrame();
        Assert.IsTrue(circleArea1.GetOverlappingAreas().Contains(circleArea2));

        for (var i = 4; i < 100; i++) {
            var spatialGrid = new SpatialGrid(i);

            Assert.IsFalse(spatialGrid.Intersect(circle1));
            Assert.IsFalse(spatialGrid.Intersect(circle2));

            spatialGrid.Add(circle1);
            Assert.IsFalse(spatialGrid.Intersect(circle1));
            Assert.IsTrue(spatialGrid.Intersect(circle2));

            spatialGrid.Add(circle2);
            Assert.IsTrue(spatialGrid.Intersect(circle1));
            Assert.IsTrue(spatialGrid.Intersect(circle2));

            Assert.That(spatialGrid.GetIntersectingShapes(circle1).ToList(), Contains.Item(circle2));
            Assert.That(spatialGrid.GetIntersectingShapes(circle2).ToList(), Contains.Item(circle1));

            Assert.That(spatialGrid.GetIntersectingShapes(new Circle { Position = new Vector2(6, 6), Radius = 4 }).ToList(),
                Contains.Item(circle1));
            Assert.That(spatialGrid.GetIntersectingShapes(new Circle { Position = new Vector2(6, 6), Radius = 4 }).ToList(),
                Contains.Item(circle2));
        }
    }

    [TestRunner.Test]
    public async Task TestCircleCircleNoIntersect() {
        var circle1 = new Circle { Position = new Vector2(5, 5), Radius = 4 };
        var circle2 = new Circle { Position = new Vector2(17, 17), Radius = 4 };

        var circleArea1 = circle1.CreateArea2D();
        var circleArea2 = circle2.CreateArea2D();
        AddChild(circleArea1);
        AddChild(circleArea2);

        await this.AwaitPhysicsFrame();
        await this.AwaitPhysicsFrame();
        Assert.IsTrue(circleArea1.GetOverlappingAreas().Count == 0);

        for (var i = 4; i < 100; i++) {
            var spatialGrid = new SpatialGrid(i);
            spatialGrid.Add(circle1);
            Assert.IsTrue(!spatialGrid.Intersect(circle2));
        }
    }

    [TestRunner.Test]
    public async Task TestCircleRectangleIntersect() {
        var circle = new Circle { Position = new Vector2(5, 5), Radius = 4 };
        var rectangle = new Rectangle { Position = new Vector2(7, 7), Size = new Vector2(4, 4) };

        var circleArea = circle.CreateArea2D();
        var rectArea = rectangle.CreateArea2D();
        AddChild(circleArea);
        AddChild(rectArea);

        await this.AwaitPhysicsFrame();
        await this.AwaitPhysicsFrame();
        Assert.IsTrue(circleArea.GetOverlappingAreas().Contains(rectArea));

        for (var i = 4; i < 100; i++) {
            var spatialGrid = new SpatialGrid(i);
            Assert.IsFalse(spatialGrid.Intersect(circle));
            Assert.IsFalse(spatialGrid.Intersect(rectangle));

            spatialGrid.Add(circle);
            Assert.IsFalse(spatialGrid.Intersect(circle));
            Assert.IsTrue(spatialGrid.Intersect(rectangle));

            spatialGrid.Add(rectangle);
            Assert.IsTrue(spatialGrid.Intersect(circle));
            Assert.IsTrue(spatialGrid.Intersect(rectangle));

            Assert.That(spatialGrid.GetIntersectingShapes(circle).ToList(), Contains.Item(rectangle));
            Assert.That(spatialGrid.GetIntersectingShapes(rectangle).ToList(), Contains.Item(circle));

            Assert.That(spatialGrid.GetIntersectingShapes(new Circle { Position = new Vector2(6, 6), Radius = 4 }).ToList(),
                Contains.Item(circle));
            Assert.That(spatialGrid.GetIntersectingShapes(new Circle { Position = new Vector2(6, 6), Radius = 4 }).ToList(),
                Contains.Item(rectangle));
        }
    }

    [TestRunner.Test]
    public async Task TestCircleRectangleNoIntersect() {
        var circle = new Circle { Position = new Vector2(5, 5), Radius = 4 };
        var rectangle = new Rectangle { Position = new Vector2(17, 17), Size = new Vector2(4, 4) };

        var circleArea = circle.CreateArea2D();
        var rectArea = rectangle.CreateArea2D();
        AddChild(circleArea);
        AddChild(rectArea);

        await this.AwaitPhysicsFrame();
        await this.AwaitPhysicsFrame();
        Assert.IsTrue(circleArea.GetOverlappingAreas().Count == 0);

        for (var i = 4; i < 100; i++) {
            var spatialGrid = new SpatialGrid(i);
            spatialGrid.Add(circle);
            Assert.IsFalse(spatialGrid.Intersect(rectangle));
        }
    }

    [TestRunner.Test]
    public async Task TestRectangleRectangleIntersect() {
        var rectangle1 = new Rectangle { Position = new Vector2(5, 5), Size = new Vector2(4, 4) };
        var rectangle2 = new Rectangle { Position = new Vector2(7, 7), Size = new Vector2(4, 4) };

        var rectArea1 = rectangle1.CreateArea2D();
        var rectArea2 = rectangle2.CreateArea2D();
        AddChild(rectArea1);
        AddChild(rectArea2);
        await this.AwaitPhysicsFrame();
        await this.AwaitPhysicsFrame();
        Assert.IsTrue(rectArea1.GetOverlappingAreas().Contains(rectArea2));
        Assert.IsTrue(rectArea2.GetOverlappingAreas().Contains(rectArea1));

        for (var i = 4; i < 100; i++) {
            var spatialGrid = new SpatialGrid(i);
            Assert.IsFalse(spatialGrid.Intersect(rectangle1));
            Assert.IsFalse(spatialGrid.Intersect(rectangle2));

            spatialGrid.Add(rectangle1);
            Assert.IsFalse(spatialGrid.Intersect(rectangle1));
            Assert.IsTrue(spatialGrid.Intersect(rectangle2));

            spatialGrid.Add(rectangle2);
            Assert.IsTrue(spatialGrid.Intersect(rectangle1));
            Assert.IsTrue(spatialGrid.Intersect(rectangle2));
            Assert.That(spatialGrid.GetIntersectingShapes(rectangle2).ToList(), Contains.Item(rectangle1));
            Assert.That(spatialGrid.GetIntersectingShapes(rectangle1).ToList(), Contains.Item(rectangle2));

            Assert.That(spatialGrid.GetIntersectingShapes(new Circle { Position = new Vector2(6, 6), Radius = 4 }).ToList(),
                Contains.Item(rectangle1));
            Assert.That(spatialGrid.GetIntersectingShapes(new Circle { Position = new Vector2(6, 6), Radius = 4 }).ToList(),
                Contains.Item(rectangle2));
        }
    }

    [TestRunner.Test]
    public async Task TestRectangleRectangleNoIntersect() {
        var rectangle1 = new Rectangle { Position = new Vector2(5, 5), Size = new Vector2(4, 4) };
        var rectangle2 = new Rectangle { Position = new Vector2(17, 17), Size = new Vector2(4, 4) };

        var rectArea1 = rectangle1.CreateArea2D();
        var rectArea2 = rectangle2.CreateArea2D();
        AddChild(rectArea1);
        AddChild(rectArea2);
        await this.AwaitPhysicsFrame();
        await this.AwaitPhysicsFrame();
        Assert.IsTrue(rectArea1.GetOverlappingAreas().Count == 0);

        for (var i = 4; i < 100; i++) {
            var spatialGrid = new SpatialGrid(i);

            spatialGrid.Add(rectangle1);

            Assert.IsFalse(spatialGrid.Intersect(rectangle2));
        }
    }


    [TestRunner.Test]
    public void TestRectangleRemove() {
        var rectangle = new Rectangle { Position = new Vector2(5, 5), Size = new Vector2(4, 4) };
        for (var i = 4; i < 100; i++) {
            var spatialGrid = new SpatialGrid(i);

            spatialGrid.Add(rectangle);
            spatialGrid.Remove(rectangle);

            Assert.IsFalse(spatialGrid.Intersect(rectangle));
        }
    }

    [TestRunner.Test]
    public void TestRectangleCells() {
        // Smaller than 10x10
        AssertRectangleFitCells(10, 0f, 0f, 9.99f, 9.99f, new[] { (0, 0) });
        // Exactly 10x10, it needs more adjacent cells
        AssertRectangleFitCells(10, 0f, 0f, 10f, 10f, new[] { (0, 0), (0, 1), (1, 0), (1, 1) });
        // Bigger than 10x10        
        AssertRectangleFitCells(10, 0.11f, 0.11f, 9.99f, 9.99f, new[] { (0, 0), (0, 1), (1, 0), (1, 1) });
        // Smaller than 10x10, negative
        AssertRectangleFitCells(10, -2f, -2f, 4f, 4f, new[] { (0, 0), (0, -1), (-1, 0), (-1, -1) });

        // Smaller than 10x10, offset
        AssertRectangleFitCells(10, 10f, 10f, 9.99f, 9.99f, new[] { (1, 1) });
        // Exactly 10x10, it needs more adjacent cells with offset
        AssertRectangleFitCells(10, 10f, 10f, 10.0f, 10.0f, new[] { (1, 1), (1, 2), (2, 1), (2, 2) });
        // Exactly 10x10, it needs more adjacent cells with offset Negative
        AssertRectangleFitCells(10, -10f, -10f, 10.0f, 10.0f, new[] { (0, 0), (0, -1), (-1, 0), (-1, -1) });
        // Bigger than 10x10        
        AssertRectangleFitCells(10, 10.11f, 10.11f, 9.99f, 9.99f, new[] { (1, 1), (1, 2), (2, 1), (2, 2) });

        // Bigger than 10x10        
        AssertRectangleFitCells(10, 10.11f, 10.11f, 19.99f, 19.99f, new[] { (1, 1), (1, 2), (1, 3), (2, 1), (2, 2), (2, 3), (3, 1), (3, 2), (3, 3) });
    }

    [TestRunner.Test]
    public void TestCircleCells() {
        // Smaller than 10x10
        AssertCircleFitCells(10, 5f, 5f, 4.99f, new[] { (0, 0) });
        // Exactly 10x10, it needs more adjacent cells
        AssertCircleFitCells(10, 5f, 5f, 5.00f, new[] { (0, 0), (0, 1), (1, 0), (1, 1) });
        // Bigger than 10x10        
        AssertCircleFitCells(10, 5.1f, 5.1f, 4.99f, new[] { (0, 0), (0, 1), (1, 0), (1, 1) });
        // Smaller than 10x10, negative
        AssertCircleFitCells(10, -2f, -2f, 4f, new[] { (0, 0), (0, -1), (-1, 0), (-1, -1) });

        // Smaller than 10x10, offset
        AssertCircleFitCells(10, 15f, 15f, 4.99f, new[] { (1, 1) });
        // Exactly 10x10, it needs more adjacent cells with offset
        AssertCircleFitCells(10, 15f, 15f, 5.00f, new[] { (1, 1), (1, 2), (2, 1), (2, 2) });
        // Exactly 10x10, it needs more adjacent cells with offset Negative
        AssertCircleFitCells(10, -15f, -15f, 5.00f, new[] { (-1, -1), (-1, -2), (-2, -1), (-2, -2) });
        // Bigger than 10x10        
        AssertCircleFitCells(10, 15.11f, 15.11f, 4.99f, new[] { (1, 1), (1, 2), (2, 1), (2, 2) });

        // Bigger than 10x10        
        AssertCircleFitCells(10, 25f, 25f, 7.49f, new[] { (1, 1), (1, 2), (1, 3), (2, 1), (2, 2), (2, 3), (3, 1), (3, 2), (3, 3) });
    }

    [TestRunner.Test]
    public void AssertRectangleMoveSameCell() {
        var spatial = new SpatialGrid(10);
        var shape = new Rectangle(5, 5, 4, 4);
        spatial.Add(shape);
        Assert.That(spatial.Grid.Count, Is.EqualTo(1));
        Assert.That(spatial.Grid[(0, 0)].Contains(shape));

        shape.Position = new Vector2(4, 4);
        Assert.That(shape.Position, Is.EqualTo(new Vector2(4, 4)));
        Assert.That(spatial.Grid[(0, 0)].Contains(shape));

        shape.Position = new Vector2(15, 15);
        Assert.That(shape.Position, Is.EqualTo(new Vector2(15, 15)));
        Assert.That(spatial.Grid.Count, Is.EqualTo(1));
        Assert.That(spatial.Grid[(1, 1)].Contains(shape));
    }

    [TestRunner.Test]
    public void AssertRectangleResizeSameCell() {
        var spatial = new SpatialGrid(10);
        var shape = new Rectangle(5, 5, 4, 4);
        spatial.Add(shape);
        Assert.That(spatial.Grid.Count, Is.EqualTo(1));
        Assert.That(spatial.Grid[(0, 0)].Contains(shape));

        shape.Size = new Vector2(3, 3);
        Assert.That(shape.Size, Is.EqualTo(new Vector2(3, 3)));
        Assert.That(spatial.Grid[(0, 0)].Contains(shape));
    }

    [TestRunner.Test]
    public void AssertRectangleMoveOtherCell() {
        var spatial = new SpatialGrid(10);
        var shape = new Rectangle(2, 2, 7, 7);
        spatial.Add(shape);
        Assert.That(spatial.Grid.Count, Is.EqualTo(1));
        Assert.That(spatial.Grid[(0, 0)].Contains(shape));

        shape.Position = new Vector2(8, 8);
        Assert.That(shape.Position, Is.EqualTo(new Vector2(8, 8)));
        Assert.That(spatial.Grid.Count, Is.EqualTo(4));
        Assert.That(spatial.Grid[(0, 0)].Contains(shape));
        Assert.That(spatial.Grid[(0, 1)].Contains(shape));
        Assert.That(spatial.Grid[(1, 0)].Contains(shape));
        Assert.That(spatial.Grid[(1, 1)].Contains(shape));
    }

    [TestRunner.Test]
    public void AssertRectangleResizeOtherCell() {
        var spatial = new SpatialGrid(10);
        var shape = new Rectangle(2, 2, 7, 7);
        spatial.Add(shape);
        Assert.That(spatial.Grid.Count, Is.EqualTo(1));
        Assert.That(spatial.Grid[(0, 0)].Contains(shape));

        shape.Size = new Vector2(9, 9);
        Assert.That(shape.Size, Is.EqualTo(new Vector2(9, 9)));
        Assert.That(spatial.Grid.Count, Is.EqualTo(4));
        Assert.That(spatial.Grid[(0, 0)].Contains(shape));
        Assert.That(spatial.Grid[(0, 1)].Contains(shape));
        Assert.That(spatial.Grid[(1, 0)].Contains(shape));
        Assert.That(spatial.Grid[(1, 1)].Contains(shape));
    }

    [TestRunner.Test]
    public void AssertCircleMoveSameCell() {
        var spatial = new SpatialGrid(10);
        var shape = new Circle(5, 5, 3);
        spatial.Add(shape);
        Assert.That(spatial.Grid.Count, Is.EqualTo(1));
        Assert.That(spatial.Grid[(0, 0)].Contains(shape));

        shape.Position = new Vector2(6, 6);
        Assert.That(shape.Position, Is.EqualTo(new Vector2(6, 6)));
        Assert.That(spatial.Grid.Count, Is.EqualTo(1));
        Assert.That(spatial.Grid[(0, 0)].Contains(shape));

        shape.Position = new Vector2(15, 15);
        Assert.That(shape.Position, Is.EqualTo(new Vector2(15, 15)));
        Assert.That(spatial.Grid.Count, Is.EqualTo(1));
        Assert.That(spatial.Grid[(1, 1)].Contains(shape));
    }

    [TestRunner.Test]
    public void AssertCircleResizeSameCell() {
        var spatial = new SpatialGrid(10);
        var shape = new Circle(5, 5, 3);
        spatial.Add(shape);
        Assert.That(spatial.Grid.Count, Is.EqualTo(1));
        Assert.That(spatial.Grid[(0, 0)].Contains(shape));

        shape.Radius = 4;
        Assert.That(shape.Radius, Is.EqualTo(4));
        Assert.That(spatial.Grid.Count, Is.EqualTo(1));
        Assert.That(spatial.Grid[(0, 0)].Contains(shape));
    }

    [TestRunner.Test]
    public void AssertCircleMoveOtherCell() {
        var spatial = new SpatialGrid(10);
        var shape = new Circle(5, 5, 3);
        spatial.Add(shape);
        Assert.That(spatial.Grid.Count, Is.EqualTo(1));
        Assert.That(spatial.Grid[(0, 0)].Contains(shape));

        shape.Position = new Vector2(9, 9);
        Assert.That(shape.Position, Is.EqualTo(new Vector2(9, 9)));
        Assert.That(spatial.Grid.Count, Is.EqualTo(4));
        Assert.That(spatial.Grid[(0, 0)].Contains(shape));
        Assert.That(spatial.Grid[(0, 1)].Contains(shape));
        Assert.That(spatial.Grid[(1, 0)].Contains(shape));
        Assert.That(spatial.Grid[(1, 1)].Contains(shape));
    }

    [TestRunner.Test]
    public void AssertCircleResizeOtherCell() {
        var spatial = new SpatialGrid(10);
        var shape = new Circle(6, 6, 3);
        spatial.Add(shape);
        Assert.That(spatial.Grid.Count, Is.EqualTo(1));
        Assert.That(spatial.Grid[(0, 0)].Contains(shape));

        shape.Radius = 5;
        Assert.That(shape.Radius, Is.EqualTo(5));
        Assert.That(spatial.Grid.Count, Is.EqualTo(4));
        Assert.That(spatial.Grid[(0, 0)].Contains(shape));
        Assert.That(spatial.Grid[(0, 1)].Contains(shape));
        Assert.That(spatial.Grid[(1, 0)].Contains(shape));
        Assert.That(spatial.Grid[(1, 1)].Contains(shape));
    }

    [TestRunner.Test]
    public void AssertCircleTryMove() {
        var spatial = new SpatialGrid(10);
        var shape1 = new Circle(0, 0, 2);
        var shape2 = new Circle(14, 6, 2);
        spatial.Add(shape1);
        spatial.Add(shape2);
        
        Assert.That(shape1.TryMove(7,6));
        Assert.That(!shape1.TryMove(10,6));
        Assert.That(shape1.Position, Is.EqualTo(new Vector2(7, 6)));
    }

    [TestRunner.Test]
    public void AssertCircleTryResize() {
        var spatial = new SpatialGrid(10);
        var shape1 = new Circle(6, 6, 1);
        var shape2 = new Circle(14, 6, 2);
        spatial.Add(shape1);
        spatial.Add(shape2);
        
        Assert.That(shape1.TryResize(5));
        Assert.That(!shape1.TryResize(6));
        Assert.That(shape1.Radius, Is.EqualTo(5));
    }

    [TestRunner.Test]
    public void AssertRectangleTryMove() {
        var spatial = new SpatialGrid(10);
        var shape1 = new Rectangle(0, 0, 2, 2);
        var shape2 = new Circle(14, 6, 2);
        spatial.Add(shape1);
        spatial.Add(shape2);
        
        Assert.That(shape1.TryMove(7,6));
        Assert.That(!shape1.TryMove(10,6));
        Assert.That(shape1.Position, Is.EqualTo(new Vector2(7, 6)));
    }

    [TestRunner.Test]
    public void AssertRectangleTryResize() {
        var spatial = new SpatialGrid(10);
        var shape1 = new Rectangle(6, 6, 4, 4);
        var shape2 = new Circle(14, 6, 2);
        spatial.Add(shape1);
        spatial.Add(shape2);
        
        Assert.That(shape1.TryResize(5, 3));
        Assert.That(!shape1.TryResize(6, 3));
        Assert.That(shape1.Size, Is.EqualTo(new Vector2(5, 3)));
    }

    private void AssertRectangleFitCells(int cellSize, float x, float y, float width, float height, (int, int)[] cells) {
        var spatial = new SpatialGrid(cellSize);
        var shape = new Rectangle(x, y, width, height);
        spatial.Add(shape);
        Assert.That(spatial.Grid.Count, Is.EqualTo(cells.Length));
        foreach (var cell in cells) {
            Assert.That(spatial.Grid[cell].Contains(shape));
        }
    }

    private void AssertCircleFitCells(int cellSize, float x, float y, float radius, (int, int)[] cells) {
        var spatial = new SpatialGrid(cellSize);
        var shape = new Circle(x, y, radius);
        spatial.Add(shape);
        Assert.That(spatial.Grid.Count, Is.EqualTo(cells.Length));
        foreach (var cell in cells) {
            Assert.That(spatial.Grid[cell].Contains(shape));
        }
    }
}