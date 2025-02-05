using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;

namespace Betauer.Core.PCG.GridTemplate;

public class TemplateSet(int cellSize) {
    private readonly List<Template> _templates = [];
    public string ParserIdPrefix { get; set; } = "@ID=";

    public int CellSize { get; } = cellSize;

    public void P() {
        var templates = FindTemplates();
        foreach (var template in templates) {
            AddTemplate(template.Transform(Transformations.Type.Rotate90));
            AddTemplate(template.Transform(Transformations.Type.Rotate180));
            AddTemplate(template.Transform(Transformations.Type.RotateMinus90));
        }
    }

    public void AddTemplate(Template template) {
        _templates.Add(template);
    }


    /// <summary>
    /// Finds templates that match the specified criteria. When searching by flags,
    /// templates must have ALL the specified flags to be included in the results.
    /// </summary>
    /// <param name="type">Optional. If specified, only templates of this type will be returned.</param>
    /// <param name="flags">Optional. If specified, only templates that have ALL of these flags will be returned.</param>
    /// <returns>A list of Template objects that match the specified criteria.</returns>
    /// <example>
    /// // Find all templates
    /// var allTemplates = FindTemplates();
    ///
    /// // Find templates of type 1
    /// var typeTemplates = FindTemplates(type: 1);
    ///
    /// // Find templates with ALL these flags
    /// var templates = FindTemplates(flags: new[] {"wall", "stone"}); // Must have BOTH flags
    ///
    /// // Find templates with ANY of these flags
    /// var anyFlagTemplates = FindTemplatesWithAnyFlag(new[] {"decorated", "damaged"}); // Must have at least one
    ///
    /// // Find templates with EXACT flags
    /// var exactTemplates = FindTemplatesWithExactFlags(new[] {"wall", "stone"}); // Must have exactly these flags
    ///
    /// // Find templates excluding specific flags
    /// var excludingTemplates = FindTemplatesExcluding(new[] {"damaged"}); // Must not have these flags
    /// </example>
    public List<Template> FindTemplates(int? type = null, string[]? flags = null) {
        var query = _templates.AsEnumerable();

        if (type.HasValue) {
            query = query.Where(t => t.Header.Type == type.Value);
        }

        if (flags != null && flags.Length > 0) {
            query = query.Where(t => t.HasAllFlags(flags));
        }

        return query.ToList();
    }

    /// <summary>
    /// Returns the first template with the type and the flags
    /// </summary>
    /// <param name="type"></param>
    /// <param name="flags"></param>
    /// <returns></returns>
    public Template? GetTemplate(int type, string[] flags) => _templates.FirstOrDefault(t => t.Header.Type == type && t.HasExactFlags(flags));

    public void LoadFromString(string content, char lineSeparator = '\n') {
        var buffer = new List<string>();
        Template? current = null;

        foreach (var line in content.Split(lineSeparator)
                     .Select(l => l.Trim())
                     .Where(v => v.Length > 0)) {
            if (line.StartsWith(ParserIdPrefix)) {
                ProcessPreviousTemplate();
                var headerString = line.Remove(0, ParserIdPrefix.Length).Split('#', StringSplitOptions.TrimEntries)[0];
                var header = TemplateHeader.Parse(headerString);
                current = new Template(header);
            } else if (current != null) {
                buffer.Add(line);

                if (buffer.Count == CellSize) {
                    ProcessPreviousTemplate();
                } else if (buffer.Count > CellSize) {
                    throw new ArgumentException($"Too many lines in template {current.Header}. Expected {CellSize} but got more.");
                }
            }
        }
        ProcessPreviousTemplate();
        return;

        void ProcessPreviousTemplate() {
            if (current == null) return;
            if (buffer.Count > 0) {
                var template = ParseTemplateBody(buffer, current.Header);
                current.Body = template;
            }
            AddTemplate(current);
            buffer.Clear();
            current = null;
        }
    }

    private Array2D<char> ParseTemplateBody(List<string> lines, TemplateHeader templateHeader) {
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
}