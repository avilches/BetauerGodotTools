using Godot;

namespace Betauer.DI {

    public abstract class Di {
        protected Di() => Bootstrap.Container.InjectAllFields(this);
    }

    public abstract class DiNode : Node {
        protected DiNode() => Bootstrap.Container.InjectAllFields(this);

        public sealed override void _Ready() {
            Bootstrap.Container.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }

    public abstract class DiControl : Control {
        protected DiControl() => Bootstrap.Container.InjectAllFields(this);

        public sealed override void _Ready() {
            Bootstrap.Container.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }

    public abstract class DiButton : Button {
        protected DiButton() => Bootstrap.Container.InjectAllFields(this);

        public sealed override void _Ready() {
            Bootstrap.Container.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }

    public abstract class DiNode2D : Node2D {
        protected DiNode2D() => Bootstrap.Container.InjectAllFields(this);

        public sealed override void _Ready() {
            Bootstrap.Container.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }

    public abstract class DiKinematicBody2D : KinematicBody2D {
        protected DiKinematicBody2D() => Bootstrap.Container.InjectAllFields(this);

        public sealed override void _Ready() {
            Bootstrap.Container.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }

    public abstract class DiCamera2D : Camera2D {
        protected DiCamera2D() => Bootstrap.Container.InjectAllFields(this);

        public sealed override void _Ready() {
            Bootstrap.Container.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }

    public abstract class DiArea2D : Area2D {
        protected DiArea2D() => Bootstrap.Container.InjectAllFields(this);

        public sealed override void _Ready() {
            Bootstrap.Container.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }

    public class RootSceneHolder : Node {
        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
        }
    }
}