using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Betauer.Core.DataMath.Collision;
using Betauer.Core.DataMath.PoissonDiskSampling.Utils;
using Godot;

namespace Betauer.Core.DataMath.PoissonDiskSampling; 

/*
 * https://www.vertexfragment.com/ramblings/variable-density-poisson-sampler/
 */
/// <summary>
/// A poisson disk sampler (blue noise) where the sample radius (minimum distance between points) is variable.<para/>
/// If the radius is constant, then <see cref="UniformPoissonSampler2D"/> should be used instead for better performance.
/// </summary>
public sealed class VariablePoissonSampler2D : PoissonSampler2D {
    private PointGrid _pointGrid;
    private List<int> _activeList;

    public delegate float GetRadiusAt(float x, float y);

    /// <param name="width">The width of the sampler domain. The maximum x value of a sampled position will be this.</param>
    /// <param name="height">The height of the sampler domain. The maximum y value of a sampled position will be this.</param>
    /// <param name="rejectionLimit">Number of generation attempts before a prospective point is rejected.</param>
    public VariablePoissonSampler2D(float width, float height, Func<Vector2, bool>? pointValidator = null, int rejectionLimit = 30) : base(width, height, pointValidator, rejectionLimit) {
    }

    /// <summary>
    /// Fills the sample domain with blue noise distributed points.
    /// </summary>
    /// <param name="radiusFunc"></param>
    /// <param name="minRadius">The minimum minimum distance between points. This is not enforced but used for optimizations.</param>
    /// <param name="maxRadius">The maximum minimum distance between points. This is not enforced but used for optimizations.</param>
    /// <param name="random"></param>
    /// <returns></returns>
    public List<Vector2> Generate(GetRadiusAt radiusFunc, float minRadius, float maxRadius, Random? random = null) {
        random ??= new Random();
        Initialize(minRadius, maxRadius);
        GenerateFirstPoint(random);

        while (_activeList.Count > 0) {
            var sampleFound = false;
            var activeIndex = random.Next(_activeList.Count);
            var currentSample = Samples[_activeList[activeIndex]];

            for (var i = 0; i < RejectionLimit; ++i) {
                var radius = radiusFunc(currentSample.X, currentSample.Y);
                var newSample = GenerateRandomPointInAnnulus(random, ref currentSample, radius);
                if (IsSampleOutOfBounds(ref newSample)) continue;
                
                if (!_pointGrid.Intersects(newSample.X, newSample.Y, radius)) {
                    _pointGrid.Add(newSample.X, newSample.Y);
                    AddSample(ref newSample);
                    sampleFound = true;
                    break;
                }
            }

            if (!sampleFound) {
                _activeList.RemoveUnorderedAt(activeIndex);
            }
        }
        return Samples;
    }

    public async Task<List<Vector2>> Generate(GetRadiusAt radiusFunc, float minRadius, float maxRadius, Func<Vector2, bool, Task> onAddSample, Random? random = null) {
        random ??= new Random();
        Initialize(minRadius, maxRadius);
        GenerateFirstPoint(random);

        while (_activeList.Count > 0) {
            var sampleFound = false;
            var activeIndex = random.Next(_activeList.Count);
            var currentSample = Samples[_activeList[activeIndex]];

            for (var i = 0; i < RejectionLimit; ++i) {
                var radius = radiusFunc(currentSample.X, currentSample.Y);
                var newSample = GenerateRandomPointInAnnulus(random, ref currentSample, radius);
                if (IsSampleOutOfBounds(ref newSample)) continue;

                if (!_pointGrid.Intersects(newSample.X, newSample.Y, radius)) {
                    _pointGrid.Add(newSample.X, newSample.Y);
                    AddSample(ref newSample);
                    sampleFound = true;
                    await onAddSample.Invoke(newSample, true);
                    break;
                } else {
                    await onAddSample.Invoke(newSample, false);
                }
            }

            if (!sampleFound) {
                _activeList.RemoveUnorderedAt(activeIndex);
            }
        }
        return Samples;
    }

    private void Initialize(float minRadius, float maxRadius) {
        _pointGrid = new PointGrid(Width, Height, minRadius, maxRadius);
        _activeList = new List<int>(_pointGrid.CellsPerX * _pointGrid.CellsPerY);
        Samples = new List<Vector2>();
    }

    private void GenerateFirstPoint(Random random) {
        Vector2 sample = new Vector2((float)random.NextDouble() * Width,(float)random.NextDouble() * Height);
        _pointGrid.Add(sample.X, sample.Y);
        AddSample(ref sample);
    }

    private void AddSample(ref Vector2 sample) {
        var sampleIndex = Samples.Count;
        Samples.Add(sample);
        _activeList.Add(sampleIndex);
    }
}