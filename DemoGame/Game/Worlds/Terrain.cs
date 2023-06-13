using System;
using Godot;
using Veronenger.Game.Platform;

namespace Veronenger.Game.Worlds; 

public partial class Terrain : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void StartNewGame() {
	}

	public void LoadGame(PlatformSaveGameConsumer consumer) {
	}
}