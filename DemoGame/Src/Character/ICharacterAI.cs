namespace Veronenger.Character;

public interface ICharacterAI {
    public void Handle(double delta);
    public void EndFrame();
    public string GetState();
}