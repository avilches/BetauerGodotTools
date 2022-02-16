using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Betauer.Application;
using Betauer.Memory;
using Betauer.TestRunner;
using Betauer.StateMachine;
using Godot;
using NUnit.Framework;

namespace Betauer.Tests {
    [TestFixture]
    public class StateMachineTests : NodeTest {
        [Test]
        [Only]
        public void Basic() {
            var builder = new StateMachine.StateMachine(this, "X").CreateBuilder();

            var x = 0;
            List<string> states = new List<string>();

            builder.CreateState("Idle")
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
                        return Context.Immediate("Jump");
                    }

                    return context.Current();
                });

            builder.CreateState("Jump")
                .Enter(context => {
                    Assert.That(context.CurrentState.Name, Is.EqualTo("Jump"));
                    Assert.That(context.FromState.Name, Is.EqualTo("Idle"));
                    states.Add("JumpEnter");
                })
                .Execute(context => {
                    Assert.That(context.CurrentState.Name, Is.EqualTo("Jump"));
                    Assert.That(context.FromState.Name, Is.EqualTo("Idle"));
                    states.Add("JumpExecute(" + x + ")");
                    return Context.Immediate("Attack");
                })
                .Exit(() => { states.Add("JumpExit"); });

            builder.CreateState("Attack")
                // No enter because it's optional
                .Execute(context => {
                    Assert.That(context.CurrentState.Name, Is.EqualTo("Attack"));
                    states.Add("AttackExecute");
                    return Context.NextFrame("Idle");
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
            Assert.That(string.Join(",", states), Is.EqualTo("IdleExecute(2),JumpEnter,JumpExecute(2),JumpExit,AttackExecute"));

            states.Clear();
            stateMachine.Execute(100f);
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("AttackExit,IdleEnter,IdleExecute(1)"));

            states.Clear();
            stateMachine.Execute(100f);
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("IdleExecute(2),JumpEnter,JumpExecute(2),JumpExit,AttackExecute"));

        }

        [Test]
        [Only]
        public async Task Node() {
            var builder = new StateMachineNode("X", StateMachineNode.ProcessMode.Idle).CreateBuilder();

            var x = 0;
            List<string> states = new List<string>();

            builder.CreateState("Start")
                .Execute(context => {
                    Assert.That(context.CurrentState.Name, Is.EqualTo("Start"));
                    Assert.That(context.FromState.Name, Is.EqualTo("Start"));
                    return Context.Immediate("Idle");
                });

            builder.CreateState("Idle")
                .Enter(context => { x = 0; })
                .Execute(context => {
                    Assert.That(context.CurrentState.Name, Is.EqualTo("Idle"));
                    x++;
                    states.Add("IdleExecute(" + x + ")");
                    if (x == 2) {
                        return Context.Immediate("Attack");
                    }

                    return Context.Immediate("Idle");
                });

            builder.CreateState("Attack")
                .Execute(context => {
                    Assert.That(context.CurrentState.Name, Is.EqualTo("Attack"));
                    Assert.That(context.FromState.Name, Is.EqualTo("Idle"));
                    x++;
                    states.Add("AttackExecute(" + x + ")");
                    return Context.NextFrame("Idle");
                });

            var stateMachine = builder.Build();
            AddChild(stateMachine);

            stateMachine.SetNextState("Start");
            stateMachine.BeforeExecute(f => states.Add("BeforeExecute"));
            stateMachine.AfterExecute(f => states.Add("AfterExecute"));

            states.Clear();
            await this.AwaitIdleFrame();
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("BeforeExecute,IdleExecute(1),IdleExecute(2),AttackExecute(3),AfterExecute"));

            states.Clear();
            await this.AwaitIdleFrame();
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("BeforeExecute,IdleExecute(1),IdleExecute(2),AttackExecute(3),AfterExecute"));
        }
    }
}