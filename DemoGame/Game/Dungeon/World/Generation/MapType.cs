using System.Linq;
using Betauer.Core;
using Betauer.Core.PCG.GridTemplate;

namespace Veronenger.Game.Dungeon.World.Generation;

public enum MapType  : byte { // 1 byte = 256 values; short = 2 bytes = 65536 values
    Wait,
}

public record MapTypeConfig(MapType Type) : EnumConfig<MapType, MapTypeConfig>(Type) {
    public TemplateSet TemplateSet { get; private set; }

    public MapTypeConfig LoadFromString(int cellSize, string templateContent) {
        TemplateSet = new TemplateSet(cellSize);
        TemplateSet.LoadFromString(templateContent);
        TemplateSet.FindTemplates(tags: ["disabled"]).ToArray().ForEach(t => TemplateSet.RemoveTemplate(t));
        TemplateSet.ApplyTransformations();
        return this;
    }
}