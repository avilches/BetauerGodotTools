using System;
using System.Text.Json.Serialization;
using Betauer.Application.Persistent;
using Betauer.DI.Attributes;
using Godot;

namespace Veronenger.Game.RTS.World;

public class CameraGameObject : GameObject {
    
    [Inject] public RtsConfig RtsConfig { get; private set; }

    public int ZoomLevel { get; set; }
    public Vector2 Position { get; set; }
    public Camera2D Camera2D { get; set; }
    
    public override void OnInitialize() {
        ZoomLevel = RtsConfig.ZoomLevels.FindIndex(z => z == RtsConfig.DefaultZoom);
    }

    public override void OnRemove() {
        throw new Exception("ScreenStateGameObject cannot be removed ever!");
    }

    public override void OnLoad(SaveObject saveObject) {
        var screenStateSaveObject = (CameraSaveObject)saveObject;
        ZoomLevel = screenStateSaveObject.ZoomLevel;
        Position = screenStateSaveObject.Position;
    }

    public void Configure(Camera2D camera2D) {
        Camera2D = camera2D;
    }

    public override SaveObject CreateSaveObject() => new CameraSaveObject(this);
}

public class CameraSaveObject : SaveObject<CameraGameObject>, IRtsSaveObject {
    [JsonInclude] public int ZoomLevel { get; set; }
    [JsonInclude] public Vector2 Position { get; set; }

    public override string Discriminator() => "camera";

    public CameraSaveObject() {
    }

    public CameraSaveObject(CameraGameObject cameraGameObject) : base(cameraGameObject) {
        ZoomLevel = cameraGameObject.ZoomLevel;
        Position = cameraGameObject.Camera2D.Position;
    }
}