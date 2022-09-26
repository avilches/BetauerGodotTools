using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace Betauer.StateMachine {
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

    public abstract class StateMachineNode : Node {
        protected static Logger Logger = LoggerFactory.GetLogger<StateMachineNode>();
    }

    public class StateMachineNode<TStateKey, TTransitionKey> : StateMachineNode, IStateMachine<TStateKey, TTransitionKey> 
        where TStateKey : Enum where TTransitionKey : Enum {
        public readonly IStateMachine<TStateKey, TTransitionKey> StateMachine;
        public IState<TStateKey, TTransitionKey> CurrentState => StateMachine.CurrentState;
        public bool Available => StateMachine.Available;
        public string? Name => StateMachine.Name; 
        public ProcessMode Mode { get; set; }
        public bool IsState(TStateKey state) => StateMachine.IsState(state);

        private Dictionary<TStateKey, Action<InputEvent>>? _input;
        private Dictionary<TStateKey, Action<InputEvent>>? _unhandledInput;

        public StateMachineNode(TStateKey initialState, string? name = null, ProcessMode mode = ProcessMode.Idle) :
            this(new StateMachine<TStateKey, TTransitionKey>(initialState, name), mode) {
        }

        public StateMachineNode(IStateMachine<TStateKey, TTransitionKey> stateMachine, ProcessMode mode = ProcessMode.Idle) {
            StateMachine = stateMachine;
            Mode = mode;
        }
        
        public void AddListener(IStateMachineListener<TStateKey> machineListener) {
            StateMachine.AddListener(machineListener);
        }

        public void AddState(IState<TStateKey, TTransitionKey> state) {
            StateMachine.AddState(state);
        }

        public StateBuilder<IStateMachine<TStateKey, TTransitionKey>, TStateKey, TTransitionKey> CreateState(TStateKey stateKey) {
            return StateMachine.CreateState(stateKey);
        }

        public void On(TTransitionKey transitionKey, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>> transition) {
            StateMachine.On(transitionKey, transition);
        }

        public void On(TStateKey stateKey, TTransitionKey transitionKey, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>> transition) {
            StateMachine.On(stateKey, transitionKey, transition);
        }

        public void Enqueue(TTransitionKey name) {
            StateMachine.Enqueue(name);
        }

        public Task Execute(float delta) {
            return StateMachine.Execute(delta);
        }


        public void OnInput(TStateKey stateKey, Action<InputEvent> input) {
            _input ??= new Dictionary<TStateKey, Action<InputEvent>>();
            _input.Add(stateKey, input);
        }

        public void OnUnhandledInput(TStateKey stateKey, Action<InputEvent> unhandledInput) {
            _unhandledInput ??= new Dictionary<TStateKey, Action<InputEvent>>();
            _unhandledInput.Add(stateKey, unhandledInput);
        }

        public override void _Input(InputEvent e) {
            if (Available && _input != null && 
                _input.TryGetValue(CurrentState.Key, out var input)) {
                input(e);
            }
        }

        public override void _UnhandledInput(InputEvent e) {
            if (Available && _unhandledInput != null && 
                _unhandledInput.TryGetValue(CurrentState.Key, out var unhandledInput)) {
                unhandledInput(e);
            }
        }

        public override void _PhysicsProcess(float delta) {
            if (Mode == ProcessMode.Physics) Process(delta);
            else SetPhysicsProcess(false);
        }

        public override void _Process(float delta) {
            if (Mode == ProcessMode.Idle) Process(delta);
            else SetProcess(false);
        }

        private void Process(float delta) {
            Execute(delta)
                .OnException((ex) => Logger.Error($"Name: {Name} | State: {CurrentState.Key}\n{ex}"));
        }
    }
}