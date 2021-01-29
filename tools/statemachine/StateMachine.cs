using Godot;
using System;

public class StateMachine {
    private State _currentState;
    private State _nextState;

    private long frame = 0;

    private DebugConfig config;

    public StateMachine(DebugConfig config) {
        this.config = config;
    }

    public void SetNextState(State nextState) {
        DebugStateFlow("#"+frame+": "+_currentState?.GetType().FullName+".NextState("+nextState.GetType().FullName+")");
        _nextState = nextState;
    }

    public void Execute() {
        frame++;
        if (_nextState != null) {
            DebugStateFlow("#"+frame+": "+_currentState?.GetType().FullName+"._PhysicsProcess(). Detected nextState = "+_nextState.GetType().FullName);
            State nextState = _nextState;
            _nextState = null; 
            ChangeStateTo(nextState);
        } else {
            _currentState?.Execute();
        }
    }

    public void ChangeStateTo(State newState) {
        if (config.DEBUG_STATE_CHANGE || config.DEBUG_STATE_FLOW) {
            GD.Print("#" + frame + ": " + _currentState?.GetType().FullName + ".ChangeState(" +
                           newState.GetType().FullName + ")");
        }

        CallEnd(_currentState);
        _currentState = newState;
        CallStart(_currentState);
        _currentState.Execute();
    }

    public void _UnhandledInput(InputEvent @event) {
        _currentState?._UnhandledInput(@event);
    }

    public void DebugStateFlow(String message) {
        if (config.DEBUG_STATE_FLOW) {
            GD.Print(message);
        }
    }

    private void CallStart(State state) {
        if (state != null) {
            DebugStateFlow("#" + frame + ": " + GetType().FullName + ".Start()");
            state.Start();
        }
    }

    private void CallEnd(State state) {
        if (state != null) {
            DebugStateFlow("#" + frame + ": " + GetType().FullName + ".End()");
            state.End();
        }
    }

}