using System;
using System.Collections.Generic;
using System.Text;
using Godot;

namespace Betauer.Core.PCG.GridTools;

public enum DecayFunction {
    Linear,
    Inverse,
    Exponential
}

public class HeatmapGrid(GridGraph graph) {
    private readonly Dictionary<Vector2I, float> _heatmap = new();
    private readonly List<(Vector2I pos, float intensity, float radius)> _heatSources = new();

    public float BaseHeat { get; set; } = 0f;
    public DecayFunction DecayFunction = DecayFunction.Exponential;

    public void AddHeatSource(Vector2I pos, float intensity, float radius) {
        _heatSources.Add((pos, intensity, radius));
    }

    private float CalculateHeat(float intensity, float distance, float radius) {
        if (distance > radius) return BaseHeat;

        var normalizedDistance = distance / radius; // 0 to 1
        return DecayFunction switch {
            DecayFunction.Linear => intensity * (1 - normalizedDistance),
            DecayFunction.Inverse => intensity / (1 + normalizedDistance),
            DecayFunction.Exponential => intensity * MathF.Exp(-2f * normalizedDistance),
            _ => intensity / (1 + normalizedDistance) // Default case also updated
        };
    }

    private void PropagateHeat(Vector2I start, float intensity, float radius) {
        var visited = new HashSet<Vector2I>();
        var distances = new Dictionary<Vector2I, float>();
        var queue = new PriorityQueue<Vector2I, float>();

        distances[start] = 0;
        queue.Enqueue(start, 0);

        while (queue.Count > 0) {
            var current = queue.Dequeue();
            if (!visited.Add(current)) continue;

            var currentDistance = distances[current];
            // Si la distancia es mayor que el radio, no seguimos propagando desde este punto
            if (currentDistance > radius) continue;

            // Calcular y acumular calor basado en la distancia
            var heat = CalculateHeat(intensity, currentDistance, radius);
            _heatmap[current] = Math.Max(_heatmap.GetValueOrDefault(current), heat);

            foreach (var (_, neighbor, weight) in graph.Adjacent(current)) {
                if (visited.Contains(neighbor)) continue;

                var newDistance = currentDistance + weight;
                if (!distances.TryGetValue(neighbor, out var value) || newDistance < value) {
                    distances[neighbor] = newDistance;
                    queue.Enqueue(neighbor, newDistance);
                }
            }
        }
    }

    public void UpdateHeatmap() {
        _heatmap.Clear();

        // First set base heat for all cells
        if (BaseHeat > 0) {
            for (int y = 0; y < graph.Bounds.Size.Y; y++) {
                for (int x = 0; x < graph.Bounds.Size.X; x++) {
                    var pos = new Vector2I(x, y);
                    _heatmap[pos] = BaseHeat;
                }
            }
        }

        // Then propagate heat from each source
        foreach (var source in _heatSources) {
            PropagateHeat(source.pos, source.intensity, source.radius);
        }
    }

    public Vector2I? GetBestDirection(Vector2I pos) {
        float bestHeat = float.MinValue;
        Vector2I? bestDir = null;

        foreach (var edge in graph.Adjacent(pos)) {
            var neighborHeat = _heatmap.GetValueOrDefault(edge.To);
            if (neighborHeat > bestHeat) {
                bestHeat = neighborHeat;
                bestDir = edge.To;
            }
        }

        return bestDir;
    }

    public float GetHeat(Vector2I pos) {
        return _heatmap.GetValueOrDefault(pos);
    }

    public string PrintHeatmap() {
        var sb = new StringBuilder();

        // Find the maximum intensity from all sources for normalization
        float maxIntensity = 0f;
        foreach (var source in _heatSources) {
            maxIntensity = Math.Max(maxIntensity, source.intensity);
        }

        // If no sources, use 1.0f as default max to avoid division by zero
        maxIntensity = Math.Max(maxIntensity, 1.0f);

        // Print the heatmap
        for (int y = 0; y < graph.Bounds.Size.Y; y++) {
            for (int x = 0; x < graph.Bounds.Size.X; x++) {
                var pos = new Vector2I(x, y);
                var heat = _heatmap.GetValueOrDefault(pos, -1);
                if (heat < 0f) {
                    sb.Append("· ");
                    continue;
                }
                // Normalize heat to 0-9 range based on maximum possible intensity
                var value = (int)Math.Floor(heat * 9 / maxIntensity);
                sb.Append(value);
                sb.Append(' ');
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }
}