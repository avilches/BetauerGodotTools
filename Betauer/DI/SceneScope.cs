using System;
using Godot;
using Godot.Collections;

namespace Betauer.DI {
    // public class RootSceneHolder : Node {
        // protected override void Dispose(bool disposing) {
            // base.Dispose(disposing);
        // }
    // }

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
        //     if (!_sceneHolders.TryGetValue(scene, out var sceneHolder)) {
        //         sceneHolder = new RootSceneHolder();
        //         _sceneHolders[scene] = sceneHolder;
        //         scene.CallDeferred("add_child", sceneHolder);
        //         return sceneHolder;
        //     }
        //     return sceneHolder;
        // }

        // TODO: not tested. Test with a node outside of the tree
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
    }
}