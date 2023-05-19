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

    public void Initialize() {
        _lastId = 0;
        _registry.Clear();
        _alias.Clear();
    }
    
    /// <summary>
    /// Mutates the saveObjects list, injecting the new created GameObject property in each saveObject 
    /// </summary>
    /// <param name="saveObjects"></param>
    public void LoadSaveObjects(List<SaveObject> saveObjects) {
        // First, create and add all gameobjects
        var gameObjects = saveObjects.Select(saveObject => {
            GameObject gameObject = CreateFromSaveObject(saveObject);
            return (gameObject, saveObject);
        }).ToList();
        // Then, call OnLoad for each gameobject, so they can access to the repository and load other gameobjects
        foreach (var (gameObject, saveObject) in gameObjects) {
            gameObject.OnLoad(saveObject);
        }
    }

    public GameObject Load(SaveObject saveObject) {
        var gameObject = CreateFromSaveObject(saveObject);
        gameObject.OnLoad(saveObject);
        return gameObject;
    }

    public List<SaveObject> GetSaveObjects() {
        return _registry.Values.Select(g => g.CreateSaveObject()).ToList();
    }

    public T Create<T>(string name, string? alias = null) where T : GameObject {
        T gameObject = Activator.CreateInstance<T>();
        gameObject.Name = name;
        gameObject.Alias = alias;
        gameObject.Id = ++_lastId;
        gameObject.GameObjectRepository = this;
        
        Container.InjectServices(gameObject);
        Index(gameObject);
        gameObject.OnInitialize();
        return gameObject;
    }

    private GameObject CreateFromSaveObject(SaveObject saveObject) {
        if (!saveObject.GetType().IsGenericSubclassOf(typeof(SaveObject<>)))
            throw new Exception(
                $"Invalid type: {saveObject.GetType().GetTypeName()} is not a subclass of {typeof(SaveObject<>).GetTypeName()}");
        var genericType = saveObject.GetType().FindGenericsFromBaseTypeDefinition(typeof(SaveObject<>))[0];
        GameObject gameObject = (GameObject)Activator.CreateInstance(genericType)!;
        gameObject.Name = saveObject.Name;
        gameObject.Alias = saveObject.Alias;
        gameObject.Id = saveObject.Id;
        gameObject.GameObjectRepository = this;
        _lastId = Math.Max(_lastId, gameObject.Id);

        saveObject.SetGameObject(gameObject);
        Container.InjectServices(gameObject);
        return Index(gameObject);
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

    private T Index<T>(T gameObject) where T : GameObject {
        _registry.Add(gameObject.Id, gameObject);
        if (gameObject.Alias != null) _alias.Add(gameObject.Alias, gameObject);
        return gameObject;
    }
}