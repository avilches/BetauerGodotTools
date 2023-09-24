using System.Collections.Generic;
using Betauer.TestRunner;
using Betauer.TileSet.Terrain;
using NUnit.Framework;

namespace Betauer.GameTools.Tests.TileSet;

[TestRunner.Test]
[Only]
public class TerrainTests : BaseBlobTests {
    [Betauer.TestRunner.Test]
    public void BasicTest() {
        var tileMap = new SingleTerrain(3, 3);
        AreEqual(tileMap.Grid, new[,] {
            { -1, -1, -1 },
            { -1, -1, -1 },
            { -1, -1, -1 },
        });
    }

    [Betauer.TestRunner.Test]
    public void ParseDictionaryTest() {
        var tileMap = SingleTerrain.Parse(@$"
 ***

 P P|
@- @@
", new Dictionary<char, int>() {
            { ' ', (int)SingleTerrain.TileType.None },
            { '@', 0 },
            { 'P', 124 },
            { '*', (int)SingleTerrain.TileType.Auto },
        });
        AreEqual(tileMap.Grid, new[,] {
            { -1, -2, -2, -2, -1 },
            { -1, -1, -1, -1, -1 },
            { -1, 124, -1, 124, 17 },
            { 0, 68, -1, 0, 0 },
        });
    }

    [Betauer.TestRunner.Test]
    public void ParseWithBordersTest() {
        var tileMap = SingleTerrain.Parse(@$"

:   :
 : * :
  :|| :


");
        AreEqual(tileMap.Grid, new[,] {
            { -1, -1, -1 },
            { -1, -2, -1 },
            { 17, 17, -1 },
        });
    }

    [TestRunner.Test]
    public void RulesTest() {
        Assert.That(TerrainRule.Parse(
            """
              1 ? .
              
            !2 X 2
            1 X X
            """).Export(), Is.EqualTo(
            """
             1  ?  .
            !2  X  2
             1  X  X
            """));

        Assert.That(TerrainRule.Parse(
            """
              1 ? . 1 !1
              
            !2 X 2   ? .
            1 X X  .  2
            !2 X 2   ? .
            . . . . .
            
            """).Export(), Is.EqualTo(
            """
             1  ?  .  1 !1
            !2  X  2  ?  .
             1  X  X  .  2
            !2  X  2  ?  .
             .  .  .  .  .
            """));
    }

    [TestRunner.Test]
    public void ParseRulesCheckTest() {
        Assert.That(TerrainRule.Parse("?").Rules[0,0], Is.EqualTo(TerrainRule.NeighbourRuleIgnore.Instance));
        Assert.True(TerrainRule.Parse(".").Rules[0,0] is TerrainRule.NeighbourRuleEquals { TerrainId: -1 });
        Assert.True(TerrainRule.Parse("X").Rules[0,0] is TerrainRule.NeighbourRuleNotEquals { TerrainId: -1 });
        Assert.True(TerrainRule.Parse("2").Rules[0,0] is TerrainRule.NeighbourRuleEquals { TerrainId: 2 });
        Assert.True(TerrainRule.Parse("!2").Rules[0,0] is TerrainRule.NeighbourRuleNotEquals { TerrainId: 2 });

        
        // !2 means anything but 2, even empty is ok
        Assert.True(TerrainRule.Parse("!2").Check(new int[,] { { -1 } }));
        Assert.True(TerrainRule.Parse("!2").Check(new int[,] { { 0 } }));
        Assert.True(TerrainRule.Parse("!2").Check(new int[,] { { 1 } }));
        Assert.False(TerrainRule.Parse("!2").Check(new int[,] { { 2 } }));

        // 2 means only 2, empty is not ok
        Assert.False(TerrainRule.Parse("2").Check(new int[,] { { -1 } }));
        Assert.False(TerrainRule.Parse("2").Check(new int[,] { { 0 } }));
        Assert.False(TerrainRule.Parse("2").Check(new int[,] { { 1 } }));
        Assert.True(TerrainRule.Parse("2").Check(new int[,] { { 2 } }));

        // X means not empty
        Assert.False(TerrainRule.Parse("X").Check(new int[,] { { -1 } }));
        Assert.True(TerrainRule.Parse("X").Check(new int[,] { { 0 } }));
        Assert.True(TerrainRule.Parse("X").Check(new int[,] { { 1 } }));
        Assert.True(TerrainRule.Parse("X").Check(new int[,] { { 2 } }));

        // . means empty
        Assert.True(TerrainRule.Parse(".").Check(new int[,] { { -1 } }));
        Assert.False(TerrainRule.Parse(".").Check(new int[,] { { 0 } }));
        Assert.False(TerrainRule.Parse(".").Check(new int[,] { { 1 } }));
        Assert.False(TerrainRule.Parse(".").Check(new int[,] { { 2 } }));

        // ? means anything, even empty
        Assert.True(TerrainRule.Parse("?").Check(new int[,] { { -1 } }));
        Assert.True(TerrainRule.Parse("?").Check(new int[,] { { 0 } }));
        Assert.True(TerrainRule.Parse("?").Check(new int[,] { { 1 } }));
        Assert.True(TerrainRule.Parse("?").Check(new int[,] { { 2 } }));
    }

    [TestRunner.Test]
    public void RulesCheckTest() {
    }
}
