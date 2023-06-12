using System.Threading.Tasks;
using Godot;

namespace Veronenger.Game;

public interface IGameView {
	public Task StartNewGame();
	public Task LoadFromMenu(string saveName);
	public Task LoadInGame(string saveName);
	public Task Save(string saveName);
	public Task End(bool unload);
	public Node GetWorld();
}
