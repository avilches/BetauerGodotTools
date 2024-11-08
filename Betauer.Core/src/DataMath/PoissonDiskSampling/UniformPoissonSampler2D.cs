using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Betauer.Core.DataMath.PoissonDiskSampling.Utils;
using Godot;

namespace Betauer.Core.DataMath.PoissonDiskSampling;

/*
 * https://www.vertexfragment.com/ramblings/variable-density-poisson-sampler/
 */
/// <summary>
/// A poisson disk sampler (blue noise) where the sample radius (minimum distance between points) is constant/uniform through the entire domain.
/// </summary>
public sealed class UniformPoissonSampler2D : PoissonSampler2D {
    // Note that we implement our own Spatial Grid here and do not use SpatialGrid2D.
    // This is because we know there is a fixed radius and exactly one point per cell and so we are able
    // to take some short cuts for better performance that is not available in the more generic SpatialGrid2D.

    /// <summary>
    /// The length of each side of a cell in the underlying spatial grid.
    /// </summary>
    public float CellSize { get; private set; }

    /// <summary>
    /// The number of cells in the x-dimension in the underlying spatial grid.
    /// </summary>
    public int CellsPerX { get; private set; }

    /// <summary>
    /// The number of cells in the y-dimension in the underlying spatial grid.
    /// </summary>
    public int CellsPerY { get; private set; }

    /// <summary>
    /// The spatial grid used at as a lookup for faster "are there any nearby points?" queries.
    /// Stored as a 1D array, and accessed via <see cref="GetSpatialGridIndex(ref Vector2)"/>.
    /// </summary>
    private List<int> _spatialLookUp;

    /// <summary>
    /// List of candidate points that we will try to generate new points around.
    /// </summary>
    private List<int> _activeList;

    // ---------------------------------------------------------------------------------
    // Methods
    // ---------------------------------------------------------------------------------

    /// <param name="random">The RNG used to generate the random points.</param>
    /// <param name="width">The width of the sampler domain. The maximum x value of a sampled position will be this.</param>
    /// <param name="height">The height of the sampler domain. The maximum y value of a sampled position will be this.</param>
    /// <param name="pointValidator">If true, the points generated will be contained in the Width x Height rect. If false, the points will be inside a ellipse of radius y=Height/2 x=Width/2</param>
    /// <param name="rejectionLimit">Number of generation attempts before a prospective point is rejected.</param>
    public UniformPoissonSampler2D(float width, float height, Func<Vector2, bool>? pointValidator = null, int rejectionLimit = 30) : base(width, height, pointValidator, rejectionLimit) {
    }

    /// <summary>
    /// Fills the sample domain with blue noise distributed points.
    /// Once generation is complete the results can be obtained from <see cref="Samples"/>.
    /// </summary>
    /// <returns></returns>
    public List<Vector2> Generate(float sampleRadius, Random? random = null) {
        random ??= new Random();
        Initialize(sampleRadius);
        GenerateFirstPoint(random);

        var radii = sampleRadius * sampleRadius;
        while (_activeList.Count > 0) {
            var sampleFound = false;
            var activeIndex = random.Next(_activeList.Count);
            Vector2 currentSample = Samples[_activeList[activeIndex]];

            for (var i = 0; i < RejectionLimit; ++i) {
                Vector2 randomSample = GenerateRandomPointInAnnulus(random, ref currentSample, sampleRadius);
                if (IsSampleOutOfBounds(ref randomSample)) continue;
                if (!IsSampleNearOthers(ref randomSample, radii)) {
                    AddSample(ref randomSample);
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

    public async Task<List<Vector2>> Generate(float sampleRadius, Func<Vector2, bool, Task> onAddSample, Random? random = null) {
        random ??= new Random();
        Initialize(sampleRadius);
        GenerateFirstPoint(random);
        var radii = sampleRadius * sampleRadius;
        while (_activeList.Count > 0) {
            var sampleFound = false;
            var activeIndex = random.Next(_activeList.Count);
            Vector2 currentSample = Samples[_activeList[activeIndex]];

            for (var i = 0; i < RejectionLimit; ++i) {
                Vector2 randomSample = GenerateRandomPointInAnnulus(random, ref currentSample, sampleRadius);
                if (IsSampleOutOfBounds(ref randomSample)) continue;
                if (!IsSampleNearOthers(ref randomSample, radii)) {
                    AddSample(ref randomSample);
                    sampleFound = true;
                    await onAddSample.Invoke(randomSample, true);
                    break;
                }
                await onAddSample.Invoke(randomSample, false);
            }
            if (!sampleFound) {
                _activeList.RemoveUnorderedAt(activeIndex);
            }
        }

        return Samples;
    }

    private void Initialize(float sampleRadius) {
        CellSize = sampleRadius / Mathf.Sqrt2;
        CellsPerX = (int)System.Math.Ceiling(Width / CellSize);
        CellsPerY = (int)System.Math.Ceiling(Height / CellSize);

        var totalCells = CellsPerX * CellsPerY;

        _spatialLookUp = new List<int>(totalCells);
        _activeList = new List<int>(totalCells);
        Samples = new List<Vector2>();

        for (var i = 0; i < totalCells; ++i) {
            _spatialLookUp.Add(-1);
        }
    }

    private void GenerateFirstPoint(Random random) {
        var firstPoint = new Vector2((float)random.NextDouble() * Width, (float)random.NextDouble() * Height);
        AddSample(ref firstPoint);
    }

    private void AddSample(ref Vector2 sample) {
        var sampleIndex = Samples.Count;
        var spatialIndex = GetSpatialGridIndex(ref sample);
        Samples.Add(sample);
        _activeList.Add(sampleIndex);
        _spatialLookUp[spatialIndex] = sampleIndex;
    }

    /// <summary>
    /// Calculates the index into the spatial grid for the given point.
    /// Does not perform bounds checking.
    /// </summary>
    /// <param name="sample"></param>
    /// <returns></returns>
    private int GetSpatialGridIndex(ref Vector2 sample) {
        var dx = (int)(sample.X / CellSize);
        var dy = (int)(sample.Y / CellSize);
        return dy * CellsPerX + dx;
    }

    /// <summary>
    /// Checks if the sample is near any others by checking neighboring cells.
    /// </summary>
    /// <param name="sample"></param>
    /// <returns></returns>
    private bool IsSampleNearOthers(ref Vector2 sample, float radii) {
        var prospectiveCell = GetSpatialGridIndex(ref sample);
        if (prospectiveCell == -1 || _spatialLookUp[prospectiveCell] != -1) {
            return true;
        }

        for (var y = -1; y <= 1; ++y) {
            for (var x = -1; x <= 1; ++x) {
                var neighbor = prospectiveCell + x + y * CellsPerX;
                if (neighbor < 0 || neighbor >= _spatialLookUp.Count) {
                    continue;
                }
                var cellSampleIndex = _spatialLookUp[neighbor];
                if (cellSampleIndex == -1) {
                    continue;
                }
                var other = Samples[cellSampleIndex];
                if (sample.DistanceSquaredTo(other) <= radii) {
                    return true;
                }
            }
        }
        return false;
    }
}