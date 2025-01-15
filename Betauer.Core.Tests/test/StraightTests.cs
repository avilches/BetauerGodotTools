using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Deck;
using Betauer.Core.Deck.Hands;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class StraightTests : PokerHandsTestBase {
    [Test]
    public void IsEquivalent_SameRanksWithDifferentSuits_ShouldBeTrue() {
        var straight1 = new Straight(
            CreateCards("7H", "8H", "TH"), // 7,8,10
            7, 10
        );
        var straight2 = new Straight(
            CreateCards("7S", "8D", "TC"), // Mismo patrón, distintos palos
            7, 10
        );
        
        Assert.That(straight1.IsEquivalent(straight2), Is.True);
        Assert.That(straight2.IsEquivalent(straight1), Is.True);
    }

    [Test]
    public void IsEquivalent_DifferentRanks_ShouldBeFalse() {
        var straight1 = new Straight(
            CreateCards("7H", "8H", "TH"), // 7,8,10
            7, 10
        );
        var straight2 = new Straight(
            CreateCards("7H", "9H", "TH"), // 7,9,10
            7, 10
        );
        
        Assert.That(straight1.IsEquivalent(straight2), Is.False);
    }

    [Test]
    public void IsEquivalent_DifferentRankRanges_ShouldBeFalse() {
        var straight1 = new Straight(
            CreateCards("7H", "8H", "TH"), // 7,8,10
            7, 10
        );
        var straight2 = new Straight(
            CreateCards("8H", "9H", "JH"), // 8,9,J
            8, 11
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
    public void IsEquivalent_DifferentCardCount_ShouldBeFalse() {
        var straight1 = new Straight(
            CreateCards("7H", "8H", "TH"), // 7,8,10
            7, 10
        );
        var straight2 = new Straight(
            CreateCards("7H", "8H", "9H", "TH"), // 7,8,9,10
            7, 10
        );
        
        Assert.That(straight1.IsEquivalent(straight2), Is.False);
    }

    [Test]
    public void IsEquivalent_WithNullComparison_ShouldBeFalse() {
        var straight = new Straight(
            CreateCards("7H", "8H", "TH"),
            7, 10
        );
        
        Assert.That(straight.IsEquivalent(null), Is.False);
    }

    [Test]
    public void IsEquivalent_SameCards_DifferentOrder_ShouldBeTrue() {
        var straight1 = new Straight(
            CreateCards("7H", "8H", "TH"),
            7, 10
        );
        var straight2 = new Straight(
            CreateCards("TH", "7H", "8H"), // Mismas cartas, distinto orden
            7, 10
        );
        
        Assert.That(straight1.IsEquivalent(straight2), Is.True);
    }

    [Test]
    public void ToString_ShouldShowCorrectRepresentation() {
        var straight = new Straight(
            CreateCards("7H", "8H", "TH"),
            7, 10
        );
        
        Assert.That(straight.ToString(), Is.EqualTo("Straight [7-10] MissingCards=1 Cards=[7H,8H,TH]"));
    }

    [Test]
    public void IsHigherThan_LessGapsWins() {
        var straight1 = new Straight(
            CreateCards("7H", "8H", "9H", "TH"), // 1 hueco
            7, 11
        );
        var straight2 = new Straight(
            CreateCards("9H", "TH", "JH"), // 2 huecos
            9, 13
        );
        
        Assert.That(straight1.IsHigherThan(straight2), Is.True);
        Assert.That(straight2.IsHigherThan(straight1), Is.False);
    }

    [Test]
    public void IsHigherThan_SameGapsHigherRankWins() {
        var straight1 = new Straight(
            CreateCards("TH", "JH", "KH"), // T,J,x,K (max 13)
            10, 13
        );
        var straight2 = new Straight(
            CreateCards("7H", "8H", "TH"), // 7,8,x,T (max 10)
            7, 10
        );
        
        Assert.That(straight1.IsHigherThan(straight2), Is.True);
        Assert.That(straight2.IsHigherThan(straight1), Is.False);
    }

}