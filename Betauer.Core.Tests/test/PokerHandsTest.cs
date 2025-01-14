using System;
using Betauer.Core.Deck;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Deck.Hands;
using Betauer.TestRunner;

namespace Betauer.Core.Tests;

[TestFixture]
public class PokerHandsTest {
    protected PokerHandsManager HandsManager => Handler.PokerHandsManager;
    protected GameStateHandler Handler;

    [SetUp]
    public void Setup() {
        Handler = new GameStateHandler(0, new PokerGameConfig());
        HandsManager.RegisterBasicPokerHands();
    }

    protected List<Card> CreateCards(params string[] cards) {
        var parse = new PokerGameConfig().Parse;
        return cards.Select(parse).ToList();
    }

    [Test]
    public void PairFromThreeOfAKind_ShouldFindAllPossiblePairs() {
        var cards = CreateCards("AS", "AH", "AD", "KH", "QD", "JC", "2C");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);

        var pairs = hands.Where(h => h is PairHand).ToList();
        Assert.That(pairs.Count, Is.EqualTo(1));
        Assert.That(pairs.All(h => h.Cards.Count == 2), Is.True);
        Assert.That(pairs.All(h => h.Cards.All(c => c.Rank == 14)), Is.True); // Todos son Ases
    }

    [Test]
    public void PairFromFourOfAKind_ShouldFindAllPossiblePairs() {
        var cards = CreateCards("AS", "AH", "AD", "AC", "KH", "KD", "QH", "QD");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var pairs = hands.Where(h => h is PairHand).ToList();
    
        Assert.Multiple(() => {
            Assert.That(pairs.Count, Is.EqualTo(3), "Should have exactly 3 pairs (Aces, Kings, Queens)");
            Assert.That(pairs.All(h => h.Cards.Count == 2), "All pairs should have 2 cards");
        
            // Verificar que tenemos un par de cada rank
            var ranks = pairs.Select(h => h.Cards[0].Rank).OrderByDescending(r => r).ToList();
            Assert.That(ranks, Is.EqualTo(new[] { 14, 13, 12 }), "Should have pairs of Aces, Kings and Queens");
        });
    }

    [Test]
    public void ThreeOfAKind_FromFourOfAKind_ShouldFindAllCombinations() {
        var cards = CreateCards("AS", "AH", "AD", "AC", "KH", "KD", "KC");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var threeOfKinds = hands.Where(h => h is ThreeOfAKindHand).ToList();
    
        Assert.Multiple(() => {
            Assert.That(threeOfKinds.Count, Is.EqualTo(2), "Should have exactly 2 three of a kinds (Aces and Kings)");
            Assert.That(threeOfKinds.All(h => h.Cards.Count == 3), "All three of a kinds should have 3 cards");
        
            // Verificar que tenemos un trío de cada rank
            var ranks = threeOfKinds.Select(h => h.Cards[0].Rank).OrderByDescending(r => r).ToList();
            Assert.That(ranks, Is.EqualTo(new[] { 14, 13 }), "Should have three of a kind of Aces and Kings");
        });
    }

    [Test]
    public void TwoPair_WithThreePairs_ShouldFindAllCombinations() {
        // Con tres pares (A,A,K,K,Q,Q) y una carta extra, debería encontrar 3 dobles parejas diferentes
        var cards = CreateCards("AS", "AH", "KS", "KH", "QS", "QH", "2C");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var twoPairs = hands.Where(h => h is TwoPairHand).ToList();

        Assert.That(twoPairs.Count, Is.EqualTo(3));

        // Verificar que tenemos todas las combinaciones (A-K, A-Q, K-Q)
        var pairCombinations = twoPairs
            .Select(h => string.Join("-", h.Cards
                .GroupBy(c => c.Rank)
                .Select(g => g.Key)
                .OrderByDescending(r => r)))
            .ToList();

        Assert.That(pairCombinations, Contains.Item("14-13")); // A-K
        Assert.That(pairCombinations, Contains.Item("14-12")); // A-Q
        Assert.That(pairCombinations, Contains.Item("13-12")); // K-Q
    }

    [Test]
    public void Flush_WithMoreThanFiveCards_ShouldFindAllCombinations() {
        // 6 cartas de corazones, debería generar un color con las primeras 5
        var cards = CreateCards("AH", "KH", "QH", "JH", "TH", "9H", "2D");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var flushes = hands.Where(h => h is FlushHand).ToList();

        Assert.Multiple(() => {
            // Debería haber solo un color
            Assert.That(flushes.Count, Is.EqualTo(1), "Should have exactly 1 flush");
            Assert.That(flushes[0].Cards.Count, Is.EqualTo(5), "Flush should have exactly 5 cards");
            Assert.That(flushes[0].Cards.All(c => c.Suit == 'H'), "All cards should be hearts");
        
            // Verificar que tenemos las cartas más altas (A,K,Q,J,10)
            var ranks = flushes[0].Cards.Select(c => c.Rank).OrderByDescending(r => r).ToList();
            Assert.That(ranks, Is.EqualTo(new[] { 14, 13, 12, 11, 10 }), 
                "Should have Ace, King, Queen, Jack, Ten of hearts");
        });
    }

    [Test]
    public void Straight_WithMoreThanFiveCards_ShouldFindAllCombinations() {
        // 6,7,8,9,10,J - debería generar 2 escaleras diferentes
        var cards = CreateCards("6H", "7H", "8H", "9H", "TH", "JH");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var straights = hands.Where(h => h is StraightHand).ToList();

        // Deberíamos tener 2 escaleras: 6-10 y 7-J
        Assert.That(straights.Count, Is.EqualTo(2));
    }

    [Test]
    public void FullHouse_WithFourOfEachRank_ShouldFindAllCombinations() {
        // 4 ases y 4 reyes deberían generar 48 full houses diferentes:
        // - Como trío de Ases:
        //   * 4C3 = 4 combinaciones para los tres ases
        //   * 4C2 = 6 combinaciones para los dos reyes
        //   * Total: 4 * 6 = 24 combinaciones
        // - Como trío de Reyes:
        //   * 4C3 = 4 combinaciones para los tres reyes
        //   * 4C2 = 6 combinaciones para los dos ases
        //   * Total: 4 * 6 = 24 combinaciones
        // Total final: 48 combinaciones diferentes
        var cards = CreateCards("AS", "AH", "AD", "AC", "KS", "KH", "KD", "KC");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var fullHouses = hands.Where(h => h is FullHouseHand).ToList();

        Assert.That(fullHouses.Count, Is.EqualTo(48));

        // Verificar que todas son diferentes
        var uniqueFullHouses = fullHouses.Select(h => string.Join(",", h.Cards.Select(c => c.ToString()).OrderBy(s => s)))
            .Distinct()
            .Count();
        Assert.That(uniqueFullHouses, Is.EqualTo(48));

        // Verificar que tenemos tanto tríos de Ases como de Reyes
        var trioAces = fullHouses.Count(h => h.Cards.Count(c => c.Rank == 14) == 3);
        var trioKings = fullHouses.Count(h => h.Cards.Count(c => c.Rank == 13) == 3);
        Assert.That(trioAces, Is.EqualTo(24)); // 24 full houses con trío de Ases
        Assert.That(trioKings, Is.EqualTo(24)); // 24 full houses con trío de Reyes
    }

    [Test]
    public void FourOfKind_WithAllSameSuit_ShouldFindOneHand() {
        var cards = CreateCards("AS", "AH", "AD", "AC", "KH");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var fourOfKinds = hands.Where(h => h is FourOfAKindHand).ToList();

        // Solo debería haber una combinación de póker
        Assert.That(fourOfKinds.Count, Is.EqualTo(1));
    }

    [Test]
    public void FourOfKind_WithMultipleGroups_ShouldFindAllHands() {
        Assert.Multiple(() => {
            // Caso con 5 ases y 5 reyes - debería generar dos FourOfAKind
            var cards = CreateCards("AS", "AH", "AD", "AC", "AS", "KS", "KH", "KD", "KC", "KH");
            var hands = HandsManager.IdentifyAllHands(Handler, cards)
                .Where(h => h is FourOfAKindHand)
                .OrderByDescending(h => h.Cards[0].Rank)
                .ToList();

            Assert.That(hands.Count, Is.EqualTo(2), "Should have exactly 2 hands");
        
            // Verificar el FourOfAKind de Ases
            Assert.That(hands[0].Cards.Count, Is.EqualTo(4), "First hand should have 4 cards");
            Assert.That(hands[0].Cards.All(c => c.Rank == 14), "First hand should be all Aces");
        
            // Verificar el FourOfAKind de Reyes
            Assert.That(hands[1].Cards.Count, Is.EqualTo(4), "Second hand should have 4 cards");
            Assert.That(hands[1].Cards.All(c => c.Rank == 13), "Second hand should be all Kings");

            // Imprimir las manos para debugging
            Console.WriteLine("Four of a Kind hands:");
            foreach (var hand in hands) {
                Console.WriteLine(string.Join(", ", hand.Cards));
            }
        });
    }
    [Test]
    public void StraightFlush_WithSixCards_ShouldFindTwoCombinations() {
        // 6♠,7♠,8♠,9♠,10♠,J♠ debería generar 2 escaleras de color: 6-10 y 7-J
        var cards = CreateCards("6S", "7S", "8S", "9S", "TS", "JS");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var straightFlushes = hands.Where(h => h is StraightFlushHand).ToList();

        Assert.That(straightFlushes.Count, Is.EqualTo(2));

        // Verificar las dos combinaciones
        var lowestRanks = straightFlushes.Select(h => h.Cards.Min(c => c.Rank)).OrderBy(r => r).ToList();
        Assert.That(lowestRanks[0], Is.EqualTo(6)); // Primera escalera empieza en 6
        Assert.That(lowestRanks[1], Is.EqualTo(7)); // Segunda escalera empieza en 7
    }

    [Test]
    public void TwoPair_WithThreePairsAndExtra_ShouldFindAllCombinations() {
        // Con tres pares (A,A,K,K,Q,Q) y una carta extra, debería encontrar 3 dobles parejas diferentes
        var cards = CreateCards("AS", "AH", "KS", "KH", "QS", "QH", "2C");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var twoPairs = hands.Where(h => h is TwoPairHand).ToList();

        Assert.That(twoPairs.Count, Is.EqualTo(3));

        // Verificar que tenemos todas las combinaciones (A-K, A-Q, K-Q)
        var pairCombinations = twoPairs
            .Select(h => string.Join("-", h.Cards
                .GroupBy(c => c.Rank)
                .Select(g => g.Key)
                .OrderByDescending(r => r)))
            .ToList();

        Assert.That(pairCombinations, Contains.Item("14-13")); // A-K
        Assert.That(pairCombinations, Contains.Item("14-12")); // A-Q
        Assert.That(pairCombinations, Contains.Item("13-12")); // K-Q
    }

    [Test]
    public void FullHouse_WithMultipleOptions_ShouldFindAllCombinations() {
        var cards = CreateCards("AS", "AH", "AD", "KS", "KH", "QS", "QH");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);

        var fullHouses = hands.Where(h => h is FullHouseHand).ToList();
        Assert.That(fullHouses.Count, Is.EqualTo(2)); // AAA-KK y AAA-QQ

        // Verificar que tenemos las combinaciones correctas
        var combinations = fullHouses
            .Select(h => (
                Three: h.Cards.GroupBy(c => c.Rank).First(g => g.Count() == 3).Key,
                Two: h.Cards.GroupBy(c => c.Rank).First(g => g.Count() == 2).Key
            ))
            .ToList();

        Assert.That(combinations, Has.Member((Three: 14, Two: 13))); // AAA-KK
        Assert.That(combinations, Has.Member((Three: 14, Two: 12))); // AAA-QQ
    }

    [Test]
    public void FullHouse_WithFourOfAKind_ShouldFindAllCombinations() {
        var cards = CreateCards("AS", "AH", "AD", "AC", "KS", "KH", "2C");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);

        var fullHouses = hands.Where(h => h is FullHouseHand).ToList();
        Assert.That(fullHouses.Count, Is.EqualTo(4)); // Cuatro posibles AAA-KK
        Assert.That(fullHouses.All(h => h.Cards.Count == 5), Is.True);

        // Todos deben tener tres Ases y dos Reyes
        Assert.That(fullHouses.All(h =>
            h.Cards.Count(c => c.Rank == 14) == 3 &&
            h.Cards.Count(c => c.Rank == 13) == 2), Is.True);
    }

    [Test]
    public void IdentifyAllHands_WithFourOfAKind_ShouldFindAllSubsets() {
        var cards = CreateCards("AS", "AH", "AD", "AC", "KH", "QD", "JC");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);

        // Verificar todas las manos posibles
        Assert.That(hands.Count(h => h is FourOfAKindHand), Is.EqualTo(1));
        Assert.That(hands.Count(h => h is ThreeOfAKindHand), Is.EqualTo(1));
        Assert.That(hands.Count(h => h is PairHand), Is.EqualTo(1));

        // Verificar orden correcto por multiplicador
        Assert.That(hands.First(), Is.TypeOf<FourOfAKindHand>());
    }

    [Test]
    public void IdentifyAllHands_WithFullHouse_ShouldFindAllSubsets() {
        var cards = CreateCards("AS", "AH", "AD", "KS", "KH", "QD", "JC");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);

        Assert.Multiple(() => {
            // Verificar todas las manos posibles
            Assert.That(hands.Count(h => h is FullHouseHand), Is.EqualTo(1), "Should have one Full House");
            Assert.That(hands.Count(h => h is ThreeOfAKindHand), Is.EqualTo(1), "Should have one Three of a Kind");
            Assert.That(hands.Count(h => h is TwoPairHand), Is.EqualTo(1), "Should have one Two Pair");
            Assert.That(hands.Count(h => h is PairHand), Is.EqualTo(2), "Should have two Pairs (one of Aces, one of Kings)");

            // Verificar los ranks de los pares
            var pairRanks = hands.Where(h => h is PairHand)
                .Select(h => h.Cards[0].Rank)
                .OrderByDescending(r => r)
                .ToList();
            Assert.That(pairRanks, Is.EqualTo(new[] { 14, 13 }), "Should have pairs of Aces and Kings");
        
            // Verificar que cada par tiene exactamente 2 cartas
            Assert.That(hands.Where(h => h is PairHand)
                .All(h => h.Cards.Count == 2), "Each pair should have exactly 2 cards");
        });
    }

    [Test]
    public void IdentifyAllHands_WithThreePairs_ShouldRankCorrectly() {
        var cards = CreateCards("AS", "AH", "KS", "KH", "QS", "QH", "JC");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);

        var twoPairs = hands.Where(h => h is TwoPairHand).ToList();
        Assert.That(twoPairs.Count, Is.EqualTo(3));

        // El primer Two Pair debería ser el de mayor valor (A-K)
        var firstTwoPair = twoPairs.First();
        var ranks = firstTwoPair.Cards.Select(c => c.Rank).OrderByDescending(r => r).ToList();
        Assert.That(ranks[0], Is.EqualTo(14)); // As
        Assert.That(ranks[2], Is.EqualTo(13)); // Rey
    }

    [Test]
    public void FiveOfAKind_WithMoreThanFiveCards_ShouldFindAllCombinations() {
        Assert.Multiple(() => {
            // Caso 1: 6 ases deberían generar un FiveOfAKind
            var cards1 = CreateCards("AS", "AH", "AD", "AC", "AH", "AS");
            var hands1 = HandsManager.IdentifyAllHands(Handler, cards1)
                .Where(h => h is FiveOfAKindHand).ToList();

            Assert.That(hands1.Count, Is.EqualTo(1), "Should have exactly 1 hand with Aces");
            Assert.That(hands1[0].Cards.Count, Is.EqualTo(5), "Hand should have 5 cards");
            Assert.That(hands1[0].Cards.All(c => c.Rank == 14), "All cards should be Aces");

            // Caso 2: 6 ases y 5 reyes deberían generar dos FiveOfAKind
            var cards2 = CreateCards("AS", "AH", "AD", "AC", "AH", "AS", "KS", "KH", "KD", "KC", "KH");
            var hands2 = HandsManager.IdentifyAllHands(Handler, cards2)
                .Where(h => h is FiveOfAKindHand)
                .OrderByDescending(h => h.Cards[0].Rank)
                .ToList();

            Assert.That(hands2.Count, Is.EqualTo(2), "Should have exactly 2 hands");
        
            // Verificar el FiveOfAKind de Ases
            Assert.That(hands2[0].Cards.Count, Is.EqualTo(5), "First hand should have 5 cards");
            Assert.That(hands2[0].Cards.All(c => c.Rank == 14), "First hand should be all Aces");
        
            // Verificar el FiveOfAKind de Reyes
            Assert.That(hands2[1].Cards.Count, Is.EqualTo(5), "Second hand should have 5 cards");
            Assert.That(hands2[1].Cards.All(c => c.Rank == 13), "Second hand should be all Kings");

        });
    }

    [Test]
    public void FlushHouse_CombinationCases() {
        Assert.Multiple(() => {
            // Caso 1: Exactamente 3 Ases y 2 Reyes del mismo suit - debe generar una sola mano
            var cards1 = CreateCards("AH", "AH", "AH", "KH", "KH");
            var hands1 = HandsManager.IdentifyAllHands(Handler, cards1)
                .Where(h => h is FlushHouseHand).ToList();
            Assert.That(hands1.Count, Is.EqualTo(1), "With exactly 3 Aces and 2 Kings should generate only one hand");

            // Caso 2: 3 Ases y 3 Reyes del mismo suit - debe generar dos manos
            var cards2 = CreateCards("AH", "AH", "AH", "KH", "KH", "KH");
            var hands2 = HandsManager.IdentifyAllHands(Handler, cards2)
                .Where(h => h is FlushHouseHand).ToList();
            Assert.That(hands2.Count, Is.EqualTo(2), "With 3 Aces and 3 Kings should generate two hands");

            // Verificar la estructura de cada mano en el caso 2
            var ranks2 = hands2.Select(h => {
                var groups = h.Cards.GroupBy(c => c.Rank).OrderByDescending(g => g.Count()).ToList();
                return (Three: groups[0].Key, Two: groups[1].Key);
            }).ToList();

            Assert.That(ranks2, Has.Member((Three: 14, Two: 13))); // AAA-KK
            Assert.That(ranks2, Has.Member((Three: 13, Two: 14))); // KKK-AA

            // Caso 3: 4 Ases y 3 Reyes del mismo suit - debe generar dos manos
            var cards3 = CreateCards("AH", "AH", "AH", "AH", "KH", "KH", "KH");
            var hands3 = HandsManager.IdentifyAllHands(Handler, cards3)
                .Where(h => h is FlushHouseHand).ToList();
            Assert.That(hands3.Count, Is.EqualTo(2), "With 4 Aces and 3 Kings should generate two hands");

            // Caso 4: 4 Ases y 4 Reyes del mismo suit - debe generar dos manos
            var cards4 = CreateCards("AH", "AH", "AH", "AH", "KH", "KH", "KH", "KH");
            var hands4 = HandsManager.IdentifyAllHands(Handler, cards4)
                .Where(h => h is FlushHouseHand).ToList();
            Assert.That(hands4.Count, Is.EqualTo(2), "With 4 Aces and 4 Kings should generate two hands");

            // Verificar que todas las manos tienen el mismo suit
            var allHands = hands1.Concat(hands2).Concat(hands3).Concat(hands4);
            Assert.That(allHands.All(h => h.Cards.All(c => c.Suit == 'H')), Is.True, "All cards should be hearts");

            // Verificar la estructura del flush house (3+2)
            Assert.That(allHands.All(h => {
                var groups = h.Cards.GroupBy(c => c.Rank).OrderByDescending(g => g.Count()).ToList();
                return groups.Count == 2 && groups[0].Count() == 3 && groups[1].Count() == 2;
            }), Is.True, "Each hand should have exactly three of one rank and two of another");
        });
    }

    [Test]
    public void FlushFive_WithSixCards_ShouldFindAllCombinations() {
        var cards = CreateCards("AH", "AH", "AH", "AH", "AH", "AH");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var flushFives = hands.Where(h => h is FlushFiveHand).ToList();

        Assert.Multiple(() => {
            // Verificar que todas son manos válidas de FlushFive
            Assert.That(flushFives.All(h => h.Cards.Count == 5), "All hands should have 5 cards");
            Assert.That(flushFives.All(h => h.Cards.All(c => c.Suit == 'H')), "All cards should be hearts");
            Assert.That(flushFives.All(h => h.Cards.All(c => c.Rank == 14)), "All cards should be Aces");

            // Verificar que solo hay una mano
            Assert.That(flushFives.Count, Is.EqualTo(1), "Should have exactly 1 hand");

            // Crear un HashSet para ver las combinaciones únicas
            var uniqueHandStrings = flushFives
                .Select(h => string.Join(",", h.Cards.Select(c => c.ToString()).OrderBy(s => s)))
                .Distinct()
                .ToList();

            Assert.That(uniqueHandStrings.Count, Is.EqualTo(1), "Should have 1 unique combination");
        });
    }

    [Test]
    public void DisabledHand_ShouldNotBeIdentified() {
        var currentHand = CreateCards("AS", "AH", "AD", "AC", "AH");
        var config = HandsManager.GetPokerHandConfig(new FiveOfAKindHand(HandsManager, []));

        // Verificar que la mano se detecta cuando está habilitada
        var handsEnabled = HandsManager.IdentifyAllHands(Handler, currentHand)
            .Where(h => h is FiveOfAKindHand)
            .ToList();
        Assert.That(handsEnabled, Is.Not.Empty, "Should identify FiveOfAKind when enabled");

        // Deshabilitar la mano
        HandsManager.RegisterHand(new FiveOfAKindHand(HandsManager, []),
            config.InitialScore, config.InitialMultiplier, config.ScorePerLevel,
            config.MultiplierPerLevel, false);

        // Verificar que la mano no se detecta cuando está deshabilitada
        var handsDisabled = HandsManager.IdentifyAllHands(Handler, currentHand)
            .Where(h => h is FiveOfAKindHand)
            .ToList();
        Assert.That(handsDisabled, Is.Empty, "Should not identify disabled FiveOfAKind");
    }
}