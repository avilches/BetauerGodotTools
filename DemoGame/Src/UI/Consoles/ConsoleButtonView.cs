namespace Veronenger.UI.Consoles;

public class ConsoleButtonView {
    public readonly int Frame;
    public readonly int FramePressed;
    public readonly string? Animation;

    public ConsoleButtonView(string? animation, int frame, int framePressed) {
        Animation = animation;
        Frame = frame;
        FramePressed = framePressed;
    }
}