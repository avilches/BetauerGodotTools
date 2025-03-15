using System.Collections.Generic;
using Betauer.Core;
using Betauer.Core.PCG.GridTemplate;
using Betauer.Core.PCG.Maze;

namespace Veronenger.Game.Dungeon.World.Generation;

public static class MazeNodeExtensions {
    public static void SetTemplate(this MazeNode node, Template template) => node.Attributes().Set("template", template);
    public static Template GetTemplate(this MazeNode node) => node.Attributes().GetAs<Template>("template")!;

    public static void AddWorldCell(this MazeNode node, WorldCell cell) => node.GetCells().Add(cell);
    public static List<WorldCell?> GetCells(this MazeNode node) => node.Attributes().GetOrCreate<List<WorldCell?>>("worldcells", () => []);

    public static void SetMazeNode(this WorldCell cell, MazeNode node) => cell.Attributes().Set("mazenode", node);

    public static MazeNode? GetMazeNode(this WorldCell cell) => cell.Attributes().GetAs<MazeNode>("mazenode");
    public static MazeNode? GetMazeNode(this EntityBase entity) => entity.Location.Cell.GetMazeNode();
}