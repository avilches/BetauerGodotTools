using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

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
            Back,
            Debug,
            Start,
            Restart,
            Video,
            NotFound
        }
        [Test(Description = "Constructor")]
        public void StateMachineConstructorsEnum() {
            var sm1 = new StateMachine<State, Trans>(State.A, "X");
            Assert.That(sm1.Name, Is.EqualTo("X"));

            var sm3 = new StateMachine<State, Trans>(State.A);
            Assert.That(sm3.Name, Is.Null);
        }
             /*
        [Test(Description = "Constructor")]
        public void StateMachineNodeConstructors() {
            var sm1 = new StateMachineNode<State, Trans>(State.A, "X");
            Assert.That(((StateMachine<State, Trans>)sm1.StateMachine).Name, Is.EqualTo("X"));
            Assert.That(sm1.Mode, Is.EqualTo(ProcessMode.Idle));

            var sm2 = new StateMachineNode<State, Trans>(State.A, null, ProcessMode.Physics);
            Assert.That(((StateMachine<State, Trans>)sm2.StateMachine).Name, Is.Null);
            Assert.That(sm2.Mode, Is.EqualTo(ProcessMode.Physics));

            var sm3 = new StateMachineNode<State, Trans>(State.A);
            Assert.That(((StateMachine<State, Trans>)sm3.StateMachine).Name, Is.Null);
            Assert.That(sm3.Mode, Is.EqualTo(ProcessMode.Idle));
        }
           */
        /*
         * Error cases
         */
        [Test(Description = "InitialState not found on start")]
        public async Task WrongStartStates() {
            var sm = new StateMachine<State, Trans>(State.Global);
            sm.CreateState(State.A).Build();

            // Start state Global not found
            Assert.ThrowsAsync<KeyNotFoundException>(async () => {
                await sm.Execute(0);
            });
        }
        [Test(Description = "A wrong InitialState can be avoided triggering a transition")]
        public async Task WrongStartWithTriggering() {
            var sm = new StateMachine<State, Trans>(State.Global);
            sm.On(Trans.Audio, context => context.Set(State.Audio));
            sm.CreateState(State.Audio).Build();

            sm.Enqueue(Trans.Audio);
            await sm.Execute(0);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Audio));
        }
        
        [Test(Description = "Error when a state changes to a not found state: Replace")]
        public async Task WrongStatesUnknownStateSet() {
            var sm = new StateMachine<State, Trans>(State.A);
            sm.CreateState(State.A).Execute(context => context.Set(State.Debug)).Build();

            Assert.ThrowsAsync<KeyNotFoundException>(async () => await sm.Execute(0f));
        }

        [Test(Description = "Error when a state changes to a not found state: PopPush")]
        public async Task WrongStatesUnknownStatePushPop() {
            var sm = new StateMachine<State, Trans>(State.A);
            sm.CreateState(State.A).Execute(context => context.PopPush(State.NotFound)).Build();

            Assert.ThrowsAsync<KeyNotFoundException>(async () => await sm.Execute(0f));
        }

        [Test(Description = "Error when a state changes to a not found state: Push")]
        public async Task WrongStatesUnknownStatePushPush() {
            var sm = new StateMachine<State, Trans>(State.A);
            sm.CreateState(State.A).Execute(context => context.Push(State.NotFound)).Build();

            Assert.ThrowsAsync<KeyNotFoundException>(async () => await sm.Execute(0f));
        }

        [Test(Description = "Error when a state triggers a not found transition")]
        public async Task WrongStatesTriggerUnknownTransition() {
            var sm = new StateMachine<State, Trans>(State.A);
            sm.CreateState(State.A).Execute(context => context.Trigger(Trans.NotFound)).Build();

            Assert.ThrowsAsync<KeyNotFoundException>(async () => await sm.Execute(0f));
        }

        [Test(Description = "Error not found transition")]
        public async Task WrongUnknownTransition() {
            var sm = new StateMachine<State, Trans>(State.A);
            sm.CreateState(State.A).Build();

            sm.Enqueue(Trans.NotFound);
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await sm.Execute(0f));
        }

        [Test(Description = "Error when a state pop in an empty stack")]
        public async Task WrongStatesPopWhenEmptyStack() {
            var sm = new StateMachine<State, Trans>(State.A);
            sm.CreateState(State.A).Execute(context => context.Pop()).Build();

            // State ends with a wrong state
            Assert.ThrowsAsync<InvalidOperationException>(async () => await sm.Execute(0f));
        }
        
        /*
         * Working StateMachine
         */
        [Test(Description = "Regular changes between root states inside state.Execute()")]
        public async Task StateMachinePlainFlow() {
            var sm = new StateMachine<State, Trans>(State.Idle);

            var x = 0;
            List<string> states = new List<string>();

            sm.CreateState(State.Idle)
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

            sm.CreateState(State.Jump)
                .Enter(() => {
                    states.Add("JumpEnter");
                })
                .Execute(context => {
                    states.Add("JumpExecute(" + x + ")");
                    return context.Set(State.Attack);
                })
                .Build();
                
            // No exit because it's optional
                
            sm.CreateState(State.Attack)
                // No enter because it's optional
                .Execute(context => {
                    states.Add("AttackExecute");
                    return context.Set(State.Idle);
                })
                .Exit(() => { states.Add("AttackExit"); })
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
            var sm = new StateMachine<State, Trans>(State.Debug1);
            const float expected1 = 10f;
            const float expected2 = 10f;
            sm.CreateState(State.Debug1).Execute(context => {
                Assert.That(context.Delta, Is.EqualTo(expected1));
                return context.Set(State.Debug2);
            }).Build();
 
            sm.CreateState(State.Debug2).Execute(async context => {
                Assert.That(context.Delta, Is.EqualTo(expected2));
                return context.None();
            }).Build();
                
            await sm.Execute(expected1);
            await sm.Execute(expected2);
        }

        [Test(Description = "State transitions have more priority than global transition")]
        public async Task StateTransitionTrigger() {
            var sm = new StateMachine<State, Trans>(State.Start);

            sm.CreateState(State.Start).On(Trans.Local, context => context.Push(State.Local)).Build();
            sm.On(Trans.Global, context => context.Set(State.Global));
            sm.On(Trans.Local, context => context.Set(State.Global));
            sm.CreateState(State.Global).Build();
            sm.CreateState(State.Local).Build();

            await sm.Execute(0);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Start));
            
            sm.Enqueue(Trans.Local);
            await sm.Execute(0);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Local));
            
            sm.Enqueue(Trans.Global);
            await sm.Execute(0);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Global));
            
            sm.Enqueue(Trans.Local);
            await sm.Execute(0);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Global));

        }

        [Test(Description = "Transition calls inside execute has higher priority than the result returned")]
        public async Task TransitionTriggerInsideExecute() {
            var sm = new StateMachine<State, Trans>(State.Start);

            sm.CreateState(State.Start).Execute((ctx) => {
                sm.Enqueue(Trans.NotFound); // IGNORED!!
                sm.Enqueue(Trans.Settings);
                return ctx.Push(State.Audio); // IGNORED!!
            }).Build();
            sm.CreateState(State.Audio).Build();
            sm.CreateState(State.Settings).Build();
                
            sm.On(Trans.Settings, context => context.Set(State.Settings));

            await sm.Execute(0);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Start));
            // The second execution has scheduled the 
            await sm.Execute(0);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Settings));
        }

        [Test(Description = "Changes using stateMachine change methods")]
        public async Task TransitionTrigger() {
            var sm = new StateMachine<State, Trans>(State.Audio);

            sm.CreateState(State.Debug).Build();
            sm.CreateState(State.MainMenu).On(Trans.Audio, context => context.Push(State.Audio)).Build();
            sm.CreateState(State.Settings).On(Trans.Back, context => context.Set(State.MainMenu)).Build();
            sm.CreateState(State.Audio).On(Trans.Back, context => context.Pop()).Build();
            sm.On(Trans.Restart, context => context.Set(State.MainMenu));
            sm.On(Trans.Settings, context => context.Set(State.Settings));
            sm.On(Trans.MainMenu, context => context.Set(State.MainMenu));

            // Global event
            await sm.Execute(0);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Audio));
            sm.Enqueue(Trans.Restart);
            await sm.Execute(0);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.MainMenu));
            
            // State event
            sm.Enqueue(Trans.Settings);
            await sm.Execute(0);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Settings));
            sm.Enqueue(Trans.Back);
            await sm.Execute(0);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.MainMenu));

            // State event: pop
            sm.Enqueue(Trans.MainMenu);
            await sm.Execute(0);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.MainMenu));
            sm.Enqueue(Trans.Audio);
            await sm.Execute(0);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Audio));
            Assert.That(sm.GetStack(), Is.EqualTo(new [] {State.MainMenu, State.Audio}));
            sm.Enqueue(Trans.Back);
            await sm.Execute(0);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.MainMenu));
        }


        [Test]
        public async Task ValidateFromAndToInEvents() {
            var sm = new StateMachine<State, Trans>(State.Start);

            sm.On(Trans.Start, context => context.Set(State.Start));
            sm.On(Trans.Settings, context => context.Set(State.Settings));

            sm.CreateState(State.Start)
                .Enter(from => {
                    // Case 1: enter in initial state, from is the same
                    Assert.That(from, Is.EqualTo(State.Start));
                })
                .Exit(to => {
                    // Case 3: exit to
                    Assert.That(to, Is.EqualTo(State.Settings));
                })
                .Build();
                
            sm.CreateState(State.Settings)
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

            sm.CreateState(State.Audio)
                .Enter(from => {
                    Assert.That(from, Is.EqualTo(State.Settings));
                })
                .Execute(context => context.Pop())
                .Exit(to => {
                    Assert.That(to, Is.EqualTo(State.Settings));
                })
                .Build();
            
            await sm.Execute(0f);

            sm.Enqueue(Trans.Settings);
            await sm.Execute(0f);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Settings));
            
            await sm.Execute(0f);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Audio));

            await sm.Execute(0f);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Settings));

        }
        
        [Test]
        public async Task ValidateFromAndToInEventsMultipleExi() {
            var sm = new StateMachine<State, Trans>(State.MainMenu);

            sm.On(Trans.Debug, context => context.Set(State.Debug));
            sm.On(Trans.Settings, context => context.Push(State.Settings));
            sm.On(Trans.Audio, context => context.Push(State.Audio));
            sm.CreateState(State.MainMenu)
                .Exit(to => Assert.That(to, Is.EqualTo(State.Debug))).Build();
            sm.CreateState(State.Settings)
                .Exit(to => Assert.That(to, Is.EqualTo(State.MainMenu))).Build();
            sm.CreateState(State.Audio)
                .Exit(to => Assert.That(to, Is.EqualTo(State.Settings))).Build();
            sm.CreateState(State.Debug).Build();

            await sm.Execute(0f);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.MainMenu));

            sm.Enqueue(Trans.Settings);
            await sm.Execute(0f);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Settings));
            
            sm.Enqueue(Trans.Audio);
            await sm.Execute(0f);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Audio));

            sm.Enqueue(Trans.Debug);
            await sm.Execute(0f);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Debug));

        }

        [Test]
        public async Task EnterOnPushExitOnPopSuspendAwakeListener() {
            var sm = new StateMachine<State, Trans>(State.Debug);

            sm.On(Trans.Settings, context => context.Push(State.Settings));
            sm.On(Trans.Back, context => context.Pop());
            sm.CreateState(State.MainMenu).Build();
            sm.CreateState(State.Debug).Build();
            sm.CreateState(State.Settings).Build();
            var stateListener = new StateMachineEvents<State>();
            List<string> states = new List<string>();
            stateListener.Enter += (State state, State from) => states.Add(state + ":enter");
            stateListener.Awake += (State state, State from)  => states.Add(state + ":awake");
            stateListener.Suspend += (State state, State to)  => states.Add(state + ":suspend");
            stateListener.Exit += (State state, State to)  => states.Add(state + ":exit");
            stateListener.Transition += (State from, State to)  => states.Add("from:" + from + "-to:" + to);
            stateListener.ExecuteStart += (float delta, State state)  => states.Add(state + ":execute.start");
            stateListener.ExecuteEnd += (State state)  => states.Add(state + ":execute.end");

            sm.AddListener(stateListener);

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
        public async Task EnterOnPushExitOnPopSuspendAwakeEventsOrder() {
            var sm = new StateMachine<State, Trans>(State.Debug);
            
            List<string> states = new List<string>();

            sm.On(Trans.Debug, context => context.Set(State.Debug));
            sm.CreateState(State.Debug)
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
            sm.CreateState(State.MainMenu)
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
            sm.CreateState(State.Settings)
                .On(Trans.Audio, context => context.Push(State.Audio))
                .On(Trans.Back, context => context.Pop())
                .Awake(() => states.Add("Settings:awake"))
                .Enter(async () => states.Add("Settings:enter"))
                .Execute(async context => {
                    states.Add("Settings");
                    return context.None();
                })
                .Suspend(() => states.Add("Settings:suspend"))
                .Exit(async () =>{
                    states.Add("Settings:exit");
                })
                .Build();
                
            
            sm.CreateState(State.Audio)
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
                

            sm.CreateState(State.Video)
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
         /*
        [Test(Description = "StateMachineNode, BeforeExecute and AfterExecute events with idle frames in the execute")]
        public async Task AsyncStateMachineNodeWithIdleFrame() {
            var stateMachine = new StateMachineNode<State, Trans>(State.Start, null, ProcessMode.Idle);
            var builder = stateMachine;

            var x = 0;
            List<string> states = new List<string>();

            builder.CreateState(State.Start)
                .Execute(async context => {
                    states.Add("Start");
                    await this.AwaitIdleFrame();
                    return context.Set(State.Idle);
                });

            builder.CreateState(State.Idle)
                .Enter(async () => {
                    await this.AwaitIdleFrame();
                    x = 0;
                })
                .Execute(async context => {
                    await this.AwaitIdleFrame();
                    x++;
                    await this.AwaitIdleFrame();
                    states.Add("IdleExecute(" + x + ")");
                    if (x == 2) {
                        return context.Set(State.Attack);
                    }

                    return context.Set(State.Idle);
                })
                .Exit(async () => {
                    await this.AwaitIdleFrame();
                    states.Add("IdleExit(" + x + ")");
                });

            builder.CreateState(State.Attack)
                .Execute(async context => {
                    x++;
                    states.Add("AttackExecute(" + x + ")");
                    return context.Set(State.End);
                });

            builder.CreateState(State.End)
                .Execute(async context => {
                    x++;
                    states.Add("End(" + x + ")");
                    await this.AwaitIdleFrame();
                    return context.None();
                });

            builder.Build();
            AddChild(stateMachine);
            
            Stopwatch stopwatch = Stopwatch.StartNew();
            while (stateMachine.CurrentState?.Key != State.End && stopwatch.ElapsedMilliseconds < 1000) {
                await this.AwaitIdleFrame();
                
            }
            Assert.That(string.Join(",", states),
                Is.EqualTo("Start,IdleExecute(1),IdleExecute(2),IdleExit(2),AttackExecute(3),End(4)"));
         
        }  */
    }
}