using System;
using System.Text.Json.Serialization;
using Betauer.Core;

namespace Betauer.Application.Persistent;

public interface ISaveObject {
    public string Discriminator();
}

public abstract class SaveObject : ISaveObject {
    [JsonInclude] public int Id { get; set; }
    [JsonInclude] public string Name { get; set; }
    [JsonInclude] public string? Alias { get; set; }

    // It needs to be a different name than "GameObject", because JsonSerialization fails with a duplicated attribute error
    protected virtual GameObject _gameObject { get; set; }

    public abstract string Discriminator();
    
    public void SetGameObject(GameObject gameObject) {
        _gameObject = gameObject;
    }

    protected SaveObject() {
    }

    protected SaveObject(GameObject gameObject) {
        Id = gameObject.Id;
        Name = gameObject.Name;
        Alias = gameObject.Alias;
    }
}

public abstract class SaveObject<T> : SaveObject where T : GameObject {
    [JsonIgnore] public T GameObject => (T)base._gameObject;

    protected SaveObject() {
    }

    protected SaveObject(GameObject gameObject) : base(gameObject) {
        if (gameObject.GetType() != typeof(T)) throw new Exception(
            $"Invalid type: SaveObject<{typeof(T).GetTypeName()}> is receiving a wrong type {gameObject.GetType().GetTypeName()}");
    }
}