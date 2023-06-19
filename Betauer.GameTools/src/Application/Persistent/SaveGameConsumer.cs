using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;

namespace Betauer.Application.Persistent;

public class SaveGameConsumer<TMetadata> where TMetadata : Metadata {
    public List<SaveObject> Pending { get; }
    public SaveGame<TMetadata> SaveGame { get; }

    public SaveGameConsumer(SaveGame<TMetadata> saveGame) {
        SaveGame = saveGame;
        Pending = new List<SaveObject>(SaveGame.GameObjects);
    }

    public void Consume<T>(T saveObject) where T : SaveObject => Pending.Remove(saveObject);

    public T Consume<T>() where T : SaveObject {
        var saveObject = Pending.OfType<T>().FirstOrDefault();
        if (saveObject == null) throw new Exception("Type not found: " + typeof(T).GetTypeName());
        Consume(saveObject);
        return saveObject;
    }

    public T ConsumeAlias<T>(string alias) where T : SaveObject {
        var saveObject = Pending.OfType<T>().FirstOrDefault(saveObject => saveObject.Name == alias);
        if (saveObject == null) throw new Exception("Alias not found: " + alias);
        Consume(saveObject);
        return saveObject;
    }

    public T? ConsumeAliasOrIgnore<T>(string alias) where T : SaveObject {
        var saveObject = Pending.OfType<T>().FirstOrDefault(saveObject => saveObject.Name == alias);
        if (saveObject == null) return null;
        Consume(saveObject);
        return saveObject;
    }


    public void ConsumeAll<T>(Action<T> action) where T : SaveObject {
        Pending.RemoveAll(s => {
            if (s is not T saveObject) return false;
            action(saveObject);
            return true;
        });
    }
    
    public void ConsumeWhere<T>(Predicate<T> predicate)  {
        Pending.RemoveAll(s => s is T saveObject && predicate(saveObject));
    }

    public void Verify() {
        if (Pending.Count > 0) {
            throw new Exception($"Not all save objects were consumed: {Pending.Count} left"); 
        }
    }
}