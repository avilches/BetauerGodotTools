using System.Threading.Tasks;
using Betauer.DI;
using Betauer.Input;
using Betauer.OnReady;
using Betauer.Core.Restorer;
using Betauer.Core.Signal;
using Betauer.UI;
using Godot;
using Veronenger.Managers;
using Container = Godot.Container;

namespace Veronenger.UI; 

public partial class PauseMenu : CanvasFaderLayer {
	[OnReady("CenterContainer")]
	private Container _centerContainer;

	[OnReady("CenterContainer/VBoxContainer/Menu")]
	private Container _menuBase;

	[OnReady("CenterContainer/VBoxContainer/Title")]
	private Label _title;

	private MenuContainer _menuContainer;

	[Inject] private MainStateMachine MainStateMachine { get; set; }

	[Inject] private InputAction UiAccept { get; set; }
	[Inject] private InputAction UiCancel { get; set; }
	[Inject] private InputAction ControllerStart { get; set; }
	[Inject] private Bus Bus { get; set; }

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
		startMenu.AddButton("Resume", "Resume").OnPressed(() => Bus.Publish(MainEvent.Back));
		startMenu.AddButton("Settings", "Settings").OnPressed(() => Bus.Publish(MainEvent.Settings));
		startMenu.AddButton("QuitGame", "Quit game").OnPressed(() => Bus.Publish(MainEvent.ModalBoxConfirmQuitGame));
		return mainMenu;
	}

	public void OnInput(InputEvent e) {
		if (UiCancel.IsEventJustPressed(e)) {
			if (_menuContainer.IsRootMenuActive()) {
				Bus.Publish(MainEvent.Back);
			} else {
				_menuContainer.Back();
			}
			GetViewport().SetInputAsHandled();

		} else if (ControllerStart.IsJustPressed()) {
			Bus.Publish(MainEvent.Back);
			GetViewport().SetInputAsHandled();
				
		}
	}
}