using System.Collections.Generic;

namespace Betauer.Application.Persistent; 

public class SaveGame<TMetadata> where TMetadata : Metadata {
    public TMetadata Metadata { get; init; }
    public List<SaveObject> GameObjects { get; init; }

    public SaveGame(TMetadata metadata, List<SaveObject> gameObjects) {
        Metadata = metadata;
        GameObjects = gameObjects;
    }

    public SaveGame() {
    }
}