using System.Collections.Generic;
using Godot;

namespace Betauer.Camera.Follower;

public class CameraContainer {
    public Dictionary<Camera2D, CameraControl> Registry = new();
    public List<CameraControl> Cameras = new();
    
    public CameraControl Camera(Camera2D camera) {
        if (Registry.TryGetValue(camera, out var control)) return control;
        var cameraControl = new CameraControl(this, camera);
        Registry[camera] = cameraControl;
        Cameras.Add(cameraControl);
        return cameraControl;
    }

    public void Remove(Camera2D camera) {
        Registry.Remove(camera);
        Cameras.RemoveAll(cameraControl => cameraControl.Camera2D == camera);
    }
}