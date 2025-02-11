using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Betauer.Core;
using Betauer.Core.DataMath;
using Betauer.Core.Examples;
using Betauer.Core.PCG.GridTemplate;
using Betauer.Core.PCG.Maze;
using Godot;

namespace Veronenger.Game.Dungeon.World.Generation;

public record MapGenerationResult(List<SpawnInfo> Spawn, Array2D<Template?> TemplateArray, Array2D<WorldCell?> Map);

public struct SpawnInfo {
    public Vector2I Position;
    public SpawnType Type;
    public int Zone;
}

public enum SpawnType {
    Loot,
    Key,
}

public static class MapGenerator {
    public const char DefinitionLoot = 'l';
    public const char DefinitionDoor = 'd';
    public const char DefinitionWall = '#';
    public const char DefinitionFloor = '·';
    public const char DefinitionEmpty = '.';

    public static MapGenerationResult CreateMap(string templatePath, int seed) {
        var rng = new Random(seed);
        var zones = MazeGraphCatalog.CogmindLong(rng, mc => {
            // mc.OnNodeCreated += (node) => PrintGraph(mc);
        });
        var solution = zones.CalculateSolution(MazeGraphCatalog.KeyFormula);
        var keys = solution.KeyLocations;
        var loot = zones.SpreadLocationsByZone(0.3f, MazeGraphCatalog.LootFormula);

        var templateSet = new TemplateSet(cellSize: 9);
        var spawn = new List<SpawnInfo>();

        // Cargar patrones de diferentes archivos
        LoadTemplates(templatePath, templateSet);

        // Creates an empty array2D of templates. This will be used in the Render method to track the template used for
        // each node during the selection the next one. The first one will be random, but the next one should try to match the
        // previous: the down side of the top template should match to the up side, the some for the node at the left: the right side of it
        // should match the left side of the current node.
        var templateArray = zones.MazeGraph.ToArray2D(Template? (_, _) => null);

        var rngMap = new Random(seed);

        var nodeShouldHaveLoot = false;
        var nodeShouldHaveKey = false;
        var nodeLootAdded = false;
        var nodeKeyAdded = false;
        var templateNodes = zones.MazeGraph.Render((nodePos, node) => {
                if (nodeShouldHaveKey && !nodeKeyAdded) {
                    // throw new InvalidOperationException($"Node {node} should have a key but it was not added");
                }
                if (nodeShouldHaveLoot && !nodeLootAdded) {
                    // throw new InvalidOperationException($"Node {node} should have a key but it was not added");
                }
                nodeLootAdded = false;
                nodeKeyAdded = false;
                var zone = zones.Zones[node.ZoneId];
                var nodeWithKeyInZone = keys[zone.ZoneId];
                nodeShouldHaveKey = nodeWithKeyInZone == node;
                return NodeRenderer(rngMap, templateArray, templateSet, nodePos, node);
            },
            (nodePos, node, cellPosition, cell) => {
                if (cell == DefinitionLoot) {
                    if (nodeShouldHaveLoot) {
                        if (!nodeLootAdded) {
                            nodeLootAdded = true;
                            spawn.Add(new SpawnInfo { Position = cellPosition, Type = SpawnType.Loot, Zone = node.ZoneId });
                        }
                    } else if (nodeShouldHaveKey) {
                        nodeLootAdded = true;
                        spawn.Add(new SpawnInfo { Position = cellPosition, Type = SpawnType.Key, Zone = node.ZoneId });
                    }
                }
                return WorldCellTypeConverter(cellPosition, cell);
            });

        MazeGraphZonedDemo.PrintGraph(zones.MazeGraph, zones);
        // PrintTemplates(templateSet.FindTemplates().ToList());

        return new MapGenerationResult(spawn, templateArray, templateNodes);
    }

    private static void LoadTemplates(string templatePath, TemplateSet templateSet) {
        try {
            var content = File.ReadAllText(templatePath);
            templateSet.LoadFromString(content);
            templateSet.FindTemplates(tags: ["disabled"]).ToArray().ForEach(t => templateSet.RemoveTemplate(t));
            templateSet.ApplyTransformations();
        } catch (FileNotFoundException e) {
            Console.WriteLine(e.Message);
            Console.WriteLine($"{nameof(templatePath)} is '{templatePath}'");
            Console.WriteLine("Ensure the working directory is the root of the project");
            throw;
        }
    }

    private static void PrintTemplates(List<Template> templates) {
        foreach (var template in templates.OrderBy(t => t.Attributes["Template"]).ThenBy(t => t.Attributes.GetValueOrDefault("shared.id"))) {
            Console.WriteLine(template);
            Console.WriteLine(template.Body);
        }
    }

    private static Array2D<char> NodeRenderer(Random rngMap, Array2D<Template?> templateArray, TemplateSet templateSet, Vector2I pos, MazeNode node) {
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
            Console.WriteLine($"Warning: no matching template for node {node} at position {pos}. Direction: {DirectionFlagTools.FlagsToString(node.GetDirectionFlags())} ");
            template = rngMap.Next(allTemplates.ToArray());
        } else {
            template = rngMap.Next(candidates);
        }
        templateArray[pos] = template;
        return template.Body;

        static bool MatchesNodeUpDown(Template? upNodeTemplate, Template? downNodeTemplate) {
            return upNodeTemplate == null || downNodeTemplate == null ||
                   Equals(upNodeTemplate.GetAttribute(DirectionFlag.Down), downNodeTemplate.GetAttribute(DirectionFlag.Up));
        }

        static bool MatchesNodeLeftRight(Template? leftNodeTemplate, Template? rightNodeTemplate) {
            return leftNodeTemplate == null || rightNodeTemplate == null ||
                   Equals(leftNodeTemplate.GetAttribute(DirectionFlag.Right), rightNodeTemplate.GetAttribute(DirectionFlag.Left));
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


    private static WorldCell? WorldCellTypeConverter(Vector2I position, char c) {
        return c switch {
            DefinitionEmpty => null,
            DefinitionWall => new WorldCell(CellType.Wall, position),
            DefinitionFloor or
                DefinitionLoot => new WorldCell(CellType.Floor, position),
            DefinitionDoor => new WorldCell(CellType.Door, position),
            _ => throw new ArgumentOutOfRangeException(nameof(c), c, "Invalid character")
        };
    }
}