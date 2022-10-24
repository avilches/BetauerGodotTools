using System;
using Betauer.Input;
using Betauer.Signal;
using Betauer.UI;
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
        public readonly Label TitleLabel = new() {
            Name = "Title"
        };
        public readonly VBoxContainer VBoxContainer = new () {
            Name = "MonitorList"
        };

        public readonly Control TopBar = new();
        public readonly ColorRect TopBarColor = new();
        public Color Transparent = new(1, 1, 1, 0.490196f);
        public Color Solid = new(1, 1, 1);
        public Object? Target { get; private set; }
        public bool IsFollowing { get; private set; } = false;
        public Func<bool>? RemoveIfFunc { get; private set; }
        private Container? _nestedContainer;

        internal DebugOverlay(DebugOverlayManager manager, int id) {
            _manager = manager;
            _mouseInsidePanel = new Mouse.InsideControl(TopBarColor);
            _offset = new Vector2(id * 64, id * 64);
            Name = $"DebugOverlay-{id}";
            Id = id; 
        }

        public DebugOverlay WithTheme(Theme theme) {
            Theme = theme;
            return this;
        }

        public DebugOverlay Title(string? title) {
            TitleLabel.Text = title;
            TopBar.RectMinSize = new Vector2(100, string.IsNullOrWhiteSpace(TitleLabel.Text) ? 10 : 20);
            return this;                               
        }

        public DebugOverlay RemoveIfInvalid(Object o) {
            Target = o;
            return this;
        }

        public DebugOverlay RemoveIf(Func<bool> removeIf) {
            RemoveIfFunc = removeIf;
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

        public DebugOverlay Follow(Node2D? followNode = null) {
            followNode ??= Target as Node2D;
            if (followNode != null) {
                IsFollowing = true;
                Target = followNode;
                _offset = Vector2.Zero;
            }
            return this;
        }

        public DebugOverlay OpenBox(Action<HBoxContainer>? config = null) {
            return OpenBox<HBoxContainer>(config);
        }

        public DebugOverlay OpenBox<T>(Action<T>? config = null) where T : Container {
            var nestedContainer = Activator.CreateInstance<T>();
            _nestedContainer = nestedContainer;
            VBoxContainer.AddChild(_nestedContainer);
            config?.Invoke(nestedContainer);
            return this;
        }

        public DebugOverlay CloseBox() {
            _nestedContainer = null;
            return this;
        }

        public DebugOverlay Add(Node control) {
            (_nestedContainer ?? VBoxContainer).AddChild(control);
            return this;
        }

        public DebugOverlay Add(BaseMonitor monitor) {
            monitor.DebugOverlayOwner = this;
            return Add((Node)monitor);
        }

        public DebugOverlay Enable(bool enabled = true) {
            Visible = enabled;
            SetProcess(enabled);
            SetProcessInput(enabled);
            SetProcess(enabled);
            return this;
        }

        public DebugOverlay Disable() {
            return Enable(false);
        }

        public override void _Ready() {
            this.NodeBuilder()
                .Child(VBoxContainer)
                    .Child(TopBar)
                        .Config(control => {
                            control.RectMinSize = new Vector2(100, 10);
                        })
                        .Child(TopBarColor)
                            .Config(rect => {
                                rect.Color = Colors.White;
                                rect.SetAnchorsAndMarginsPreset(LayoutPreset.Wide);
                            })
                        .End()
                        .Child(TitleLabel)
                            .Config(label => {
                                label.AddColorOverride("font_color", Colors.White);
                                label.SetAnchorsAndMarginsPreset(LayoutPreset.Center);                        
                            })
                        .End()
                        .Child<HBoxContainer>()
                            .Config(buttonBar => {
                                buttonBar.GrowHorizontal = GrowDirection.Begin;
                                buttonBar.SetAnchorsPreset(LayoutPreset.TopRight);
                                buttonBar.RectMinSize = Vector2.Zero;
                            })
                            .Button("f", () => { if (IsFollowing) StopFollowing(); else Follow(); }).End()
                            .Button("o", () => { Modulate = Modulate.a <= 0.9f ? Solid : Transparent; }).End()
                            .Button("*", () => _manager.All()).End()
                            .Button("s", () => _manager.Solo(Id)).End()
                            .Button("x", () => _manager.Mute(Id)).End()
                        .End()
                    .End()
                .End();
            MouseFilter = MouseFilterEnum.Pass;
            Modulate = Transparent;
            Disable();
        }

        public override void _Input(InputEvent @event) {
            if (@event.IsMouse() && _mouseInsidePanel.Inside) {
                if (@event.IsLeftClick()) {
                    if (@event.IsJustPressed()) {
                        _startDragPosition = _offset - GetGlobalMousePosition();
                        Raise();
                    } else {
                        _startDragPosition = null;
                    }

                } else if (_startDragPosition != null && @event.IsMouseMotion()) {
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
        }

        public override void _Process(float delta) {
            if ((Target != null && !IsInstanceValid(Target)) || (RemoveIfFunc != null && RemoveIfFunc())) {
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