using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Betauer.StateMachine.Async;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;
using NullReferenceException = System.NullReferenceException;

namespace Betauer.StateMachine.Tests {
    [TestFixture]
    public class StateMachineAsyncTests : Node {
        enum State {
            A,
            Idle,
            Settings,
            MainMenu,
            Audio,
            Local,
            Global,
            Debug,
            Start,
            Debug1,
            Debug2,
            Video,
            Jump,
            Attack,
            End,
            NotFound
        }
        enum Trans {
            Settings,
            MainMenu,
            Audio,
            Local,
            Global,
            End,
            Back,
            Debug,
            Start,
            Restart,
            Video,
            NotFound
        }
        [Test(Description = "Constructor")]
        public void StateMachineConstructorsEnum() {
            var sm1 = new StateMachineAsync<State, Trans>(State.A, "X");
            Assert.That(sm1.Name, Is.EqualTo("X"));

            var sm3 = new StateMachineAsync<State, Trans>(State.A);
            Assert.That(sm3.Name, Is.Null);
        }
            
        /*
         * Error cases
         */
        [Test(Description = "InitialState not found on start")]
        public void WrongStartStates() {
            var sm = new StateMachineAsync<State, Trans>(State.Global);
            sm.State(State.A).Build();

            // Start state Global not found
            Assert.ThrowsAsync<KeyNotFoundException>(async () => {
                await sm.Execute();
            });
        }
        
