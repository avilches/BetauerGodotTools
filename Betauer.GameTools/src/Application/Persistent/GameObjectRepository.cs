using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Betauer.Application.Persistent;

public abstract class GameObjectRepository {

    private int _lastId = 0;
    private readonly Dictionary<int, GameObject> _registry = new();
    private readonly Dictionary<string, GameObject> _alias = new();

    public void Clear() {
        _lastId = 0;
        _registry.Clear();
        _alias.Clear();
    }

    public TItem Create<TItem>(string name, string? alias = null) where TItem : GameObject {
        TItem item = Activator.CreateInstance<TItem>();
        item.Name = name;
        item.Alias = alias;
        item.Id = ++_lastId;
        return Add(item);
    }

    private GameObject Create(SaveObject saveObject) {
        GameObject item = (GameObject)Activator.CreateInstance(saveObject.GameObjectType)!;
        item.Name = saveObject.Name;
        item.Alias = saveObject.Alias;
        item.Id = saveObject.Id;
        _lastId = Math.Max(_lastId, item.Id);
        return Add(item);
    }

    public GameObject Get(int id) => _registry[id];

    public GameObject? GetOrNull(int id) => _registry.TryGetValue(id, out var r) ? r : null;

    public TItem Get<TItem>(int id) where TItem : GameObject => (TItem)_registry[id];

    public TItem? GetOrNull<TItem>(int id) where TItem : GameObject => _registry.TryGetValue(id, out var r) ? r as TItem : null;

    public GameObject Get(string alias) => _alias[alias];

    public GameObject? GetOrNull(string alias) => _alias.TryGetValue(alias, out var r) ? r : null;

    public TItem Get<TItem>(string alias) where TItem : GameObject => (TItem)_alias[alias];

    public TItem? GetOrNull<TItem>(string alias) where TItem : GameObject => _alias.TryGetValue(alias, out var r) ? r as TItem : null;

    public void Remove(GameObject gameObject) {
        if (gameObject.Alias != null) _alias.Remove(gameObject.Alias);
        _registry.Remove(gameObject.Id);
        gameObject.OnRemove();
    }

    public void Remove(int id) {
        Remove(_registry[id]);
    }

    private TItem Add<TItem>(TItem item) where TItem : GameObject {
        _registry.Add(item.Id, item);
        if (item.Alias != null) _alias.Add(item.Alias, item);
        return item;
    }

    public async Task Save(string saveName) {
        var fileName = AppTools.GetUserFile(saveName);
        var saveObjects = _registry.Values.Select(g => g.CreateSaveObject());
        await using FileStream createStream = File.Create(fileName);
        await SaveObjects(createStream, saveObjects);
    }

    public async Task<Dictionary<int, SaveObject>> Load(string saveName) {
        var fileName = AppTools.GetUserFile(saveName);
        await using FileStream openStream = File.OpenRead(fileName);
        var objects = await LoadSaveObjects(openStream);
        Clear();

        var save = new Dictionary<int, SaveObject>();
        foreach (var saveObject in objects) {
            save[saveObject.Id] = saveObject;
            Create(saveObject);
        }
        return save;
    }

    public abstract Task SaveObjects(FileStream createStream, IEnumerable<SaveObject> saveObjects);

    public abstract Task<List<SaveObject>> LoadSaveObjects(FileStream openStream);
    
}