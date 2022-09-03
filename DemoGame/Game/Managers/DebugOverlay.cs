using System;
using System.Collections.Generic;
using Betauer.DI;
using Betauer.Input;
using Betauer.Loader;
using Betauer.OnReady;
using Godot;

namespace Veronenger.Game.Managers {
	public class Monitor {
		private bool _isEnabled = true;
		private bool _isDestroyed = false;

		public string Label;
		public Godot.Object? Target;
		public Func<string> Delegate;

		public bool IsEnabled => _isEnabled && (Target is Node n ? n.IsInsideTree() : true);
		public bool IsDestroyed => _isDestroyed || (Target != null && !Godot.Object.IsInstanceValid(Target));
		
		public Monitor(string label) {
			Label = label;
		}

		public Monitor Bind(Godot.Object node) {
			Target = node;
			return this;
		}

		public Monitor Do(Node node, NodePath property) {
			return Bind(node)
				.Do(() => node.GetIndexed(property).ToString());
		}

		public Monitor Do(Func<string> action) {
			Delegate = action;
			return this;
		}

		public override string ToString() {
			try {
				return $"{Label}: {Delegate()}";
			} catch (Exception e) {
				return $"{Label}: {e.Message}";
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

		public override void _Ready() {
			Disable();
		}

		public Monitor Add(string label) {
			var monitor = new Monitor(label);
			_monitors.Add(monitor);
			return monitor;
		}

		public Monitor Add(string label, Func<string> action) {
			var monitor = new Monitor(label).Do(action);
			_monitors.Add(monitor);
			return monitor;
		}

		public override void _Input(InputEvent @event) {
			if (DebugOverlayAction.IsEventPressed(@event)) Enable(!Visible);
		}

		public void Enable(bool enabled = true) {
			Visible = enabled;
			SetProcess(enabled);
		}

		public void Disable() {
			Enable(false);
		}

		public override void _Process(float delta) {
			if (!Visible) {
				Disable();
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