using System;
using System.Collections.Generic;

namespace Veronenger.Game.Dungeon.World;

public class WorldCell {
    private static readonly IReadOnlyList<Entity> EmptyList = Array.Empty<Entity>().AsReadOnly();

    private List<Entity>? _entities;
    private IReadOnlyList<Entity>? _readOnlyEntities;

    public event Action<Entity>? OnEntityAdded;
    public event Action<Entity>? OnEntityRemoved;

    public IReadOnlyList<Entity> Entities => _entities == null
        ? EmptyList
        : _readOnlyEntities ??= _entities.AsReadOnly();


    internal void AddEntity(Entity entity) {
        _entities ??= [];
        if (_entities.Contains(entity)) return;
        _entities.Add(entity);
        OnEntityAdded?.Invoke(entity);
    }

    internal bool RemoveEntity(Entity entity) {
        if (_entities == null) return false;
        if (!_entities.Remove(entity)) return false;
        OnEntityRemoved?.Invoke(entity);
        return true;
    }
}