using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Betauer.StateMachine.Sync;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;
using NullReferenceException = System.NullReferenceException;

namespace Betauer.StateMachine.Tests {
    [TestFixture]
    public class StateMachineTests : Node {
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
            var sm1 = new StateMachineSync<State, Trans>(State.A, "X");
            Assert.That(sm1.Name, Is.EqualTo("X"));

            var sm3 = new StateMachineSync<State, Trans>(State.A);
            Assert.That(sm3.Name, Is.Null);
        }
            
        /*
         * Error cases
         */
        [Test(Description = "InitialState not found on start")]
        public void WrongStartStates() {
            var sm = new StateMachineSync<State, Trans>(State.Global);
            sm.State(State.A).Build();

            // Start state Global not found
            Assert.Throws<KeyNotFoundException>( () => {
                sm.Execute(0);
            });
        }
        
        [Test(Description = "IsState")]
        public void IsStateTests() {
            var sm = new StateMachineSync<State, Trans>(State.Global);
            sm.State(State.Global).Build();
            
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Global));
            Assert.That(sm.IsState(State.Global), Is.True);
            Assert.That(sm.IsState(State.Settings), Is.False);
        }
        
        [Test(Description = "A wrong InitialState can be avoided triggering a transition")]
        public void WrongStartWithTriggering() {
            var sm = new StateMachineSync<State, Trans>(State.Global);
            sm.On(Trans.Audio, context => context.Set(State.Audio));
            sm.State(State.Audio).Build();

            sm.Enqueue(Trans.Audio);
            sm.Execute(0);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Audio));
            Assert.That(sm.IsState(State.Audio), Is.True);
            Assert.That(sm.IsState(State.Global), Is.False);
        }
        
        [Test(Description = "Error when a state changes to a not found state: Replace")]
        public void WrongStatesUnknownStateSet() {
            var sm = new StateMachineSync<State, Trans>(State.A);
            sm.State(State.A).Execute(context => context.Set(State.Debug)).Build();

            Assert.Throws<KeyNotFoundException>(() => sm.Execute(0f));
        }

        [Test(Description = "Error when a state changes to a not found state: PopPush")]
        public void WrongStatesUnknownStatePushPop() {
            var sm = new StateMachineSync<State, Trans>(State.A);
            sm.State(State.A).Execute(context => context.PopPush(State.NotFound)).Build();

            Assert.Throws<KeyNotFoundException>(() => sm.Execute(0f));
        }

        [Test(Description = "Error when a state changes to a not found state: Push")]
        public void WrongStatesUnknownStatePushPush() {
            var sm = new StateMachineSync<State, Trans>(State.A);
            sm.State(State.A).Execute(context => context.Push(State.NotFound)).Build();

            Assert.Throws<KeyNotFoundException>(() => sm.Execute(0f));
        }

        [Test(Description = "Error when a state triggers a not found transition")]
        public void WrongStatesTriggerUnknownTransition() {
            var sm = new StateMachineSync<State, Trans>(State.A);
            sm.State(State.A).Execute(context => context.Trigger(Trans.NotFound)).Build();

            Assert.Throws<KeyNotFoundException>(() => sm.Execute(0f));
        }

        [Test(Description = "Error not found transition")]
        public void WrongUnknownTransition() {
            var sm = new StateMachineSync<State, Trans>(State.A);
            sm.State(State.A).Build();

            sm.Enqueue(Trans.NotFound);
            Assert.Throws<KeyNotFoundException>(() => sm.Execute(0f));
        }

        [Test(Description = "Error when a state pop in an empty stack")]
        public void WrongStatesPopWhenEmptyStack() {
            var sm = new StateMachineSync<State, Trans>(State.A);
            sm.State(State.A).Execute(context => context.Pop()).Build();

            // State ends with a wrong state
            Assert.Throws<InvalidOperationException>(() => sm.Execute(0f));
        }
        
        [Test(Description = "Pop the same state in the stack is allowed")]
        public void PopSameStateInTheStackIsAllowed() {
            var sm = new StateMachineSync<State, Trans>(State.A);
            sm.State(State.A).Execute(context => context.Push(State.A)).Build();

            sm.Execute(0f);
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A }));

            sm.Execute(0f);
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A, State.A }));

            sm.Execute(0f);
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A, State.A , State.A }));
        }
        
        /*
         * Working StateMachine
         */
        [Test(Description = "Regular changes between root states inside state.Execute()")]
        public void StateMachinePlainFlow() {
            var sm = new StateMachineSync<State, Trans>(State.Idle);

            var x = 0;
            List<string> states = new List<string>();

            sm.State(State.Idle)
                .Enter(() => {
                    x = 0;
                    states.Add("IdleEnter");
                })
                .Execute(context => {
                    x++;
                    states.Add("IdleExecute(" + x + ")");
                    if (x == 2) {
                        return context.Set(State.Jump);
                    }

                    return context.None();
                })
                .Build();

            sm.State(State.Jump)
                .Enter(() => {
                    states.Add("JumpEnter");
                })
                .Execute(context => {
                    states.Add("JumpExecute(" + x + ")");
                    return context.Set(State.Attack);
                })
                .Build();
                
            // No exit because it's optional
                
            sm.State(State.Attack)
                // No enter because it's optional
                .Execute(context => {
                    states.Add("AttackExecute");
                    return context.Set(State.Idle);
                })
                .Exit(() => { states.Add("AttackExit"); })
                .Build();
                
            states.Clear();
            sm.Execute(100f);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Idle)); 
            
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("IdleEnter,IdleExecute(1)"));

            states.Clear();
            sm.Execute(100f);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Idle)); 
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("IdleExecute(2)"));
            states.Clear();
            
            states.Clear();
            sm.Execute(100f);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Jump)); 
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("JumpEnter,JumpExecute(2)"));

            states.Clear();
            sm.Execute(100f);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Attack)); 
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("AttackExecute"));

            states.Clear();
            sm.Execute(100f);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Idle)); 
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("AttackExit,IdleEnter,IdleExecute(1)"));

        }


        [Test(Description = "Check delta works")]
        public void DeltaInExecute() {
            var sm = new StateMachineSync<State, Trans>(State.Debug1);
            const float expected1 = 10f;
            const float expected2 = 10f;
            sm.State(State.Debug1).Execute(context => {
                Assert.That(context.Delta, Is.EqualTo(expected1));
                return context.Set(State.Debug2);
            }).Build();
 
            sm.State(State.Debug2).Execute(context => {
                Assert.That(context.Delta, Is.EqualTo(expected2));
                return context.None();
            }).Build();
                
            sm.Execute(expected1);
            sm.Execute(expected2);
        }

        [Test(Description = "State transitions have more priority than global transition")]
        public void StateTransitionTrigger() {
            var sm = new StateMachineSync<State, Trans>(State.Start);

            sm.State(State.Start).On(Trans.Local, context => context.Push(State.Local)).Build();
            sm.On(Trans.Global, context => context.Set(State.Global));
            sm.On(Trans.Local, context => context.Set(State.Global));
            sm.State(State.Global).Build();
            sm.State(State.Local).Build();

            sm.Execute(0);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Start));
            
            sm.Enqueue(Trans.Local);
            sm.Execute(0);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Local));
            
            sm.Enqueue(Trans.Global);
            sm.Execute(0);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Global));
            
            sm.Enqueue(Trans.Local);
            sm.Execute(0);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Global));

        }

        [Test(Description = "Transition calls inside execute has higher priority than the result returned")]
        public void TransitionTriggerInsideExecute() {
            var sm = new StateMachineSync<State, Trans>(State.Start);

            sm.State(State.Start).Execute((ctx) => {
                sm.Enqueue(Trans.NotFound); // IGNORED!!
                sm.Enqueue(Trans.Settings);
                return ctx.Push(State.Audio); // IGNORED!!
            }).Build();
            sm.State(State.Audio).Build();
            sm.State(State.Settings).Build();
                
            sm.On(Trans.Settings, context => context.Set(State.Settings));

            sm.Execute(0);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Start));
            // The second execution has scheduled the 
            sm.Execute(0);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Settings));
        }

        [Test(Description = "Changes using stateMachine change methods")]
        public void TransitionTrigger() {
            var sm = new StateMachineSync<State, Trans>(State.Audio);

            sm.State(State.Debug).Build();
            sm.State(State.MainMenu).On(Trans.Audio, context => context.Push(State.Audio)).Build();
            sm.State(State.Settings).On(Trans.Back, context => context.Set(State.MainMenu)).Build();
            sm.State(State.Audio).On(Trans.Back, context => context.Pop()).Build();
            sm.On(Trans.Restart, context => context.Set(State.MainMenu));
            sm.On(Trans.Settings, context => context.Set(State.Settings));
            sm.On(Trans.MainMenu, context => context.Set(State.MainMenu));

            // Global event
            sm.Execute(0);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Audio));
            sm.Enqueue(Trans.Restart);
            sm.Execute(0);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.MainMenu));
            
            // State event
            sm.Enqueue(Trans.Settings);
            sm.Execute(0);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Settings));
            sm.Enqueue(Trans.Back);
            sm.Execute(0);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.MainMenu));

            // State event: pop
            sm.Enqueue(Trans.MainMenu);
            sm.Execute(0);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.MainMenu));
            sm.Enqueue(Trans.Audio);
            sm.Execute(0);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Audio));
            Assert.That(sm.GetStack(), Is.EqualTo(new [] {State.MainMenu, State.Audio}));
            sm.Enqueue(Trans.Back);
            sm.Execute(0);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.MainMenu));
        }


        [Test]
        public void ValidateFromAndToInEvents() {
            var sm = new StateMachineSync<State, Trans>(State.Start);

            sm.On(Trans.Start, context => context.Set(State.Start));
            sm.On(Trans.Settings, context => context.Set(State.Settings));

            sm.State(State.Start)
                .Enter(from => {
                    // Case 1: enter in initial state, from is the same
                    Assert.That(from, Is.EqualTo(State.Start));
                })
                .Exit(to => {
                    // Case 3: exit to
                    Assert.That(to, Is.EqualTo(State.Settings));
                })
                .Build();
                
            sm.State(State.Settings)
                .Enter(from => {
                    // Case 2: enter from other state
                    Assert.That(from, Is.EqualTo(State.Start));
                })
                .Execute(context => context.Push(State.Audio))
                .Suspend(to => {
                    // Case 3: suspend when push Audio
                    Assert.That(to, Is.EqualTo(State.Audio));
                })
                .Awake(from => {
                    // Case 3: await when pop Audio
                    Assert.That(from, Is.EqualTo(State.Audio));
                })
                .Build();

            sm.State(State.Audio)
                .Enter(from => {
                    Assert.That(from, Is.EqualTo(State.Settings));
                })
                .Execute(context => context.Pop())
                .Exit(to => {
                    Assert.That(to, Is.EqualTo(State.Settings));
                })
                .Build();
            
            sm.Execute(0f);

            sm.Enqueue(Trans.Settings);
            sm.Execute(0f);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Settings));
            
            sm.Execute(0f);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Audio));

            sm.Execute(0f);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Settings));

        }
        
        [Test]
        public void ValidateFromAndToInEventsMultipleExi() {
            var sm = new StateMachineSync<State, Trans>(State.MainMenu);

            sm.On(Trans.Debug, context => context.Set(State.Debug));
            sm.On(Trans.Settings, context => context.Push(State.Settings));
            sm.On(Trans.Audio, context => context.Push(State.Audio));
            sm.State(State.MainMenu)
                .Exit(to => Assert.That(to, Is.EqualTo(State.Debug))).Build();
            sm.State(State.Settings)
                .Exit(to => Assert.That(to, Is.EqualTo(State.MainMenu))).Build();
            sm.State(State.Audio)
                .Exit(to => Assert.That(to, Is.EqualTo(State.Settings))).Build();
            sm.State(State.Debug).Build();

            sm.Execute(0f);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.MainMenu));

            sm.Enqueue(Trans.Settings);
            sm.Execute(0f);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Settings));
            
            sm.Enqueue(Trans.Audio);
            sm.Execute(0f);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Audio));

            sm.Enqueue(Trans.Debug);
            sm.Execute(0f);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Debug));

        }

        [Test]
        public void EnterOnPushExitOnPopSuspendAwakeListener() {
            var sm = new StateMachineSync<State, Trans>(State.Debug);

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
            sm.AddOnExecuteStart((float delta, State state)  => states.Add(state + ":execute.start"));
            sm.AddOnExecuteEnd((State state)  => states.Add(state + ":execute.end"));

            sm.Execute(0f);
            sm.Enqueue(Trans.Settings);
            sm.Execute(0f);
            sm.Enqueue(Trans.Back);
            sm.Execute(0f);
            // Test listener
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo(
                "from:Debug-to:Debug," +
                "Debug:enter," +
                    "Debug:execute.start,Debug:execute.end," +
                    "Debug:suspend," +
                    "from:Debug-to:Settings," +
                    "Settings:enter," +
                        "Settings:execute.start,Settings:execute.end," +
                    "Settings:exit," +
                    "from:Settings-to:Debug," +
                    "Debug:awake," +
                "Debug:execute.start,Debug:execute.end"));

        }

        [Test]
        public void EnterOnPushExitOnPopSuspendAwakeEventsOrder() {
            var sm = new StateMachineSync<State, Trans>(State.Debug);
            
            List<string> states = new List<string>();

            sm.On(Trans.Debug, context => context.Set(State.Debug));
            sm.State(State.Debug)
                .Awake(() => states.Add("Debug:awake"))
                .Enter(() => states.Add("Debug:enter"))
                .Execute(context => {
                    states.Add("Debug");
                    return context.None();

                })
                .Suspend(() => states.Add("Debug:suspend"))
                .Exit(() => states.Add("Debug:exit"))
                .Build();

            sm.On(Trans.MainMenu, context => context.Set(State.MainMenu));
            sm.State(State.MainMenu)
                .Awake(() => states.Add("MainMenu:awake"))
                .Enter(() => states.Add("MainMenu:enter"))
                .Execute(context => {
                    states.Add("MainMenu");
                    return context.None();
                })
                .Suspend(() => states.Add("MainMenu:suspend"))
                .Exit(()=>{
                    states.Add("MainMenu:exit");
                })
                .Build();
                
            
            sm.On(Trans.Settings, context => context.Push(State.Settings));
            sm.State(State.Settings)
                .On(Trans.Audio, context => context.Push(State.Audio))
                .On(Trans.Back, context => context.Pop())
                .Awake(() => states.Add("Settings:awake"))
                .Enter(() => states.Add("Settings:enter"))
                .Execute(context => {
                    states.Add("Settings");
                    return context.None();
                })
                .Suspend(() => states.Add("Settings:suspend"))
                .Exit(() =>{
                    states.Add("Settings:exit");
                })
                .Build();
                
            
            sm.State(State.Audio)
                .On(Trans.Video, context => context.PopPush(State.Video))
                .On(Trans.Back, context => context.Pop())
                .Awake(() => states.Add("Audio:awake"))
                .Enter(() => states.Add("Audio:enter"))
                .Execute(context => {
                    states.Add("Audio");
                    return context.None();
                })
                .Suspend(() => states.Add("Audio:suspend"))
                .Exit(()=>{
                    states.Add("Audio:exit");
                })
                .Build();
                

            sm.State(State.Video)
                .On(Trans.Back, context => context.Pop())
                .Awake(() => states.Add("Video:awake"))
                .Enter(() => states.Add("Video:enter"))
                .Execute(context => {
                    states.Add("Video");
                    return context.None();
                })
                .Suspend(() => states.Add("Video:suspend"))
                .Exit(() => states.Add("Video:exit"))
                .Build();
            
            sm.Execute(0f);
            Console.WriteLine(string.Join(",", states));
            sm.Enqueue(Trans.MainMenu);
            sm.Execute(0f);
            Console.WriteLine(string.Join(",", states));
            sm.Enqueue(Trans.Settings);
            sm.Execute(0f);
            Console.WriteLine(string.Join(",", states));
            sm.Enqueue(Trans.Audio);
            sm.Execute(0f);
            Console.WriteLine(string.Join(",", states));
            sm.Enqueue(Trans.Video);
            sm.Execute(0f);
            Console.WriteLine(string.Join(",", states));
            sm.Enqueue(Trans.Back);
            sm.Execute(0f);
            Console.WriteLine(string.Join(",", states));
            sm.Enqueue(Trans.Back);
            sm.Execute(0f);
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
            sm.Execute(0f);
            Console.WriteLine(string.Join(",", states));
            sm.Enqueue(Trans.Audio);
            sm.Execute(0f);
            Console.WriteLine(string.Join(",", states));
            sm.Enqueue(Trans.Debug);
            sm.Execute(0f);
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo(
                "MainMenu:suspend," +
                    "Settings:enter,Settings,Settings:suspend," +
                        "Audio:enter,Audio," +
                        "Audio:exit,Settings:exit,MainMenu:exit," + // This is the important part: three end in a row 
                "Debug:enter,Debug"));


        }

        [Test]
        public void ErrorChangingState() {
            var sm = new StateMachineSync<State, Trans>(State.Start);

            List<string> states = new List<string>();
            var throws = 0;
            sm.On(Trans.Debug, context => context.Set(State.Debug));
            sm.State(State.Debug)
                .Enter((e) => {
                    throws++;                    
                    throw new NullReferenceException();
                })
                .Execute(ctx => {
                    states.Add("Debug:Execute");
                    return ctx.Set(State.End);
                })
                .Build();

            sm.On(Trans.MainMenu, context => context.Set(State.MainMenu));
            sm.State(State.MainMenu)
                .Enter(() => states.Add("MainMenu:Enter"))
                .Execute(context => {
                    throws++;                    
                    throw new NullReferenceException();
                    return context.None();
                })
                .Build();

            sm.On(Trans.Global, context => context.Set(State.Global));
            sm.State(State.Global)
                .Enter(() => states.Add("Global:Enter"))
                .Execute(context => {
                    states.Add("Global:Execute");
                    return context.Set(State.End);
                })
                .Exit(() => {
                    throws++;                    
                    throw new NullReferenceException();
                })
                .Build();

            sm.On(Trans.End, context => context.Set(State.End));
            sm.State(State.Start).Build();
            sm.State(State.End).Build();
            
            // 1-Error when state machine is not initialized
            sm.Enqueue(Trans.Debug);
            Assert.Throws<NullReferenceException>(() => sm.Execute(0f));
            // It returns to non-initialized state (state = null)
            Assert.That(throws, Is.EqualTo(1));
            throws = 0;

            // Run again, the SM enters in the initial state "Start"
            sm.Execute(0f);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Start));
            Assert.That(throws, Is.EqualTo(0));

            // 2-Error when state machine has state. Error in Enter
            sm.Enqueue(Trans.Debug);
            Assert.Throws<NullReferenceException>(() => sm.Execute(0f));
            // It returns to Start state
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Start));
            Assert.That(throws, Is.EqualTo(1));
            throws = 0;

            // 3-Error when state machine has state. Error in Execute
            sm.Enqueue(Trans.MainMenu);
            Assert.Throws<NullReferenceException>(() => sm.Execute(0f));
            // It returns to Start state
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Start));
            Assert.That(throws, Is.EqualTo(1));
            throws = 0;

            // 4-Error when state machine has state. Error in Exit
            sm.Enqueue(Trans.Global);
            sm.Execute(0f);
            sm.Enqueue(Trans.End);
            Assert.Throws<NullReferenceException>(() => sm.Execute(0f));
            // It returns to Start state
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Global));
            Assert.That(throws, Is.EqualTo(1));
            throws = 0;

            Assert.That(string.Join(",", states), Is.EqualTo(
                "MainMenu:Enter,Global:Enter,Global:Execute"));

        }

    }
}