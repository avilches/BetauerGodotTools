using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Betauer.Core;
using Godot;

namespace Veronenger.Game.Dungeon.Scheduling;

public class TurnContext;

public class TurnSystem {

    public static int TicksPerTurn = 10;

    public List<Entity> Entities { get; } = [];
    public int CurrentTick { get; private set; } = 0;
    public int CurrentTurn { get; private set; } = 0;

    public TurnContext Context { get; }

    public void AddEntity(Entity entity) {
        if (!Entities.Contains(entity)) Entities.Add(entity);
    }

    public bool RemoveEntity(Entity entity) {
        return Entities.Remove(entity);
    }

    public async Task ProcessTickAsync() {
        if (CurrentTick % TicksPerTurn == 0) {
            CurrentTurn++;
            Console.WriteLine($"# Turn: {CurrentTurn}");
        }
        CurrentTick++;


        // ToArray() to avoid concurrent modification
        var actingEntities = Entities.Where(e => e.DoCanAct()).ToArray();
        var tasks = new Task[actingEntities.Length];
        for (var i = 0; i < actingEntities.Length; i++) {
            var action = await actingEntities[i].DecideAction(Context);
            tasks[i] = action.Actor.DoExecute(Context, action);
        }
        await Task.WhenAll(tasks);

        Entities.ForEach(p => p.PostTick());
    }
}

public class TurnSystemProcess(TurnSystem turnSystem) {
    public bool Busy { get; private set; } = false;
    private TurnSystem TurnSystem { get; } = turnSystem;

    private Exception? _exception = null;

    public void _Process() {
        if (_exception != null) {
            var e = _exception;
            _exception = null;
            throw e;
        }
        if (Busy) return;
        Busy = true;
        TurnSystem.ProcessTickAsync().ContinueWith(t => {
            if (t.IsFaulted) {
                _exception = t.Exception?.GetBaseException() ?? t.Exception;
            }
            Busy = false;
        });
    }

}

