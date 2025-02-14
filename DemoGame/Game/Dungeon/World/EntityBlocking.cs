using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Veronenger.Game.Dungeon.World;

public class EntityBlocking {
    private TaskCompletionSource<ActionCommand>? _promise;
    public Queue<ActionCommand> Queue { get; } = [];

    public Entity Entity { get; }

    public EntityBlocking(Entity entity) {
        Entity = entity;
        entity.OnDecideAction = DecideAction;
    }

    public bool IsWaiting => _promise != null;

    public void SetResult(ActionCommand actionCommand) {
        var promise = _promise;
        if (promise == null) {
            throw new Exception("No action to resolve");
        }
        if (promise.Task.IsCompleted) {
            throw new Exception("Player action already set");
        }
        _promise = null;
        promise.TrySetResult(actionCommand);
    }

    public void ScheduleNextAction(ActionCommand nextActionCommand) {
        Queue.Enqueue(nextActionCommand);
    }

    public Task<ActionCommand> DecideAction() {
        if (Queue.Count > 0) {
            var action = Queue.Dequeue();
            return Task.FromResult(action);
        }
        _promise ??= new TaskCompletionSource<ActionCommand>();
        return _promise.Task;
    }
}