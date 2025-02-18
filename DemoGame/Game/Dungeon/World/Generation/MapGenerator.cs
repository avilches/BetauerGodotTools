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

public class MapGenerator {
    // The MazeNode contains two attributes
    public record MapGenerationResult(MazeZones Zones, Array2D<WorldCell?> WorldCellMap, WorldCell StartCell);

    public static MapGenerationResult GenerateMap(MapType mapType, int seed) {
        var mapTypeConfig = MapTypeConfig.Get(mapType);
        var zones = mapTypeConfig.GenerateZones(seed);
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

        return new MapGenerationResult(zones, worldCellMap, FindCenterCell(zones.Start.GetCells()!, cell => !cell.CellDefinitionConfig.Blocking));
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