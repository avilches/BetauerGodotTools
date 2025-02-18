using System;
using System.Collections.Generic;
using Godot;
using Veronenger.Game.Dungeon.World.Generation;

namespace Veronenger.Game.Dungeon.World;

public class WorldCell {
    private static readonly IReadOnlyList<EntityBase> EmptyEntitiesList = Array.Empty<EntityBase>().AsReadOnly();

    private List<EntityBase>? _entities;
    private IReadOnlyList<EntityBase>? _readOnlyEntities;

    public Vector2I Position { get; }
    public CellType Type { get; set; }
    public CellTypeConfig Config { get; private set; }
    public CellDefinitionConfig CellDefinitionConfig { get; set; }

    public event Action<EntityBase>? OnEntityAdded;
    public event Action<EntityBase>? OnEntityRemoved;

    public WorldCell(CellType type, Vector2I position) {
        Position = position;
        Type = type;
        Config = CellTypeConfig.Get(Type);
    }

    public IReadOnlyList<EntityBase> Entities => _entities == null
        ? EmptyEntitiesList
        : _readOnlyEntities ??= _entities.AsReadOnly();

    internal void AddEntity(EntityBase entity) {
        _entities ??= [];
        if (_entities.Contains(entity)) return;
        _entities.Add(entity);
        OnEntityAdded?.Invoke(entity);
    }

    internal bool RemoveEntity(EntityBase entity) {
        if (_entities == null) return false;
        if (!_entities.Remove(entity)) return false;
        OnEntityRemoved?.Invoke(entity);
        return true;
    }
}