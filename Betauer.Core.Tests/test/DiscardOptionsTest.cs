/*using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Deck;
using Betauer.Core.Deck.Hands;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class DiscardOptionsTests {
    protected PokerHandsManager HandsManager => Handler.PokerHandsManager;
    protected GameStateHandler Handler;

    [SetUp]
    public void Setup() {
        Handler = new GameStateHandler(1, new PokerGameConfig());
        HandsManager.RegisterBasicPokerHands();
    }

    protected List<Card> CreateCards(params string[] cards) {
        var parse = new PokerGameConfig().Parse;
        return cards.Select(parse).ToList();
    }

    [Test]
    public void HighCardHand_SuggestDiscards_ShouldReturnEmpty() {
        var currentHand = CreateCards("AS", "KH", "QD", "JC", "TD");
        var hand = new HighCardHand(HandsManager, []);
        var discards = hand.SuggestDiscards(new PokerHandAnalysis(Handler.Config, currentHand), 3);

        Assert.That(discards, Is.Empty, "High card hand should not suggest any discards");
    }


    [Test]
    public void PairHand_SuggestDiscards_WithNoPair_ShouldDiscardLowest() {
        var currentHand = CreateCards("AH", "KH", "QD", "JC", "TD");
        var hand = new PairHand(HandsManager, []);
        var discards = hand.SuggestDiscards(new PokerHandAnalysis(Handler.Config, currentHand), 2)[0];

        Assert.Multiple(() => {
            Assert.That(discards, Has.Count.EqualTo(2), "Should discard 2 lowest cards");
            Assert.That(discards, Does.Contain(currentHand[4]), "Should discard TD");
            Assert.That(discards, Does.Contain(currentHand[3]), "Should discard JC");
            Assert.That(discards, Does.Not.Contain(currentHand[0]), "Should not discard AH");
            Assert.That(discards, Does.Not.Contain(currentHand[1]), "Should not discard KH");
            Assert.That(discards, Does.Not.Contain(currentHand[2]), "Should not discard QD");
        });
    }

    [Test]
    public void StraightHand_SuggestDiscards_ShouldKeepConnectedCards() {
        var currentHand = CreateCards("TD", "JH", "QC", "KD", "2S");
        var hand = new StraightHand(HandsManager, []);
        var discards = hand.SuggestDiscards(new PokerHandAnalysis(Handler.Config, currentHand), 1)[0];

        Assert.Multiple(() => {
            Assert.That(discards, Has.Count.EqualTo(1), "Should discard 1 non-connected card");
            Assert.That(discards, Does.Contain(currentHand[4]), "Should discard 2S");
        });
    }

    [Test]
    public void FlushHand_SuggestDiscards_ShouldKeepSameSuitCards() {
        // 4 hearts and 1 spade
        var currentHand = CreateCards("AH", "KH", "QH", "JH", "2S");
        var hand = new FlushHand(HandsManager, []);
        var discards = hand.SuggestDiscards(new PokerHandAnalysis(Handler.Config, currentHand), 1)[0];

        Assert.Multiple(() => {
            Assert.That(discards, Has.Count.EqualTo(1), "Should discard 1 off-suit card");
            Assert.That(discards, Does.Contain(currentHand[4]), "Should discard 2S");
            Assert.That(discards.All(c => c.Suit != 'H'), "Should not discard any hearts");
        });
    }

    [Test]
    public void FullHouseHand_SuggestDiscards_ShouldKeepTrioAndPair() {
        var currentHand = CreateCards("AS", "AH", "AD", "KH", "KC");
        var hand = new FullHouseHand(HandsManager, []);
        var discards = hand.SuggestDiscards(new PokerHandAnalysis(Handler.Config, currentHand), 2);

        Assert.That(discards, Is.Empty, "Should not discard any cards from a full house");
    }

    [Test]
    public void StraightFlushHand_SuggestDiscards_KeepConnectedSameSuit() {
        // 4 connected hearts and 1 spade
        var currentHand = CreateCards("TH", "JH", "QH", "KH", "2S");
        var hand = new StraightFlushHand(HandsManager, []);
        var discards = hand.SuggestDiscards(new PokerHandAnalysis(Handler.Config, currentHand), 1)[0];

        Assert.Multiple(() => {
            Assert.That(discards, Has.Count.EqualTo(1), "Should discard 1 card");
            Assert.That(discards, Does.Contain(currentHand[4]), "Should discard 2S");
            Assert.That(discards.All(c => c.Suit != 'H'), "Should not discard any hearts");
        });
    }

    [Test]
    public void SuggestDiscards_ShouldRespectMaxDiscardLimit() {
        var currentHand = CreateCards("2H", "3D", "4C", "5S", "6H", "7D", "8C");
        var maxDiscards = 2;

        foreach (var handConfig in GetAllHandTypes()) {
            var discards = handConfig.SuggestDiscards(new PokerHandAnalysis(Handler.Config, currentHand), maxDiscards);

            foreach (var discard in discards) {
                Assert.That(discard, Has.Count.LessThanOrEqualTo(maxDiscards),
                    $"{handConfig.GetType().Name} suggested more discards than allowed");
            }
        }
    }

    private IEnumerable<PokerHand> GetAllHandTypes() {
        yield return new HighCardHand(HandsManager, []);
        yield return new PairHand(HandsManager, []);
        yield return new TwoPairHand(HandsManager, []);
        yield return new ThreeOfAKindHand(HandsManager, []);
        yield return new StraightHand(HandsManager, []);
        yield return new FlushHand(HandsManager, []);
        yield return new FullHouseHand(HandsManager, []);
        yield return new FourOfAKindHand(HandsManager, []);
        yield return new StraightFlushHand(HandsManager, []);
    }

    [Test]
    public void HandUtils_FindRepeatedCards_Test() {
        var cards = CreateCards("AS", "AH", "AD", "KH", "KC");
        var groups = HandUtils.FindRepeatedCards(cards);

        Assert.Multiple(() => {
            Assert.That(groups, Has.Count.EqualTo(2), "Should find 2 groups");
            Assert.That(groups.First().Count(), Is.EqualTo(3), "First group should have 3 Aces");
            Assert.That(groups.Last().Count(), Is.EqualTo(2), "Second group should have 2 Kings");
        });
    }

    [Test]
    public void HandUtils_FindLargestSuitGroup_Test() {
        var cards = CreateCards("AH", "KH", "QH", "JD", "TD");
        var group = HandUtils.FindLargestSuitGroup(cards);

        Assert.Multiple(() => {
            Assert.That(group, Has.Count.EqualTo(3), "Should find 3 hearts");
            Assert.That(group.All(c => c.Suit == 'H'), "All cards should be hearts");
        });
    }

    [Test]
    public void HandUtils_FindStraightSequences_CompleteStraight() {
        var cards = CreateCards("9H", "TH", "JH", "QH", "KH");
        var sequences = HandUtils.FindStraightSequences(cards);

        Assert.Multiple(() => {
            Assert.That(sequences, Has.Count.EqualTo(1), "Should find exactly one sequence");
            Assert.That(sequences[0], Has.Count.EqualTo(5), "Sequence should have 5 cards");
            Assert.That(sequences[0].Select(c => c.Rank),
                Is.EqualTo(new[] { 9, 10, 11, 12, 13 }), "Should be 9-K sequence");
        });
    }

    [Test]
    public void HandUtils_FindStraightSequences_FourConnected() {
        var cards = CreateCards("9H", "TH", "JH", "QH", "2S");
        var sequences = HandUtils.FindStraightSequences(cards);
        var mainSequence = sequences[0];

        Assert.Multiple(() => {
            Assert.That(mainSequence, Has.Count.EqualTo(4), "Should find 4 connected cards");
            Assert.That(mainSequence.Select(c => c.Rank),
                Is.EqualTo(new[] { 9, 10, 11, 12 }), "Should be 9-Q sequence");
        });
    }


    [Test]
    public void HandUtils_FindStraightSequences_LowAceStraight() {
        var cards = CreateCards("AH", "2H", "3H", "4H", "5D", "KS");
        var sequences = HandUtils.FindStraightSequences(cards);
        var mainSequence = sequences[0];

        Assert.Multiple(() => {
            Assert.That(mainSequence, Has.Count.EqualTo(5), "Should find A-5 straight");
            // Convertimos explícitamente el As a 1 y materializamos la secuencia en un array
            var ranks = mainSequence.Select(c => c.Rank == 14 ? 1 : c.Rank)
                .OrderBy(r => r)
                .ToArray();
            Assert.That(ranks, Is.EqualTo(new[] { 1, 2, 3, 4, 5 }),
                "Should be A-5 sequence with Ace as 1");
        });
    }

    [Test]
    public void HandUtils_FindStraightSequences_HighAceStraight() {
        var cards = CreateCards("AH", "KH", "QH", "JH", "TD", "2S");
        var sequences = HandUtils.FindStraightSequences(cards);
        var mainSequence = sequences[0];

        Assert.Multiple(() => {
            Assert.That(mainSequence, Has.Count.EqualTo(5), "Should find T-A straight");
            Assert.That(mainSequence.Select(c => c.Rank).OrderBy(r => r),
                Is.EqualTo(new[] { 10, 11, 12, 13, 14 }), "Should be T-A sequence with Ace as 14");
        });
    }

    [Test]
    public void HandUtils_FindStraightSequences_WithGap() {
        var cards = CreateCards("8H", "9H", "JH", "QH", "KD", "2S");
        var sequences = HandUtils.FindStraightSequences(cards);
        var mainSequence = sequences[0];

        Assert.Multiple(() => {
            Assert.That(mainSequence, Has.Count.EqualTo(5), "Should find 8-K sequence with gap");
            Assert.That(mainSequence.Select(c => c.Rank).OrderBy(r => r),
                Is.EqualTo(new[] { 8, 9, 11, 12, 13 }), "Should be 8-K sequence with gap at 10");
        });
    }

    [Test]
    public void HandUtils_FindStraightSequences_MultipleSequences() {
        var cards = CreateCards("7H", "8H", "9H", "TH", "JD", "QS", "KH");
        var sequences = HandUtils.FindStraightSequences(cards);

        Assert.Multiple(() => {
            Assert.That(sequences, Has.Count.GreaterThan(1), "Should find multiple sequences");
            // Debería encontrar 7-J, 8-Q, y 9-K
            Assert.That(sequences[0].Count, Is.EqualTo(5), "First sequence should have 5 cards");
            Assert.That(sequences[0].Select(c => c.Rank).OrderBy(r => r),
                Is.EqualTo(new[] { 9, 10, 11, 12, 13 }), "Should prioritize highest straight 9-K");
        });
    }

    [Test]
    public void HandUtils_FindStraightSequences_SevenCardHand() {
        var cards = CreateCards("5H", "6H", "7H", "8H", "9D", "TS", "JH");
        var sequences = HandUtils.FindStraightSequences(cards);

        Assert.Multiple(() => {
            Assert.That(sequences, Has.Count.GreaterThan(1), "Should find multiple potential sequences");
            Assert.That(sequences[0].Count, Is.EqualTo(5), "Best sequence should have 5 cards");
            Assert.That(sequences[0].Select(c => c.Rank).OrderBy(r => r),
                Is.EqualTo(new[] { 7, 8, 9, 10, 11 }), "Should prioritize highest straight 7-J");
        });
    }

    [Test]
    public void StraightHand_SuggestDiscards_WithSevenCards() {
        var currentHand = CreateCards("5H", "6H", "7H", "8H", "9D", "TS", "JH");
        var hand = new StraightHand(HandsManager, []);
        var discards = hand.SuggestDiscards(new PokerHandAnalysis(Handler.Config, currentHand), 2)[0];

        Assert.Multiple(() => {
            Assert.That(discards, Has.Count.EqualTo(2), "Should discard 2 cards");
            var remainingCards = currentHand.Except(discards).ToList();
            Assert.That(HandUtils.FindStraightSequences(remainingCards)[0],
                Has.Count.GreaterThanOrEqualTo(4),
                "Should keep at least 4 connected cards");
        });
    }

    [Test]
    public void StraightHand_SuggestDiscards_WithGap() {
        var currentHand = CreateCards("8H", "9H", "JH", "QH", "KD", "2S", "3D");
        var hand = new StraightHand(HandsManager, []);
        var discards = hand.SuggestDiscards(new PokerHandAnalysis(Handler.Config, currentHand), 2)[0];

        Assert.Multiple(() => {
            Assert.That(discards, Has.Count.EqualTo(2), "Should discard 2 cards");
            var remainingCards = currentHand.Except(discards).ToList();
            var sequences = HandUtils.FindStraightSequences(remainingCards);
            Assert.That(sequences, Is.Not.Empty, "Should find at least one sequence");
            Assert.That(sequences[0], Has.Count.GreaterThanOrEqualTo(4),
                "Should keep at least 4 connected cards");
            Assert.That(sequences[0].Max(c => c.Rank), Is.GreaterThanOrEqualTo(10),
                "Should keep high value cards in sequence");
            // Verificar que descarta las cartas bajas
            Assert.That(discards, Does.Contain(currentHand.First(c => c.Rank == 2)),
                "Should discard 2");
            Assert.That(discards, Does.Contain(currentHand.First(c => c.Rank == 3)),
                "Should discard 3");
        });
    }

    [Test]
    public void StraightHand_SuggestDiscards_WithLowAce() {
        var currentHand = CreateCards("AH", "2H", "3H", "4D", "5S", "KH", "QD");
        var hand = new StraightHand(HandsManager, []);
        var discards = hand.SuggestDiscards(new PokerHandAnalysis(Handler.Config, currentHand), 2)[0];

        Assert.Multiple(() => {
            Assert.That(discards, Has.Count.EqualTo(2), "Should discard 2 cards");
            var remainingCards = currentHand.Except(discards).ToList();
            var sequences = HandUtils.FindStraightSequences(remainingCards);

            Assert.That(sequences, Is.Not.Empty, "Should find at least one sequence");
            Assert.That(sequences[0], Has.Count.EqualTo(5), "Should keep complete A-5 straight");

            // Verificar que tenemos todas las cartas necesarias para A-5
            var ranks = sequences[0].Select(c => c.Rank == 14 ? 1 : c.Rank).OrderBy(r => r).ToList();
            Assert.That(ranks, Is.EqualTo(new[] { 1, 2, 3, 4, 5 }), "Should have A-5 sequence");

            // Verificar que descartamos K,Q
            Assert.That(discards, Does.Contain(currentHand.First(c => c.Rank == 13)), "Should discard King");
            Assert.That(discards, Does.Contain(currentHand.First(c => c.Rank == 12)), "Should discard Queen");
        });
    }

    [Test]
    public void StraightHand_SuggestDiscards_WithHighAce() {
        var currentHand = CreateCards("AH", "KH", "QH", "JD", "TS", "2H", "3D");
        var hand = new StraightHand(HandsManager, []);
        var discards = hand.SuggestDiscards(new PokerHandAnalysis(Handler.Config, currentHand), 2)[0];

        Assert.Multiple(() => {
            Assert.That(discards, Has.Count.EqualTo(2), "Should discard 2 cards");
            var remainingCards = currentHand.Except(discards).ToList();
            var sequences = HandUtils.FindStraightSequences(remainingCards);
            Assert.That(sequences[0], Has.Count.EqualTo(5),
                "Should keep complete T-A straight");
            Assert.That(sequences[0].Select(c => c.Rank).Max(), Is.EqualTo(14),
                "Should keep Ace as highest card");
        });
    }

    [Test]
    public void StraightHand_SuggestDiscards_MaxDiscardsRespected() {
        var currentHand = CreateCards("2H", "3H", "7H", "8D", "9S", "TH", "JD", "QD", "KD");
        var maxDiscards = 5;
        var hand = new StraightHand(HandsManager, []);
        var discards = hand.SuggestDiscards(new PokerHandAnalysis(Handler.Config, currentHand), maxDiscards)[0];

        Assert.Multiple(() => {
            Assert.That(discards, Has.Count.LessThanOrEqualTo(maxDiscards),
                "Should respect maxDiscards limit");
            var remainingCards = currentHand.Except(discards).ToList();
            var sequences = HandUtils.FindStraightSequences(remainingCards);
            Assert.That(sequences[0], Has.Count.GreaterThanOrEqualTo(4),
                "Should keep at least 4 connected cards");
            Assert.That(remainingCards.Count, Is.GreaterThanOrEqualTo(5),
                "Should keep enough cards for a straight");
        });
    }


    private List<Card> GetSampleHandForType(PokerHand prototype) {
        // Devuelve una mano de ejemplo para cada tipo
        return prototype switch {
            HighCardHand => CreateCards("AS", "KH", "QD", "JC", "TD"),
            PairHand => CreateCards("AS", "AH", "KD", "QC", "TD"),
            TwoPairHand => CreateCards("AS", "AH", "KS", "KH", "TD"),
            ThreeOfAKindHand => CreateCards("AS", "AH", "AD", "KH", "TD"),
            StraightHand => CreateCards("9H", "TH", "JH", "QH", "KH"),
            FlushHand => CreateCards("AH", "KH", "QH", "JH", "TH"),
            FullHouseHand => CreateCards("AS", "AH", "AD", "KS", "KH"),
            FourOfAKindHand => CreateCards("AS", "AH", "AD", "AC", "KH"),
            StraightFlushHand => CreateCards("9H", "TH", "JH", "QH", "KH"),
            FlushHouseHand => CreateCards("AS", "AS", "AS", "KS", "KS"),
            FiveOfAKindHand => CreateCards("AH", "AS", "AD", "AC", "AH", "QC"),
            FlushFiveHand => CreateCards("AH", "AH", "AH", "AH", "AH", "QC"),
            _ => throw new ArgumentException($"Unknown hand type: {prototype.GetType().Name}")
        };
    }

    [Test]
    public void GetDiscardOptions_WithEmptyHand_ReturnsEmptyList() {
        var availableCards = CreateCards("2H", "3H", "4H", "5H", "6H");
        var result = HandsManager.GetDiscardOptions(Handler, [], availableCards, 3);

        Assert.That(result.Discards, Is.Empty);
    }

    [Test]
    public void GetDiscardOptions_WithNegativeMaxDiscards_ThrowsException() {
        var currentHand = CreateCards("2H", "3H", "4H");
        var availableCards = CreateCards("5H", "6H");

        Assert.Throws<ArgumentException>(() => HandsManager.GetDiscardOptions(Handler, currentHand, availableCards, -1));
    }

    [Test]
    public void GetDiscardOptions_WithNoAvailableCards_ReturnsEmptyList() {
        var currentHand = CreateCards("2H", "3H", "4H");
        var result = HandsManager.GetDiscardOptions(Handler, currentHand, [], 3);
        Assert.That(result.Discards, Is.Empty);
    }

    [Test]
    public void HandUtils_FindStraightSequences_WithDuplicateRanks() {
        var cards = CreateCards("8H", "8D", "9H", "TH", "JH");
        var sequences = HandUtils.FindStraightSequences(cards);
        var mainSequence = sequences[0];

        Assert.Multiple(() => {
            Assert.That(mainSequence, Has.Count.EqualTo(4),
                "Should find 4-card sequence ignoring duplicate rank");
            Assert.That(mainSequence.Select(c => c.Rank).Distinct().Count(),
                Is.EqualTo(mainSequence.Count),
                "Sequence should not contain duplicate ranks");
        });
    }

    [Test]
    public void FlushHouse_SuggestDiscards_ShouldKeepSameSuitAndRanks() {
        var currentHand = CreateCards("AH", "AH", "AH", "KH", "KH", "2S");
        var hand = new FlushHouseHand(HandsManager, []);
        var discards = hand.SuggestDiscards(new PokerHandAnalysis(Handler.Config, currentHand), 1)[0];

        Assert.Multiple(() => {
            Assert.That(discards, Has.Count.EqualTo(1), "Should discard 1 card");
            Assert.That(discards, Does.Contain(currentHand[5]), "Should discard 2S");
            Assert.That(discards.All(c => c.Suit != 'H'), "Should not discard any hearts");
        });
    }

    [Test]
    public void FlushFive_WhenComplete_ShouldNotSuggestDiscards() {
        var currentHand = CreateCards("AH", "AH", "AH", "AH", "AH");
        var hand = new FlushFiveHand(HandsManager, []);
        var discards = hand.SuggestDiscards(new PokerHandAnalysis(Handler.Config, currentHand), 1);

        Assert.That(discards, Is.Empty, "Should not suggest discards when FlushFive is complete");
    }


    [Test]
    public void DisabledHand_ShouldNotBeIdentified() {
        var currentHand = CreateCards("AS", "AH", "AD", "AC", "AH");
        HandsManager.RegisterHand(new FiveOfAKindHand(HandsManager, []), 120, 12, 40, 4, false);

        var hands = HandsManager.IdentifyAllHands(Handler, currentHand);

        Assert.That(hands.Count == 0 || !hands.Any(h => h is FiveOfAKindHand),
            "Should not identify disabled hand type");
    }

    [Test]
    public void PairHand_SuggestDiscards_WithPairOrBetter_ShouldNotSuggestDiscards() {
        // Test con par
        var pairHand = CreateCards("AS", "AH", "QD", "JC", "TD");
        var withPair = new PairHand(HandsManager, []);
        Assert.That(withPair.SuggestDiscards(new PokerHandAnalysis(Handler.Config, pairHand), 3), Is.Empty,
            "Should not suggest discards when we already have a pair");

        // Test con trío
        var threeKind = CreateCards("AS", "AH", "AD", "JC", "TD");
        Assert.That(withPair.SuggestDiscards(new PokerHandAnalysis(Handler.Config, threeKind), 3), Is.Empty,
            "Should not suggest discards when we already have three of a kind");

        // Test con póker
        var fourKind = CreateCards("AS", "AH", "AD", "AC", "TD");
        Assert.That(withPair.SuggestDiscards(new PokerHandAnalysis(Handler.Config, fourKind), 3), Is.Empty,
            "Should not suggest discards when we already have four of a kind");

        // Test con repóker
        var fiveKind = CreateCards("AS", "AH", "AD", "AC", "AH");
        Assert.That(withPair.SuggestDiscards(new PokerHandAnalysis(Handler.Config, fiveKind), 3), Is.Empty,
            "Should not suggest discards when we already have five of a kind");
    }

    [Test]
    public void TwoPairHand_SuggestDiscards_WithTwoPairOrBetter_ShouldNotSuggestDiscards() {
        // Test con doble pareja
        var twoPairHand = CreateCards("AS", "AH", "KS", "KH", "TD");
        var withTwoPair = new TwoPairHand(HandsManager, []);
        Assert.That(withTwoPair.SuggestDiscards(new PokerHandAnalysis(Handler.Config, twoPairHand), 3), Is.Empty,
            "Should not suggest discards when we already have two pair");

        // Test con trío
        var threeKind = CreateCards("AS", "AH", "AD", "JC", "TD");
        Assert.That(withTwoPair.SuggestDiscards(new PokerHandAnalysis(Handler.Config, threeKind), 3), Is.Empty,
            "Should not suggest discards when we already have three of a kind");

        // Test con póker
        var fourKind = CreateCards("AS", "AH", "AD", "AC", "TD");
        Assert.That(withTwoPair.SuggestDiscards(new PokerHandAnalysis(Handler.Config, fourKind), 3), Is.Empty,
            "Should not suggest discards when we already have four of a kind");
    }

    [Test]
    public void ThreeOfAKindHand_SuggestDiscards_WithThreeOrBetter_ShouldNotSuggestDiscards() {
        // Test con trío
        var threeKindHand = CreateCards("AS", "AH", "AD", "JC", "TD");
        var withThree = new ThreeOfAKindHand(HandsManager, []);
        Assert.That(withThree.SuggestDiscards(new PokerHandAnalysis(Handler.Config, threeKindHand), 3), Is.Empty,
            "Should not suggest discards when we already have three of a kind");

        // Test con póker
        var fourKind = CreateCards("AS", "AH", "AD", "AC", "TD");
        Assert.That(withThree.SuggestDiscards(new PokerHandAnalysis(Handler.Config, fourKind), 3), Is.Empty,
            "Should not suggest discards when we already have four of a kind");

        // Test con repóker
        var fiveKind = CreateCards("AS", "AH", "AD", "AC", "AH");
        Assert.That(withThree.SuggestDiscards(new PokerHandAnalysis(Handler.Config, fiveKind), 3), Is.Empty,
            "Should not suggest discards when we already have five of a kind");
    }

    [Test]
    public void FourOfAKindHand_SuggestDiscards_WithFourOrBetter_ShouldNotSuggestDiscards() {
        // Test con póker
        var fourKindHand = CreateCards("AS", "AH", "AD", "AC", "TD");
        var withFour = new FourOfAKindHand(HandsManager, []);
        Assert.That(withFour.SuggestDiscards(new PokerHandAnalysis(Handler.Config, fourKindHand), 3), Is.Empty,
            "Should not suggest discards when we already have four of a kind");

        // Test con repóker
        var fiveKind = CreateCards("AS", "AH", "AD", "AC", "AH");
        Assert.That(withFour.SuggestDiscards(new PokerHandAnalysis(Handler.Config, fiveKind), 3), Is.Empty,
            "Should not suggest discards when we already have five of a kind");
    }

    [Test]
    public void FiveOfAKindHand_SuggestDiscards_WithFiveOfAKind_ShouldNotSuggestDiscards() {
        var fiveKindHand = CreateCards("AS", "AH", "AD", "AC", "AH");
        var withFive = new FiveOfAKindHand(HandsManager, []);
        Assert.That(withFive.SuggestDiscards(new PokerHandAnalysis(Handler.Config, fiveKindHand), 3), Is.Empty,
            "Should not suggest discards when we already have five of a kind");
    }

    [Test]
    public void PairHand_SuggestDiscards_WithNoPair_ShouldDiscardNonDuplicates() {
        var currentHand = CreateCards("AH", "KH", "QD", "JC", "TD");
        var hand = new PairHand(HandsManager, []);
        var discards = hand.SuggestDiscards(new PokerHandAnalysis(Handler.Config, currentHand), 2)[0];

        Assert.Multiple(() => {
            Assert.That(discards, Has.Count.EqualTo(2), "Should discard 2 lowest cards");
            Assert.That(discards, Does.Contain(currentHand[4]), "Should discard TD");
            Assert.That(discards, Does.Contain(currentHand[3]), "Should discard JC");
        });
    }
}*/