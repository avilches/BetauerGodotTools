using System;
using Betauer.Application.Notifications;
using Betauer.Input;
using Betauer.Core.Nodes;
using Betauer.DI;
using Betauer.Input.Controller;
using Betauer.UI;
using Godot;
using Color = Godot.Color;

namespace Betauer.Application.Monitor;

/*
 * Label
 * 
 *
 * DebugOverlay uses a label for the title with a variation called "TitleLabel"
 * - TitleLabel.font_color
 * - TitleLabel.background_color
 *
 * TextEdit monitor label (without variation) and other label (with a variation called "LabelContent") for the content like "speed: 200px", where the 200px is the content
 * - Label.font_color
 * - ContentLabel.font_color
 */

public partial class DebugOverlay : Panel, IInjectable {
    private static readonly int VisibilityStateEnumSize = Enum.GetNames(typeof(VisibilityStateEnum)).Length;
    public enum VisibilityStateEnum {
        Solid, Float, SolidTransparent, FloatTransparent
    }
    
    public static Color ColorTransparent = new(1, 1, 1, 0.490196f);
    public static Color ColorSolid = new(1, 1, 1);
    public static Color ColorInvisible = new(1, 1, 1, 0);
    public event Action? OnDestroyEvent;
    
    private Vector2 FollowPosition => IsFollowing && Target is Node2D node ? node.GetGlobalTransformWithCanvas().Origin : Vector2.Zero;
    private Vector2 _position;
    private VisibilityStateEnum _visibilityState = VisibilityStateEnum.Float;
    private const int MarginScrollBar = 10;

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
    public GodotObject? Target { get; private set; }
    public bool IsFollowing { get; private set; } = false;
    public bool CanFollow => Target is Node2D;
    public Func<bool>? DestroyIfFunc { get; private set; }
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
    }

    public DebugOverlay WithTheme(Theme theme) {
        Theme = theme;
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

    public DebugOverlay DestroyIf(Func<bool> destroyIf) {
        DestroyIfFunc = destroyIf;
        return this;
    }

    public DebugOverlay OnDestroy(Action destroyIf) {
        OnDestroyEvent += destroyIf;
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

    public DebugOverlay Attach(GodotObject o) {
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

    public NodeBuilder<VBoxContainer> Children() {
        return OverlayContent.Children();
    }

    public DebugOverlay Enable(bool enable = true) {
        Visible = enable;
        return this;
    }

    public DebugOverlay Disable() {
        return Enable(false);
    }

    private void CheckProcessBasedOnVisibility() {
        var isVisibleInTree = IsVisibleInTree();
        SetProcess(isVisibleInTree);
        SetProcessInput(isVisibleInTree);
    }

    private void FitContent() {
        var hasTitle = !string.IsNullOrWhiteSpace(TitleLabel.Text);
        var realTitleHeight = hasTitle ? TitleLabel.Size.Y: 0;
        var contentSize = OverlayContent.Size.Clamp(MinSize, MaxSize) + new Vector2(MarginScrollBar, MarginScrollBar);
        
        // Panel size
        CustomMinimumSize = new Vector2(contentSize.X, contentSize.Y + realTitleHeight);
        ScrollContainer.CustomMinimumSize = contentSize;
        
        TitleBar.CustomMinimumSize = new Vector2(contentSize.X, realTitleHeight);
        TitleBar.Visible = hasTitle;
        ButtonBar.SetAnchorsAndOffsetsPreset(LayoutPreset.TopRight);
    }

    public override void _Ready() {
        new NodeBuilder<DebugOverlay>(this)
            .Add<VBoxContainer>(box => {
                box.Children()
                    .Add(TitleBar, titleBar => {
                        titleBar.Children()
                            .Add(TitleBackground, rect => {
                                rect.ThemeTypeVariation = "TitleLabel";
                                rect.Color = rect.GetThemeColor("background_color");
                                rect.SetAnchorsPreset(LayoutPreset.FullRect);
                            })
                            .Add(TitleLabel, label => {
                                label.ThemeTypeVariation = "TitleLabel";
                                label.SetAnchorsPreset(LayoutPreset.TopWide);
                                label.HorizontalAlignment = HorizontalAlignment.Center;
                            });
                    })
                    .Add(ScrollContainer, scroll => {
                        scroll.Children()
                            .Add<MarginContainer>(margin => {
                                margin.SetMargin(0, MarginScrollBar, MarginScrollBar, 0);
                                margin.Children()
                                    .Add(OverlayContent);
                            });
                    });
            })
            .Add<HBoxContainer>(ButtonBar, buttonBar => {
                buttonBar.Visible = false;
                buttonBar.Alignment = BoxContainer.AlignmentMode.End;
                buttonBar.SetAnchorsAndOffsetsPreset(LayoutPreset.TopRight);
                buttonBar.Children()
                    .Button<CheckButton>("f", () => {
                        if (IsFollowing) StopFollowing();
                        else Follow();
                    }, (button) => {
                        button.FocusMode = FocusModeEnum.None;
                        button.TooltipText = "Follow";
                        FollowButton = button;
                        UpdateFollowButtonState();
                    })
                    .Button("o", () => {
                        var newState = ((int)_visibilityState + 1) % VisibilityStateEnumSize;
                        VisibilityState = (VisibilityStateEnum)newState;
                    }, (button) => {
                        button.FocusMode = FocusModeEnum.None;
                        button.TooltipText = "Opacity";
                    })
                    .Button("s", () => DebugOverlayManager.SoloOverlay(Id),
                        (button) => {
                            button.FocusMode = FocusModeEnum.None;
                            button.TooltipText = "Solo mode";
                        })
                    .Button("*", () => DebugOverlayManager.ShowAllOverlays(), (button) => {
                        button.FocusMode = FocusModeEnum.None;
                        button.TooltipText = "Open all";
                    })
                    .Button("x", () => DebugOverlayManager.CloseOrHideOverlay(Id), (button) => {
                        button.FocusMode = FocusModeEnum.None;
                        button.TooltipText = "Close";
                    })
                    .Add<Control>(control => control.CustomMinimumSize = new Vector2(4, 0));
                
            });
        MouseFilter = MouseFilterEnum.Pass;
        OverlayContent.Resized += FitContent;
        FitContent();
        CheckProcessBasedOnVisibility();
        VisibilityChanged += CheckProcessBasedOnVisibility;
    }

    public void PostInject() {
        _dragAndDropController = new DragAndDropController().WithMouseButton(MouseButton.Left).OnlyIf(DragPredicate);
        _dragAndDropController.OnStartDrag += OnStartDrag;
        _dragAndDropController.OnDrag += OnDrag;
        DefaultNotificationsHandler.Instance.OnWMMouseExit += () => _dragAndDropController.ForceDrop();
        DefaultNotificationsHandler.Instance.OnWMWindowFocusOut += () => _dragAndDropController.ForceDrop(); 
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
        var limitX = Size.X >= screenSize.X ? 20 : Size.X;
        var limitY = Size.Y >= screenSize.Y ? 20 : Size.Y;
        // Ensure the user can't drag and drop the overlay outside of the screen
        newPosition = new Vector2(
            Mathf.Clamp(newPosition.X, -origin.X, -origin.X + screenSize.X - limitX),
            Mathf.Clamp(newPosition.Y, -origin.Y, -origin.Y + screenSize.Y - limitY));
        _position = newPosition;
    }

    private bool DragPredicate(InputEvent input) =>
        input.IsMouseInside(TitleBackground) && !input.IsMouseInside(ButtonBar);


    public void Destroy() {
        OnDestroyEvent?.Invoke();
        QueueFree();
    }
    
    public override void _Process(double delta) {
        if ((Target != null && !IsInstanceValid(Target)) || (DestroyIfFunc != null && DestroyIfFunc())) {
            Destroy();
        } else {
            if (IsFollowing) {
                var newPosition = FollowPosition + _position;
                // Ensure the overlay doesn't go out of the screen when following the node
                var screenSize = GetTree().Root.Size;
                var limitX = Size.X >= screenSize.X ? 20 : Size.X; 
                var limitY = Size.Y >= screenSize.Y ? 20 : Size.Y; 
                newPosition = new Vector2(
                    Mathf.Clamp(newPosition.X, 0, screenSize.X - limitX),
                    Mathf.Clamp(newPosition.Y, 0, screenSize.Y - limitY));
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