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
        context
            .Compilation.SyntaxTrees
            .SelectMany(tree =>
                tree.GetRoot().DescendantNodes()
                    .OfType<ClassDeclarationSyntax>()
                    .SelectGodotScriptClasses(context.Compilation, true, "Godot.Node")
                    .Select(x => new NodeEventsClassGenerator(context, x.cds, x.symbol, ClassGeneratedSuffix)))
            .Where(partialClass => partialClass.MustGenerate)
            .ForEach(partialClass => { partialClass.GenerateSource(); });
    }
}