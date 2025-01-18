using NUnit.Framework;using Veronenger.Game.Deck.Hands;

namespace Veronenger.Tests.Deck;

[TestFixture]
public class StraightTests : PokerHandsTestBase {
    [Test]
    public void IsEquivalent_SameRanksWithDifferentSuits_ShouldBeTrue() {
        var straight1 = new Straight(
            CreateCards("7H", "8H", "TH"), // 7,8,10
            7, 4 // Size 4: 7-10
        );
        var straight2 = new Straight(
            CreateCards("7S", "8D", "TC"), // Mismo patrón, distintos palos
            7, 4
        );
        
        Assert.That(straight1.IsEquivalent(straight2), Is.True);
        Assert.That(straight2.IsEquivalent(straight1), Is.True);
    }

    [Test]
    public void IsEquivalent_DifferentRanks_ShouldBeFalse() {
        var straight1 = new Straight(
            CreateCards("7H", "8H", "TH"), // 7,8,10
            7, 4
        );
        var straight2 = new Straight(
            CreateCards("7H", "9H", "TH"), // 7,9,10
            7, 4
        );
        
        Assert.That(straight1.IsEquivalent(straight2), Is.False);
    }

    [Test]
    public void IsEquivalent_DifferentRankRanges_ShouldBeFalse() {
        var straight1 = new Straight(
            CreateCards("7H", "8H", "TH"), // 7,8,10
            7, 4
        );
        var straight2 = new Straight(
            CreateCards("8H", "9H", "JH"), // 8,9,J
            8, 4
        );
        
        Assert.That(straight1.IsEquivalent(straight2), Is.False);
    }

    [Test]
    public void IsEquivalent_WithAce_ShouldCompareCorrectly() {
        var straight1 = new Straight(
            CreateCards("AH", "2H", "3H", "5H"), // A,2,3,5
            1, 5
        );
        var straight2 = new Straight(
            CreateCards("AS", "2D", "3C", "5H"), // Mismo patrón, distintos palos
            1, 5
        );
        
        Assert.That(straight1.IsEquivalent(straight2), Is.True);
    }

    [Test]
    public void Gaps_ShouldCalculateCorrectly() {
        var straight1 = new Straight(
            CreateCards("7H", "8H", "TH"), // 7,8,_,10 (1 gap)
            7, 4
        );
        Assert.That(straight1.Gaps, Is.EqualTo(1));

        var straight2 = new Straight(
            CreateCards("7H", "TH"), // 7,_,_,10 (1 gap)
            7, 4
        );
        Assert.That(straight2.Gaps, Is.EqualTo(1));

        var straight3 = new Straight(
            CreateCards("7H", "8H", "9H", "TH"), // 7,8,9,10 (no gaps)
            7, 4
        );
        Assert.That(straight3.Gaps, Is.EqualTo(0));

        var straight4 = new Straight(
            CreateCards("AH", "3H", "5H"), // A,_,3,_,5 (2 gaps)
            1, 5
        );
        Assert.That(straight4.Gaps, Is.EqualTo(2));
    }
    
    [Test]
    public void Gaps_EmptyOrSingleCard_ShouldBeZero() {
        var straight1 = new Straight(
            CreateCards(), // Sin cartas
            7, 4
        );
        Assert.That(straight1.Gaps, Is.EqualTo(0));

        var straight2 = new Straight(
            CreateCards("7H"), // Una sola carta
            7, 4
        );
        Assert.That(straight2.Gaps, Is.EqualTo(0));
    }

    [Test]
    public void Gaps_WithAceAsOne_ShouldCalculateCorrectly() {
        var straight1 = new Straight(
            CreateCards("AH", "2H", "4H"), // A,2,_,4 (1 gap)
            14, 4  // MinRank 14 se convierte a 1
        );
        Assert.That(straight1.Gaps, Is.EqualTo(1));
    
        var straight2 = new Straight(
            CreateCards("AH", "4H"), // A,_,_,4 (1 gap)
            14, 4  // MinRank 14 se convierte a 1
        );
        Assert.That(straight2.Gaps, Is.EqualTo(1));
    
        var straight3 = new Straight(
            CreateCards("AH", "2H", "3H", "4H"), // A,2,3,4 (no gaps)
            14, 4  // MinRank 14 se convierte a 1
        );
        Assert.That(straight3.Gaps, Is.EqualTo(0));
    }

