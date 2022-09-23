using Betauer.Input;
using Betauer.Signal;
using Godot;
using Color = Godot.Color;

namespace Betauer.Application.Monitor {
    public class DebugOverlay : PopupPanel {
        public readonly MonitorList MonitorList = new MonitorList();
        public readonly int Id;
        public readonly Label Label;

        public Color Transparent = new Color(1, 1, 1, 0.490196f);
        public Color Solid = new Color(1, 1, 1);

        private readonly Mouse.InsideControl _mouseInsidePanel;
        private Vector2? _startDragPosition = null;
        private readonly DebugOverlayManager _manager;
        
        internal DebugOverlay(DebugOverlayManager manager, int id) {
            _manager = manager;
            _mouseInsidePanel = new Mouse.InsideControl(this);

            Id = id; 
            Name = $"DebugOverlay-{id}";
            MouseFilter = MouseFilterEnum.Pass;
            Modulate = new Color(1, 1, 1, 0.490196f);
            RectPosition = new Vector2(id * 64, id * 64);

            Label = new Label();
            Label.Name = "Label";
            AddChild(Label);
        }

        public override void _Ready() {
            PauseMode = PauseModeEnum.Process;
            Disable();
        }

        public override void _Input(InputEvent @event) {
            if (@event.IsDoubleClick(ButtonList.Left)) {
                Modulate = Modulate.a <= 0.9f ? Solid : Transparent;
                
            } else if (_mouseInsidePanel.Inside && @event.IsClick(ButtonList.Left)) {
                if (@event.IsJustPressed()) {
                    _startDragPosition = RectPosition - GetGlobalMousePosition();
                    Raise();
                } else {
                    _startDragPosition = null;
                }
                
            } else if (_mouseInsidePanel.Inside && _startDragPosition != null && @event.IsMouseMotion()) {
                var newPosition = GetGlobalMousePosition() + _startDragPosition.Value;
                newPosition = new Vector2(
                    Mathf.Clamp(newPosition.x, 0, GetTree().Root.Size.x - 100),
                    Mathf.Clamp(newPosition.y, 0, GetTree().Root.Size.y - 100));
                SetPosition(newPosition);
            }
        }

        public void Enable(bool enabled = true) {
            Visible = enabled;
            SetProcess(enabled);
        }

        public void Disable() {
            Enable(false);
        }

        public IMonitor Add(IMonitor monitor) => MonitorList.Add(monitor);

        public override void _Process(float delta) {
            if (!Visible) {
                Disable();
                return;
            }
            Label.Text = string.Join("\n", MonitorList.GetText());
            // Hack time: set a very small size to ensure the panel is resized big enough for the data inside
            RectSize = Vector2.Zero;
        }
    }
}