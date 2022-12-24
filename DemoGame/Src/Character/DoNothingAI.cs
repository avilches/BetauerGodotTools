namespace Veronenger.Character;

public class DoNothingAI : ICharacterAI {
    public static readonly DoNothingAI Instance = new();

    private DoNothingAI() {
    }

    public void Execute() {
    }

    public void EndFrame() {
    }

    public string GetState() => "DoNothing";
}