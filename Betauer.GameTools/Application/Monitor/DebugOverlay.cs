using System;
using Betauer.Input;
using Betauer.Signal;
using Godot;
using Color = Godot.Color;

namespace Betauer.Application.Monitor {
    public class DebugOverlay : PopupPanel {
        private readonly Mouse.InsideControl _mouseInsidePanel;
        private Vector2? _startDragPosition = null;
        private readonly DebugOverlayManager _manager;
        private Vector2 FollowPosition => NodeToFollow != null ? NodeToFollow.GetGlobalTransformWithCanvas().origin : Vector2.Zero;
        private Vector2 _offset;
        private Node2D? _nodeToFollow;

        public readonly int Id;
        public readonly Label TitleLabel;
        public readonly VBoxContainer Container;
        public Color Transparent = new(1, 1, 1, 0.490196f);
        public Color Solid = new(1, 1, 1);
        public Node2D? NodeToFollow {
            get => _nodeToFollow;
            set => Follow(_nodeToFollow);
        }

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

        public DebugOverlay Follow(Node2D followNode) {
            NodeToFollow = followNode;
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
            if (NodeToFollow != null && !IsInstanceValid(NodeToFollow)) {
                QueueFree();
            } else if (!Visible) {
                Disable();
            } else {
                if (NodeToFollow != null) {
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