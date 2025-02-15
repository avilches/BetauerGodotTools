using System;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;
using Veronenger.Game.Dungeon.World;

namespace Veronenger.Tests;

[TestFixture]
public class TurnWorldEventTests {
    private TurnWorld _world;
    private Entity _entity;
    private int _worldEntityAddedCount;
    private int _worldEntityRemovedCount;
    private int _cellEntityAddedCount;
    private int _cellEntityRemovedCount;
    private int _entityWorldAddedCount;
    private int _entityWorldRemovedCount;
    private int _tickCount;
    private Vector2I _lastOldPosition;
    private Vector2I _lastNewPosition;

    [SetUp]
    public void Setup() {
        CellTypeConfig.DefaultConfig();
        _world = new TurnWorld(5, 5);
        _world.Cells.Load((p) => new WorldCell(CellType.Floor, p));
        _entity = EntityBuilder.Create("TestEntity", new EntityStats { BaseSpeed = 100 })
            .Build();

        ResetCounters();

        // Subscribe to World events
        _world.OnEntityAdded += (entity) => _worldEntityAddedCount++;
        _world.OnEntityRemoved += (entity) => _worldEntityRemovedCount++;
        _world.OnTick += (tick) => _tickCount++;
    }

    private void ResetCounters() {
        _worldEntityAddedCount = 0;
        _worldEntityRemovedCount = 0;
        _cellEntityAddedCount = 0;
        _cellEntityRemovedCount = 0;
        _entityWorldAddedCount = 0;
        _entityWorldRemovedCount = 0;
        _tickCount = 0;
        _lastOldPosition = Vector2I.Zero;
        _lastNewPosition = Vector2I.Zero;
    }

    [Test]
    public void AddEntity_ShouldTriggerCorrectEvents() {
        var position = new Vector2I(2, 2);
        var cell = _world[position];

        // Subscribe to cell events
        cell.OnEntityAdded += (entity) => _cellEntityAddedCount++;

        // Subscribe to entity events
        _entity.OnWorldAdded += () => _entityWorldAddedCount++;

        _world.AddEntity(_entity, position);

        Assert.Multiple(() => {
            Assert.That(_worldEntityAddedCount, Is.EqualTo(1), "World OnEntityAdded should be called once");
            Assert.That(_cellEntityAddedCount, Is.EqualTo(1), "Cell OnEntityAdded should be called once");
            Assert.That(_entityWorldAddedCount, Is.EqualTo(1), "Entity OnWorldAdded should be called once");
            Assert.That(_world[position].Entities, Contains.Item(_entity), "Entity should be in the cell");
            Assert.That(_entity.Location.Position, Is.EqualTo(position), "Entity should be at the correct position");
        });
    }

    [Test]
    public void RemoveEntity_ShouldTriggerCorrectEvents() {
        var position = new Vector2I(2, 2);
        var cell = _world[position];

        // Add entity first
        _world.AddEntity(_entity, position);

        // Subscribe to cell events
        cell.OnEntityRemoved += (entity) => _cellEntityRemovedCount++;

        // Subscribe to entity events
        _entity.OnWorldRemoved += () => _entityWorldRemovedCount++;

        ResetCounters(); // Reset counters after adding entity

        _world.RemoveEntity(_entity);

        Assert.Multiple(() => {
            Assert.That(_worldEntityRemovedCount, Is.EqualTo(1), "World OnEntityRemoved should be called once");
            Assert.That(_cellEntityRemovedCount, Is.EqualTo(1), "Cell OnEntityRemoved should be called once");
            Assert.That(_entityWorldRemovedCount, Is.EqualTo(1), "Entity OnWorldRemoved should be called once");
            Assert.That(_world[position].Entities, Is.Empty, "Cell should be empty");
            Assert.That(_entity.World, Is.Null, "Entity World reference should be null");
            Assert.That(_entity.Location, Is.Null, "Entity Location should be null");
        });
    }

    [Test]
    public void MoveEntity_ShouldTriggerCorrectCellEvents() {
        var startPosition = new Vector2I(1, 1);
        var endPosition = new Vector2I(2, 2);

        _world.AddEntity(_entity, startPosition);

        var oldCell = _world[startPosition];
        var newCell = _world[endPosition];

        // Subscribe to cell events
        oldCell.OnEntityRemoved += (entity) => _cellEntityRemovedCount++;
        newCell.OnEntityAdded += (entity) => _cellEntityAddedCount++;

        // Subscribe to location events
        _entity.OnMoved += (oldPos, newPos) => {
            _lastOldPosition = oldPos;
            _lastNewPosition = newPos;
        };

        ResetCounters();

        _entity.Location.Position = endPosition;

        Assert.Multiple(() => {
            Assert.That(_cellEntityRemovedCount, Is.EqualTo(1), "Old cell OnEntityRemoved should be called once");
            Assert.That(_cellEntityAddedCount, Is.EqualTo(1), "New cell OnEntityAdded should be called once");
            Assert.That(_lastOldPosition, Is.EqualTo(startPosition), "Old position should be correct");
            Assert.That(_lastNewPosition, Is.EqualTo(endPosition), "New position should be correct");
            Assert.That(_world[startPosition]!.Entities, Is.Empty, "Old cell should be empty");
            Assert.That(_world[endPosition]!.Entities, Contains.Item(_entity), "New cell should contain entity");
        });
    }

    [Test]
    public void AddEntity_ShouldThrowWhenEntityAlreadyInWorld() {
        _world.AddEntity(_entity, Vector2I.Zero);

        Assert.Throws<InvalidOperationException>(() => {
            _world.AddEntity(_entity, Vector2I.Zero);
        });
    }

    [Test]
    public void AddEntity_ShouldThrowWhenPositionInvalid() {
        var invalidPosition = new Vector2I(10, 10); // Outside of 5x5 world

        Assert.Throws<InvalidOperationException>(() => {
            _world.AddEntity(_entity, invalidPosition);
        });
    }

    [Test]
    public void ProcessTick_ShouldTriggerTickEvent() {
        var turnSystem = new TurnSystem(_world);

        Assert.That(_tickCount, Is.EqualTo(0), "Tick count should start at 0");

        // Process a few ticks
        for (var i = 0; i < 3; i++) {
            turnSystem.ProcessTickAsync().Wait();
        }

        Assert.That(_tickCount, Is.EqualTo(3), "Tick event should be called for each tick");
        Assert.That(_world.CurrentTick, Is.EqualTo(3), "Current tick should be updated");
    }

    [Test]
    public void GetEntity_ShouldReturnCorrectEntity() {
        _world.AddEntity(_entity, Vector2I.Zero);

        var retrievedEntity = _world.GetEntity(_entity.Name);
        var typedEntity = _world.GetEntity<Entity>(_entity.Name);

        Assert.Multiple(() => {
            Assert.That(retrievedEntity, Is.EqualTo(_entity), "GetEntity should return the correct entity");
            Assert.That(typedEntity, Is.EqualTo(_entity), "GetEntity<T> should return the correct entity");
        });
    }
}