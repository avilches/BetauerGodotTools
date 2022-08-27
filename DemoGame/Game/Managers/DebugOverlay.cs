using System;
using System.Collections.Generic;
using Betauer.DI;
using Betauer.Input;
using Betauer.Loader;
using Betauer.OnReady;
using Godot;

namespace Veronenger.Game.Managers {
	public class Monitor {
		internal readonly Node Node;
		internal readonly Func<string> Delegate;
		internal bool IsEnabled  => _isEnabled && Node.IsInsideTree();
		internal bool IsDestroyed => _isDestroyed || !Godot.Object.IsInstanceValid(Node);
		private bool _isEnabled = true;
		private bool _isDestroyed = false;
		internal readonly string _label;
		public Monitor(string label, Node node, Func<string> @delegate) {
			_label = label;
			Node = node;
			Delegate = @delegate;
		}

		public override string ToString() {
			try {
				return $"{_label}: {Delegate()}";
			} catch (Exception e) {
				return $"{_label}: {e.Message}";
			}
		}

		public void Disable() => _isEnabled = false;
		public void Enable() => _isEnabled = true;
		public void Destroy() => _isDestroyed = true;
	}

	public class DebugOverlay : CanvasLayer {
		private readonly List<Monitor> _monitors = new List<Monitor>();

		[OnReady("PanelContainer")] private PanelContainer _panelContainer;
		[OnReady("PanelContainer/Label")] private Label? _label;
		[Inject] private InputAction DebugOverlayAction { get; set; }

		private bool _visible = false;
		public bool Visible {
			get => _visible;
			set {
				_visible = value;
				SetProcess(value);
				if (_panelContainer != null) {
					if (_visible) _panelContainer.Show();
					else _panelContainer.Hide();
				}
			}
		}

		public override void _Ready() {
			Visible = false;
		}

		public Monitor Add(string label, Node node, string property) {
			var monitor = new Monitor(label, node, ()=> node.GetIndexed(property).ToString());
			_monitors.Add(monitor);
			return monitor;
		}

		public Monitor Add(string label, Node node, Func<string> action) {
			var monitor = new Monitor(label, node, action);
			_monitors.Add(monitor);
			return monitor;
		}

		public override void _Input(InputEvent @event) {
			if (DebugOverlayAction.IsEventPressed(@event)) Visible = !Visible;
		}

		public override void _Process(float delta) {
			if (!_visible) {
				SetProcess(false);
				return;
			}
			var labelText = "";
			labelText += $"FPS {Engine.GetFramesPerSecond()}\n";
			labelText += $"Static Memory: {OS.GetStaticMemoryUsage()}\n";

			_monitors.RemoveAll(monitor => {
				if (monitor.IsDestroyed) return true;
				if (monitor.IsEnabled) {
					labelText += monitor + "\n";
				}
				return false;
			});
			_label.Text = labelText;
		}
	}
}