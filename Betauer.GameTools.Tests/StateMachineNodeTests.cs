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
            NotFound,
            Settings,
            Debug,
            MainMenu
        }
        enum Trans {
            NotFound,
            Settings,
            Back
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
                .If(() => true).Then(context => context.Set(State.Idle))
                .Build();

            sm.State(State.Idle)
                .Enter(() => {
                    
                    x = 0;
                })
                .Execute(() => {

                    x++;

                    states.Add("IdleExecute(" + x + ")");
                })
                .If(() => x == 2).Then(context => context.Set(State.Attack))
                .If(() => true).Then(context => context.Set(State.Idle))
                .Exit(() => {
                    
                    states.Add("IdleExit(" + x + ")");
                }).Build();

            sm.State(State.Attack)
                .Execute(() => {
                    x++;
                    states.Add("AttackExecute(" + x + ")");
                })
                .If(() => true).Then(context => context.Set(State.End))
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
        
        [Test]
        public async Task EnterOnPushExitOnPopSuspendAwakeListener() {
            var sm = new StateMachineNodeSync<State, Trans>(State.MainMenu);

            sm.State(State.MainMenu)
                .If(() => true).Set(State.Debug)
                .Build();
            var awake = false;
            sm.State(State.Debug)
                .Awake(() => awake = true)
                .If(() => awake).Set(State.End)
                .If(() => true).Push(State.Settings)
                .Build();
            sm.State(State.Settings)
                .If(() => true).Pop()
                .Build();
            sm.State(State.End)
                .Build();
                
            List<string> states = new List<string>();
            sm.AddOnEnter((args) => states.Add(args.To + ":enter"));
            sm.AddOnAwake((args)  => states.Add(args.To + ":awake"));
            sm.AddOnSuspend((args)  => states.Add(args.From + ":suspend"));
            sm.AddOnExit((args)  => states.Add(args.From + ":exit"));
            sm.AddOnTransition((args)  => states.Add("from:" + args.From + "-to:" + args.To));
            sm.AddOnExecuteStart((float delta, State state)  => states.Add(state + ":execute.start"));
            sm.AddOnExecuteEnd((State state)  => states.Add(state + ":execute.end"));

            AddChild(sm);

            Stopwatch stopwatch = Stopwatch.StartNew();
            while (sm.CurrentState?.Key != State.End && stopwatch.ElapsedMilliseconds < 1000) {
                Console.Write(sm.CurrentState?.Key + " ");
                await this.AwaitIdleFrame();
            }

            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo(
                "MainMenu:execute.start,MainMenu:enter,MainMenu:execute.end," +
                "MainMenu:execute.start,MainMenu:exit,from:MainMenu-to:Debug,Debug:enter,Debug:execute.end," +
                "Debug:execute.start,Debug:suspend,from:Debug-to:Settings,Settings:enter,Settings:execute.end," +
                "Settings:execute.start,Settings:exit,from:Settings-to:Debug,Debug:awake,Debug:execute.end," +
                "Debug:execute.start,Debug:exit,from:Debug-to:End,End:enter,End:execute.end"));
        }
    }
}