using System.Collections.Generic;
using System.Threading.Tasks;

namespace Betauer.Application.Persistent;

public interface IGameObjectLoader<TSaveGame> where TSaveGame : SaveGame {
    public Task Save(TSaveGame savegame, List<SaveObject> saveObjects, string saveName);

    public Task<TSaveGame> Load(string saveName);
}