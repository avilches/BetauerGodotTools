using System;
using Betauer.Application.Notifications;
using Betauer.Input;
using Betauer.Core.Nodes;
using Betauer.Core.Signal;
using Betauer.DI;
using Betauer.Input.Controller;
using Betauer.UI;
using Godot;
using Color = Godot.Color;
using Container = Godot.Container;
using Object = Godot.Object;

namespace Betauer.Application.Monitor;

public partial class DebugOverlay : Panel {
    private static readonly int VisibilityStateEnumSize = Enum.GetNames(typeof(VisibilityStateEnum)).Length;
    public enum VisibilityStateEnum {
        Solid, Float, SolidTransparent, FloatTransparent
    }
    
    public static Color ColorTransparent = new(1, 1, 1, 0.490196f);
    public static Color ColorSolid = new(1, 1, 1);
    public static Color ColorInvisible = new(1, 1, 1, 0);
    
    private Vector2 FollowPosition => IsFollowing && Target is Node2D node ? node.GetGlobalTransformWithCanvas().origin : Vector2.Zero;
    private Vector2 _position;
    private Container? _nestedContainer;
    private VisibilityStateEnum _visibilityState = VisibilityStateEnum.Float;

    public readonly int Id;

    public readonly ScrollContainer ScrollContainer = new() {
        Name = nameof(ScrollContainer)
    };
    public readonly VBoxContainer OverlayContent = new () {
        Name = nameof(OverlayContent)
    };
    public readonly Control TitleBar = new() {
        Name = nameof(TitleBar)
    };
    public readonly ColorRect TitleBackground = new() {
        Name = nameof(TitleBackground)
    };
    public readonly Label TitleLabel = new() {
        Name = "Title"
    };
    public readonly HBoxContainer ButtonBar = new() {
        Name = "ButtonBar"
    };
    public DebugOverlayManager DebugOverlayManager { get; }
    public Vector2 MaxSize { get; private set; } = new(400, 400);
    public Vector2 MinSize { get; private set; } = new(50, 50);
    public Object? Target { get; private set; }
    public bool IsFollowing { get; private set; } = false;
    public bool CanFollow => Target is Node2D;
    public Func<bool>? RemoveIfFunc { get; private set; }
    public Button? FollowButton { get; private set; }
    public bool IsHideOnClose { get; set; } = true;
    
    private DragAndDropController _dragAndDropController;

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
        _position = new Vector2(id * 16, id * 16);
        Name = $"DebugOverlay-{id}";
        Id = id;
        VisibilityState = VisibilityStateEnum.Float;
        Visible = true;
        
