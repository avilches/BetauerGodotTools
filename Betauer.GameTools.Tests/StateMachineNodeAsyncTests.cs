using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Betauer.StateMachine;
using Betauer.StateMachine.Async;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.GameTools.Tests {
    [TestFixture]
    public class StateMachineNodeAsyncTests : Node {
        enum State {
            A,
            Idle,
            Start,
            Attack,
            End,
            NotFound
        }
        enum Trans {
            NotFound
        }
        [Test(Description = "Constructor")]
        public void StateMachineNodeConstructors() {
            var sm1 = new StateMachineNodeAsync<State, Trans>(State.A, "X");
            Assert.That(sm1.StateMachine.Name, Is.EqualTo("X"));
            Assert.That(sm1.Mode, Is.EqualTo(ProcessMode.Idle));

            var sm2 = new StateMachineNodeAsync<State, Trans>(State.A, null, ProcessMode.Physics);
            Assert.That(sm2.StateMachine.Name, Is.Null);
            Assert.That(sm2.Mode, Is.EqualTo(ProcessMode.Physics));

            var sm3 = new StateMachineNodeAsync<State, Trans>(State.A);
            Assert.That(sm3.StateMachine.Name, Is.Null);
            Assert.That(sm3.Mode, Is.EqualTo(ProcessMode.Idle));
        }

        [Test(Description = "StateMachineNode, BeforeExecute and AfterExecute events with idle frames in the execute")]
        public async Task AsyncStateMachineNodeWithIdleFrame() {
            var sm = new StateMachineNodeAsync<State, Trans>(State.Start, null, ProcessMode.Idle);

            var x = 0;
            List<string> states = new List<string>();

            sm.State(State.Start)
                .Execute(async context => {
                    states.Add("Start");
                    await this.AwaitIdleFrame();
                    return context.Set(State.Idle);
                }).Build();

            sm.State(State.Idle)
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
                }).Build();

            sm.State(State.Attack)
                .Execute(async context => {
                    x++;
                    states.Add("AttackExecute(" + x + ")");
                    return context.Set(State.End);
                }).Build();

            sm.State(State.End)
                .Execute(async context => {
                    x++;
                    states.Add("End(" + x + ")");
                    await this.AwaitIdleFrame();
                    return context.None();
                }).Build();

            AddChild(sm);
            
            Stopwatch stopwatch = Stopwatch.StartNew();
            while (sm.CurrentState?.Key != State.End && stopwatch.ElapsedMilliseconds < 1000) {
                await this.AwaitIdleFrame();
                
            }
            Assert.That(string.Join(",", states),
                Is.EqualTo("Start,IdleExecute(1),IdleExecute(2),IdleExit(2),AttackExecute(3),End(4)"));
         
        }
    }
}