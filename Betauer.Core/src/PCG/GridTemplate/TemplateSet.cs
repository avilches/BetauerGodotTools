using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public IEnumerable<Template> FindTemplates(byte? directionFlags = null, string[]? tags = null) {
        var query = _templates.AsEnumerable();

        if (directionFlags.HasValue) {
            query = query.Where(t => t.DirectionFlags == directionFlags.Value);
        }

        if (tags != null && tags.Length > 0) {
            query = query.Where(t => t.HasAllTags(tags));
        }

        return query;
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
        foreach (var line in content.Split(lineSeparator).Select(l => l.Trim())) {
            lineNumber++;
            if (line.Length == 0 || line.StartsWith("# ", StringComparison.Ordinal) || line.StartsWith("// ", StringComparison.Ordinal)) {
                continue;
            }

            if (line.StartsWith("@ ", StringComparison.Ordinal)) {
                ProcessPreviousTemplate();
                // Parseamos toda la línea como un conjunto de atributos y tags
                var parseResult = AttributeParser.Parse(line[1..], true);
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
                foreach (var (k, value) in parseResult.Attributes) {
                    if (k.StartsWith("dir:")) {
                        var key = k[4..];
                        // If the user wrote "dir:N" (north) or "dir:t" (top) instead of "U", we normalize it and write it as "U" (up)
                        var dir = DirectionFlagTools.StringToDirectionFlag(key);
                        if (dir != DirectionFlag.None) {
                            current.SetAttribute(dir, value);
                        } else {
                            current.SetAttribute(key, value);
                        }
                    } else {
                        current.SetAttribute(k, value);
                    }
                }
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

    public static readonly (string[], Transformations.Type Transform)[] Transformations = [
        (["rotate:90", "rotate90"], DataMath.Transformations.Type.Rotate90),
        (["rotate:180", "rotate180"], DataMath.Transformations.Type.Rotate180),
        (["rotate:-90", "rotate-90", "rotate:minus90"], DataMath.Transformations.Type.RotateMinus90),
        (["flip:h", "fliph", "flip:horizontal"], DataMath.Transformations.Type.FlipH),
        (["flip:v", "flipv", "flip:vertical"], DataMath.Transformations.Type.FlipV),
        (["flip:diagonal", "flipdiagonal"], DataMath.Transformations.Type.FlipDiagonal),
        (["flip:secondary", "flip:diagonalsecondary", "flipdiagonalsecondary"], DataMath.Transformations.Type.FlipDiagonalSecondary),
        (["mirror:lr", "mirror:we", "mirror:leftright", "mirror:westeast", "mirror:left-right", "mirror:west-east"], DataMath.Transformations.Type.MirrorLR),
        (["mirror:rl", "mirror:ew", "mirror:rightleft", "mirror:eastwest", "mirror:right-left", "mirror:east-west"], DataMath.Transformations.Type.MirrorRL),
        (["mirror:tb", "mirror:ud", "mirror:ns", "mirror:topbottom", "mirror:updown", "mirror:northsouth", "mirror:top-bottom", "mirror:up-down", "mirror:north-south"], DataMath.Transformations.Type.MirrorTB),
        (["mirror:bt", "mirror:du", "mirror:sn", "mirror:bottomtop", "mirror:downup", "mirror:southnorth", "mirror:bottom-top", "mirror:down-up", "mirror:south-north"], DataMath.Transformations.Type.MirrorBT)
    ];

    private static readonly (string Type, Transformations.Type[] Transforms)[] TransformGroups = [
        ("rotate:all", [
            DataMath.Transformations.Type.Rotate90,
            DataMath.Transformations.Type.Rotate180,
            DataMath.Transformations.Type.RotateMinus90
        ]),
        ("flip:all", [
            DataMath.Transformations.Type.FlipH,
            DataMath.Transformations.Type.FlipV,
            DataMath.Transformations.Type.FlipDiagonal,
            DataMath.Transformations.Type.FlipDiagonalSecondary
        ]),
        ("mirror:all", [
            DataMath.Transformations.Type.MirrorLR,
            DataMath.Transformations.Type.MirrorRL,
            DataMath.Transformations.Type.MirrorTB,
            DataMath.Transformations.Type.MirrorBT
        ])
    ];

    /// <summary>
    /// Reads the tags of every template and try to apply the transformations. "Try" means if the transformations results in an identical body, it will not be added.
    /// </summary>
    public void ApplyTransformations() {
        foreach (var template in FindTemplates().ToArray()) {
            if (template.HasTag("transform:all")) {
                ApplyTransformations(template, Enum.GetValues<DataMath.Transformations.Type>());
            } else {
                var transformsToApply = new HashSet<Transformations.Type>();
                // Check group tags (rotate:all, flip:all, mirror:all)
                foreach (var (tag, transforms) in TransformGroups) {
                    if (template.HasTag(tag)) transformsToApply.UnionWith(transforms);
                }
                // Check individual transformation tags
                foreach (var (tags, transform) in Transformations) {
                    if (template.HasAnyTag(tags)) transformsToApply.Add(transform);
                }
                ApplyTransformations(template, transformsToApply);
            }
        }
    }

    private const string SharedId = "shared.id";

    /// <summary>
    /// Returns true if the templates share the same origin. That means, if they are equals, or one of them was generated through a transformation from the other
    /// </summary>
    /// <param name="one"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static bool ShareOrigin(Template one, Template other) {
        if (Equals(one, other)) return true;
        var oneId = one.GetAttributeAs<int>(SharedId);
        var otherId = other.GetAttributeAs<int>(SharedId);
        return oneId == otherId;
    }

    private void ApplyTransformations(Template template, IEnumerable<Transformations.Type> transformsToApply) {
        // Apply all collected transformations
        foreach (var transform in transformsToApply) {
            var transformed = template.TryTransform(transform);
            if (transformed == null) continue;
            transformed.SetAttribute("transformed.type", transform.ToString());
            var id = RuntimeHelpers.GetHashCode(template);
            transformed.SetAttribute(SharedId, id);
            template.SetAttribute(SharedId, id);
            AddTemplate(transformed);
        }
    }
}