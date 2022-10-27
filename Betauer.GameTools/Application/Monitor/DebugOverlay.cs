using System;
using Betauer.Input;
using Betauer.Nodes;
using Betauer.Signal;
using Betauer.UI;
using Godot;
using Color = Godot.Color;
using Object = Godot.Object;

namespace Betauer.Application.Monitor {
    public class DebugOverlay : PopupPanel {
        private static readonly int VisibilityStateEnumSize = Enum.GetNames(typeof(VisibilityStateEnum)).Length;
        public enum VisibilityStateEnum {
            Solid, Float, SolidTransparent, FloatTransparent
        }
        
        public static Color ColorTransparent = new(1, 1, 1, 0.490196f);
        public static Color ColorSolid = new(1, 1, 1);
        public static Color ColorInvisible = new(1, 1, 1, 0);
        
        private readonly Mouse.InsideControl _mouseInsidePanel;
        private Vector2? _startDragPosition = null;
        private Vector2 FollowPosition => IsFollowing && Target is Node2D node ? node.GetGlobalTransformWithCanvas().origin : Vector2.Zero;
        private Vector2 _position;
        private Container? _nestedContainer;
        private VisibilityStateEnum _visibilityState = VisibilityStateEnum.Float;

        public readonly int Id;
        public readonly Label TitleLabel = new() {
            Name = "Title"
        };
        public readonly ScrollContainer ScrollContainer = new() {
            Name = nameof(ScrollContainer)
        };
        public readonly VBoxContainer OverlayContent = new () {
            Name = nameof(OverlayContent)
        };
        public readonly Label TopBar = new() {
            Name = nameof(TopBar)
        };
        public readonly ColorRect TopBarColor = new() {
            Name = nameof(TopBarColor)
        };
        public DebugOverlayManager DebugOverlayManager { get; }
        public Vector2 MaxSize { get; private set; } = new(600, 600);
        public Object? Target { get; private set; }
        public bool IsFollowing { get; private set; } = false;
        public bool CanFollow => Target is Node2D;
        public Func<bool>? RemoveIfFunc { get; private set; }
        public Button? FollowButton { get; private set; }
        public bool IsDragging => _startDragPosition.HasValue;
        public bool IsPermanent { get; set; } = true;

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

        internal DebugOverlay(DebugOverlayManager debugOverlayManager, int id) {
            DebugOverlayManager = debugOverlayManager;
            _mouseInsidePanel = new Mouse.InsideControl(TopBarColor).Disconnect();
            _position = new Vector2(id * 16, id * 16);
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

        public DebugOverlay SetMaxSize(Vector2 maxSize) {
            MaxSize = maxSize;
            return this;
        }

        public DebugOverlay Title(string? title) {
            TitleLabel.Text = title;
            TopBar.RectMinSize = new Vector2(100, string.IsNullOrWhiteSpace(TitleLabel.Text) ? 10 : 20);
            return this;                               
        }

        public DebugOverlay Permanent(bool isPermanent = true) {
            IsPermanent = isPermanent;
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
            OverlayContent.AddChild(_nestedContainer);
            config?.Invoke(nestedContainer);
            return this;
        }

        public DebugOverlay CloseBox() {
            _nestedContainer = null;
            return this;
        }

        public DebugOverlay Add(Node control) {
            (_nestedContainer ?? OverlayContent).AddChild(control);
            return this;
        }

        public DebugOverlay Add(BaseMonitor monitor) {
            monitor.DebugOverlayOwner = this;
            return Add((Node)monitor);
        }

        public DebugOverlay Enable(bool enable = true) {
            if (Visible == enable) return this;
            Visible = enable;
            if (enable) {
                _mouseInsidePanel.Connect();
            } else {
                _mouseInsidePanel.Disconnect();
            }
            SetProcess(enable);
            SetProcessInput(enable);
            SetProcess(enable);
            return this;
        }

        public DebugOverlay Disable() {
            return Enable(false);
        }

        private void FitContent() {
            var x = Math.Min(OverlayContent.RectSize.x + 10, MaxSize.x);
            var y = Math.Min(OverlayContent.RectSize.y + 10, MaxSize.x);
            ScrollContainer.RectMinSize = new Vector2(x, y);
            TopBar.RectMinSize = new Vector2(x, 10);
        }

        public override void _Ready() {
            this.NodeBuilder()
                .Child<VBoxContainer>()
                    .Child(TopBar)
                        .Config(control => {
                            control.RectMinSize = new Vector2(100, 10);
                            control.SetAnchorsAndMarginsPreset(LayoutPreset.Wide);
                        })
                        .Child(TopBarColor)
                            .Config(rect => {
                                rect.Color = Colors.White;
                                rect.SetAnchorsAndMarginsPreset(LayoutPreset.Wide);
                            })
                        .End()
                        .Child(TitleLabel)
                            .Config(label => {
                                label.SetFontColor(Colors.White);
                                label.SetAnchorsAndMarginsPreset(LayoutPreset.Center);                        
                            })
                        .End()
                        .Child<HBoxContainer>()
                            .Config(buttonBar => {
                                buttonBar.GrowHorizontal = GrowDirection.Begin;
                                buttonBar.SetAnchorsPreset(LayoutPreset.TopRight);
                                buttonBar.MarginLeft = 0;
                                buttonBar.MarginRight = 0;
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
                                    var newState = ((int)_visibilityState + 1) % VisibilityStateEnumSize;
                                    VisibilityState = (VisibilityStateEnum)newState;
                                })
                                .Config(button => {
                                    button.FocusMode = FocusModeEnum.None;
                                    button.HintTooltip = "Opacity";
                                })
                            .End()
                            .Button("s", () => DebugOverlayManager.SoloOverlay(Id))
                                .Config(button => {
                                    button.FocusMode = FocusModeEnum.None;
                                    button.HintTooltip = "Solo mode";
                                })
                            .End()
                            .Button("*", () => DebugOverlayManager.ShowAllOverlays())
                                .Config(button => {
                                    button.FocusMode = FocusModeEnum.None;
                                    button.HintTooltip = "Open all";
                                })
                            .End()
                            .Button("x", () => DebugOverlayManager.CloseOrHideOverlay(Id))
                                .Config(button => {
                                    button.FocusMode = FocusModeEnum.None;
                                    button.HintTooltip = "Close";
                                })
                            .End()
                        .End()
                    .End()
                    .Child(ScrollContainer)
                        .Child<MarginContainer>()
                            .Config(margin => {
                                margin.SetMargin(0, 10, 10, 0);
                            })
                            .Child(OverlayContent)
                            .End()
                        .End()
                    .End()
                .End();
            MouseFilter = MouseFilterEnum.Pass;
            OverlayContent.OnResized(FitContent);
            FitContent();
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