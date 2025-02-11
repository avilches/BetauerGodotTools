using System;
using System.Linq;
using Betauer.Core;
using Betauer.Core.PCG.GridTemplate;
using Betauer.Core.PCG.Maze.Zoned;

namespace Veronenger.Game.Dungeon.World.Generation;

public enum MapType : byte {
    OfficeEasy,
}

public record MapTypeConfig(MapType Type, TemplateSet TemplateSet, Func<int, MazeZones> Factory) : EnumConfig<MapType, MapTypeConfig>(Type) {

    public MapTypeConfig(MapType Type, TemplateSetType templateSetType, Func<int, MazeZones> Factory) :
        this(Type, TemplateSetTypeConfig.Get(templateSetType).TemplateSet, Factory) {
    }

    public MazeZones Create(int seed) => Factory(seed);
}
