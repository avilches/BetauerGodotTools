using System.Collections.Generic;
using System.Linq;
using Betauer.DI;
using Betauer.Input;
using Godot;

namespace Betauer.Application.Monitor {
    public class DebugOverlayManager : CanvasLayer {

        [Inject(Nullable = true)]
        public InputAction? DebugOverlayAction { get; set; }

        private int _count = 0;
        private int _current = -1;

        private HashSet<int> _actives = new();
        private HashSet<int> _preSolo = new();
        private bool _isSolo = false;

        public override void _Ready() {
            Layer = 1000000;
            PauseMode = PauseModeEnum.Process;
            Visible = false;
        }

        public DebugOverlay Overlay(Object target) {
            return (GetChildren()
                       .OfType<DebugOverlay>()
                       .FirstOrDefault(d => d.Target == target)
                   ?? CreateOverlay().RemoveIfInvalid(target)).Enable(VisibleCount >= 1);
        }

        public DebugOverlay Follow(Node2D follow) {
            return Overlay(follow).Follow(follow);
        }

        public DebugOverlay CreateOverlay() {
            var overlay = new DebugOverlay(this, _count++);
            AddChild(overlay);
            _actives.Add(overlay.Id);
            return overlay;
        }

        public int VisibleCount =>
            GetChildren().OfType<DebugOverlay>().Count(debugOverlay => debugOverlay.Visible);

        public override void _Input(InputEvent @event) {
            if (DebugOverlayAction != null && DebugOverlayAction.IsEventPressed(@event)) {
                if (Visible) Disable(); else Enable();
            }
        }

        public DebugOverlayManager Enable(bool enable = true) {
            if (enable) {
                Visible = true;
                GetChildren().OfType<DebugOverlay>()
                    .ForEach(overlay => overlay.Enable(_actives.Contains(overlay.Id)));
            } else {
                Visible = false;
                _actives = GetChildren().OfType<DebugOverlay>()
                    .Where(overlay => overlay.Visible)
                    .Select(overlay => overlay.Id)
                    .ToHashSet();
                GetChildren().OfType<DebugOverlay>().ForEach(overlay => overlay.Disable());
            }
            return this;
        }

        public DebugOverlayManager Disable() {
            return Enable(false);
        }

        public void All() {
            _isSolo = false;
            GetChildren().OfType<DebugOverlay>().ForEach(overlay => overlay.Enable());
        }

        public void Solo(int id) {
            if (_isSolo) {
                GetChildren().OfType<DebugOverlay>()
                    .ForEach(overlay => overlay.Enable(_preSolo.Contains(overlay.Id)));
            } else {
                _preSolo = GetChildren().OfType<DebugOverlay>()
                    .Where(overlay => overlay.Visible)
                    .Select(overlay => overlay.Id)
                    .ToHashSet();
                GetChildren().OfType<DebugOverlay>().ForEach(overlay => overlay.Enable(overlay.Id == id));
            }
            _isSolo = !_isSolo;
        }

        public DebugOverlay Find(int id) => 
            GetChildren().OfType<DebugOverlay>().First(overlay => overlay.Id == id);

        public void Mute(int id) {
            var visibleWithButtons = GetChildren().OfType<DebugOverlay>()
                .Count(debugOverlay => debugOverlay.Visible && debugOverlay.TopBar.Visible);
            if (visibleWithButtons > 1) Find(id).Disable(); else Disable();
        }
    }
}