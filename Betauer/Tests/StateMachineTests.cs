using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Betauer.TestRunner;
using Betauer.StateMachine;
using NUnit.Framework;

namespace Betauer.Tests {
    [TestFixture]
    [Only]
    public class StateMachineTests : NodeTest {
        [Test(Description = "Constructor")]
        public void StateMachineConstructors() {
            var sm1 = new StateMachine.StateMachine("X");
            Assert.That(sm1.Name, Is.EqualTo("X"));

            var sm3 = new StateMachine.StateMachine();
            Assert.That(sm3.Name, Is.Null);
        }

        [Test(Description = "Constructor")]
        public void StateMachineNodeConstructors() {
            var sm1 = new StateMachineNode("X");
            Assert.That(((StateMachine.StateMachine)sm1.StateMachine).Name, Is.EqualTo("X"));
            Assert.That(sm1.Mode, Is.EqualTo(StateMachineNode.ProcessMode.Idle));

            var sm2 = new StateMachineNode(StateMachineNode.ProcessMode.Physics);
            Assert.That(((StateMachine.StateMachine)sm2.StateMachine).Name, Is.Null);
            Assert.That(sm2.Mode, Is.EqualTo(StateMachineNode.ProcessMode.Physics));

            var sm3 = new StateMachineNode();
            Assert.That(((StateMachine.StateMachine)sm3.StateMachine).Name, Is.Null);
            Assert.That(sm3.Mode, Is.EqualTo(StateMachineNode.ProcessMode.Idle));
        }

        /*
         * Error cases
         */

        [Test(Description = "Error with Start")]
        public async Task WrongStartStates() {
            var sm = new StateMachine.StateMachine()
                .CreateBuilder()
                .State("A").End()
                .State("B").End()
                .Build();

            // Execute without Start (current state)
            Assert.ThrowsAsync<StateMachineNotInitializedException>(async () => await sm.Execute(0f));
            
            // Start state not found
            Assert.Throws<KeyNotFoundException>( () => {
                sm.InitialState("NOT FOUND");
            });

            // Start ok
            sm.InitialState("A");
            sm.InitialState("B");
            sm.InitialState("A");
            sm.InitialState("B");
            await sm.Execute(0);
            Assert.That(sm.State.Name, Is.EqualTo("B"));
            
            // Start again error
            Assert.Throws<StateMachineAlreadyStartedException>( () => sm.InitialState("A"));
        }

        [Test(Description = "Error when a state changes to a not found state: set")]
        public async Task WrongStatesUnknownStateSet() {
            var sm = new StateMachine.StateMachine()
                .CreateBuilder()
                .State("B").Execute(() => Transition.Set("NOT FOUND")).End()
                .Build();

            sm.InitialState("B");
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await sm.Execute(0f));
        }

