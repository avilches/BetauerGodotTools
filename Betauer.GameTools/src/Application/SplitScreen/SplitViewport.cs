using System;
using Betauer.Core.Nodes;
using Godot;

namespace Betauer.Application.SplitScreen;

public partial class SplitViewport : BaseSplitScreen {
	private readonly World2D _noWorld = new(); // A cached World2D to re-use
	private Node? _nodeWorld1 = null;

	public SubViewportContainer SubViewportContainer1 { get; } = new() {
		Name = "SubViewportContainer1"
	};

	public SubViewportContainer SubViewportContainer2 { get; } = new() {
		Name = "SubViewportContainer2"
	};

	public SubViewport SubViewport1 { get; } = new() {
		Name = "SubViewport1",
		RenderTargetUpdateMode = SubViewport.UpdateMode.Always,
		HandleInputLocally = false
	};

	public SubViewport SubViewport2 { get; } = new() {
		Name = "SubViewport2",
		RenderTargetUpdateMode = SubViewport.UpdateMode.Always,
		HandleInputLocally = false
	};

	public Camera2D Camera1 { get; } = new() {
		Name = "Camera1"
	};

	public Camera2D Camera2 { get; } = new() {
		Name = "Camera2"
	};

	public SplitViewport(Node parent, Func<Vector2> getParentSize) {
		GetParentSize = getParentSize;
		parent.AddChild(SubViewportContainer1);
		parent.AddChild(SubViewportContainer2);
		SubViewportContainer1.AddChild(SubViewport1);
		SubViewportContainer2.AddChild(SubViewport2);
		SubViewport1.AddChild(Camera1);
		SubViewport2.AddChild(Camera2);
	}

	public void SetCommonWorld(Node node) {
		_nodeWorld1?.RemoveFromParent();
		_nodeWorld1 = node;
		SubViewport1.AddChild(node);
	}

	protected override bool ToSingle() {
		if (SubViewport1 == null) return false;

		var windowSize = GetParentSize();

		var full = new Vector2I((int)windowSize.X, (int)windowSize.Y);
		SubViewportContainer2.Visible = false;
		SubViewport2.World2D = _noWorld;

		SubViewport1.Size = full; // Always change the subviewport size first, then the parent container size!
		SubViewportContainer1.Position = Vector2.Zero;
		SubViewportContainer1.Size = full;
		return true;
	}

	protected override bool ToSplitScreen() {
		if (SubViewport1 == null || SubViewport2 == null) return false;

		var windowSize = GetParentSize();
		var splitSize = Horizontal ? new Vector2I((int)windowSize.X, (int)windowSize.Y / 2) : new Vector2I((int)windowSize.X / 2, (int)windowSize.Y);

		SubViewportContainer2.Visible = true;
		SubViewport2.World2D = SubViewport1.World2D;

		SubViewport1.Size = splitSize; // Always change the subviewport size first, then the parent container size!
		SubViewportContainer1.Position = Vector2.Zero;
		SubViewportContainer1.Size = splitSize;

		SubViewport2.Size = splitSize; // Always change first the subviewport size, then the parent container size!
		SubViewportContainer2.Position = Horizontal ? new Vector2(0, windowSize.Y / 2) : new Vector2(windowSize.X / 2, 0);
		SubViewportContainer2.Size = splitSize;

		return true;
	}
}
