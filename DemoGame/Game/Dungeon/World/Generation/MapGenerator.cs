using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Betauer.Core;
using Betauer.Core.DataMath;
using Betauer.Core.Examples;
using Betauer.Core.PCG.GridTemplate;
using Betauer.Core.PCG.Maze;
using Betauer.Core.PCG.Maze.Zoned;
using Godot;

namespace Veronenger.Game.Dungeon.World.Generation;

public enum CellDefinitionType : byte {
    Empty,
    Wall,

    Floor,
    Door,
    Loot,
    Key
}

/// <summary>
/// Cell definition comes from the template. It tries to explain the behaviour expected from the cell.
/// For instance, the definition could be "loot", but the cell will be a Floor with, or without a loot.
/// </summary>
/// <param name="Type"></param>
public record CellDefinitionConfig(CellDefinitionType Type) : EnumConfig<CellDefinitionType, CellDefinitionConfig>(Type) {
    // Blocking means the cell is not traversable by the player or enemies.
    public required bool Blocking { get; init; }


    public required char TemplateCharacter { get; init; }
    public required Func<Vector2I, WorldCell?> Factory { get; init; }

    public static HashSet<char> AllChars { get; private set; }
    public static HashSet<char> BlockingChars { get; private set; }

    public static void InitializeDefaults() {
        RegisterAll(
            new CellDefinitionConfig(CellDefinitionType.Empty) { Blocking = true, TemplateCharacter = '.', Factory = (_) => null },
            new CellDefinitionConfig(CellDefinitionType.Wall) { Blocking = true, TemplateCharacter = '#', Factory = (pos) => new WorldCell(CellType.Wall, pos) },
            new CellDefinitionConfig(CellDefinitionType.Floor) { Blocking = false, TemplateCharacter = '·', Factory = (pos) => new WorldCell(CellType.Floor, pos) },
            new CellDefinitionConfig(CellDefinitionType.Door) { Blocking = false, TemplateCharacter = 'd', Factory = (pos) => new WorldCell(CellType.Door, pos) },
            new CellDefinitionConfig(CellDefinitionType.Loot) { Blocking = false, TemplateCharacter = '$', Factory = (pos) => new WorldCell(CellType.Floor, pos) },
            new CellDefinitionConfig(CellDefinitionType.Key) { Blocking = false, TemplateCharacter = 'k', Factory = (pos) => new WorldCell(CellType.Floor, pos) }
        );

        // Avoid duplicated characters
        HashSet<char> used = [];
        foreach (var config in All) {
            if (!used.Add(config.TemplateCharacter)) {
                throw new InvalidDataException($"Character {config.TemplateCharacter} is duplicated in the CellDefinitionConfig");
            }
        }

        AllChars = All.Select(c => c.TemplateCharacter).ToHashSet();
        BlockingChars = All.Where(c => c.Blocking).Select(c => c.TemplateCharacter).ToHashSet();
    }

    public static CellDefinitionType Find(char cell) {
        return All.First(config => config.TemplateCharacter == cell).Type;
    }

    public static bool IsValid(char c) => AllChars.Contains(c);
    public static bool IsBlockingChar(char c) => BlockingChars.Contains(c);

    public static WorldCell? CreateCell(char c, Vector2I pos) {
        var cellDef = All.First(config => config.TemplateCharacter == c);
        var worldCell = cellDef.Factory(pos);
        if (worldCell != null) {
            worldCell.CellDefinitionConfig = cellDef;
        }
        return worldCell;
    }
}

public static class MazeNodeExtensions {
    public static void SetTemplate(this MazeNode node, Template template) => node.SetAttribute("template", template);
    public static Template GetTemplate(this MazeNode node) => node.GetAttributeAs<Template>("template")!;

    public static void AddWorldCell(this MazeNode node, WorldCell cells) => node.GetCells().Add(cells);
    public static List<WorldCell?> GetCells(this MazeNode node) => node.GetAttributeOrCreate<List<WorldCell?>>("worldcells", () => []);
}

public class MapGenerator {
    // The MazeNode contains two attributes
    public record MapGenerationResult(MazeZones Zones, Array2D<WorldCell?> WorldCellMap);

    public static MapGenerationResult CreateMap(MapType mapType, int seed) {
        var mapTypeConfig = MapTypeConfig.Get(mapType);
        var zones = mapTypeConfig.Create(seed);
        var templateSet = mapTypeConfig.TemplateSet;

        // Creates an empty array2D of templates. This will be used in the Render method to track the template used for
        // each node during the selection the next one. The first one will be random, but the next one should try to match the
        // previous: the down side of the top template should match to the up side, the some for the node at the left: the right side of it
        // should match the left side of the current node.
        var templateArray = zones.MazeGraph.ToArray2D(Template? (_, _) => null);
        var rngMap = new Random(seed);

        Template lastTemplate = null!;
        Array2D<WorldCell?> worldCellMap = zones.MazeGraph.Render((nodePos, node) => {
                lastTemplate = GetTemplate(rngMap, templateArray, templateSet, nodePos, node);
                node.SetTemplate(lastTemplate);
                return lastTemplate.Body;
            },
            (nodePos, node, cellPosition, cellChar) => {
                var worldCell = CellDefinitionConfig.CreateCell(cellChar, cellPosition);
                if (worldCell != null) {
                    node.AddWorldCell(worldCell);
                }
                return worldCell;
            });

        MazeGraphZonedDemo.PrintGraph(zones.MazeGraph, zones);
        // PrintTemplates(templateSet.FindTemplates().ToList());

        return new MapGenerationResult(zones, worldCellMap);
    }

