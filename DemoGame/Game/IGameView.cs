using System.Threading.Tasks;

namespace Veronenger.Game;

public interface IGameView {
	public Task StartNewGame(string? saveName = null);
	public Task End(bool unload);
}
