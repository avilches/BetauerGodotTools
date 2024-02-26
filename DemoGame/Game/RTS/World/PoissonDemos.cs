using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Betauer.Core;
using Betauer.Core.Collision;
using Betauer.Core.Collision.Spatial2D;
using Betauer.Core.Data;
using Betauer.Core.Image;
using Betauer.Core.PoissonDiskSampling;
using Godot;
using FastNoiseLite = Betauer.Core.Data.FastNoiseLite;

namespace Veronenger.Game.RTS.World;

public class PoissonDemos {
    public void TestAll() {
        // await GenerateUniformPoissonDisks();
        // await GenerateUniformPoissonDisksExpanded();
        // await GenerateVariablePoissonDisksWithNoise();
        // await GenerateVariablePoissonDisksWithRandomRadius();
        // await GenerateVariablePoissonDisksWithNoiseExpanded();
        // await GenerateVariablePoissonDisksWithNoiseExpanded2();
        // await GenerateVariablePoissonDisksWithRandomRadius();
        // await GenerateVariablePoissonDisksWithRandomRadius2();
        // await GenerateVariablePoissonDisksWithRandomRadiusRemovingRandom();
        // await GenerateVariablePoissonDisksWithRectangleRandomRadiusRemovingRandom();
    }

    public async Task GenerateUniformPoissonDisks(FastImage fast) {
        var radius = 30;
        var random = new Random(1);
        var uni = new UniformPoissonSampler2D(fast.Width, fast.Height);
        var points = uni.Generate(30f, random);
        for (int i = 0; i < (int)uni.Width; i++) {
            for (int j = 0; j < (int)uni.Height; j++) {
                if (i % (int)uni.CellSize == 0 || j % (int)uni.CellSize == 0) {
                    fast.SetPixel(i, j, new Color("#332211", 0.5f));
                }
            }
        }
        points.ForEach(v => fast.SetPixel((int)v.X, (int)v.Y, Colors.Green));
        points.ForEach(v => { fast.DrawCircle((int)v.X, (int)v.Y, radius / 2, new Color("#AA5555", 0.5f)); });
        fast.Flush();
    }

    public async Task GenerateUniformPoissonDisksExpanded(FastImage fast) {
        var radius = 30;
        var random = new Random(1);
        var center = new Vector2(fast.Width / 2f, fast.Height / 2f);
        var uni = new UniformPoissonSampler2D(fast.Width, fast.Height, (v) => Geometry.IsPointInCircle(v.X, v.Y, center.X, center.Y, fast.Height / 2f));
        var points = uni.Generate(30f, random);
        points.ForEach(v => fast.SetPixel((int)v.X, (int)v.Y, Colors.Green));
        points.ForEach(v => { fast.DrawCircle((int)v.X, (int)v.Y, radius / 2, new Color("#AA5555", 0.5f)); });
        var grid = ExpandCircles(radius, radius, points);
        grid.ForEach<Circle>(v => { fast.DrawCircle((int)v.Position.X, (int)v.Position.Y, (int)v.Radius, Colors.Red); });
        fast.Flush();
    }

    public async Task GenerateVariablePoissonDisksWithNoise(FastImage fast, FastNoiseLite noise) {
        var minRadius = 1;
        var maxRadius = 30;
        var random = new Random(1);
        var points = new VariablePoissonSampler2D(fast.Width, fast.Height).GenerateFromNoise(CreateNormalizedFunc(noise, fast.Width, fast.Height), minRadius, maxRadius, random);
        points.ForEach(v => fast.SetPixel((int)v.X, (int)v.Y, Colors.Green));
        fast.Flush();
    }

    private Func<float, float, float> CreateNormalizedFunc(FastNoiseLite fastNoise, int width, int height) {
        var dataGrid = new YxDataGrid<float>(width, height).Load((x, y) => fastNoise.GetNoise(x, y)).Normalize();
        return (x, y) => dataGrid.GetValue(Math.Clamp(Mathf.RoundToInt(x), 0, width - 1), Math.Clamp(Mathf.RoundToInt(y), 0, height - 1));
    }

    public async Task GenerateVariablePoissonDisksWithNoiseExpanded(FastImage fast, FastNoiseLite noise) {
        var minRadius = 4;
        var maxRadius = 60;
        var random = new Random(1);
        var points = new VariablePoissonSampler2D(fast.Width, fast.Height).GenerateFromNoise(CreateNormalizedFunc(noise, fast.Width, fast.Height), minRadius, maxRadius, random);
        var grid = ExpandCircles(minRadius, maxRadius, points);
        points.ForEach(v => fast.SetPixel((int)v.X, (int)v.Y, Colors.Green));
        grid.ForEach<Circle>(v => { fast.DrawCircle((int)v.Position.X, (int)v.Position.Y, (int)v.Radius, Colors.Red); });
        fast.Flush();
    }