    public static void Spawn(MapType mapType, MazeZones zones, Array2D<WorldCell?> worldCellMap) {
        var solution = zones.CalculateSolution(MazeGraphCatalog.KeyFormula);
        var keys = solution.KeyLocations;
        var loot = zones.SpreadLocationsByZone(0.3f, MazeGraphCatalog.LootFormula);
    }

    // Returns the template
    private static Template GetTemplate(Random rngMap, Array2D<Template?> templateArray, TemplateSet templateSet, Vector2I pos, MazeNode node) {
        var upNodeTemplate = templateArray.GetValueSafe(pos + Vector2I.Up);
        var leftNodeTemplate = templateArray.GetValueSafe(pos + Vector2I.Left);
        var allTemplates = templateSet.FindTemplates(node.GetDirectionFlags()).ToList();
        var candidates = allTemplates
            .Where(t => MatchesNodeUpDown(upNodeTemplate, t) && MatchesNodeLeftRight(leftNodeTemplate, t))
            .ToList();
        var x = candidates.Count;

        RemoveSimilarCandidates(candidates, templateArray);
        Console.WriteLine($"Candidates before: {x} - after: {candidates.Count} = {x - candidates.Count} delted");
        Template template = null!;
        if (candidates.Count == 0) {
            throw new Exception($"Warning: no matching template for node {node} at position {pos}. Direction: {DirectionFlagTools.FlagsToString(node.GetDirectionFlags())} ");
            // template = rngMap.Next(allTemplates.ToArray());
        } else {
            template = rngMap.Next(candidates);
        }
        templateArray[pos] = template;
        return template;

        static bool MatchesNodeUpDown(Template? upNodeTemplate, Template? downNodeTemplate) {
            return upNodeTemplate == null || downNodeTemplate == null ||
                   Equals(upNodeTemplate.GetAttributeOrDefault(DirectionFlag.Down, "size", 1), downNodeTemplate.GetAttributeOrDefault(DirectionFlag.Up, "size", 1));
        }

        static bool MatchesNodeLeftRight(Template? leftNodeTemplate, Template? rightNodeTemplate) {
            return leftNodeTemplate == null || rightNodeTemplate == null ||
                   Equals(leftNodeTemplate.GetAttributeOrDefault(DirectionFlag.Right, "size", 1), rightNodeTemplate.GetAttributeOrDefault(DirectionFlag.Left, "size", 1));
        }

        // Removes all templates from the templateArray array that are similar to the check template
        // the matchingTemplates will end up with at least one element (we don't want to delete all the templates!)
        static void RemoveSimilarCandidates(List<Template> matchingTemplates, Array2D<Template?> templateArray) {
            foreach (var check in templateArray.GetIndexedValues()
                         .Select(tuple => tuple.Value)
                         .Where(t => t != null).Cast<Template>().Reverse()) {
                foreach (var similar in matchingTemplates.Where(t => TemplateSet.ShareOrigin(t, check)).ToArray()) {
                    if (matchingTemplates.Remove(similar) && matchingTemplates.Count == 1) return;
                }
            }
        }
    }

    public static void Validate() {
        HashSet<char> lootOrKeys = [
            CellDefinitionConfig.Get(CellDefinitionType.Key).TemplateCharacter,
            CellDefinitionConfig.Get(CellDefinitionType.Loot).TemplateCharacter
        ];

        TemplateSetTypeConfig.All.ForEach(mapConfig => {
            foreach (var t in mapConfig.TemplateSet.FindTemplates()) {
                var hasLootOrKey = t.Body.GetValues().Any(c => lootOrKeys.Contains(c));
                if (!hasLootOrKey) {
                    // throw new InvalidDataException($"Template: {t}\n does not have a loot definition: {string.Join(",", lootOrKeys.Select(l => $"'{l}'"))}\n{t.Body}");
                }
                var found = t.Body.GetValues().FirstOrDefault((c) => !CellDefinitionConfig.IsValid(c), '\0');
                if (found != '\0') {
                    throw new InvalidDataException($"Template: {t}\n invalid character '{found}'. Valid characters are: {string.Join(",", CellDefinitionConfig.AllChars.Select(l => $"'{l}'"))}\n{t.Body}");
                }
            }
        });
    }
}