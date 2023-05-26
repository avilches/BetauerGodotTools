using System;
using Betauer.Core.Nodes;
using Betauer.Tools.Logging;
using Godot;

namespace Betauer.Camera; 

public class StageController {
    protected static readonly Logger Logger = LoggerFactory.GetLogger<StageController>();
    public Area2D? CurrentStage;
    private readonly int _layer;

    /// <summary>
    /// Called with (oldStage?, newStage)
    /// </summary>
    public event Action<Area2D?, Area2D>? OnChangeStage;

    public StageController(int layer) {
        _layer = layer;
    }

    public Action AddTarget(Area2D target) {
        return target.OnAreaEntered(_layer, ChangeStage);
    }

    public void ConfigureStage(Area2D stageArea2D) {
        ValidateStageArea2D(stageArea2D);
        stageArea2D.CollisionLayer = 0;
        stageArea2D.CollisionMask = 0;
        stageArea2D.AddToLayer(_layer);
    }

    private static void ValidateStageArea2D(Area2D area2D) {
        var nodeChild = area2D.GetChildCount() > 0 ? area2D.GetChild<CollisionShape2D>(0) : null;
        if (nodeChild?.Shape is not RectangleShape2D ) {
            throw new Exception($"Stage {area2D.Name}/{nodeChild?.Name} is not a CollisionShape2D with a RectangleShape2D shape");
        }
    }

    public void ChangeStage(Area2D stageToEnter) {
        if (stageToEnter == CurrentStage) return;
        var oldStage = CurrentStage;
        CurrentStage = stageToEnter;
        TriggerChangeState(oldStage, CurrentStage);
    }

    public void ClearState() {
        CurrentStage = null;
    }
    
    protected void TriggerChangeState(Area2D? from, Area2D stageToEnter) {
        OnChangeStage?.Invoke(from, stageToEnter);
    }
        
    public Rect2 CreateAbsoluteRect2(Area2D area2D) {
        RectangleShape2D shape2D = (RectangleShape2D)area2D.GetChild<CollisionShape2D>(0).Shape;
        return new Rect2(area2D.GlobalPosition - shape2D.Size / 2, shape2D.Size);
    }

}