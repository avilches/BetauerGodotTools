using System;
using Betauer.Core.Nodes;
using Betauer.NodePath;
using Godot;

namespace Veronenger.Game;

public partial class SplitViewport : Control {
	private static readonly World2D NoWorld = new(); // A cached World2D to re-use
	private Node? _currentWorld = null;
	private bool _configured = false;
	private bool _split = false;
	private bool _horizontal = false;

	public event Action<bool> OnChange;
	public Func<Vector2> GetSize { get; set; }

	[NodePath("%SubViewport1")] public SubViewport SubViewport1 { get; private set; }
	[NodePath("%SubViewport2")] public SubViewport SubViewport2 { get; private set; }
	[NodePath("%Camera2D1")] public Camera2D Camera1 { get; private set; }
	[NodePath("%Camera2D2")] public Camera2D Camera2 { get; private set; }

	public bool BusyPlayerTransition { get; private set; } = false;

	public void SetWorld(Node node) {
		_currentWorld?.RemoveFromParent();
		SubViewport1.AddChild(node);
	}

	public bool Horizontal {
		get => _horizontal;
		set {
			if (_horizontal == value) return;
			_horizontal = value;
			Refresh();
		}
	}

	public bool Split {
		get => _split;
		set {
			if (_configured && _split == value) return;
			_split = value;
			Refresh();
		}
	}
	
	public void Refresh() {
		if (_split) ToSplitScreen();
		else ToSingle();
	}
		
	private void ToSingle() {
		if (GetSize == null) throw new Exception("Please, define a valid GetSize function!");
		if (SubViewport1 == null) return;
		_configured = true;
		
		var windowSize = GetSize();

		var full = new Vector2I((int)windowSize.X, (int)windowSize.Y);
		SubViewport2.GetParent<SubViewportContainer>().Visible = false;
		SubViewport2.World2D = NoWorld;
		
		SubViewport1.Size = full; // Always change first the subviewport size, then the parent container size!
		SubViewport1.GetParent<SubViewportContainer>().Position = Vector2.Zero;
		SubViewport1.GetParent<SubViewportContainer>().Size = full;
		OnChange?.Invoke(false);
		BusyPlayerTransition = false;
	}

	private void ToSplitScreen() {
		if (GetSize == null) throw new Exception("Please, define a valid GetSize function!");
		if (SubViewport1 == null || SubViewport2 == null) return;
		_configured = true;

		var windowSize = GetSize();
		var splitSize = Horizontal ? new Vector2I((int)windowSize.X, (int)windowSize.Y / 2) : new Vector2I((int)windowSize.X / 2, (int)windowSize.Y);
		
		SubViewport2.GetParent<SubViewportContainer>().Visible = true;
		SubViewport2.World2D = SubViewport1.World2D;
		
		SubViewport1.Size = splitSize; // Always change first the subviewport size, then the parent container size!
		SubViewport1.GetParent<SubViewportContainer>().Position = Vector2.Zero;
		SubViewport1.GetParent<SubViewportContainer>().Size = splitSize;
		
		SubViewport2.Size = splitSize; // Always change first the subviewport size, then the parent container size!
		SubViewport2.GetParent<SubViewportContainer>().Position = Horizontal ? new Vector2(0, windowSize.Y / 2) : new Vector2(windowSize.X / 2, 0);
		SubViewport2.GetParent<SubViewportContainer>().Size = splitSize;
		
		OnChange?.Invoke(true);
		BusyPlayerTransition = false;
	}
}
