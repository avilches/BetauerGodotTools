using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Betauer.StateMachine.Sync;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;
using NullReferenceException = System.NullReferenceException;

namespace Betauer.StateMachine.Tests {
    [TestFixture]
    public class StateMachineSyncTests {
        enum State {
            A,
            Idle,
            Settings,
            MainMenu,
            Audio,
            Local,
            Global,
            Debug,
            Start,
            Debug1,
            Debug2,
            Video,
            Jump,
            Attack,
            End,
            NotFound
        }
        enum Event {
            Settings,
            MainMenu,
            Audio,
            Local,
            Global,
            End,
            Back,
            Debug,
            Start,
            Restart,
            Video,
            NotFound
        }
        [Test(Description = "Constructor")]
        public void StateMachineConstructorsEnum() {
            var sm1 = new StateMachineSync<State, Event>(State.A, "X");
            Assert.That(sm1.Name, Is.EqualTo("X"));

            var sm3 = new StateMachineSync<State, Event>(State.A);
            Assert.That(sm3.Name, Is.Null);
        }
            
        /*
         * Error cases
         */
        [Test(Description = "InitialState not found on start")]
        public void WrongStartStates() {
            var sm = new StateMachineSync<State, Event>(State.Global);
            sm.State(State.A).Build();

            // Start state Global not found
            Assert.Throws<KeyNotFoundException>( () => {
                sm.Execute();
            });
        }
        
