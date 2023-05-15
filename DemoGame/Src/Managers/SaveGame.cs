using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Application.Persistent;
using Betauer.Core;
using Veronenger.Persistent;

namespace Veronenger.Managers;

public class SaveGame {
    public List<SaveObject> SaveObjects { get; }
    public List<SaveObject> Pending { get; }
    private GameObjectRepository GameObjectRepository { get; }

    public PlayerSaveObject Player0 { get; private set; }
    public PlayerSaveObject Player1 { get; private set; }
    // public PlayerSaveObject Player2 { get; }

    public SaveGame(GameObjectRepository gameObjectRepository, List<SaveObject> saveObjects) {
        GameObjectRepository = gameObjectRepository;
        SaveObjects = saveObjects;
        Pending = new List<SaveObject>(saveObjects);
        ConsumePlayers();
    }

    public void Consume(SaveObject saveObject) => Pending.Remove(saveObject);

    public void ForEach<T>(Action<T> action) where T : SaveObject {
        Pending.OfType<T>().ToArray().ForEach(action);
    }

    public void Consume<T>(Action<T> action) where T : SaveObject {
        ForEach<T>(saveObject => {
            action(saveObject);
            Consume(saveObject);
        });
    }

    public void ConsumePlayers() {
        Consume<PlayerSaveObject>(saveObject => {
            if (saveObject.Name == "Player0") Player0 = saveObject;
            else if (saveObject.Name == "Player1") Player1 = saveObject;
            // else if (saveObject.Name == "Player2") Player2 = saveObject;
            else throw new Exception("Unknown player in the savegame: " + saveObject.Name);
        });
    }
}