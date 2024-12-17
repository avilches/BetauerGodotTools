using System;
using System.Collections.Generic;
using Betauer.Core.DataMath;
using Betauer.Core.PCG.GridMatching;

namespace Betauer.Core.PCG.GridTools;

public static class ConnectDiagonals {
    public static void ConnectBorderDiagonals(Array2D<bool> region) {
        var automata = new CellularAutomata<bool>(region);
        automata.AddNeighborhoodRule(3, neighbors => Diagonals.FindTilePatternId(neighbors));
        automata.SingleUpdate();
    }

    private static readonly Dictionary<string, Func<bool, bool>> DiagonalRules = new() {
        { "·", (v) => v == false },
        { "#", (v) => v == true },
        { "?", (_) => true },
    };

    public static readonly GridMatcherSet<bool, bool> Diagonals =
        new GridMatcherSet<bool, bool> { DefaultRules = DiagonalRules }
            .Add(true, """
                       ? ? ?
                       ? # ?
                       ? ? ?
                       """)
            .Add(true, """
                       · # ?
                       # · ?
                       ? ? ?
                       """)
            .Add(true, """
                       ? # ·
                       ? · #
                       ? ? ?
                       """)
            .Add(true, """
                       ? ? ?
                       ? · #
                       ? # ·
                       """)
            .Add(true, """
                       ? ? ?
                       # · ?
                       · # ?
                       """);
}