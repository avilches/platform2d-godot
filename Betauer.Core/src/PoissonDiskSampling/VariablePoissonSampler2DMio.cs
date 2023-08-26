﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Betauer.Core.Collision;
using Betauer.Core.Collision.Spatial2D;
using Betauer.Core.PoissonDiskSampling.Utils;
using Godot;

namespace Betauer.Core.PoissonDiskSampling; 

/*
 * https://www.vertexfragment.com/ramblings/variable-density-poisson-sampler/
 */
/// <summary>
/// A poisson disk sampler (blue noise) where the sample radius (minimum distance between points) is variable.<para/>
/// If the radius is constant, then <see cref="UniformPoissonSampler2D"/> should be used instead for better performance.
/// </summary>
public sealed class VariablePoissonSampler2DMio {
    /// <summary>
    /// The sample point generated by the sampler. The <c>z</c> component is the radius<c>0</c>.
    /// </summary>
    public List<Vector2> Samples { get; private set; }

    /// <summary>
    /// The width of the domain of the sampler. This is the maximum <c>x</c> value in a generated point.
    /// </summary>
    public float Width => _maxPoint.X;

    /// <summary>
    /// The height of the domain of the sampler. This is the maximum <c>y</c> value in a generated point.
    /// </summary>
    public float Height => _maxPoint.Y;

    /// <summary>
    /// The maximum number of attempts to generate a valid new point.
    /// The higher this value, the higher the coverage of the sampler but an increased runtime cost.
    /// </summary>
    public int RejectionLimit { get; }

    /// <summary>
    /// Represents the bounds of the sampler region, on the range <c>[(0, 0), (MaxPoint.X, MaxPoint.Y)]</c>.
    /// </summary>
    private readonly Vector2 _maxPoint;

    /// <summary>
    /// The RNG used to generate the points in the sampled annulus.
    /// </summary>
    private readonly Random _random;

    /// <summary>
    /// 
    /// </summary>
    // private SpatialGrid2D _spatialGrid;
    private SpatialGrid _spatialGrid2;

    /// <summary>
    /// List of candidate points that we will try to generate new points around.
    /// </summary>
    private List<int> _activeList;

    // ---------------------------------------------------------------------------------
    // Delegates
    // ---------------------------------------------------------------------------------

    public delegate float GetRadiusAt(float x, float y);

    // ---------------------------------------------------------------------------------
    // Methods
    // ---------------------------------------------------------------------------------

    /// <param name="random">The RNG used to generate the random points.</param>
    /// <param name="width">The width of the sampler domain. The maximum x value of a sampled position will be this.</param>
    /// <param name="height">The height of the sampler domain. The maximum y value of a sampled position will be this.</param>
    /// <param name="rejectionLimit">Number of generation attempts before a prospective point is rejected.</param>
    public VariablePoissonSampler2DMio(System.Random random, float width, float height, int rejectionLimit = 30) {
        _random = random;
        RejectionLimit = rejectionLimit;
        _maxPoint = new Vector2(width, height);
    }
    
