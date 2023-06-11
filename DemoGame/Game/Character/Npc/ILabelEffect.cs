using Betauer.Core.Pool.Lifecycle;

namespace Veronenger.Game.Character.Npc;

public interface ILabelEffect : IPoolLifecycle {
    public void Show(string text);
}