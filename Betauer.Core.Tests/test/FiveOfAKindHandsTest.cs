using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Deck;
using Betauer.Core.Deck.Hands;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class FiveOfAKindHandsTest : PokerHandsTestBase {
    [Test]
    public void BasicFiveOfAKind_ShouldBeIdentified() {
        var cards = CreateCards("AS", "AH", "AD", "AC", "AH");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var fiveOfKinds = hands.Where(h => h is FiveOfAKindHand).ToList();

        Assert.Multiple(() => {
            Assert.That(fiveOfKinds.Count, Is.EqualTo(1), "Should identify exactly one five of a kind");
            Assert.That(fiveOfKinds[0].Cards.Count, Is.EqualTo(5), "Five of a kind should have 5 cards");
            Assert.That(fiveOfKinds[0].Cards.All(c => c.Rank == 14), Is.True, "Should be five Aces");
        });
    }

    [Test]
    public void WithTwoFiveOfAKind_ShouldIdentifyHighestOnly() {
        // Con AAAAA KKKKK, debería identificar solo AAAAA
        var cards = CreateCards("AS", "AH", "AD", "AC", "AH", 
            "KS", "KH", "KD", "KC", "KH");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var fiveOfKinds = hands.Where(h => h is FiveOfAKindHand).ToList();

        Assert.Multiple(() => {
            Assert.That(fiveOfKinds.Count, Is.EqualTo(1), "Should identify exactly one five of a kind");
            Assert.That(fiveOfKinds[0].Cards.Count, Is.EqualTo(5), "Five of a kind should have 5 cards");
            Assert.That(fiveOfKinds[0].Cards.All(c => c.Rank == 14), Is.True, "Should be five Aces (highest rank)");
        });
    }

    [Test]
    public void WithSixOfSameRank_ShouldIdentifyFiveOfAKind() {
        // Con 6 Ases, debería identificar un FiveOfAKind
        var cards = CreateCards("AS", "AH", "AD", "AC", "AH", "AS");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var fiveOfKinds = hands.Where(h => h is FiveOfAKindHand).ToList();

        Assert.Multiple(() => {
            Assert.That(fiveOfKinds.Count, Is.EqualTo(1), "Should identify exactly one five of a kind");
            Assert.That(fiveOfKinds[0].Cards.Count, Is.EqualTo(5), "Five of a kind should have 5 cards");
            Assert.That(fiveOfKinds[0].Cards.All(c => c.Rank == 14), Is.True, "Should be five Aces");
        });
    }

    [Test]
    public void FourOfAKind_ShouldNotIdentifyFiveOfAKind() {
        var cards = CreateCards("AS", "AH", "AD", "AC", "KH");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var fiveOfKinds = hands.Where(h => h is FiveOfAKindHand).ToList();
        
        Assert.That(fiveOfKinds, Is.Empty, "Should not identify five of a kind with only four of a kind");
    }

    [Test]
    public void FullHouse_ShouldNotIdentifyFiveOfAKind() {
        var cards = CreateCards("AS", "AH", "AD", "KS", "KH");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var fiveOfKinds = hands.Where(h => h is FiveOfAKindHand).ToList();
        
        Assert.That(fiveOfKinds, Is.Empty, "Should not identify five of a kind with only full house");
    }

    [Test]
    public void EmptyHand_ShouldReturnNoFiveOfAKind() {
        var cards = new List<Card>();
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var fiveOfKinds = hands.Where(h => h is FiveOfAKindHand).ToList();
        Assert.That(fiveOfKinds, Is.Empty);
    }

    [Test]
    public void SuggestDiscards_WithoutFiveOfAKind_ShouldSuggestDiscardingLowestCards() {
        var cards = CreateCards("AS", "AH", "AD", "AC", "KH");
        var hand = new FiveOfAKindHand(HandsManager, []);
        var discards = hand.SuggestDiscards(new PokerHandAnalysis(cards), 2);
        
        Assert.That(discards.Count, Is.EqualTo(1), "Should suggest one discard option");
        var cardsToDiscard = discards[0];
        Assert.That(cardsToDiscard.Count, Is.LessThanOrEqualTo(2), "Should not discard more than requested");
        Assert.That(cardsToDiscard.All(c => !cards.Where(card => card.Rank == 14).Contains(c)), 
            "Should not discard cards from the highest rank");
    }

    [Test]
    public void SuggestDiscards_WithFiveOfAKind_ShouldNotSuggestDiscards() {
        var cards = CreateCards("AS", "AH", "AD", "AC", "AH");
        var hand = new FiveOfAKindHand(HandsManager, []);
        var discards = hand.SuggestDiscards(new PokerHandAnalysis(cards), 3);
        
        Assert.That(discards, Is.Empty, "Should not suggest discards when five of a kind exists");
    }
}