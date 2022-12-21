namespace Veronenger.Character.Handler;

using Betauer.Input;

public interface ICharacterHandler {
    public IDirectional Directional { get; }
    public IAction Jump { get; }
    public IAction Attack { get; }
    public IAction Float { get; }

    public void EndFrame();
}