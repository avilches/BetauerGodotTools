using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;
using Betauer.Core.PCG.Maze;

namespace Betauer.Core.PCG.GridTemplate;

public class TemplateSet(int cellSize) {
    private readonly List<Template> _templates = [];
    public string TemplateId { get; set; } = "Template";

    public int CellSize { get; } = cellSize;

    public void AddTemplate(Template template) {
        _templates.Add(template);
    }

    public void RemoveTemplate(Template template) {
        _templates.Remove(template);
    }


    /// <summary>
    /// Finds templates that match the specified criteria. When searching by tags,
    /// templates must have ALL the specified tags to be included in the results.
    /// </summary>
    /// <param name="directionFlags">Optional. If specified, only templates of these directionFlags will be returned.</param>
    /// <param name="tags">Optional. If specified, only templates that have ALL of these tags will be returned.</param>
    /// <returns>A list of Template objects that match the specified criteria.</returns>
    /// <example>
    /// // Find all templates
    /// var allTemplates = FindTemplates();
    ///
    /// // Find templates of directionFlags 1
    /// var directionFlagsTemplates = FindTemplates(directionFlags: 1);
    ///
    /// // Find templates with ALL these tags
    /// var templates = FindTemplates(tags: new[] {"wall", "stone"}); // Must have BOTH tags
    ///
    /// // Find templates with ANY of these tags
    /// var anyFlagTemplates = FindTemplatesWithAnyFlag(new[] {"decorated", "damaged"}); // Must have at least one
    ///
    /// // Find templates with EXACT tags
    /// var exactTemplates = FindTemplatesWithExactFlags(new[] {"wall", "stone"}); // Must have exactly these tags
    ///
    /// // Find templates excluding specific tags
    /// var excludingTemplates = FindTemplatesExcluding(new[] {"damaged"}); // Must not have these tags
    /// </example>
    public List<Template> FindTemplates(byte? directionFlags = null, string[]? tags = null) {
        var query = _templates.AsEnumerable();

        if (directionFlags.HasValue) {
            query = query.Where(t => t.DirectionFlags == directionFlags.Value);
        }

        if (tags != null && tags.Length > 0) {
            query = query.Where(t => t.HasAllTags(tags));
        }

        return query.ToList();
    }

    public List<Template> FindTemplates(MazeNode node, string[]? tags = null) {
        return FindTemplates(DirectionFlagTools.GetDirectionFlags(node), tags);
    }

    /// <summary>
    /// Returns the first template with the directionFlags and the tags
    /// </summary>
    /// <param name="directionFlags"></param>
    /// <param name="tags"></param>
    /// <returns></returns>
    public Template? GetTemplate(byte directionFlags, string[] tags) => _templates.FirstOrDefault(t => t.DirectionFlags == directionFlags && t.HasExactTags(tags));

    public void LoadFromString(string content, char lineSeparator = '\n') {
        var buffer = new List<string>();
        string headerString = null;
        Template? current = null;

        var lineNumber = 0;
        foreach (var line in content.Split(lineSeparator)
                     .Select(l => l.Trim())) {

            lineNumber++;
            if (line.Length == 0 || line.StartsWith("# ", StringComparison.Ordinal) || line.StartsWith("// ", StringComparison.Ordinal)) {
                continue;
            }

            if (line.StartsWith("@ ", StringComparison.Ordinal)) {
                ProcessPreviousTemplate();
                // Parseamos toda la línea como un conjunto de atributos y tags
                var parseResult = AttributeParser.Parse(line[1..]);
                headerString = line;

                if (parseResult.Attributes.TryGetValue(TemplateId, out var templateValue)) {
                    var directionFlags = templateValue switch {
                        string directionFlagString => DirectionFlagTools.StringToFlags(directionFlagString),
                        int directionFlagByte => (byte)directionFlagByte,
                        _ => throw new ArgumentException($"Error in line #{lineNumber}. Invalid direction flags in template {line}")
                    };
                    current = new Template { DirectionFlags = directionFlags };
                    parseResult.Attributes.Remove(TemplateId);
                }
                if (current == null) {
                    throw new ArgumentException($"Error in line #{lineNumber}. First define {TemplateId}: {line}");
                }
                foreach (var tag in parseResult.Tags) current.AddTag(tag);
                foreach (var (key, value) in parseResult.Attributes) current.SetAttribute(key, value);

            } else if (current != null) {
                buffer.Add(line);

                if (buffer.Count == CellSize) {
                    ProcessPreviousTemplate();
                } else if (buffer.Count > CellSize) {
                    throw new ArgumentException($"Error in line #{lineNumber}. Too many lines in template {current}. Expected {CellSize} but got more.");
                }
            }
        }
        ProcessPreviousTemplate();
        return;

        void ProcessPreviousTemplate() {
            if (current == null || buffer.Count == 0) return;
            var template = ParseTemplateBody(buffer, headerString);
            current.Body = template;
            AddTemplate(current);
            buffer.Clear();
            current = null;
        }
    }

