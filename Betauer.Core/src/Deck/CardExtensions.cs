using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.Deck;

// Clase auxiliar para las combinaciones
public static class EnumerableExtensions {
    public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> elements, int k) {
        return k == 0
            ? new[] { new T[0] }
            : elements.SelectMany((e, i) =>
                elements.Skip(i + 1).Combinations(k - 1).Select(c => (new[] { e }).Concat(c)));
    }
}

public static class CardExtensions {
    // Encuentra todas las escaleras posibles en un conjunto de cartas
    public static IEnumerable<List<Card>> FindStraights(this IEnumerable<Card> cards) {
        var uniqueRanks = cards.Select(c => c.Rank)
            .Distinct()
            .OrderBy(r => r)
            .ToList();

        // Añadir el As como 1 si existe un As (14)
        if (uniqueRanks.Contains(14)) {
            uniqueRanks.Insert(0, 1);
        }

        // Buscar secuencias de 5 cartas
        for (int i = 0; i <= uniqueRanks.Count - 5; i++) {
            var sequence = uniqueRanks.Skip(i).Take(5);
            if (sequence.Zip(sequence.Skip(1)).All(p => p.Second - p.First == 1)) {
                // Para cada rango en la escalera, encuentra todas las cartas posibles
                var straightCards = sequence.Select(rank =>
                    rank == 1
                        ? cards.First(c => c.Rank == 14)
                        : // Usar el As (14) cuando el rank es 1
                        cards.First(c => c.Rank == rank)
                ).ToList();

                yield return straightCards;
            }
        }
    }

    // Encuentra el mejor color posible por cada palo
    public static IEnumerable<List<Card>> FindFlushes(this IEnumerable<Card> cards) {
        return cards.GroupBy(c => c.Suit)
            .Where(g => g.Count() >= 5)
            .Select(g => g.OrderByDescending(c => c.Rank)
                .Take(5)
                .ToList());
    }
}

