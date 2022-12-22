using System;
using Betauer.Core.Nodes;
using Betauer.Tools.Logging;
using Godot;

namespace Betauer.Camera; 

public class CameraStageLimiter {
    private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(CameraStageLimiter));
    private Area2D? _enteredStage;
    private bool _exitedStage;
    private Area2D? _currentStage;
    private Camera2D _camera2D;
    private bool IsValidStageChange => _exitedStage && _enteredStage != null;
    private readonly int _layer;

    /// <summary>
    /// Called with (oldStage?, newStage)
    /// </summary>
    public event Action<Area2D?, Area2D>? OnChangeStage;

    public CameraStageLimiter(int layer) {
        _layer = layer;
    }

    public void ConfigureStageCamera(Camera2D camera2D, Area2D stageDetector) {
        _camera2D = camera2D;
        stageDetector.OnAreaEntered(_layer, OnEnterStage);
        stageDetector.OnAreaExited(_layer, OnExitStage);
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
            throw new Exception(
                $"Stage {area2D.Name}/{nodeChild?.Name} is not a CollisionShape2D with a RectangleShape2D shape");
        }
    }

    private void OnEnterStage(Area2D stageToEnter) {
        if (_currentStage == null) {
            ChangeStage(stageToEnter);
            return;
        }
        _enteredStage = stageToEnter;
        if (IsValidStageChange) {
            Logger.Debug($"Transition finished. Exit: \"{_currentStage.Name}\" -> Enter: \"{_enteredStage.Name}\" REVERSED (first exit -> then enter)");
            ChangeStage(_enteredStage);
        }
    }

    private void OnExitStage(Area2D stageExitedArea2D) {
        if (_enteredStage != null && stageExitedArea2D.Equals(_enteredStage)) {
            _enteredStage = null;
            _exitedStage = false;
        }
        _exitedStage = true;
        if (IsValidStageChange) {
            Logger.Debug($"Transition finished. Exit: \"{_currentStage!.Name}\" -> Enter: \"{_enteredStage!.Name}\"");
            ChangeStage(_enteredStage);
        }
    }

    private void ChangeStage(Area2D newStage) {
        var oldStage = _currentStage;
        ClearState();
        _currentStage = newStage;
        var rect2 = CreateAbsoluteRect2(_currentStage);
        Logger.Debug($"Camera {rect2.Position} {rect2.End}");
        _camera2D.LimitLeft = (int)rect2.Position.x;
        _camera2D.LimitTop = (int)rect2.Position.y;
        _camera2D.LimitRight = (int)rect2.End.x;
        _camera2D.LimitBottom = (int)rect2.End.y;
        OnChangeStage?.Invoke(oldStage, newStage);
    }

    public void ClearState() {
        _exitedStage = false;
        _currentStage = null;
        _enteredStage = null;
    }
        
    public Rect2 CreateAbsoluteRect2(Area2D Area2D) {
        RectangleShape2D shape2D = (Area2D.GetChild(0) as CollisionShape2D)?.Shape as RectangleShape2D;
        return new Rect2(Area2D.GlobalPosition - shape2D.Size / 2, shape2D.Size);
    }

}