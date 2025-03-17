using System;
using System.Text;
using Betauer.Core;
using Betauer.Core.DataMath;

namespace Veronenger.Game.Dungeon.GenCity;

public class CityRender(City city) {
    public char EMPTY = ' '; // Espacio vacío

    public char ROAD_H = '═'; // Carretera horizontal
    public char ROAD_V = '║'; // Carretera vertical

    public char CROSS_H = '-'; // Intersección con carretera delante y detrás
    public char CROSS_V = '|'; // Intersección con carretera arriba y abajo

    public char T_RIGHT = '╠';
    public char T_LEFT = '╣';
    public char T_UP = '╩';
    public char T_DOWN = '╦';

    public char CROSS = '╬'; // Intersección

    public char TURN_NE = '╚'; // Giro noreste
    public char TURN_NW = '╝'; // Giro noroeste
    public char TURN_SE = '╔'; // Giro sureste
    public char TURN_SW = '╗'; // Giro suroeste
    public char DEAD_END_R = '╞'; // Fin de carretera right
    public char DEAD_END_L = '╡'; // Fin de carretera left
    public char DEAD_END_T = '╨'; // Fin de carretera top
    public char DEAD_END_B = '╥'; // Fin de carretera bottom

    public Array2D<char> AsciiMap { get; } = new(city.Width, city.Height);

    public City City { get; } = city;

    public void Render() {
        ClearRender();
        RenderRoads();
        RenderIntersections();
    }

    public void ClearRender() {
        AsciiMap.Fill(EMPTY);
    }

    private void RenderRoads() {
        foreach (var path in City.GetAllPaths()) {
            var isHorizontal = path.Direction.IsHorizontal();
            var roadChar = isHorizontal ? ROAD_H : ROAD_V;
            foreach (var position in path.GetPathOnlyPositions()) {
                AsciiMap[position] = roadChar;
            }
        }
    }

    private void RenderIntersections() {
        foreach (var intersection in City.Intersections) {
            var hasNorth = intersection.Up != null;
            var hasSouth = intersection.Down != null;
            var hasEast = intersection.Right != null;
            var hasWest = intersection.Left != null;
            var intersectionChar = DetermineIntersectionChar(hasNorth, hasSouth, hasEast, hasWest);
            AsciiMap[intersection.Position] = intersectionChar;
        }

        foreach (var (p, e) in City.Data.GetIndexedValues()) {
            if (e is Other other) {
                AsciiMap[p] = other.C;
            }
        }
    }

    private char DetermineIntersectionChar(bool hasNorth, bool hasSouth, bool hasEast, bool hasWest) {
        // 4 paths: Intersección completa
        if (hasNorth && hasSouth && hasEast && hasWest) return CROSS;

        // 3 paths: Intersecciones en T
        if (hasNorth && hasSouth && hasEast) return T_RIGHT; // ╠
        if (hasNorth && hasSouth && hasWest) return T_LEFT; // ╣
        if (hasNorth && hasEast && hasWest) return T_UP; // ╩
        if (hasSouth && hasEast && hasWest) return T_DOWN; // ╦

        // 2 paths: Calles rectas
        if (hasNorth && hasSouth) {
            return CROSS_V;
        }
        if (hasEast && hasWest) {
            return CROSS_H;
        }

        // 2 paths: Giros
        if (hasNorth && hasEast) return TURN_NE; // ╚
        if (hasNorth && hasWest) return TURN_NW; // ╝
        if (hasSouth && hasEast) return TURN_SE; // ╔
        if (hasSouth && hasWest) return TURN_SW; // ╗

        // 1 path: dead ends
        if (hasNorth) return DEAD_END_T; // Fin de carretera arriba
        if (hasSouth) return DEAD_END_B; // Fin de carretera abajo
        if (hasEast) return DEAD_END_R; // Fin de carretera derecha
        if (hasWest) return DEAD_END_L; // Fin de carretera izquierda
        throw new Exception("How can be this possible?");
    }

    public override string ToString() {
        Render();
        StringBuilder buffer = new StringBuilder();
        for (var y = 0; y < City.Height; y++) {
            for (var x = 0; x < City.Width; x++) {
                buffer.Append(AsciiMap[y, x]);
            }
            if (y < City.Height - 1) buffer.AppendLine();
        }
        return buffer.ToString();
    }
}