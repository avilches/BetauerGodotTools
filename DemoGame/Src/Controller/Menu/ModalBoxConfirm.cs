using System.Threading.Tasks;
using Betauer.Animation;
using Betauer.DI;
using Betauer.Input;
using Betauer.OnReady;
using Betauer.Core.Signal;
using Betauer.UI;
using Godot;

namespace Veronenger.Controller.Menu {
	public partial class ModalBoxConfirm : CanvasLayer {
		// [OnReady("Panel/VBoxContainer/Menu")]
		// private Godot.Container _menuBase;

		[OnReady("%ContainerDouble")]
		private Godot.Container _containerDouble;
		[OnReady("%DTitle")]
		private Label _titleDouble;
		[OnReady("%DSubtitle")]
		private Label _subtitleDouble;

		[OnReady("%ContainerSingle")]
		private Godot.Container _containerSingle;
		[OnReady("%STitle")]
		private Label _titleSingle;

		[OnReady("ColorRect")] private ColorRect _colorRect;

		[OnReady("%Accept")] private Button _acceptButton;
		[OnReady("%Cancel")] private Button _cancelButton;

		// private MenuContainer _menuContainer;

		[Inject] private InputAction UiCancel { get; set; }

		private readonly TaskCompletionSource<bool> _promise = new();

		private string _titleText;
		private string? _subtitleText;
		private string _acceptText = "Accept";
		private string _cancelText = "Cancel";
		private Style _style = Style.None;

		public enum Style {
			FadeToBlack,
			Dim,
			None                          
		}

		public ModalBoxConfirm Title(string title, string subtitle = null) {
			_titleText = title;
			_subtitleText = subtitle;
			return this;
		}

		public ModalBoxConfirm FadeToBlack() {
			_style = Style.FadeToBlack;
			return this;
		}

		public ModalBoxConfirm Dim() {
			_style = Style.Dim;
			return this;
		}

		public ModalBoxConfirm Buttons(string accept, string cancel) {
			_acceptText = accept;
			_cancelText = cancel;
			return this;
		}
		
		private Tween _sceneTreeTween;
		public override void _Ready() {
			// TODO i18n
			if (_subtitleText != null) {
				_containerSingle.Visible = false;
				_containerDouble.Visible = true;
				_titleDouble.Text = _titleText;
				_subtitleDouble.Text = _subtitleText;
			} else {
				_containerDouble.Visible = false;
				_containerSingle.Visible = true;
				_titleSingle.Text = _titleText;
			}
			_acceptButton.Text = $"  {_acceptText}  ";
			_cancelButton.Text = $"  {_cancelText}  ";
			_acceptButton.OnPressed(() => _promise.TrySetResult(true));
			_cancelButton.OnPressed(() => _promise.TrySetResult(false));
			
			if (_style == Style.Dim) {
				_colorRect.Visible = true;
				_sceneTreeTween?.Kill();
				_sceneTreeTween = Templates.FadeOut.Play(_colorRect, 0f, 1f);
			} else if (_style == Style.FadeToBlack) {
				_colorRect.Visible = true;
				_sceneTreeTween?.Kill();
				_sceneTreeTween = Templates.FadeOut.Play(_colorRect, 0f, 1f);
			} else {
				_colorRect.Visible = false;
			}
		}

		public Task<bool> AwaitResult() {
			return _promise.Task;
		}

		public override void _Process(double delta) {
			if (UiCancel.IsJustPressed()) {
				_promise.TrySetResult(false);
			}
		}
	}
}
