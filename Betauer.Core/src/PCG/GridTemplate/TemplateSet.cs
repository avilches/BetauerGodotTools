using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;

namespace Betauer.Core.PCG.GridTemplate;

public class TemplateSet(int cellSize) {
    private readonly Dictionary<int, List<Template>> _templates = new();

    public int CellSize { get; } = cellSize;

    public void LoadTemplates(string content) {
        var loader = new TemplateLoader(CellSize);
        var newTemplates = loader.LoadFromString(content);
        foreach (var (id, templates) in newTemplates) {
            if (!_templates.TryGetValue(id, out List<Template>? listTemplate)) {
                _templates[id] = listTemplate = [];
            }
            listTemplate.AddRange(templates);
        }
    }

    // Encuentra todos los patrones para el valor dado
    public List<Array2D<char>> FindTemplates(int type) {
        return GetTemplateDefinitions(type).Select(d => d.Data).ToList();
    }

    // Encuentra todos los patrones que coincidan con los flags requeridos y opcionales
    public List<Array2D<char>> FindTemplates(int type, string[] requiredFlags, string[]? optionalFlags = null) {
        return GetTemplateDefinitions(type, requiredFlags, optionalFlags).Select(d => d.Data).ToList();
    }

    // Obtiene un único patrón que coincida exactamente con los flags requeridos
    public Array2D<char> GetTemplate(int type, string[] requiredFlags) {
        var matchingDefinitions = FindTemplatesWithExactFlags(type, requiredFlags);
        if (matchingDefinitions.Count > 1) {
            throw new ArgumentException(
                $"Multiple templates found for type {TemplateId.TypeToDirectionsString(type)} ({type}) " +
                $"with flags: {string.Join(", ", requiredFlags)}");
        }
        return matchingDefinitions[0].Data;
    }

    private List<Template> GetTemplateDefinitions(int type) {
        if (!_templates.TryGetValue(type, out var definitions)) {
            throw new ArgumentException(
                $"No template found for type {TemplateId.TypeToDirectionsString(type)} ({type})");
        }
        return definitions;
    }

    private List<Template> GetTemplateDefinitions(int type, string[] requiredFlags, string[]? optionalFlags = null) {
        if (!_templates.TryGetValue(type, out var definitions)) {
            throw new ArgumentException(
                $"No template found for type {TemplateId.TypeToDirectionsString(type)} ({type})");
        }

        // Primero filtramos por flags requeridos
        var matchingDefinitions = definitions
            .Where(d => HasRequiredFlags(d, requiredFlags))
            .ToList();

        if (matchingDefinitions.Count == 0) {
            throw new ArgumentException(
                $"No template found for type {TemplateId.TypeToDirectionsString(type)} ({type}) " +
                $"with required flags: {string.Join(", ", requiredFlags)}");
        }

        // Si hay flags opcionales, ordenamos por cantidad de coincidencias
        if (optionalFlags != null && optionalFlags.Length > 0) {
            matchingDefinitions = matchingDefinitions
                .OrderByDescending(d => CountMatchingOptionalFlags(d, optionalFlags))
                .ThenBy(d => d.Id.Flags.Count) // Preferimos patrones con menos flags adicionales
                .ToList();
        }

        return matchingDefinitions;
    }

    private List<Template> FindTemplatesWithExactFlags(int type, string[] requiredFlags) {
        if (!_templates.TryGetValue(type, out var definitions)) {
            throw new ArgumentException(
                $"No template found for type {TemplateId.TypeToDirectionsString(type)} ({type})");
        }

        var matchingDefinitions = definitions
            .Where(d => d.HasExactFlags(requiredFlags))
            .ToList();

        if (matchingDefinitions.Count == 0) {
            throw new ArgumentException(
                $"No template found for type {TemplateId.TypeToDirectionsString(type)} ({type}) " +
                $"with exact flags: {string.Join(", ", requiredFlags)}");
        }

        return matchingDefinitions;
    }

    private static bool HasRequiredFlags(Template template, string[] requiredFlags) {
        return requiredFlags.All(flag => template.Id.Flags.Contains(flag));
    }

    private static int CountMatchingOptionalFlags(Template template, string[] optionalFlags) {
        return optionalFlags.Count(flag => template.Id.Flags.Contains(flag));
    }
}