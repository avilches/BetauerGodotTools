namespace Veronenger.Game.Platform.Character.Npc;

public class DoNothingAI : ICharacterAi {
    public static readonly DoNothingAI Instance = new();

    private DoNothingAI() {
    }

    public void Execute() {
    }

    public void EndFrame() {
    }

    public void Reset() {
    }

    public string GetState() => "DoNothing";
}