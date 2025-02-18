using System;
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

        foreach (var entity in worldMap.Entities.ToArray()) { // ToArray() to avoid concurrent modification
            // AddEntity ensures all the Async entities goes first

            entity.TickStart();
            if (entity.CanAct()) {
                if (entity is IEntitySync syncEntity) {
                    entity.Execute(syncEntity.DecideAction());

                } else if (entity is IEntityAsync asyncEntity) {
                    var action = await DecideAction(entity.Name, asyncEntity);
                    if (action != null) entity.Execute(action);
                    if (!Running || action?.Type == ActionType.EndGame) {
                        break;
                    }
                }
            }
            entity.TickEnd();
        }
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