using System;
using System.Collections.Generic;
using Betauer.DI.Attributes;
using Betauer.Input;
using Betauer.NodePath;
using Godot;

namespace Veronenger.Game.UI;

public partial class BottomBar : CanvasLayer {
	[NodePath("%ActionHint1")] private ActionHint _actionHint1;
	[NodePath("%ActionHint2")] private ActionHint _actionHint2;
	[NodePath("%ActionHint3")] private ActionHint _actionHint3;
	[NodePath("%ActionHint4")] private ActionHint _actionHint4;
	private readonly List<ActionHint> _actionHintList = new();
	
	[Inject] private UiActions UiActions { get; set; }
	private InputAction UiAccept => UiActions.UiAccept;
	private InputAction UiCancel => UiActions.UiCancel;


	public override void _Ready() {
		_actionHintList.Add(_actionHint1);
		_actionHintList.Add(_actionHint2);
		_actionHintList.Add(_actionHint3);
		_actionHintList.Add(_actionHint4);
		HideAll();
	}

	public BottomBar HideAll() {
		_actionHint1.Visible = _actionHint2.Visible = _actionHint3.Visible = _actionHint4.Visible = false;
		return this;
	}

	public BottomBar AddButton(string? label1, InputAction inputAction, string? label2, bool animate = true) {
		ActionHint hint = _actionHintList.Find(actionHint => !actionHint.Visible)!;
		hint.Labels(label1, label2).InputAction(inputAction, animate);
		hint.Visible = true;
		return this;
	}

	public BottomBar AddAxis(string? label1, AxisAction axisAction, string? label2, bool animate = true) {
		ActionHint hint = _actionHintList.Find(actionHint => !actionHint.Visible)!;
		hint.Labels(label1, label2).AxisAction(axisAction, animate);
		hint.Visible = true;
		return this;
	}

	// TODO: i18n
	public void AcceptBack() {
		HideAll()
			.AddButton(null, UiAccept, "Accept")
			.AddButton(null, UiCancel, "Back");
	}

	public void AcceptCancel() {
		HideAll()
			.AddButton(null, UiAccept, "Accept")
			.AddButton(null, UiCancel, "Cancel");
	}

	public void ChangeBack() {
		HideAll()
			.AddButton(null, UiAccept, "Change")
			.AddButton(null, UiCancel, "Back");
	}

	public void UpdateState(MainState to) {
		switch (to) {
			case MainState.ModalExitDesktop:
			case MainState.ModalQuitGame:
				AcceptCancel();
				break;
			case MainState.MainMenu:
			case MainState.PauseMenu:
				AcceptBack();
				break;
			default:
				HideAll();
				break;
		}
	}
}
