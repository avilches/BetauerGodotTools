using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Veronenger.Game.Dungeon.Scheduling;

public class TurnContext {
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

    public void NextTick() {
        if (CurrentTick % TicksPerTurn == 0) {
            CurrentTurn++;
            // Console.WriteLine($"# Turn: {CurrentTurn}");
        }
        CurrentTick++;
    }
}

public class TurnSystem(TurnContext context) {
    public TurnContext Context { get; set; } = context;
    public async Task ProcessTickAsync() {
        Context.NextTick();

        // ToArray() to avoid concurrent modification
        foreach (var entity in Context.Entities.ToArray()) {
            entity.TickStart(Context);
            if (entity.CanAct(Context)) {
                var action = await entity.DecideAction(Context);
                entity.Execute(Context, action);
            }
            entity.TickEnd(Context);
        }
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