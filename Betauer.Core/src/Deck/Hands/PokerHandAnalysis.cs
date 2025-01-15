using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.Deck.Hands;

public class PokerHandAnalysis {
    public IReadOnlyList<Card> Cards { get; }
    
    // Grupos básicos
    public IReadOnlyDictionary<int, List<Card>> CardsByRank { get; }
    public IReadOnlyDictionary<char, List<Card>> CardsBySuit { get; }
    
    // Grupos por cantidad
    public IReadOnlyList<(int rank, List<Card> cards)> Pairs { get; }
    public IReadOnlyList<(int rank, List<Card> cards)> ThreeOfAKind { get; }
    public IReadOnlyList<(int rank, List<Card> cards)> FourOfAKind { get; }
    public IReadOnlyList<(int rank, List<Card> cards)> FiveOfAKind { get; }
    
    // Escaleras
    public IReadOnlyList<Straight> Straights { get; }
    public bool HasCompleteStraight => Straights.Count > 0 && !Straights[0].Incomplete;
    
    // Flushes
    public IReadOnlyList<List<Card>> Flushes { get; }
    public bool HasCompleteFlush => Flushes.Count > 0 && Flushes[0].Count >= 5;
    
    // Combinaciones especiales
    public IReadOnlyList<(List<Card> firstPair, List<Card> secondPair)> TwoPairs { get; }
    public IReadOnlyList<(List<Card> threeCards, List<Card> pair)> FullHouses { get; }
    public IReadOnlyList<(List<Card> threeCards, List<Card> pair)> FlushHouses { get; }

    public PokerHandAnalysis(IReadOnlyList<Card> cards) {
        Cards = cards;
        
        // Análisis básico por rank y suit
        CardsByRank = AnalyzeByRank(cards);
        CardsBySuit = AnalyzeBySuit(cards);
        
        // Análisis de grupos por cantidad
        var groupsBySize = AnalyzeGroups(CardsByRank);
        Pairs = groupsBySize.GetValueOrDefault(2, []);
        ThreeOfAKind = groupsBySize.GetValueOrDefault(3, []);
        FourOfAKind = groupsBySize.GetValueOrDefault(4, []);
        FiveOfAKind = groupsBySize.GetValueOrDefault(5, []);
        
        // Análisis de secuencias
        Straights = HandUtils.FindStraightSequences(cards); 
        
        // Análisis de flushes
        Flushes = AnalyzeFlushes(CardsBySuit);
        
        // Análisis de combinaciones especiales
        TwoPairs = AnalyzeTwoPairs(Pairs);
        FullHouses = AnalyzeFullHouses(ThreeOfAKind, Pairs);
        FlushHouses = AnalyzeFlushHouses(CardsBySuit);
    }

    private static Dictionary<int, List<Card>> AnalyzeByRank(IReadOnlyList<Card> cards) {
        return cards
            .GroupBy(c => c.Rank)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    private static Dictionary<char, List<Card>> AnalyzeBySuit(IReadOnlyList<Card> cards) {
        return cards
            .GroupBy(c => c.Suit)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    private static Dictionary<int, List<(int rank, List<Card> cards)>> AnalyzeGroups(IReadOnlyDictionary<int, List<Card>> cardsByRank) {
        var groups = new Dictionary<int, List<(int rank, List<Card> cards)>>();
    
        foreach (var (rank, cards) in cardsByRank) {
            var count = cards.Count;
            if (count >= 2) {
                for (int size = 2; size <= count && size <= 5; size++) {
                    if (!groups.ContainsKey(size)) {
                        groups[size] = [];
                    }
                    // Se ordena por suit para que si hay dos cartas con el mismo suit, se queden juntas
                    // y además las cartas con suits más frecuentes aparezcan primero
                    var group = cards
                        .GroupBy(c => c.Suit)
                        .OrderByDescending(g => g.Count())
                        .SelectMany(g => g)
                        .Take(size)
                        .ToList();                    
                    groups[size].Add((rank, group));
                }
            }
        }
    
        // Ordenar cada grupo por rank descendente
        foreach (var size in groups.Keys) {
            groups[size] = groups[size].OrderByDescending(g => g.rank).ToList();
        }
    
        return groups;
    }

    private static List<List<Card>> AnalyzeFlushes(IReadOnlyDictionary<char, List<Card>> cardsBySuit) {
        return cardsBySuit.Values
            .Where(suitGroup => suitGroup.Count >= 3)
            .OrderByDescending(suitGroup => suitGroup, new FlushComparer())
            .ToList();
    }
    
    private List<(List<Card> firstPair, List<Card> secondPair)> AnalyzeTwoPairs(IReadOnlyList<(int rank, List<Card> cards)> pairs) {
        var result = new List<(List<Card>, List<Card>)>();
        
        for (int i = 0; i < pairs.Count - 1; i++) {
            for (int j = i + 1; j < pairs.Count; j++) {
                result.Add((pairs[i].cards, pairs[j].cards));
            }
        }
        
        return result;
    }

    private static List<(List<Card> threeCards, List<Card> pair)> AnalyzeFullHouses(
        IReadOnlyList<(int rank, List<Card> cards)> threes,
        IReadOnlyList<(int rank, List<Card> cards)> pairs) {
        return AnalyzeHouseVariants(threes, pairs);
    }

    private static List<(List<Card> threeCards, List<Card> pair)> AnalyzeFlushHouses(
        IReadOnlyDictionary<char, List<Card>> cardsBySuit) {
        var result = new List<(List<Card>, List<Card>)>();

        foreach (var suitGroup in cardsBySuit.Values) {
            if (suitGroup.Count >= 5) {
                // Primero agrupamos por rank y verificamos si hay grupos suficientes
                var rankGroups = suitGroup
                    .GroupBy(c => c.Rank)
                    .Where(g => g.Count() >= 2)  // Solo nos interesan grupos de 2 o más
                    .OrderByDescending(g => g.Count())
                    .ToList();

                // Si no hay al menos 2 grupos, o el grupo más grande tiene menos de 3 cartas,
                // no es posible formar un flush house
                if (rankGroups.Count >= 2 && rankGroups[0].Count() >= 3) {
                    var cardsByRank = rankGroups.ToDictionary(g => g.Key, g => g.ToList());
                    var groups = AnalyzeGroups(cardsByRank);
                    var threes = groups.GetValueOrDefault(3, []);
                    var pairs = groups.GetValueOrDefault(2, []);
                
                    result.AddRange(AnalyzeHouseVariants(threes, pairs));
                }
            }
        }
    
        return result;
    }

    private static List<(List<Card> threeCards, List<Card> pair)> AnalyzeHouseVariants(
        IReadOnlyList<(int rank, List<Card> cards)> threes,
        IReadOnlyList<(int rank, List<Card> cards)> pairs) {
        var result = new List<(List<Card>, List<Card>)>();
    
        foreach (var three in threes) {
            foreach (var pair in pairs) {
                if (three.rank != pair.rank) {
                    result.Add((three.cards, pair.cards));
                }
            }
        }
    
        return result;
    }
}