using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Betauer.DI;
using Betauer.Input;
using Betauer.UI;
using Godot;

namespace Betauer.Application.Monitor {
    public class DebugOverlayManager : CanvasLayer {

        private int _count = 0;
        private HashSet<int> _actives = new();
        private HashSet<int> _preSolo = new();
        private bool _isSolo = false;

        public readonly Control OverlayContainer = new() {
            Name = nameof(OverlayContainer)
        };
        public readonly DebugConsole DebugConsole;
        public IEnumerable<DebugOverlay> Overlays => OverlayContainer.GetChildren().OfType<DebugOverlay>().Where(IsInstanceValid);
        public int VisibleCount => Overlays.Count(debugOverlay => debugOverlay.Visible);
        public DebugOverlay Find(int id) => Overlays.First(overlay => overlay.Id == id);

        [Inject(Nullable = true)]
        public InputAction? DebugOverlayAction { get; set; }

        public DebugOverlayManager() {
            DebugConsole = new DebugConsole(this) {
                Name = nameof(DebugConsole),
                Visible = false
            };
        }

        public override void _Ready() {
            Name = "DebugOverlayManager";
            Layer = 1000000;
            PauseMode = PauseModeEnum.Process;
            Visible = false;
            this.NodeBuilder()
                .Child(OverlayContainer)
                .End()
                .Child(DebugConsole);
        }

        public DebugOverlay Overlay(string title) {
            return (Overlays.FirstOrDefault(d => d.TitleLabel.Text == title) ?? 
                    CreateOverlay().Title(title)).Enable(VisibleCount >= 1);
        }

        public DebugOverlay Overlay(Object target) {
            return (Overlays.FirstOrDefault(d => d.Target == target) ?? 
                    CreateOverlay().RemoveIfInvalid(target)).Enable(VisibleCount >= 1);
        }

        public DebugOverlay Follow(Node2D follow) {
            return Overlay(follow).Follow(follow);
        }

        public DebugOverlay CreateOverlay(string? title = null) {
            var overlay = new DebugOverlay(this, _count++).Title(title);
            OverlayContainer.AddChild(overlay);
            _actives.Add(overlay.Id);
            return overlay;
        }

        public override void _Input(InputEvent @event) {
            if (DebugOverlayAction != null && DebugOverlayAction.IsEventPressed(@event)) {
                if (@event.HasShift()) {
                    if (Visible) {
                        DebugConsole.Enable(!DebugConsole.Visible);
                    } else {
                        Enable();
                        DebugConsole.Enable();
                    }
                } else {
                    if (Visible) {
                        Disable();
                    } else {
                        Enable();
                    }
                }
            }
        }

        public DebugOverlayManager Enable(bool enable = true) {
            if (Visible == enable) return this;
            if (enable) {
                if (DebugConsole.Visible) DebugConsole.Enable();
                Visible = true;
                Overlays.ForEach(overlay => overlay.Enable(_actives.Contains(overlay.Id)));
            } else {
                DebugConsole.Sleep();
                Visible = false;
                _actives = Overlays
                    .Where(overlay => overlay.Visible)
                    .Select(overlay => overlay.Id)
                    .ToHashSet();
                Overlays.ForEach(overlay => overlay.Disable());
            }
            return this;
        }

        public DebugOverlayManager Disable() {
            return Enable(false);
        }

        public void ShowAllOverlays() {
            _isSolo = false;
            Overlays.ForEach(overlay => overlay.Enable());
        }

        public void SoloOverlay(int id) {
            if (_isSolo) {
                Overlays.ForEach(overlay => overlay.Enable(_preSolo.Contains(overlay.Id)));
            } else {
                _preSolo = Overlays
                    .Where(overlay => overlay.Visible)
                    .Select(overlay => overlay.Id)
                    .ToHashSet();
                Overlays.ForEach(overlay => overlay.Enable(overlay.Id == id));
            }
            _isSolo = !_isSolo;
        }

        public void CloseOrHideOverlay(int id) {
            if (VisibleCount == 1) {
                Disable();
            } else {
                var debugOverlay = Find(id);
                if (debugOverlay.IsPermanent) debugOverlay.Disable();
                else debugOverlay.QueueFree();
            }
        }
    }
}