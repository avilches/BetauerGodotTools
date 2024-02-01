using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Betauer.Core;
using Betauer.Core.Collision.Spatial2D;
using Betauer.Core.Image;
using Betauer.Core.PoissonDiskSampling;
using Godot;

namespace Veronenger.Game.RTS.World; 

public partial class PoissonDemos : Node2D {
    
    private Sprite2D TextureTerrainMap { get; set; }
    private Sprite2D TexturePoisson { get; set; }

    public PoissonDemos(Sprite2D textureTerrainMap, Sprite2D texturePoisson) {
        TextureTerrainMap = textureTerrainMap;
        TexturePoisson = texturePoisson;
    }

    public override async void _Ready() {
        await GenerateUniformPoissonDisks();
        await GenerateUniformPoissonDisksExpanded();
        await GenerateVariablePoissonDisksWithNoise();
        await GenerateVariablePoissonDisksWithRandomRadius();
        await GenerateVariablePoissonDisksWithNoiseExpanded();
        await GenerateVariablePoissonDisksWithNoiseExpanded2();
        
        await GenerateVariablePoissonDisksWithRandomRadius();
        await GenerateVariablePoissonDisksWithRandomRadius2();
        await GenerateVariablePoissonDisksWithRandomRadiusRemovingRandom();
        await GenerateVariablePoissonDisksWithRectangleRandomRadiusRemovingRandom();
    }

    private async Task GenerateUniformPoissonDisks() {
        var fast = new FastImage(512, 512);
        fast.Fill(Color.FromHtml("221133"));
        var radius = 30;
        var random = new Random(1);
        var uni = new UniformPoissonSampler2D(512, 512);
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
        AddChild(new Sprite2D() {
            GlobalPosition = TextureTerrainMap.GlobalPosition + new Vector2(512, 0),
            Texture = fast.CreateImageTexture()
        });
    }

    private async Task GenerateUniformPoissonDisksExpanded() {
        var fast = new FastImage(512, 512);
        fast.Fill(Color.FromHtml("221133"));
        var imageTexture = new ImageTexture();
        imageTexture.SetSizeOverride(new Vector2I(fast.Width, fast.Height));
        var radius = 30;
        var random = new Random(1);
        var uni = new UniformPoissonSampler2D(512, 512);
        var points = uni.Generate(30f, random);
        points.ForEach(v => fast.SetPixel((int)v.X, (int)v.Y, Colors.Green));
        points.ForEach(v => { fast.DrawCircle((int)v.X, (int)v.Y, radius / 2, new Color("#AA5555", 0.5f)); });
        var grid = ExpandCircles(radius, radius, points);
        grid.ForEach<Circle>(v => { fast.DrawCircle((int)v.Position.X, (int)v.Position.Y, (int)v.Radius, Colors.Red); });
        fast.Flush();
        AddChild(new Sprite2D() {
            GlobalPosition = TextureTerrainMap.GlobalPosition + new Vector2(1024, 0),
            Texture = imageTexture
        });
    }

    private async Task GenerateVariablePoissonDisksWithNoise() {
        var fast = new FastImage(512, 512);
        fast.Fill(Color.FromHtml("221133"));
        var minRadius = 1;
        var maxRadius = 30;
        var fastNoise = new FastNoise((NoiseTexture2D)TexturePoisson.Texture);
        var random = new Random(1);
        var points = new VariablePoissonSampler2D(512, 512).GenerateFromNoise(fastNoise, minRadius, maxRadius, random);
        points.ForEach(v => fast.SetPixel((int)v.X, (int)v.Y, Colors.Green));
        fast.Flush();
        AddChild(new Sprite2D() {
            GlobalPosition = TextureTerrainMap.GlobalPosition + new Vector2(0, 512),
            Texture = fast.CreateImageTexture()
        });
    }

    private async Task GenerateVariablePoissonDisksWithNoiseExpanded() {
        var fast = new FastImage(512, 512);
        fast.Fill(Color.FromHtml("221133"));
        var minRadius = 4;
        var maxRadius = 60;
        var fastNoise = new FastNoise((NoiseTexture2D)TexturePoisson.Texture);
        var random = new Random(1);
        var points = new VariablePoissonSampler2D(512, 512).GenerateFromNoise(fastNoise, minRadius, maxRadius, random);
        var grid = ExpandCircles(minRadius, maxRadius, points);
        points.ForEach(v => fast.SetPixel((int)v.X, (int)v.Y, Colors.Green));
        grid.ForEach<Circle>(v => { fast.DrawCircle((int)v.Position.X, (int)v.Position.Y, (int)v.Radius, Colors.Red); });
        fast.Flush();
        AddChild(new Sprite2D() {
            GlobalPosition = TextureTerrainMap.GlobalPosition + new Vector2(512, 512),
            Texture = fast.CreateImageTexture()
        });
    }

