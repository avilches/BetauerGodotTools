using System.Linq;
using Betauer.Core;
using Betauer.Core.PCG.GridTemplate;

namespace Veronenger.Game.Dungeon.World.Generation;

// Every template set is a collection of templates that can be used to generate a map
public enum TemplateSetType  : byte { // 1 byte = 256 values; short = 2 bytes = 65536 values
    Office,
}

public record TemplateSetTypeConfig(TemplateSetType Type) : EnumConfig<TemplateSetType, TemplateSetTypeConfig>(Type) {
    public TemplateSet TemplateSet { get; private set; }

    public TemplateSetTypeConfig LoadFromString(int cellSize, string templateContent) {
        TemplateSet = new TemplateSet(cellSize);
        TemplateSet.LoadFromString(templateContent);
        TemplateSet.FindTemplates(tags: ["disabled"]).ToArray().ForEach(t => TemplateSet.RemoveTemplate(t));
        TemplateSet.ApplyTransformations();
        return this;
    }
}