        [Test(Description = "IsState")]
        public void IsStateTests() {
            var sm = new StateMachineAsync<State, Trans>(State.Global);
            sm.State(State.Global).Build();
            
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Global));
            Assert.That(sm.IsState(State.Global), Is.True);
            Assert.That(sm.IsState(State.Settings), Is.False);
        }
        
        [Test(Description = "A wrong InitialState can be avoided triggering a transition")]
        public async Task WrongStartWithTriggering() {
            var sm = new StateMachineAsync<State, Trans>(State.Global);
            sm.On(Trans.Audio).Then(context => context.Set(State.Audio));
            sm.State(State.Audio).Build();

            sm.Send(Trans.Audio);
            await sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Audio));
            Assert.That(sm.IsState(State.Audio), Is.True);
            Assert.That(sm.IsState(State.Global), Is.False);
        }
        
        [Test(Description = "Error when a state changes to a not found state: Replace")]
        public void WrongStatesUnknownStateSet() {
            var sm = new StateMachineAsync<State, Trans>(State.A);
            sm.State(State.A).If(() => true).Then(context => context.Set(State.Debug)).Build();

            Assert.ThrowsAsync<KeyNotFoundException>(async () => await sm.Execute());
        }

        [Test(Description = "Error when a state changes to a not found state: PopPush")]
        public void WrongStatesUnknownStatePushPop() {
            var sm = new StateMachineAsync<State, Trans>(State.A);
            sm.State(State.A).If(() => true).Then(context => context.PopPush(State.NotFound)).Build();

            Assert.ThrowsAsync<KeyNotFoundException>(async () => await sm.Execute());
        }

        [Test(Description = "Error when a state changes to a not found state: Push")]
        public void WrongStatesUnknownStatePushPush() {
            var sm = new StateMachineAsync<State, Trans>(State.A);
            sm.State(State.A).If(() => true).Then(context => context.Push(State.NotFound)).Build();

            Assert.ThrowsAsync<KeyNotFoundException>(async () => await sm.Execute());
        }

        [Test(Description = "Error when a state triggers a not found transition")]
        public void WrongStatesTriggerUnknownTransition() {
            var sm = new StateMachineAsync<State, Trans>(State.A);
            sm.State(State.A).If(() => true).Then(context => context.Trigger(Trans.NotFound)).Build();

            Assert.ThrowsAsync<KeyNotFoundException>(async () => await sm.Execute());
        }

        [Test(Description = "Error not found transition")]
        public void WrongUnknownTransition() {
            var sm = new StateMachineAsync<State, Trans>(State.A);
            sm.State(State.A).Build();

            sm.Send(Trans.NotFound);
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await sm.Execute());
        }

        [Test(Description = "Error when a state pop in an empty stack")]
        public void WrongStatesPopWhenEmptyStack() {
            var sm = new StateMachineAsync<State, Trans>(State.A);
            sm.State(State.A).If(() => true).Then(context => context.Pop()).Build();

            // State ends with a wrong state
            Assert.ThrowsAsync<InvalidOperationException>(async () => await sm.Execute());
        }
        
        [Test(Description = "Pop the same state in the stack is allowed")]
        public async Task PopSameStateInTheStackIsAllowed() {
            var sm = new StateMachineAsync<State, Trans>(State.A);
            sm.State(State.A).If(() => true).Then(context => context.Push(State.A)).Build();

            await sm.Execute();
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A }));

            await sm.Execute();
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A, State.A }));

            await sm.Execute();
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A, State.A , State.A }));
        }

        [Test(Description = "Multiple if are loaded in order")]
        public async Task Ifs() {
            var sm = new StateMachineAsync<State, Trans>(State.A);
            var x = 0;
            sm.State(State.A)
                .If(() => {
                    x++;
                    return false;
                }).Set(State.Attack)
                .If(() => {
                    x++;
                    return false;
                }).Set(State.MainMenu)
                .If(() => {
                    x++;
                    return true;
                }).Set(State.Audio)
                .If(() => {
                    x++;
                    return false;
                }).Push(State.Debug).Build();
            sm.State(State.Attack).Build();
            sm.State(State.MainMenu).Build();
            sm.State(State.Audio).Build();
            await sm.Execute();
            await sm.Execute();
            Assert.That(x, Is.EqualTo(3));
        }

        /**
         * If with methods, instead lambda Then
         */
        [Test(Description = "If with Set result")]
        public async Task IfSetResult() {
            var sm = new StateMachineAsync<State, Trans>(State.A);
            sm.State(State.A).If(() => true).Set(State.Debug).Build();
            sm.State(State.Debug).Build();
            await sm.Execute();
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A }));
            await sm.Execute();
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.Debug }));
        }

        [Test(Description = "If with Push, PopPush and Pop result")]
        public async Task IfPushPopPushResult() {
            var sm = new StateMachineAsync<State, Trans>(State.A);
            sm.State(State.A).If(() => true).Push(State.Debug).Build();
            sm.State(State.Debug).If(() => true).PopPush(State.MainMenu).Build();
            sm.State(State.MainMenu).If(() => true).Pop().Build();
            await sm.Execute();
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A }));
            await sm.Execute();
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A, State.Debug }));
            await sm.Execute();
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A, State.MainMenu }));
            await sm.Execute();
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A }));
        }

        [Test(Description = "If with none result")]
        public async Task IfNoneResult() {
            var sm = new StateMachineAsync<State, Trans>(State.A);
            sm.State(State.A).If(() => true).None().Build();
            await sm.Execute();
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A }));
            await sm.Execute();
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A }));
        }

        /**
         * Transition with methods, instead of lambda Then
         */
        [Test(Description = "Transition with Set result")]
        public async Task TransitionSetResult() {
            var sm = new StateMachineAsync<State, Trans>(State.A);
            sm.On(Trans.Debug).Set(State.Debug);
            sm.State(State.A).Build();
            sm.State(State.Debug).Build();
            await sm.Execute();
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A }));
            sm.Send(Trans.Debug);
            await sm.Execute();
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.Debug }));
        }

        [Test(Description = "Transition with Push, PopPush and Pop result")]
        public async Task TransitionPushPopPushResult() {
            var sm = new StateMachineAsync<State, Trans>(State.A);
            sm.On(Trans.Start).Push(State.Debug);
            sm.On(Trans.MainMenu).PopPush(State.MainMenu);
            sm.On(Trans.End).Pop();
            sm.State(State.A).Build();
            sm.State(State.Debug).Build();
            sm.State(State.MainMenu).Build();
            
            await sm.Execute();
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A }));

            sm.Send(Trans.Start);
            await sm.Execute();
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A, State.Debug }));
            
            sm.Send(Trans.MainMenu);
            await sm.Execute();
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A, State.MainMenu }));

            sm.Send(Trans.End);
            await sm.Execute();
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A }));
        }

        [Test(Description = "Transition with none result")]
        // State transitions have more priority than global transitions. So, override a global with a local None will disable it
        public async Task TransitionNoneResult() {
            var sm = new StateMachineAsync<State, Trans>(State.Start);

            sm.On(Trans.Global).Set(State.Global);
            sm.State(State.Start)
                .On(Trans.Global).None().Build();
            
            sm.Send(Trans.Global);
            await sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Start));
        }
        
        /*
         * Working StateMachine
         */
        [Test(Description = "Regular changes between root states inside state.Execute()")]
        public async Task StateMachinePlainFlow() {
            var sm = new StateMachineAsync<State, Trans>(State.Idle);

            var x = 0;
            List<string> states = new List<string>();

            sm.State(State.Idle)
                .Enter(async () => {
                    x = 0;
                    states.Add("IdleEnter");
                })
                .Execute(async () => {
                    x++;
                    states.Add("IdleExecute(" + x + ")");
                })
                .If(() => x == 2).Then(context => context.Set(State.Jump))
                .If(() => true).Then(context => context.None())
                .Build();

            sm.State(State.Jump)
                .Enter(async () => {
                    states.Add("JumpEnter");
                })
                .Execute(async () => states.Add("JumpExecute(" + x + ")"))
                .If(() => true).Then(context => context.Set(State.Attack))
                .Build();
                
            // No exit because it's optional
                
            sm.State(State.Attack)
                // No enter because it's optional
                .Execute(async () => states.Add("AttackExecute"))
                .If(() => true).Then(context => context.Set(State.Idle))
                .Exit(async () => { states.Add("AttackExit"); })
                .Build();
                
            states.Clear();
            await sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Idle)); 
            
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("IdleEnter,IdleExecute(1)"));

            states.Clear();
            await sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Idle)); 
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("IdleExecute(2)"));
            states.Clear();
            
            states.Clear();
            await sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Jump)); 
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("JumpEnter,JumpExecute(2)"));

            states.Clear();
            await sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Attack)); 
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("AttackExecute"));

            states.Clear();
            await sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Idle)); 
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("AttackExit,IdleEnter,IdleExecute(1)"));

        }

        [Test(Description = "Ignore events with lower weight")]
        public async Task SendEventWeight() {
            var sm = new StateMachineAsync<State, Trans>(State.Start);

            sm.On(Trans.Global).Then(context => context.Set(State.Global));
            sm.On(Trans.Local).Then(context => context.Set(State.Local));
            sm.On(Trans.Start).Then(context => context.Set(State.Start));
            sm.State(State.Start).Build();
            sm.State(State.Global).Build();
            sm.State(State.Local).Build();

            await sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Start));
            
            sm.Send(Trans.Start, 10);
            sm.Send(Trans.Local, 9);
            sm.Send(Trans.Global);
            await sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Start));
            
            sm.Send(Trans.Local);
            sm.Send(Trans.Global);
            await sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Global));
        }

        [Test(Description = "State transitions have more priority than global transition")]
        public async Task StateTransitionTrigger() {
            var sm = new StateMachineAsync<State, Trans>(State.Start);

            sm.State(State.Start).On(Trans.Local).Then(context => context.Push(State.Local)).Build();
            sm.On(Trans.Global).Then(context => context.Set(State.Global));
            sm.On(Trans.Local).Then(context => context.Set(State.Global));
            sm.State(State.Global).Build();
            sm.State(State.Local).Build();

            await sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Start));
            
            sm.Send(Trans.Local);
            await sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Local));
            
            sm.Send(Trans.Global);
            await sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Global));
            
            sm.Send(Trans.Local);
            await sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Global));

        }

        [Test(Description = "Transition calls inside execute has higher priority than the result returned")]
        public async Task TransitionTriggerInsideExecute() {
            var sm = new StateMachineAsync<State, Trans>(State.Start);

            sm.State(State.Start).Execute(async () => {
                    sm.Send(Trans.NotFound); // IGNORED!!
                    sm.Send(Trans.Settings);
                })
                .If(() => true).Then(context => context.Push(State.Audio)) // IGNORED!!
                .Build();
            sm.State(State.Audio).Build();
            sm.State(State.Settings).Build();
                
            sm.On(Trans.Settings).Then(context => context.Set(State.Settings));

            await sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Start));
            // The second execution has scheduled the 
            await sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Settings));
        }

        [Test(Description = "Changes using stateMachine change methods")]
        public async Task TransitionTrigger() {
            var sm = new StateMachineAsync<State, Trans>(State.Audio);

            sm.State(State.Debug).Build();
            sm.State(State.MainMenu).On(Trans.Audio).Then(context => context.Push(State.Audio)).Build();
            sm.State(State.Settings).On(Trans.Back).Then(context => context.Set(State.MainMenu)).Build();
            sm.State(State.Audio).On(Trans.Back).Then(context => context.Pop()).Build();
            sm.On(Trans.Restart).Then(context => context.Set(State.MainMenu));
            sm.On(Trans.Settings).Then(context => context.Set(State.Settings));
            sm.On(Trans.MainMenu).Then(context => context.Set(State.MainMenu));

            // Global event
            await sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Audio));
            sm.Send(Trans.Restart);
            await sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.MainMenu));
            
            // State event
            sm.Send(Trans.Settings);
            await sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Settings));
            sm.Send(Trans.Back);
            await sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.MainMenu));

            // State event: pop
            sm.Send(Trans.MainMenu);
            await sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.MainMenu));
            sm.Send(Trans.Audio);
            await sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Audio));
            Assert.That(sm.GetStack(), Is.EqualTo(new [] {State.MainMenu, State.Audio}));
            sm.Send(Trans.Back);
            await sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.MainMenu));
        }


        [Test]
        public async Task EnterOnPushExitOnPopSuspendAwakeListener() {
            var sm = new StateMachineAsync<State, Trans>(State.Debug);

            sm.On(Trans.Settings).Then(context => context.Push(State.Settings));
            sm.On(Trans.Back).Then(context => context.Pop());
            sm.State(State.MainMenu).Build();
            sm.State(State.Debug).Build();
            sm.State(State.Settings).Build();
            List<string> states = new List<string>();
            sm.OnEnter += (args) => states.Add(args.To + ":enter");
            sm.OnAwake += (args)  => states.Add(args.To + ":awake");
            sm.OnSuspend += (args)  => states.Add(args.From + ":suspend");
            sm.OnExit += (args)  => states.Add(args.From + ":exit");
            sm.OnTransition += (args)  => states.Add("from:" + args.From + "-to:" + args.To);

            
            
            await sm.Execute();
            sm.Send(Trans.Settings);
            await sm.Execute();
            sm.Send(Trans.Back);
            await sm.Execute();
            // Test listener
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo(
                "Debug:enter," +
                
                    "Debug:suspend," +
                    "from:Debug-to:Settings," +
                    "Settings:enter," +
                
                    "Settings:exit," +
                    "from:Settings-to:Debug," +
                    "Debug:awake"));
            

        }

        [Test]
        public async Task EnterOnPushExitOnPopSuspendAwakeEventsOrder() {
            var sm = new StateMachineAsync<State, Trans>(State.Debug);
            
            List<string> states = new List<string>();

            sm.On(Trans.Debug).Then(context => context.Set(State.Debug));
            sm.State(State.Debug)
                .Awake(async () => states.Add("Debug:awake"))
                .Enter(async () => states.Add("Debug:enter"))
                .Execute(async () => states.Add("Debug"))
                .If(() => true).Then(context => context.None())
                .Suspend(async () => states.Add("Debug:suspend"))
                .Exit(async () => states.Add("Debug:exit"))
                .Build();

            sm.On(Trans.MainMenu).Then(context => context.Set(State.MainMenu));
            sm.State(State.MainMenu)
                .Awake(async () => states.Add("MainMenu:awake"))
                .Enter(async () => states.Add("MainMenu:enter"))
                .Execute(async () => states.Add("MainMenu"))
                .If(() => true).Then(context => context.None())
                .Suspend(async () => states.Add("MainMenu:suspend"))
                .Exit(async ()=>{
                    states.Add("MainMenu:exit");
                })
                .Build();
                
            
            sm.On(Trans.Settings).Then(context => context.Push(State.Settings));
            sm.State(State.Settings)
                .On(Trans.Audio).Then(context => context.Push(State.Audio))
                .On(Trans.Back).Then(context => context.Pop())
                .Awake(async () => states.Add("Settings:awake"))
                .Enter(async () => states.Add("Settings:enter"))
                .Execute(async () => states.Add("Settings"))
                .If(() => true).Then(context => context.None())
                .Suspend(async () => states.Add("Settings:suspend"))
                .Exit(async () =>{
                    states.Add("Settings:exit");
                })
                .Build();
                
            
            sm.State(State.Audio)
                .On(Trans.Video).Then(context => context.PopPush(State.Video))
                .On(Trans.Back).Then(context => context.Pop())
                .Awake(async () => states.Add("Audio:awake"))
                .Enter(async () => states.Add("Audio:enter"))
                .Execute(async () => states.Add("Audio"))
                .If(() => true).Then(context => context.None())
                .Suspend(async () => states.Add("Audio:suspend"))
                .Exit(async ()=> {
                    states.Add("Audio:exit");
                })
                .Build();
                

            sm.State(State.Video)
                .On(Trans.Back).Then(context => context.Pop())
                .Awake(async () => states.Add("Video:awake"))
                .Enter(async () => states.Add("Video:enter"))
                .Execute(async () => states.Add("Video"))
                .If(() => true).Then(context => context.None())
                .Suspend(async () => states.Add("Video:suspend"))
                .Exit(async () => states.Add("Video:exit"))
                .Build();
            
            await sm.Execute();
            Console.WriteLine(string.Join(",", states));
            sm.Send(Trans.MainMenu);
            await sm.Execute();
            Console.WriteLine(string.Join(",", states));
            sm.Send(Trans.Settings);
            await sm.Execute();
            Console.WriteLine(string.Join(",", states));
            sm.Send(Trans.Audio);
            await sm.Execute();
            Console.WriteLine(string.Join(",", states));
            sm.Send(Trans.Video);
            await sm.Execute();
            Console.WriteLine(string.Join(",", states));
            sm.Send(Trans.Back);
            await sm.Execute();
            Console.WriteLine(string.Join(",", states));
            sm.Send(Trans.Back);
            await sm.Execute();
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo(
                "Debug:enter,Debug,Debug:exit," +
                "MainMenu:enter,MainMenu,MainMenu:suspend," +
                    "Settings:enter,Settings,Settings:suspend," +
                        "Audio:enter,Audio,Audio:exit," +
                        "Video:enter,Video,Video:exit," +
                    "Settings:awake,Settings,Settings:exit," +
                "MainMenu:awake,MainMenu"));
            
            // Test multiple exits when more than one state is in the stack and change is Replace instead of Pop
            states.Clear();
            sm.Send(Trans.Settings);
            await sm.Execute();
            Console.WriteLine(string.Join(",", states));
            sm.Send(Trans.Audio);
            await sm.Execute();
            Console.WriteLine(string.Join(",", states));
            sm.Send(Trans.Debug);
            await sm.Execute();
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo(
                "MainMenu:suspend," +
                    "Settings:enter,Settings,Settings:suspend," +
                        "Audio:enter,Audio," +
                        "Audio:exit,Settings:exit,MainMenu:exit," + // This is the important part: three end in a row 
                "Debug:enter,Debug"));


        }

        [Test]
        public async Task ErrorChangingState() {
            var sm = new StateMachineAsync<State, Trans>(State.Start);

            List<string> states = new List<string>();
            var throws = 0;
            sm.On(Trans.Debug).Then(context => context.Set(State.Debug));
            sm.State(State.Debug)
                .Enter(async () => {
                    throws++;                    
                    throw new NullReferenceException();
                })
                .Execute(async () => states.Add("Debug:Execute"))
                .If(() => true).Then(ctx => ctx.Set(State.End))
                .Build();

            sm.On(Trans.MainMenu).Then(context => context.Set(State.MainMenu));
            sm.State(State.MainMenu)
                .Enter(async () => states.Add("MainMenu:Enter"))
                .Execute(async () => {
                    throws++;                    
                    throw new NullReferenceException();
                })
                .Build();

            sm.On(Trans.Global).Then(context => context.Set(State.Global));
            sm.State(State.Global)
                .Enter(async () => states.Add("Global:Enter"))
                .Execute(async () => states.Add("Global:Execute"))
                .If(() => true).Then(ctx => ctx.Set(State.End))
                .Exit(async () => {
                    throws++;                    
                    throw new NullReferenceException();
                })
                .Build();

            sm.On(Trans.End).Then(context => context.Set(State.End));
            sm.State(State.Start).Build();
            sm.State(State.End).Build();
            
            // 1-Error when state machine is not initialized
            sm.Send(Trans.Debug);
            Assert.ThrowsAsync<NullReferenceException>(async () => await sm.Execute());
            // It returns to non-initialized state (state = null)
            Assert.That(throws, Is.EqualTo(1));
            throws = 0;

            // Run again, the SM enters in the initial state "Start"
            await sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Start));
            Assert.That(throws, Is.EqualTo(0));

            // 2-Error when state machine has state. Error in Enter
            sm.Send(Trans.Debug);
            Assert.ThrowsAsync<NullReferenceException>(async () => await sm.Execute());
            // It returns to Start state
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Start));
            Assert.That(throws, Is.EqualTo(1));
            throws = 0;

            // 3-Error when state machine has state. Error in Execute
            sm.Send(Trans.MainMenu);
            Assert.ThrowsAsync<NullReferenceException>(async () => await sm.Execute());
            // It returns to Start state
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Start));
            Assert.That(throws, Is.EqualTo(1));
            throws = 0;

            // 4-Error when state machine has state. Error in Exit
            sm.Send(Trans.Global);
            await sm.Execute();
            sm.Send(Trans.End);
            Assert.ThrowsAsync<NullReferenceException>(async () => await sm.Execute());
            // It returns to Start state
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Global));
            Assert.That(throws, Is.EqualTo(1));
            throws = 0;

            Assert.That(string.Join(",", states), Is.EqualTo(
                "MainMenu:Enter,Global:Enter,Global:Execute"));

        }

    }
}