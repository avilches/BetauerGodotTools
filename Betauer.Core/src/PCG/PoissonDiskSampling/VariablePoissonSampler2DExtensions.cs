using System;
using System.Collections.Generic;
using Betauer.Core.DataMath.Collision.Spatial2D;
using Godot;

namespace Betauer.Core.PCG.PoissonDiskSampling; 

public static class VariablePoissonSampler2DExtensions {

    public static List<Vector2> GenerateRandom(this VariablePoissonSampler2D sampler, float minRadius, float maxRadius, Random? random = null) {
        return sampler.Generate((x, y) => random.Range(minRadius, maxRadius), minRadius, maxRadius, random);
    }
    
    public static SpatialGrid GenerateRandomAndExpand(this VariablePoissonSampler2D sampler, float minRadius, float maxRadius, float expandBy = 1f, Random? random = null) {
        var points = GenerateRandom(sampler, minRadius, maxRadius, random);
        var spatialGrid = SpatialGrid.FromAverageDistance(minRadius, maxRadius);
        spatialGrid.AddPointsAsCircles(points);
        spatialGrid.ExpandAll(expandBy);
        return spatialGrid;
        
    }
    
    public static List<Vector2> GenerateFromNoise(this VariablePoissonSampler2D sampler, Func<float, float, float> normalizedNoise, float minRadius, float maxRadius, Random? random = null) {
        return sampler.Generate((x, y) => {
            var noise = normalizedNoise.Invoke(x, y);
            if (noise is < 0f or > 1f) throw new Exception($"Noise must be normalized between 0..1, but it was {noise:0.0}");
            return Mathf.Lerp(minRadius, maxRadius, noise);
        }, minRadius, maxRadius, random);
    }

    public static SpatialGrid GenerateFromNoiseAndExpand(this VariablePoissonSampler2D sampler, Func<float, float, float> normalizedNoise, float minRadius, float maxRadius,
        float expandBy = 1f, Random? random = null) {
        var points = GenerateFromNoise(sampler, normalizedNoise, minRadius, maxRadius, random);
        var spatialGrid = SpatialGrid.FromAverageDistance(minRadius, maxRadius);
        spatialGrid.AddPointsAsCircles(points);
        spatialGrid.ExpandAll(expandBy);
        return spatialGrid;
    }

}