using System;
using System.Threading.Tasks;
using Betauer.Application;
using Betauer.Core.Restorer;
using Betauer.Core.Signal;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.Input;
using Betauer.NodePath;
using Betauer.UI;
using Godot;
using Veronenger.Managers;

namespace Veronenger.UI; 

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

	[Inject] private EventBus EventBus { get; set; }

	[Inject] private InputAction UiAccept { get; set; }
	[Inject] private InputAction UiCancel { get; set; }

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
		startMenu.AddButton("Start", "Start").Pressed += () => EventBus.Publish(MainEvent.StartGame);
		startMenu.AddButton("Settings", "Settings").Pressed += () => EventBus.Publish(MainEvent.Settings);
		startMenu.AddButton("Exit", "Exit").Pressed += () => EventBus.Publish(MainEvent.ExitDesktop);
		return mainMenu;
	}

	public void OnInput(InputEvent e) {
		if (UiCancel.IsEventJustPressed(e)) { 
			if (_menuContainer.IsRootMenuActive()) {
				EventBus.Publish(MainEvent.ModalBoxConfirmExitDesktop);
			} else {
				_menuContainer.Back();
			}
			GetViewport().SetInputAsHandled();
		}
	}

}
