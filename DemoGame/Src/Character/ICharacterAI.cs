namespace Veronenger.Character;

public interface ICharacterAI {
    public void Execute();
    public void EndFrame();
    public string GetState();
    public void Reset(); // Shared with IStateMachine
}