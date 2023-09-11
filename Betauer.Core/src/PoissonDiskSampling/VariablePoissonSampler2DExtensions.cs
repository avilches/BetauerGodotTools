using System;
using System.Collections.Generic;
using Betauer.Core.Collision.Spatial2D;
using Betauer.Core.Image;
using Godot;

namespace Betauer.Core.PoissonDiskSampling; 

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
    
    public static List<Vector2> GenerateFromNoise(this VariablePoissonSampler2D sampler, INoise2D fastNoise, float minRadius, float maxRadius, Random? random = null) {
        return sampler.Generate((x, y) => {
            var noise = fastNoise.GetNoise((int)x, (int)y);
            return Mathf.Lerp(minRadius, maxRadius, noise);
        }, minRadius, maxRadius, random);
    }

    public static SpatialGrid GenerateFromNoiseAndExpand(this VariablePoissonSampler2D sampler, INoise2D fastNoise, float minRadius, float maxRadius,
        float expandBy = 1f, Random? random = null) {
        var points = GenerateFromNoise(sampler, fastNoise, minRadius, maxRadius, random);
        var spatialGrid = SpatialGrid.FromAverageDistance(minRadius, maxRadius);
        spatialGrid.AddPointsAsCircles(points);
        spatialGrid.ExpandAll(expandBy);
        return spatialGrid;
    }

}