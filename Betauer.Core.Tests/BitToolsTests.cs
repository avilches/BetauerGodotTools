using NUnit.Framework;

namespace Betauer.Core.Tests; 

[TestFixture]
public class BitToolsTests {

    [Test]
    public void EnableDisableHasTest() {
        for (var i = 1; i < 20; i++) {
            int bit = 0b00000000000000000000;
            Assert.That(BitTools.HasBit(bit, i), Is.False);
            bit = BitTools.SetBit(0, i, true);
            Assert.That(BitTools.HasBit(bit, i), Is.True);
        }
        
        for (var i = 1; i < 20; i++) {
            int bit = 0b11111111111111111111;
            Assert.That(BitTools.HasBit(bit, i), Is.True);
            bit = BitTools.SetBit(0, i, false);
            Assert.That(BitTools.HasBit(bit, i), Is.False);
        }
    }
}