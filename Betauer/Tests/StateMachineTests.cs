using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Betauer.TestRunner;
using Betauer.StateMachine;
using NUnit.Framework;

namespace Betauer.Tests {
    [TestFixture]
    public class StateMachineTests : NodeTest {
        enum State {
            A, B                        
        }
        enum Trans {
            T1, T2
            
        }
        [Test(Description = "Constructor")]
        public void StateMachineConstructorsEnum() {
            var sm1 = new StateMachine<State, Trans>(State.A, "X");
            Assert.That(sm1.Name, Is.EqualTo("X"));

            var sm3 = new StateMachine<State, Trans>(State.A);
            Assert.That(sm3.Name, Is.Null);
        }

        [Test(Description = "Constructor")]
        public void StateMachineConstructors() {
            var sm1 = new StateMachine<string, string>("init", "X");
            Assert.That(sm1.Name, Is.EqualTo("X"));

            var sm3 = new StateMachine<string, string>("init");
            Assert.That(sm3.Name, Is.Null);
        }

        [Test(Description = "Constructor")]
        public void StateMachineNodeConstructors() {
            var sm1 = new StateMachineNode<string, string>("init", "X");
            Assert.That(((StateMachine<string, string>)sm1.StateMachine).Name, Is.EqualTo("X"));
            Assert.That(sm1.Mode, Is.EqualTo(ProcessMode.Idle));

            var sm2 = new StateMachineNode<string, string>("init", ProcessMode.Physics);
            Assert.That(((StateMachine<string, string>)sm2.StateMachine).Name, Is.Null);
            Assert.That(sm2.Mode, Is.EqualTo(ProcessMode.Physics));

            var sm3 = new StateMachineNode<string, string>("init");
            Assert.That(((StateMachine<string, string>)sm3.StateMachine).Name, Is.Null);
            Assert.That(sm3.Mode, Is.EqualTo(ProcessMode.Idle));
        }

        /*
         * Error cases
         */

        [Test(Description = "InitialState not found on start")]
        public async Task WrongStartStates() {
            var sm = new StateMachine<string, string>("NOT FOUND")
                .CreateBuilder()
                .State("A").End()
                .Build();

            // Start state not found
            Assert.ThrowsAsync<KeyNotFoundException>(async () => {
                await sm.Execute(0);
            });
        }

        [Test(Description = "A wrong InitialState can be avoided triggering a transition")]
        public async Task WrongStartStates2() {
            var sm = new StateMachine<string, string>("NOT FOUND")
                .CreateBuilder()
                .On("T1", context => context.Set("A"))
                .State("A").End()
                .Build();

            sm.Trigger("T1");
            await sm.Execute(0);
            Assert.That(sm.State.Key, Is.EqualTo("A"));
        }

        [Test(Description = "Error when a state changes to a not found state: set")]
        public async Task WrongStatesUnknownStateSet() {
            var sm = new StateMachine<string, string>("B")
                .CreateBuilder()
                .State("B").Execute(context => context.Set("NOT FOUND")).End()
                .Build();

            Assert.ThrowsAsync<KeyNotFoundException>(async () => await sm.Execute(0f));
        }

        [Test(Description = "Error when a state changes to a not found state: PopPush")]
        public async Task WrongStatesUnknownStatePushPop() {
            var sm = new StateMachine<string, string>("B")
                .CreateBuilder()
                .State("B").Execute(context => context.PopPush("NOT FOUND")).End()
                .Build();

            Assert.ThrowsAsync<KeyNotFoundException>(async () => await sm.Execute(0f));
        }

        [Test(Description = "Error when a state changes to a not found state: Push")]
        public async Task WrongStatesUnknownStatePushPush() {
            var sm = new StateMachine<string, string>("B")
                .CreateBuilder()
                .State("B").Execute(context => context.Push("NOT FOUND")).End()
                .Build();

            Assert.ThrowsAsync<KeyNotFoundException>(async () => await sm.Execute(0f));
        }

