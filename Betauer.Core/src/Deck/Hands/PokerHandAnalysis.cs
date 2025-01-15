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
    
    // Secuencias
    public IReadOnlyList<List<Card>> PotentialStraights { get; }
    public IReadOnlyList<List<Card>> CompleteStraights { get; }
    
    // Flushes
    public IReadOnlyList<List<Card>> PotentialFlushes { get; }
    public IReadOnlyList<List<Card>> CompleteFlushes { get; }
    
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
        var straightAnalysis = AnalyzeStraights(cards);
        PotentialStraights = straightAnalysis.potential;
        CompleteStraights = straightAnalysis.complete;
        
        // Análisis de flushes
        var flushAnalysis = AnalyzeFlushes(CardsBySuit);
        PotentialFlushes = flushAnalysis.potential;
        CompleteFlushes = flushAnalysis.complete;
        
        // Análisis de combinaciones especiales
        TwoPairs = AnalyzeTwoPairs(Pairs);
        FullHouses = AnalyzeFullHouses(ThreeOfAKind, Pairs);
        FlushHouses = AnalyzeFlushHouses(CardsBySuit);
    }

    private Dictionary<int, List<Card>> AnalyzeByRank(IReadOnlyList<Card> cards) {
        return cards
            .GroupBy(c => c.Rank)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    private Dictionary<char, List<Card>> AnalyzeBySuit(IReadOnlyList<Card> cards) {
        return cards
            .GroupBy(c => c.Suit)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    private Dictionary<int, List<(int rank, List<Card> cards)>> AnalyzeGroups(IReadOnlyDictionary<int, List<Card>> cardsByRank) {
        var groups = new Dictionary<int, List<(int rank, List<Card> cards)>>();
    
        foreach (var (rank, cards) in cardsByRank) {
            var count = cards.Count;
            if (count >= 2) {
                for (int size = 2; size <= count && size <= 5; size++) {
                    if (!groups.ContainsKey(size)) {
                        groups[size] = [];
                    }
                    groups[size].Add((rank, cards.Take(size).ToList()));
                }
            }
        }
    
        // Ordenar cada grupo por rank descendente
        foreach (var size in groups.Keys) {
            groups[size] = groups[size].OrderByDescending(g => g.rank).ToList();
        }
    
        return groups;
    }

    private (List<List<Card>> potential, List<List<Card>> complete) AnalyzeStraights(IReadOnlyList<Card> cards) {
        var (complete, oneGap, twoGaps) = HandUtils.FindStraightSequences(cards);
        // Primero intentamos las secuencias con un hueco
        var potential = oneGap;
        // Si no hay secuencias con un hueco, usamos las de dos huecos
        if (potential.Count == 0) {
            potential = twoGaps;
        }
        return (potential, complete);
    }

    private (List<List<Card>> potential, List<List<Card>> complete) AnalyzeFlushes(IReadOnlyDictionary<char, List<Card>> cardsBySuit) {
        var potential = new List<List<Card>>();
        var complete = new List<List<Card>>();
        
        foreach (var suitGroup in cardsBySuit.Values) {
            if (suitGroup.Count >= 3) {
                potential.Add(suitGroup);
            }
            if (suitGroup.Count >= 5) {
                complete.Add(suitGroup.ToList());
            }
        }
        
        return (potential, complete);
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

    private List<(List<Card> threeCards, List<Card> pair)> AnalyzeFullHouses(
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

    private List<(List<Card> threeCards, List<Card> pair)> AnalyzeFlushHouses(IReadOnlyDictionary<char, List<Card>> cardsBySuit) {
        var result = new List<(List<Card>, List<Card>)>();
    
        foreach (var suitGroup in cardsBySuit.Values) {
            if (suitGroup.Count >= 5) {
                var groupsByRank = suitGroup
                    .GroupBy(c => c.Rank)
                    .Where(g => g.Count() >= 2)
                    .OrderByDescending(g => g.Count())
                    .ThenByDescending(g => g.Key)
                    .ToList();

                if (groupsByRank.Count >= 2) {
                    var firstGroup = groupsByRank[0].ToList();
                    var secondGroup = groupsByRank[1].ToList();
                
                    if (firstGroup.Count >= 3 && secondGroup.Count >= 2) {
                        result.Add((firstGroup.Take(3).ToList(), secondGroup.Take(2).ToList()));
                    }
                
                    // Si el segundo grupo también tiene 3 o más, probar la combinación inversa
                    if (firstGroup.Count >= 2 && secondGroup.Count >= 3) {
                        result.Add((secondGroup.Take(3).ToList(), firstGroup.Take(2).ToList()));
                    }
                }
            }
        }
    
        return result;
    }
}