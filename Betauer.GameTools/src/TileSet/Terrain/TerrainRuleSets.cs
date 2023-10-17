using System.Collections.Generic;

namespace Betauer.TileSet.Terrain;

public static class TerrainRuleSets {
    public static readonly TerrainRuleSet Blob47Rules = new TerrainRuleSet(new List<TerrainRule> {
        TerrainRule.Parse(0, """
                             ? ! ?
                             ! # !
                             ? ! ?
                             """),
        TerrainRule.Parse(1, """
                             ? # ?
                             ! # !
                             ? ! ?
                             """),
        TerrainRule.Parse(4, """
                             ? ! ?
                             ! # #
                             ? ! ?
                             """),
        TerrainRule.Parse(5, """
                             ? # !
                             ! # #
                             ? ! ?
                             """),
        TerrainRule.Parse(7, """
                             ? # #
                             ! # #
                             ? ! ?
                             """),
        TerrainRule.Parse(16, """
                              ? ! ?
                              ! # !
                              ? # ?
                              """),
        TerrainRule.Parse(17, """
                              ? # ?
                              ! # !
                              ? # ?
                              """),
        TerrainRule.Parse(20, """
                              ? ! ?
                              ! # #
                              ? # !
                              """),
        TerrainRule.Parse(21, """
                              ? # !
                              ! # #
                              ? # !
                              """),
        TerrainRule.Parse(23, """
                              ? # #
                              ! # #
                              ? # !
                              """),
        TerrainRule.Parse(28, """
                              ? ! ?
                              ! # #
                              ? # #
                              """),
        TerrainRule.Parse(29, """
                              ? # !
                              ! # #
                              ? # #
                              """),
        TerrainRule.Parse(31, """
                              ? # #
                              ! # #
                              ? # #
                              """),
        TerrainRule.Parse(64, """
                              ? ! ?
                              # # !
                              ? ! ?
                              """),
        TerrainRule.Parse(64, """
                              ? ! ?
                              # # !
                              ? ! ?
                              """),
        TerrainRule.Parse(65, """
                              ! # ?
                              # # !
                              ? ! ?
                              """),
        TerrainRule.Parse(68, """
                              ? ! ?
                              # # #
                              ? ! ?
                              """),
        TerrainRule.Parse(69, """
                              ! # !
                              # # #
                              ? ! ?
                              """),
        TerrainRule.Parse(71, """
                              ! # #
                              # # #
                              ? ! ?
                              """),
        TerrainRule.Parse(80, """
                              ? ! ?
                              # # !
                              ! # ?
                              """),
        TerrainRule.Parse(81, """
                              ! # ?
                              # # !
                              ! # ?
                              """),
        TerrainRule.Parse(84, """
                              ? ! ?
                              # # #
                              ! # !
                              """),
        TerrainRule.Parse(85, """
                              ! # !
                              # # #
                              ! # !
                              """),
        TerrainRule.Parse(87, """
                              ! # #
                              # # #
                              ! # !
                              """),
        TerrainRule.Parse(92, """
                              ? ! ?
                              # # #
                              ! # #
                              """),
        TerrainRule.Parse(93, """
                              ! # !
                              # # #
                              ! # #
                              """),
        TerrainRule.Parse(95, """
                              ! # #
                              # # #
                              ! # #
                              """),
        TerrainRule.Parse(112, """
                               ? ! ?
                               # # !
                               # # ?
                               """),
        TerrainRule.Parse(113, """
                               ! # ?
                               # # !
                               # # ?
                               """),
        TerrainRule.Parse(116, """
                               ? ! ?
                               # # #
                               # # !
                               """),
        TerrainRule.Parse(117, """
                               ! # !
                               # # #
                               # # !
                               """),
        TerrainRule.Parse(119, """
                               ! # #
                               # # #
                               # # !
                               """),
        TerrainRule.Parse(124, """
                               ? ! ?
                               # # #
                               # # #
                               """),
        TerrainRule.Parse(125, """
                               ! # !
                               # # #
                               # # #
                               """),
        TerrainRule.Parse(127, """
                               ! # #
                               # # #
                               # # #
                               """),
        TerrainRule.Parse(193, """
                               # # ?
                               # # !
                               ? ! ?
                               """),
        TerrainRule.Parse(197, """
                               # # !
                               # # #
                               ? ! ?
                               """),
        TerrainRule.Parse(199, """
                               # # #
                               # # #
                               ? ! ?
                               """),
        TerrainRule.Parse(209, """
                               # # ?
                               # # !
                               ! # ?
                               """),
        TerrainRule.Parse(213, """
                               # # !
                               # # #
                               ! # !
                               """),
        TerrainRule.Parse(215, """
                               # # #
                               # # #
                               ! # !
                               """),
        TerrainRule.Parse(221, """
                               # # !
                               # # #
                               ! # #
                               """),
        TerrainRule.Parse(223, """
                               # # #
                               # # #
                               ! # #
                               """),
        TerrainRule.Parse(241, """
                               # # ?
                               # # !
                               # # ?
                               """),
        TerrainRule.Parse(241, """
                               # # ?
                               # # !
                               # # ?
                               """),
        TerrainRule.Parse(245, """
                               # # !
                               # # #
                               # # !
                               """),
        TerrainRule.Parse(247, """
                               # # #
                               # # #
                               # # !
                               """),
        TerrainRule.Parse(253, """
                               # # !
                               # # #
                               # # #
                               """),
        TerrainRule.Parse(255, """
                               # # #
                               # # #
                               # # #
                               """),
    });
}