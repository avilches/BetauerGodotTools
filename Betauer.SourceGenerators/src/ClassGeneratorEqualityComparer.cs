using System.Collections.Generic;
using Betauer.SourceGenerators.NodeEvents;
using Microsoft.CodeAnalysis;

namespace Betauer.SourceGenerators;

public class ClassGeneratorEqualityComparer : IEqualityComparer<NodeEventsClassGenerator> {
    private readonly IEqualityComparer<INamedTypeSymbol> _comparer = SymbolEqualityComparer.Default;

    public bool Equals(NodeEventsClassGenerator? obj, NodeEventsClassGenerator? other) {
        return _comparer.Equals(obj?.Symbol, other?.Symbol);
    }

    public int GetHashCode(NodeEventsClassGenerator obj) {
        return _comparer.GetHashCode(obj.Symbol);
    }
}