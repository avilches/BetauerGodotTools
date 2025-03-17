using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;
using Godot;

namespace Veronenger.Game.Dungeon.GenCity;

public class BuildingGenerator {
    public static List<Building> GenerateBuildings(City city, List<Path> paths, BuildingGenerationOptions options) {
        if (options.Total == 0) return [];
        var random = new Random(options.Seed);
        var buildings = new List<Building>();
        foreach (var path in paths) {
            var nextPos = path.Start.Position + path.Direction;

            foreach (var facing in CityGenerator.BothTurnDirections(path.Direction)) {
                if (!SidewalkIsEmpty(path, facing)) {
                    continue;
                }
                
                var stepOffset = options.Sidewalk;
                while (stepOffset < path.GetLength()) {
                    var stepShift = path.Direction * stepOffset;
                    var shiftFromPath = facing * (options.Sidewalk + 1);
                    var startPosition = nextPos + stepShift + shiftFromPath;

                    var buildingWidth = random.Next(options.MinSize, options.MaxSize + 1);
                    var buildingHeight = random.Next(options.MinSize, options.MaxSize + 1);

                    if (stepOffset + buildingWidth + options.Sidewalk > path.GetLength()) {
                        break;
                    }
                    
                    var building = CreateBuilding(path, startPosition, buildingWidth, buildingHeight, facing);
                    if (building != null) {
                        buildings.Add(building);
                        if (options.Total > 0 && buildings.Count >= options.Total) {
                            return buildings;
                        }
                    }

                    var spaceBetweenBuildings = random.Next(options.MinSpace, options.MaxSpace + 1);
                    stepOffset += buildingWidth + spaceBetweenBuildings;
                }
            }
        }
        return buildings; 

        bool SidewalkIsEmpty(Path path, Vector2I facing) {
            if (options.Sidewalk == 0) return true;
            foreach (var pathPos in path.GetPathOnlyPositions()) {
                var checkPos = pathPos;
                for (var i = 1; i <= options.Sidewalk; i++) {
                    checkPos += facing;
                    if (!city.Data.IsInBounds(checkPos) || city.Data[checkPos] != null) {
                        return false;
                    }
                }
            }
            return true;
        }

        Building? CreateBuilding(Path path, Vector2I position, int buildingWidth, int buildingHeight, Vector2I facing) {
            var (minX, minY, maxX, maxY) = (int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);
            // Primero recopilamos todas las posiciones para verificar que el edificio se puede colocar
            for (var i = 0; i < buildingWidth; i++) {
                var shiftParallel = path.Direction * i;
                var startFromPathPosition = position + shiftParallel;

                for (var j = 0; j < buildingHeight; j++) {
                    var shiftPerpendicular = facing * j;
                    var tilePosition = startFromPathPosition + shiftPerpendicular;

                    if (city.Data.IsInBounds(tilePosition) && city.Data[tilePosition] == null) {
                        (minX, minY) = (Math.Min(minX, tilePosition.X), Math.Min(minY, tilePosition.Y));
                        (maxX, maxY) = (Math.Max(maxX, tilePosition.X), Math.Max(maxY, tilePosition.Y));
                    } else {
                        return null;
                    }
                }
            }

            var buildingRect = new Rect2I(minX, minY, maxX - minX + 1, maxY - minY + 1);
            var building = city.CreateBuilding(path, buildingRect);

            // Determinar las celdas del borde del edificio que están adyacentes al camino
            var potentialPortals = new List<(Vector2I buildingCell, Vector2I pathEntrance)>();

            // Buscar todas las celdas del edificio que están en el borde adyacente al camino
            // Iterar sobre las posiciones del camino para encontrar celdas del edificio adyacentes
            var pathPositions = path.GetPathOnlyPositions();
            foreach (var pathPos in pathPositions) {
                var buildingCell = pathPos + (facing * (options.Sidewalk + 1));

                // Verificar si la celda es parte del edificio
                if (building.Bounds.HasPoint(buildingCell)) {
                    potentialPortals.Add((buildingCell, pathPos));
                }
            }
            // Si encontramos potenciales portales, elegir uno aleatoriamente
            if (potentialPortals.Count > 0) {
                // Si hay 4 o mas candidates, descartar los laterales
                if (potentialPortals.Count > 3) {
                    potentialPortals.RemoveAt(0);
                    potentialPortals.RemoveAt(potentialPortals.Count - 1);
                }
                var selectedPortal = random.Next(potentialPortals.Count);
                var (entrance, pathEntrance) = potentialPortals[selectedPortal];

                // Asignar los valores al edificio
                building.Entrance = entrance;
                building.PathEntrance = pathEntrance;
                building.PathDirection = path.Direction;
            }
            return building;
        }
    }
}