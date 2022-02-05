using System;
using Godot;

namespace Betauer.DI {
    /**
     * DiBootstrap. Singleton + Node + Special Di (scan all + autowire ifself)
     */
    public abstract class Bootstrap : Node
        /* needed because 1) it's an autoload 2) all Node singletons scanned will be added as child */ {
        public static Container Container;
        private static bool _booted = false;

        public Bootstrap() {
            if (_booted) {
                throw new Exception("A Bootstrap class can't be instantiated more than once: " + GetType().Name);
            }
            _booted = true;
            Container = CreateContainer();
            Container.Instance(Container); // this allow to have [Inject] private Container container
            Container.Scanner.Scan();
            Container.Instance<Func<SceneTree>>(GetTree);
            /*
            Container.Function<Node, RootSceneHolder?>((node) => {
                Viewport? root = node.GetTree()?.Root;
                if (root == null) {
                    // TODO: not tested. Test with a node outside of the tree
                    return null;
                }
                Node lastFound = node;
                Node parent = lastFound.GetParent();
                bool found = parent == root;
                while (!found && parent != null) {
                    lastFound = parent;
                    parent = lastFound.GetParent();
                    found = parent == root;
                }
                RootSceneHolder? sceneHolder = lastFound.FindFirstChild<RootSceneHolder>();
                if (sceneHolder == null) {
                    sceneHolder = new RootSceneHolder();
                    sceneHolder.Name = nameof(RootSceneHolder);
                    lastFound.AddChild(sceneHolder);
                }
                return sceneHolder;
            });
            */
            Container.Build();
            Container.InjectAllFields(this); // TODO: change this and use a [Bootstrap] attribute
        }

        public virtual Container CreateContainer() {
            return new Container(this);
        }

    }

}