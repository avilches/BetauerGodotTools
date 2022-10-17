using System;
using Betauer.Input;
using Betauer.Signal;
using Godot;
using Color = Godot.Color;
using Object = Godot.Object;

namespace Betauer.Application.Monitor {
    public class DebugOverlay : PopupPanel {
        private readonly Mouse.InsideControl _mouseInsidePanel;
        private Vector2? _startDragPosition = null;
        private readonly DebugOverlayManager _manager;
        private Vector2 FollowPosition => IsFollowing && Target is Node2D node ? node.GetGlobalTransformWithCanvas().origin : Vector2.Zero;
        private Vector2 _offset;

        public readonly int Id;
        public readonly Label TitleLabel;
        public readonly VBoxContainer Container;
        public Color Transparent = new(1, 1, 1, 0.490196f);
        public Color Solid = new(1, 1, 1);
        public Object? Target { get; private set; }
        public bool IsFollowing { get; private set; } = false;
        private Func<bool>? _removeIf;

        internal DebugOverlay(DebugOverlayManager manager, int id) {
            _manager = manager;
            _mouseInsidePanel = new Mouse.InsideControl(this);
            _offset = new Vector2(id * 64, id * 64);

            Id = id; 
            Name = $"DebugOverlay-{id}";
            MouseFilter = MouseFilterEnum.Pass;
            Modulate = new Color(1, 1, 1, 0.490196f);
            Container = new VBoxContainer();
            TitleLabel = new Label();
            TitleLabel.Name = "Title";
            Container.AddChild(TitleLabel);

            AddChild(Container);
        }

        public DebugOverlay WithTheme(Theme theme) {
            Theme = theme;
            return this;
        }

        public DebugOverlay Title(string? title) {
            TitleLabel.Text = title;
            TitleLabel.Visible = title != null;
            return this;
        }

        public DebugOverlay RemoveIfInvalid(Object o) {
            Target = o;
            return this;
        }

        public DebugOverlay RemoveIf(Func<bool> removeIf) {
            _removeIf = removeIf;
            return this;
        }

        public DebugOverlay StopFollowing() {
            IsFollowing = false;
            _offset = RectPosition;
            return this;
        }

        public DebugOverlay Offset(Vector2 offset) {
            _offset = offset;
            return this;
        }

        public DebugOverlay Follow(Node2D followNode) {
            IsFollowing = true;
            Target = followNode;
            _offset = Vector2.Zero;
            return this;
        }

        public DebugOverlay Add(Control control) {
            Container.AddChild(control);
            return this;
        }

        public DebugOverlay Add(BaseMonitor monitor) {
            monitor.DebugOverlayOwner = this;
            Container.AddChild(monitor);
            return this;
        }

        public void Enable(bool enabled = true) {
            Visible = enabled;
            SetProcess(enabled);
        }

        public void Disable() {
            Enable(false);
        }

        public override void _Ready() {
            PauseMode = PauseModeEnum.Process;
            Disable();
        }

        public override void _Input(InputEvent @event) {
            if (@event.IsDoubleClick(ButtonList.Left)) {
                Modulate = Modulate.a <= 0.9f ? Solid : Transparent;
                
            } else if (_mouseInsidePanel.Inside && @event.IsLeftClick()) {
                if (@event.IsJustPressed()) {
                    _startDragPosition = _offset - GetGlobalMousePosition();
                    Raise();
                } else {
                    _startDragPosition = null;
                }
                
            } else if (_mouseInsidePanel.Inside && _startDragPosition != null && @event.IsMouseMotion()) {
                var newPosition = GetGlobalMousePosition() + _startDragPosition.Value;
                var origin = FollowPosition;
                // TODO: GetTree().Root.Size doesn't work well with scaled viewport
                // Ensure the user can't drag and drop the overlay outside of the screen
                newPosition = new Vector2(
                    Mathf.Clamp(newPosition.x, -origin.x, -origin.x + GetTree().Root.Size.x - RectSize.x),
                    Mathf.Clamp(newPosition.y, -origin.y, -origin.y + GetTree().Root.Size.y - RectSize.y));
                _offset = newPosition;
            }
        }

        public override void _Process(float delta) {
            if ((_removeIf != null && _removeIf.Invoke()) ||
                (Target != null && !IsInstanceValid(Target))) {
                QueueFree();
            } else if (!Visible) {
                Disable();
            } else {
                if (IsFollowing) {
                    var newPosition = FollowPosition + _offset;
                    // Ensure the overlay doesn't go out of the screen when following the node
                    newPosition = new Vector2(
                        Mathf.Clamp(newPosition.x, 0, GetTree().Root.Size.x - RectSize.x),
                        Mathf.Clamp(newPosition.y, 0, GetTree().Root.Size.y - RectSize.y));
                    SetPosition(newPosition);
                } else {
                    SetPosition(_offset);
                }
                // Hack time: set a very small size to ensure the panel is resized big enough for the data inside
                RectSize = Vector2.Zero;
            }
        }
    }
}