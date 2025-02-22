using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Betauer.SourceGenerators.Metadata;

// DISABLED [Generator]
public class MetadataGenerator : IIncrementalGenerator {
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        var metadataClasses = context.SyntaxProvider.CreateSyntaxProvider(
            // Seleccionar clases con atributos
            predicate: static (s, _) => s is ClassDeclarationSyntax { AttributeLists.Count: > 0 },
            // Filtrar solo las que tienen el atributo [Metadata]
            transform: (ctx, ct) => {
                var cds = (ClassDeclarationSyntax)ctx.Node;
                if (ctx.SemanticModel.GetDeclaredSymbol(cds, cancellationToken: ct) is not INamedTypeSymbol symbol) {
                    return default;
                }
                foreach (var attributeName in Generate.Keys) {
                    if (symbol.HasAttribute(attributeName)) {
                        return new ClassData(cds, symbol, attributeName);
                    }
                }
                return default;
            }
        ).Where(static x => x.Symbol is not null);

        var values = context.CompilationProvider.Combine(metadataClasses.Collect());

        context.RegisterSourceOutput(values, static (spc, source) => {
            var compilation = source.Left;
            var classes = source.Right;
            Execute(spc, compilation, classes);
        });
    }

    private static void Execute(SourceProductionContext context, Compilation compilation,
        ImmutableArray<ClassData> classDatas) {
        foreach (var classData in classDatas) {
            var generator = new MetadataClassGenerator(
                context,
                classData.DeclarationSyntax,
                classData.Symbol,
                Generate[classData.AttributeName]); // Pass the code to generate
            if (generator.Verify()) {
                generator.GenerateSource();
            }
        }
    }

    public static Dictionary<string, string> Generate = new() {
        {
            "Betauer.Core.MetadataAttribute",
            """
            public object Metadata { get; set; }
            public void SetMetadata<T>(T value) => Metadata = value;
            public T GetMetadataAs<T>() => (T)Metadata;
            public T GetMetadataOr<T>(T defaultValue) => Metadata is T value ? value : defaultValue;
            public T GetMetadataOrCreate<T>(System.Func<T> factory) {
                if (Metadata is T value) return value;
                Metadata = factory();
                return (T)Metadata;
            }
            public bool HasMetadata<T>() => Metadata is T;
            public bool HasAnyMetadata => Metadata != null;
            public void ClearMetadata() => Metadata = null;
            """
        }, {
            "Betauer.Core.AttributesAttribute",
            """
            public static readonly Betauer.Core.AttributesHolder AttributesHolder = new Betauer.Core.AttributesHolder();

            // Attributes
            public void SetAttribute(string key, object value) => AttributesHolder.SetAttribute(this, key, value);
            public object? GetAttribute(string key) => AttributesHolder.GetAttribute(this, key);
            public T? GetAttributeAs<T>(string key) => AttributesHolder.GetAttributeAs<T>(this, key);
            public T GetAttributeOr<T>(string key, T defaultValue) => AttributesHolder.GetAttributeOr(this, key, defaultValue);
            public T GetAttributeOrCreate<T>(string key, System.Func<T> factory) => AttributesHolder.GetAttributeOrCreate(this, key, factory);
            public bool RemoveAttribute(string key) => AttributesHolder.RemoveAttribute(this, key);
            public bool HasAttribute(string key) => AttributesHolder.HasAttribute(this, key);
            public IEnumerable<KeyValuePair<string, object>> GetAttributes() => AttributesHolder.GetAttributes(this);
            public int AttributeCount => AttributesHolder.GetAttributeCount(this);
            public bool HasAnyAttribute => AttributesHolder.HasAnyAttribute(this);
            public void ClearAttributes() => AttributesHolder.ClearAttributes(this);

            """
        }
    };
}

public readonly record struct ClassData(ClassDeclarationSyntax DeclarationSyntax, INamedTypeSymbol Symbol, string AttributeName);