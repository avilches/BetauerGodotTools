namespace Veronenger.Character;

public class DoNothingAI : ICharacterAI {
    public static readonly DoNothingAI Instance = new();

    private DoNothingAI() {
    }

    public void Handle(double delta) {
    }

    public void EndFrame() {
    }

    public string GetState() => "DoNothing";
}