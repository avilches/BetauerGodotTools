using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Betauer.SourceGenerators.Metadata;

public class MetadataClassGenerator(SourceProductionContext context, ClassDeclarationSyntax cds, INamedTypeSymbol symbol, string codeToGenerate)
    : ClassGenerator(context, cds, symbol, "_Betauer_Metadata") {
    public bool Verify() {
        if (!Cds.IsPartial()) {
            Context.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(
                    id: "BM001",
                    title: "Class with [Metadata] must be partial",
                    messageFormat: "Class '{0}' with [Metadata] attribute must be declared as partial",
                    category: "Usage",
                    defaultSeverity: DiagnosticSeverity.Error,
                    isEnabledByDefault: true
                ),
                Cds.GetLocation(),
                Symbol.Name));
            return false;
        }

        if (Cds.IsNested() && !Cds.AreAllOuterTypesPartial(out var typeMissingPartial)) {
            Context.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(
                    id: "BM002",
                    title: "Outer class must be partial",
                    messageFormat: "Outer class '{0}' must be declared as partial because it contains a nested class with [Metadata] attribute",
                    category: "Usage",
                    defaultSeverity: DiagnosticSeverity.Error,
                    isEnabledByDefault: true
                ),
                typeMissingPartial!.GetLocation(),
                typeMissingPartial.Identifier.Text));
            return false;
        }

        return true;
    }

    public void GenerateSource() {
        AppendLine("#nullable enable");

        AddNameSpace();
        AddParentInnerClasses();

        AppendLine($"partial class {Symbol.NameWithTypeParameters()} {{");
        BeginScope();

        AppendLine(codeToGenerate);

        EndScope();
        AppendLine("}");

        EndParentInnerClasses();
        AppendLine("#nullable disable");

        Context.AddSource(UniqueHint, Content.ToString());
    }
}