using System;
using Betauer.DI;
using Godot;
using Container = Betauer.DI.Container;

namespace Betauer.Application {
    /**
     * A Container that listen for nodes added to the tree and inject services inside of them + process the OnReady tag
     */
    public class GodotContainer : Node {
        private Container _container;

        public void SetContainer(Container container) {
            _container = container;
        }

        public void CreateAndScan() {
            var builder = new ContainerBuilder(this);
            builder.Static<Func<SceneTree>>(GetTree);
            builder.Scan();
            SetContainer(builder.Build());
        }

        public override void _EnterTree() {
            EnableInjection();
        }

        public void EnableInjection() {
            if (!GetTree().IsConnected(GodotConstants.GODOT_SIGNAL_node_added, this, nameof(GodotExecuteNodeAdded)))
                GetTree().Connect(GodotConstants.GODOT_SIGNAL_node_added, this, nameof(GodotExecuteNodeAdded));
        }

        public void DisableInjection() {
            if (GetTree().IsConnected(GodotConstants.GODOT_SIGNAL_node_added, this, nameof(GodotExecuteNodeAdded)))
                GetTree().Disconnect(GodotConstants.GODOT_SIGNAL_node_added, this, nameof(GodotExecuteNodeAdded));
        }

        private const string MetaInjected = "__injected";

        public void GodotExecuteNodeAdded(Node node) {
            if (node.GetScript() is CSharpScript script) {
                _container.LoadOnReadyNodes(node);
                if (!node.HasMeta(MetaInjected)) {
                    _container.InjectAllFields(node);
                    node.SetMeta(MetaInjected, true);
                }
            }
        }
    }
}