    public async Task GenerateVariablePoissonDisksWithNoiseExpanded2(FastImage fast, FastNoiseLite noise) {
        var minRadius = 2f;
        var maxRadius = 80f;
        var random = new Random(1);
        var points = new VariablePoissonSampler2D(fast.Width, fast.Height).GenerateFromNoise(CreateNormalizedFunc(noise, fast.Width, fast.Height), minRadius, maxRadius, random);
        var grid = ExpandCircles(minRadius, maxRadius, points);
        points.ForEach(v => fast.SetPixel((int)v.X, (int)v.Y, Colors.Green));
        grid.ForEach<Circle>(v => { fast.DrawCircle((int)v.Position.X, (int)v.Position.Y, (int)v.Radius, Colors.Red); });
        fast.Flush();
    }

    public async Task GenerateVariablePoissonDisksWithRandomRadius(FastImage fast) {
        var minRadius = 10f;
        var maxRadius = 60f;
        var random = new Random(1);
        var points = new VariablePoissonSampler2D(fast.Width, fast.Height)
            .GenerateRandom(minRadius, maxRadius, random);
        var grid = ExpandCircles(minRadius, maxRadius, points);
        grid.ForEach<Circle>(v => { fast.DrawCircle((int)v.Position.X, (int)v.Position.Y, (int)v.Radius, Colors.Red); });
        fast.Flush();
    }

    public async Task GenerateVariablePoissonDisksWithRandomRadius2(FastImage fast) {
        var minRadius = 20f;
        var maxRadius = 120f;
        var random = new Random(1);
        var points = new VariablePoissonSampler2D(fast.Width, fast.Height)
            .Generate((x, y) => random.Range(minRadius, maxRadius), minRadius, maxRadius, random);
        var grid = ExpandCircles(minRadius, maxRadius, points);
        grid.ForEach<Circle>(v => { fast.DrawCircle((int)v.Position.X, (int)v.Position.Y, (int)v.Radius, Colors.Red); });
        fast.Flush();
    }

    public async Task GenerateVariablePoissonDisksWithRandomRadiusRemovingRandom(FastImage fast) {
        var minRadius = 20f;
        var maxRadius = 120f;
        var random = new Random(1);

        var sampler = new VariablePoissonSampler2D(fast.Width, fast.Height);
        var grid = ExpandCircles(minRadius, maxRadius, sampler.GenerateRandom(minRadius, maxRadius, random));
        var gridMini = ExpandCircles(5, 50, sampler.GenerateRandom(25, 50, random));

        var x = 0;
        grid.RemoveAll<Circle>(c => x++ % 2 == 0);

        gridMini.ForEach<Circle>(circle => {
            if (!grid.IntersectCircle(circle.Position.X, circle.Position.Y, circle.Radius)) {
                grid.Add(circle);
            }
        });
        grid.ForEach<Circle>(v => {
            fast.DrawCircle((int)v.Position.X, (int)v.Position.Y, (int)v.Radius, Colors.Red);
        });
        fast.Flush();
    }

    public async Task GenerateVariablePoissonDisksWithRectangleRandomRadiusRemovingRandom(FastImage fast) {
        var minRadius = 20f;
        var maxRadius = 120f;
        var random = new Random(1);

        var sampler = new VariablePoissonSampler2D(fast.Width, fast.Height);
        var grid = ExpandRectangles(minRadius, maxRadius, sampler.GenerateRandom(minRadius, maxRadius, random));
        var gridMini = ExpandRectangles(5, 50, sampler.GenerateRandom(25, 50, random));

        var x = 0;
        grid.RemoveAll<Rectangle>(c => x++ % 2 == 0);

        gridMini.ForEach<Rectangle>(rectangle => {
            if (grid.IntersectRectangle(rectangle.Position.X, rectangle.Position.Y, rectangle.Width, rectangle.Height)) {
                grid.Add(rectangle);
            }
        });
        grid.RemoveAll<Rectangle>(c => x++ % 2 == 0);
        grid.AdjustAll();
        grid.ForEach<Rectangle>(v => {
            fast.DrawRect((int)v.Position.X, (int)v.Position.Y, (int)v.Width, (int)v.Height, Colors.Red);
        });
        fast.Flush();
    }

    public static SpatialGrid ExpandCircles(float minRadius, float maxRadius, List<Vector2> points) {
        var grid = SpatialGrid.FromAverageDistance(minRadius, maxRadius);
        grid.AddPointsAsCircles(points);
        grid.ExpandAll(1);
        return grid;
    }

    public static SpatialGrid ExpandRectangles(float minRadius, float maxRadius, List<Vector2> points) {
        var grid = SpatialGrid.FromAverageDistance(minRadius, maxRadius);
        grid.AddPointsAsRectangles(points);
        grid.ExpandAll(1);
        return grid;
    }
}