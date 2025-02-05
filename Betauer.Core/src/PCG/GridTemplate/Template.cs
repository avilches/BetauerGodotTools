using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;

namespace Betauer.Core.PCG.GridTemplate;

public class Template {
    public int DirectionFlags { get; set; }= 0;
    public HashSet<string> Tags { get; set; } = [];
    public Array2D<char> Body { get; set; }

    public bool HasExactTags(IEnumerable<string> tags) {
        return Tags.SetEquals(new HashSet<string>(tags));
    }

    public bool HasAllTags(IEnumerable<string> tags) {
        return tags.All(tag => Tags.Contains(tag));
    }

    public bool HasTag(string tag) {
        return Tags.Contains(tag);
    }

    public bool HasAnyTag(IEnumerable<string> tags) {
        return tags.Any(tag => Tags.Contains(tag));
    }

    public IEnumerable<string> MatchingTags(string[] tags) {
        return tags.Where(tag => Tags.Contains(tag));
    }

    public override string ToString() {
        var baseString = DirectionFlagTools.FlagsToString(DirectionFlags);
        if (Tags.Count == 0) return baseString;
        return baseString + "/" + string.Join("/", Tags);
    }

    public Template Transform(Transformations.Type type) {
        return new Template {
            DirectionFlags = DirectionTransformations.TransformDirections(DirectionFlags, type),
            Tags = [..Tags],
            Body = new Array2D<char>(Body.Data.Transform(type))
        };
    }
}