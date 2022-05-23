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
        [Test(Description = "Regular change between root states with immediate or next frame using inside every state")]
        public async Task StateMachinePlainFlow() {
            var builder = new StateMachine.StateMachine(this, "X").CreateBuilder();

            var x = 0;
            List<string> states = new List<string>();

            builder.State("Idle")
                .Enter(context => {
                    Assert.That(context.CurrentState.Name, Is.EqualTo("Idle"));
                    x = 0;
                    states.Add("IdleEnter");
                })
                .Execute(context => {
                    Assert.That(context.CurrentState.Name, Is.EqualTo("Idle"));
                    x++;
                    states.Add("IdleExecute(" + x + ")");
                    if (x == 2) {
                        return context.Immediate("Jump");
                    }

                    return context.Repeat();
                })
                .End()
                .State("Jump")
                .Enter(context => {
                    Assert.That(context.CurrentState.Name, Is.EqualTo("Jump"));
                    Assert.That(context.FromState.Name, Is.EqualTo("Idle"));
                    states.Add("JumpEnter");
                })
                .Execute(context => {
                    Assert.That(context.CurrentState.Name, Is.EqualTo("Jump"));
                    Assert.That(context.FromState.Name, Is.EqualTo("Idle"));
                    states.Add("JumpExecute(" + x + ")");
                    return context.Immediate("Attack");
                })
                .Exit(() => { states.Add("JumpExit"); })
                .End()
                .State("Attack")
                // No enter because it's optional
                .Execute(context => {
                    Assert.That(context.CurrentState.Name, Is.EqualTo("Attack"));
                    states.Add("AttackExecute");
                    return context.NextFrame("Idle");
                })
                .Exit(() => { states.Add("AttackExit"); });

            var stateMachine = builder.Build();

            stateMachine.SetNextState("Idle");

            states.Clear();
            await stateMachine.Execute(100f);
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("IdleEnter,IdleExecute(1)"));

            states.Clear();
            await stateMachine.Execute(100f);
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states),
                Is.EqualTo("IdleExecute(2),JumpEnter,JumpExecute(2),JumpExit,AttackExecute"));

            states.Clear();
            await stateMachine.Execute(100f);
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("AttackExit,IdleEnter,IdleExecute(1)"));

            states.Clear();
            await stateMachine.Execute(100f);
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states),
                Is.EqualTo("IdleExecute(2),JumpEnter,JumpExecute(2),JumpExit,AttackExecute"));
        }

        [Test(Description = "Changes with nested states using stateMachine change methods")]
        public void BaseState() {
            var sm = new StateMachine.StateMachine(this, "X");

            // sm.AddState();
            // var builder = sm.CreateBuilder();
            
            
            // var sm = builder
                // .State("Debug").End()
                // .State("MainMenu")
        }

        [Test]
        public void AsyncActions() {
            var sm = new StateMachine.StateMachine(this, "X")
                .CreateBuilder()
                .State("A").Execute(context => context.Immediate("B")).End()
                .State("B").Execute(context => context.Immediate("A")).End()
                .Build();

            sm.SetNextState("A");
            Assert.ThrowsAsync<StackOverflowException>(async () => await sm.Execute(0f));
        }

        [Test]
        public void InfiniteLoopChange() {
            var sm = new StateMachine.StateMachine(this, "X")
                .CreateBuilder()
                .State("A").Execute(context => context.Immediate("B")).End()
                .State("B").Execute(context => context.Immediate("A")).End()
                .Build();

            sm.SetNextState("A");
            Assert.ThrowsAsync<StackOverflowException>(async () => await sm.Execute(0f));
        }

        [Test]
        public void InfiniteLoopPushPop() {
            var sm = new StateMachine.StateMachine(this, "X")
                .CreateBuilder()
                .State("N1").Execute(context => context.Immediate("N2"))
                    .State("N2").Execute(context => context.PopImmediate()).End()
                .End()
                .Build();

            sm.SetNextState("N1");
            Assert.ThrowsAsync<StackOverflowException>(async () => await sm.Execute(0f));
        }

        [Test(Description = "Changes with nested states using stateMachine change methods")]
        public async Task NestedStates() {
            var builder = new StateMachine.StateMachine(this, "X").CreateBuilder();

            var sm = builder
                .State("Debug").End()
                .State("MainMenu")
                    .State("Settings")
                        .State("Audio").End()
                        .State("Video").End()
                    .End()
                .End()
                .Build();

            Assert.That(sm.FindState("Debug").Parent, Is.Null);
            Assert.That(sm.FindState("MainMenu").Parent, Is.Null);
            Assert.That(sm.FindState("Settings").Parent, Is.EqualTo(sm.FindState("MainMenu")));
            Assert.That(sm.FindState("Audio").Parent, Is.EqualTo(sm.FindState("Settings")));
            Assert.That(sm.FindState("Video").Parent, Is.EqualTo(sm.FindState("Settings")));

            // Wrong initial state
            Assert.ThrowsAsync<Exception>(async () => await sm.Execute(0f));

            Assert.Throws<Exception>((() => sm.PopNextState()));
            Assert.Throws<Exception>((() => sm.SetNextState("Settings")));
            Assert.Throws<Exception>((() => sm.SetNextState("Audio")));
            sm.SetNextState("Debug");
            await sm.Execute(0f);
            Assert.That(sm.CurrentState.Name, Is.EqualTo("Debug"));

            // Wrong next states from Debug
            Assert.Throws<Exception>((() => sm.PopNextState()));
            Assert.Throws<Exception>((() => sm.SetNextState("Settings")));
            Assert.Throws<Exception>((() => sm.SetNextState("Audio")));
            Assert.Throws<Exception>((() => sm.SetNextState("Video")));

            // Debug to MainMenu (sibling)
            sm.SetNextState("MainMenu");
            await sm.Execute(0f);
            Assert.That(sm.CurrentState.Name, Is.EqualTo("MainMenu"));

            // Wrong next states from MainMenu
            Assert.Throws<Exception>((() => sm.PopNextState()));
            Assert.Throws<Exception>((() => sm.SetNextState("Audio")));
            Assert.Throws<Exception>((() => sm.SetNextState("Video")));

            // MainMenu to Settings (child)
            sm.SetNextState("Settings");
            await sm.Execute(0f);
            Assert.That(sm.CurrentState.Name, Is.EqualTo("Settings"));
            Assert.Throws<Exception>((() => sm.SetNextState("Debug")));

            // Settings to Audio (child)
            sm.SetNextState("Audio");
            await sm.Execute(0f);
            Assert.That(sm.CurrentState.Name, Is.EqualTo("Audio"));
            Assert.Throws<Exception>((() => sm.SetNextState("MainMenu")));
            Assert.Throws<Exception>((() => sm.SetNextState("Debug")));

            // POP: Audio to Settings (parent)
            sm.SetNextState("Settings");
            await sm.Execute(0f);
            Assert.That(sm.CurrentState.Name, Is.EqualTo("Settings"));

            // POP: Settings to MainMenu (parent)
            sm.PopNextState();
            await sm.Execute(0f);
            Assert.That(sm.CurrentState.Name, Is.EqualTo("MainMenu"));
            Assert.Throws<Exception>(() => sm.PopNextState());
        }
        
        [Test]
        public async Task NestedStatesPushPopFlow() {
            var builder = new StateMachine.StateMachine(this, "X").CreateBuilder();
            List<string> states = new List<string>();

            var sm = builder
                .State("Debug")
                .Enter(context => {
                    states.Add("Debug:start");
                })
                .Execute(context => {
                    states.Add("Debug");
                    return context.Repeat();

                })
                .Exit(() => {
                    states.Add("Debug:end");
                })
                .End()
                .State("MainMenu")
                .Enter(context => {
                    states.Add("MainMenu:start");
                })
                .Execute(context => {
                    states.Add("MainMenu");
                    return context.Repeat();
                        
                })
                .Exit(()=>{
                    states.Add("MainMenu:end");
                })
                .State("Settings")
                .Enter(async context => {
                    states.Add("Settings:start");
                })
                .Execute(async context => {
                    states.Add("Settings");
                    return context.Repeat();
                        
                })
                .Exit(async () =>{
                    states.Add("Settings:end");
                })
                .State("Audio")
                .Enter(context => {
                    states.Add("Audio:start");
                })
                .Execute(context => {
                    states.Add("Audio");
                    return context.Repeat();
                            
                })
                .Exit(()=>{
                    states.Add("Audio:end");
                })
                .End()
                .State("Video")
                .Enter(context => {
                    states.Add("Video:start");
                })
                .Execute(context => {
                    states.Add("Video");
                    return context.Repeat();
                })
                .Exit(()=>{
                    states.Add("Video:end");
                })
                .End()
                .End()
                .End()
                .Build();

            sm.SetNextState("Debug");
            await sm.Execute(0f);
            // Console.WriteLine(String.Join(", ", states));
            sm.SetNextState("MainMenu");
            await sm.Execute(0f);
            // Console.WriteLine(String.Join(", ", states));
            sm.SetNextState("Settings");
            await sm.Execute(0f);
            // Console.WriteLine(String.Join(", ", states));
            sm.SetNextState("Audio");
            await sm.Execute(0f);
            // Console.WriteLine(String.Join(", ", states));
            sm.SetNextState("Video");
            await sm.Execute(0f);
            // Console.WriteLine(String.Join(",", states));
            sm.PopNextState();
            await sm.Execute(0f);
            // Console.WriteLine(String.Join(",", states));
            sm.PopNextState();
            await sm.Execute(0f);
            // Console.WriteLine(String.Join(",", states));
            
            Assert.That(string.Join(",", states), Is.EqualTo(
                "Debug:start,Debug,Debug:end," +
                "MainMenu:start,MainMenu," +
                    "Settings:start," +
                    "Settings," +
                        "Audio:start,Audio,Audio:end," +
                        "Video:start,Video,Video:end," +
                    "Settings,Settings:end," +
                "MainMenu"));
        }

        [Test]
        public async Task AsyncStateMachineNodeTest() {
            var builder = new StateMachineNode("X", StateMachineNode.ProcessMode.Idle).CreateBuilder();

            var x = 0;
            List<string> states = new List<string>();

            builder.State("Start")
                .Execute(async (context) => {
                    Assert.That(context.CurrentState.Name, Is.EqualTo("Start"));
                    Assert.That(context.FromState.Name, Is.EqualTo("Start"));
                    return context.Immediate("Idle");
                });

            builder.State("Idle")
                .Enter(async (context) => { x = 0; })
                .Execute(async (context) => {
                    Assert.That(context.CurrentState.Name, Is.EqualTo("Idle"));
                    x++;
                    states.Add("IdleExecute(" + x + ")");
                    if (x == 2) {
                        return context.Immediate("Attack");
                    }

                    return context.Immediate("Idle");
                });

            builder.State("Attack")
                .Execute(async (context) => {
                    Assert.That(context.CurrentState.Name, Is.EqualTo("Attack"));
                    Assert.That(context.FromState.Name, Is.EqualTo("Idle"));
                    x++;
                    states.Add("AttackExecute(" + x + ")");
                    return context.NextFrame("Idle");
                });

            var stateMachine = builder.Build();
            AddChild(stateMachine);

            stateMachine.SetNextState("Start");
            stateMachine.BeforeExecute(f => states.Add("BeforeExecute"));
            stateMachine.AfterExecute(f => states.Add("AfterExecute"));

            states.Clear();
            await this.AwaitIdleFrame();
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states),
                Is.EqualTo("BeforeExecute,IdleExecute(1),IdleExecute(2),AttackExecute(3),AfterExecute"));

            states.Clear();
            await this.AwaitIdleFrame();
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states),
                Is.EqualTo("BeforeExecute,IdleExecute(1),IdleExecute(2),AttackExecute(3),AfterExecute"));
        }
        
        [Test]
        public async Task AsyncStateMachineNodeWithIdleFrame() {
            var builder = new StateMachineNode("X", StateMachineNode.ProcessMode.Idle).CreateBuilder();

            var x = 0;
            List<string> states = new List<string>();

            builder.State("Start")
                .Execute(async context => {
                    Assert.That(context.CurrentState.Name, Is.EqualTo("Start"));
                    Assert.That(context.FromState.Name, Is.EqualTo("Start"));
                    await this.AwaitIdleFrame();
                    return context.Immediate("Idle");
                });

            builder.State("Idle")
                .Enter(async context => {
                    await this.AwaitIdleFrame();
                    x = 0;
                })
                .Execute(async context => {
                    Assert.That(context.CurrentState.Name, Is.EqualTo("Idle"));
                    await this.AwaitIdleFrame();
                    x++;
                    await this.AwaitIdleFrame();
                    states.Add("IdleExecute(" + x + ")");
                    if (x == 2) {
                        return context.Immediate("Attack");
                    }

                    return context.Immediate("Idle");
                });

            builder.State("Attack")
                .Execute(async (context) => {
                    Assert.That(context.CurrentState.Name, Is.EqualTo("Attack"));
                    Assert.That(context.FromState.Name, Is.EqualTo("Idle"));
                    x++;
                    states.Add("AttackExecute(" + x + ")");
                    return context.NextFrame("End");
                });

            builder.State("End")
                .Execute(async context => {
                    Assert.That(context.FromState.Name, Is.EqualTo("Attack"));
                    x++;
                    states.Add("End(" + x + ")");
                    return context.Repeat();
                });

            var stateMachine = builder.Build();
            stateMachine.SetNextState("Start");
            AddChild(stateMachine);
            
            stateMachine.BeforeExecute(async (f) => {
                states.Add("BeforeExecute");
            });
            stateMachine.AfterExecute(async (f)  => {
                states.Add("AfterExecute");
            });

            Stopwatch stopwatch = Stopwatch.StartNew();
            while (stateMachine.CurrentState?.Name != "End" && stopwatch.ElapsedMilliseconds < 1000) {
                await this.AwaitIdleFrame();
                
            }
            Assert.That(string.Join(",", states),
                Is.EqualTo("BeforeExecute,IdleExecute(1),IdleExecute(2),AttackExecute(3),AfterExecute,BeforeExecute,End(4),AfterExecute"));
         
        }
    }
}