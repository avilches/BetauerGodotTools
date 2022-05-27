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

            sm.SetState("Idle");

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

        [Test]
        public async Task WrongStates() {
            var sm = new StateMachine.StateMachine()
                .CreateBuilder()
                .State("A").End()
                .Build();

            Assert.ThrowsAsync<Exception>(async () => await sm.Execute(0f));
            Assert.Throws<Exception>((() => sm.PopState()));
            
            sm.PushState("A");
            await sm.Execute(0f);
            Assert.That(sm.State.Name, Is.EqualTo("A"));
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { "A" }));
            Assert.Throws<Exception>((() => sm.PopState()));
        }

        [Test(Description = "Changes using stateMachine change methods")]
        public async Task DeltaInExecute() {
            var builder = new StateMachine.StateMachine().CreateBuilder();
            const float expected1 = 10f;
            const float expected2 = 10f;
            builder.State("Debug1").Execute(delta => {
                Assert.That(delta, Is.EqualTo(expected1));
                return Transition.None();
            });
            builder.State("Debug2").Execute(async delta => {
                Assert.That(delta, Is.EqualTo(expected2));
                return Transition.None();
            });
            var sm = builder.Build();
            sm.SetState("Debug1");
            await sm.Execute(expected1);
            sm.SetState("Debug2");
            await sm.Execute(expected2);
        }

        [Test(Description = "Changes using stateMachine change methods")]
        public async Task NestedStates() {
            var builder = new StateMachine.StateMachine().CreateBuilder();

            builder.State("Debug");
            builder.State("MainMenu");
            builder.State("Settings");
            builder.State("Audio");
            builder.State("Video");
            var sm = builder.Build();

            // Wrong initial state
            Assert.ThrowsAsync<Exception>(async () => await sm.Execute(0f));

            Assert.Throws<Exception>((() => sm.PopState()));
            sm.SetState("Debug");
            await sm.Execute(0f);
            Assert.That(sm.State.Name, Is.EqualTo("Debug"));
            Assert.That(sm.GetStack(), Is.EqualTo(new [] {"Debug"}));

            // Wrong next states from Debug
            Assert.Throws<Exception>((() => sm.PopState()));

            // Debug to MainMenu (sibling)
            sm.SetState("MainMenu");
            await sm.Execute(0f);
            Assert.That(sm.State.Name, Is.EqualTo("MainMenu"));
            Assert.That(sm.GetStack(), Is.EqualTo(new [] {"MainMenu"}));

            // Wrong next states from MainMenu
            Assert.Throws<Exception>((() => sm.PopState()));

            // MainMenu to Settings (child)
            sm.PushState("Settings");
            await sm.Execute(0f);
            Assert.That(sm.State.Name, Is.EqualTo("Settings"));
            Assert.That(sm.GetStack(), Is.EqualTo(new [] {"MainMenu", "Settings"}));

            // Settings to Audio (child)
            sm.PushState("Audio");
            await sm.Execute(0f);
            Assert.That(sm.State.Name, Is.EqualTo("Audio"));
            Assert.That(sm.GetStack(), Is.EqualTo(new [] {"MainMenu", "Settings", "Audio"}));

            // Settings to Audio (child)
            sm.PopPushState("Video");
            await sm.Execute(0f);
            Assert.That(sm.State.Name, Is.EqualTo("Video"));
            Assert.That(sm.GetStack(), Is.EqualTo(new [] {"MainMenu", "Settings", "Video"}));

            // POP: Audio to Settings (parent)
            sm.PopState();
            await sm.Execute(0f);
            Assert.That(sm.State.Name, Is.EqualTo("Settings"));
            Assert.That(sm.GetStack(), Is.EqualTo(new [] {"MainMenu", "Settings"}));

            // POP: Settings to MainMenu (parent)
            sm.PopState();
            await sm.Execute(0f);
            Assert.That(sm.State.Name, Is.EqualTo("MainMenu"));
            Assert.That(sm.GetStack(), Is.EqualTo(new [] {"MainMenu"}));

            Assert.Throws<Exception>(() => sm.PopState());
            
            // MainMenu to Settings (child)
            sm.PushState("Settings");
            await sm.Execute(0f);
            Assert.That(sm.State.Name, Is.EqualTo("Settings"));
            Assert.That(sm.GetStack(), Is.EqualTo(new [] {"MainMenu", "Settings"}));

            // Settings to Audio (child)
            sm.PushState("Audio");
            await sm.Execute(0f);
            Assert.That(sm.State.Name, Is.EqualTo("Audio"));
            Assert.That(sm.GetStack(), Is.EqualTo(new [] {"MainMenu", "Settings", "Audio"}));
            
            // MainMenu to Settings (child)
            sm.SetState("Debug");
            await sm.Execute(0f);
            Assert.That(sm.State.Name, Is.EqualTo("Debug"));
            Assert.That(sm.GetStack(), Is.EqualTo(new [] {"Debug"}));
            
        }

        [Test]
        public async Task EnterOnPushExitOnPopSuspendAwakeEventsOrder() {
            var builder = new StateMachine.StateMachine().CreateBuilder();
            
            List<string> states = new List<string>();

            builder.State("Debug")
                .Awake(() => states.Add("Debug:awake"))
                .Enter(() => states.Add("Debug:start"))
                .Execute(() => {
                    states.Add("Debug");
                    return Transition.None();

                })
                .Suspend(() => states.Add("Debug:suspend"))
                .Exit(() => states.Add("Debug:end"));
            
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
            
            builder.State("Settings")
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
                .Awake(() => states.Add("Video:awake"))
                .Enter(() => states.Add("Video:start"))
                .Execute(() => {
                    states.Add("Video");
                    return Transition.None();
                })
                .Suspend(() => states.Add("Video:suspend"))
                .Exit(() => states.Add("Video:end"));

            var sm = builder.Build();
            sm.SetState("Debug");
            await sm.Execute(0f);
            Console.WriteLine(string.Join(",", states));
            sm.SetState("MainMenu");
            await sm.Execute(0f);
            Console.WriteLine(string.Join(",", states));
            sm.PushState("Settings");
            await sm.Execute(0f);
            Console.WriteLine(string.Join(",", states));
            sm.PushState("Audio");
            await sm.Execute(0f);
            Console.WriteLine(string.Join(",", states));
            sm.PopPushState("Video");
            await sm.Execute(0f);
            Console.WriteLine(string.Join(",", states));
            sm.PopState();
            await sm.Execute(0f);
            Console.WriteLine(string.Join(",", states));
            sm.PopState();
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
            
            
            states.Clear();
            sm.PushState("Settings");
            await sm.Execute(0f);
            sm.PushState("Audio");
            await sm.Execute(0f);
            sm.PushState("Video");
            await sm.Execute(0f);
            sm.SetState("Debug");
            await sm.Execute(0f);
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo(
                "MainMenu:suspend,Settings:start,Settings," +
                    "Settings:suspend,Audio:start,Audio," +
                        "Audio:suspend,Video:start,Video," +
                        "Video:end,Audio:end,Settings:end,MainMenu:end," +
                "Debug:start,Debug"));
            
        }

        [Test(Description = "StateMachineNode, BeforeExecute and AfterExecute events with idle frames in the execute")]
        public async Task AsyncStateMachineNodeWithIdleFrame() {
            var stateMachine = new StateMachineNode(StateMachineNode.ProcessMode.Idle);
            var builder = stateMachine.CreateBuilder();

            var x = 0;
            List<string> states = new List<string>();

            builder.State("Start")
                .Execute(async () => {
                    stateMachine.SetState("Attack"); // This change is ignored
                    states.Add("Start");
                    await this.AwaitIdleFrame();
                    return Transition.Set("Idle");
                });

            builder.State("Idle")
                .Enter(async () => {
                    stateMachine.SetState("Idle"); // This change is ignored
                    await this.AwaitIdleFrame();
                    x = 0;
                })
                .Execute(async () => {
                    await this.AwaitIdleFrame();
                    stateMachine.SetState("Idle"); // This change is ignored
                    x++;
                    await this.AwaitIdleFrame();
                    states.Add("IdleExecute(" + x + ")");
                    if (x == 2) {
                        return Transition.Set("Attack");
                    }

                    return Transition.Set("Idle");
                })
                .Exit(async () => {
                    stateMachine.SetState("Idle"); // This change is ignored
                    await this.AwaitIdleFrame();
                    states.Add("IdleExit(" + x + ")");
                });

            builder.State("Attack")
                .Execute(async () => {
                    x++;
                    stateMachine.SetState("Start"); // This change is ignored
                    states.Add("AttackExecute(" + x + ")");
                    return Transition.Set("End");
                });

            builder.State("End")
                .Execute(async () => {
                    x++;
                    states.Add("End(" + x + ")");
                    stateMachine.SetState("Start"); // This change is ignored
                    await this.AwaitIdleFrame();
                    return Transition.None();
                });

            builder.Build();
            stateMachine.SetState("Start");
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