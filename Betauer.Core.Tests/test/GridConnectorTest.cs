using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Betauer.Core.DataMath;
using Betauer.Core.DataMath.Maze;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestRunner.Test]
public class GridConnectorTest {
    [TestRunner.Test]
    public void Test() {
        var xYGrid = Array2D.Parse("""
                                   #·#####
                                   ···##··  
                                   ###····
                                   ·##·###
                                   ##·#··#
                                   #······
                                   ·······    
                                   """, new Dictionary<char, bool> {
            { '#', true }, { '·', false },
        });

        var connector = new GridConnector(xYGrid);
        Assert.That(PrintState(connector), Is.EqualTo("""
                                                      1·22222
                                                      ···22··
                                                      333····
                                                      ·33·444
                                                      33·5··4
                                                      3·i·ii·
                                                      ·iiiiii
                                                      """));

        Assert.That(connector.GetRegions(), Is.EqualTo(5));
        CollectionAssert.AreEquivalent(connector.GetRegionsIds(), new[] { 1, 2, 3, 4, 5 });

        CollectionAssert.AreEquivalent(connector.GetRegionCells(1), new[] { (0, 0) }.Select(t => new Vector2I(t.Item1, t.Item2)));
        CollectionAssert.AreEquivalent(connector.GetRegionCells(2), new[] { (2, 0), (3, 0), (4, 0), (3, 1), (5, 0), (4, 1), (6, 0) }.Select(t => new Vector2I(t.Item1, t.Item2)));
        CollectionAssert.AreEquivalent(connector.GetRegionCells(3), new[] { (0, 2), (1, 2), (2, 2), (1, 3), (2, 3), (1, 4), (0, 4), (0, 5) }.Select(t => new Vector2I(t.Item1, t.Item2)));
        CollectionAssert.AreEquivalent(connector.GetRegionCells(4), new[] { (4, 3), (5, 3), (6, 3), (6, 4) }.Select(t => new Vector2I(t.Item1, t.Item2)));
        CollectionAssert.AreEquivalent(connector.GetRegionCells(5), new[] { (3, 4) }.Select(t => new Vector2I(t.Item1, t.Item2)));
        CollectionAssert.AreEquivalent(connector.GetRegionCells(0), new[] {
            (1, 0), (0, 1), (1, 1), (2, 1), (5, 1), (6, 1), (3, 2), (4, 2), (5, 2), (6, 2), (0, 3), (3, 3), (2, 4), (4, 4), (5, 4), (1, 5), (2, 5), (3, 5), (4, 5), (5, 5), (6, 5), (0, 6), (1, 6), (2, 6), (3, 6), (4, 6), (5, 6), (6, 6)
        }.Select(t => new Vector2I(t.Item1, t.Item2)));

        var connectingCells = connector.FindConnectingCells();

        CollectionAssert.AreEquivalent(connectingCells.Isolated, new[] { (2, 5), (4, 5), (5, 5), (1, 6), (2, 6), (3, 6), (4, 6), (5, 6), (6, 6) }.Select(t => new Vector2I(t.Item1, t.Item2)));
        
        
        CollectionAssert.AreEquivalent(connectingCells.ConnectingCells[new Vector2I(1, 0)],new[] {  1, 2 } );
        CollectionAssert.AreEquivalent(connectingCells.ConnectingCells[new Vector2I(0, 1)],new[] { 1, 3 } );
        CollectionAssert.AreEquivalent(connectingCells.ConnectingCells[new Vector2I(2, 1)],new[] { 2, 3 } );
        CollectionAssert.AreEquivalent(connectingCells.ConnectingCells[new Vector2I(3, 2)],new[] { 3, 2 } );
        CollectionAssert.AreEquivalent(connectingCells.ConnectingCells[new Vector2I(4, 2)],new[] { 2, 4 } );
        CollectionAssert.AreEquivalent(connectingCells.ConnectingCells[new Vector2I(3, 3)],new[] { 3, 4, 5 } );
        CollectionAssert.AreEquivalent(connectingCells.ConnectingCells[new Vector2I(2, 4)],new[] { 3, 5 } );
        CollectionAssert.AreEquivalent(connectingCells.ConnectingCells[new Vector2I(4, 4)],new[] { 5, 4 } );

        
        Console.WriteLine("Marking 0,0, ignore");
        connector.ToggleCell(new Vector2I(0, 0), true);
        Console.WriteLine("Marking 1,1, expand region 3 without change");
        connector.ToggleCell(new Vector2I(1, 1), true);
        Assert.That(PrintState(connector), Is.EqualTo("""
                                                      1·22222
                                                      ·3·22··
                                                      333····
                                                      ·33·444
                                                      33·5··4
                                                      3·i·ii·
                                                      ·iiiiii
                                                      """));
        Console.WriteLine("Marking 0,1, join regions 3 and 1");
        connector.ToggleCell(new Vector2I(0, 1), true);
        Assert.That(PrintState(connector), Is.EqualTo("""
                                                      3·22222
                                                      33·22··
                                                      333····
                                                      ·33·444
                                                      33·5··4
                                                      3·i·ii·
                                                      ·iiiiii
                                                      """));
        Console.WriteLine("Marking 1,0, join regions 3 and 2");
        connector.ToggleCell(new Vector2I(1, 0), true);
        Assert.That(connector.GetRegions(), Is.EqualTo(3));
        CollectionAssert.AreEquivalent(connector.GetRegionsIds(), new[] { 3, 4, 5 });

        Assert.That(PrintState(connector), Is.EqualTo("""
                                                      3333333
                                                      33·33··
                                                      333····
                                                      ·33·444
                                                      33·5··4
                                                      3·i·ii·
                                                      ·iiiiii
                                                      """));
        Console.WriteLine("Marking 1,6 and 2,6, create an independent region 1, 2");
        connector.ToggleCell(new Vector2I(1, 6), true);
        connector.ToggleCell(new Vector2I(2, 6), true);
        connector.ToggleCell(new Vector2I(6, 6), true);
        Assert.That(connector.GetRegions(), Is.EqualTo(5));
        CollectionAssert.AreEquivalent(connector.GetRegionsIds(), new[] { 1, 2, 3, 4, 5 });
        Assert.That(PrintState(connector), Is.EqualTo("""
                                                      3333333
                                                      33·33··
                                                      333····
                                                      ·33·444
                                                      33·5··4
                                                      3···ii·
                                                      ·11·i·2
                                                      """));

        Console.WriteLine("Unmark 0,2, unjoin regions");
        connector.ToggleCell(new Vector2I(1, 1), false);

        Assert.That(PrintState(connector), Is.EqualTo("""
                                                      1111111
                                                      1··11··
                                                      111····
                                                      ·11·222
                                                      11·3··2
                                                      1···ii·
                                                      ·44·i·5
                                                      """));

        connector.ToggleCell(new Vector2I(0, 1), false);
        connector.ToggleCell(new Vector2I(1, 0), false);
        connector.ToggleCell(new Vector2I(3, 3), false);
        Assert.That(PrintState(connector), Is.EqualTo("""
                                                      1·22222
                                                      ···22··
                                                      333····
                                                      ·33·444
                                                      33·5··4
                                                      3···ii·
                                                      ·66·i·7
                                                      """));

    }

