using System.Collections.Generic;
using Godot;

namespace Betauer.Camera.Control;

public class CameraContainer {
    public readonly Dictionary<Camera2D, CameraController> Registry = new();
    public readonly List<CameraController> Cameras = new();
    
    public CameraController Camera(Camera2D camera) {
        if (Registry.TryGetValue(camera, out var control)) return control;
        var cameraControl = new CameraController(this, camera);
        Registry[camera] = cameraControl;
        Cameras.Add(cameraControl);
        return cameraControl;
    }

    public void Remove(Camera2D camera) {
        Registry.Remove(camera);
        Cameras.RemoveAll(cameraControl => cameraControl.Camera2D == camera);
    }
}