namespace Veronenger.Character.Handler;

using Betauer.Input;

public interface ICharacterHandler {
    public IDirectional Directional { get; }
    public IAction JumpAction { get; }
    public IAction AttackAction { get; }
    public IAction FloatAction { get; }

    public void EndFrame();
}