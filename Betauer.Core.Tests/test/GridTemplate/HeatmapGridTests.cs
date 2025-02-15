using System;
using Betauer.Core.PCG.GridTools;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Core.Tests.GridTemplate;

[TestFixture]
public class HeatmapGridTests {
    private GridGraph _graph;
    private HeatmapGrid _heatmap;

    [SetUp]
    public void Setup() {
        // Create a 5x5 grid where all cells are walkable
        _graph = new GridGraph(5, 5, _ => false);
        _heatmap = new HeatmapGrid(_graph);
    }

    [Test]
    public void SingleHeatSource_ShouldPropagateHeat() {
        var source = new Vector2I(2, 2);
        _heatmap.AddHeatSource(source, 1.0f, 3.0f); // Radio de 3 para cubrir buena parte de la grid 5x5
        _heatmap.UpdateHeatmap();

        // Check source position has maximum heat
        Assert.That(_heatmap.GetHeat(source), Is.GreaterThan(0.9f));

        // Check adjacent cells have less heat
        var adjacentHeat = _heatmap.GetHeat(new Vector2I(2, 1));
        Assert.That(adjacentHeat, Is.LessThan(_heatmap.GetHeat(source)));
        Assert.That(adjacentHeat, Is.GreaterThan(0));
    }

    [Test]
    public void MultipleHeatSources_ShouldCombineHeat() {
        var source1 = new Vector2I(1, 1);
        var source2 = new Vector2I(3, 3);
        var middlePoint = new Vector2I(2, 2);

        _heatmap.AddHeatSource(source1, 1.0f, 2.0f); // Radio suficiente para llegar al punto medio
        _heatmap.AddHeatSource(source2, 1.0f, 2.0f);
        _heatmap.UpdateHeatmap();

        // Middle point should have heat from both sources
        var middleHeat = _heatmap.GetHeat(middlePoint);
        var source1Heat = _heatmap.GetHeat(source1);
        var source2Heat = _heatmap.GetHeat(source2);

        Assert.That(middleHeat, Is.GreaterThan(0));
        Assert.That(middleHeat, Is.LessThan(source1Heat + source2Heat));
    }

    [Test]
    public void GetBestDirection_ShouldPointTowardHeatSource() {
        var source = new Vector2I(4, 4);
        var startPos = new Vector2I(0, 0);

        _heatmap.AddHeatSource(source, 1.0f, 6.0f); // Radio grande para cubrir toda la distancia
        _heatmap.UpdateHeatmap();

        var direction = _heatmap.GetBestDirection(startPos);
        Assert.That(direction, Is.Not.Null);

        // Best direction should be diagonal or orthogonal towards source
        var diff = direction!.Value - startPos;
        Assert.That(diff.X >= 0 && diff.Y >= 0); // Should move towards source (up and right)
    }

    [Test]
    public void GetHeat_OutOfBounds_ShouldReturnZero() {
        var outOfBounds = new Vector2I(10, 10);
        _heatmap.AddHeatSource(new Vector2I(2, 2), 1.0f, 2.0f);
        _heatmap.UpdateHeatmap();

        Assert.That(_heatmap.GetHeat(outOfBounds), Is.EqualTo(0));
    }

    [Test]
    public void HeatIntensity_ShouldAffectPropagation() {
        var source = new Vector2I(2, 2);
        var checkPoint = new Vector2I(3, 3);

        // Test with low intensity
        _heatmap.AddHeatSource(source, 0.5f, 2.0f); // Radio suficiente para llegar al punto de control
        _heatmap.UpdateHeatmap();
        var lowHeat = _heatmap.GetHeat(checkPoint);

        // Clear and test with high intensity
        _heatmap = new HeatmapGrid(_graph);
        _heatmap.AddHeatSource(source, 2.0f, 2.0f);
        _heatmap.UpdateHeatmap();
        var highHeat = _heatmap.GetHeat(checkPoint);

        Assert.That(highHeat, Is.GreaterThan(lowHeat));
    }

