using System.Collections.Generic;
using Betauer.Core.PCG.GridTemplate;
using Betauer.Core.PCG.Maze;

namespace Veronenger.Game.Dungeon.World.Generation;

public static class MazeNodeExtensions {
    public static void SetTemplate(this MazeNode node, Template template) => node.SetAttribute("template", template);
    public static Template GetTemplate(this MazeNode node) => node.GetAttributeAs<Template>("template")!;

    public static void AddWorldCell(this MazeNode node, WorldCell cells) => node.GetCells().Add(cells);
    public static List<WorldCell?> GetCells(this MazeNode node) => node.GetAttributeOrCreate<List<WorldCell?>>("worldcells", () => []);
}