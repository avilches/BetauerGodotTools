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

            Assert.IsFalse(spatialGrid.Overlaps(circle1));
            Assert.IsFalse(spatialGrid.Overlaps(circle2));
            Assert.IsFalse(spatialGrid.Overlaps(circle3));

            spatialGrid.AddShape(circle1);
            Assert.IsTrue(spatialGrid.Overlaps(new Circle { Position = new Vector2(5, 5), Radius = 4 }));
            Assert.IsTrue(spatialGrid.Overlaps(new Circle { Position = new Vector2(7, 7), Radius = 4 }));
            Assert.IsFalse(spatialGrid.Overlaps(new Circle { Position = new Vector2(17, 17), Radius = 4 }));
            Assert.IsFalse(spatialGrid.Overlaps(circle1));
            Assert.IsTrue(spatialGrid.Overlaps(circle2));
            Assert.IsFalse(spatialGrid.Overlaps(circle3));

            spatialGrid.AddShape(circle2);
            Assert.IsTrue(spatialGrid.Overlaps(new Circle { Position = new Vector2(5, 5), Radius = 4 }));
            Assert.IsTrue(spatialGrid.Overlaps(new Circle { Position = new Vector2(7, 7), Radius = 4 }));
            Assert.IsFalse(spatialGrid.Overlaps(new Circle { Position = new Vector2(17, 17), Radius = 4 }));
            Assert.IsTrue(spatialGrid.Overlaps(circle1));
            Assert.IsTrue(spatialGrid.Overlaps(circle2));
            Assert.IsFalse(spatialGrid.Overlaps(circle3));

            spatialGrid.AddShape(circle3);
            Assert.IsTrue(spatialGrid.Overlaps(new Circle { Position = new Vector2(5, 5), Radius = 4 }));
            Assert.IsTrue(spatialGrid.Overlaps(new Circle { Position = new Vector2(7, 7), Radius = 4 }));
            Assert.IsTrue(spatialGrid.Overlaps(new Circle { Position = new Vector2(17, 17), Radius = 4 }));
            Assert.IsTrue(spatialGrid.Overlaps(circle1));
            Assert.IsTrue(spatialGrid.Overlaps(circle2));
            Assert.IsFalse(spatialGrid.Overlaps(circle3));

        }
    }

    [TestRunner.Test]
    public async Task TestCircleCircleOverlap() {
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

            Assert.IsFalse(spatialGrid.Overlaps(circle1));
            Assert.IsFalse(spatialGrid.Overlaps(circle2));
            
            spatialGrid.AddShape(circle1);
            Assert.IsFalse(spatialGrid.Overlaps(circle1));
            Assert.IsTrue(spatialGrid.Overlaps(circle2));

            spatialGrid.AddShape(circle2);
            Assert.IsTrue(spatialGrid.Overlaps(circle1));
            Assert.IsTrue(spatialGrid.Overlaps(circle2));
        }
    }

    [TestRunner.Test]
    public async Task TestCircleCircleNoOverlap() {
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
            spatialGrid.AddShape(circle1);
            Assert.IsTrue(!spatialGrid.Overlaps(circle2));
        }
    }

    [TestRunner.Test]
    public async Task TestCircleRectangleOverlap() {
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
            Assert.IsFalse(spatialGrid.Overlaps(circle));
            Assert.IsFalse(spatialGrid.Overlaps(rectangle));

            spatialGrid.AddShape(circle);
            Assert.IsFalse(spatialGrid.Overlaps(circle));
            Assert.IsTrue(spatialGrid.Overlaps(rectangle));
            
            spatialGrid.AddShape(rectangle);
            Assert.IsTrue(spatialGrid.Overlaps(circle));
            Assert.IsTrue(spatialGrid.Overlaps(rectangle));
        }
    }

    [TestRunner.Test]
    public async Task TestCircleRectangleNoOverlap() {
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
            spatialGrid.AddShape(circle);
            Assert.IsFalse(spatialGrid.Overlaps(rectangle));
        }
    }

    [TestRunner.Test]
    public async Task TestRectangleRectangleOverlap() {
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
            Assert.IsFalse(spatialGrid.Overlaps(rectangle1));
            Assert.IsFalse(spatialGrid.Overlaps(rectangle2));

            spatialGrid.AddShape(rectangle1);
            Assert.IsFalse(spatialGrid.Overlaps(rectangle1));
            Assert.IsTrue(spatialGrid.Overlaps(rectangle2));

            spatialGrid.AddShape(rectangle2);
            Assert.IsTrue(spatialGrid.Overlaps(rectangle1));
            Assert.IsTrue(spatialGrid.Overlaps(rectangle2));
        }
    }

    [TestRunner.Test]
    public async Task TestRectangleRectangleNoOverlap() {
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

            spatialGrid.AddShape(rectangle1);

            Assert.IsFalse(spatialGrid.Overlaps(rectangle2));
        }
    }


    [TestRunner.Test]
    public void TestRectangleRemove() {
        var rectangle = new Rectangle { Position = new Vector2(5, 5), Size = new Vector2(4, 4) };
        for (var i = 4; i < 100; i++) {
            var spatialGrid = new SpatialGrid(i);

            spatialGrid.AddShape(rectangle);
            spatialGrid.RemoveShape(rectangle);

            Assert.IsFalse(spatialGrid.Overlaps(rectangle));
        }
    }
}