using Betauer.DI;
using Betauer.Memory;

namespace DemoAnimation.Game.Managers.Autoload {
    [Singleton]
    public class ObjectLifeCycleManagerNode : Betauer.Memory.ObjectLifeCycleManagerNode {
        public ObjectLifeCycleManagerNode() : base(600, null) {
        }
    }
}