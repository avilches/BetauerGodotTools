using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Veronenger.Game.Dungeon.GenCity;

public partial class CityGenerator {
    public void GenerateBuildings() {
        var random = new Random(Options.Seed);
        foreach (var path in City.GetAllPaths()) {
            var nextPos = path.Start.Position + path.Direction;

            foreach (var side in BothTurnDirections(path.Direction)) {
                // Validación: comprobar si hay espacio suficiente a lo largo del camino
                if (!SidewalkIsEmpty(path, side)) continue;
                var stepOffset = Options.BuildingSidewalk;

                while (stepOffset < path.GetLength()) {
                    var stepShift = path.Direction * stepOffset;
                    var shiftFromPath = side * (Options.BuildingSidewalk + 1);
                    var startPosition = nextPos + stepShift + shiftFromPath;

                    var buildingWidth = random.Next(Options.BuildingMinSize, Options.BuildingMaxSize + 1);
                    var buildingHeight = random.Next(Options.BuildingMinSize, Options.BuildingMaxSize + 1);

                    if (stepOffset + buildingWidth + Options.BuildingSidewalk > path.GetLength()) {
                        break;
                    }
                    ProcessingBuilding(path, startPosition, buildingWidth, buildingHeight, side);

                    var spaceBetweenBuildings = random.Next(Options.BuildingMinSpace, Options.BuildingMaxSpace + 1);
                    stepOffset += buildingWidth + spaceBetweenBuildings;
                }
            }
        }
        return;

        bool SidewalkIsEmpty(Path path, Vector2I side) {
            if (Options.BuildingSidewalk == 0) return true;
            foreach (var pathPos in path.GetPathOnlyPositions()) {
                var checkPos = pathPos;
                for (var i = 1; i <= Options.BuildingSidewalk; i++) {
                    checkPos += side;
                    if (!City.Data.IsInBounds(checkPos) || City.Data[checkPos] != null) {
                        return false;
                    }
                }
            }
            return true;
        }

        Building? ProcessingBuilding(Path path, Vector2I position, int buildingWidth, int buildingHeight, Vector2I facing) {
            var tiles = new List<Vector2I>();

            // Primero recopilamos todas las posiciones para verificar que el edificio se puede colocar
            for (var i = 0; i < buildingWidth; i++) {
                var shiftParallel = path.Direction * i;
                var startFromPathPosition = position + shiftParallel;

                for (var j = 0; j < buildingHeight; j++) {
                    var shiftPerpendicular = facing * j;
                    var tilePosition = startFromPathPosition + shiftPerpendicular;

                    if (City.Data.IsInBounds(tilePosition) && City.Data[tilePosition] == null) {
                        tiles.Add(tilePosition);
                    } else {
                        return null;
                    }
                }
            }
            var minX = tiles.Min(pos => pos.X);
            var minY = tiles.Min(pos => pos.Y);
            var maxX = tiles.Max(pos => pos.X);
            var maxY = tiles.Max(pos => pos.Y);
            
            var buildingRect = new Rect2I(minX, minY, maxX - minX + 1, maxY - minY + 1);
            var building = City.CreateBuilding(path, buildingRect);

            // Determinar las celdas del borde del edificio que están adyacentes al camino
            var potentialPortals = new List<(Vector2I buildingCell, Vector2I pathEntrance)>();

            // Buscar todas las celdas del edificio que están en el borde adyacente al camino
            // Iterar sobre las posiciones del camino para encontrar celdas del edificio adyacentes
            var pathPositions = path.GetPathOnlyPositions();
            foreach (var pathPos in pathPositions) {
                var buildingCell = pathPos + (facing * (Options.BuildingSidewalk + 1));
                
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