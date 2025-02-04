using System;
using System.Collections.Generic;
using Godot;

namespace Veronenger.Game.Dungeon.World;

public class WorldCell(CellType type, Vector2I position) {
    private static readonly IReadOnlyList<Entity> EmptyList = Array.Empty<Entity>().AsReadOnly();

    private List<Entity>? _entities;
    private IReadOnlyList<Entity>? _readOnlyEntities;

    public Vector2I Position { get; } = position;
    public CellType Type { get; set; } = type;
    public CellTypeConfig Config => CellTypeConfig.Get(Type);

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