    [Test]
    public void PrintHeatmap_ShouldShowHeatDistribution() {
        // Create a larger grid for better visualization
        _graph = new GridGraph(30, 30, _ => false);
        _heatmap = new HeatmapGrid(_graph);

        // Add two heat sources with larger radius
        _heatmap.AddHeatSource(new Vector2I(3, 3), 1.0f, 5.0f); // Moved from (1,1) and increased radius
        _heatmap.AddHeatSource(new Vector2I(11, 11), 2.0f, 5.0f); // Moved from (5,5) and increased radius
        _heatmap.AddHeatSource(new Vector2I(16, 9), 2.0f, 8.0f); // Moved from (5,5) and increased radius
        _heatmap.UpdateHeatmap();


        // PORQUE SON ROMBOS?
        //     CONTROLAN COLISIONES (es decir, el calor de cada celda tiene en cuenta la distancia a
        // la fuente de calor evitando los pasos bloqueados? por ejemplo, una celda contigua con una separacion,
        //     linealmente esta cerca, pero si hay una pared en medio, deberia tener menos calro poq el camino es mas largo

        // Print and verify the heatmap
        var heatmapString = _heatmap.PrintHeatmap();
        Console.WriteLine("Heatmap visualization:");
        Console.WriteLine(heatmapString);

        // Basic verification that the string contains numbers and proper formatting
        Assert.That(heatmapString, Does.Contain("\n"));
        Assert.That(heatmapString, Does.Match(@"[0-9 \n]+"));

    }

    [Test]
    public void HeatSource_WithSmallRadius_ShouldLimitPropagation() {
        var source = new Vector2I(2, 2);
        var nearPoint = new Vector2I(3, 2);  // A un paso de la fuente
        var farPoint = new Vector2I(4, 2);   // A dos pasos de la fuente

        _heatmap.AddHeatSource(source, 1.0f, 1.0f); // Radio pequeño de 1
        _heatmap.UpdateHeatmap();

        Console.WriteLine("Heatmap with small radius:");
        Console.WriteLine(_heatmap.PrintHeatmap());

        // El punto cercano debería tener calor
        Assert.That(_heatmap.GetHeat(nearPoint), Is.GreaterThan(0));
        // El punto lejano no debería tener calor por estar fuera del radio
        Assert.That(_heatmap.GetHeat(farPoint), Is.EqualTo(0));
    }

    [Test]
    public void BaseHeat_ShouldFillNonHeatedAreas() {
        var source = new Vector2I(2, 2);
        var farPoint = new Vector2I(4, 4);   // Punto fuera del radio

        _heatmap.BaseHeat = 0.2f;
        _heatmap.AddHeatSource(source, 1.0f, 1.0f); // Radio pequeño
        _heatmap.UpdateHeatmap();

        Console.WriteLine("Heatmap with base heat:");
        Console.WriteLine(_heatmap.PrintHeatmap());

        // El punto lejano debería tener el calor base
        Assert.That(_heatmap.GetHeat(farPoint), Is.EqualTo(0.2f));
    }

    [Test]
    public void DifferentDecayFunctions_ShouldShowDifferentPatterns() {
        var source = new Vector2I(2, 2);
        var checkPoint = new Vector2I(3, 2); // Punto a distancia 1

        // Test Linear Decay
        _heatmap.DecayFunction = DecayFunction.Linear;
        _heatmap.AddHeatSource(source, 1.0f, 2.0f);
        _heatmap.UpdateHeatmap();
        var linearHeat = _heatmap.GetHeat(checkPoint);

        Console.WriteLine("Linear Decay:");
        Console.WriteLine(_heatmap.PrintHeatmap());

        // Test Exponential Decay
        _heatmap = new HeatmapGrid(_graph);
        _heatmap.DecayFunction = DecayFunction.Exponential;
        _heatmap.AddHeatSource(source, 1.0f, 2.0f);
        _heatmap.UpdateHeatmap();
        var exponentialHeat = _heatmap.GetHeat(checkPoint);

        Console.WriteLine("\nExponential Decay:");
        Console.WriteLine(_heatmap.PrintHeatmap());

        // El decay exponencial debería ser más pronunciado que el lineal
        Assert.That(exponentialHeat, Is.LessThan(linearHeat));
    }
}