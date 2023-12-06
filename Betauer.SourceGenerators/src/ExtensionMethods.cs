using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Betauer.SourceGenerators;

internal static class ExtensionMethods {

    public const string GodotObject = "Godot.GodotObject";

    /// <summary>
    /// Immediately executes the given action on each element in the source sequence.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the sequence</typeparam>
    /// <param name="source">The sequence of elements</param>
    /// <param name="action">The action to execute on each element</param>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (action == null) throw new ArgumentNullException(nameof(action));

        foreach (var element in source)
            action(element);
    }

    public static bool InheritsFrom(this INamedTypeSymbol? symbol, string assemblyName, string typeFullName) {
        while (symbol != null) {
            if (symbol.ContainingAssembly?.Name == assemblyName &&
                symbol.FullQualifiedNameOmitGlobal() == typeFullName) {
                return true;
            }

            symbol = symbol.BaseType;
        }
        return false;
    }
    
    /*
    public static bool TryGetGlobalAnalyzerProperty(
        this AnalyzerConfigOptionsProvider provider, string property, out string? value
    ) => provider.GlobalOptions.TryGetValue("build_property." + property, out value);

    public static bool AreGodotSourceGeneratorsDisabled(this AnalyzerConfigOptionsProvider context)
        => context.TryGetGlobalAnalyzerProperty("GodotSourceGenerators", out string? toggle) &&
           toggle != null &&
           toggle.Equals("disabled", StringComparison.OrdinalIgnoreCase);

    public static bool IsGodotToolsProject(this AnalyzerConfigOptionsProvider context)
        => context.TryGetGlobalAnalyzerProperty("IsGodotToolsProject", out string? toggle) &&
           toggle != null &&
           toggle.Equals("true", StringComparison.OrdinalIgnoreCase);
    */
    
    // Incremental generator
    public static bool IsGodotScriptClass(this ClassDeclarationSyntax cds, SemanticModel sm, out INamedTypeSymbol? symbol, string parentClassFullName) {
        var classTypeSymbol = sm.GetDeclaredSymbol(cds);

        if (classTypeSymbol?.BaseType == null
            || !classTypeSymbol.BaseType.InheritsFrom("GodotSharp", parentClassFullName))
        {
            symbol = null;
            return false;
        }
        symbol = classTypeSymbol;
        return true;
        
    }

    
    // Regular generator
    /*
    public static bool IsGodotScriptClass(this ClassDeclarationSyntax cds, Compilation compilation, out INamedTypeSymbol? symbol, string parentClassFullName) {
        var sm = compilation.GetSemanticModel(cds.SyntaxTree);

        var classTypeSymbol = sm.GetDeclaredSymbol(cds);

        if (classTypeSymbol?.BaseType == null || !classTypeSymbol.BaseType.InheritsFrom("GodotSharp", parentClassFullName)) {
            symbol = null;
            return false;
        }

        symbol = classTypeSymbol;
        return true;
    }

    public static IEnumerable<(ClassDeclarationSyntax cds, INamedTypeSymbol symbol)> SelectGodotScriptClasses(
        this IEnumerable<ClassDeclarationSyntax> source, Compilation compilation, bool needPartial, string parentClassFullName = GodotObject) {
        foreach (var cds in source) {
            if (!cds.IsGodotScriptClass(compilation, out var symbol, parentClassFullName)) continue;
            if (needPartial && !cds.IsPartialAndAllOuterTypes()) continue;
            yield return (cds, symbol!);
        }
    }
    */

    public static bool IsNested(this TypeDeclarationSyntax cds) => cds.Parent is TypeDeclarationSyntax;

    public static bool IsPartial(this TypeDeclarationSyntax cds)
        => cds.Modifiers.Any(SyntaxKind.PartialKeyword);

    public static bool AreAllOuterTypesPartial(this TypeDeclarationSyntax cds, out TypeDeclarationSyntax? typeMissingPartial) {
        SyntaxNode? outerSyntaxNode = cds.Parent;

        while (outerSyntaxNode is TypeDeclarationSyntax outerTypeDeclSyntax) {
            if (!outerTypeDeclSyntax.IsPartial()) {
                typeMissingPartial = outerTypeDeclSyntax;
                return false;
            }

            outerSyntaxNode = outerSyntaxNode.Parent;
        }

        typeMissingPartial = null;
        return true;
    }

    public static bool IsPartialAndAllOuterTypes(this TypeDeclarationSyntax cds) => 
        cds.IsPartial() && (!cds.IsNested() || cds.AreAllOuterTypesPartial(out var typeMissingPartial));

    public static string GetDeclarationKeyword(this INamedTypeSymbol namedTypeSymbol) {
        string? keyword = namedTypeSymbol.DeclaringSyntaxReferences
            .OfType<TypeDeclarationSyntax>().FirstOrDefault()?
            .Keyword.Text;

        return keyword ?? namedTypeSymbol.TypeKind switch {
            TypeKind.Interface => "interface",
            TypeKind.Struct => "struct",
            _ => "class"
        };
    }

    public static string NameWithTypeParameters(this INamedTypeSymbol symbol) {
        return symbol.IsGenericType ? string.Concat(symbol.Name, "<", string.Join(", ", symbol.TypeParameters), ">") : symbol.Name;
    }

    private static SymbolDisplayFormat FullyQualifiedFormatOmitGlobal { get; } =
        SymbolDisplayFormat.FullyQualifiedFormat
            .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted);

    private static SymbolDisplayFormat FullyQualifiedFormatIncludeGlobal { get; } =
        SymbolDisplayFormat.FullyQualifiedFormat
            .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Included);

    public static string FullQualifiedNameOmitGlobal(this ITypeSymbol symbol)
        => symbol.ToDisplayString(NullableFlowState.NotNull, FullyQualifiedFormatOmitGlobal);

    public static string FullQualifiedNameOmitGlobal(this INamespaceSymbol namespaceSymbol)
        => namespaceSymbol.ToDisplayString(FullyQualifiedFormatOmitGlobal);

    public static string FullQualifiedNameIncludeGlobal(this ITypeSymbol symbol)
        => symbol.ToDisplayString(NullableFlowState.NotNull, FullyQualifiedFormatIncludeGlobal);

    public static string FullQualifiedNameIncludeGlobal(this INamespaceSymbol namespaceSymbol)
        => namespaceSymbol.ToDisplayString(FullyQualifiedFormatIncludeGlobal);

    public static string FullQualifiedSyntax(this SyntaxNode node, SemanticModel sm) {
        StringBuilder sb = new();
        FullQualifiedSyntax(node, sm, sb, true);
        return sb.ToString();
    }

    private static void FullQualifiedSyntax(SyntaxNode node, SemanticModel sm, StringBuilder sb, bool isFirstNode) {
        if (node is NameSyntax ns && isFirstNode) {
            SymbolInfo nameInfo = sm.GetSymbolInfo(ns);
            sb.Append(nameInfo.Symbol?.ToDisplayString(FullyQualifiedFormatIncludeGlobal) ?? ns.ToString());
            return;
        }

        bool innerIsFirstNode = true;
        foreach (var child in node.ChildNodesAndTokens()) {
            if (child.HasLeadingTrivia) {
                sb.Append(child.GetLeadingTrivia());
            }

            if (child.IsNode) {
                FullQualifiedSyntax(child.AsNode()!, sm, sb, isFirstNode: innerIsFirstNode);
                innerIsFirstNode = false;
            } else {
                sb.Append(child);
            }

            if (child.HasTrailingTrivia) {
                sb.Append(child.GetTrailingTrivia());
            }
        }
    }

    public static string SanitizeQualifiedNameForUniqueHint(this string qualifiedName)
        => qualifiedName
            // AddSource() doesn't support angle brackets
            .Replace("<", "(Of ")
            .Replace(">", ")");

    public static bool HasAttribute(this INamedTypeSymbol symbol, string attributeFullName) =>
        symbol.GetAttribute(attributeFullName) != null;
    
    public static AttributeData? GetAttribute(this INamedTypeSymbol symbol, string attributeFullName) => 
        symbol.GetAttributes().FirstOrDefault(attr => attr.AttributeClass!.FullQualifiedNameOmitGlobal() == attributeFullName);

    public static MethodDeclarationSyntax? FindPartialMethod(this ClassDeclarationSyntax cds, string methodName, string[] paramTypes) {
        return cds.Members.OfType<MethodDeclarationSyntax>().FirstOrDefault(
            method => method.Modifiers.Any(modifier => modifier.ValueText == "public") &&
                      method.Modifiers.Any(modifier => modifier.ValueText == "override") &&
                      method.Modifiers.Any(modifier => modifier.ValueText == "partial") &&
                      method.ReturnType.ToString() == "void" &&
                      method.Identifier.ValueText == methodName &&
                      method.ParameterList.Parameters.Count == paramTypes.Length &&
                      method.ParameterList.Parameters.Zip(paramTypes, (param, type) => param.Type?.ToString() == type).All(x => x)
        );
    }


}