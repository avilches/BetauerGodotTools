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
            Container.Scanner.Scan();
            Container.Instance<Func<SceneTree>>(GetTree);
/*            DefaultRepository.Register<RootSceneHolder>((Node node) => {
                Viewport root = GetTree().Root;
                Node lastFound = node;
                Node parent = lastFound.GetParent();
                bool found = parent == root;
                while (!found) {
                    lastFound = parent;
                    parent = lastFound.GetParent();
                    found = parent == root;
                }
                RootSceneHolder sceneHolder = lastFound.GetNode<RootSceneHolder>(nameof(RootSceneHolder));
                if (sceneHolder == null) {
                    sceneHolder = new RootSceneHolder();
                    lastFound.AddChild(sceneHolder);
                }
                return sceneHolder;
            });
*/
            Container.Build();
            Container.AutoWire(this);
        }

        public virtual Container CreateContainer() {
            return new Container(this);
        }

    }

}