        // Default colors
        SetColors(new(0.2f, 0.2f, 0.2f), Colors.White, Colors.Black);
    }

    public DebugOverlay WithTheme(Theme theme) {
        Theme = theme;
        return this;
    }

    public DebugOverlay SetColors(Color backgroundColor, Color titleColor, Color titleBackgroundColor) {
        TitleBackground.Color = titleBackgroundColor;
        AddThemeStyleboxOverride("panel", new StyleBoxFlat {
            BgColor = backgroundColor
        });
        TitleLabel.SetFontColor(titleColor);
        return this;
    }

    public DebugOverlay Solid() {
        VisibilityState = VisibilityStateEnum.Solid;
        return this;
    }

    public DebugOverlay SetMaxSize(int x, int y) => SetMaxSize(new Vector2(x, y));
    public DebugOverlay SetMinSize(int x, int y) => SetMinSize(new Vector2(x, y));

    public DebugOverlay SetMaxSize(Vector2 maxSize) {
        MaxSize = maxSize;
        return this;
    }

    public DebugOverlay SetMinSize(Vector2 minSize) {
        MinSize = minSize;
        return this;
    }

    public DebugOverlay Title(string? title) {
        TitleLabel.Text = title;
        return this;                               
    }

    public DebugOverlay HideOnClose(bool isHideOnClose = true) {
        IsHideOnClose = isHideOnClose;
        return this;
    }

    public DebugOverlay RemoveIf(Func<bool> removeIf) {
        RemoveIfFunc = removeIf;
        return this;
    }

    public DebugOverlay StopFollowing() {
        IsFollowing = false;
        UpdateFollowButtonState();
        _position = Position;
        return this;
    }

    public DebugOverlay Offset(Vector2 offset) {
        _position += offset;
        return this;
    }

    public DebugOverlay Attach(Object o) {
        Target = o;
        if (o is not Node2D) StopFollowing();
        UpdateFollowButtonState();
        return this;
    }

    public DebugOverlay Follow(Node2D? followNode) => Attach(followNode).Follow();

    public DebugOverlay Follow() {
        if (Target is Node2D followNode) {
            IsFollowing = true;
            _position = Vector2.Zero;
        }
        UpdateFollowButtonState();
        return this;
    }
    
    public DebugOverlay RemoveButtons() {
        ButtonBar.Visible = false;
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
        Visible = enable;
        SetProcess(enable);
        SetProcessInput(enable);
        return this;
    }

    public DebugOverlay Disable() {
        return Enable(false);
    }

    private int MarginScrollBar = 10;
    
    private void FitContent() {
        var hasTitle = !string.IsNullOrWhiteSpace(TitleLabel.Text);
        var realTitleHeight = hasTitle ? TitleLabel.Size.y: 0;
        var contentSize = OverlayContent.Size.Clamp(MinSize, MaxSize) + new Vector2(MarginScrollBar, MarginScrollBar);
        
        // Panel size
        CustomMinimumSize = new Vector2(contentSize.x, contentSize.y + realTitleHeight);
        ScrollContainer.CustomMinimumSize = contentSize;
        
        TitleBar.CustomMinimumSize = new Vector2(contentSize.x, realTitleHeight);
        TitleBar.Visible = hasTitle;
        ButtonBar.SetAnchorsAndOffsetsPreset(LayoutPreset.TopRight);
    }

    public override void _Ready() {
        this.NodeBuilder()
            .Child<VBoxContainer>()
                .Child(TitleBar)
                    .Child(TitleBackground)
                        .Config(rect => {
                            rect.SetAnchorsPreset(LayoutPreset.FullRect);
                        })
                    .End()
                    .Child(TitleLabel)
                        .Config(label => {
                            label.SetAnchorsPreset(LayoutPreset.TopWide);
                            label.HorizontalAlignment = HorizontalAlignment.Center;
                    })
                    .End()
                .End()
                .Child(ScrollContainer)
                    .Child<MarginContainer>()
                        .Config(margin => {
                            margin.SetMargin(0, MarginScrollBar, MarginScrollBar, 0);
                        })
                        .Child(OverlayContent)
                        .End()
                    .End()
                .End()
            .End()
            .Child<HBoxContainer>(ButtonBar)
                .Config(buttonBar => {
                    buttonBar.Visible = false;
                    buttonBar.Alignment = BoxContainer.AlignmentMode.End;
                    buttonBar.SetAnchorsAndOffsetsPreset(LayoutPreset.TopRight);
                })
                .Button<CheckButton>("f", () => { if (IsFollowing) StopFollowing(); else Follow(); })
                    .Config(button => {
                        button.FocusMode = FocusModeEnum.None;
                        button.TooltipText = "Follow";
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
                        button.TooltipText = "Opacity";
                    })
                .End()
                .Button("s", () => DebugOverlayManager.SoloOverlay(Id))
                    .Config(button => {
                        button.FocusMode = FocusModeEnum.None;
                        button.TooltipText = "Solo mode";
                    })
                .End()
                .Button("*", () => DebugOverlayManager.ShowAllOverlays())
                    .Config(button => {
                        button.FocusMode = FocusModeEnum.None;
                        button.TooltipText = "Open all";
                    })
                .End()
                .Button("x", () => DebugOverlayManager.CloseOrHideOverlay(Id))
                    .Config(button => {
                        button.FocusMode = FocusModeEnum.None;
                        button.TooltipText = "Close";
                    })
                .End()
            .End();
        MouseFilter = MouseFilterEnum.Pass;
        OverlayContent.OnResized(FitContent);
        FitContent();
    }

    [PostInject]
    public void Configure() {
        _dragAndDropController = new DragAndDropController().WithMouseButton(MouseButton.Left).OnlyIf(DragPredicate);
        _dragAndDropController.OnStartDrag += OnStartDrag;
        _dragAndDropController.OnDrag += OnDrag;
        DefaultNotificationsHandler.Instance.OnWmMouseExit += () => _dragAndDropController.ForceDrop();
        DefaultNotificationsHandler.Instance.OnWmWindowFocusOut += () => _dragAndDropController.ForceDrop(); 
        DefaultNotificationsHandler.Instance.OnApplicationFocusOut += () => _dragAndDropController.ForceDrop();
    }

    public override void _Input(InputEvent input) {
        if (input.IsMouseMotion()) {
            ButtonBar.Visible = input.IsMouseInside(TitleBackground);
        }
        _dragAndDropController.Handle(input);
    }

    private void OnStartDrag(Vector2 position) {
        StopFollowing();
        GetParent().MoveChild(this, -1);
        AcceptEvent();
    }

    private void OnDrag(Vector2 offset) {
        var newPosition = _position + offset;
        var origin = FollowPosition;
        // TODO: GetTree().Root.Size doesn't work well with scaled viewport
        var screenSize = GetTree().Root.Size;
        var limitX = Size.x >= screenSize.x ? 20 : Size.x;
        var limitY = Size.y >= screenSize.y ? 20 : Size.y;
        // Ensure the user can't drag and drop the overlay outside of the screen
        newPosition = new Vector2(
            Mathf.Clamp(newPosition.x, -origin.x, -origin.x + screenSize.x - limitX),
            Mathf.Clamp(newPosition.y, -origin.y, -origin.y + screenSize.y - limitY));
        _position = newPosition;
    }

    private bool DragPredicate(InputEvent input) =>
        input.IsMouseInside(TitleBackground) && !input.IsMouseInside(ButtonBar);

    public override void _Process(double delta) {
        if ((Target != null && !IsInstanceValid(Target)) || (RemoveIfFunc != null && RemoveIfFunc())) {
            QueueFree();
        } else if (!Visible) {
            Disable();
        } else {
            if (IsFollowing) {
                var newPosition = FollowPosition + _position;
                // Ensure the overlay doesn't go out of the screen when following the node
                var screenSize = GetTree().Root.Size;
                var limitX = Size.x >= screenSize.x ? 20 : Size.x; 
                var limitY = Size.y >= screenSize.y ? 20 : Size.y; 
                newPosition = new Vector2(
                    Mathf.Clamp(newPosition.x, 0, screenSize.x - limitX),
                    Mathf.Clamp(newPosition.y, 0, screenSize.y - limitY));
                SetPosition(newPosition);
            } else {
                SetPosition(_position);
            }
            // Hack time: set a very small size to ensure the panel is resized big enough for the data inside
            Size = Vector2.Zero;
        }
    }
    
    private void UpdateFollowButtonState() {
        if (FollowButton != null) {
            FollowButton.Visible = CanFollow;
            FollowButton.ButtonPressed = IsFollowing;
        }
    }
}