    private Array2D<char> ParseTemplateBody(List<string> lines, string templateHeader) {
        if (lines.Count == 0) {
            throw new ArgumentException($"Empty template for {templateHeader}");
        }

        var height = lines.Count;
        var width = lines[0].Length;

        // Validar que todas las líneas tengan la misma anchura
        for (var y = 1; y < height; y++) {
            if (lines[y].Length != width) {
                throw new ArgumentException(
                    $"Inconsistent width in template {templateHeader}: " +
                    $"line 0 has width {width} but line {y} has width {lines[y].Length}");
            }
        }

        // Validar que el patrón no esté vacío
        if (width == 0 || height == 0) {
            throw new ArgumentException($"Template {templateHeader} has invalid dimensions: {width}x{height}");
        }

        // Validar que las dimensiones coincidan con el tamaño esperado
        if (width != CellSize || height != CellSize) {
            throw new ArgumentException(
                $"Template {templateHeader} has wrong dimensions: {width}x{height}. " +
                $"Expected {CellSize}x{CellSize}:\n{string.Join("\n", lines)}");
        }

        var template = new Array2D<char>(width, height);
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                template[y, x] = lines[y][x];
            }
        }
        return template;
    }

    public void ApplyTransformations() {
        foreach (var template in FindTemplates()) {
            if (template.HasAnyTag("#rotate:90", "rotate90")) {
                AddTemplate(template.Transform(Transformations.Type.Rotate90));
            }
            if (template.HasAnyTag("#rotate:180", "#rotate180")) {
                AddTemplate(template.Transform(Transformations.Type.Rotate180));
            }
            if (template.HasAnyTag("#rotate:-90", "#rotate-90", "#rotate:minus90")) {
                AddTemplate(template.Transform(Transformations.Type.Rotate180));
            }
            if (template.HasAnyTag("#flip:h", "#fliph", "#flip:horizontal")) {
                AddTemplate(template.Transform(Transformations.Type.FlipH));
            }
            if (template.HasAnyTag("#flip:v", "#flipv", "#flip:vertical")) {
                AddTemplate(template.Transform(Transformations.Type.FlipV));
            }
            if (template.HasAnyTag("#mirror:lr", "#mirror:we", "#mirror:leftright", "#mirror:westeast", "#mirror:left-right", "#mirror:west-east")) {
                AddTemplate(template.Transform(Transformations.Type.MirrorLR));
            }
            if (template.HasAnyTag("#mirror:rl", "#mirror:ew", "#mirror:rightleft", "#mirror:eastwest", "#mirror:right-left", "#mirror:east-west")) {
                AddTemplate(template.Transform(Transformations.Type.MirrorRL));
            }
            if (template.HasAnyTag("#mirror:tb", "#mirror:ud", "#mirror:ns", "#mirror:topbottom", "#mirror:updown", "#mirror:northsouth", "#mirror:top-bottom", "#mirror:up-down", "#mirror:north-south")) {
                AddTemplate(template.Transform(Transformations.Type.MirrorTB));
            }
            if (template.HasAnyTag("#mirror:bt", "#mirror:du", "#mirror:sn", "#mirror:bottomtop", "#mirror:downup", "#mirror:southnorth", "#mirror:bottom-top", "#mirror:down-up", "#mirror:south-north")) {
                AddTemplate(template.Transform(Transformations.Type.MirrorBT));
            }
        }

    }
}