        [Test(Description = "IsState")]
        public void IsStateTests() {
            var sm = new StateMachineSync<State, Event>(State.Global);
            sm.State(State.Global).Build();
            
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Global));
            Assert.That(sm.IsState(State.Global), Is.True);
            Assert.That(sm.IsState(State.Settings), Is.False);
        }
        
        [Test(Description = "Event are not allowed before initialize")]
        public void EventsAreNotAllowedBeforeInitialize() {
            var sm = new StateMachineSync<State, Event>(State.Audio);
            sm.On(Event.Audio).Stay();
            sm.State(State.Audio).Build();

            Assert.Throws<InvalidOperationException>(() => sm.Send(Event.Audio));
        }
        
        [Test(Description = "Error when a state changes to a not found state")]
        public void WrongStatesUnknownStateThenSet() {
            var sm = new StateMachineSync<State, Event>(State.A);
            var thenEvaluated = false;
            sm.State(State.A).If(() => true).Then(context => {
                thenEvaluated = true;
                return context.Set(State.Debug);
            }).Build();

            sm.Execute();
            Assert.That(thenEvaluated, Is.True);
            Assert.Throws<KeyNotFoundException>(() => sm.Execute());
        }

        [Test(Description = "Error when a state changes to a not found state")]
        public void WrongStatesUnknownStateSet() {
            var sm = new StateMachineSync<State, Event>(State.A);
            sm.State(State.A).If(() => true).Set(State.Debug).Build();

            sm.Execute();
            Assert.Throws<KeyNotFoundException>(() => sm.Execute());
        }

        [Test(Description = "Error not found event")]
        public void EventNotFound() {
            var sm = new StateMachineSync<State, Event>(State.A);
            sm.State(State.A).Build();

            sm.Execute();
            sm.Send(Event.NotFound);
            Assert.Throws<KeyNotFoundException>(() => sm.Execute());
        }

        [Test(Description = "Error when a state pop in an empty stack")]
        public void WrongStatesPopWhenEmptyStack() {
            var sm = new StateMachineSync<State, Event>(State.A);
            sm.State(State.A).If(() => true).Then(context => context.Pop()).Build();

            // State ends with a wrong state
            sm.Execute();
            Assert.Throws<InvalidOperationException>(() => sm.Execute());
        }
        
        [Test(Description = "Pop the same state in the stack is allowed")]
        public void PopSameStateInTheStackIsAllowed() {
            var sm = new StateMachineSync<State, Event>(State.A);
            sm.State(State.A).If(() => true).Then(context => context.Push(State.A)).Build();

            sm.Execute();
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A }));

            sm.Execute();
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A, State.A }));

            sm.Execute();
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A, State.A , State.A }));
        }
        
        [Test(Description = "Multiple if are loaded in order")]
        public void Ifs() {
            var sm = new StateMachineSync<State, Event>(State.A);
            var x = 0;
            sm.State(State.A)
                .If(() => {
                    x++;
                    return false;
                }).Set(State.Attack)
                .If(() => {
                    x++;
                    return false;
                }).Set(State.MainMenu)
                .If(() => {
                    x++;
                    return true;
                }).Set(State.Audio)
                .If(() => {
                    x++;
                    return false;
                }).Push(State.Debug).Build();
            sm.State(State.Attack).Build();
            sm.State(State.MainMenu).Build();
            sm.State(State.Audio).Build();
            sm.Execute();
            sm.Execute();
            Assert.That(x, Is.EqualTo(3));
        }

        /**
         * If with methods, instead lambda Then
         */
        [Test(Description = "If with Set result")]
        public void IfSetResult() {
            var sm = new StateMachineSync<State, Event>(State.A);
            sm.State(State.A).If(() => true).Set(State.Debug).Build();
            sm.State(State.Debug).Build();
            sm.Execute();
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A }));
            sm.Execute();
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.Debug }));
        }

        [Test(Description = "If with Push, PopPush and Pop result")]
        public void IfPushPopPushResult() {
            var sm = new StateMachineSync<State, Event>(State.A);
            sm.State(State.A).If(() => true).Push(State.Debug).Build();
            sm.State(State.Debug).If(() => true).PopPush(State.MainMenu).Build();
            sm.State(State.MainMenu).If(() => true).Pop().Build();
            sm.Execute();
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A }));
            sm.Execute();
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A, State.Debug }));
            sm.Execute();
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A, State.MainMenu }));
            sm.Execute();
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A }));
        }

        [Test(Description = "If with none result")]
        public void IfNoneResult() {
            var sm = new StateMachineSync<State, Event>(State.A);
            sm.State(State.A).If(() => true).Stay().Build();
            sm.Execute();
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A }));
            sm.Execute();
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A }));
        }

        /**
         * Transition with methods, instead of lambda Then
         */
        [Test(Description = "Transition with Set result")]
        public void TransitionSetResult() {
            var sm = new StateMachineSync<State, Event>(State.A);
            sm.On(Event.Debug).Set(State.Debug);
            sm.State(State.A).Build();
            sm.State(State.Debug).Build();
            sm.Execute();
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A }));
            sm.Send(Event.Debug);
            sm.Execute();
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.Debug }));
        }

        [Test(Description = "Transition with Push, PopPush and Pop result")]
        public void TransitionPushPopPushResult() {
            var sm = new StateMachineSync<State, Event>(State.A);
            sm.On(Event.Start).Push(State.Debug);
            sm.On(Event.MainMenu).PopPush(State.MainMenu);
            sm.On(Event.End).Pop();
            sm.State(State.A).Build();
            sm.State(State.Debug).Build();
            sm.State(State.MainMenu).Build();
            
            sm.Execute();
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A }));

            sm.Send(Event.Start);
            sm.Execute();
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A, State.Debug }));
            
            sm.Send(Event.MainMenu);
            sm.Execute();
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A, State.MainMenu }));

            sm.Send(Event.End);
            sm.Execute();
            Assert.That(sm.GetStack(), Is.EqualTo(new[] { State.A }));
        }

        [Test(Description = "Transition with none result")]
        // State transitions have more priority than global transitions. So, override a global with a local None will disable it
        public void TransitionNoneResult() {
            var sm = new StateMachineSync<State, Event>(State.Start);

            sm.On(Event.Global).Set(State.Global);
            sm.State(State.Start)
                .On(Event.Global).Stay().Build();
            
            sm.Execute();
            sm.Send(Event.Global);
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Start));
        }

        /*
         * Working StateMachine
         */
        [Test(Description = "Regular changes between root states inside state.Execute()")]
        public void StateMachinePlainFlow() {
            var sm = new StateMachineSync<State, Event>(State.Idle);

            var x = 0;
            List<string> states = new List<string>();

            sm.State(State.Idle)
                .Enter(() => {
                    x = 0;
                    states.Add("IdleEnter");
                })
                .Execute(() => {
                    x++;
                    states.Add("IdleExecute(" + x + ")");
                })
                .If(() => x == 2).Then(context => context.Set(State.Jump))
                .If(() => true).Then(context => context.Stay())
                .Build();

            sm.State(State.Jump)
                .Enter(() => {
                    states.Add("JumpEnter");
                })
                .Execute(() => states.Add("JumpExecute(" + x + ")"))
                .If(() => true).Then(context => context.Set(State.Attack))
                .Build();
                
            // No exit because it's optional
                
            sm.State(State.Attack)
                // No enter because it's optional
                .Execute(() => states.Add("AttackExecute"))
                .If(() => true).Then(context => context.Set(State.Idle))
                .Exit(() => { states.Add("AttackExit"); })
                .Build();
                
            states.Clear();
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Idle)); 
            
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("IdleEnter,IdleExecute(1)"));

            states.Clear();
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Idle)); 
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("IdleExecute(2)"));
            states.Clear();
            
            states.Clear();
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Jump)); 
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("JumpEnter,JumpExecute(2)"));

            states.Clear();
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Attack)); 
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("AttackExecute"));

            states.Clear();
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Idle)); 
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo("AttackExit,IdleEnter,IdleExecute(1)"));

        }

        [Test(Description = "Ignore events with lower weight")]
        public void SendEventWeight() {
            var sm = new StateMachineSync<State, Event>(State.Start);

            sm.On(Event.Global).Then(context => context.Set(State.Global));
            sm.On(Event.Local).Then(context => context.Set(State.Local));
            sm.On(Event.Start).Then(context => context.Set(State.Start));
            sm.State(State.Start).Build();
            sm.State(State.Global).Build();
            sm.State(State.Local).Build();

            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Start));
            
            sm.Send(Event.Start, 10);
            sm.Send(Event.Local, 9);
            sm.Send(Event.Global);
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Start));
            
            sm.Send(Event.Local);
            sm.Send(Event.Global);
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Global));
        }

        [Test(Description = "State events have more priority than global events")]
        public void StateEventsVsGlobalEvents() {
            var sm = new StateMachineSync<State, Event>(State.Start);

            sm.State(State.Start).On(Event.Local).Then(context => context.Push(State.Local)).Build();
            sm.On(Event.Global).Then(context => context.Set(State.Global));
            sm.On(Event.Local).Then(context => context.Set(State.Global));
            sm.State(State.Global).Build();
            sm.State(State.Local).Build();

            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Start));
            
            sm.Send(Event.Local);
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Local));
            
            sm.Send(Event.Global);
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Global));
            
            sm.Send(Event.Local);
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Global));

        }

        [Test(Description = "Event calls inside execute are ignored")]
        public void SendEventInsideExecuteIsIgnored() {
            var sm = new StateMachineSync<State, Event>(State.Start);

            sm.State(State.Start).On(Event.MainMenu).Set(State.MainMenu).Build();
            sm.Execute();

            sm.State(State.MainMenu).Execute(() => {
                sm.Send(Event.Settings); // this is overwritten by the If(() => true).Then()
            })
            .If(() => true).Then(context => context.Push(State.Audio))
            .Build();
            sm.State(State.Audio).Build();
            sm.State(State.Settings).Build();
                
            sm.On(Event.Settings).Then(context => context.Set(State.Settings));
            
            sm.Send(Event.MainMenu);
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.MainMenu));
            // The second execution has scheduled the 
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Audio));
        }

        [Test(Description = "Changes using stateMachine change methods")]
        public void SendEvent() {
            var sm = new StateMachineSync<State, Event>(State.Audio);

            sm.State(State.Debug).Build();
            sm.State(State.MainMenu).On(Event.Audio).Then(context => context.Push(State.Audio)).Build();
            sm.State(State.Settings).On(Event.Back).Then(context => context.Set(State.MainMenu)).Build();
            sm.State(State.Audio).On(Event.Back).Then(context => context.Pop()).Build();
            sm.On(Event.Restart).Then(context => context.Set(State.MainMenu));
            sm.On(Event.Settings).Then(context => context.Set(State.Settings));
            sm.On(Event.MainMenu).Then(context => context.Set(State.MainMenu));

            // Global event
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Audio));
            sm.Send(Event.Restart);
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.MainMenu));
            
            // State event
            sm.Send(Event.Settings);
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Settings));
            sm.Send(Event.Back);
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.MainMenu));

            // State event: pop
            sm.Send(Event.MainMenu);
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.MainMenu));
            sm.Send(Event.Audio);
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.Audio));
            Assert.That(sm.GetStack(), Is.EqualTo(new [] {State.MainMenu, State.Audio}));
            sm.Send(Event.Back);
            sm.Execute();
            Assert.That(sm.CurrentState.Key, Is.EqualTo(State.MainMenu));
        }


        [Test]
        public void EnterOnPushExitOnPopSuspendAwakeListener() {
            var sm = new StateMachineSync<State, Event>(State.Debug);

            sm.On(Event.Settings).Then(context => context.Push(State.Settings));
            sm.On(Event.Back).Then(context => context.Pop());
            sm.State(State.MainMenu).Build();
            sm.State(State.Debug).Build();
            sm.State(State.Settings).Build();
            List<string> states = new List<string>();
            sm.OnEnter += (args) => states.Add(args.To + ":enter");
            sm.OnAwake += (args)  => states.Add(args.To + ":awake");
            sm.OnSuspend += (args)  => states.Add(args.From + ":suspend");
            sm.OnExit += (args)  => states.Add(args.From + ":exit");
            sm.OnTransition += (args)  => states.Add("from:" + args.From + "-to:" + args.To);

            
            
            sm.Execute();
            sm.Send(Event.Settings); 
            sm.Execute();
            sm.Send(Event.Back);
            sm.Execute();
            // Test listener
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo(
                "Debug:enter," +
                
                "Debug:suspend," +
                "from:Debug-to:Settings," +
                "Settings:enter," +
                
                "Settings:exit," +
                "from:Settings-to:Debug," +
                "Debug:awake"));
            

        }

        [Test]
        public void EnterOnPushExitOnPopSuspendAwakeEventsOrder() {
            var sm = new StateMachineSync<State, Event>(State.Debug);
            
            List<string> states = new List<string>();

            sm.On(Event.Debug).Then(context => context.Set(State.Debug));
            sm.State(State.Debug)
                .Awake(() => states.Add("Debug:awake"))
                .Enter(() => states.Add("Debug:enter"))
                .Execute(() => states.Add("Debug"))
                .If(() => true).Then(context => context.Stay())
                .Suspend(() => states.Add("Debug:suspend"))
                .Exit(() => states.Add("Debug:exit"))
                .Build();

            sm.On(Event.MainMenu).Then(context => context.Set(State.MainMenu));
            sm.State(State.MainMenu)
                .Awake(() => states.Add("MainMenu:awake"))
                .Enter(() => states.Add("MainMenu:enter"))
                .Execute(() => states.Add("MainMenu"))
                .If(() => true).Then(context => context.Stay())
                .Suspend(() => states.Add("MainMenu:suspend"))
                .Exit(()=>{
                    states.Add("MainMenu:exit");
                })
                .Build();
                
            
            sm.On(Event.Settings).Then(context => context.Push(State.Settings));
            sm.State(State.Settings)
                .On(Event.Audio).Then(context => context.Push(State.Audio))
                .On(Event.Back).Then(context => context.Pop())
                .Awake(() => states.Add("Settings:awake"))
                .Enter(() => states.Add("Settings:enter"))
                .Execute(() => states.Add("Settings"))
                .If(() => true).Then(context => context.Stay())
                .Suspend(() => states.Add("Settings:suspend"))
                .Exit(() =>{
                    states.Add("Settings:exit");
                })
                .Build();
                
            
            sm.State(State.Audio)
                .On(Event.Video).Then(context => context.PopPush(State.Video))
                .On(Event.Back).Then(context => context.Pop())
                .Awake(() => states.Add("Audio:awake"))
                .Enter(() => states.Add("Audio:enter"))
                .Execute(() => states.Add("Audio"))
                .If(() => true).Then(context => context.Stay())
                .Suspend(() => states.Add("Audio:suspend"))
                .Exit(()=>{
                    states.Add("Audio:exit");
                })
                .Build();
                

            sm.State(State.Video)
                .On(Event.Back).Then(context => context.Pop())
                .Awake(() => states.Add("Video:awake"))
                .Enter(() => states.Add("Video:enter"))
                .Execute(() => states.Add("Video"))
                .If(() => true).Then(context => context.Stay())
                .Suspend(() => states.Add("Video:suspend"))
                .Exit(() => states.Add("Video:exit"))
                .Build();
            
            sm.Execute();
            Console.WriteLine(string.Join(",", states));
            sm.Send(Event.MainMenu);
            sm.Execute();
            Console.WriteLine(string.Join(",", states));
            sm.Send(Event.Settings);
            sm.Execute();
            Console.WriteLine(string.Join(",", states));
            sm.Send(Event.Audio);
            sm.Execute();
            Console.WriteLine(string.Join(",", states));
            sm.Send(Event.Video);
            sm.Execute();
            Console.WriteLine(string.Join(",", states));
            sm.Send(Event.Back);
            sm.Execute();
            Console.WriteLine(string.Join(",", states));
            sm.Send(Event.Back);
            sm.Execute();
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo(
                "Debug:enter,Debug,Debug:exit," +
                "MainMenu:enter,MainMenu,MainMenu:suspend," +
                    "Settings:enter,Settings,Settings:suspend," +
                        "Audio:enter,Audio,Audio:exit," +
                        "Video:enter,Video,Video:exit," +
                    "Settings:awake,Settings,Settings:exit," +
                "MainMenu:awake,MainMenu"));
            
            // Test multiple exits when more than one state is in the stack and change is Replace instead of Pop
            states.Clear();
            sm.Send(Event.Settings);
            sm.Execute();
            Console.WriteLine(string.Join(",", states));
            sm.Send(Event.Audio);
            sm.Execute();
            Console.WriteLine(string.Join(",", states));
            sm.Send(Event.Debug);
            sm.Execute();
            Console.WriteLine(string.Join(",", states));
            Assert.That(string.Join(",", states), Is.EqualTo(
                "MainMenu:suspend," +
                    "Settings:enter,Settings,Settings:suspend," +
                        "Audio:enter,Audio," +
                        "Audio:exit,Settings:exit,MainMenu:exit," + // This is the important part: three end in a row 
                "Debug:enter,Debug"));


        }

        [Test]
        public void ErrorInEnter() {
            var sm = new StateMachineSync<State, Event>(State.Start);

            var steps = 0;
            sm.State(State.Start)
                .Enter(() => {
                    steps++;
                    throw new NullReferenceException();
                })
                .Execute(() => steps++)
                .If(() => true).Then(ctx => ctx.Set(State.End))
                .Build();

            Assert.Throws<NullReferenceException>(() => sm.Execute());
            Assert.That(steps, Is.EqualTo(1));
        }
        
        [Test]
        public void ErrorInExecute() {
            var sm = new StateMachineSync<State, Event>(State.Start);

            var steps = 0;
            sm.State(State.Start)
                .Enter(() => {
                    steps++;
                })
                .Execute(() => {
                    steps++;
                    throw new NullReferenceException();
                })
                .If(() => true).Then(ctx => ctx.Set(State.End))
                .Build();

            Assert.Throws<NullReferenceException>(() => sm.Execute());
            Assert.That(steps, Is.EqualTo(2));
        }
        
        [Test]
        public void ErrorInExit() {
            var sm = new StateMachineSync<State, Event>(State.Start);

            var steps = 0;
            sm.State(State.Start)
                .Enter(() => {
                    steps++;
                })
                .Execute(() => {
                    steps++;
                })
                .Exit(() => {
                    steps++;
                    throw new NullReferenceException();
                })
                .If(() => true).Then(ctx => ctx.Set(State.End))
                .Build();
            sm.State(State.End).Build(); 

            sm.Execute();
            Assert.Throws<NullReferenceException>(() => sm.Execute());
            Assert.That(steps, Is.EqualTo(3));
        }

        [Test(Description = "State never changes")]
        public void ConditionalOrderStateNeverChanges() {
            var sm = new StateMachineSync<State, Event>(State.Start);

            const bool NeverChanges = false;
            var steps = new List<string>();
            sm.State(State.Start)
                .Execute(() => {
                    steps.Add("Start.Execute");
                })
                .If(() => {
                    steps.Add("Start.If.Asap");
                    return NeverChanges;
                }, Condition.Type.Asap)
                .Set(State.MainMenu)
                .If(() => {
                    steps.Add("Start.If.Lazy");
                    return NeverChanges;
                }, Condition.Type.Lazy)
                .Set(State.MainMenu)
                .If(() => {
                    steps.Add("Start.If.Always");
                    return NeverChanges;
                })
                .Set(State.MainMenu)
                .Build();

            steps.Clear();
            sm.Execute();
            // Start is a new state, so no conditions will be evaluated before execution, and all conditions are evaluated after execution
            Assert.That(string.Join(",", steps), Is.EqualTo("Start.Execute," +
                                                            "Start.If.Asap,Start.If.Lazy,Start.If.Always"));

            steps.Clear();
            sm.Execute();
            // State didn't change, so only asap and always conditions will be evaluated before execution.
            // State didn't change neither, so execute the state
            // And all conditions are evaluated after execution
            Assert.That(string.Join(",", steps), Is.EqualTo("Start.If.Asap,Start.If.Always," +
                                                            "Start.Execute," +
                                                            "Start.If.Lazy,Start.If.Always"));

            steps.Clear();
            sm.Execute();
            // State didn't change, same as step before
            Assert.That(string.Join(",", steps), Is.EqualTo("Start.If.Asap,Start.If.Always," +
                                                            "Start.Execute," +
                                                            "Start.If.Lazy,Start.If.Always"));


        }
        
        [Test(Description = "State change at the end of the state")]
        public void ConditionalOrderStateChangesAtTheBeginning() {
            var sm = new StateMachineSync<State, Event>(State.Start);

            var steps = new List<string>();
            sm.State(State.Start)
                .Execute(() => {
                    steps.Add("Start.Execute");
                })
                .If(() => {
                    steps.Add("Start.If.Asap");
                    return false;
                }, Condition.Type.Asap)
                .Set(State.MainMenu)
                .If(() => {
                    steps.Add("Start.If.Lazy");
                    return false;
                }, Condition.Type.Lazy)
                .Set(State.MainMenu)
                .If(() => {
                    steps.Add("Start.If.Always");
                    return true;
                })
                .Set(State.MainMenu)
                .Build();
            
            sm.State(State.MainMenu)
                .Execute(() => {
                    steps.Add("Main.Execute");
                })
                .If(() => {
                    steps.Add("Main.If.Asap");
                    return false;
                }, Condition.Type.Asap)
                .Set(State.Start)
                .If(() => {
                    steps.Add("Main.If.Lazy");
                    return false;
                }, Condition.Type.Lazy)
                .Set(State.Start)
                .If(() => {
                    steps.Add("Main.If.Always");
                    return true;
                })
                .Set(State.Start)
                .Build();
            
            steps.Clear();
            sm.Execute();
            // Start is a new state, so no conditions will be evaluated before execution, and all conditions are evaluated after execution
            Assert.That(string.Join(",", steps), Is.EqualTo("Start.Execute," +
                                                            "Start.If.Asap,Start.If.Lazy,Start.If.Always"));
            
            steps.Clear();
            sm.Execute();
            // Main is a new state, so no conditions will be evaluated before execution, and all conditions are evaluated after execution
            Assert.That(string.Join(",", steps), Is.EqualTo("Main.Execute," +
                                                            "Main.If.Asap,Main.If.Lazy,Main.If.Always"));

            steps.Clear();
            sm.Execute();
            // Repeat
            Assert.That(string.Join(",", steps), Is.EqualTo("Start.Execute," +
                                                            "Start.If.Asap,Start.If.Lazy,Start.If.Always"));
        }
        
        [Test(Description = "All Ifs() of any type are executed once (at the end) when the state change at the end of the state")]
        public void ConditionalOrderStateChangesAtTheEnd() {
            var sm = new StateMachineSync<State, Event>(State.Start);

            var externalChange = false;
            var steps = new List<string>();
            sm.State(State.Start)
                .Execute(() => {
                    steps.Add("Start.Execute");
                })
                .If(() => {
                    steps.Add("Start.If.Asap");
                    return false;
                }, Condition.Type.Asap)
                .Set(State.MainMenu)
                .If(() => {
                    steps.Add("Start.If.Lazy");
                    return false;
                }, Condition.Type.Lazy)
                .Set(State.MainMenu)
                .If(() => {
                    steps.Add("Start.If.Always");
                    return externalChange;
                })
                .Set(State.MainMenu)
                .Build();
            
            sm.State(State.MainMenu)
                .Execute(() => {
                    steps.Add("Main.Execute");
                })
                .If(() => {
                    steps.Add("Main.If.Asap");
                    return false;
                }, Condition.Type.Asap)
                .Set(State.Start)
                .If(() => {
                    steps.Add("Main.If.Lazy");
                    return false;
                }, Condition.Type.Lazy)
                .Set(State.Start)
                .If(() => {
                    steps.Add("Main.If.Always");
                    return false;
                })
                .Set(State.Start)
                .Build();
            
            steps.Clear();
            sm.Execute();
            // Start is a new state, so no conditions will be evaluated before execution, and all conditions are evaluated after execution
            Assert.That(string.Join(",", steps), Is.EqualTo("Start.Execute," +
                                                            "Start.If.Asap,Start.If.Lazy,Start.If.Always"));

            externalChange = true;
            steps.Clear();
            sm.Execute();
            // The state is the same as previous execution, so, only the Asap and Always conditions are evaluated before execution.
            // In this condition, the state changed, so, execute the new state and evaluate all conditions after
            Assert.That(string.Join(",", steps), Is.EqualTo("Start.If.Asap,Start.If.Always," +
                                                            "Main.Execute," +
                                                            "Main.If.Asap,Main.If.Lazy,Main.If.Always"));

        }
    }
}