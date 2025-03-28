using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Betauer.Core;
using Betauer.Core.DataMath;
using Betauer.Core.Examples;
using Betauer.Core.PCG;
using Betauer.Core.PCG.GridTemplate;
using Betauer.Core.PCG.Maze;
using Betauer.Core.PCG.Maze.Zoned;
using Godot;

namespace Veronenger.Game.Dungeon.World.Generation;

public class MapGenerator {
    public record MapGenerationResult(MazeZones Zones, Array2D<WorldCell?> WorldCellMap);

    public static MapGenerationResult GenerateMap(MapType mapType, int seed) {
        var mapTypeConfig = MapTypeConfig.Get(mapType);
        var zones = mapTypeConfig.GenerateZones(seed);
        var templateSet = mapTypeConfig.TemplateSet;

        // Creates an empty array2D of templates. This will be filled later step by step to track the template used for
        // each node during the selection the next one. The first one will be random, but the next one should try to match the
        // previous: the down side of the top template should match to the up side, the some for the node at the left: the right side of it
        // should match the left side of the current node.
        var templateArray = new Array2D<Template?>(zones.MazeGraph.GetSize());
        var rngMap = new Random(seed);
        var size = templateSet.CellSize;
        var nodeArray = zones.MazeGraph.ToArray2D<MazeNode>((nodePos, node) => {
            if (node == null) return null;
            var template = GetTemplate(rngMap, templateArray, templateSet, nodePos, node);
            node.SetTemplate(template);
            return node;
        });

        var worldCellMap = templateArray.Expand<WorldCell>(size, (pos, template) => {
            if (template == null) return null;
            var node = nodeArray[pos];
            var expandedPart = new Array2D<WorldCell?>(size, size);
            foreach (var (innerPos, templateCell) in template.Body.GetIndexedValues()) {
                var worldCell = CellDefinitionConfig.CreateCell(templateCell, pos * size  + innerPos);
                if (worldCell != null) {
                    node.AddWorldCell(worldCell);
                    worldCell.SetMazeNode(node);
                }
                expandedPart[innerPos] = worldCell;
            }
            return expandedPart;
        });

        var r = new RegionConnections(worldCellMap.Clone(c => c != null));
        r.Update();

        Console.WriteLine(r.Labels.GetString(tile =>
            tile == 0
                ? " "
                : tile.ToString("x8").Substring(7, 1)));

        /*
        // Double the size to create a 2x2 grid for each cell in the template
        var doubledSize = size * 2;
        var worldCellMap = templateArray.Expand<WorldCell>(doubledSize, (pos, template) => {
            if (template == null) return null;
            var node = nodeArray[pos];
            var expandedPart = new Array2D<WorldCell?>(doubledSize, doubledSize);

            foreach (var (innerPos, templateCell) in template.Body.GetIndexedValues()) {
                // For each template cell, create a 2x2 grid of world cells
                for (int dy = 0; dy < 2; dy++) {
                    for (int dx = 0; dx < 2; dx++) {
                        // Calculate the doubled position
                        var doubledInnerPos = new Vector2I(innerPos.X * 2 + dx, innerPos.Y * 2 + dy);
                        // Create the world cell at the new position
                        var worldCell = CellDefinitionConfig.CreateCell(templateCell, pos * doubledSize + doubledInnerPos);
                        if (worldCell != null) {\\\\\
                            node.AddWorldCell(worldCell);
                            worldCell.SetMazeNode(node);
                        }
                        expandedPart[doubledInnerPos] = worldCell;
                    }
                }
            }
            return expandedPart;
        });
        */




        MazeGraphZonedDemo.PrintGraph(zones.MazeGraph, zones);
        // PrintTemplates(templateSet.FindTemplates().ToList());

        return new MapGenerationResult(zones, worldCellMap);
    }


    public static WorldCell FindCenterCell(List<WorldCell> worldCells, Func<WorldCell, bool> isCellValid) {
        if (worldCells == null || worldCells.Count == 0) {
            throw new InvalidOperationException("No cells to find the center");
        }

        // Creamos un diccionario para acceso rápido por posición
        var cellsByPosition = worldCells.ToDictionary(cell => cell.Position);

        // Encontrar los límites del área
        int minX = int.MaxValue, maxX = int.MinValue;
        int minY = int.MaxValue, maxY = int.MinValue;

        foreach (var pos in cellsByPosition.Keys) {
            minX = Math.Min(minX, pos.X);
            maxX = Math.Max(maxX, pos.X);
            minY = Math.Min(minY, pos.Y);
            maxY = Math.Max(maxY, pos.Y);
        }

        // Calculamos el centro aproximado
        var centerX = (minX + maxX) / 2;
        var centerY = (minY + maxY) / 2;
        Vector2I center = new(centerX, centerY);

        // Conjunto para trackear las posiciones ya visitadas
        var visited = new HashSet<Vector2I>();

        // Cola de prioridad para obtener las posiciones en orden de distancia al centro
        PriorityQueue<Vector2I, float> queue = new();
        queue.Enqueue(center, 0);

        while (queue.Count > 0) {
            var pos = queue.Dequeue();

            if (!visited.Add(pos)) continue;

            // Si encontramos una celda válida en esta posición, la retornamos
            if (cellsByPosition.TryGetValue(pos, out var cell) && isCellValid(cell)) {
                return cell;
            }

            // Añadimos las 8 posiciones adyacentes a la cola
            for (int dy = -1; dy <= 1; dy++) {
                for (int dx = -1; dx <= 1; dx++) {
                    if (dx == 0 && dy == 0) continue;

                    Vector2I newPos = new(pos.X + dx, pos.Y + dy);

                    // Solo añadimos posiciones dentro de los límites
                    if (newPos.X < minX || newPos.X > maxX ||
                        newPos.Y < minY || newPos.Y > maxY) {
                        continue;
                    }

                    if (visited.Contains(newPos)) continue;

                    // Calculamos la distancia Manhattan al centro
                    float distance = Math.Abs(newPos.X - centerX) + Math.Abs(newPos.Y - centerY);
                    queue.Enqueue(newPos, distance);
                }
            }
        }

        return null;
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
                   Equals(upNodeTemplate.GetAttributeOr(DirectionFlag.Down, "size", 1), downNodeTemplate.GetAttributeOr(DirectionFlag.Up, "size", 1));
        }

        static bool MatchesNodeLeftRight(Template? leftNodeTemplate, Template? rightNodeTemplate) {
            return leftNodeTemplate == null || rightNodeTemplate == null ||
                   Equals(leftNodeTemplate.GetAttributeOr(DirectionFlag.Right, "size", 1), rightNodeTemplate.GetAttributeOr(DirectionFlag.Left, "size", 1));
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