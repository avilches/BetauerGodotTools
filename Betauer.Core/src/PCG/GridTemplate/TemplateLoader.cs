using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;

namespace Betauer.Core.PCG.GridTemplate;

public class TemplateLoader {
    private readonly int _cellSize;
    private const string IdPrefix = "@ID=";

    private static readonly Dictionary<string, Func<Array2D<char>, Array2D<char>>> Transformations = new() {
        ["90"] = template => new Array2D<char>(template.Data.Rotate90()),
        ["180"] = template => new Array2D<char>(template.Data.Rotate180()),
        ["-90"] = template => new Array2D<char>(template.Data.RotateMinus90()),
        ["FlipH"] = template => new Array2D<char>(template.Data.FlipH()),
        ["FlipV"] = template => new Array2D<char>(template.Data.FlipV()),
        ["MirrorLR"] = template => new Array2D<char>(template.Data.MirrorLeftToRight()),
        ["MirrorRL"] = template => new Array2D<char>(template.Data.MirrorRightToLeft()),
        ["MirrorTB"] = template => new Array2D<char>(template.Data.MirrorTopToBottom()),
        ["MirrorBT"] = template => new Array2D<char>(template.Data.MirrorBottomToTop()),
    };

    public TemplateLoader(int cellSize) {
        if (cellSize < 3) {
            throw new ArgumentException($"Cell size must be at least 3, but was {cellSize}");
        }
        _cellSize = cellSize;
    }

    public Dictionary<int, List<Template>> LoadFromString(string content) {
        var templates = new Dictionary<int, List<Template>>();
        var lines = content.Split('\n');
        var currentTemplate = new List<string>();
        Template? current = null;

        void ProcessCurrentTemplate() {
            if (current == null) return;

            if (templates.TryGetValue(current.Id.Type, out var existingTemplates)) {
                var duplicateTemplate = existingTemplates.FirstOrDefault(p =>
                    p.HasExactFlags(current.Id.Flags.ToArray()));

                if (duplicateTemplate != null) {
                    throw new ArgumentException($"Duplicate template found: {current}");
                }
            } else {
                templates[current.Id.Type] = new List<Template>();
            }

            if (current.TemplateBase != null) {
                var parentTemplate = current.TemplateBase.Data;
                if (current.TransformBase != null) {
                    parentTemplate = TransformTemplate(parentTemplate, current.TransformBase);
                }
                current.Data = parentTemplate;
            } else if (currentTemplate.Count > 0) {
                var template = ParseTemplate(currentTemplate, current.Id);
                current.Data = template;
            }

            templates[current.Id.Type].Add(current);
            currentTemplate.Clear();
            current = null;
        }

        foreach (var line in lines) {
            var trimmed = line.Trim();

            if (string.IsNullOrEmpty(trimmed)) {
                continue;
            }

            if (trimmed.StartsWith(IdPrefix)) {
                if (current != null) {
                    ProcessCurrentTemplate();
                }

                var withoutComments = trimmed.Split('#')[0].Trim();
                var parts = withoutComments[IdPrefix.Length..].Split(["from"], StringSplitOptions.TrimEntries);

                var id = TemplateId.Parse(parts[0]);
                current = new Template(id);

                if (parts.Length > 1) {
                    var originalFromPart = parts[1].Trim();
                    var (baseIdString, fromId, transform) = TemplateId.ParseFromString(parts[1]);

                    // Validación temprana y asignación directa del template base
                    if (templates.TryGetValue(fromId.Type, out var existingTemplates)) {
                        var baseTemplate = existingTemplates.FirstOrDefault(p =>
                            p.HasExactFlags(fromId.Flags.ToArray()));
                        if (baseTemplate == null) {
                            throw new ArgumentException(
                                $"Error in template '{current.Id}': Reference to base template '{baseIdString}' not found");
                        }
                        current.TemplateBase = baseTemplate;
                        current.TransformBase = transform;
                    } else {
                        throw new ArgumentException(
                            $"Error in template '{current.Id}': Reference to base template '{baseIdString}' not found " +
                            $"(no templates exist for type {TemplateId.TypeToDirectionsString(fromId.Type)})");
                    }
                }
            } else if (current != null) {
                // Si estamos dentro de un patrón, añadimos la línea
                currentTemplate.Add(trimmed);

                // Si ya tenemos todas las líneas necesarias, procesamos el patrón
                if (currentTemplate.Count == _cellSize) {
                    ProcessCurrentTemplate();
                } else if (currentTemplate.Count > _cellSize) {
                    throw new ArgumentException(
                        $"Too many lines in template {current.Id}. Expected {_cellSize} but got more.");
                }
            }
            // Si no estamos en un patrón y la línea no es @ID, la ignoramos
        }

        // Procesar el último patrón si existe
        if (current != null) {
            ProcessCurrentTemplate();
        }

        return templates;
    }

    private Array2D<char> ParseTemplate(List<string> lines, TemplateId templateId) {
        if (lines.Count == 0) {
            throw new ArgumentException($"Empty template for {templateId}");
        }

        var height = lines.Count;
        var width = lines[0].Length;

        // Validar que todas las líneas tengan la misma anchura
        for (var y = 1; y < height; y++) {
            if (lines[y].Length != width) {
                throw new ArgumentException(
                    $"Inconsistent width in template {templateId}: " +
                    $"line 0 has width {width} but line {y} has width {lines[y].Length}");
            }
        }

        // Validar que el patrón no esté vacío
        if (width == 0 || height == 0) {
            throw new ArgumentException($"Template {templateId} has invalid dimensions: {width}x{height}");
        }

        // Validar que las dimensiones coincidan con el tamaño esperado
        if (width != _cellSize || height != _cellSize) {
            throw new ArgumentException(
                $"Template {templateId} has wrong dimensions: {width}x{height}. " +
                $"Expected {_cellSize}x{_cellSize}:\n{string.Join("\n", lines)}");
        }

        var template = new Array2D<char>(width, height);
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                template[y, x] = lines[y][x];
            }
        }
        return template;
    }


    private static Array2D<char> TransformTemplate(Array2D<char> template, string transform) {
        if (!Transformations.TryGetValue(transform, out var transformation)) {
            throw new ArgumentException($"Invalid transform: {transform}");
        }
        return transformation(template);
    }

    // Este método se puede mover a TemplateId ya que es usado allí para validar
    public static bool IsValidTransform(string transform) {
        return Transformations.ContainsKey(transform);
    }

    private static Array2D<char> GetTemplateFromDictionary(
        Dictionary<int, List<Template>> templates,
        TemplateId baseTemplateId,
        Template currentTemplate) {
        // Ya no necesitamos validar porque se validó en LoadFromString
        var definitions = templates[baseTemplateId.Type];
        var template = definitions.First(d => d.HasExactFlags(baseTemplateId.Flags.ToArray()));
        return template.Data;
    }
}