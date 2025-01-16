using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.Deck.Hands;

public class PokerHandAnalysis {
    public PokerGameConfig Config { get; }
    public IReadOnlyList<Card> Cards { get; }
    
    // Grupos básicos
    public Dictionary<int, List<Card>> CardsByRank { get; }
    public Dictionary<char, List<Card>> CardsBySuit { get; }
    
    // Grupos por cantidad, ya ordenados de mayor a menor rank
    public List<(int Rank, List<Card> Cards)> Pairs { get; }
    public List<(int Rank, List<Card> Cards)> ThreeOfAKind { get; }
    public List<(int Rank, List<Card> Cards)> FourOfAKind { get; }
    public List<(int Rank, List<Card> Cards)> FiveOfAKind { get; }
    
    // Escaleras
    // Ya vienen ordenadas StraightComparer: de menos a mas huecos (completas primera, de color siempre antes que no de color)
    // y luego por la carta mas alta 
    public List<Straight> Straights { get; }
    public static StraightComparer StraightComparer { get; } = new StraightComparer();
    public bool HasCompleteStraight => Straights.Count > 0 && !Straights[0].Incomplete;
    
    // Flushes
    // Ya vienen ordenados FlushComparer, por la carta mas alta y en caso de empate, por la siguiente mas alta
    public List<List<Card>> Flushes { get; }
    public static FlushComparer FlushComparer { get; } = new FlushComparer();
    public bool HasCompleteFlush => Flushes.Count > 0 && Flushes[0].Count >= Config.FlushSize;
    
    public List<(List<Card> FirstPair, List<Card> SecondPair)> TwoPairs { get; }
    public List<(List<Card> ThreeCards, List<Card> Pair)> FullHouses { get; }
    public List<(List<Card> ThreeCards, List<Card> Pair)> FlushHouses { get; }

    public PokerHandAnalysis(PokerGameConfig config, IReadOnlyList<Card> cards) {
        Config = config;
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
        Straights = HandUtils.FindStraightSequences(config.StraightSize, cards); 
        
        // Análisis de flushes
        Flushes = AnalyzeFlushes(config.AnalysisMinFlushSize, CardsBySuit);
        
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

    private static Dictionary<int, List<(int Rank, List<Card> Cards)>> AnalyzeGroups(IReadOnlyDictionary<int, List<Card>> cardsByRank) {
        var groups = new Dictionary<int, List<(int Rank, List<Card> Cards)>>();
    
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
            groups[size] = groups[size].OrderByDescending(g => g.Rank).ToList();
        }
    
        return groups;
    }

    private static List<List<Card>> AnalyzeFlushes(int minSize, IReadOnlyDictionary<char, List<Card>> cardsBySuit) {
        return cardsBySuit.Values
            .Where(suitGroup => suitGroup.Count >= minSize)
            .OrderByDescending(suitGroup => suitGroup, FlushComparer)
            .ToList();
    }
    
    private static List<(List<Card> FirstPair, List<Card> SecondPair)> AnalyzeTwoPairs(List<(int Rank, List<Card> Cards)> pairs) {
        var result = new List<(List<Card>, List<Card>)>();
        
        for (var i = 0; i < pairs.Count - 1; i++) {
            for (var j = i + 1; j < pairs.Count; j++) {
                result.Add((pairs[i].Cards, pairs[j].Cards));
            }
        }
        
        return result;
    }

    private static List<(List<Card> ThreeCards, List<Card> Pair)> AnalyzeFullHouses(
        List<(int Rank, List<Card> Cards)> threes,
        List<(int Rank, List<Card> Cards)> pairs) {
        return AnalyzeHouseVariants(threes, pairs);
    }

    private static List<(List<Card> ThreeCards, List<Card> Pair)> AnalyzeFlushHouses(
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

    private static List<(List<Card> ThreeCards, List<Card> Pair)> AnalyzeHouseVariants(
        List<(int Rank, List<Card> Cards)> threes,
        List<(int Rank, List<Card> Cards)> pairs) {
        var result = new List<(List<Card>, List<Card>)>();
    
        foreach (var three in threes) {
            foreach (var pair in pairs) {
                if (three.Rank != pair.Rank) {
                    result.Add((three.Cards, pair.Cards));
                }
            }
        }
    
        return result;
    }
}