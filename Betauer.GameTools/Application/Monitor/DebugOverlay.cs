using System;
using Betauer.Input;
using Betauer.Signal;
using Betauer.UI;
using Godot;
using Color = Godot.Color;
using Object = Godot.Object;

namespace Betauer.Application.Monitor {
    public class DebugOverlay : PopupPanel {
        public enum VisibilityStateEnum {
            Solid, Float, SolidTransparent, FloatTransparent
        }
        
        private static readonly Color ColorTransparent = new(1, 1, 1, 0.490196f);
        private static readonly Color ColorSolid = new(1, 1, 1);
        private static readonly Color ColorInvisible = new(1, 1, 1, 0);
        private readonly DebugOverlayManager _manager;
        private readonly Mouse.InsideControl _mouseInsidePanel;
        private Vector2? _startDragPosition = null;
        private Vector2 FollowPosition => IsFollowing && Target is Node2D node ? node.GetGlobalTransformWithCanvas().origin : Vector2.Zero;
        private Vector2 _position;
        private Container? _nestedContainer;

        public readonly int Id;
        public readonly Label TitleLabel = new() {
            Name = "Title"
        };
        public readonly VBoxContainer VBoxContainer = new () {
            Name = "MonitorList"
        };

        public readonly Control TopBar = new() {
            Name = "TopBar"
        };

        public readonly ColorRect TopBarColor = new() {
            Name = "TopParColorRect"
        };
        public Object? Target { get; private set; }
        public bool IsFollowing { get; private set; } = false;
        public bool CanFollow => Target is Node2D;
        public Func<bool>? RemoveIfFunc { get; private set; }
        public Button? FollowButton { get; private set; }
        public bool IsDragging => _startDragPosition.HasValue;

        private VisibilityStateEnum _visibilityState = VisibilityStateEnum.Float;
        public VisibilityStateEnum VisibilityState {
            get => _visibilityState;
            set {
                _visibilityState = value;
                if (_visibilityState == VisibilityStateEnum.Solid) {
                    SelfModulate = ColorSolid;
                    Modulate = ColorSolid;
                } else if (_visibilityState == VisibilityStateEnum.SolidTransparent) {
                    SelfModulate = ColorSolid;
                    Modulate = ColorTransparent;
                } else if (_visibilityState == VisibilityStateEnum.Float) {
                    SelfModulate = ColorInvisible;
                    Modulate = ColorSolid;
                } else if (_visibilityState == VisibilityStateEnum.FloatTransparent) {
                    SelfModulate = ColorInvisible;
                    Modulate = ColorTransparent;
                }
            }
        }


        internal DebugOverlay(DebugOverlayManager manager, int id) {
            _manager = manager;
            _mouseInsidePanel = new Mouse.InsideControl(TopBarColor);
            _position = new Vector2(id * 64, id * 64);
            Name = $"DebugOverlay-{id}";
            Id = id;
            VisibilityState = VisibilityStateEnum.Float;
        }

        public DebugOverlay WithTheme(Theme theme) {
            Theme = theme;
            return this;
        }

        public DebugOverlay Solid() {
            VisibilityState = VisibilityStateEnum.Solid;
            return this;
        }

        public DebugOverlay Hint(string hint) {
            HintTooltip = hint;
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
            UpdateFollowButtonState();
            _position = RectPosition;
            return this;
        }

        public DebugOverlay Offset(Vector2 offset) {
            _position += offset;
            return this;
        }

        public DebugOverlay Follow(Node2D? followNode = null) {
            followNode ??= Target as Node2D;
            if (followNode != null) {
                Target = followNode;
                IsFollowing = true;
                _position = Vector2.Zero;
            }
            UpdateFollowButtonState();
            return this;
        }
        
        public DebugOverlay RemoveButtons() {
            TopBar.Visible = false;
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
                            .Button<CheckButton>("f", () => { if (IsFollowing) StopFollowing(); else Follow(); })
                                .Config(button => {
                                    button.FocusMode = FocusModeEnum.None;
                                    button.HintTooltip = "Follow";
                                    FollowButton = button;
                                    UpdateFollowButtonState();
                                })
                            .End()
                            .Button("o", () => {
                                    var newState = ((int)_visibilityState + 1) % 4;
                                    VisibilityState = (VisibilityStateEnum)newState;
                                })
                                .Config(button => {
                                    button.FocusMode = FocusModeEnum.None;
                                    button.HintTooltip = "Opacity";
                                })
                            .End()
                            .Button("s", () => _manager.Solo(Id))
                                .Config(button => {
                                    button.FocusMode = FocusModeEnum.None;
                                    button.HintTooltip = "Solo mode";
                                })
                            .End()
                            .Button("*", () => _manager.All())
                                .Config(button => {
                                    button.FocusMode = FocusModeEnum.None;
                                    button.HintTooltip = "Open all";
                                })
                            .End()
                            .Button("x", () => _manager.Mute(Id))
                                .Config(button => {
                                    button.FocusMode = FocusModeEnum.None;
                                    button.HintTooltip = "Close";
                                })
                            .End()
                        .End()
                    .End()
                .End();
            MouseFilter = MouseFilterEnum.Pass;
            Disable();
        }

        public override void _Input(InputEvent @event) {
            if (@event.IsMouse() && _mouseInsidePanel.Inside) {
                if (@event.IsLeftClick()) {
                    if (@event.IsJustPressed()) {
                        StopFollowing();
                        _startDragPosition = _position - GetGlobalMousePosition();
                        Raise();
                    } else {
                        _startDragPosition = null;
                    }

                } else if (IsDragging && @event.IsMouseMotion()) {
                    var newPosition = GetGlobalMousePosition() + _startDragPosition.Value;
                    var origin = FollowPosition;
                    // TODO: GetTree().Root.Size doesn't work well with scaled viewport
                    var screenSize = GetTree().Root.Size;
                    var limitX = RectSize.x >= screenSize.x ? 20 : RectSize.x; 
                    var limitY = RectSize.y >= screenSize.y ? 20 : RectSize.y; 
                    // Ensure the user can't drag and drop the overlay outside of the screen
                    newPosition = new Vector2(
                        Mathf.Clamp(newPosition.x, -origin.x, -origin.x + screenSize.x - limitX),
                        Mathf.Clamp(newPosition.y, -origin.y, -origin.y + screenSize.y - limitY));
                    _position = newPosition;
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
                    var newPosition = FollowPosition + _position;
                    // Ensure the overlay doesn't go out of the screen when following the node
                    var screenSize = GetTree().Root.Size;
                    var limitX = RectSize.x >= screenSize.x ? 20 : RectSize.x; 
                    var limitY = RectSize.y >= screenSize.y ? 20 : RectSize.y; 
                    newPosition = new Vector2(
                        Mathf.Clamp(newPosition.x, 0, screenSize.x - limitX),
                        Mathf.Clamp(newPosition.y, 0, screenSize.y - limitY));
                    SetPosition(newPosition);
                } else {
                    SetPosition(_position);
                }
                // Hack time: set a very small size to ensure the panel is resized big enough for the data inside
                RectSize = Vector2.Zero;
            }
        }
        
        private void UpdateFollowButtonState() {
            if (FollowButton != null) {
                FollowButton.Visible = CanFollow;
                FollowButton.Pressed = IsFollowing;
            }
        }
    }
}