namespace Veronenger.Platform.Character;

public interface ICharacterAi {
    public void Execute();
    public void EndFrame();
    public string GetState();
    public void Reset(); // Shared with IFSM
}