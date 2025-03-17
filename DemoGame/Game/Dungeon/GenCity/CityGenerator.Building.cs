using System;
using System.Collections.Generic;
using Godot;

namespace Veronenger.Game.Dungeon.GenCity;

public partial class CityGenerator {
    public void GenerateBuildings() {
        var random = new Random(Options.Seed);
        foreach (var path in City.GetAllPaths()) {
            var nextPos = path.Start.Position + path.Direction;

            foreach (var facing in TurnDirection(path.Direction)) {
                var stepOffset = Options.BuildingOffset;

                while (stepOffset < path.GetLength()) {
                    var stepShift = path.Direction * stepOffset;
                    var shiftFromPath = facing * (Options.BuildingOffset + 1);
                    var startPosition = nextPos + stepShift + shiftFromPath;

                    var buildingWidth = random.Next(Options.BuildingMinSize, Options.BuildingMaxSize + 1);
                    var buildingHeight = random.Next(Options.BuildingMinSize, Options.BuildingMaxSize + 1);

                    if (stepOffset + buildingWidth + Options.BuildingOffset > path.GetLength()) {
                        break;
                    }
                    ProcessingBuilding(path, startPosition, buildingWidth, buildingHeight, path.Direction, facing);

                    var spaceBetweenBuildings = random.Next(Options.BuildingMinSpace, Options.BuildingMaxSpace + 1);
                    stepOffset += buildingWidth + spaceBetweenBuildings;
                }
            }
        }
        return;

        void ProcessingBuilding(Path path, Vector2I position, int buildingWidth, int buildingHeight, Vector2I pathDirection, Vector2I facing) {
            var tiles = new List<Vector2I>();

            // Primero recopilamos todas las posiciones para verificar que el edificio se puede colocar
            for (var i = 0; i < buildingWidth; i++) {
                var shiftParallel = pathDirection * i;
                var startFromPathPosition = position + shiftParallel;

                for (var j = 0; j < buildingHeight; j++) {
                    var shiftPerpendicular = facing * j;
                    var tilePosition = startFromPathPosition + shiftPerpendicular;

                    if (City.Data.IsInBounds(tilePosition) && City.Data[tilePosition] == null) {
                        tiles.Add(tilePosition);
                    } else {
                        return;
                    }
                }
            }
            // Calcular los límites del edificio (min/max coordinates)
            var (minX, minY, maxX, maxY) = (int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);
            foreach (var pos in tiles) {
                minX = Math.Min(minX, pos.X);
                minY = Math.Min(minY, pos.Y);
                maxX = Math.Max(maxX, pos.X);
                maxY = Math.Max(maxY, pos.Y);
            }

            // Crear el Rect2I y el edificio
            var buildingRect = new Rect2I(minX, minY, maxX - minX + 1, maxY - minY + 1);
            var building = City.CreateBuilding(path, buildingRect);

            // Determinar las celdas del borde del edificio que están adyacentes al camino
            var potentialPortals = new List<(Vector2I buildingCell, Vector2I pathEntrance)>();

            // Buscar todas las celdas del edificio que están en el borde adyacente al camino
            foreach (var buildingCell in building.GetPositions()) {
                // Comprobar si esta celda está en el borde del edificio adyacente al camino
                var adjacentCell = buildingCell - facing;

                // Verificar si la celda adyacente es parte del camino
                if (City.Data.IsInBounds(adjacentCell) && City.Data[adjacentCell] is Path) {
                    potentialPortals.Add((buildingCell, adjacentCell));
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
                building.PathDirection = pathDirection;
            }
        }
    }
}