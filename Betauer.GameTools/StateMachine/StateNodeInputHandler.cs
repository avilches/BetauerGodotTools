using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.StateMachine;

public class StateNodeInputHandler {
    private readonly Action<InputEvent>? _input;
    private readonly Action<InputEvent>? _shortcutInput;
    private readonly Action<InputEvent>? _unhandledInput;
    private readonly Action<InputEvent>? _unhandledKeyInput;
    private readonly Action<IEnumerable<InputEvent>>? _inputBatch;
    private readonly Action<IEnumerable<InputEvent>>? _shortcutInputBatch;
    private readonly Action<IEnumerable<InputEvent>>? _unhandledInputBatch;
    private readonly Action<IEnumerable<InputEvent>>? _unhandledInputKeyBatch;
    private readonly List<InputEvent> _inputBatchList = new();
    private readonly List<InputEvent> _shortcutInputBatchList = new();
    private readonly List<InputEvent> _unhandledInputBatchList = new();
    private readonly List<InputEvent> _unhandledInputKeyBatchList = new();

    public StateNodeInputHandler(
        Action<InputEvent>? input,
        Action<InputEvent>? shortcutInput,
        Action<InputEvent>? unhandledInput,
        Action<InputEvent>? unhandledKeyInput,
        Action<IEnumerable<InputEvent>>? inputBatch,
        Action<IEnumerable<InputEvent>>? shortcutInputBatch,
        Action<IEnumerable<InputEvent>>? unhandledInputBatch,
        Action<IEnumerable<InputEvent>>? unhandledInputKeyBatch
    ) {
        _input = input;
        _shortcutInput = shortcutInput;
        _unhandledInput = unhandledInput;
        _unhandledKeyInput = unhandledKeyInput;
        _inputBatch = inputBatch;
        _shortcutInputBatch = shortcutInputBatch;
        _unhandledInputBatch = unhandledInputBatch;
        _unhandledInputKeyBatch = unhandledInputKeyBatch;
    }

    public void _Input(InputEvent e) =>
        HandleEvent(_input, _inputBatch, _inputBatchList, e);

    public void _ShortcutInput(InputEvent e) =>
        HandleEvent(_shortcutInput, _shortcutInputBatch, _shortcutInputBatchList, e);

    public void _UnhandledInput(InputEvent e) =>
        HandleEvent(_unhandledInput, _unhandledInputBatch, _unhandledInputBatchList, e);

    public void _UnhandledKeyInput(InputEvent e) =>
        HandleEvent(_unhandledKeyInput, _unhandledInputKeyBatch, _unhandledInputKeyBatchList, e);

    public void _InputBatch() =>
        HandleBatch(_inputBatch, _inputBatchList);

    public void _ShortcutInputBatch() =>
        HandleBatch(_shortcutInputBatch, _shortcutInputBatchList);

    public void _UnhandledInputBatch() =>
        HandleBatch(_unhandledInputBatch, _unhandledInputBatchList);

    public void _UnhandledKeyInputBatch() =>
        HandleBatch(_unhandledInputKeyBatch, _unhandledInputKeyBatchList);

    private static void HandleEvent(Action<InputEvent>? input, Action<IEnumerable<InputEvent>>? inputBatch, ICollection<InputEvent> inputBatchList, InputEvent e) {
        input?.Invoke(e);
        if (inputBatch != null) inputBatchList.Add(e);
    }

    private static void HandleBatch(Action<IEnumerable<InputEvent>>? inputBatch, ICollection<InputEvent> inputBatchList) {
        if (inputBatch != null && inputBatchList.Count > 0) {
            inputBatch.Invoke(inputBatchList);
            inputBatchList.Clear();
        }
    }
}