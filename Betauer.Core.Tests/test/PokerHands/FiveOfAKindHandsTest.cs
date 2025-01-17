using System.Linq;
using Betauer.Core.Deck.Hands;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests.PokerHands;

[TestFixture]

public class FiveOfAKindHandsTest : PokerHandsTestBase {
    [Test]
    public void BasicFiveOfAKind_ShouldBeIdentified() {
        var cards = CreateCards("AS", "AH", "AD", "AC", "AH");
        var fiveOfKinds = PokerHandDefinition.Prototypes[PokerHandType.FiveOfAKind].IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            Assert.That(fiveOfKinds.Count, Is.EqualTo(1), "Should identify exactly one five of a kind");
            Assert.That(fiveOfKinds[0].Cards.Count, Is.EqualTo(5), "Five of a kind should have 5 cards");
            Assert.That(fiveOfKinds[0].Cards.All(c => c.Rank == 14), Is.True, "Should be five Aces");
        });
    }

    [Test]
    public void WithTwoFiveOfAKind_ShouldIdentifyBothOrderedByRank() {
        // First test case: AAAAA KKKKK
        var cards = CreateCards("AS", "AH", "AD", "AC", "AH", "KS", "KH", "KD", "KC", "KH");
        var fiveOfKinds = PokerHandDefinition.Prototypes[PokerHandType.FiveOfAKind].IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            Assert.That(fiveOfKinds.Count, Is.EqualTo(2), "Should identify two five of a kind");
            Assert.That(fiveOfKinds[0].Cards.All(c => c.Rank == 14), Is.True, "First should be five Aces");
            Assert.That(fiveOfKinds[1].Cards.All(c => c.Rank == 13), Is.True, "Second should be five Kings");
        });

        // Second test case: KKKKK AAAAA (reverse order input)
        cards = CreateCards("KS", "KH", "KD", "KC", "KH", "AS", "AH", "AD", "AC", "AH");
        fiveOfKinds = PokerHandDefinition.Prototypes[PokerHandType.FiveOfAKind].IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            Assert.That(fiveOfKinds.Count, Is.EqualTo(2), "Should identify two five of a kind");
            Assert.That(fiveOfKinds[0].Cards.All(c => c.Rank == 14), Is.True, "First should be five Aces");
            Assert.That(fiveOfKinds[1].Cards.All(c => c.Rank == 13), Is.True, "Second should be five Kings");
        });
    }

    [Test]
    public void WithSixOfSameRank_ShouldIdentifyFiveOfAKind() {
        // Con 6 Ases, debería identificar un FiveOfAKind
        var cards = CreateCards("AS", "AH", "AD", "AC", "AH", "AS");
        var fiveOfKinds = PokerHandDefinition.Prototypes[PokerHandType.FiveOfAKind].IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            Assert.That(fiveOfKinds.Count, Is.EqualTo(1), "Should identify exactly one five of a kind");
            Assert.That(fiveOfKinds[0].Cards.Count, Is.EqualTo(5), "Five of a kind should have 5 cards");
            Assert.That(fiveOfKinds[0].Cards.All(c => c.Rank == 14), Is.True, "Should be five Aces");
        });
    }

    [Test]
    public void FourOfAKind_ShouldNotIdentifyFiveOfAKind() {
        var cards = CreateCards("AS", "AH", "AD", "AC", "KH");
        var fiveOfKinds = PokerHandDefinition.Prototypes[PokerHandType.FiveOfAKind].IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));
        
        Assert.That(fiveOfKinds, Is.Empty, "Should not identify five of a kind with only four of a kind");
    }

    [Test]
    public void ThreeOfAKind_ShouldNotIdentifyFiveOfAKind() {
        var cards = CreateCards("AS", "AH", "AD", "KH", "QD");
        var fiveOfKinds = PokerHandDefinition.Prototypes[PokerHandType.FiveOfAKind].IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));
        
        Assert.That(fiveOfKinds, Is.Empty, "Should not identify five of a kind with only three of a kind");
    }

    [Test]
    public void SixOfSameRank_ShouldIdentifyOneFiveOfAKind() {
        // Con 6 Ases, debería identificar un cinco de kind con los primeros 5
        var cards = CreateCards("AS", "AH", "AD", "AC", "AH", "AS");
        var fiveOfKinds = PokerHandDefinition.Prototypes[PokerHandType.FiveOfAKind].IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            Assert.That(fiveOfKinds.Count, Is.EqualTo(1), "Should identify exactly one five of a kind");
            Assert.That(fiveOfKinds[0].Cards.Count, Is.EqualTo(5), "Should have exactly 5 cards");
            Assert.That(fiveOfKinds[0].Cards.All(c => c.Rank == 14), Is.True, "Should be Aces");
        });
    }

    [Test]
    public void SuggestDiscards_WithoutFiveOfAKind_ShouldSuggestDiscardingLowestCards() {
        var cards = CreateCards("AS", "AH", "AD", "AC", "KH");
        var discards = PokerHandDefinition.Prototypes[PokerHandType.FiveOfAKind].SuggestDiscards(new PokerHandAnalysis(Handler.Config, cards), 2);
        
        Assert.That(discards.Count, Is.EqualTo(1), "Should suggest one discard option");
        var cardsToDiscard = discards[0];
        Assert.That(cardsToDiscard.Count, Is.LessThanOrEqualTo(2), "Should not discard more than requested");
        Assert.That(cardsToDiscard.All(c => !cards.Where(card => card.Rank == 14).Contains(c)), 
            "Should not discard cards from the highest rank");
    }

    [Test]
    public void SuggestDiscards_WithFiveOfAKind_ShouldNotSuggestDiscards() {
        var cards = CreateCards("AS", "AH", "AD", "AC", "AH");
        var discards = PokerHandDefinition.Prototypes[PokerHandType.FiveOfAKind].SuggestDiscards(new PokerHandAnalysis(Handler.Config, cards), 3);
        
        Assert.That(discards, Is.Empty, "Should not suggest discards when five of a kind exists");
    }

    [Test]
    public void EmptyHand_ShouldReturnNoFiveOfAKind() {
        var fiveOfKinds = PokerHandDefinition.Prototypes[PokerHandType.FiveOfAKind].IdentifyHands(new PokerHandAnalysis(Handler.Config, []));
        Assert.That(fiveOfKinds, Is.Empty, "Should not identify five of a kind in empty hand");
    }
}