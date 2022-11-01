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
                sm.Execute();
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
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Audio));
            Assert.That(sm.IsState(State.Audio), Is.True);
            Assert.That(sm.IsState(State.Global), Is.False);
        }
        
        [Test(Description = "Error when a state changes to a not found state: Replace")]
        public void WrongStatesUnknownStateSet() {
            var sm = new StateMachineSync<State, Trans>(State.A);
            sm.State(State.A).Condition(() => true, context => context.Set(State.Debug)).Build();

            Assert.Throws<KeyNotFoundException>(() => sm.Execute());
        }

        [Test(Description = "Error when a state changes to a not found state: PopPush")]
        public void WrongStatesUnknownStatePushPop() {
            var sm = new StateMachineSync<State, Trans>(State.A);
            sm.State(State.A).Condition(() => true, context => context.PopPush(State.NotFound)).Build();

            Assert.Throws<KeyNotFoundException>(() => sm.Execute());
        }

        [Test(Description = "Error when a state changes to a not found state: Push")]
        public void WrongStatesUnknownStatePushPush() {
            var sm = new StateMachineSync<State, Trans>(State.A);
            sm.State(State.A).Condition(() => true, context => context.Push(State.NotFound)).Build();

            Assert.Throws<KeyNotFoundException>(() => sm.Execute());
        }

        [Test(Description = "Error when a state triggers a not found transition")]
        public void WrongStatesTriggerUnknownTransition() {
            var sm = new StateMachineSync<State, Trans>(State.A);
            sm.State(State.A).Condition(() => true, context => context.Trigger(Trans.NotFound)).Build();

            Assert.Throws<KeyNotFoundException>(() => sm.Execute());
        }

        [Test(Description = "Error not found transition")]
        public void WrongUnknownTransition() {
            var sm = new StateMachineSync<State, Trans>(State.A);
            sm.State(State.A).Build();

            sm.Enqueue(Trans.NotFound);
            Assert.Throws<KeyNotFoundException>(() => sm.Execute());
        }

        [Test(Description = "Error when a state pop in an empty stack")]
        public void WrongStatesPopWhenEmptyStack() {
            var sm = new StateMachineSync<State, Trans>(State.A);
            sm.State(State.A).Condition(() => true, context => context.Pop()).Build();

            // State ends with a wrong state
            Assert.Throws<InvalidOperationException>(() => sm.Execute());
        }
        
        [Test(Description = "Pop the same state in the stack is allowed")]
        public void PopSameStateInTheStackIsAllowed() {
            var sm = new StateMachineSync<State, Trans>(State.A);
            sm.State(State.A).Condition(() => true, context => context.Push(State.A)).Build();

            sm.Execute();
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A }));

            sm.Execute();
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A, State.A }));

            sm.Execute();
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
                .Execute(() => {
                    x++;
                    states.Add("IdleExecute(" + x + ")");
                })
                .Condition(() => x == 2, context => context.Set(State.Jump))
                .Condition(() => true, context => context.None())
                .Build();

            sm.State(State.Jump)
                .Enter(() => {
                    states.Add("JumpEnter");
                })
                .Execute(() => states.Add("JumpExecute(" + x + ")"))
                .Condition(() => true, context => context.Set(State.Attack))
                .Build();
                
            // No exit because it's optional
                
            sm.State(State.Attack)
                // No enter because it's optional
                .Execute(() => states.Add("AttackExecute"))
                .Condition(() => true, context => context.Set(State.Idle))
                .Exit(() => { states.Add("AttackExit"); })
                .Build();
                
            states.Clear();
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Idle)); 
            
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("IdleEnter,IdleExecute(1)"));

            states.Clear();
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Idle)); 
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("IdleExecute(2)"));
            states.Clear();
            
            states.Clear();
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Jump)); 
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("JumpEnter,JumpExecute(2)"));

            states.Clear();
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Attack)); 
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("AttackExecute"));

            states.Clear();
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Idle)); 
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("AttackExit,IdleEnter,IdleExecute(1)"));

        }

        [Test(Description = "State transitions have more priority than global transition")]
        public void StateTransitionTrigger() {
            var sm = new StateMachineSync<State, Trans>(State.Start);

            sm.State(State.Start).On(Trans.Local, context => context.Push(State.Local)).Build();
            sm.On(Trans.Global, context => context.Set(State.Global));
            sm.On(Trans.Local, context => context.Set(State.Global));
            sm.State(State.Global).Build();
            sm.State(State.Local).Build();

            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Start));
            
            sm.Enqueue(Trans.Local);
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Local));
            
            sm.Enqueue(Trans.Global);
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Global));
            
            sm.Enqueue(Trans.Local);
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Global));

        }

        [Test(Description = "Transition calls inside execute has higher priority than the result returned")]
        public void TransitionTriggerInsideExecute() {
            var sm = new StateMachineSync<State, Trans>(State.Start);

            sm.State(State.Start).Execute(() => {
                sm.Enqueue(Trans.NotFound); // IGNORED!!
                sm.Enqueue(Trans.Settings);
            })
            .Condition(() => true, context => context.Push(State.Audio)) // IGNORED!!
            .Build();
            sm.State(State.Audio).Build();
            sm.State(State.Settings).Build();
                
            sm.On(Trans.Settings, context => context.Set(State.Settings));

            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Start));
            // The second execution has scheduled the 
            sm.Execute();
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
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Audio));
            sm.Enqueue(Trans.Restart);
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.MainMenu));
            
            // State event
            sm.Enqueue(Trans.Settings);
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Settings));
            sm.Enqueue(Trans.Back);
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.MainMenu));

            // State event: pop
            sm.Enqueue(Trans.MainMenu);
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.MainMenu));
            sm.Enqueue(Trans.Audio);
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Audio));
            Assert.That(sm.GetStack(), Is.EqualTo(new [] {State.MainMenu, State.Audio}));
            sm.Enqueue(Trans.Back);
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.MainMenu));
        }


        [Test]
        public void EnterOnPushExitOnPopSuspendAwakeEventsOrder() {
            var sm = new StateMachineSync<State, Trans>(State.Debug);
            
            List<string> states = new List<string>();

            sm.On(Trans.Debug, context => context.Set(State.Debug));
            sm.State(State.Debug)
                .Awake(() => states.Add("Debug:awake"))
                .Enter(() => states.Add("Debug:enter"))
                .Execute(() => states.Add("Debug"))
                .Condition(() => true, context => context.None())
                .Suspend(() => states.Add("Debug:suspend"))
                .Exit(() => states.Add("Debug:exit"))
                .Build();

            sm.On(Trans.MainMenu, context => context.Set(State.MainMenu));
            sm.State(State.MainMenu)
                .Awake(() => states.Add("MainMenu:awake"))
                .Enter(() => states.Add("MainMenu:enter"))
                .Execute(() => states.Add("MainMenu"))
                .Condition(() => true, context => context.None())
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
                .Execute(() => states.Add("Settings"))
                .Condition(() => true, context => context.None())
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
                .Execute(() => states.Add("Audio"))
                .Condition(() => true, context => context.None())
                .Suspend(() => states.Add("Audio:suspend"))
                .Exit(()=>{
                    states.Add("Audio:exit");
                })
                .Build();
                

            sm.State(State.Video)
                .On(Trans.Back, context => context.Pop())
                .Awake(() => states.Add("Video:awake"))
                .Enter(() => states.Add("Video:enter"))
                .Execute(() => states.Add("Video"))
                .Condition(() => true, context => context.None())
                .Suspend(() => states.Add("Video:suspend"))
                .Exit(() => states.Add("Video:exit"))
                .Build();
            
            sm.Execute();
            Console.WriteLine(string.Join(",", states));
            sm.Enqueue(Trans.MainMenu);
            sm.Execute();
            Console.WriteLine(string.Join(",", states));
            sm.Enqueue(Trans.Settings);
            sm.Execute();
            Console.WriteLine(string.Join(",", states));
            sm.Enqueue(Trans.Audio);
            sm.Execute();
            Console.WriteLine(string.Join(",", states));
            sm.Enqueue(Trans.Video);
            sm.Execute();
            Console.WriteLine(string.Join(",", states));
            sm.Enqueue(Trans.Back);
            sm.Execute();
            Console.WriteLine(string.Join(",", states));
            sm.Enqueue(Trans.Back);
            sm.Execute();
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
            sm.Execute();
            Console.WriteLine(string.Join(",", states));
            sm.Enqueue(Trans.Audio);
            sm.Execute();
            Console.WriteLine(string.Join(",", states));
            sm.Enqueue(Trans.Debug);
            sm.Execute();
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
                .Enter(() => {
                    throws++;                    
                    throw new NullReferenceException();
                })
                .Execute(() => states.Add("Debug:Execute"))
                .Condition(() => true, ctx => ctx.Set(State.End))
                .Build();

            sm.On(Trans.MainMenu, context => context.Set(State.MainMenu));
            sm.State(State.MainMenu)
                .Enter(() => states.Add("MainMenu:Enter"))
                .Execute(() => {
                    throws++;                    
                    throw new NullReferenceException();
                })
                .Build();

            sm.On(Trans.Global, context => context.Set(State.Global));
            sm.State(State.Global)
                .Enter(() => states.Add("Global:Enter"))
                .Execute(() => states.Add("Global:Execute"))
                .Condition(() => true, ctx => ctx.Set(State.End))
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
            Assert.Throws<NullReferenceException>(() => sm.Execute());
            // It returns to non-initialized state (state = null)
            Assert.That(throws, Is.EqualTo(1));
            throws = 0;

            // Run again, the SM enters in the initial state "Start"
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Start));
            Assert.That(throws, Is.EqualTo(0));

            // 2-Error when state machine has state. Error in Enter
            sm.Enqueue(Trans.Debug);
            Assert.Throws<NullReferenceException>(() => sm.Execute());
            // It returns to Start state
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Start));
            Assert.That(throws, Is.EqualTo(1));
            throws = 0;

            // 3-Error when state machine has state. Error in Execute
            sm.Enqueue(Trans.MainMenu);
            Assert.Throws<NullReferenceException>(() => sm.Execute());
            // It returns to Start state
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Start));
            Assert.That(throws, Is.EqualTo(1));
            throws = 0;

            // 4-Error when state machine has state. Error in Exit
            sm.Enqueue(Trans.Global);
            sm.Execute();
            sm.Enqueue(Trans.End);
            Assert.Throws<NullReferenceException>(() => sm.Execute());
            // It returns to Start state
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Global));
            Assert.That(throws, Is.EqualTo(1));
            throws = 0;

            Assert.That(string.Join(",", states), Is.EqualTo(
                "MainMenu:Enter,Global:Enter,Global:Execute"));

        }

    }
}