    /// <summary>
    /// Fills the sample domain with blue noise distributed points.
    /// Once generation is complete the results can be obtained from <see cref="SamplesList"/>.
    /// </summary>
    /// <param name="minRadius">The minimum minimum distance between points. This is not enforced but used for optimizations.</param>
    /// <param name="maxRadius">The maximum minimum distance between points. This is not enforced but used for optimizations.</param>
    /// <returns></returns>
    public async Task<List<Vector2>> Generate(GetRadiusAt radiusFunc, float minRadius, float maxRadius, Func<Vector2, bool, Task> onAddSample = null) {
        Initialize(minRadius, maxRadius);
        GenerateFirstPoint();

        while (_activeList.Count > 0) {
            var sampleFound = false;
            var activeIndex = GetRandomActiveListIndex();

            Vector2 currentSample = Samples[_activeList[activeIndex]];

            for (var i = 0; i < RejectionLimit; ++i) {
                var radius = radiusFunc(currentSample.X, currentSample.Y);
                Vector2 randomSample = GenerateRandomPointInAnnulus(ref currentSample, radius);

                if (!Geometry.IsPointInsideRectangle(randomSample.X, randomSample.Y, 0, 0, Width, Height)) continue;
                
                if (!_spatialGrid2.IntersectCircle(randomSample.X, randomSample.Y, radius)) {
                    _spatialGrid2.Add(new Point(randomSample));
                // if (_spatialGrid.AddIfOpen(randomSample.X, randomSample.Y, radius)) {
                    AddSample(ref randomSample);
                    if (onAddSample != null) await onAddSample.Invoke(randomSample, true);
                    sampleFound = true;
                    break;
                } else {
                    if (onAddSample != null) await onAddSample.Invoke(randomSample, false);
                }
            }

            if (!sampleFound) {
                _activeList.RemoveUnorderedAt(activeIndex);
            }
        }
        return Samples;
    }
    public List<Vector2> Generate(GetRadiusAt radiusFunc, float minRadius, float maxRadius) {
        Initialize(minRadius, maxRadius);
        GenerateFirstPoint();

        while (_activeList.Count > 0) {
            var sampleFound = false;
            var activeIndex = GetRandomActiveListIndex();

            Vector2 currentSample = Samples[_activeList[activeIndex]];

            for (var i = 0; i < RejectionLimit; ++i) {
                var radius = radiusFunc(currentSample.X, currentSample.Y);
                Vector2 randomSample = GenerateRandomPointInAnnulus(ref currentSample, radius);
                if (!Geometry.IsPointInsideRectangle(randomSample.X, randomSample.Y, 0, 0, Width, Height)) continue;
                
                if (!_spatialGrid2.IntersectCircle(randomSample.X, randomSample.Y, radius)) {
                    _spatialGrid2.Add(new Point(randomSample));
                // if (_spatialGrid.AddIfOpen(randomSample.X, randomSample.Y, radius)) {
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

    /// <summary>
    /// Initializes the sampler for a new run.
    /// </summary>
    private void Initialize(float minRadius, float maxRadius) {
        // _spatialGrid = new SpatialGrid2D(Width, Height, minRadius, maxRadius);
        _spatialGrid2 = new SpatialGrid(minRadius, maxRadius);
        _activeList = new List<int>();
        Samples = new List<Vector2>();
    }

    /// <summary>
    /// Generates the first random point in the sample domain and adds it to our collections.
    /// </summary>
    private void GenerateFirstPoint() {
        Vector2 sample = new Vector2(
            (float)_random.NextDouble() * _maxPoint.X,
            (float)_random.NextDouble() * _maxPoint.Y);

        // _spatialGrid.Add(sample.X, sample.Y);
        _spatialGrid2.Add(new Point(sample));

        AddSample(ref sample);
    }

    /// <summary>
    /// Adds the new sample to the samples list and active list.
    /// </summary>
    /// <param name="sample"></param>
    private void AddSample(ref Vector2 sample) {
        int sampleIndex = Samples.Count;

        Samples.Add(sample);
        _activeList.Add(sampleIndex);
    }

    /// <summary>
    /// Retrieves a random index from the active list.
    /// </summary>
    /// <returns></returns>
    private int GetRandomActiveListIndex() {
        return _random.Next(_activeList.Count);
    }

    /// <summary>
    /// Generate a new random point in the annulus around the provided point.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    private Vector2 GenerateRandomPointInAnnulus(ref Vector2 point, float radius) {
        float min = radius;
        float max = radius * 2.0f;
        // float min = (point.Z + radius) / 2;
        // float max = (point.Z + radius + radius) / 2;

        float distance = ((float)_random.NextDouble() * (max - min)) + min;
        float angle = (float)_random.NextDouble() * Mathf.Tau;

        return new Vector2(
            point.X + ((float)Math.Cos(angle) * distance),
            point.Y + ((float)Math.Sin(angle) * distance));
    }
}