using System.Linq;
using Betauer.Application.Lifecycle;
using Betauer.Core;
using Betauer.Core.Nodes;
using Betauer.Core.Pool;
using Betauer.DI;
using Betauer.DI.Factory;
using Godot;
using Veronenger.Managers;
using Veronenger.Persistent;

namespace Veronenger.Worlds;

public partial class World3 : Node {
	[Inject] private Game Game { get; set; }
	[Inject] private ItemRepository ItemRepository { get; set; }
	[Inject] private PlatformManager PlatformManager { get; set; }
	[Inject] private StageManager StageManager { get; set; }
	[Inject] private IPool<PickableItemNode> PickableItemNodeFactory { get; set; }

	public override void _Ready() {
		GetNode("EnemySpawn").GetChildren().OfType<Marker2D>().ForEach(m => {
			if (m.Visible) Game.ZombieSpawn(this, m.GlobalPosition);
		});

		GetChildren().OfType<TileMap>().ForEach(PlatformManager.ConfigureTileMapCollision);
		GetChildren().OfType<CanvasModulate>().ForEach(cm => cm.Visible = true);
		GetNode<Node>("Lights").GetChildren().OfType<PointLight2D>().ForEach(light => {
			if (light.Name.ToString().StartsWith("Candle")) {
				CandleOff(light);
			}
		});
		PlatformManager.ConfigurePlatformsCollisions();
		GetNode<Node>("Stages").GetChildren().OfType<Area2D>().ForEach(StageManager.ConfigureStage);

		var metalbarMarker = GetNode<Marker2D>("MetalbarSpawn");
		var metalbarPosition = metalbarMarker.GlobalPosition;
		RemoveChild(metalbarMarker);
		var pickableItemNode = PickableItemNodeFactory.Get();
		ItemRepository.AddRangeWeapon()
		pickableItemNode.AddToScene(this, metalbarPosition);
	}
	
	private void CandleOff(PointLight2D light) {
		light.Enabled = true;
		light.Color = new Color("ffd1c8");
		light.TextureScale = 0.2f;
		light.ShadowEnabled = true;
		light.ShadowFilter = Light2D.ShadowFilterEnum.None;
		light.GetNode<Area2D>("Area2D")
			?.OnBodyEntered(LayerConstants.LayerPlayerBody, (player) => {
				if (player is CharacterBody2D character && ItemRepository.IsPlayer(character)) CandleOn(light);
			});
	}

	private void CandleOn(PointLight2D light) {
		light.Enabled = true;
		light.Color = new Color("ffd1c8");
		light.TextureScale = 0.8f;
		// light.ShadowEnabled = false;
	}

	
}