    private static string PrintState(GridConnector connector) {
        var connectingCells = connector.FindConnectingCells();
        Console.WriteLine("Total Regions: " + connector.GetRegions() + ", Ids: " + string.Join(", ", connector.GetRegionsIds()));

        foreach (var id in connector.GetRegionsIds()) {
            Console.WriteLine($"Region {id}: {string.Join(", ", connector.GetRegionCells(id))}");
        }
        Console.WriteLine($"No region (0): {string.Join(", ", connector.GetRegionCells(0))}");

        Console.WriteLine("Isolated: " + string.Join(", ", connectingCells.Isolated));
        foreach (var (cell, regions) in connectingCells.ConnectingCells) {
            Console.WriteLine($"Cell ({cell.X}, {cell.Y}) connects regions: {string.Join(", ", regions)}");
        }
        var s = new StringBuilder();
        foreach (var cell in connector.Labels) {
            if (cell.Value == 0) {
                if (connectingCells.Isolated.Contains(cell.Position)) {
                    s.Append('i');
                } else {
                    s.Append('·');
                }
            } else {
                s.Append(cell.Value.ToString("x8").AsSpan(7, 1));
            }
            if (cell.Position.X == connector.Width - 1) {
                s.Append('\n');
            }
        }
        Console.WriteLine(s);
        return s.ToString().Trim();
    }
}