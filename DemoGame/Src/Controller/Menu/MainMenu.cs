using System.Threading.Tasks;
using Betauer.Animation;
using Betauer.Application;
using Betauer.DI;
using Betauer.Input;
using Betauer.OnReady;
using Betauer.Core.Restorer;
using Betauer.Core.Signal;
using Betauer.UI;
using Godot;
using Veronenger.Managers;

namespace Veronenger.Controller.Menu {
	public partial class MainMenu : CanvasFaderLayer {
		private const float FadeMainMenuEffectTime = 0.75f;

		[OnReady("%Menu")]
		private Godot.Container _menuBase;

		[OnReady("%Version")]
		private Label _version;

		[OnReady("%Author")]
		private Label _author;

		[OnReady("%Title")]
		private Label _title;

		private MenuContainer _menuContainer;

		[Inject] private Bus Bus { get; set; }

		[Inject] private InputAction UiAccept { get; set; }
		[Inject] private InputAction UiCancel { get; set; }

		public override void _Ready() {
			_version.Text = $"{AppTools.GetProjectName()} {AppTools.GetProjectVersion()} - Betauer 2022";
			_menuContainer = BuildMenu();
		}

		public async Task ShowMenu() {
			GetTree().Root.GuiDisableInput = true;
			Visible = true;
			await _menuContainer.Start();
			GetTree().Root.GuiDisableInput = false;
		}

		public async Task HideMainMenu() {
			GetTree().Root.GuiDisableInput = true;
			await Templates.FadeOut.Play(this, 0f, FadeMainMenuEffectTime).AwaitFinished();
			Visible = false;
			GetTree().Root.GuiDisableInput = false;
		}

		private Restorer _menuRestorer;

		public void DisableMenus() {
			_menuRestorer = _menuContainer.ActiveMenu.DisableButtons();
		}

		public void EnableMenus() {
			_menuRestorer?.Restore();
		}

		public MenuContainer BuildMenu() {
			foreach (var child in _menuBase.GetChildren()) (child as Node)?.Free();

			var mainMenu = new MenuContainer(_menuBase);
			var startMenu = mainMenu.GetRootMenu();
			startMenu.AddButton("Start", "Start").OnPressed(() => Bus.Publish(MainEvent.StartGame));
			startMenu.AddButton("Settings", "Settings").OnPressed(() => Bus.Publish(MainEvent.Settings));
			startMenu.AddButton("Exit", "Exit").OnPressed(() => Bus.Publish(MainEvent.ExitDesktop));
			return mainMenu;
		}

		public void OnInput(InputEvent e) {
			if (UiCancel.IsEventJustPressed(e)) { 
				if (_menuContainer.IsRootMenuActive()) {
					Bus.Publish(MainEvent.ModalBoxConfirmExitDesktop);
				} else {
					_menuContainer.Back();
				}
				GetViewport().SetInputAsHandled();
			}
		}

	}
}
