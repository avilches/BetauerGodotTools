using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Deck;
using Betauer.Core.Deck.Hands;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests.PokerHands;

[TestFixture]
public class FourOfAKindHandsTest : PokerHandsTestBase {
    [Test]
    public void BasicFourOfAKind_ShouldBeIdentified() {
        var cards = CreateCards("AS", "AH", "AD", "AC", "KH");
        var fourOfKinds = new FourOfAKindHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            Assert.That(fourOfKinds.Count, Is.EqualTo(1), "Should identify exactly one four of a kind");
            Assert.That(fourOfKinds[0].Cards.Count, Is.EqualTo(4), "Four of a kind should have 4 cards");
            Assert.That(fourOfKinds[0].Cards.All(c => c.Rank == 14), Is.True, "Should be four Aces");
        });
    }

    [Test]
    public void WithTwoFourOfAKind_ShouldIdentifyHighestOnly() {
        // Con AAAA KKKK, debería identificar solo AAAA
        var cards = CreateCards("AS", "AH", "AD", "AC", "KS", "KH", "KD", "KC");
        var fourOfKinds = new FourOfAKindHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            Assert.That(fourOfKinds.Count, Is.EqualTo(1), "Should identify exactly one four of a kind");
            Assert.That(fourOfKinds[0].Cards.All(c => c.Rank == 14), Is.True, "Should be four Aces (highest rank)");
        });
    }

    [Test]
    public void FourOfAKind_WithFiveOfAKind_ShouldBeIdentified() {
        // Con AAAAA, debería poder identificar AAAA
        var cards = CreateCards("AS", "AH", "AD", "AC", "AH");
        var fourOfKinds = new FourOfAKindHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            Assert.That(fourOfKinds.Count, Is.EqualTo(1), "Should identify exactly one four of a kind");
            Assert.That(fourOfKinds[0].Cards.Count, Is.EqualTo(4), "Four of a kind should have 4 cards");
            Assert.That(fourOfKinds[0].Cards.All(c => c.Rank == 14), Is.True, "Should be four Aces");
        });
    }

    [Test]
    public void ThreeOfAKind_ShouldNotIdentifyFourOfAKind() {
        var cards = CreateCards("AS", "AH", "AD", "KH", "QD");
        var fourOfKinds = new FourOfAKindHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));
        
        Assert.That(fourOfKinds, Is.Empty, "Should not identify four of a kind with only three of a kind");
    }

    [Test]
    public void FullHouse_ShouldNotIdentifyFourOfAKind() {
        var cards = CreateCards("AS", "AH", "AD", "KS", "KH");
        var fourOfKinds = new FourOfAKindHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));
        
        Assert.That(fourOfKinds, Is.Empty, "Should not identify four of a kind with only full house");
    }

    [Test]
    public void SuggestDiscards_WithoutFourOfAKind_ShouldSuggestDiscardingLowestCards() {
        var cards = CreateCards("AS", "AH", "AD", "KH", "QD");
        var hand = new FourOfAKindHand(HandsManager, []);
        var discards = hand.SuggestDiscards(new PokerHandAnalysis(Handler.Config, cards), 2);
        
        Assert.That(discards.Count, Is.EqualTo(1), "Should suggest one discard option");
        var cardsToDiscard = discards[0];
        Assert.That(cardsToDiscard.Count, Is.LessThanOrEqualTo(2), "Should not discard more than requested");
        Assert.That(cardsToDiscard.All(c => !cards.Where(card => card.Rank == 14).Contains(c)), 
            "Should not discard cards from the highest rank");
    }

    [Test]
    public void SuggestDiscards_WithFourOfAKind_ShouldNotSuggestDiscards() {
        var cards = CreateCards("AS", "AH", "AD", "AC", "KH");
        var hand = new FourOfAKindHand(HandsManager, []);
        var discards = hand.SuggestDiscards(new PokerHandAnalysis(Handler.Config, cards), 3);
        
        Assert.That(discards, Is.Empty, "Should not suggest discards when four of a kind exists");
    }

    [Test]
    public void EmptyHand_ShouldReturnNoFourOfAKind() {
        var cards = new List<Card>();
        var fourOfKinds = new FourOfAKindHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));
        Assert.That(fourOfKinds, Is.Empty);
    }
}