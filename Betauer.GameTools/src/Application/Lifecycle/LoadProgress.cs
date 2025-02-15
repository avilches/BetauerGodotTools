using System;

namespace Betauer.Application.Lifecycle;

public class LoadProgress(Action<LoadProgress>? action) {
    public float TotalPercent { get; private set; }
    public float ResourcePercent { get; private set; }
    public string? Resource { get; private set; }

    internal void Update(float totalPercent, float resourcePercent, string? resource) {
        TotalPercent = totalPercent;
        ResourcePercent = resourcePercent;
        Resource = resource;
        action?.Invoke(this);
    }
}