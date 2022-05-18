using Betauer.Application;
using Godot;

namespace Betauer.DI {

    public abstract class Di {
        protected Di() => DefaultContainer.Container.InjectAllFields(this);
    }

    public abstract class DiNode : Node {
        protected DiNode() => DefaultContainer.Container.InjectAllFields(this);

        public sealed override void _Ready() {
            DefaultContainer.Container.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }

    public abstract class DiCanvasLayer : CanvasLayer {
        protected DiCanvasLayer() => DefaultContainer.Container.InjectAllFields(this);

        public sealed override void _Ready() {
            DefaultContainer.Container.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }

    public abstract class DiSprite : Sprite {
        protected DiSprite() => DefaultContainer.Container.InjectAllFields(this);

        public sealed override void _Ready() {
            DefaultContainer.Container.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }

    public abstract class DiControl : Control {
        protected DiControl() => DefaultContainer.Container.InjectAllFields(this);

        public sealed override void _Ready() {
            DefaultContainer.Container.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }
    
    public abstract class DiHBoxContainer : HBoxContainer {
        protected DiHBoxContainer() => DefaultContainer.Container.InjectAllFields(this);

        public sealed override void _Ready() {
            DefaultContainer.Container.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }
    
    public abstract class DiVBoxContainer : HBoxContainer {
        protected DiVBoxContainer() => DefaultContainer.Container.InjectAllFields(this);

        public sealed override void _Ready() {
            DefaultContainer.Container.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }

    public abstract class DiButton : Button {
        protected DiButton() => DefaultContainer.Container.InjectAllFields(this);

        public sealed override void _Ready() {
            DefaultContainer.Container.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }

    public abstract class DiNode2D : Node2D {
        protected DiNode2D() => DefaultContainer.Container.InjectAllFields(this);

        public sealed override void _Ready() {
            DefaultContainer.Container.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }

    public abstract class DiKinematicBody2D : KinematicBody2D {
        protected DiKinematicBody2D() => DefaultContainer.Container.InjectAllFields(this);

        public sealed override void _Ready() {
            DefaultContainer.Container.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }

    public abstract class DiCamera2D : Camera2D {
        protected DiCamera2D() => DefaultContainer.Container.InjectAllFields(this);

        public sealed override void _Ready() {
            DefaultContainer.Container.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }

    public abstract class DiArea2D : Area2D {
        protected DiArea2D() => DefaultContainer.Container.InjectAllFields(this);

        public sealed override void _Ready() {
            DefaultContainer.Container.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }

}