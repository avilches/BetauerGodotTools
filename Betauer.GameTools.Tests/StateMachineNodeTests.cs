using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Betauer.StateMachine;
using Betauer.StateMachine.Sync;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.GameTools.Tests {
    [TestFixture]
    public class StateMachineNodeTests : Node {
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
            var sm1 = new StateMachineNodeSync<State, Trans>(State.A, "X");
            Assert.That(sm1.StateMachine.Name, Is.EqualTo("X"));
            Assert.That(sm1.Mode, Is.EqualTo(ProcessMode.Idle));

            var sm2 = new StateMachineNodeSync<State, Trans>(State.A, null, ProcessMode.Physics);
            Assert.That(sm2.StateMachine.Name, Is.Null);
            Assert.That(sm2.Mode, Is.EqualTo(ProcessMode.Physics));

            var sm3 = new StateMachineNodeSync<State, Trans>(State.A);
            Assert.That(sm3.StateMachine.Name, Is.Null);
            Assert.That(sm3.Mode, Is.EqualTo(ProcessMode.Idle));
        }

        [Test(Description = "StateMachineNode, BeforeExecute and AfterExecute events with idle frames in the execute")]
        public async Task AsyncStateMachineNodeWithIdleFrame() {
            var sm = new StateMachineNodeSync<State, Trans>(State.Start, null, ProcessMode.Idle);

            var x = 0;
            List<string> states = new List<string>();

            sm.State(State.Start)
                .Execute(() => states.Add("Start"))
                .Condition(() => true, context => context.Set(State.Idle))
                .Build();

            sm.State(State.Idle)
                .Enter(() => {
                    
                    x = 0;
                })
                .Execute(() => {

                    x++;

                    states.Add("IdleExecute(" + x + ")");
                })
                .Condition(() => x == 2, context => context.Set(State.Attack))
                .Condition(() => true, context => context.Set(State.Idle))
                .Exit(() => {
                    
                    states.Add("IdleExit(" + x + ")");
                }).Build();

            sm.State(State.Attack)
                .Execute(() => {
                    x++;
                    states.Add("AttackExecute(" + x + ")");
                })
                .Condition(() => true, context => context.Set(State.End))
                .Build();

            sm.State(State.End)
                .Execute(() => {
                    x++;
                    states.Add("End(" + x + ")");
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