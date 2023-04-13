using Godot;

namespace Betauer.Core.Signal;

// TODO: this class is not tested

public abstract class EnterExitHandler {
    public bool Inside { get; protected set; } = false;
    public bool Connected { get; private set; }

    public void Connect() {
        if (!Connected) {
            DoConnect();
            Connected = true;
        }
    }

    public void Disconnect() {
        if (Connected) {
            DoDisconnect();
            Connected = false;
        }
    }

    protected void Entered() => Inside = true;

    protected void Exited() => Inside = true;

    protected abstract void DoConnect();

    protected abstract void DoDisconnect();
}

public static class IsVisibleOnScreenNotifier2D {
    public sealed class InsideScreen : EnterExitHandler {
        public readonly VisibleOnScreenNotifier2D Origin;

        public InsideScreen(VisibleOnScreenNotifier2D origin) {
            Origin = origin;
            Connect();
        }

        protected override void DoConnect() {
            Origin.ScreenEntered += Entered;
            Origin.ScreenExited += Exited;
        }

        protected override void DoDisconnect() {
            Origin.ScreenEntered -= Entered;
            Origin.ScreenExited -= Exited;
        }
    }
}

public static class Mouse {
    public sealed class InsideControl : EnterExitHandler {
        public readonly Control Origin;

        public InsideControl(Control origin) {
            Origin = origin;
            Connect();
        }

        protected override void DoConnect() {
            Origin.MouseEntered += Entered;
            Origin.MouseExited += Exited;
        }

        protected override void DoDisconnect() {
            Origin.MouseEntered -= Entered;
            Origin.MouseExited -= Exited;
        }
    }

    public class InsideWindow : EnterExitHandler {
        public readonly Window Origin;

        public InsideWindow(Window origin) {
            Origin = origin;
            Connect();
        }

        protected override void DoConnect() {
            Origin.MouseEntered += Entered;
            Origin.MouseExited += Exited;
        }

        protected override void DoDisconnect() {
            Origin.MouseEntered -= Entered;
            Origin.MouseExited -= Exited;
        }
    }

    public class InsideCollisionObject2D : EnterExitHandler {
        public readonly CollisionObject2D Origin;

        public InsideCollisionObject2D(CollisionObject2D origin) {
            Origin = origin;
        }

        protected override void DoConnect() {
            Origin.MouseEntered += Entered;
            Origin.MouseExited += Exited;
        }

        protected override void DoDisconnect() {
            Origin.MouseEntered -= Entered;
            Origin.MouseExited -= Exited;
        }
    }
}