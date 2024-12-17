using System.Collections.Generic;
using Betauer.Core.DataMath;

namespace Betauer.Core.PCG.GridTemplate;

public class Template(TemplateId id) {
    public TemplateId Id { get; } = id;
    public Array2D<char> Data { get; set; }
    public Template? TemplateBase { get; set; }
    public string? TransformBase { get; set; }

    public bool HasExactFlags(string[] requiredFlags) {
        return Id.Flags.SetEquals(new HashSet<string>(requiredFlags));
    }

    public override string ToString() {
        return Id.ToString();
    }
}