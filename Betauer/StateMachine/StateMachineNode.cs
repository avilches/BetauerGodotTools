using System;
using Godot;

namespace Betauer.StateMachine {
    public class StateMachineNode : Node, IStateMachine {
        public enum ProcessMode {
            /// <summary>
            /// <para>The state machine is updated in the <c>_PhysicsProcess</c> callback.</para>
            /// </summary>
            Physics,

            /// <summary>
            /// <para>The state machine is updated int the <c>_Process</c> callback.</para>
            /// </summary>
            Idle,
        }

        public readonly IStateMachine StateMachine;
        public ProcessMode Mode { get; set; } = ProcessMode.Idle;

        private Action<float>? _beforeExecute;
        private Action<float>? _afterExecute;

        public StateMachineNode(string name, ProcessMode mode) {
            StateMachine = new StateMachine(this, name);
            Mode = mode;
        }
        public StateMachineNode(IStateMachine stateMachine, ProcessMode mode) {
            StateMachine = stateMachine;
            Mode = mode;
        }

        public StateMachineBuilder<StateMachineNode> CreateBuilder() {
            return new StateMachineBuilder<StateMachineNode>(this);
        }

        public IStateMachine AddState(IState state) {
            return StateMachine.AddState(state);
        }

        public IState GetState(string name) {
            return StateMachine.GetState(name);
        }

        public IStateMachine SetNextState(string nextState) {
            return StateMachine.SetNextState(nextState);
        }

        public void Execute(float delta) {
            _beforeExecute?.Invoke(delta);
            StateMachine.Execute(delta);
            _afterExecute?.Invoke(delta);
        }

        public override void _PhysicsProcess(float delta) {
            if (Mode == ProcessMode.Physics) Execute(delta);
        }

        public override void _Process(float delta) {
            if (Mode == ProcessMode.Idle) Execute(delta);
        }

        public StateMachineNode BeforeExecute(Action<float> beforeExecute) {
            _beforeExecute = beforeExecute;
            return this;
        }
        public StateMachineNode AfterExecute(Action<float> afterExecute) {
            _afterExecute = afterExecute;
            return this;
        }
    }
}