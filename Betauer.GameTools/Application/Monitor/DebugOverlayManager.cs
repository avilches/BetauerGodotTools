using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Betauer.Core;
using Betauer.DI;
using Betauer.Input;
using Betauer.UI;
using Godot;

namespace Betauer.Application.Monitor;
public partial class DebugOverlayManager : CanvasLayer {
    private int _count = 0;
    private HashSet<int> _actives = new();
    private HashSet<int> _preSolo = new();
    private bool _isSolo = false;

    public readonly Control OverlayContainer = new() {
        Name = nameof(OverlayContainer)
    };
    
    public readonly DebugConsole DebugConsole;

    public readonly Label Right = new() {
        Name = "Right"
    };
    
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
        ProcessMode = ProcessModeEnum.Always;
        Visible = false;
        this.NodeBuilder()
            .Child(Right).
                Config(label => {
                    label.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.TopRight);
                    label.GrowHorizontal = Control.GrowDirection.Begin;
                    // label.MarginLeft = 0;
                    // label.MarginRight = 0;
                    label.HorizontalAlignment = HorizontalAlignment.Right;
            })
            .End()
            .Child(OverlayContainer)
            .End()
            .Child(DebugConsole);
    }

    public DebugOverlay Overlay(string title) {
        return Overlays.FirstOrDefault(d => d.TitleLabel.Text == title) ??
               CreateOverlay(title).Enable();
    }

    public DebugOverlay Overlay(GodotObject target) {
        return Overlays.FirstOrDefault(d => d.Target == target) ??
               CreateOverlay(target).Enable();
    }

    public DebugOverlay Follow(Node2D follow) {
        return Overlay(follow).Follow(follow);
    }

    public DebugOverlay CreateOverlay(GodotObject target, string? title = null) {
        var overlay = CreateOverlay(title);
        overlay.Attach(target);
        return overlay;
    }

    public DebugOverlay CreateOverlay(string? title = null) {
        var overlay = new DebugOverlay(this, _count++).Title(title);
        OverlayContainer.AddChild(overlay);
        _actives.Add(overlay.Id);
        return overlay;
    }

    public bool HasOverlay(string title) =>
        Overlays.FirstOrDefault(overlay => overlay.TitleLabel.Text == title) != null;
    
    public bool HasOverlay(GodotObject target) => 
        Overlays.FirstOrDefault(overlay => overlay.Target == target) != null;

    public override void _Input(InputEvent input) {
        if (DebugOverlayAction != null && DebugOverlayAction.IsEventJustPressed(input)) {
            if (input.HasShift()) {
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

    public override void _PhysicsProcess(double delta) {
        if (!Visible) {
            Disable();
        } else {
            Right.Text = ((int)Engine.GetFramesPerSecond()).ToString();
        }
    }

    public DebugOverlayManager Enable(bool enable = true) {
        Visible = enable;
        SetPhysicsProcess(enable);
        if (enable) {
            if (DebugConsole.Visible) DebugConsole.Enable();
            Overlays.ForEach(overlay => overlay.Enable(_actives.Contains(overlay.Id)));
        } else {
            DebugConsole.Sleep();
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
        if (VisibleCount == 1 && !DebugConsole.Visible) {
            // If the overlay to close is the last one (and there is no console), hide the manager instead, so
            // when the manager is shown again, the last closed overlay will be shown.
            Disable();
        } else {
            var debugOverlay = Find(id);
            if (debugOverlay.IsHideOnClose) debugOverlay.Disable();
            else debugOverlay.QueueFree();
        }
    }
}  