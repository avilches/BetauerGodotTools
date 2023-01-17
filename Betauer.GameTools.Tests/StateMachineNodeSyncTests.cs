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
    public partial class StateMachineNodeSyncTests : Node {
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
            Assert.That(sm1.GetStateMachineEvents().Name, Is.EqualTo("X"));
            Assert.That(sm1.ProcessInPhysics, Is.False);

            var sm2 = new StateMachineNodeSync<State, Trans>(State.A, null, true);
            Assert.That(sm2.GetStateMachineEvents().Name, Is.Null);
            Assert.That(sm2.ProcessInPhysics, Is.True);

            var sm3 = new StateMachineNodeSync<State, Trans>(State.A);
            Assert.That(sm3.GetStateMachineEvents().Name, Is.Null);
            Assert.That(sm3.ProcessInPhysics, Is.False);
        }

        [Test(Description = "StateMachineNode, BeforeExecute and AfterExecute events with idle frames in the execute")]
        public async Task AsyncStateMachineNodeWithIdleFrame() {
            var sm = new StateMachineNodeSync<State, Trans>(State.Start);

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
                await this.AwaitProcessFrame();
                
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
            sm.OnEnter += (args) => states.Add(args.To + ":enter");
            sm.OnAwake += (args)  => states.Add(args.To + ":awake");
            sm.OnSuspend += (args)  => states.Add(args.From + ":suspend");
            sm.OnExit += (args)  => states.Add(args.From + ":exit");
            sm.OnTransition += (args)  => states.Add("from:" + args.From + "-to:" + args.To);
            sm.OnBeforeExecute += ()  => states.Add(":execute.start");
            sm.OnAfterExecute += ()  => states.Add(":execute.end");

            AddChild(sm);

            Stopwatch stopwatch = Stopwatch.StartNew();
            while (sm.CurrentState?.Key != State.End && stopwatch.ElapsedMilliseconds < 1000) {
                Console.WriteLine(sm.CurrentState?.Key + "...");
                await this.AwaitProcessFrame();
            }

            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo(
                "MainMenu:enter,:execute.start,:execute.end,MainMenu:exit," +
                "from:MainMenu-to:Debug," +
                "Debug:enter,:execute.start,:execute.end,Debug:suspend," +
                "from:Debug-to:Settings," +
                "Settings:enter,:execute.start,:execute.end,Settings:exit," +
                "from:Settings-to:Debug," +
                "Debug:awake,:execute.start,:execute.end,Debug:exit," +
                "from:Debug-to:End," +
                "End:enter,:execute.start,:execute.end"));
        }
    }
}