using System.Threading.Tasks;
using Betauer.Bus.Signal;
using Betauer.Core.Signal;
using Betauer.TestRunner;
using Betauer.Tools.Logging;
using NUnit.Framework;

namespace Betauer.Bus.Tests.Signal; 

[TestFixture]
public class AreaShapeOnArea2DTests : BaseNodeTest {
    [OneTimeSetUp]
    public void Setup() {
        LoggerFactory.OverrideTraceLevel(TraceLevel.All);
    }

    [Test]
    public async Task AreaShapeOnArea2DCollisionTests() {
        var body = CreateArea2D("player body", 0, 0);
        var area2D = CreateArea2D("area", 2000, 2000);
            
        await this.AwaitPhysicsFrame();

        AreaShapeOnArea2D.Status status = new AreaShapeOnArea2D.Status();
        AreaShapeOnArea2D.Collection collection = new AreaShapeOnArea2D.Collection();
        status.Connect(area2D);
        collection.Connect(area2D);

        await this.AwaitPhysicsFrame();

        // They are not colliding
        Assert.That(status.Status, Is.False);
        Assert.That(collection.Size(), Is.EqualTo(0));

        await ForceCollision(area2D, body);

        Assert.That(status.Status, Is.True);
        Assert.That(collection.Size(), Is.EqualTo(1));
        Assert.That(collection.Contains(body), Is.True);

        await ForceNotCollision(area2D, body);
            
        Assert.That(status.Status, Is.False);
        Assert.That(collection.Size(), Is.EqualTo(0));
    }
}