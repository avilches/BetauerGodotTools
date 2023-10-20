using System.Collections.Generic;

namespace Betauer.TileSet.Terrain;

public static class TilePatternRuleSets {
    public static readonly TemplateTilePatternRuleSet<int> Blob47Rules = new (new List<(int, TilePattern)> {
        (0, TilePattern.Parse("""
                              ? ! ?
                              ! # !
                              ? ! ?
                              """)),
        (1, TilePattern.Parse("""
                              ? # ?
                              ! # !
                              ? ! ?
                              """)),
        (4, TilePattern.Parse("""
                              ? ! ?
                              ! # #
                              ? ! ?
                              """)),
        (5, TilePattern.Parse("""
                              ? # !
                              ! # #
                              ? ! ?
                              """)),
        (7, TilePattern.Parse("""
                              ? # #
                              ! # #
                              ? ! ?
                              """)),
        (16, TilePattern.Parse("""
                               ? ! ?
                               ! # !
                               ? # ?
                               """)),
        (17, TilePattern.Parse("""
                               ? # ?
                               ! # !
                               ? # ?
                               """)),
        (20, TilePattern.Parse("""
                               ? ! ?
                               ! # #
                               ? # !
                               """)),
        (21, TilePattern.Parse("""
                               ? # !
                               ! # #
                               ? # !
                               """)),
        (23, TilePattern.Parse("""
                               ? # #
                               ! # #
                               ? # !
                               """)),
        (28, TilePattern.Parse("""
                               ? ! ?
                               ! # #
                               ? # #
                               """)),
        (29, TilePattern.Parse("""
                               ? # !
                               ! # #
                               ? # #
                               """)),
        (31, TilePattern.Parse("""
                               ? # #
                               ! # #
                               ? # #
                               """)),
        (64, TilePattern.Parse("""
                               ? ! ?
                               # # !
                               ? ! ?
                               """)),
        (64, TilePattern.Parse("""
                               ? ! ?
                               # # !
                               ? ! ?
                               """)),
        (65, TilePattern.Parse("""
                               ! # ?
                               # # !
                               ? ! ?
                               """)),
        (68, TilePattern.Parse("""
                               ? ! ?
                               # # #
                               ? ! ?
                               """)),
        (69, TilePattern.Parse("""
                               ! # !
                               # # #
                               ? ! ?
                               """)),
        (71, TilePattern.Parse("""
                               ! # #
                               # # #
                               ? ! ?
                               """)),
        (80, TilePattern.Parse("""
                               ? ! ?
                               # # !
                               ! # ?
                               """)),
        (81, TilePattern.Parse("""
                               ! # ?
                               # # !
                               ! # ?
                               """)),
        (84, TilePattern.Parse("""
                               ? ! ?
                               # # #
                               ! # !
                               """)),
        (85, TilePattern.Parse("""
                               ! # !
                               # # #
                               ! # !
                               """)),
        (87, TilePattern.Parse("""
                               ! # #
                               # # #
                               ! # !
                               """)),
        (92, TilePattern.Parse("""
                               ? ! ?
                               # # #
                               ! # #
                               """)),
        (93, TilePattern.Parse("""
                               ! # !
                               # # #
                               ! # #
                               """)),
        (95, TilePattern.Parse("""
                               ! # #
                               # # #
                               ! # #
                               """)),
        (112, TilePattern.Parse("""
                                ? ! ?
                                # # !
                                # # ?
                                """)),
        (113, TilePattern.Parse("""
                                ! # ?
                                # # !
                                # # ?
                                """)),
        (116, TilePattern.Parse("""
                                ? ! ?
                                # # #
                                # # !
                                """)),
        (117, TilePattern.Parse("""
                                ! # !
                                # # #
                                # # !
                                """)),
        (119, TilePattern.Parse("""
                                ! # #
                                # # #
                                # # !
                                """)),
        (124, TilePattern.Parse("""
                                ? ! ?
                                # # #
                                # # #
                                """)),
        (125, TilePattern.Parse("""
                                ! # !
                                # # #
                                # # #
                                """)),
        (127, TilePattern.Parse("""
                                ! # #
                                # # #
                                # # #
                                """)),
        (193, TilePattern.Parse("""
                                # # ?
                                # # !
                                ? ! ?
                                """)),
        (197, TilePattern.Parse("""
                                # # !
                                # # #
                                ? ! ?
                                """)),
        (199, TilePattern.Parse("""
                                # # #
                                # # #
                                ? ! ?
                                """)),
        (209, TilePattern.Parse("""
                                # # ?
                                # # !
                                ! # ?
                                """)),
        (213, TilePattern.Parse("""
                                # # !
                                # # #
                                ! # !
                                """)),
        (215, TilePattern.Parse("""
                                # # #
                                # # #
                                ! # !
                                """)),
        (221, TilePattern.Parse("""
                                # # !
                                # # #
                                ! # #
                                """)),
        (223, TilePattern.Parse("""
                                # # #
                                # # #
                                ! # #
                                """)),
        (241, TilePattern.Parse("""
                                # # ?
                                # # !
                                # # ?
                                """)),
        (241, TilePattern.Parse("""
                                # # ?
                                # # !
                                # # ?
                                """)),
        (245, TilePattern.Parse("""
                                # # !
                                # # #
                                # # !
                                """)),
        (247, TilePattern.Parse("""
                                # # #
                                # # #
                                # # !
                                """)),
        (253, TilePattern.Parse("""
                                # # !
                                # # #
                                # # #
                                """)),
        (255, TilePattern.Parse("""
                                # # #
                                # # #
                                # # #
                                """)),
    });
}