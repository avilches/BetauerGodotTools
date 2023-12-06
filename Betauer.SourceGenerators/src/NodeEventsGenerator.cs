using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Betauer.SourceGenerators;

[Generator]
public class NodeEventsGenerator : IIncrementalGenerator {
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        var godotClasses = CreateValuesProviderForGodotClasses(context.SyntaxProvider, "Godot.Node");
        var values = context.CompilationProvider
            .Combine(godotClasses.Collect());

        context.RegisterSourceOutput(values, static (spc, source) => {
            var compilation = source.Left;
            var godotClasses = source.Right;
            Execute(spc, compilation, godotClasses);
        });
    }

    public static IncrementalValuesProvider<GodotClassData> CreateValuesProviderForGodotClasses(SyntaxValueProvider provider,
        string parentClassFullName,
        Func<SyntaxNode, CancellationToken, bool>? customPredicate = null,
        Func<GeneratorSyntaxContext, CancellationToken, GodotClassData>? customTransform = null) {
        
        return provider.CreateSyntaxProvider(
            // By default select class declarations that inherit from something
            // since Godot classes must at least inherit from Godot.Object
            predicate: customPredicate ?? (static (s, _) => s is ClassDeclarationSyntax { BaseList.Types.Count: > 0 }),
            // Filter out non-Godot classes and retrieve the symbol
            transform: customTransform ?? ((ctx, _) => {
                var cds = (ClassDeclarationSyntax)ctx.Node;
                if (!cds.IsGodotScriptClass(ctx.SemanticModel, out var symbol, parentClassFullName))
                    return default;
                return new GodotClassData(cds, symbol);
            })
        ).Where(static x => x.Symbol is not null);
    }

    private static void Execute(SourceProductionContext context, Compilation compilation, ImmutableArray<GodotClassData> godotClassDatas) {
        godotClassDatas.Where(x => {
                // Report and skip non-partial classes
                if (x.DeclarationSyntax.IsPartial()) {
                    if (x.DeclarationSyntax.IsNested() && !x.DeclarationSyntax.AreAllOuterTypesPartial(out var typeMissingPartial)) {
                        // error, no need to report because the GodotSDK already does it
                        return false;
                    }
                    return true;
                }
                return false;
            }).Select(x => new NodeEventsClassGenerator(context, x.DeclarationSyntax, x.Symbol, ClassGeneratedSuffix))
            .Distinct(new ClassGeneratorEqualityComparer())
            .Where(partialClass => partialClass.Verify())
            .ForEach(partialClass => partialClass.GenerateSource());
    }

    private const string ClassGeneratedSuffix = "_Betauer_NodeEvents";
}

public readonly struct GodotClassData {
    public GodotClassData(ClassDeclarationSyntax cds, INamedTypeSymbol symbol) {
        DeclarationSyntax = cds;
        Symbol = symbol;
    }

    public ClassDeclarationSyntax DeclarationSyntax { get; }
    public INamedTypeSymbol Symbol { get; }
}

public class ClassGeneratorEqualityComparer : IEqualityComparer<NodeEventsClassGenerator> {
    private readonly IEqualityComparer<INamedTypeSymbol> _comparer = SymbolEqualityComparer.Default;

    public bool Equals(NodeEventsClassGenerator? obj, NodeEventsClassGenerator? other) {
        return _comparer.Equals(obj?.Symbol, other?.Symbol);
    }

    public int GetHashCode(NodeEventsClassGenerator obj) {
        return _comparer.GetHashCode(obj.Symbol);
    }
}