        [Test(Description = "Error when a state changes to a not found state: PopPush")]
        public async Task WrongStatesUnknownStatePushPop() {
            var sm = new StateMachine.StateMachine()
                .CreateBuilder()
                .State("B").Execute(() => Transition.PopPush("NOT FOUND")).End()
                .Build();

            sm.InitialState("B");
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await sm.Execute(0f));
        }

        [Test(Description = "Error when a state changes to a not found state: Push")]
        public async Task WrongStatesUnknownStatePushPush() {
            var sm = new StateMachine.StateMachine()
                .CreateBuilder()
                .State("B").Execute(() => Transition.Push("NOT FOUND")).End()
                .Build();

            sm.InitialState("B");
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await sm.Execute(0f));
        }

        [Test(Description = "Error when a state pop in an empty stack")]
        public async Task WrongStatesPopWhenEmptyStack() {
            var sm = new StateMachine.StateMachine()
                .CreateBuilder()
                .State("A").Execute(Transition.Pop).End()
                .Build();

            // State ends with a wrong state
            sm.InitialState("A");
            Assert.ThrowsAsync<InvalidOperationException>(async () => await sm.Execute(0f));
        }

        [Test(Description = "Error when a state trigger a not found transition")]
        public async Task WrongStatesTriggerUnknownTransition() {
            var sm = new StateMachine.StateMachine()
                .CreateBuilder()
                .State("C").Execute(() => Transition.Trigger("T")).End()
                .Build();

            Assert.Throws<KeyNotFoundException>(() => sm.Trigger("NOT FOUND"));

            sm.InitialState("C");
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await sm.Execute(0f));
        }

        [Test(Description = "Error when a state trigger another transition")]
        public async Task WrongStatesTriggerTransitionOnOtherTransition() {
            var sm = new StateMachine.StateMachine()
                .CreateBuilder()
                .State("C").Execute(() => Transition.Trigger("T")).End()
                .Build();

            Assert.Throws<KeyNotFoundException>(() => sm.Trigger("NOT FOUND"));

            sm.InitialState("C");
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await sm.Execute(0f));
        }

        [Test(Description = "Error when create a transition with another transition")]
        public async Task TriggerOnTrigger() {
            var b = new StateMachine.StateMachine().CreateBuilder();

            Assert.Throws<StackOverflowException>(() => b.State("X").On("T1", Transition.Trigger("L")));
            Assert.Throws<StackOverflowException>(() => b.On("T2", Transition.Trigger("L")));
            Assert.Throws<StackOverflowException>(() => b.Build().On("T3", Transition.Trigger("L")));
            
        }
        
        /*
         * Working StateMachine
         */
        
        [Test(Description = "Regular changes between root states inside state.Execute()")]
        public async Task StateMachinePlainFlow() {
            var builder = new StateMachine.StateMachine().CreateBuilder();

            var x = 0;
            List<string> states = new List<string>();

            builder.State("Idle")
                .Enter(() => {
                    x = 0;
                    states.Add("IdleEnter");
                })
                .Execute(() => {
                    x++;
                    states.Add("IdleExecute(" + x + ")");
                    if (x == 2) {
                        return Transition.Set("Jump");
                    }

                    return Transition.None();
                });

            builder.State("Jump")
                .Enter(() => {
                    states.Add("JumpEnter");
                })
                .Execute(() => {
                    states.Add("JumpExecute(" + x + ")");
                    return Transition.Set("Attack");
                });
            // No exit because it's optional
                
            builder.State("Attack")
                // No enter because it's optional
                .Execute(() => {
                    states.Add("AttackExecute");
                    return Transition.Set("Idle");
                })
                .Exit(() => { states.Add("AttackExit"); });

            var sm = builder.Build();

            sm.InitialState("Idle");

            states.Clear();
            await sm.Execute(100f);
            Assert.That(sm.State.Name, Is.EqualTo("Idle")); 
            Assert.That(sm.Transition.Name, Is.EqualTo("Idle"));
            Assert.That(sm.Transition.State.Name, Is.EqualTo("Idle"));
            Assert.That(sm.Transition.Type, Is.EqualTo(Transition.TransitionType.None));
            
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("IdleEnter,IdleExecute(1)"));

            states.Clear();
            await sm.Execute(100f);
            Assert.That(sm.State.Name, Is.EqualTo("Idle")); 
            Assert.That(sm.Transition.Name, Is.EqualTo("Jump"));
            Assert.That(sm.Transition.State.Name, Is.EqualTo("Jump"));
            Assert.That(sm.Transition.Type, Is.EqualTo(Transition.TransitionType.Change));
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("IdleExecute(2)"));
            states.Clear();
            
            states.Clear();
            await sm.Execute(100f);
            Assert.That(sm.State.Name, Is.EqualTo("Jump")); 
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("JumpEnter,JumpExecute(2)"));

            states.Clear();
            await sm.Execute(100f);
            Assert.That(sm.State.Name, Is.EqualTo("Attack")); 
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("AttackExecute"));

            states.Clear();
            await sm.Execute(100f);
            Assert.That(sm.State.Name, Is.EqualTo("Idle")); 
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("AttackExit,IdleEnter,IdleExecute(1)"));

        }


        [Test(Description = "Check delta works")]
        public async Task DeltaInExecute() {
            var builder = new StateMachine.StateMachine().CreateBuilder();
            const float expected1 = 10f;
            const float expected2 = 10f;
            builder.State("Debug1").Execute(delta => {
                Assert.That(delta, Is.EqualTo(expected1));
                return Transition.Set("Debug2");
            });
            builder.State("Debug2").Execute(async delta => {
                Assert.That(delta, Is.EqualTo(expected2));
                return Transition.None();
            });
            var sm = builder.Build();
            sm.InitialState("Debug1");
            await sm.Execute(expected1);
            await sm.Execute(expected2);
        }

        [Test(Description = "Changes using stateMachine change methods")]
        public async Task TransitionTrigger() {
            var builder = new StateMachine.StateMachine().CreateBuilder();

            builder.State("Debug");
            builder.State("MainMenu").On("Audio", Transition.Push("Audio"));
            builder.State("Settings").On("Back", Transition.Set("MainMenu"));;
            builder.State("Audio").On("Back", Transition.Pop());
            builder.On("Restart", Transition.Set("MainMenu"));
            builder.On("Settings", Transition.Set("Settings"));
            builder.On("MainMenu", Transition.Set("MainMenu"));
            var sm = builder.Build();


            // Global event
            sm.InitialState("Audio");
            await sm.Execute(0);
            Assert.That(sm.State.Name, Is.EqualTo("Audio"));
            sm.Trigger("Restart");
            await sm.Execute(0);
            Assert.That(sm.State.Name, Is.EqualTo("MainMenu"));
            
            // State event
            sm.Trigger("Settings");
            await sm.Execute(0);
            Assert.That(sm.State.Name, Is.EqualTo("Settings"));
            sm.Trigger("Back");
            await sm.Execute(0);
            Assert.That(sm.State.Name, Is.EqualTo("MainMenu"));

            // State event: pop
            sm.Trigger("MainMenu");
            await sm.Execute(0);
            Assert.That(sm.State.Name, Is.EqualTo("MainMenu"));
            sm.Trigger("Audio");
            await sm.Execute(0);
            Assert.That(sm.State.Name, Is.EqualTo("Audio"));
            Assert.That(sm.GetStack(), Is.EqualTo(new [] {"MainMenu", "Audio"}));
            sm.Trigger("Back");
            await sm.Execute(0);
            Assert.That(sm.State.Name, Is.EqualTo("MainMenu"));
        }


        [Test]
        public async Task EnterOnPushExitOnPopSuspendAwakeEventsOrder() {
            var builder = new StateMachine.StateMachine().CreateBuilder();
            
            List<string> states = new List<string>();

            builder.On("Debug", Transition.Set("Debug"));
            builder.State("Debug")
                .Awake(() => states.Add("Debug:awake"))
                .Enter(() => states.Add("Debug:start"))
                .Execute(() => {
                    states.Add("Debug");
                    return Transition.None();

                })
                .Suspend(() => states.Add("Debug:suspend"))
                .Exit(() => states.Add("Debug:end"));

            builder.On("MainMenu", Transition.Set("MainMenu"));
            builder.State("MainMenu")
                .Awake(() => states.Add("MainMenu:awake"))
                .Enter(() => states.Add("MainMenu:start"))
                .Execute(() => {
                    states.Add("MainMenu");
                    return Transition.None();
                })
                .Suspend(() => states.Add("MainMenu:suspend"))
                .Exit(()=>{
                    states.Add("MainMenu:end");
                });
            
            builder.On("Settings", Transition.Push("Settings"));
            builder.State("Settings")
                .On("Audio", Transition.Push("Audio"))
                .On("Back", Transition.Pop())
                .Awake(() => states.Add("Settings:awake"))
                .Enter(async () => states.Add("Settings:start"))
                .Execute(async () => {
                    states.Add("Settings");
                    return Transition.None();
                })
                .Suspend(() => states.Add("Settings:suspend"))
                .Exit(async () =>{
                    states.Add("Settings:end");
                });
            
            builder.State("Audio")
                .On("Video", Transition.PopPush("Video"))
                .On("Back", Transition.Pop())
                .Awake(() => states.Add("Audio:awake"))
                .Enter(() => states.Add("Audio:start"))
                .Execute(() => {
                    states.Add("Audio");
                    return Transition.None();
                })
                .Suspend(() => states.Add("Audio:suspend"))
                .Exit(()=>{
                    states.Add("Audio:end");
                });

            builder.State("Video")
                .On("Back", Transition.Pop())
                .Awake(() => states.Add("Video:awake"))
                .Enter(() => states.Add("Video:start"))
                .Execute(() => {
                    states.Add("Video");
                    return Transition.None();
                })
                .Suspend(() => states.Add("Video:suspend"))
                .Exit(() => states.Add("Video:end"));

            var sm = builder.Build();
            sm.InitialState("Debug");
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
            var stateMachine = new StateMachineNode(StateMachineNode.ProcessMode.Idle);
            var builder = stateMachine.CreateBuilder();

            var x = 0;
            List<string> states = new List<string>();

            builder.State("Start")
                .Execute(async () => {
                    states.Add("Start");
                    await this.AwaitIdleFrame();
                    return Transition.Set("Idle");
                });

            builder.State("Idle")
                .Enter(async () => {
                    await this.AwaitIdleFrame();
                    x = 0;
                })
                .Execute(async () => {
                    await this.AwaitIdleFrame();
                    x++;
                    await this.AwaitIdleFrame();
                    states.Add("IdleExecute(" + x + ")");
                    if (x == 2) {
                        return Transition.Set("Attack");
                    }

                    return Transition.Set("Idle");
                })
                .Exit(async () => {
                    await this.AwaitIdleFrame();
                    states.Add("IdleExit(" + x + ")");
                });

            builder.State("Attack")
                .Execute(async () => {
                    x++;
                    states.Add("AttackExecute(" + x + ")");
                    return Transition.Set("End");
                });

            builder.State("End")
                .Execute(async () => {
                    x++;
                    states.Add("End(" + x + ")");
                    await this.AwaitIdleFrame();
                    return Transition.None();
                });

            builder.Build();
            stateMachine.InitialState("Start");
            AddChild(stateMachine);
            
            stateMachine.BeforeExecute(async (f) => {
                states.Add(">");
            });
            stateMachine.AfterExecute(async (f)  => {
                states.Add("<");
            });

            Stopwatch stopwatch = Stopwatch.StartNew();
            while (stateMachine.State?.Name != "End" && stopwatch.ElapsedMilliseconds < 1000) {
                await this.AwaitIdleFrame();
                
            }
            Assert.That(string.Join(",", states),
                Is.EqualTo(">,Start,<,>,IdleExecute(1),<,>,IdleExecute(2),<,>,IdleExit(2),AttackExecute(3),<,>,End(4)"));
         
        }
    }
}