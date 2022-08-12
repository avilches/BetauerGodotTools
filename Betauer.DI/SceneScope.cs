using System;
// using Godot;
// using Godot.Collections;

namespace Betauer.DI {
    // public class RootSceneHolder : Node {
        // protected override void Dispose(bool disposing) {
            // base.Dispose(disposing);
        // }
    // }

    // TODO: not tested and not used yet. The RootSceneHolder is an object child of the scene and can be accessed
    // from everywhere. The SceneScope keeps the track in a Dictionary as a cache only for the first frame, where
    // its built. It means if 3 objects try to access to the RootSceneHolder before its created, the first call
    // will create it, but it will add the RootSceneHolder as a child of the scene in the next frame (deferred).
    // So, the next two objects will not found RootSceneHolder neither but they will use the instance stored in the
    // Dictionary.

    public class SceneScope {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(SceneScope));

        // private readonly Dictionary<Node, RootSceneHolder> _sceneHolders = new Dictionary<Node, RootSceneHolder>();
        //
        // public RootSceneHolder GetSceneHolder(Node node) {
        //     Node? scene = FindSceneNode(node);
        //     if (scene == null) return null;
        //     if (scene is Bootstrap) {
        //         // TODO: test
        //         throw new Exception(nameof(RootSceneHolder) + " can't be used from singleton");
        //     }
        //     if (scene != null) // remove from _sceneHolders dictionary and mark is as already searchable, so it could
        //     be safely deleted in other thread, just to avoid to fill the dictionary with not needed stuff.
        //
        //     if (!_sceneHolders.TryGetValue(scene, out var sceneHolder)) {
        //         sceneHolder = new RootSceneHolder();
        //         _sceneHolders[scene] = sceneHolder;
        //         scene.CallDeferred("add_child", sceneHolder);
        //         return sceneHolder;
        //     }
        //     return sceneHolder;
        // }

        // TODO: not tested. Test with a node outside of the tree
        /*
        private static Node? FindSceneNode(Node node) {
            Viewport? root = node.GetTree()?.Root;
            if (root == null) {
                return null;
            }
            var scene = node;
            Node? parent = scene.GetParent();
            var isScene = parent == root;
            while (!isScene && parent != null) {
                scene = parent;
                parent = scene.GetParent();
                isScene = parent == root;
            }

            return isScene ? scene : null;
        }
        */
    }
}