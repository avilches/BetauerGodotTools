using System.Threading.Tasks;
using Betauer.Animation;
using Betauer.DI;
using Betauer.Input;
using Betauer.NodePath;
using Betauer.Core.Signal;
using Betauer.DI.Attributes;
using Betauer.UI;
using Godot;

namespace Veronenger.UI; 

public partial class ModalBoxConfirm : CanvasFaderLayer {

	[NodePath("%ContainerDouble")]
	private Godot.Container _containerDouble;
	[NodePath("%DTitle")]
	private Label _titleDouble;
	[NodePath("%DSubtitle")]
	private Label _subtitleDouble;

	[NodePath("%ContainerSingle")]
	private Godot.Container _containerSingle;
	[NodePath("%STitle")]
	private Label _titleSingle;

	[NodePath("%Accept")] private Button _acceptButton;
	[NodePath("%Cancel")] private Button _cancelButton;

	[Inject] private InputAction UiCancel { get; set; }

	private readonly TaskCompletionSource<bool> _promise = new();

	private string _titleText;
	private string? _subtitleText;
	private string _acceptText = "Accept";
	private string _cancelText = "Cancel";
	private bool _acceptIsDefault = true;

	public ModalBoxConfirm Title(string title, string subtitle = null) {
		_titleText = title;
		_subtitleText = subtitle;
		return this;
	}

	public ModalBoxConfirm Buttons(string accept, string cancel) {
		_acceptText = accept;
		_cancelText = cancel;
		return this;
	}

	public ModalBoxConfirm SetAcceptIsDefault(bool acceptIsDefault) {
		_acceptIsDefault = acceptIsDefault;
		return this;
	}
		
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
		_acceptButton.Pressed += () => SetResult(true);
		_cancelButton.Pressed += () => SetResult(false);
		if (_acceptIsDefault) {
			_acceptButton.GrabFocus();
		} else {
			_cancelButton.GrabFocus();
		}
	}

	public Task<bool> AwaitResult() {
		return _promise.Task;
	}

	public override void _Input(InputEvent e) {
		if (UiCancel.IsEventJustPressed(e)) {
			SetResult(false);
			GetViewport().SetInputAsHandled();
		}
	}

	private void SetResult(bool result) {
		_promise.TrySetResult(result);
	}
}