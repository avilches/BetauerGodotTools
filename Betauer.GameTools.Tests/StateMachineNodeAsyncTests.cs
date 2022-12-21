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
    public partial class StateMachineNodeAsyncTests : Node {
        enum State {
            A,
            Idle,
            Start,
            Attack,
            End,
            NotFound,
            MainMenu,
            Debug,
            Settings
        }
        enum Trans {
            NotFound
        }
        [Test(Description = "Constructor")]
        public void StateMachineNodeConstructors() {
            var sm1 = new StateMachineNodeAsync<State, Trans>(State.A, "X");
            Assert.That(sm1.StateMachine.Name, Is.EqualTo("X"));
            Assert.That(sm1.ProcessInPhysics, Is.False);

            var sm2 = new StateMachineNodeAsync<State, Trans>(State.A, null, true);
            Assert.That(sm2.StateMachine.Name, Is.Null);
            Assert.That(sm2.ProcessInPhysics, Is.True);

            var sm3 = new StateMachineNodeAsync<State, Trans>(State.A);
            Assert.That(sm3.StateMachine.Name, Is.Null);
            Assert.That(sm3.ProcessInPhysics, Is.False);
        }

        [Test(Description = "StateMachineNode, BeforeExecute and AfterExecute events with idle frames in the execute")]
        public async Task AsyncStateMachineNodeWithIdleFrame() {
            var sm = new StateMachineNodeAsync<State, Trans>(State.Start);

            var x = 0;
            List<string> states = new List<string>();

            sm.State(State.Start)
                .Execute(async () => states.Add("Start"))
                .If(() => true).Then(context => context.Set(State.Idle))
                .Build();

            sm.State(State.Idle)
                .Enter(async () => {
                    
                    x = 0;
                })
                .Execute(async () => {

                    x++;

                    states.Add("IdleExecute(" + x + ")");
                })
                .If(() => x == 2).Then(context => context.Set(State.Attack))
                .If(() => true).Then(context => context.Set(State.Idle))
                .Exit(async () => {
                    
                    states.Add("IdleExit(" + x + ")");
                }).Build();

            sm.State(State.Attack)
                .Execute(async () => {
                    x++;
                    states.Add("AttackExecute(" + x + ")");
                })
                .If(() => true).Then(context => context.Set(State.End))
                .Build();

            sm.State(State.End)
                .Execute(async () => {
                    x++;
                    states.Add("End(" + x + ")");
                }).Build();

            AddChild(sm);
            
            Stopwatch stopwatch = Stopwatch.StartNew();
            while (sm.CurrentState?.Key != State.End && stopwatch.ElapsedMilliseconds < 1000) {
                await this.AwaitProcessFrame();
                
            }
            Assert.That(string.Join(",", states),
                Is.EqualTo("Start,IdleExecute(1),IdleExecute(2),IdleExit(2),AttackExecute(3),End(4)"));
         
        }
        
        [Test]
        public async Task EnterOnPushExitOnPopSuspendAwakeListener() {
            var sm = new StateMachineNodeAsync<State, Trans>(State.MainMenu);

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
            sm.AddOnExecuteStart((double delta, State state)  => states.Add(state + ":process.start"));
            sm.AddOnExecuteEnd((State state)  => states.Add(state + ":process.end"));

            AddChild(sm);

            Stopwatch stopwatch = Stopwatch.StartNew();
            while (sm.CurrentState?.Key != State.End && stopwatch.ElapsedMilliseconds < 1000) {
                Console.Write(sm.CurrentState?.Key + " ");
                await this.AwaitProcessFrame();
            }

            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo(
                "MainMenu:process.start,MainMenu:enter,MainMenu:process.end," +
                "MainMenu:process.start,MainMenu:exit,from:MainMenu-to:Debug,Debug:enter,Debug:process.end," +
                "Debug:process.start,Debug:suspend,from:Debug-to:Settings,Settings:enter,Settings:process.end," +
                "Settings:process.start,Settings:exit,from:Settings-to:Debug,Debug:awake,Debug:process.end," +
                "Debug:process.start,Debug:exit,from:Debug-to:End,End:enter,End:process.end"));
        }
    }
}