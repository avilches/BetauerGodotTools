using Godot;
using NUnit.Framework;

namespace Veronenger.Tests.NodeTests; 

[Betauer.TestRunner.Test]
// [Ignore("")]
public partial class NodeTests : Node {
    [Betauer.TestRunner.Test]
    public void SpawnAndFreeManyObjectsInLessThanFiveSeconds() {
        const int seconds = 5;
        var startTime = Time.GetTicksMsec();

        for (var i = 0; i < 100000; i++) {
            new Node().Free();
        }

        Assert.That((Time.GetTicksMsec() - startTime) / 1000.0f, Is.LessThan(seconds));
    }
}