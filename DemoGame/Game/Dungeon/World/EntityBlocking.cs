using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Veronenger.Game.Dungeon.World;

public class EntityBlocking {
    private TaskCompletionSource<EntityAction>? _promise;
    public Queue<EntityAction> Queue { get; } = [];

    public Entity Entity { get; }

    public EntityBlocking(Entity entity) {
        Entity = entity;
        entity.OnDecideAction = DecideAction;
    }

    public bool IsWaiting => _promise != null;

    public void SetResult(EntityAction action) {
        var promise = _promise;
        if (promise == null) {
            throw new Exception("No action to resolve");
        }
        if (promise.Task.IsCompleted) {
            throw new Exception("Player action already set");
        }
        _promise = null;
        promise.TrySetResult(action);
    }

    public void ScheduleNextAction(EntityAction nextAction) {
        Queue.Enqueue(nextAction);
    }

    public Task<EntityAction> DecideAction() {
        if (Queue.Count > 0) {
            var action = Queue.Dequeue();
            return Task.FromResult(action);
        }
        _promise ??= new TaskCompletionSource<EntityAction>();
        return _promise.Task;
    }
}