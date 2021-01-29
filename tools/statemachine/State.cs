using Godot;
using System;

public abstract class State {

    public abstract void Execute();

    public virtual void _UnhandledInput(InputEvent @event) {
    }

    public virtual void Start() {
    }

    public virtual void End() {
    }

}
