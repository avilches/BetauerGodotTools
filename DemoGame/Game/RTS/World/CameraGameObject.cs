using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Betauer.Application.Persistent;
using Betauer.DI.Attributes;
using Godot;

namespace Veronenger.Game.RTS.World;

public class CameraGameObject : GameObject {
    private float _zoom;
    private int _zoomPosition;
    
    [Inject] public RtsConfig RtsConfig { get; private set; }
    public float Zoom {
        get => _zoom;
        set {
            _zoom = value;
            _zoomPosition = FindClosestPosition(RtsConfig.ZoomLevels, _zoom);
        }
    }

    public Vector2 Position { get; set; }
    public Camera2D Camera2D { get; set; }
    
    public override void OnInitialize() {
    }

    public override void OnRemove() {
        throw new Exception("ScreenStateGameObject cannot be removed ever!");
    }

    public override void OnLoad(SaveObject saveObject) {
        var screenStateSaveObject = (CameraSaveObject)saveObject;
        Zoom = screenStateSaveObject.Zoom;
        Position = screenStateSaveObject.Position;
    }

    public void Configure(Camera2D camera2D) {
        Camera2D = camera2D;
    }

    public override SaveObject CreateSaveObject() => new CameraSaveObject(this);

    public bool IncreaseZoomLevel() {
        if (_zoomPosition < RtsConfig.ZoomLevels.Count - 1) {
            _zoomPosition++;
            _zoom = RtsConfig.ZoomLevels[_zoomPosition];
            return true;
        }
        return false;
    }
    
    public bool DecreaseZoomLevel() {
        if (_zoomPosition > 0) {
            _zoomPosition--;
            _zoom = RtsConfig.ZoomLevels[_zoomPosition];
            return true;
        }
        return false;
    }
    
    private static int FindClosestPosition(IReadOnlyList<float> array, float value) {
        var closestPosition = 0;
        for (var i = 0; i < array.Count; i++) {
            if (Math.Abs(array[i] - value) < Math.Abs(array[closestPosition] - value)) {
                closestPosition = i;
            }
        }
        return closestPosition;
    }
}

public class CameraSaveObject : SaveObject<CameraGameObject>, IRtsSaveObject {
    [JsonInclude] public float Zoom { get; set; }
    [JsonInclude] public Vector2 Position { get; set; }

    public override string Discriminator() => "camera";

    public CameraSaveObject() {
    }

    public CameraSaveObject(CameraGameObject cameraGameObject) : base(cameraGameObject) {
        Zoom = cameraGameObject.Zoom;
        Position = cameraGameObject.Camera2D.Position;
    }
}