        [Test(Description = "Error when a state pop in an empty stack")]
        public async Task WrongStatesPopWhenEmptyStack() {
            var sm = new StateMachine<string, string>("A")
                .CreateBuilder()
                .State("A").Execute(context => context.Pop()).End()
                .Build();

            // State ends with a wrong state
            Assert.ThrowsAsync<InvalidOperationException>(async () => await sm.Execute(0f));
        }

        [Test(Description = "Error when a state trigger a not found transition")]
        public async Task WrongStatesTriggerUnknownTransition() {
            var sm = new StateMachine<string, string>("C")
                .CreateBuilder()
                .State("C").Execute(context => context.Trigger("T")).End()
                .Build();

            Assert.Throws<KeyNotFoundException>(() => sm.Trigger("NOT FOUND"));

            Assert.ThrowsAsync<KeyNotFoundException>(async () => await sm.Execute(0f));
        }

        [Test(Description = "Error when a state trigger another transition")]
        public async Task WrongStatesTriggerTransitionOnOtherTransition() {
            var sm = new StateMachine<string, string>("C")
                .CreateBuilder()
                .State("C").Execute(context => context.Trigger("T")).End()
                .Build();

            Assert.Throws<KeyNotFoundException>(() => sm.Trigger("NOT FOUND"));

            Assert.ThrowsAsync<KeyNotFoundException>(async () => await sm.Execute(0f));
        }
        
        /*
         * Working StateMachine
         */
        
        [Test(Description = "Regular changes between root states inside state.Execute()")]
        public async Task StateMachinePlainFlow() {
            var builder = new StateMachine<string, string>("Idle").CreateBuilder();

            var x = 0;
            List<string> states = new List<string>();

            builder.State("Idle")
                .Enter(() => {
                    x = 0;
                    states.Add("IdleEnter");
                })
                .Execute(context => {
                    x++;
                    states.Add("IdleExecute(" + x + ")");
                    if (x == 2) {
                        return context.Set("Jump");
                    }

                    return context.None();
                });

            builder.State("Jump")
                .Enter(() => {
                    states.Add("JumpEnter");
                })
                .Execute(context => {
                    states.Add("JumpExecute(" + x + ")");
                    return context.Set("Attack");
                });
            // No exit because it's optional
                
            builder.State("Attack")
                // No enter because it's optional
                .Execute(context => {
                    states.Add("AttackExecute");
                    return context.Set("Idle");
                })
                .Exit(() => { states.Add("AttackExit"); });

            var sm = builder.Build();

            states.Clear();
            await sm.Execute(100f);
            Assert.That(sm.State.Key, Is.EqualTo("Idle")); 
            
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("IdleEnter,IdleExecute(1)"));

            states.Clear();
            await sm.Execute(100f);
            Assert.That(sm.State.Key, Is.EqualTo("Idle")); 
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("IdleExecute(2)"));
            states.Clear();
            
