using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Betauer.TestRunner;
using Betauer.StateMachine;
using Godot;
using NUnit.Framework;

namespace Betauer.Tests {
    [TestFixture]
    public class StateMachineTests : NodeTest {
        [Test]
        public void StateMachineFlow() {
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
                        return NextState.Immediate("Jump");
                    }

                    return context.Current();
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
                    return NextState.Immediate("Attack");
                })
                .Exit(() => { states.Add("JumpExit"); })
                .End()
                .State("Attack")
                // No enter because it's optional
                .Execute(context => {
                    Assert.That(context.CurrentState.Name, Is.EqualTo("Attack"));
                    states.Add("AttackExecute");
                    return NextState.NextFrame("Idle");
                })
                .Exit(() => { states.Add("AttackExit"); });

            var stateMachine = builder.Build();

            stateMachine.SetNextState("Idle");

            states.Clear();
            stateMachine.Execute(100f);
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("IdleEnter,IdleExecute(1)"));

            states.Clear();
            stateMachine.Execute(100f);
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states),
                Is.EqualTo("IdleExecute(2),JumpEnter,JumpExecute(2),JumpExit,AttackExecute"));

            states.Clear();
            stateMachine.Execute(100f);
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("AttackExit,IdleEnter,IdleExecute(1)"));

            states.Clear();
            stateMachine.Execute(100f);
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states),
                Is.EqualTo("IdleExecute(2),JumpEnter,JumpExecute(2),JumpExit,AttackExecute"));
        }

        [Test]
        public void NestedStates() {
            var builder = new StateMachine.StateMachine(this, "X").CreateBuilder();

            List<string> states = new List<string>();

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
            Assert.Throws<Exception>(() => sm.Execute(0f));

            Assert.Throws<Exception>((() => sm.PopNextState()));
            Assert.Throws<Exception>((() => sm.SetNextState("Settings")));
            Assert.Throws<Exception>((() => sm.SetNextState("Audio")));
            sm.SetNextState("Debug");
            sm.Execute(0f);
            Assert.That(sm.CurrentState.Name, Is.EqualTo("Debug"));

            // Wrong next states from Debug
            Assert.Throws<Exception>((() => sm.SetNextState("Settings")));
            Assert.Throws<Exception>((() => sm.SetNextState("Audio")));
            Assert.Throws<Exception>((() => sm.SetNextState("Video")));

            // Debug to MainMenu (sibling)
            sm.SetNextState("MainMenu");
            sm.Execute(0f);
            Assert.That(sm.CurrentState.Name, Is.EqualTo("MainMenu"));

            // Wrong next states from MainMenu
            Assert.Throws<Exception>((() => sm.SetNextState("Audio")));
            Assert.Throws<Exception>((() => sm.SetNextState("Video")));

            // MainMenu to Settings (child)
            sm.SetNextState("Settings");
            sm.Execute(0f);
            Assert.That(sm.CurrentState.Name, Is.EqualTo("Settings"));
            Assert.Throws<Exception>((() => sm.SetNextState("Debug")));

            // Settings to Audio (child)
            sm.SetNextState("Audio");
            sm.Execute(0f);
            Assert.That(sm.CurrentState.Name, Is.EqualTo("Audio"));
            Assert.Throws<Exception>((() => sm.SetNextState("MainMenu")));
            Assert.Throws<Exception>((() => sm.SetNextState("Debug")));

            // POP: Audio to Settings (parent)
            sm.SetNextState("Settings");
            sm.Execute(0f);
            Assert.That(sm.CurrentState.Name, Is.EqualTo("Settings"));

            // POP: Settings to MainMenu (parent)
            sm.PopNextState();
            sm.Execute(0f);
            Assert.That(sm.CurrentState.Name, Is.EqualTo("MainMenu"));
            Assert.Throws<Exception>(() => sm.PopNextState());

            
        }
        [Test]
        public void NestedStatesPushPopFlow() {
            var builder = new StateMachine.StateMachine(this, "X").CreateBuilder();

            List<string> states = new List<string>();

            var sm = builder
                .State("Debug")
                .Enter(context => {
                    states.Add("MM:start");
                })
                .Execute(context => {
                    states.Add("MM");
                    return NextState.NextFrame("MainMenu");
                        
                })
                .Exit(() => {
                    states.Add("MM:end");
                })
                .End()
                .State("MainMenu")
                .Enter(context => {
                    states.Add("MM:start");
                })
                .Execute(context => {
                    states.Add("MM");
                    return context.Current();
                        
                })
                .Exit(()=>{
                    states.Add("MM:end");
                })
                .State("Settings")
                .Enter(context => {
                    states.Add("Settings:start");
                })
                .Execute(context => {
                    states.Add("Settings");
                    return context.Current();
                        
                })
                .Exit(()=>{
                    states.Add("Settings:end");
                })
                .State("Audio")
                .Enter(context => {
                    states.Add("Audio:start");
                })
                .Execute(context => {
                    states.Add("Audio");
                    return context.Current();
                            
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
                    return context.Current();
                            
                })
                .Exit(()=>{
                    states.Add("Video:end");
                })
                .End()
                .End()
                .End()
                .Build();

            Assert.That(sm.FindState("Debug").Parent, Is.Null);
            Assert.That(sm.FindState("MainMenu").Parent, Is.Null);
            Assert.That(sm.FindState("Settings").Parent, Is.EqualTo(sm.FindState("MainMenu")));
            Assert.That(sm.FindState("Audio").Parent, Is.EqualTo(sm.FindState("Settings")));
            Assert.That(sm.FindState("Video").Parent, Is.EqualTo(sm.FindState("Settings")));

            // Wrong initial state
            Assert.Throws<Exception>(() => sm.Execute(0f));

            Assert.Throws<Exception>((() => sm.PopNextState()));
            Assert.Throws<Exception>((() => sm.SetNextState("Settings")));
            Assert.Throws<Exception>((() => sm.SetNextState("Audio")));
            sm.SetNextState("Debug");
            sm.Execute(0f);
            Assert.That(sm.CurrentState.Name, Is.EqualTo("Debug"));

            // Wrong next states from Debug
            Assert.Throws<Exception>((() => sm.SetNextState("Settings")));
            Assert.Throws<Exception>((() => sm.SetNextState("Audio")));
            Assert.Throws<Exception>((() => sm.SetNextState("Video")));

            // Debug to MainMenu (sibling)
            sm.SetNextState("MainMenu");
            sm.Execute(0f);
            Assert.That(sm.CurrentState.Name, Is.EqualTo("MainMenu"));

            // Wrong next states from MainMenu
            Assert.Throws<Exception>((() => sm.SetNextState("Audio")));
            Assert.Throws<Exception>((() => sm.SetNextState("Video")));

            // MainMenu to Settings (child)
            sm.SetNextState("Settings");
            sm.Execute(0f);
            Assert.That(sm.CurrentState.Name, Is.EqualTo("Settings"));
            Assert.Throws<Exception>((() => sm.SetNextState("Debug")));

            // Settings to Audio (child)
            sm.SetNextState("Audio");
            sm.Execute(0f);
            Assert.That(sm.CurrentState.Name, Is.EqualTo("Audio"));
            Assert.Throws<Exception>((() => sm.SetNextState("MainMenu")));
            Assert.Throws<Exception>((() => sm.SetNextState("Debug")));

            // POP: Audio to Settings (parent)
            sm.SetNextState("Settings");
            sm.Execute(0f);
            Assert.That(sm.CurrentState.Name, Is.EqualTo("Settings"));

            // POP: Settings to MainMenu (parent)
            sm.PopNextState();
            sm.Execute(0f);
            Assert.That(sm.CurrentState.Name, Is.EqualTo("MainMenu"));
            Assert.Throws<Exception>(() => sm.PopNextState());
        }

        [Test]
        public async Task StateMachineNodeTest() {
            var builder = new StateMachineNode("X", StateMachineNode.ProcessMode.Idle).CreateBuilder();

            var x = 0;
            List<string> states = new List<string>();

            builder.State("Start")
                .Execute(context => {
                    Assert.That(context.CurrentState.Name, Is.EqualTo("Start"));
                    Assert.That(context.FromState.Name, Is.EqualTo("Start"));
                    return NextState.Immediate("Idle");
                });

            builder.State("Idle")
                .Enter(context => { x = 0; })
                .Execute(context => {
                    Assert.That(context.CurrentState.Name, Is.EqualTo("Idle"));
                    x++;
                    states.Add("IdleExecute(" + x + ")");
                    if (x == 2) {
                        return NextState.Immediate("Attack");
                    }

                    return NextState.Immediate("Idle");
                });

            builder.State("Attack")
                .Execute(context => {
                    Assert.That(context.CurrentState.Name, Is.EqualTo("Attack"));
                    Assert.That(context.FromState.Name, Is.EqualTo("Idle"));
                    x++;
                    states.Add("AttackExecute(" + x + ")");
                    return NextState.NextFrame("Idle");
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
    }
}