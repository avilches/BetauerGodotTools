using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Deck;
using Betauer.Core.Deck.Hands;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests.PokerHands;

[TestFixture]
public class PairHandsTest : PokerHandsTestBase {
    // Tests básicos de identificación de pares
    [Test]
    public void SinglePair_ShouldBeIdentified() {
        var cards = CreateCards("AS", "AH", "KH", "QD", "JC");
        var pairs = new PairHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));
        Assert.That(pairs.Count, Is.EqualTo(1));
        Assert.That(pairs[0].Cards.Count, Is.EqualTo(2));
        Assert.That(pairs[0].Cards.All(c => c.Rank == 14), Is.True); // Todos son Ases
    }

    [Test]
    public void Pair_WithThreeOfSameRank_ShouldIdentifyOnePair() {
        var cards = CreateCards("AS", "AH", "AD", "KH", "QD", "JC", "2C");
        var pairs = new PairHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));
        Assert.That(pairs.Count, Is.EqualTo(1));
        Assert.That(pairs.All(h => h.Cards.Count == 2), Is.True);
        Assert.That(pairs.All(h => h.Cards.All(c => c.Rank == 14)), Is.True); // Todos son Ases
    }

    [Test]
    public void WithMultiplePairs_ShouldIdentifyAllOfThem() {
        // Tenemos par de Ases y par de Reyes
        var cards = CreateCards("AS", "AH", "KS", "KH", "QD");
        var pairs = new PairHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            // Verificamos que hay exactamente 2 parejas
            Assert.That(pairs.Count, Is.EqualTo(2), "Should have exactly 2 pairs");
        
            // Verificamos la primera pareja (Ases)
            Assert.That(pairs[0].Cards.Count, Is.EqualTo(2), "First pair should have 2 cards");
            Assert.That(pairs[0].Cards.All(c => c.Rank == 14), Is.True, "First pair should be Aces");
        
            // Verificamos la segunda pareja (Reyes)
            Assert.That(pairs[1].Cards.Count, Is.EqualTo(2), "Second pair should have 2 cards");
            Assert.That(pairs[1].Cards.All(c => c.Rank == 13), Is.True, "Second pair should be Kings");
        });
    }
    
    // Tests de sugerencia de descartes
    [Test]
    public void SuggestDiscards_WithoutPair_ShouldSuggestDiscardingLowestCards() {
        var cards = CreateCards("KS", "QH", "JD", "TC", "9H");
        var pairs = new PairHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));
        
        var pair = new PairHand(HandsManager, []);
        var discards = pair.SuggestDiscards(new PokerHandAnalysis(Handler.Config, cards), 2);
        
        Assert.That(discards.Count, Is.EqualTo(1), "Should suggest one discard option");
        var cardsToDiscard = discards[0];
        Assert.That(cardsToDiscard.Count, Is.EqualTo(2), "Should suggest discarding 2 cards");
        Assert.That(cardsToDiscard.All(c => c.Rank <= 10), Is.True, "Should discard lowest cards");
    }

    [Test]
    public void SuggestDiscards_WithExistingPair_ShouldNotSuggestDiscards() {
        var cards = CreateCards("AS", "AH", "KH", "QD", "JC");
        var pairs = new PairHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));
        
        var pair = new PairHand(HandsManager, []);
        var discards = pair.SuggestDiscards(new PokerHandAnalysis(Handler.Config, cards), 3);
        
        Assert.That(discards, Is.Empty, "Should not suggest discards when pair exists");
    }

    [Test]
    public void SingleCard_ShouldReturnNoPairs() {
        var cards = CreateCards("AS");
        var pairs = new PairHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));
        Assert.That(pairs, Is.Empty);
    }

    [Test]
    public void EmptyHand_ShouldReturnNoPairs() {
        var pairs = new PairHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, []));
        Assert.That(pairs, Is.Empty);
    }
}