            states.Clear();
            await sm.Execute(100f);
            Assert.That(sm.State.Key, Is.EqualTo("Jump")); 
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("JumpEnter,JumpExecute(2)"));

            states.Clear();
            await sm.Execute(100f);
            Assert.That(sm.State.Key, Is.EqualTo("Attack")); 
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("AttackExecute"));

            states.Clear();
            await sm.Execute(100f);
            Assert.That(sm.State.Key, Is.EqualTo("Idle")); 
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("AttackExit,IdleEnter,IdleExecute(1)"));

        }


        [Test(Description = "Check delta works")]
        public async Task DeltaInExecute() {
            var builder = new StateMachine<string, string>("Debug1").CreateBuilder();
            const float expected1 = 10f;
            const float expected2 = 10f;
            builder.State("Debug1").Execute(context => {
                Assert.That(context.Delta, Is.EqualTo(expected1));
                return context.Set("Debug2");
            });
            builder.State("Debug2").Execute(async context => {
                Assert.That(context.Delta, Is.EqualTo(expected2));
                return context.None();
            });
            var sm = builder.Build();
            await sm.Execute(expected1);
            await sm.Execute(expected2);
        }

        [Test(Description = "State transitions have more priority than global transition")]
        public async Task StateTransitionTrigger() {
            var builder = new StateMachine<string, string>("Start").CreateBuilder();

            builder.State("Start").On("Local", context => context.Push("Local"));
            builder.On("Global", context => context.Set("Global"));
            builder.On("Local", context => context.Set("Global"));
            builder.State("Global");
            builder.State("Local");
            var sm = builder.Build();

            await sm.Execute(0);
            Assert.That(sm.State.Key, Is.EqualTo("Start"));
            
            sm.Trigger("Local");
            await sm.Execute(0);
            Assert.That(sm.State.Key, Is.EqualTo("Local"));
            
            sm.Trigger("Global");
            await sm.Execute(0);
            Assert.That(sm.State.Key, Is.EqualTo("Global"));
            
            sm.Trigger("Local");
            await sm.Execute(0);
            Assert.That(sm.State.Key, Is.EqualTo("Global"));

        }

        [Test(Description = "Changes using stateMachine change methods")]
        public async Task TransitionTrigger() {
            var builder = new StateMachine<string, string>("Audio").CreateBuilder();

            builder.State("Debug");
            builder.State("MainMenu").On("Audio", context => context.Push("Audio"));
            builder.State("Settings").On("Back", context => context.Set("MainMenu"));;
            builder.State("Audio").On("Back", context => context.Pop());
            builder.On("Restart", context => context.Set("MainMenu"));
            builder.On("Settings", context => context.Set("Settings"));
            builder.On("MainMenu", context => context.Set("MainMenu"));
            var sm = builder.Build();


            // Global event
            await sm.Execute(0);
            Assert.That(sm.State.Key, Is.EqualTo("Audio"));
            sm.Trigger("Restart");
            await sm.Execute(0);
            Assert.That(sm.State.Key, Is.EqualTo("MainMenu"));
            
            // State event
            sm.Trigger("Settings");
            await sm.Execute(0);
            Assert.That(sm.State.Key, Is.EqualTo("Settings"));
            sm.Trigger("Back");
            await sm.Execute(0);
            Assert.That(sm.State.Key, Is.EqualTo("MainMenu"));

            // State event: pop
            sm.Trigger("MainMenu");
            await sm.Execute(0);
            Assert.That(sm.State.Key, Is.EqualTo("MainMenu"));
            sm.Trigger("Audio");
            await sm.Execute(0);
            Assert.That(sm.State.Key, Is.EqualTo("Audio"));
            Assert.That(sm.GetStack(), Is.EqualTo(new [] {"MainMenu", "Audio"}));
            sm.Trigger("Back");
            await sm.Execute(0);
            Assert.That(sm.State.Key, Is.EqualTo("MainMenu"));
        }


        [Test]
        public async Task EnterOnPushExitOnPopSuspendAwakeEventsOrder() {
            var builder = new StateMachine<string, string>("Debug").CreateBuilder();
            
            List<string> states = new List<string>();

            builder.On("Debug", context => context.Set("Debug"));
            builder.State("Debug")
                .Awake(() => states.Add("Debug:awake"))
                .Enter(() => states.Add("Debug:start"))
                .Execute(context => {
                    states.Add("Debug");
                    return context.None();

                })
                .Suspend(() => states.Add("Debug:suspend"))
                .Exit(() => states.Add("Debug:end"));

            builder.On("MainMenu", context => context.Set("MainMenu"));
            builder.State("MainMenu")
                .Awake(() => states.Add("MainMenu:awake"))
                .Enter(() => states.Add("MainMenu:start"))
                .Execute(context => {
                    states.Add("MainMenu");
                    return context.None();
                })
                .Suspend(() => states.Add("MainMenu:suspend"))
                .Exit(()=>{
                    states.Add("MainMenu:end");
                });
            
            builder.On("Settings", context => context.Push("Settings"));
            builder.State("Settings")
                .On("Audio", context => context.Push("Audio"))
                .On("Back", context => context.Pop())
                .Awake(() => states.Add("Settings:awake"))
                .Enter(async () => states.Add("Settings:start"))
                .Execute(async context => {
                    states.Add("Settings");
                    return context.None();
                })
                .Suspend(() => states.Add("Settings:suspend"))
                .Exit(async () =>{
                    states.Add("Settings:end");
                });
            
            builder.State("Audio")
                .On("Video", context => context.PopPush("Video"))
                .On("Back", context => context.Pop())
                .Awake(() => states.Add("Audio:awake"))
                .Enter(() => states.Add("Audio:start"))
                .Execute(context => {
                    states.Add("Audio");
                    return context.None();
                })
                .Suspend(() => states.Add("Audio:suspend"))
                .Exit(()=>{
                    states.Add("Audio:end");
                });

            builder.State("Video")
                .On("Back", context => context.Pop())
                .Awake(() => states.Add("Video:awake"))
                .Enter(() => states.Add("Video:start"))
                .Execute(context => {
                    states.Add("Video");
                    return context.None();
                })
                .Suspend(() => states.Add("Video:suspend"))
                .Exit(() => states.Add("Video:end"));

            var sm = builder.Build();
            await sm.Execute(0f);
            Console.WriteLine(string.Join(",", states));
            sm.Trigger("MainMenu");
            await sm.Execute(0f);
            Console.WriteLine(string.Join(",", states));
            sm.Trigger("Settings");
            await sm.Execute(0f);
            Console.WriteLine(string.Join(",", states));
            sm.Trigger("Audio");
            await sm.Execute(0f);
            Console.WriteLine(string.Join(",", states));
            sm.Trigger("Video");
            await sm.Execute(0f);
            Console.WriteLine(string.Join(",", states));
            sm.Trigger("Back");
            await sm.Execute(0f);
            Console.WriteLine(string.Join(",", states));
            sm.Trigger("Back");
            await sm.Execute(0f);
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo(
                "Debug:start,Debug,Debug:end," +
                "MainMenu:start,MainMenu,MainMenu:suspend," +
                    "Settings:start,Settings,Settings:suspend," +
                        "Audio:start,Audio,Audio:end," +
                        "Video:start,Video,Video:end," +
                    "Settings:awake,Settings,Settings:end," +
                "MainMenu:awake,MainMenu"));
            
        }

        [Test(Description = "StateMachineNode, BeforeExecute and AfterExecute events with idle frames in the execute")]
        public async Task AsyncStateMachineNodeWithIdleFrame() {
            var stateMachine = new StateMachineNode<string, string>("Start", ProcessMode.Idle);
            var builder = stateMachine.CreateBuilder();

            var x = 0;
            List<string> states = new List<string>();

            builder.State("Start")
                .Execute(async context => {
                    states.Add("Start");
                    await this.AwaitIdleFrame();
                    return context.Set("Idle");
                });

            builder.State("Idle")
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
                        return context.Set("Attack");
                    }

                    return context.Set("Idle");
                })
                .Exit(async () => {
                    await this.AwaitIdleFrame();
                    states.Add("IdleExit(" + x + ")");
                });

            builder.State("Attack")
                .Execute(async context => {
                    x++;
                    states.Add("AttackExecute(" + x + ")");
                    return context.Set("End");
                });

            builder.State("End")
                .Execute(async context => {
                    x++;
                    states.Add("End(" + x + ")");
                    await this.AwaitIdleFrame();
                    return context.None();
                });

            builder.Build();
            AddChild(stateMachine);
            
            stateMachine.BeforeExecute(async (f) => {
                states.Add(">");
            });
            stateMachine.AfterExecute(async (f)  => {
                states.Add("<");
            });

            Stopwatch stopwatch = Stopwatch.StartNew();
            while (stateMachine.State?.Key != "End" && stopwatch.ElapsedMilliseconds < 1000) {
                await this.AwaitIdleFrame();
                
            }
            Assert.That(string.Join(",", states),
                Is.EqualTo(">,Start,<,>,IdleExecute(1),<,>,IdleExecute(2),<,>,IdleExit(2),AttackExecute(3),<,>,End(4)"));
         
        }
    }
}