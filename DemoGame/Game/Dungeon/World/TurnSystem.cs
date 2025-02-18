using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Betauer.Tools.Logging;

namespace Veronenger.Game.Dungeon.World;

public class TurnSystem(WorldMap worldMap) {

    private static readonly Logger Logger = LoggerFactory.GetLogger<TurnSystem>();

    public bool Running { get; private set; } = false;
    public bool Busy { get; private set; } = false;

    public async Task Run(double maxLoopDurationSecs = 0.0166d, Func<Task>? awaiter = null) {
        if (Running) return;
        Running = true;

        var loopStartTime = DateTime.Now;

        while (Running) {
            await ProcessTickAsync();

            if (awaiter != null) {
                var loopDuration = (DateTime.Now - loopStartTime).TotalSeconds;
                if (loopDuration > maxLoopDurationSecs) {
                    await awaiter();
                    loopStartTime = DateTime.Now;
                }
            }
        }
    }

    public async Task ProcessTickAsync() {
        if (Busy) return;
        Running = true;
        Busy = true;
        worldMap.NextTick();

        List<EntityBase> destroyedQueue = null;

        foreach (var entity in worldMap.Entities) {
            if (entity.Removed) {
                destroyedQueue ??= [];
                destroyedQueue.Add(entity);
                continue;
            }

            // AddEntity ensures all the Async entities (players) goes first

            entity.TickStart();
            if (entity.CanAct()) {
                if (entity is IEntityAsync asyncEntity) {
                    var action = await DecideAction(entity.Name, asyncEntity);
                    if (action != null) entity.Execute(action);
                    if (!Running || action?.Type == ActionType.EndGame) {
                        // Only players can stop the tick (setting Running = false or ActionType.EndGame)
                        // If the game is stopped, we can end the tick immediately
                        entity.TickEnd();
                        break;
                    }
                } else if (entity is IEntitySync syncEntity) {
                    entity.Execute(syncEntity.DecideAction());

                }
            }
            entity.TickEnd();
        }

        if (destroyedQueue != null) {
            foreach (var entity in destroyedQueue) worldMap.RemoveEntity(entity);
        }
        worldMap.ApplyPendingChanges();
        Busy = false;
    }

    public void Stop() {
        Running = false;
    }

    private async Task<ActionCommand?> DecideAction(string name, IEntityAsync entity) {
        while (Running) {
            try {
                var decideTask = entity.DecideAction();
                var timeoutTask = Task.Delay(166);
                var completedTask = await Task.WhenAny(decideTask, timeoutTask);
                if (completedTask == decideTask) {
                    return await decideTask;
                }
            } catch (Exception e) {
                Logger.Error($"Error deciding action for {name}: {e.Message}");
                return null;
            }
        }
        return null;
    }
}