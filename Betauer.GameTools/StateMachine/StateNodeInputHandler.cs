using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Betauer.StateMachine.Async;
using Godot;

namespace Betauer.StateMachine; 

public class StateNodeInputHandler {
        
    private readonly Action<InputEvent>? _input;
    private readonly Action<InputEvent>? _unhandledInput;
    
    private readonly Action<IEnumerable<InputEvent>>? _inputBatch;
    private readonly Action<IEnumerable<InputEvent>>? _unhandledInputBatch;

    private readonly List<InputEvent> _inputBatchList = new();
    private readonly List<InputEvent> _unhandledInputBatchList = new();
        
    public StateNodeInputHandler(
        Action<InputEvent> input,
        Action<InputEvent> unhandledInput,
        Action<IEnumerable<InputEvent>>? inputBatch,
        Action<IEnumerable<InputEvent>>? unhandledInputBatch) {
        _input = input;
        _unhandledInput = unhandledInput;
        _inputBatch = inputBatch;
        _unhandledInputBatch = unhandledInputBatch;
    }

    public void _Input(InputEvent e) {
        _input?.Invoke(e);
        if (_inputBatch != null) _inputBatchList.Add(e);
    }

    public void _UnhandledInput(InputEvent e) {
        _unhandledInput?.Invoke(e);
        if (_unhandledInputBatch != null) _unhandledInputBatchList.Add(e);
    }

    public void _InputBatch() {
        _inputBatch?.Invoke(_inputBatchList);
        _inputBatchList.Clear();
    }

    public void _UnhandledInputBatch() {
        _unhandledInputBatch?.Invoke(_unhandledInputBatchList);
        _unhandledInputBatchList.Clear();
    }
}