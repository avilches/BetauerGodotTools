using System.Threading.Tasks;
using Betauer.Core.Restorer;
using Betauer.DI.Attributes;
using Betauer.Input;
using Betauer.NodePath;
using Betauer.UI;
using Godot;
using Container = Godot.Container;

namespace Veronenger.Game.UI; 

public partial class PauseMenu : CanvasFaderLayer {
	[NodePath("CenterContainer")]
	private Container _centerContainer;

	[NodePath("CenterContainer/VBoxContainer/Menu")]
	private Container _menuBase;

	[NodePath("CenterContainer/VBoxContainer/Title")]
	private Label _title;

	private MenuContainer _menuContainer;

	[Inject] private MainBus MainBus { get; set; }

	[Inject] private InputAction UiAccept { get; set; }
	[Inject] private InputAction UiCancel { get; set; }
	[Inject] private InputAction ControllerStart { get; set; }

	public override void _Ready() {
		_menuContainer = BuildMenu();
		Hide();
	}

	public Task ShowPauseMenu() {
		Show();
		FadeBackgroundOut(0.4f, 0.5f);
		return _menuContainer.Start();
	}

	public void HidePauseMenu() {
		// _launcher.RemoveAll();
		Hide();
	}

	private Restorer _menuRestorer;

	public void DisableMenus() {
		_menuRestorer = _menuContainer.ActiveMenu.DisableButtons();
	}

	public void EnableMenus() {
		_menuRestorer?.Restore();
	}

	public MenuContainer BuildMenu() {
		// TODO i18n
		_title.Text = "Paused";
		foreach (var child in _menuBase.GetChildren()) (child as Node)?.Free();

		var mainMenu = new MenuContainer(_menuBase);
		var startMenu = mainMenu.GetRootMenu();
		startMenu.AddButton("Resume", "Resume").Pressed += () => MainBus.Publish(MainEvent.ClosePauseMenu);
		startMenu.AddButton("Settings", "Settings").Pressed += () => MainBus.Publish(MainEvent.OpenSettingsMenu);
		startMenu.AddButton("QuitGame", "Quit game").Pressed += () => MainBus.Publish(MainEvent.OpenModalBoxQuitGame);
		return mainMenu;
	}

	public void OnInput(InputEvent e) {
		if (UiCancel.IsEventJustPressed(e)) {
			if (_menuContainer.IsRootMenuActive()) {
				MainBus.Publish(MainEvent.ClosePauseMenu);
			} else {
				_menuContainer.Back();
			}
			GetViewport().SetInputAsHandled();

		} else if (ControllerStart.IsJustPressed) {
			MainBus.Publish(MainEvent.ClosePauseMenu);
			GetViewport().SetInputAsHandled();
				
		}
	}
}