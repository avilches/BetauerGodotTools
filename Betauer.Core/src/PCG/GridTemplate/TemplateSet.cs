using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Betauer.Core.DataMath;
using Betauer.Core.PCG.Maze;
using Godot;

namespace Betauer.Core.PCG.GridTemplate;

public class TemplateSet(int cellSize) {
    private readonly List<Template> _templates = [];
    public string TemplateId { get; set; } = "Template";
    public string DefaultToken { get; set; } = "default";
    public string DefineId { get; set; } = "Define";

    public int CellSize { get; } = cellSize;

    public void AddTemplate(Template template) {
        if (GetTemplateById(template.Id) != null) {
            throw new ArgumentException($"Template with id {template.Id} already exists");
        }
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

    public Template? GetTemplateById(string id) => _templates.FirstOrDefault(t => t.Id == id);

    public Template? GetTemplate(byte directionFlags) => _templates.FirstOrDefault(t => t.DirectionFlags == directionFlags);

    public void LoadFromString(string content, char lineSeparator = '\n') {
        LoadFromString(content, _ => true, lineSeparator);
    }

    public void LoadFromString(string content, Func<Template, bool> filter, char lineSeparator = '\n') {
        var buffer = new List<string>();
        Template? template = null;
        AttributeParser.ParseResult? currentParseResult = null;
        var defines = new Dictionary<string, AttributeParser.ParseResult>(StringComparer.OrdinalIgnoreCase) {
            [DefaultToken] = new(StringComparer.OrdinalIgnoreCase)
        };

        var lineNumber = 0;
        foreach (var line in content.Split(lineSeparator).Select(l => l.Trim())) {
            lineNumber++;
            if (line.Length == 0 || line.StartsWith("# ", StringComparison.Ordinal) || line.StartsWith("// ", StringComparison.Ordinal)) {
                continue;
            }

            if (line.StartsWith("@ ", StringComparison.Ordinal) || line.StartsWith("@\t", StringComparison.Ordinal)) {
                ProcessPreviousTemplate();
                
                // Parseamos toda la línea como un conjunto de atributos y tags
                var lineParserResult = AttributeParser.Parse(line[1..], true);

                // Define template
                if (IsTemplateDefinition(lineParserResult, defines)) {
                    continue;
                }

                if (template == null || currentParseResult == null) {
                    template = new Template();
                    currentParseResult = new AttributeParser.ParseResult(StringComparer.OrdinalIgnoreCase);
                }

                // Use a template
                UseTemplateDefinition(lineParserResult, currentParseResult, lineNumber, line, defines);

                // Add the current parse result to the current template
                foreach (var tag in lineParserResult.Tags) {
                    currentParseResult.Tags.Add(tag);
                }
                foreach (var (k, value) in lineParserResult.Attributes) {
                    currentParseResult.Attributes[k] = value;
                }
            } else if (template != null) {
                buffer.Add(line);

                if (buffer.Count == CellSize) {
                    ProcessPreviousTemplate();
                }
            } else {
                throw new ArgumentException($"Error in line #{lineNumber}. Too many lines in template {template}. Expected {CellSize} but got more.");
            }
        }
        ProcessPreviousTemplate();
        return;

        void ProcessPreviousTemplate() {
            if (template == null || buffer.Count == 0) {
                return;
            }
            var body = ParseTemplateBody(buffer, currentParseResult);
            template.Body = body;

            foreach (var tag in currentParseResult.Tags) {
                template.AddTag(tag);
            }
            foreach (var (k, value) in currentParseResult.Attributes) {
                if (k.StartsWith("dir:", StringComparison.OrdinalIgnoreCase)) {
                    var parts = k[4..].Split(':');
                    if (parts.Length == 0) continue;

                    // If the user wrote "dir:N" (north) or "dir:t" (top) instead of "U", we normalize it and write it as "U" (up)
                    var dir = DirectionFlagTools.StringToDirectionFlag(parts[0]);
                    if (dir != DirectionFlag.None) {
                        // If there's a key after the direction, use it
                        var key = parts.Length > 1 ? string.Join(":", parts.Skip(1)) : "";
                        template.SetAttribute(dir, key, value);
                    } else {
                        // If the direction is invalid, keep the original key
                        template.SetAttribute(k[4..], value);
                    }
                } else if (k.Equals("dir", StringComparison.OrdinalIgnoreCase)) {
                    var directionFlags = value switch {
                        string directionFlagString => DirectionFlagTools.StringToFlags(directionFlagString),
                        int directionFlagByte => (byte)directionFlagByte,
                        _ => throw new ArgumentException($"Invalid direction flags dir={value} in template: {currentParseResult}")
                    };
                    template.DirectionFlags = directionFlags;
                } else if (k.Equals("id", StringComparison.OrdinalIgnoreCase)) {
                    template.Id = value.ToString()!;
                } else {
                    template.SetAttribute(k, value);
                }
            }
            if (template.DirectionFlags == 0) {
                throw new ArgumentException($"No direction flags (attribute like dir=\"U\") in template: {currentParseResult}");
            }

            if (filter.Invoke(template)) {
                AddTemplate(template);
            }
            buffer.Clear();
            template = null;
            currentParseResult = null;
        }
    }

    private bool IsTemplateDefinition(AttributeParser.ParseResult lineParserResult, Dictionary<string, AttributeParser.ParseResult> defines) {
        if (!lineParserResult.Attributes.Remove(DefineId, out var defineName)) {
            return false;
        }
        if (defines.TryGetValue(defineName.ToString()!, out var previous)) {
            foreach (var tag in lineParserResult.Tags) previous.Tags.Add(tag);
            foreach (var (k, value) in lineParserResult.Attributes) previous.Attributes[k] = value;
        } else {
            defines[defineName.ToString()!] = lineParserResult;
        }
        return true;
    }

    private void UseTemplateDefinition(AttributeParser.ParseResult lineParserResult, AttributeParser.ParseResult currentParseResult, int lineNumber, string line, Dictionary<string, AttributeParser.ParseResult> defines) {
        if (!lineParserResult.Attributes.TryGetValue(TemplateId, out var templateName)) {
            return;
        }
        if (currentParseResult.Attributes.TryGetValue(TemplateId, out var previousTemplateName)) {
            throw new ArgumentException($"Error in line #{lineNumber}: {line}\nCan't use template definition \"{templateName}\", it already has a template \"{previousTemplateName}\": {currentParseResult}");
        }
        if (defines.TryGetValue(templateName.ToString()!, out var templateParseResult)) {
            foreach (var tag in templateParseResult.Tags) currentParseResult.Tags.Add(tag);
            foreach (var (k, value) in templateParseResult.Attributes) {
                currentParseResult.Attributes.TryAdd(k, value);
            }
        } else {
            throw new ArgumentException($"Error in line #{lineNumber}: {line}\nCan't find template \"{templateName}\"");
        }
    }

    private Array2D<char> ParseTemplateBody(List<string> lines, AttributeParser.ParseResult currentParseResult) {
        if (lines.Count == 0) {
            throw new ArgumentException($"Empty template for: {currentParseResult}");
        }

        var height = lines.Count;
        var width = lines[0].Length;

        // Validar que todas las líneas tengan la misma anchura
        for (var y = 1; y < height; y++) {
            if (lines[y].Length != width) {
                throw new ArgumentException(
                    $"Inconsistent dimensions in template: {currentParseResult}\n" +
                    $"Line {y} has width {lines[y].Length}, expected with was {width}:\n{string.Join("\n", lines)}");
            }
        }
        // Validar que las dimensiones coincidan con el tamaño esperado
        if (width != CellSize || height != CellSize) {
            throw new ArgumentException(
                $"Invalid dimensions {width}x{height} in template: {currentParseResult}\n" +
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

    public bool ValidateAll(Func<char, bool> isBlockingChar, bool throwException = false) {
        return _templates.Select(template => template.IsValid(isBlockingChar, throwException)).All(valid => valid);
    }

    public static readonly (string[], Transformations.Type Transform)[] Transformations = [
        (["rotate:90", "rotate:right", "rotate:clockwise"], DataMath.Transformations.Type.Rotate90),
        (["rotate:180", "rotate:twice"], DataMath.Transformations.Type.Rotate180),
        (["rotate:-90", "rotate:left", "rotate:270", "rotate:counterclockwise", "rotate:anticlockwise", "rotate:minus90"], DataMath.Transformations.Type.RotateMinus90),
        (["flip:h", "flip:x", "flip:horizontal"], DataMath.Transformations.Type.FlipH),
        (["flip:v", "flip:y", "flip:vertical"], DataMath.Transformations.Type.FlipV),
        (["flip:d", "flip:diagonal"], DataMath.Transformations.Type.FlipDiagonal),
        (["flip:ds", "flip:secondary", "flip:diagonalsecondary", "flip:diagonal-secondary"], DataMath.Transformations.Type.FlipDiagonalSecondary),
        (["mirror:lr", "mirror:we", "mirror:l-r", "mirror:w-e", "mirror:leftright", "mirror:westeast", "mirror:left-right", "mirror:west-east"], DataMath.Transformations.Type.MirrorLR),
        (["mirror:rl", "mirror:ew", "mirror:r-l", "mirror:e-w", "mirror:rightleft", "mirror:eastwest", "mirror:right-left", "mirror:east-west"], DataMath.Transformations.Type.MirrorRL),
        (["mirror:tb", "mirror:ud", "mirror:ns", "mirror:t-b", "mirror:u-d", "mirror:n-s", "mirror:topbottom", "mirror:updown", "mirror:northsouth", "mirror:top-bottom", "mirror:up-down", "mirror:north-south"], DataMath.Transformations.Type.MirrorTB),
        (["mirror:bt", "mirror:du", "mirror:sn", "mirror:b-t", "mirror:d-u", "mirror:s-n", "mirror:bottomtop", "mirror:downup", "mirror:southnorth", "mirror:bottom-top", "mirror:down-up", "mirror:south-north"], DataMath.Transformations.Type.MirrorBT)
    ];

    private static readonly Dictionary<string, Transformations.Type[]> TransformGroups = new() {
        ["rotate:all"] = [
            DataMath.Transformations.Type.Rotate90,
            DataMath.Transformations.Type.Rotate180,
            DataMath.Transformations.Type.RotateMinus90
        ],
        ["flip:all"] = [
            DataMath.Transformations.Type.FlipH,
            DataMath.Transformations.Type.FlipV,
            DataMath.Transformations.Type.FlipDiagonal,
            DataMath.Transformations.Type.FlipDiagonalSecondary
        ],
        ["mirror:all"] = [
            DataMath.Transformations.Type.MirrorLR,
            DataMath.Transformations.Type.MirrorRL,
            DataMath.Transformations.Type.MirrorTB,
            DataMath.Transformations.Type.MirrorBT
        ]
    };

    /// <summary>
    /// Reads the tags of every template and try to apply the transformations. "Try" means if a transformation
    /// results in an identical body or an invalid template (mirror transformation could break the connection between exits),
    /// it will not be added.
    /// </summary>
    public void ApplyTransformations(Func<char, bool> isBlocked) {
        foreach (var template in FindTemplates().ToArray() /* Clone to allow adding templates during the loop*/) {
            var transformsToApply = new HashSet<Transformations.Type>();
            foreach (var tag in template.Tags
                         .Where(t => t.Contains(':'))
                         .Select(t => t.ToLowerInvariant())) {
                // Check group tags (rotate:all, flip:all, mirror:all)
                if (TransformGroups.TryGetValue(tag, out var groupTransforms)) {
                    transformsToApply.UnionWith(groupTransforms);
                    continue;
                }

                // Check if tag starts with any transformation prefix
                var prefix =
                    tag.StartsWith("rotate:") ? "rotate:" :
                    tag.StartsWith("flip:") ? "flip:" :
                    tag.StartsWith("mirror:") ? "mirror:" : null;

                if (prefix == null) continue;
                
                var found = false;
                foreach (var (tags, transform) in Transformations) {
                    if (tags.Contains(tag)) {
                        transformsToApply.Add(transform);
                        found = true;
                        break;
                    }
                }
                if (!found) ThrowUnknownTransformationError(prefix, tag);
            }

            if (transformsToApply.Count > 0) {
                ApplyTransformations(template, transformsToApply, isBlocked);
            }
        }
    }

    private static void ThrowUnknownTransformationError(string prefix, string tag) {
        var validValues = Transformations
            .SelectMany(t => t.Item1)
            .Where(t => t.StartsWith(prefix))
            .Concat(TransformGroups.Keys.Where(k => k.StartsWith(prefix)))
            .OrderBy(t => t)
            .ToList();

        throw new ArgumentException(
            $"Invalid transformation tag '{tag}'. " +
            $"Valid values for prefix '{prefix}' are: {string.Join(", ", validValues)}");
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

    private void ApplyTransformations(Template template, IEnumerable<Transformations.Type> transformsToApply, Func<char, bool> isBlocked) {
        // Apply all collected transformations
        foreach (var transform in transformsToApply) {
            var transformed = template.TryTransform(transform);
            if (transformed == null ||
                !transformed.IsValid(isBlocked, false) ||
                _templates
                    .Where(t => t != template)
                    .Any(other => Template.BodyIsEquals(other.Body, transformed.Body))) {
                // TryTransform returns null if the transformation does not change the template body
                // or the transformed template is already in the set (same body)
                continue;
            }
            // transformed.SetAttribute("transformed.type", transform.ToString());
            var id = RuntimeHelpers.GetHashCode(template);
            transformed.SetAttribute(SharedId, id);
            template.SetAttribute(SharedId, id);

            AddTemplate(transformed);
        }
    }
}