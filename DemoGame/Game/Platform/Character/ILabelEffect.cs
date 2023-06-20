using Betauer.Core.Pool.Lifecycle;

namespace Veronenger.Game.Platform.Character;

public interface ILabelEffect : IPoolLifecycle {
    public void Show(string text);
}