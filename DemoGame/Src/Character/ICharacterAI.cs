namespace Veronenger.Character;

public interface ICharacterAI {
    public void Execute();
    public void EndFrame();
    public string GetState();
}