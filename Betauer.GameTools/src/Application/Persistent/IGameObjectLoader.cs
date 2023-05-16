using System.Collections.Generic;
using System.Threading.Tasks;

namespace Betauer.Application.Persistent;

public interface IGameObjectLoader<TSaveGame> where TSaveGame : SaveGame {
    public Task Save(TSaveGame saveGame, List<SaveObject> saveObjects, string saveName);
    public Task<List<TSaveGame>> ListSaveGames();
    public Task<TSaveGame> Load(string saveName);
    public Task<TSaveGame> LoadHeader(string saveName);
}