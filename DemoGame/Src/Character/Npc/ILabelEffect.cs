using Betauer.Core.Pool;

namespace Veronenger.Character.Npc;

public interface ILabelEffect : IPoolElement {
    public void Show(string text);
}