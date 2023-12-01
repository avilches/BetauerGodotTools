using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Betauer.SourceGenerators;

[Generator]
public class NodeEventsGenerator : ISourceGenerator {
    private const string ClassGeneratedSuffix = "_Betauer_NodeEvents";

    public void Initialize(GeneratorInitializationContext context) {
    }

    public void Execute(GeneratorExecutionContext context) {
        try {
            context
                .Compilation.SyntaxTrees
                .SelectMany(tree =>
                    tree.GetRoot().DescendantNodes()
                        .OfType<ClassDeclarationSyntax>()
                        .SelectGodotScriptClasses(context.Compilation, true, "Godot.Node")
                        .Select(x => new NodeEventsClassGenerator(context, x.cds, x.symbol, ClassGeneratedSuffix)))
                .Distinct(new ClassGeneratorEqualityComparer())
                .Where(partialClass => partialClass.Verify())
                .ForEach(partialClass => partialClass.GenerateSource());
        } catch (Exception e) {
            context.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(
                    id: "BETAUER0001",
                    title: $"Exception in {nameof(NodeEventsGenerator)}",
                    messageFormat:
                    $"Exception in {nameof(NodeEventsGenerator)}: `{e.Message}`",
                    category: "Usage",
                    defaultSeverity: DiagnosticSeverity.Error,
                    isEnabledByDefault: true
                ),
                null));

            throw;
        }
    }
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