    private async Task GenerateVariablePoissonDisksWithNoiseExpanded2() {
        var fast = new FastImage(512, 512);
        fast.Fill(Color.FromHtml("332211"));
        var minRadius = 2f;
        var maxRadius = 80f;
        var fastNoise = new FastNoise((NoiseTexture2D)TexturePoisson.Texture);
        var random = new Random(1);
        var points = new VariablePoissonSampler2D(512, 512).GenerateFromNoise(fastNoise, minRadius, maxRadius, random);
        var grid = ExpandCircles(minRadius, maxRadius, points);
        points.ForEach(v => fast.SetPixel((int)v.X, (int)v.Y, Colors.Green));
        grid.ForEach<Circle>(v => { fast.DrawCircle((int)v.Position.X, (int)v.Position.Y, (int)v.Radius, Colors.Red); });
        fast.Flush();
        AddChild(new Sprite2D() {
            GlobalPosition = TextureTerrainMap.GlobalPosition + new Vector2(1024, 512),
            Texture = fast.CreateImageTexture()
        });
    }

    private async Task GenerateVariablePoissonDisksWithRandomRadius() {
        var fast = new FastImage(512, 512);
        fast.Fill(Color.FromHtml("332211"));
        fast.Flush();
        var minRadius = 10f;
        var maxRadius = 60f;
        var random = new Random(1);
        var points = new VariablePoissonSampler2D(512, 512)
            .GenerateRandom(minRadius, maxRadius, random);
        var grid = ExpandCircles(minRadius, maxRadius, points);
        grid.ForEach<Circle>(v => { fast.DrawCircle((int)v.Position.X, (int)v.Position.Y, (int)v.Radius, Colors.Red); });
        fast.Flush();
        AddChild(new Sprite2D() {
            GlobalPosition = TextureTerrainMap.GlobalPosition + new Vector2(0, 1024),
            Texture = fast.CreateImageTexture()
        });
    }

    private async Task GenerateVariablePoissonDisksWithRandomRadius2() {
        var fast = new FastImage(512, 512);
        fast.Fill(Color.FromHtml("332211"));
        fast.Flush();
        var minRadius = 20f;
        var maxRadius = 120f;
        var random = new Random(1);
        var points = new VariablePoissonSampler2D(512, 512)
            .Generate((x, y) => random.Range(minRadius, maxRadius), minRadius, maxRadius, random);
        var grid = ExpandCircles(minRadius, maxRadius, points);
        grid.ForEach<Circle>(v => { fast.DrawCircle((int)v.Position.X, (int)v.Position.Y, (int)v.Radius, Colors.Red); });
        fast.Flush();
        AddChild(new Sprite2D() {
            GlobalPosition = TextureTerrainMap.GlobalPosition + new Vector2(512, 1024),
            Texture = fast.CreateImageTexture()
        });
    }

    private async Task GenerateVariablePoissonDisksWithRandomRadiusRemovingRandom() {
        var fast = new FastImage(512, 512);
        fast.Fill(Color.FromHtml("332211"));
        fast.Flush();
        var minRadius = 20f;
        var maxRadius = 120f;
        var random = new Random(1);

        var sampler = new VariablePoissonSampler2D(512, 512);
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
        AddChild(new Sprite2D() {
            GlobalPosition = TextureTerrainMap.GlobalPosition + new Vector2(1024, 1024),
            Texture = fast.CreateImageTexture()
        });
    }

    private async Task GenerateVariablePoissonDisksWithRectangleRandomRadiusRemovingRandom() {
        var fast = new FastImage(512, 512);
        fast.Fill(Color.FromHtml("333333"));
        fast.Flush();
        var minRadius = 20f;
        var maxRadius = 120f;
        var random = new Random(1);

        var sampler = new VariablePoissonSampler2D(512, 512);
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
        AddChild(new Sprite2D() {
            GlobalPosition = TextureTerrainMap.GlobalPosition + new Vector2(1024+512, 1024),
            Texture = fast.CreateImageTexture()
        });
    }

    private static SpatialGrid ExpandCircles(float minRadius, float maxRadius, List<Vector2> points) {
        var grid = SpatialGrid.FromAverageDistance(minRadius, maxRadius);
        grid.AddPointsAsCircles(points);
        grid.ExpandAll(1);
        return grid;
    }

    private static SpatialGrid ExpandRectangles(float minRadius, float maxRadius, List<Vector2> points) {
        var grid = SpatialGrid.FromAverageDistance(minRadius, maxRadius);
        grid.AddPointsAsRectangles(points);
        grid.ExpandAll(1);
        return grid;
    }
}