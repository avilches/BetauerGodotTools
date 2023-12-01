using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Betauer.SourceGenerators;

public class ClassGenerator {
    public GeneratorExecutionContext Context { get; }
    public ClassDeclarationSyntax Cds { get; }
    public INamedTypeSymbol Symbol { get; }

    public bool IsInnerClass { get; }
    public string? Namespace { get; }
    public string UniqueHint { get; }
    
    public StringBuilder Content { get; } = new();
    public int IndentLevel { get; private set; } = 0;
    public string Indent { get; private set; } = string.Empty;

    public ClassGenerator(GeneratorExecutionContext generatorExecutionContext, ClassDeclarationSyntax cds, INamedTypeSymbol symbol,
        string classGeneratedSuffix) {
        Context = generatorExecutionContext;
        Cds = cds;
        Symbol = symbol;
        var namespaceSymbol = symbol.ContainingNamespace;
        Namespace = namespaceSymbol is { IsGlobalNamespace: false } ? namespaceSymbol.FullQualifiedNameOmitGlobal() : null;
        IsInnerClass = symbol.ContainingType != null;
        UniqueHint = symbol.FullQualifiedNameOmitGlobal().SanitizeQualifiedNameForUniqueHint() + classGeneratedSuffix + ".generated";
    }

    public void BeginScope() {
        IndentLevel++;
        Indent = new string('\t', IndentLevel);
    }

    public void EndScope() {
        IndentLevel--;
        Indent = new string('\t', IndentLevel);
    }

    public void AddNameSpace() {
        if (Namespace == null) return;
        AppendLine($"namespace {Namespace};");
        EmptyLine();
    }

    public void AddParentInnerClasses() {
        if (!IsInnerClass) return;
        var stack = new Stack<string>();
        var containingType = Symbol.ContainingType;
        while (containingType != null) {
            stack.Push($"partial {containingType.GetDeclarationKeyword()} {containingType.NameWithTypeParameters()} {{");
            containingType = containingType.ContainingType;
        }

        while (stack.Count > 0) {
            AppendLine(stack.Pop());
            EmptyLine();
            BeginScope();
        }
    }

    public void EndParentInnerClasses() {
        if (!IsInnerClass) return;
        var containingType = Symbol.ContainingType;
        while (containingType != null) {
            EmptyLine();
            EndScope();
            AppendLine("}"); // outer class
            containingType = containingType.ContainingType;
        }
    }

    public void EmptyLine() {
        Content.AppendLine();
    }

    public void AppendLine(string lines) {
        lines = NormalizeLineEndings(lines);
        var newLine = Environment.NewLine;
        if (lines.Length > newLine.Length &&
            lines.Substring(0, lines.Length - newLine.Length).Contains(newLine)) {
            lines.Split(newLine).ForEach(AppendSingleLine);
        } else {
            AppendSingleLine(lines);
        }
    }

    private void AppendSingleLine(string line) {
        if (IndentLevel > 0) Content.Append(Indent);
        Content.AppendLine(line);
    }

    public string NormalizeLineEndings(string text) {
        var newLine = Environment.NewLine;
        return text
            .Replace("\r\n", newLine)
            .Replace("\r", newLine)
            .Replace("\n", newLine);
    }
    
    
    public void ReportNonPartialMethod(string id, string methodName, string parametersSignature, string attribute) {
        Context.ReportDiagnostic(Diagnostic.Create(
            new DiagnosticDescriptor(
                id: id,
                title: $"Missing partial `{methodName}` method signature.",
                messageFormat:
                $"Class with attribute [{attribute}] must have a partial `{methodName}` method with this signature: `public override partial void{methodName}{parametersSignature};`",
                category: "Usage",
                defaultSeverity: DiagnosticSeverity.Error,
                isEnabledByDefault: true
            ),
            Cds.GetLocation(),
            Cds.SyntaxTree.FilePath));
    }

    public override string ToString() {
        return Symbol.ToDisplayString();
    }
}