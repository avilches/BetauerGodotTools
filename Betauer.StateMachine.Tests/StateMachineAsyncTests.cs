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
            sm.On(Trans.Audio, context => context.Set(State.Audio));
            sm.State(State.Audio).Build();

            sm.Enqueue(Trans.Audio);
            await sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Audio));
            Assert.That(sm.IsState(State.Audio), Is.True);
            Assert.That(sm.IsState(State.Global), Is.False);
        }
        
        [Test(Description = "Error when a state changes to a not found state: Replace")]
        public void WrongStatesUnknownStateSet() {
            var sm = new StateMachineAsync<State, Trans>(State.A);
            sm.State(State.A).Condition(() => true, context => context.Set(State.Debug)).Build();

            Assert.ThrowsAsync<KeyNotFoundException>(async () => await sm.Execute(0f));
        }

        [Test(Description = "Error when a state changes to a not found state: PopPush")]
        public void WrongStatesUnknownStatePushPop() {
            var sm = new StateMachineAsync<State, Trans>(State.A);
            sm.State(State.A).Condition(() => true, context => context.PopPush(State.NotFound)).Build();

            Assert.ThrowsAsync<KeyNotFoundException>(async () => await sm.Execute(0f));
        }

        [Test(Description = "Error when a state changes to a not found state: Push")]
        public void WrongStatesUnknownStatePushPush() {
            var sm = new StateMachineAsync<State, Trans>(State.A);
            sm.State(State.A).Condition(() => true, context => context.Push(State.NotFound)).Build();

            Assert.ThrowsAsync<KeyNotFoundException>(async () => await sm.Execute(0f));
        }

        [Test(Description = "Error when a state triggers a not found transition")]
        public void WrongStatesTriggerUnknownTransition() {
            var sm = new StateMachineAsync<State, Trans>(State.A);
            sm.State(State.A).Condition(() => true, context => context.Trigger(Trans.NotFound)).Build();

            Assert.ThrowsAsync<KeyNotFoundException>(async () => await sm.Execute(0f));
        }

        [Test(Description = "Error not found transition")]
        public void WrongUnknownTransition() {
            var sm = new StateMachineAsync<State, Trans>(State.A);
            sm.State(State.A).Build();

            sm.Enqueue(Trans.NotFound);
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await sm.Execute(0f));
        }

        [Test(Description = "Error when a state pop in an empty stack")]
        public void WrongStatesPopWhenEmptyStack() {
            var sm = new StateMachineAsync<State, Trans>(State.A);
            sm.State(State.A).Condition(() => true, context => context.Pop()).Build();

            // State ends with a wrong state
            Assert.ThrowsAsync<InvalidOperationException>(async () => await sm.Execute(0f));
        }
        
        [Test(Description = "Pop the same state in the stack is allowed")]
        public async Task PopSameStateInTheStackIsAllowed() {
            var sm = new StateMachineAsync<State, Trans>(State.A);
            sm.State(State.A).Condition(() => true, context => context.Push(State.A)).Build();

            await sm.Execute(0f);
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A }));

            await sm.Execute(0f);
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A, State.A }));

            await sm.Execute(0f);
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A, State.A , State.A }));
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
                .Condition(() => x == 2, context => context.Set(State.Jump))
                .Condition(() => true, context => context.None())
                .Build();

            sm.State(State.Jump)
                .Enter(async () => {
                    states.Add("JumpEnter");
                })
                .Execute(async () => states.Add("JumpExecute(" + x + ")"))
                .Condition(() => true, context => context.Set(State.Attack))
                .Build();
                
            // No exit because it's optional
                
            sm.State(State.Attack)
                // No enter because it's optional
                .Execute(async () => states.Add("AttackExecute"))
                .Condition(() => true, context => context.Set(State.Idle))
                .Exit(async () => { states.Add("AttackExit"); })
                .Build();
                
            states.Clear();
            await sm.Execute(100f);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Idle)); 
            
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("IdleEnter,IdleExecute(1)"));

            states.Clear();
            await sm.Execute(100f);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Idle)); 
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("IdleExecute(2)"));
            states.Clear();
            
            states.Clear();
            await sm.Execute(100f);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Jump)); 
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("JumpEnter,JumpExecute(2)"));

            states.Clear();
            await sm.Execute(100f);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Attack)); 
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("AttackExecute"));

            states.Clear();
            await sm.Execute(100f);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Idle)); 
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("AttackExit,IdleEnter,IdleExecute(1)"));

        }


        [Test(Description = "Check delta works")]
        public async Task DeltaInExecute() {
            var sm = new StateMachineAsync<State, Trans>(State.Debug1);
            const float expected1 = 10f;
            const float expected2 = 10f;
            sm.State(State.Debug1).Execute(async () => {
                    // Assert.That(context.Delta, Is.EqualTo(expected1));
                })
                .Condition(() => true, context => context.Set(State.Debug2))
                .Build();
 
            sm.State(State.Debug2)
                // .Execute(() => 
                // Assert.That(context.Delta, Is.EqualTo(expected2));
                .Condition(() => true, context => context.None())
                .Build();
                
            await sm.Execute(expected1);
            await sm.Execute(expected2);
        }

        [Test(Description = "State transitions have more priority than global transition")]
        public async Task StateTransitionTrigger() {
            var sm = new StateMachineAsync<State, Trans>(State.Start);

            sm.State(State.Start).On(Trans.Local, context => context.Push(State.Local)).Build();
            sm.On(Trans.Global, context => context.Set(State.Global));
            sm.On(Trans.Local, context => context.Set(State.Global));
            sm.State(State.Global).Build();
            sm.State(State.Local).Build();

            await sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Start));
            
            sm.Enqueue(Trans.Local);
            await sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Local));
            
            sm.Enqueue(Trans.Global);
            await sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Global));
            
            sm.Enqueue(Trans.Local);
            await sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Global));

        }

        [Test(Description = "Transition calls inside execute has higher priority than the result returned")]
        public async Task TransitionTriggerInsideExecute() {
            var sm = new StateMachineAsync<State, Trans>(State.Start);

            sm.State(State.Start).Execute(async () => {
                    sm.Enqueue(Trans.NotFound); // IGNORED!!
                    sm.Enqueue(Trans.Settings);
                })
                .Condition(() => true, context => context.Push(State.Audio)) // IGNORED!!
                .Build();
            sm.State(State.Audio).Build();
            sm.State(State.Settings).Build();
                
            sm.On(Trans.Settings, context => context.Set(State.Settings));

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
            sm.State(State.MainMenu).On(Trans.Audio, context => context.Push(State.Audio)).Build();
            sm.State(State.Settings).On(Trans.Back, context => context.Set(State.MainMenu)).Build();
            sm.State(State.Audio).On(Trans.Back, context => context.Pop()).Build();
            sm.On(Trans.Restart, context => context.Set(State.MainMenu));
            sm.On(Trans.Settings, context => context.Set(State.Settings));
            sm.On(Trans.MainMenu, context => context.Set(State.MainMenu));

            // Global event
            await sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Audio));
            sm.Enqueue(Trans.Restart);
            await sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.MainMenu));
            
            // State event
            sm.Enqueue(Trans.Settings);
            await sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Settings));
            sm.Enqueue(Trans.Back);
            await sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.MainMenu));

            // State event: pop
            sm.Enqueue(Trans.MainMenu);
            await sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.MainMenu));
            sm.Enqueue(Trans.Audio);
            await sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Audio));
            Assert.That(sm.GetStack(), Is.EqualTo(new [] {State.MainMenu, State.Audio}));
            sm.Enqueue(Trans.Back);
            await sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.MainMenu));
        }


        [Test]
        public async Task EnterOnPushExitOnPopSuspendAwakeListener() {
            var sm = new StateMachineAsync<State, Trans>(State.Debug);

            sm.On(Trans.Settings, context => context.Push(State.Settings));
            sm.On(Trans.Back, context => context.Pop());
            sm.State(State.MainMenu).Build();
            sm.State(State.Debug).Build();
            sm.State(State.Settings).Build();
            List<string> states = new List<string>();
            sm.AddOnEnter((args) => states.Add(args.To + ":enter"));
            sm.AddOnAwake((args)  => states.Add(args.To + ":awake"));
            sm.AddOnSuspend((args)  => states.Add(args.From + ":suspend"));
            sm.AddOnExit((args)  => states.Add(args.From + ":exit"));
            sm.AddOnTransition((args)  => states.Add("from:" + args.From + "-to:" + args.To));

            
            
            await sm.Execute(0f);
            sm.Enqueue(Trans.Settings);
            await sm.Execute(0f);
            sm.Enqueue(Trans.Back);
            await sm.Execute(0f);
            // Test listener
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo(
                "from:Debug-to:Debug," +
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

            sm.On(Trans.Debug, context => context.Set(State.Debug));
            sm.State(State.Debug)
                .Awake(async () => states.Add("Debug:awake"))
                .Enter(async () => states.Add("Debug:enter"))
                .Execute(async () => states.Add("Debug"))
                .Condition(() => true, context => context.None())
                .Suspend(async () => states.Add("Debug:suspend"))
                .Exit(async () => states.Add("Debug:exit"))
                .Build();

            sm.On(Trans.MainMenu, context => context.Set(State.MainMenu));
            sm.State(State.MainMenu)
                .Awake(async () => states.Add("MainMenu:awake"))
                .Enter(async () => states.Add("MainMenu:enter"))
                .Execute(async () => states.Add("MainMenu"))
                .Condition(() => true, context => context.None())
                .Suspend(async () => states.Add("MainMenu:suspend"))
                .Exit(async ()=>{
                    states.Add("MainMenu:exit");
                })
                .Build();
                
            
            sm.On(Trans.Settings, context => context.Push(State.Settings));
            sm.State(State.Settings)
                .On(Trans.Audio, context => context.Push(State.Audio))
                .On(Trans.Back, context => context.Pop())
                .Awake(async () => states.Add("Settings:awake"))
                .Enter(async () => states.Add("Settings:enter"))
                .Execute(async () => states.Add("Settings"))
                .Condition(() => true, context => context.None())
                .Suspend(async () => states.Add("Settings:suspend"))
                .Exit(async () =>{
                    states.Add("Settings:exit");
                })
                .Build();
                
            
            sm.State(State.Audio)
                .On(Trans.Video, context => context.PopPush(State.Video))
                .On(Trans.Back, context => context.Pop())
                .Awake(async () => states.Add("Audio:awake"))
                .Enter(async () => states.Add("Audio:enter"))
                .Execute(async () => states.Add("Audio"))
                .Condition(() => true, context => context.None())
                .Suspend(async () => states.Add("Audio:suspend"))
                .Exit(async ()=> {
                    states.Add("Audio:exit");
                })
                .Build();
                

            sm.State(State.Video)
                .On(Trans.Back, context => context.Pop())
                .Awake(async () => states.Add("Video:awake"))
                .Enter(async () => states.Add("Video:enter"))
                .Execute(async () => states.Add("Video"))
                .Condition(() => true, context => context.None())
                .Suspend(async () => states.Add("Video:suspend"))
                .Exit(async () => states.Add("Video:exit"))
                .Build();
            
            await sm.Execute(0f);
            Console.WriteLine(string.Join(",", states));
            sm.Enqueue(Trans.MainMenu);
            await sm.Execute(0f);
            Console.WriteLine(string.Join(",", states));
            sm.Enqueue(Trans.Settings);
            await sm.Execute(0f);
            Console.WriteLine(string.Join(",", states));
            sm.Enqueue(Trans.Audio);
            await sm.Execute(0f);
            Console.WriteLine(string.Join(",", states));
            sm.Enqueue(Trans.Video);
            await sm.Execute(0f);
            Console.WriteLine(string.Join(",", states));
            sm.Enqueue(Trans.Back);
            await sm.Execute(0f);
            Console.WriteLine(string.Join(",", states));
            sm.Enqueue(Trans.Back);
            await sm.Execute(0f);
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
            sm.Enqueue(Trans.Settings);
            await sm.Execute(0f);
            Console.WriteLine(string.Join(",", states));
            sm.Enqueue(Trans.Audio);
            await sm.Execute(0f);
            Console.WriteLine(string.Join(",", states));
            sm.Enqueue(Trans.Debug);
            await sm.Execute(0f);
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
            sm.On(Trans.Debug, context => context.Set(State.Debug));
            sm.State(State.Debug)
                .Enter(async () => {
                    throws++;                    
                    throw new NullReferenceException();
                })
                .Execute(async () => states.Add("Debug:Execute"))
                .Condition(() => true, ctx => ctx.Set(State.End))
                .Build();

            sm.On(Trans.MainMenu, context => context.Set(State.MainMenu));
            sm.State(State.MainMenu)
                .Enter(async () => states.Add("MainMenu:Enter"))
                .Execute(async () => {
                    throws++;                    
                    throw new NullReferenceException();
                })
                .Build();

            sm.On(Trans.Global, context => context.Set(State.Global));
            sm.State(State.Global)
                .Enter(async () => states.Add("Global:Enter"))
                .Execute(async () => states.Add("Global:Execute"))
                .Condition(() => true, ctx => ctx.Set(State.End))
                .Exit(async () => {
                    throws++;                    
                    throw new NullReferenceException();
                })
                .Build();

            sm.On(Trans.End, context => context.Set(State.End));
            sm.State(State.Start).Build();
            sm.State(State.End).Build();
            
            // 1-Error when state machine is not initialized
            sm.Enqueue(Trans.Debug);
            Assert.ThrowsAsync<NullReferenceException>(async () => await sm.Execute(0f));
            // It returns to non-initialized state (state = null)
            Assert.That(throws, Is.EqualTo(1));
            throws = 0;

            // Run again, the SM enters in the initial state "Start"
            await sm.Execute(0f);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Start));
            Assert.That(throws, Is.EqualTo(0));

            // 2-Error when state machine has state. Error in Enter
            sm.Enqueue(Trans.Debug);
            Assert.ThrowsAsync<NullReferenceException>(async () => await sm.Execute(0f));
            // It returns to Start state
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Start));
            Assert.That(throws, Is.EqualTo(1));
            throws = 0;

            // 3-Error when state machine has state. Error in Execute
            sm.Enqueue(Trans.MainMenu);
            Assert.ThrowsAsync<NullReferenceException>(async () => await sm.Execute(0f));
            // It returns to Start state
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Start));
            Assert.That(throws, Is.EqualTo(1));
            throws = 0;

            // 4-Error when state machine has state. Error in Exit
            sm.Enqueue(Trans.Global);
            await sm.Execute(0f);
            sm.Enqueue(Trans.End);
            Assert.ThrowsAsync<NullReferenceException>(async () => await sm.Execute(0f));
            // It returns to Start state
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Global));
            Assert.That(throws, Is.EqualTo(1));
            throws = 0;

            Assert.That(string.Join(",", states), Is.EqualTo(
                "MainMenu:Enter,Global:Enter,Global:Execute"));

        }

    }
}