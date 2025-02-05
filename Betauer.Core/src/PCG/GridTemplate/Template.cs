using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;

namespace Betauer.Core.PCG.GridTemplate;

public class Template(TemplateHeader header) {
    public TemplateHeader Header { get; } = header;
    public Array2D<char> Body { get; set; }

    public bool HasExactFlags(IEnumerable<string> requiredFlags) {
        return Header.Flags.SetEquals(new HashSet<string>(requiredFlags));
    }

    public bool HasAllFlags(IEnumerable<string> requiredFlags) {
        return requiredFlags.All(flag => Header.Flags.Contains(flag));
    }

    public bool HasFlag(string flag) {
        return Header.Flags.Contains(flag);
    }

    public bool HasAnyFlags(IEnumerable<string> requiredFlags) {
        return requiredFlags.Any(flag => Header.Flags.Contains(flag));
    }

    public IEnumerable<string> MatchingFlags(string[] flags) {
        return flags.Where(flag => Header.Flags.Contains(flag));
    }

    public override string ToString() {
        return Header.ToString();
    }

    public Template Transform(Transformations.Type type) {
        var newData = new Array2D<char>(Body.Data.Transform(type));
        var newType = DirectionTransformations.TransformDirections(Header.Type, type);
        return new Template(new TemplateHeader(newType, Header.Flags)) {
            Body = newData
        };
    }
}