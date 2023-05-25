using System.Collections.Generic;

namespace Betauer.Application.Persistent; 

public class SaveGame<TMetadata> where TMetadata : Metadata {
    public TMetadata Metadata { get; }
    public List<SaveObject> GameObjects { get; }

    public SaveGame(TMetadata metadata, List<SaveObject> gameObjects) {
        Metadata = metadata;
        GameObjects = gameObjects;
    }
}