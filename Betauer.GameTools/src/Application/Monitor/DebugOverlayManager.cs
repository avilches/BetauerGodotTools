using System.Collections.Generic;
using System.Linq;
using Betauer.Core;
using Betauer.DI.Attributes;
using Betauer.Input;
using Betauer.UI;
using Godot;

namespace Betauer.Application.Monitor;
public partial class DebugOverlayManager : CanvasLayer {
    private int _count = 0;
    private HashSet<int> _preSolo = new();
    private bool _isSolo = false;

    public readonly Control OverlayContainer = new() {
        Name = nameof(OverlayContainer)
    };
    
    public readonly DebugConsole DebugConsole;

    public readonly Label Right = new() {
        Name = "Right"
    };
    
    private readonly List<DebugOverlay> _overlays = new();
    private List<DebugOverlay> Overlays {
        get {
            PurgeOverlays();
            return _overlays;
        }
    }

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
        this.Children()
            .Add(Right, label => {
                label.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.TopRight);
                label.GrowHorizontal = Control.GrowDirection.Begin;
                // label.MarginLeft = 0;
                // label.MarginRight = 0;
                label.HorizontalAlignment = HorizontalAlignment.Right;
            })
            .Add(OverlayContainer)
            .Add(DebugConsole);
        Name = "DebugOverlayManager";
        Layer = 1000000;
        ProcessMode = ProcessModeEnum.Always;
        Visible = false;
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
        Overlays.Add(overlay);
        return overlay;
    }

    public bool HasOverlay(string title) =>
        Overlays.FirstOrDefault(overlay => overlay.TitleLabel.Text == title) != null;
    
    public bool HasOverlay(GodotObject target) => 
        Overlays.FirstOrDefault(overlay => overlay.Target == target) != null;

    public void DisableIfExist(GodotObject target) {
        var overlay = Overlays.FirstOrDefault(overlay => overlay.Target == target);
        if (overlay != null) CloseOrHideOverlay(overlay.Id);
    }

    public void ShowIfExist(GodotObject target) {
        Overlays.FirstOrDefault(overlay => overlay.Target == target)?.Enable();
    }

    private Control? _lastFocus;
    public override void _Input(InputEvent input) {
        if (DebugOverlayAction != null && DebugOverlayAction.IsEventJustPressed(input)) {
            UserHitDebug(input);
        }
    }

    private void UserHitDebug(InputEvent input) {
        if (input.HasShift()) {
            if (Visible) {
                // Overlay visible: change the console only
                if (DebugConsole.Visible) {
                    DebugConsole.Disable();
                    if (_lastFocus != null && _lastFocus.IsInstanceValid() && _lastFocus.IsVisibleInTree()) _lastFocus.GrabFocus();
                } else {
                    _lastFocus = OverlayContainer.GetViewport().GuiGetFocusOwner();
                    DebugConsole.Enable();
                }
            } else {
                // Overlay not visible: enable console and overlay
                _lastFocus = OverlayContainer.GetViewport().GuiGetFocusOwner();
                Enable();
                DebugConsole.Enable();
            }
        } else {
            if (Visible) {
                // Overlay visible: just hide everything
                Disable();
                if (_lastFocus != null && _lastFocus.IsInstanceValid() && _lastFocus.IsVisibleInTree()) _lastFocus.GrabFocus();
            } else {
                Enable();
            }
        }
    }

    public override void _Process(double delta) {
        Right.Text = ((int)Engine.GetFramesPerSecond()).ToString();
    }

    private void PurgeOverlays() {
        _overlays.RemoveAll(overlay => !IsInstanceValid(overlay));
    }

    public DebugOverlayManager Enable(bool enable = true) {
        Visible = enable;
        SetProcess(enable);
        if (enable) {
            if (DebugConsole.Visible) DebugConsole.Enable();
        } else {
            DebugConsole.Sleep();
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
        var visibleCount = Overlays.Count(debugOverlay => debugOverlay.Visible);
        if (visibleCount == 1 && !DebugConsole.Visible) {
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