using System.Collections.Generic;
using System.Threading.Tasks;

namespace Betauer.Application.Persistent;

public interface IGameObjectLoader {

    public Task Save(List<SaveObject> saveObjects, string saveName);

    public Task<List<SaveObject>> Load(string saveName);
}