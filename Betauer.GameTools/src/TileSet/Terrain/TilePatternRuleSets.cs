using System.Collections.Generic;

namespace Betauer.TileSet.Terrain;

public static class TilePatternRuleSets {
    public static Dictionary<string, NeighborRule> TilePatternRules = new() {
        { "!", NeighborRule.CreateEqualsTo(-1) },
        { "#", NeighborRule.Equals0 },
    };
    public static readonly TilePatternSet<int> Blob47 = new (new List<(int, TilePattern)> {
        (0, TilePattern.Parse("""
                              ? ! ?
                              ! # !
                              ? ! ?
                              """, TilePatternRules)),
        (1, TilePattern.Parse("""
                              ? # ?
                              ! # !
                              ? ! ?
                              """, TilePatternRules)),
        (4, TilePattern.Parse("""
                              ? ! ?
                              ! # #
                              ? ! ?
                              """, TilePatternRules)),
        (5, TilePattern.Parse("""
                              ? # !
                              ! # #
                              ? ! ?
                              """, TilePatternRules)),
        (7, TilePattern.Parse("""
                              ? # #
                              ! # #
                              ? ! ?
                              """, TilePatternRules)),
        (16, TilePattern.Parse("""
                               ? ! ?
                               ! # !
                               ? # ?
                               """, TilePatternRules)),
        (17, TilePattern.Parse("""
                               ? # ?
                               ! # !
                               ? # ?
                               """, TilePatternRules)),
        (20, TilePattern.Parse("""
                               ? ! ?
                               ! # #
                               ? # !
                               """, TilePatternRules)),
        (21, TilePattern.Parse("""
                               ? # !
                               ! # #
                               ? # !
                               """, TilePatternRules)),
        (23, TilePattern.Parse("""
                               ? # #
                               ! # #
                               ? # !
                               """, TilePatternRules)),
        (28, TilePattern.Parse("""
                               ? ! ?
                               ! # #
                               ? # #
                               """, TilePatternRules)),
        (29, TilePattern.Parse("""
                               ? # !
                               ! # #
                               ? # #
                               """, TilePatternRules)),
        (31, TilePattern.Parse("""
                               ? # #
                               ! # #
                               ? # #
                               """, TilePatternRules)),
        (64, TilePattern.Parse("""
                               ? ! ?
                               # # !
                               ? ! ?
                               """, TilePatternRules)),
        (64, TilePattern.Parse("""
                               ? ! ?
                               # # !
                               ? ! ?
                               """, TilePatternRules)),
        (65, TilePattern.Parse("""
                               ! # ?
                               # # !
                               ? ! ?
                               """, TilePatternRules)),
        (68, TilePattern.Parse("""
                               ? ! ?
                               # # #
                               ? ! ?
                               """, TilePatternRules)),
        (69, TilePattern.Parse("""
                               ! # !
                               # # #
                               ? ! ?
                               """, TilePatternRules)),
        (71, TilePattern.Parse("""
                               ! # #
                               # # #
                               ? ! ?
                               """, TilePatternRules)),
        (80, TilePattern.Parse("""
                               ? ! ?
                               # # !
                               ! # ?
                               """, TilePatternRules)),
        (81, TilePattern.Parse("""
                               ! # ?
                               # # !
                               ! # ?
                               """, TilePatternRules)),
        (84, TilePattern.Parse("""
                               ? ! ?
                               # # #
                               ! # !
                               """, TilePatternRules)),
        (85, TilePattern.Parse("""
                               ! # !
                               # # #
                               ! # !
                               """, TilePatternRules)),
        (87, TilePattern.Parse("""
                               ! # #
                               # # #
                               ! # !
                               """, TilePatternRules)),
        (92, TilePattern.Parse("""
                               ? ! ?
                               # # #
                               ! # #
                               """, TilePatternRules)),
        (93, TilePattern.Parse("""
                               ! # !
                               # # #
                               ! # #
                               """, TilePatternRules)),
        (95, TilePattern.Parse("""
                               ! # #
                               # # #
                               ! # #
                               """, TilePatternRules)),
        (112, TilePattern.Parse("""
                                ? ! ?
                                # # !
                                # # ?
                                """, TilePatternRules)),
        (113, TilePattern.Parse("""
                                ! # ?
                                # # !
                                # # ?
                                """, TilePatternRules)),
        (116, TilePattern.Parse("""
                                ? ! ?
                                # # #
                                # # !
                                """, TilePatternRules)),
        (117, TilePattern.Parse("""
                                ! # !
                                # # #
                                # # !
                                """, TilePatternRules)),
        (119, TilePattern.Parse("""
                                ! # #
                                # # #
                                # # !
                                """, TilePatternRules)),
        (124, TilePattern.Parse("""
                                ? ! ?
                                # # #
                                # # #
                                """, TilePatternRules)),
        (125, TilePattern.Parse("""
                                ! # !
                                # # #
                                # # #
                                """, TilePatternRules)),
        (127, TilePattern.Parse("""
                                ! # #
                                # # #
                                # # #
                                """, TilePatternRules)),
        (193, TilePattern.Parse("""
                                # # ?
                                # # !
                                ? ! ?
                                """, TilePatternRules)),
        (197, TilePattern.Parse("""
                                # # !
                                # # #
                                ? ! ?
                                """, TilePatternRules)),
        (199, TilePattern.Parse("""
                                # # #
                                # # #
                                ? ! ?
                                """, TilePatternRules)),
        (209, TilePattern.Parse("""
                                # # ?
                                # # !
                                ! # ?
                                """, TilePatternRules)),
        (213, TilePattern.Parse("""
                                # # !
                                # # #
                                ! # !
                                """, TilePatternRules)),
        (215, TilePattern.Parse("""
                                # # #
                                # # #
                                ! # !
                                """, TilePatternRules)),
        (221, TilePattern.Parse("""
                                # # !
                                # # #
                                ! # #
                                """, TilePatternRules)),
        (223, TilePattern.Parse("""
                                # # #
                                # # #
                                ! # #
                                """, TilePatternRules)),
        (241, TilePattern.Parse("""
                                # # ?
                                # # !
                                # # ?
                                """, TilePatternRules)),
        (241, TilePattern.Parse("""
                                # # ?
                                # # !
                                # # ?
                                """, TilePatternRules)),
        (245, TilePattern.Parse("""
                                # # !
                                # # #
                                # # !
                                """, TilePatternRules)),
        (247, TilePattern.Parse("""
                                # # #
                                # # #
                                # # !
                                """, TilePatternRules)),
        (253, TilePattern.Parse("""
                                # # !
                                # # #
                                # # #
                                """, TilePatternRules)),
        (255, TilePattern.Parse("""
                                # # #
                                # # #
                                # # #
                                """, TilePatternRules)),
    });
}