using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Deck;
using Betauer.Core.Deck.Hands;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class HandImprovementTest {
    protected PokerHands Hands;

    [SetUp]
    public void Setup() {
        Hands = new PokerHands();
        Hands.RegisterBasicPokerHands();
    }

    protected List<Card> CreateCards(params string[] cards) {
        var parse = new PokerGameConfig().Parse;
        return cards.Select(parse).ToList();
    }

    [Test]
    public void PairHand_WithHighCard_ShouldFindPotentialPair() {
        var currentHand = CreateCards("AS", "KH", "QD");
        var availableCards = CreateCards("AH", "3H", "4H", "5H", "6H", "7H"); // Más cartas para simular una baraja real

        var hand = new PairHand(Hands, []);
        var improvements = hand.FindPossibleImprovements(currentHand, availableCards, 2);

        Assert.Multiple(() => {
            Assert.That(improvements.Count, Is.EqualTo(2)); // 2 formas de descartar (KH o QD)
            var bestImprovement = improvements.OrderByDescending(i => i.Score).First();
            Assert.That(bestImprovement.CardsToKeep.Single().ToString(), Is.EqualTo("AS"));
            Assert.That(bestImprovement.Probability, Is.EqualTo(1.0 / 6)); // 1 as de 6 cartas
            Assert.That(bestImprovement.TargetHand.Cards.Count, Is.EqualTo(2));
            Assert.That(bestImprovement.TargetHand.Cards.All(c => c.Rank == 14), Is.True);
        });
    }

    [Test]
    public void PairHand_WithNoMatchingCardsAvailable_ShouldReturnEmpty() {
        var currentHand = CreateCards("AS", "KH", "QD");
        var availableCards = CreateCards("JH", "TD", "9H", "8H", "7H", "6H"); // No hay cartas que coincidan

        var hand = new PairHand(Hands, []);
        var improvements = hand.FindPossibleImprovements(currentHand, availableCards, 2);

        Assert.That(improvements, Is.Empty);
    }

    [Test]
    public void PairHand_WithMaxDiscardsExceeded_ShouldLimitOptions() {
        var currentHand = CreateCards("AS", "KH", "QD", "JD", "TD");
        var availableCards = CreateCards("AH", "AD", "3H", "4H", "5H", "6H");
        var maxDiscards = 2;

        var hand = new PairHand(Hands, []);
        var improvements = hand.FindPossibleImprovements(currentHand, availableCards, maxDiscards);

        Assert.That(improvements.All(i => i.CardsToDiscard.Count <= maxDiscards), Is.True);
    }

    [Test]
    public void TwoPairHand_FromPair_WithMultipleOptions_ShouldFindAllPossibilities() {
        var currentHand = CreateCards("AS", "AH", "KH", "QD", "JD");
        var availableCards = CreateCards("KS", "KD", "QS", "QH", "3H", "4H", "5H", "6H");

        var hand = new TwoPairHand(Hands, []);
        var improvements = hand.FindPossibleImprovements(currentHand, availableCards, 2);

        // Debería encontrar mejoras tanto para K como para Q
        var kImprovements = improvements.Where(i => i.TargetHand.Cards.Count(c => c.Rank == 13) == 2);
        var qImprovements = improvements.Where(i => i.TargetHand.Cards.Count(c => c.Rank == 12) == 2);

        Assert.Multiple(() => {
            Assert.That(kImprovements, Is.Not.Empty, "Should find K pair improvements");
            Assert.That(qImprovements, Is.Not.Empty, "Should find Q pair improvements");
            Assert.That(improvements.First().CardsToKeep.Count(c => c.Rank == 14), Is.EqualTo(2), "Should keep the A pair");

            // Con 8 cartas disponibles y 2 reyes entre ellas
            Assert.That(kImprovements.First().Probability, Is.EqualTo(2.0 / 8), "Should calculate correct probability for K pair");
            // Con 8 cartas disponibles y 2 reinas entre ellas
            Assert.That(qImprovements.First().Probability, Is.EqualTo(2.0 / 8), "Should calculate correct probability for Q pair");
        });
    }

    [Test]
    public void ThreeOfAKind_WithPair_DifferentProbabilities() {
        var currentHand = CreateCards("AS", "AH", "KH", "QD"); // Par de ases
        var availableCards = CreateCards("AD", "KS", "KD", "3H", "4H", "5H"); // Un as y dos reyes entre 6 cartas

        var hand = new ThreeOfAKindHand(Hands, []);
        var improvements = hand.FindPossibleImprovements(currentHand, availableCards, 2);

        var aceImprovements = improvements.Where(i => i.TargetHand.Cards.All(c => c.Rank == 14));
        var kingImprovements = improvements.Where(i => i.TargetHand.Cards.All(c => c.Rank == 13));

        Assert.Multiple(() => {
            Assert.That(improvements, Is.Not.Empty, "Should find some improvements");

            // Para el trío de ases (1 as de 6 cartas disponibles)
            Assert.That(aceImprovements.First().Probability, Is.EqualTo(1.0 / 6),
                "Probability for Aces should be 1/6 (one ace in six available cards)");

            // Para el trío de reyes (2 reyes de 6 cartas disponibles)
            Assert.That(kingImprovements.First().Probability, Is.EqualTo(2.0 / 6),
                "Probability for Kings should be 2/6 (two kings in six available cards)");
        });
    }

    [Test]
    public void StraightHand_WithConnectedCards_MultiplePossibilities() {
        // Tenemos 4 cartas conectadas: T-J-Q-K
        var currentHand = CreateCards("TH", "JH", "QH", "KH", "2S");
        // Dos opciones para completar la escalera: 9 para T-K o A para T-A, más otras cartas
        var availableCards = CreateCards("9H", "AH", "3H", "4H", "5H", "6H", "7H", "8D");

        var hand = new StraightHand(Hands, []);
        var improvements = hand.FindPossibleImprovements(currentHand, availableCards, 1);

        Assert.Multiple(() => {
            Assert.That(improvements.Count, Is.EqualTo(2), "Should find two possible improvements");

            // Verificar que guardamos las 4 cartas conectadas
            Assert.That(improvements.All(i => i.CardsToKeep.Count == 4), "Should keep 4 connected cards");
            Assert.That(improvements.All(i => i.CardsToKeep.Contains(currentHand[0])), "Should keep Ten");
            Assert.That(improvements.All(i => i.CardsToKeep.Contains(currentHand[1])), "Should keep Jack");
            Assert.That(improvements.All(i => i.CardsToKeep.Contains(currentHand[2])), "Should keep Queen");
            Assert.That(improvements.All(i => i.CardsToKeep.Contains(currentHand[3])), "Should keep King");

            // Verificar que descartamos el 2S
            Assert.That(improvements.All(i => i.CardsToDiscard.Single().ToString() == "2S"), "Should discard 2S");

            // Verificar las dos posibles escaleras
            var targetHands = improvements.Select(i => i.TargetHand.Cards.ToList()).ToList();
            Assert.That(targetHands.Count, Is.EqualTo(2), "Should have two different target hands");

            // Una escalera debería ser 9-T-J-Q-K
            var firstStraight = targetHands.Any(h => h.Select(c => c.Rank).OrderBy(r => r)
                .SequenceEqual(new[] { 9, 10, 11, 12, 13 }));

            // La otra escalera debería ser T-J-Q-K-A
            var secondStraight = targetHands.Any(h => h.Select(c => c.Rank).OrderBy(r => r)
                .SequenceEqual(new[] { 10, 11, 12, 13, 14 }));

            Assert.That(firstStraight, Is.True, "Should find 9-K straight");
            Assert.That(secondStraight, Is.True, "Should find T-A straight");

            // Verificar probabilidades (1 carta de 8 disponibles para cada mejora)
            Assert.That(improvements.All(i => i.Probability == 1.0 / 8), "Each improvement should have 1/8 probability");
        });
    }

    [Test]
    public void FlushHand_WithFourOfSuit_SingleOption() {
        // Tenemos 4 corazones y queremos hacer color
        var currentHand = CreateCards("AH", "KH", "QH", "JH", "2S");
        // Solo hay un corazón disponible entre varias cartas
        var availableCards = CreateCards("TH", "3D", "4D", "5D", "6D", "7D", "8D", "9D");

        var hand = new FlushHand(Hands, []);
        var improvements = hand.FindPossibleImprovements(currentHand, availableCards, 1);

        Assert.Multiple(() => {
            Assert.That(improvements.Count, Is.EqualTo(1), "Should find exactly one improvement");
            var improvement = improvements[0];
            // Verificamos que guardamos los 4 corazones
            Assert.That(improvement.CardsToKeep.Count, Is.EqualTo(4), "Should keep 4 hearts");
            Assert.That(improvement.CardsToKeep.All(c => c.Suit == 'H'), "Should keep only hearts");
            // Verificamos que descartamos el 2S
            Assert.That(improvement.CardsToDiscard.Single().ToString(), Is.EqualTo("2S"));
            // Una carta favorable de 8 disponibles
            Assert.That(improvement.Probability, Is.EqualTo(1.0 / 8), "Probability should be 1/8 (one heart in eight available cards)");
            // La mano objetivo debe tener 5 cartas del mismo palo
            Assert.That(improvement.TargetHand.Cards.Count, Is.EqualTo(5), "Target hand should have 5 cards");
            Assert.That(improvement.TargetHand.Cards.All(c => c.Suit == 'H'), "Target hand should be all hearts");
        });
    }

    [Test]
    public void FullHouse_FromThreeOfAKind_MultiplePairOptions() {
        var currentHand = CreateCards("AS", "AH", "AD", "KH", "QD");
        var availableCards = CreateCards("KS", "KD", "QS", "QH", "3H", "4H", "5H", "6H");

        var hand = new FullHouseHand(Hands, []);
        var improvements = hand.FindPossibleImprovements(currentHand, availableCards, 2);

        var kPairImprovements = improvements.Where(i => i.TargetHand.Cards.Count(c => c.Rank == 13) == 2);
        var qPairImprovements = improvements.Where(i => i.TargetHand.Cards.Count(c => c.Rank == 12) == 2);

        Assert.Multiple(() => {
            Assert.That(kPairImprovements, Is.Not.Empty, "Should find K pair improvements");
            Assert.That(qPairImprovements, Is.Not.Empty, "Should find Q pair improvements");
            Assert.That(improvements.All(i => i.TargetHand.Cards.Count(c => c.Rank == 14) == 3),
                "Should keep three Aces");

            // 2 reyes entre 8 cartas disponibles para completar el par
            Assert.That(kPairImprovements.First().Probability, Is.EqualTo(1.0 / 8 * 1.0 / 7).Within(0.0001),
                "Probability for K pair should be (2/8)*(1/7)");
            // 2 reinas entre 8 cartas disponibles para completar el par
            Assert.That(qPairImprovements.First().Probability, Is.EqualTo(1.0 / 8 * 1.0 / 7).Within(0.0001),
                "Probability for Q pair should be (2/8)*(1/7)");
        });
    }

    [Test]
    public void FullHouse_FromTwoPairs_ShouldFindImprovements() {
        var currentHand = CreateCards("AS", "AH", "KS", "KH", "QD");
        var availableCards = CreateCards("AD", "KD", "3H", "4H", "5H", "6H", "7H", "8H");

        var hand = new FullHouseHand(Hands, []);
        var improvements = hand.FindPossibleImprovements(currentHand, availableCards, 1);

        var aceImprovements = improvements.Where(i => i.TargetHand.Cards.Count(c => c.Rank == 14) == 3);
        var kingImprovements = improvements.Where(i => i.TargetHand.Cards.Count(c => c.Rank == 13) == 3);

        Assert.Multiple(() => {
            Assert.That(aceImprovements, Is.Not.Empty, "Should find Ace improvements");
            Assert.That(kingImprovements, Is.Not.Empty, "Should find King improvements");

            // 1 as entre 8 cartas disponibles
            Assert.That(aceImprovements.First().Probability, Is.EqualTo(1.0 / 8),
                "Probability for Ace should be 1/8");
            // 1 rey entre 8 cartas disponibles
            Assert.That(kingImprovements.First().Probability, Is.EqualTo(1.0 / 8),
                "Probability for King should be 1/8");
        });
    }

    [Test]
    public void StraightFlushHand_WithFourConnected_SameSuit() {
        var currentHand = CreateCards("9H", "TH", "JH", "QH", "2S");
        var availableCards = CreateCards("8H", "3D", "4D", "5D", "6D", "7D", "KD", "AD");

        var hand = new StraightFlushHand(Hands, []);
        var improvements = hand.FindPossibleImprovements(currentHand, availableCards, 1);

        Assert.Multiple(() => {
            Assert.That(improvements.Count, Is.EqualTo(1));
            var improvement = improvements[0];
            Assert.That(improvement.CardsToKeep.All(c => c.Suit == 'H'), Is.True);
            Assert.That(improvement.CardsToDiscard.Single().ToString(), Is.EqualTo("2S"));
            Assert.That(improvement.TargetHand.Cards.All(c => c.Suit == 'H'), Is.True);
            Assert.That(IsValidStraight(improvement.TargetHand.Cards), Is.True);
            // Un 8 de corazones entre 8 cartas disponibles
            Assert.That(improvement.Probability, Is.EqualTo(1.0 / 8), "Probability should be 1/8");
        });
    }

    private bool IsValidStraight(IReadOnlyList<Card> cards) {
        var ranks = cards.Select(c => c.Rank).OrderBy(r => r).ToList();
        return ranks.Count == 5 && ranks[4] - ranks[0] == 4;
    }
}