using Betauer.DI;
using Betauer.Input;
using Godot;

namespace Betauer.Application.Monitor {
    public class DebugOverlay : CanvasLayer {
        public readonly MonitorList MonitorList = new MonitorList();

        public PanelContainer Panel;
        public Label Label;
        
        [Inject(Nullable = true)]
        public InputAction? DebugOverlayAction { get; set; }
        
        public DebugOverlay() {
            Layer = 1000;
            Label = new Label();
            Label.Name = "Label";
            Label.MarginLeft = 7.0f;
            Label.MarginTop = 7.0f;
            Label.MarginRight = 98.0f;
            Label.MarginBottom = 38.0f;

            Panel = new PanelContainer();
            Panel.Name = "PanelContainer";
            Panel.Modulate = new Color(1, 1, 1, 0.490196f);
            Panel.MarginRight = 105f;
            Panel.MarginBottom = 45;
            Panel.AddChild(Label);
            AddChild(Panel);
        }

        public override void _Ready() {
            PauseMode = PauseModeEnum.Process;
            Disable();
        }

        public override void _Input(InputEvent @event) {
            if (DebugOverlayAction != null && DebugOverlayAction.IsEventPressed(@event)) {
                Enable(!Visible);
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

        public override void _PhysicsProcess(float delta) {
            if (!Visible) {
                Disable();
                return;
            }
            Label.Text = string.Join("\n", MonitorList.GetText());
        }
    }
}