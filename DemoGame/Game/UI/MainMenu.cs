using System.Threading.Tasks;
using Betauer.Application;
using Betauer.Core.Restorer;
using Betauer.DI.Attributes;
using Betauer.NodePath;
using Betauer.UI;
using Godot;
using Veronenger.Game.RTS.World;

namespace Veronenger.Game.UI; 

public partial class MainMenu : CanvasFaderLayer {
	private const float FadeMainMenuEffectTime = 0.75f;

	[NodePath("%Menu")]
	private Godot.Container _menuBase;

	[NodePath("%Version")]
	private Label _version;

	[NodePath("%Author")]
	private Label _author;

	[NodePath("%Title")]
	private Label _title;

	private MenuContainer _menuContainer;

	[Inject] private MainBus MainBus { get; set; }

	[Inject] private UiActions UiActions { get; set; }
	[Inject] private WorldGenerator WorldGenerator { get; set; }
	
	public MainMenu() {
		Visible = false;
	}

	public override void _Ready() {
		_version.Text = $"{AppTools.GetProjectName()} {AppTools.GetProjectVersion()} - Betauer 2022";
		_menuContainer = BuildMenu();
	}

	public async Task ShowMenu() {
		GetViewport().GuiDisableInput = true;
		Visible = true;
		await _menuContainer.Start();
		GetViewport().GuiDisableInput = false;
	}

	public async Task HideMainMenu() {
		GetViewport().GuiDisableInput = true;
		await FadeToBlack(/*FadeMainMenuEffectTime*/0.1f);
		ResetFade();
		Visible = false;
		GetViewport().GuiDisableInput = false;
	}

	private Restorer _menuRestorer;

	public void DisableMenus() {
		_menuRestorer = _menuContainer.ActiveMenu.DisableButtons();
	}

	public void EnableMenus() {
		_menuRestorer?.Restore();
	}

	public MenuContainer BuildMenu() {
		foreach (var child in _menuBase.GetChildren()) child?.Free();

		var mainMenu = new MenuContainer(_menuBase);
		var startMenu = mainMenu.GetRootMenu();
		startMenu.AddButton("Start RTS", "RTS").Pressed += () => MainBus.Publish(MainEvent.TriggerStartGameRts);
		startMenu.AddButton("Start Plat", "Platform").Pressed += () => MainBus.Publish(MainEvent.TriggerStartGamePlatform);
		startMenu.AddButton("Settings", "Settings").Pressed += () => MainBus.Publish(MainEvent.TriggerOpenSettingsMenu);
		startMenu.AddButton("Exit", "Exit").Pressed += () => MainBus.Publish(MainEvent.TriggerExitDesktop);
		return mainMenu;
	}

	public void OnInput(InputEvent e) {
		if (UiActions.UiCancel.IsEventJustPressed(e)) { 
			if (_menuContainer.IsRootMenuActive()) {
				MainBus.Publish(MainEvent.TriggerOpenModalBoxExitToDesktop);
			} else {
				_menuContainer.Back();
			}
			GetViewport().SetInputAsHandled();
		}
	}

}
