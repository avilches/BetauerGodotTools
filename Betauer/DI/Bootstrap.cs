using System;
using System.Diagnostics;
using Godot;

namespace Betauer.DI {
    /**
     * DiBootstrap. Singleton + Node + Special Di (scan all + autowire ifself)
     */
    public abstract class Bootstrap : Node
        /* needed because 1) it's an autoload 2) all Node singletons scanned will be added as child */ {
        protected readonly Logger Logger = LoggerFactory.GetLogger(typeof(Container));
        public static Container Container;
        private static bool _booted = false;

        public Bootstrap() {
            if (_booted) {
                throw new Exception("A Bootstrap class can't be instantiated more than once: " + GetType().Name);
            }
            _booted = true;

            Stopwatch stopwatch = Stopwatch.StartNew();
            SceneScope holder = new SceneScope();
            Container = CreateContainer();
            Container.Scanner.Scan();
            Container.Instance<Func<SceneTree>>(GetTree);
            Container.Function<Node, RootSceneHolder?>((node) => holder.GetSceneHolder(node));
            Container.Build();
            Container.InjectAllFields(this); // TODO: change this and use a [Bootstrap] attribute
            stopwatch.Stop();
            Logger.Info($"IoC time: {stopwatch.ElapsedMilliseconds} ms");

        }

        public virtual Container CreateContainer() {
            return new Container(this);
        }

    }

}