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

        /*
         * Error cases
         */
        [Test(Description = "InitialState not found on start")]
        public async Task WrongStartStates() {
            var sm = new StateMachine<State, Trans>(State.Global)
                .CreateBuilder()
                .State(State.A).End()
                .Build();

            // Start state Global not found
            Assert.ThrowsAsync<KeyNotFoundException>(async () => {
                await sm.Execute(0);
            });
        }
        [Test(Description = "A wrong InitialState can be avoided triggering a transition")]
        public async Task WrongStartWithTriggering() {
            var sm = new StateMachine<State, Trans>(State.Global)
                .CreateBuilder()
                .On(Trans.Audio, context => context.Set(State.Audio))
                .State(State.Audio).End()
                .Build();

            sm.Enqueue(Trans.Audio);
            await sm.Execute(0);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Audio));
        }
        
        [Test(Description = "Error when a state changes to a not found state: Replace")]
        public async Task WrongStatesUnknownStateSet() {
            var sm = new StateMachine<State, Trans>(State.A)
                .CreateBuilder()
                .State(State.A).Execute(context => context.Set(State.Debug)).End()
                .Build();

            Assert.ThrowsAsync<KeyNotFoundException>(async () => await sm.Execute(0f));
        }

        [Test(Description = "Error when a state changes to a not found state: PopPush")]
        public async Task WrongStatesUnknownStatePushPop() {
            var sm = new StateMachine<State, Trans>(State.A)
                .CreateBuilder()
                .State(State.A).Execute(context => context.PopPush(State.NotFound)).End()
                .Build();

            Assert.ThrowsAsync<KeyNotFoundException>(async () => await sm.Execute(0f));
        }

        [Test(Description = "Error when a state changes to a not found state: Push")]
        public async Task WrongStatesUnknownStatePushPush() {
            var sm = new StateMachine<State, Trans>(State.A)
                .CreateBuilder()
                .State(State.A).Execute(context => context.Push(State.NotFound)).End()
                .Build();

            Assert.ThrowsAsync<KeyNotFoundException>(async () => await sm.Execute(0f));
        }

        [Test(Description = "Error when a state triggers a not found transition")]
        public async Task WrongStatesTriggerUnknownTransition() {
            var sm = new StateMachine<State, Trans>(State.A)
                .CreateBuilder()
                .State(State.A).Execute(context => context.Trigger(Trans.NotFound)).End()
                .Build();

            Assert.ThrowsAsync<KeyNotFoundException>(async () => await sm.Execute(0f));
        }

        [Test(Description = "Error not found transition")]
        public async Task WrongUnknownTransition() {
            var sm = new StateMachine<State, Trans>(State.A)
                .CreateBuilder()
                .State(State.A).End()
                .Build();

            sm.Enqueue(Trans.NotFound);
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await sm.Execute(0f));
        }

        [Test(Description = "Error when a state pop in an empty stack")]
        public async Task WrongStatesPopWhenEmptyStack() {
            var sm = new StateMachine<State, Trans>(State.A)
                .CreateBuilder()
                .State(State.A).Execute(context => context.Pop()).End()
                .Build();

            // State ends with a wrong state
            Assert.ThrowsAsync<InvalidOperationException>(async () => await sm.Execute(0f));
        }
        
        /*
         * Working StateMachine
         */
        [Test(Description = "Regular changes between root states inside state.Execute()")]
        public async Task StateMachinePlainFlow() {
            var builder = new StateMachine<State, Trans>(State.Idle).CreateBuilder();

            var x = 0;
            List<string> states = new List<string>();

            builder.State(State.Idle)
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
                });

            builder.State(State.Jump)
                .Enter(() => {
                    states.Add("JumpEnter");
                })
                .Execute(context => {
                    states.Add("JumpExecute(" + x + ")");
                    return context.Set(State.Attack);
                });
            // No exit because it's optional
                
            builder.State(State.Attack)
                // No enter because it's optional
                .Execute(context => {
                    states.Add("AttackExecute");
                    return context.Set(State.Idle);
                })
                .Exit(() => { states.Add("AttackExit"); });

            var sm = builder.Build();

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
            var builder = new StateMachine<State, Trans>(State.Debug1).CreateBuilder();
            const float expected1 = 10f;
            const float expected2 = 10f;
            builder.State(State.Debug1).Execute(context => {
                Assert.That(context.Delta, Is.EqualTo(expected1));
                return context.Set(State.Debug2);
            });
            builder.State(State.Debug2).Execute(async context => {
                Assert.That(context.Delta, Is.EqualTo(expected2));
                return context.None();
            });
            var sm = builder.Build();
            await sm.Execute(expected1);
            await sm.Execute(expected2);
        }

        [Test(Description = "State transitions have more priority than global transition")]
        public async Task StateTransitionTrigger() {
            var builder = new StateMachine<State, Trans>(State.Start).CreateBuilder();

            builder.State(State.Start).On(Trans.Local, context => context.Push(State.Local));
            builder.On(Trans.Global, context => context.Set(State.Global));
            builder.On(Trans.Local, context => context.Set(State.Global));
            builder.State(State.Global);
            builder.State(State.Local);
            var sm = builder.Build();

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
            var builder = sm.CreateBuilder();
            
            builder.State(State.Start).Execute((ctx) => {
                sm.Enqueue(Trans.NotFound); // IGNORED!!
                sm.Enqueue(Trans.Settings);
                return ctx.Push(State.Audio); // IGNORED!!
            });
            builder.State(State.Audio);
            builder.State(State.Settings);
            builder.On(Trans.Settings, context => context.Set(State.Settings));
            builder.Build();

            await sm.Execute(0);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Start));
            // The second execution has scheduled the 
            await sm.Execute(0);
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Settings));
        }

        [Test(Description = "Changes using stateMachine change methods")]
        public async Task TransitionTrigger() {
            var builder = new StateMachine<State, Trans>(State.Audio).CreateBuilder();

            builder.State(State.Debug);
            builder.State(State.MainMenu).On(Trans.Audio, context => context.Push(State.Audio));
            builder.State(State.Settings).On(Trans.Back, context => context.Set(State.MainMenu));
            builder.State(State.Audio).On(Trans.Back, context => context.Pop());
            builder.On(Trans.Restart, context => context.Set(State.MainMenu));
            builder.On(Trans.Settings, context => context.Set(State.Settings));
            builder.On(Trans.MainMenu, context => context.Set(State.MainMenu));
            var sm = builder.Build();


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
            var builder = new StateMachine<State, Trans>(State.Start).CreateBuilder();

            builder.On(Trans.Start, context => context.Set(State.Start));
            builder.On(Trans.Settings, context => context.Set(State.Settings));

            builder.State(State.Start)
                .Enter(from => {
                    // Case 1: enter in initial state, from is the same
                    Assert.That(from, Is.EqualTo(State.Start));
                })
                .Exit(to => {
                    // Case 3: exit to
                    Assert.That(to, Is.EqualTo(State.Settings));
                });
                
            builder.State(State.Settings)
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
                });

            builder.State(State.Audio)
                .Enter(from => {
                    Assert.That(from, Is.EqualTo(State.Settings));
                })
                .Execute(context => context.Pop())
                .Exit(to => {
                    Assert.That(to, Is.EqualTo(State.Settings));
                });
            
            
            var sm = builder.Build();
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
            var builder = new StateMachine<State, Trans>(State.MainMenu).CreateBuilder();

            builder.On(Trans.Debug, context => context.Set(State.Debug));
            builder.On(Trans.Settings, context => context.Push(State.Settings));
            builder.On(Trans.Audio, context => context.Push(State.Audio));
            builder.State(State.MainMenu)
                .Exit(to => Assert.That(to, Is.EqualTo(State.Debug)));
            builder.State(State.Settings)
                .Exit(to => Assert.That(to, Is.EqualTo(State.MainMenu)));
            builder.State(State.Audio)
                .Exit(to => Assert.That(to, Is.EqualTo(State.Settings)));
            builder.State(State.Debug);
            
            var sm = builder.Build();

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

        class TestMachineListener : IStateMachineListener<State> {
            internal List<string> states = new List<string>();
            public void OnEnter(State state, State from) {
                states.Add(state + ":enter");
            }

            public void OnAwake(State state, State from) {
                states.Add(state + ":awake");
            }

            public void OnSuspend(State state, State to) {
                states.Add(state + ":suspend");
            }

            public void OnExit(State state, State to) {
                states.Add(state + ":exit");
            }

            public void OnTransition(State from, State to) {
                states.Add("from:" + from + "-to:" + to);
            }

            public void OnExecuteStart(float delta, State state) {
                states.Add(state + ":execute.start");
            }

            public void OnExecuteEnd(State state) {
                states.Add(state + ":execute.end");
            }
        }

        [Test]
        public async Task EnterOnPushExitOnPopSuspendAwakeListener() {
            var builder = new StateMachine<State, Trans>(State.Debug).CreateBuilder();


            builder.On(Trans.Settings, context => context.Push(State.Settings));
            builder.On(Trans.Back, context => context.Pop());
            builder.State(State.MainMenu);
            builder.State(State.Debug);
            builder.State(State.Settings);
            var stateListener = new TestMachineListener();
            var sm = builder.AddListener(stateListener).Build();

            await sm.Execute(0f);
            sm.Enqueue(Trans.Settings);
            await sm.Execute(0f);
            sm.Enqueue(Trans.Back);
            await sm.Execute(0f);
            // Test listener
            Console.WriteLine(string.Join(",", stateListener.states));
            Assert.That(string.Join(",", stateListener.states), Is.EqualTo(
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
            var builder = new StateMachine<State, Trans>(State.Debug).CreateBuilder();
            
            List<string> states = new List<string>();

            builder.On(Trans.Debug, context => context.Set(State.Debug));
            builder.State(State.Debug)
                .Awake(() => states.Add("Debug:awake"))
                .Enter(() => states.Add("Debug:enter"))
                .Execute(context => {
                    states.Add("Debug");
                    return context.None();

                })
                .Suspend(() => states.Add("Debug:suspend"))
                .Exit(() => states.Add("Debug:exit"));

            builder.On(Trans.MainMenu, context => context.Set(State.MainMenu));
            builder.State(State.MainMenu)
                .Awake(() => states.Add("MainMenu:awake"))
                .Enter(() => states.Add("MainMenu:enter"))
                .Execute(context => {
                    states.Add("MainMenu");
                    return context.None();
                })
                .Suspend(() => states.Add("MainMenu:suspend"))
                .Exit(()=>{
                    states.Add("MainMenu:exit");
                });
            
            builder.On(Trans.Settings, context => context.Push(State.Settings));
            builder.State(State.Settings)
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
                });
            
            builder.State(State.Audio)
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
                });

            builder.State(State.Video)
                .On(Trans.Back, context => context.Pop())
                .Awake(() => states.Add("Video:awake"))
                .Enter(() => states.Add("Video:enter"))
                .Execute(context => {
                    states.Add("Video");
                    return context.None();
                })
                .Suspend(() => states.Add("Video:suspend"))
                .Exit(() => states.Add("Video:exit"));
            
            var sm = builder.Build();
            
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

        [Test(Description = "StateMachineNode, BeforeExecute and AfterExecute events with idle frames in the execute")]
        public async Task AsyncStateMachineNodeWithIdleFrame() {
            var stateMachine = new StateMachineNode<State, Trans>(State.Start, null, ProcessMode.Idle);
            var builder = stateMachine.CreateBuilder();

            var x = 0;
            List<string> states = new List<string>();

            builder.State(State.Start)
                .Execute(async context => {
                    states.Add("Start");
                    await this.AwaitIdleFrame();
                    return context.Set(State.Idle);
                });

            builder.State(State.Idle)
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

            builder.State(State.Attack)
                .Execute(async context => {
                    x++;
                    states.Add("AttackExecute(" + x + ")");
                    return context.Set(State.End);
                });

            builder.State(State.End)
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
         
        }
    }
}