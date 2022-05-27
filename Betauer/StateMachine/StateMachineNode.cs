using System;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1,1);
        public readonly IStateMachine StateMachine;
        public IState State => StateMachine.State;
        public Transition Transition => StateMachine.Transition;
        public ProcessMode Mode { get; set; }
        
        private Func<float, Task>? _beforeExecute;
        private Func<float, Task>? _afterExecute;

        public StateMachineNode(ProcessMode mode = ProcessMode.Idle) {
            StateMachine = new StateMachine();
            Mode = mode;
        }

        public StateMachineNode(string name, ProcessMode mode = ProcessMode.Idle) {
            StateMachine = new StateMachine(name);
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

        public IState FindState(string name) {
            return StateMachine.FindState(name);
        }

        public IStateMachine SetState(string nextState) {
            return StateMachine.SetState(nextState);
        }

        public IStateMachine PushState(string nextState) {
            return StateMachine.PushState(nextState);
        }

        public IStateMachine PopPushState(string nextState) {
            return StateMachine.PopPushState(nextState);
        }

        public IStateMachine PopState() {
            return StateMachine.PopState();
        }

        public async Task Execute(float delta) {
            var canEnter = await _semaphoreSlim.WaitAsync(0);
            if (!canEnter) {
                return;
            }
            try {
                if (_beforeExecute != null) await _beforeExecute.Invoke(delta);
                await StateMachine.Execute(delta);
                if (_afterExecute != null) await _afterExecute.Invoke(delta);
            } finally {
                _semaphoreSlim.Release();
            }
        }

        public override async void _PhysicsProcess(float delta) {
            if (Mode == ProcessMode.Physics) {
                await Execute(delta);
            } else {
                SetPhysicsProcess(false);
            }
        }

        public override async void _Process(float delta) {
            if (Mode == ProcessMode.Idle) {
                await Execute(delta);
            } else {
                SetProcess(false);
            }
        }

        public StateMachineNode BeforeExecute(Action<float> beforeExecute) {
            _beforeExecute = async delta => beforeExecute(delta);
            return this;
        }

        public StateMachineNode BeforeExecute(Func<float, Task> beforeExecute) {
            _beforeExecute = beforeExecute;
            return this;
        }

        public StateMachineNode AfterExecute(Action<float> afterExecute) {
            _afterExecute = async delta => afterExecute(delta);
            return this;
        }
        public StateMachineNode AfterExecute(Func<float, Task> afterExecute) {
            _afterExecute = afterExecute;
            return this;
        }
    }
}