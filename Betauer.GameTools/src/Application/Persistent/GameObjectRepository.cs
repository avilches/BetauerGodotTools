using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;
using Betauer.DI;
using Betauer.DI.Attributes;

namespace Betauer.Application.Persistent;

public class GameObjectRepository {
    [Inject] protected Container Container { get; set; }

    private int _lastId = 0;
    private readonly Dictionary<int, GameObject> _registry = new();
    private readonly Dictionary<string, GameObject> _alias = new();

    public void Initialize(List<SaveObject>? objects = null) {
        _lastId = 0;
        _registry.Clear();
        _alias.Clear();
        objects?.ForEach(saveObject => {
            saveObject.GameObjectRepository = this;
            CreateFrom(saveObject);
        });
    }

    public List<SaveObject> GetSaveObjects() {
        return _registry.Values.Select(g => g.CreateSaveObject()).ToList();
    }

    public T Create<T>(string name, string? alias = null) where T : GameObject {
        T gameObject = Activator.CreateInstance<T>();
        gameObject.Name = name;
        gameObject.Alias = alias;
        gameObject.Id = ++_lastId;
        return Add(gameObject);
    }

    public GameObject CreateFrom(SaveObject saveObject) {
        GameObject item = (GameObject)Activator.CreateInstance(saveObject.GameObjectType)!;
        item.Name = saveObject.Name;
        item.Alias = saveObject.Alias;
        item.Id = saveObject.Id;
        _lastId = Math.Max(_lastId, item.Id);
        return Add(item);
    }

    public T CreateFrom<T>(SaveObject<T> saveObject) where T : GameObject {
        if (saveObject.GameObjectType != typeof(T))
            throw new Exception($"Invalid type: Create<{typeof(T).GetTypeName()}> is receiving a wrong type {saveObject.GameObjectType.GetTypeName()}");
        return (T)CreateFrom((SaveObject)saveObject);
    }

    public GameObject Get(int id) => _registry[id];

    public GameObject? GetOrNull(int id) => _registry.TryGetValue(id, out var r) ? r : null;

    public T Get<T>(int id) where T : GameObject => (T)_registry[id];

    public T? GetOrNull<T>(int id) where T : GameObject => _registry.TryGetValue(id, out var r) ? r as T : null;

    public GameObject Get(string alias) => _alias[alias];

    public GameObject? GetOrNull(string alias) => _alias.TryGetValue(alias, out var r) ? r : null;

    public T Get<T>(string alias) where T : GameObject => (T)_alias[alias];

    public T? GetOrNull<T>(string alias) where T : GameObject => _alias.TryGetValue(alias, out var r) ? r as T : null;

    public void Remove(GameObject gameObject) {
        if (gameObject.Alias != null) _alias.Remove(gameObject.Alias);
        _registry.Remove(gameObject.Id);
        gameObject.OnRemove();
    }

    public void Remove(int id) {
        Remove(_registry[id]);
    }

    private T Add<T>(T gameObject) where T : GameObject {
        _registry.Add(gameObject.Id, gameObject);
        if (gameObject.Alias != null) _alias.Add(gameObject.Alias, gameObject);
        Container.InjectServices(gameObject);
        return gameObject;
    }
}