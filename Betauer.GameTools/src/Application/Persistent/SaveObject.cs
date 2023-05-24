using System;
using System.Text.Json.Serialization;
using Betauer.Core;

namespace Betauer.Application.Persistent;

public abstract class SaveObject {
    [JsonInclude] public int Id { get; set; }
    [JsonInclude] public string Name { get; set; }
    [JsonInclude] public string? Alias { get; set; }
    [JsonIgnore] public virtual GameObject GameObject { get; set; }
    public abstract string Discriminator();
    
    public void SetGameObject(GameObject gameObject) {
        GameObject = gameObject;
    }

    protected SaveObject() {
    }

    protected SaveObject(GameObject gameObject) {
        Id = gameObject.Id;
        Name = gameObject.Name;
        Alias = gameObject.Alias;
    }

    public abstract int Hash();
}

public abstract class SaveObject<T> : SaveObject where T : GameObject {
    [JsonIgnore] public T GameObject => (T)base.GameObject;

    protected SaveObject() {
    }

    protected SaveObject(GameObject gameObject) : base(gameObject) {
        if (gameObject.GetType() != typeof(T)) throw new Exception(
            $"Invalid type: SaveObject<{typeof(T).GetTypeName()}> is receiving a wrong type {gameObject.GetType().GetTypeName()}");
    }
}