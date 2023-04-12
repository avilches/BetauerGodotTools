using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Betauer.FSM.Async;
using Betauer.TestRunner;
using Betauer.Tools.Reflection;
using Godot;
using NUnit.Framework;

namespace Betauer.GameTools.Tests; 

[TestRunner.Test]
public partial class FSMNodeAsyncTests : Node {
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
    }
    [TestRunner.Test(Description = "Constructor")]
    public void FSMNodeConstructors() {
        var sm1 = new FsmNodeAsync<State, Trans>(State.A, "X");
        Assert.That(sm1.GetFsmEvents().Name, Is.EqualTo("X"));
        Assert.That(sm1.ProcessInPhysics, Is.False);

        var sm2 = new FsmNodeAsync<State, Trans>(State.A, null, true);
        Assert.That(sm2.GetFsmEvents().Name, Is.EqualTo(sm2.GetType().GetTypeName()));
        Assert.That(sm2.ProcessInPhysics, Is.True);

        var sm3 = new FsmNodeAsync<State, Trans>(State.A);
        Assert.That(sm3.GetFsmEvents().Name, Is.EqualTo(sm3.GetType().GetTypeName()));
        Assert.That(sm3.ProcessInPhysics, Is.False);
    }

    [TestRunner.Test(Description = "FSMNode, BeforeExecute and AfterExecute events with idle frames in the execute")]
    public async Task AsyncFSMNodeWithIdleFrame() {
        var sm = new FsmNodeAsync<State, Trans>(State.Start);

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
        
    [TestRunner.Test]
    public async Task EnterOnPushExitOnPopSuspendAwakeListener() {
        var sm = new FsmNodeAsync<State, Trans>(State.MainMenu);

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
        sm.OnBefore += ()  => states.Add(":execute.start");
        sm.OnAfter += ()  => states.Add(":execute.end");

        AddChild(sm);

        Stopwatch stopwatch = Stopwatch.StartNew();
        while (sm.CurrentState?.Key != State.End && stopwatch.ElapsedMilliseconds < 1000) {
            Console.WriteLine(sm.CurrentState?.Key + "...");
            await this.AwaitProcessFrame();
        }

        Console.WriteLine(string.Join(",", states));
        Assert.That(string.Join(",", states), Is.EqualTo(
            ":execute.start,from:MainMenu-to:MainMenu,MainMenu:enter,:execute.end," +
            ":execute.start,MainMenu:exit,from:MainMenu-to:Debug,Debug:enter,:execute.end," +
            ":execute.start,Debug:suspend,from:Debug-to:Settings,Settings:enter,:execute.end," +
            ":execute.start,Settings:exit,from:Settings-to:Debug,Debug:awake,:execute.end," +
            ":execute.start,Debug:exit,from:Debug-to:End,End:enter,:execute.end"));
    }
        
    [TestRunner.Test]
    public async Task OnInput() {
        var sm = new FsmNodeAsync<State, Trans>(State.MainMenu);

        var i = 0;
        var u = 0;
        sm.State(State.MainMenu)
            .OnInput((e) => i++)
            .OnInput((e) => i++)
            .OnUnhandledInput((e) => u ++)
            .OnUnhandledInput((e) => u ++)
            .Build();
            
        AddChild(sm);
        await this.AwaitProcessFrame();
        GetTree().Root.PushInput(new InputEventJoypadButton {
            ButtonIndex = JoyButton.A,
        });
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        Assert.That(i, Is.EqualTo(2));
        Assert.That(u, Is.EqualTo(0));
    }     
        
    [TestRunner.Test]
    public async Task OnUnhandledInput() {
        var sm = new FsmNodeAsync<State, Trans>(State.MainMenu);

        var i = 0;
        var u = 0;
        sm.State(State.MainMenu)
            .OnInput((e) => i++)
            .OnInput((e) => i++)
            .OnUnhandledInput((e) => u ++)
            .OnUnhandledInput((e) => u ++)
            .Build();
            
        AddChild(sm);
        await this.AwaitProcessFrame();
        GetTree().Root.PushUnhandledInput(new InputEventJoypadButton {
            ButtonIndex = JoyButton.A,
        });
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        await this.AwaitProcessFrame();
        Assert.That(i, Is.EqualTo(0));
        Assert.That(u, Is.EqualTo(2));
    }
}