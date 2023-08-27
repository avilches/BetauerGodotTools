using System;
using System.Collections.Generic;
using Betauer.Core.Image;
using Godot;

namespace Betauer.Core.PoissonDiskSampling; 

public static class VariablePoissonSampler2DExtensions {

    public static List<Vector2> GenerateRandom(this VariablePoissonSampler2D sampler, float minRadius, float maxRadius, Random? random = null) {
        return sampler.Generate((x, y) => random.Range(minRadius, maxRadius), minRadius, maxRadius, random);
    }
    
    public static List<Vector2> GenerateFromNoise(this VariablePoissonSampler2D sampler, FastNoise fastNoise, float minRadius, float maxRadius, Random? random = null) {
        return sampler.Generate((x, y) => {
            var noise = fastNoise.GetNoise((int)x, (int)y);
            return Mathf.Lerp(minRadius, maxRadius, noise);
        }, minRadius, maxRadius, random);
    }
    
    
}