    [Test]
    public void Gaps_WithHighCards_ShouldCalculateCorrectly() {
        var straight1 = new Straight(
            CreateCards("TH", "JH", "KH"), // 10,J,_,K (1 gap)
            10, 4
        );
        Assert.That(straight1.Gaps, Is.EqualTo(1));
    
        var straight2 = new Straight(
            CreateCards("JH", "KH"), // J,_,K (1 gap)
            11, 3
        );
        Assert.That(straight2.Gaps, Is.EqualTo(1));
    
        var straight3 = new Straight(
            CreateCards("TH", "JH", "QH", "KH"), // 10,J,Q,K (no gaps)
            10, 4
        );
        Assert.That(straight3.Gaps, Is.EqualTo(0));
    }    

    [Test]
    public void IsEquivalent_DifferentCardCount_ShouldBeFalse() {
        var straight1 = new Straight(
            CreateCards("7H", "8H", "TH"), // 7,8,10
            7, 4
        );
        var straight2 = new Straight(
            CreateCards("7H", "8H", "9H", "TH"), // 7,8,9,10
            7, 4
        );
        
        Assert.That(straight1.IsEquivalent(straight2), Is.False);
    }

    [Test]
    public void IsEquivalent_WithNullComparison_ShouldBeFalse() {
        var straight = new Straight(
            CreateCards("7H", "8H", "TH"),
            7, 4
        );
        
        Assert.That(straight.IsEquivalent(null), Is.False);
    }

    [Test]
    public void IsEquivalent_SameCards_DifferentOrder_ShouldBeTrue() {
        var straight1 = new Straight(
            CreateCards("7H", "8H", "TH"),
            7, 4
        );
        var straight2 = new Straight(
            CreateCards("TH", "7H", "8H"), // Mismas cartas, distinto orden
            7, 4
        );
        
        Assert.That(straight1.IsEquivalent(straight2), Is.True);
    }

    [Test]
    public void ToString_ShouldShowCorrectRepresentation() {
        var straight = new Straight(
            CreateCards("7H", "8H", "TH"),
            7, 4
        );
        
        Assert.That(straight.ToString(), Is.EqualTo("Straight [7-10] Size=4 MissingCards=1 Gaps=1 Cards=[7H,8H,TH]"));
    }

    [Test]
    public void IsHigherThan_LessGapsWins() {
        var straight1 = new Straight(
            CreateCards("7H", "8H", "9H", "TH"), // No gaps
            7, 5
        );
        var straight2 = new Straight(
            CreateCards("9H", "TH", "JH"), // 2 gaps
            9, 5
        );
        
        Assert.That(straight1.IsHigherThan(straight2), Is.True);
        Assert.That(straight2.IsHigherThan(straight1), Is.False);
    }

    [Test]
    public void IsHigherThan_SameGapsHigherRankWins() {
        var straight1 = new Straight(
            CreateCards("TH", "JH", "KH"), // T,J,_,K (1 gap)
            10, 4
        );
        var straight2 = new Straight(
            CreateCards("7H", "8H", "TH"), // 7,8,_,T (1 gap)
            7, 4
        );
        
        Assert.That(straight1.IsHigherThan(straight2), Is.True);
        Assert.That(straight2.IsHigherThan(straight1), Is.False);
    }
    
    [Test]
    public void MinRank_WhenIs14_ShouldBeConvertedTo1() {
        var straight = new Straight(
            CreateCards("AH", "2H", "3H"), // A,2,3
            14, 5  // MinRank 14 debería convertirse a 1
        );
    
        Assert.That(straight.MinRank, Is.EqualTo(1));
        Assert.That(straight.MaxRank, Is.EqualTo(5)); // 1 + 5 - 1 = 5
        Assert.That(straight.Size, Is.EqualTo(5));
        Assert.That(straight.MissingCards, Is.EqualTo(2)); // 5 - 3 = 2
        Assert.That(straight.ToString(), Is.EqualTo("Straight [1-5] Size=5 MissingCards=2 Gaps=0 Cards=[2H,3H,AH]"));
    }
}