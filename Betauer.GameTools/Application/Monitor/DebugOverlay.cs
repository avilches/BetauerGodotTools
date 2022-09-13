using Betauer.DI;
using Betauer.Input;
using Betauer.Signal;
using Godot;
using Color = Godot.Color;

namespace Betauer.Application.Monitor {
    public class DebugOverlay : CanvasLayer {
        public readonly MonitorList MonitorList = new MonitorList();

        public PopupPanel Panel;
        public Label Label;
        
        [Inject(Nullable = true)]
        public InputAction? DebugOverlayAction { get; set; }

        [Inject(Nullable = true)]
        public InputAction? LMB { get; set; }

        private bool _mouseInsidePanel = false;
        private Vector2? _startDragPosition = null;
        private readonly Color _transparent = new Color(1, 1, 1, 0.490196f);
        private readonly Color _solid = new Color(1, 1, 1);
        public DebugOverlay() {
            Name = "DebugOverlay";
            Layer = 1000;

            Panel = new PopupPanel();
            Panel.MouseFilter = Control.MouseFilterEnum.Pass;
            Panel.Name = "PanelContainer";
            Panel.Modulate = new Color(1, 1, 1, 0.490196f);
            Panel.OnMouseEntered(() => _mouseInsidePanel = true);
            Panel.OnMouseExited(() => _mouseInsidePanel = false);
            AddChild(Panel);

            var vbox = new VBoxContainer();
            Panel.AddChild(vbox);
                
            Label = new Label();
            Label.Name = "Label";
            vbox.AddChild(Label);
        }

        public override void _Ready() {
            PauseMode = PauseModeEnum.Process;
            Disable();
        }

        public override void _Input(InputEvent @event) {
            if (DebugOverlayAction != null && DebugOverlayAction.IsEventPressed(@event)) {
                Enable(!Panel.Visible);
                
            } else if (@event.IsDoubleClick(ButtonList.Left)) {
                Panel.Modulate = Panel.Modulate.a <= 0.9f ? _solid : _transparent;
                
            } else if (_mouseInsidePanel && @event.IsClick(ButtonList.Left)) {
                if (@event.IsJustPressed()) {
                    _startDragPosition = Panel.RectPosition - Panel.GetGlobalMousePosition();
                } else {
                    _startDragPosition = null;
                }
                
            } else if (_mouseInsidePanel && _startDragPosition != null && @event.IsMouseMotion()) {
                var newPosition = Panel.GetGlobalMousePosition() + _startDragPosition.Value;
                newPosition = new Vector2(
                    Mathf.Clamp(newPosition.x, 0, GetTree().Root.Size.x - 100),
                    Mathf.Clamp(newPosition.y, 0, GetTree().Root.Size.y - 100));
                Panel.SetPosition(newPosition);
            }
        }

        public void Enable(bool enabled = true) {
            Panel.Visible = enabled;
            SetProcess(enabled);
        }

        public void Disable() {
            Enable(false);
        }

        public IMonitor Add(IMonitor monitor) => MonitorList.Add(monitor);

        public override void _Process(float delta) {
            if (!Panel.Visible) {
                Disable();
                return;
            }
            Label.Text = string.Join("\n", MonitorList.GetText());
            // Hack time: set a very small size to ensure the panel is resized big enough for the data inside
            Panel.RectSize = Vector2.Zero;
        }
    }
}