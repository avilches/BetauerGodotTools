using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.FSM;

public class StateNodeInputHandler {
    private readonly Action<InputEvent>? _input;
    private readonly Action<InputEvent>? _shortcutInput;
    private readonly Action<InputEvent>? _unhandledInput;
    private readonly Action<InputEvent>? _unhandledKeyInput;

    public StateNodeInputHandler(
        Action<InputEvent>? input,
        Action<InputEvent>? shortcutInput,
        Action<InputEvent>? unhandledInput,
        Action<InputEvent>? unhandledKeyInput
    ) {
        _input = input;
        _shortcutInput = shortcutInput;
        _unhandledInput = unhandledInput;
        _unhandledKeyInput = unhandledKeyInput;
    }

    public void _Input(InputEvent e) =>
        HandleEvent(_input, e);

    public void _ShortcutInput(InputEvent e) =>
        HandleEvent(_shortcutInput, e);

    public void _UnhandledInput(InputEvent e) =>
        HandleEvent(_unhandledInput, e);

    public void _UnhandledKeyInput(InputEvent e) =>
        HandleEvent(_unhandledKeyInput, e);

    private static void HandleEvent(Action<InputEvent>? input, InputEvent e) {
        input?.Invoke(e);
    }
}