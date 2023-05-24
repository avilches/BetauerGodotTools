using System;

namespace Betauer.Application.Lifecycle;

public class ResourceProgress {
    public float TotalPercent { get; private set; }
    public float ResourcePercent { get; private set; }
    public string? Resource { get; private set; }

    private readonly Action<ResourceProgress>? _action;

    public ResourceProgress(Action<ResourceProgress>? action) {
        _action = action;
    }

    internal void Update(float totalPercent, float resourcePercent, string? resource) {
        TotalPercent = totalPercent;
        ResourcePercent = resourcePercent;
        Resource = resource;
        _action?.Invoke(this);
    }
}