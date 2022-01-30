using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Godot;
using Expression = System.Linq.Expressions.Expression;
using Object = Godot.Object;

namespace Betauer {
    [AttributeUsage(AttributeTargets.Field)]
    public class InjectAttribute : Attribute {
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class OnReadyAttribute : Attribute {
        public string Path;

        public OnReadyAttribute(string path) {
            Path = path;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class SingletonAttribute : Attribute {
    }

    public interface IService {
        public Type GetServiceType();
        public Lifestyle GetLifestyle();
        public object Resolve(DiRepository repository);
    }

    public class SingletonInstanceHandler : IService {
        private readonly object _instance;

        public SingletonInstanceHandler(object instance) {
            _instance = instance;
        }

        public object Resolve(DiRepository repository) => _instance;
        public Type GetServiceType() => _instance.GetType();
        public Lifestyle GetLifestyle() => Lifestyle.Singleton;
    }

    public class LifestyleHandler<T> : IService {
        private readonly Func<T> _factory;
        private readonly Lifestyle _lifestyle;
        private readonly Type _type;

        private bool _singletonDefined;
        private T _singleton;

        public LifestyleHandler(Func<T> factory, Lifestyle lifestyle) : this(typeof(T), factory, lifestyle) {
        }

        public LifestyleHandler(Type type, Func<T> factory, Lifestyle lifestyle) {
            _factory = factory;
            _lifestyle = lifestyle;
            _type = type;
        }

        public Type GetServiceType() => _type;
        public Lifestyle GetLifestyle() => _lifestyle;
        public object Resolve(DiRepository repository) {
            if (_lifestyle == Lifestyle.Singleton) {
                if (_singletonDefined) return _singleton;
                lock (this) {
                    // Just in case another was waiting for the lock
                    if (_singletonDefined) return _singleton;
                    _singletonDefined = true;
                    _singleton = _factory();
                    repository.AfterCreate(_singleton);
                }
                return _singleton;
            }
            // Transient or Scene
            var o = _factory();
            repository.AfterCreate(o);
            return o;
        }
    }

    public class SingletonFactoryWithNode<T> {
        private readonly Func<Node, T> _factory;

        public SingletonFactoryWithNode(Func<Node, T> factory) {
            _factory = factory;
        }

        // public T Build() {
        // return new T;
        // }

        public T Build(Node node) {
            return _factory(node);
        }
    }

    public enum Lifestyle {
        Transient,
        Singleton
    }

    public class DiRepository : Node {
        private readonly Dictionary<Type, IService> _singletons = new Dictionary<Type, IService>();
        private readonly Logger _logger = LoggerFactory.GetLogger(typeof(DiRepository));
        public Action<object>? OnInstanceCreated;
        public Node _owner;

        public DiRepository(Node owner) {
            _owner = owner;
        }

        public void RegisterSingleton<T>(T instance) {
            RegisterSingleton(typeof(T), instance);
        }

        public void RegisterSingleton(object instance) {
            RegisterSingleton(instance.GetType(), instance);
        }

        public void RegisterSingleton(Type type, object instance) {
            if (!type.IsInstanceOfType(instance)) {
                throw new ArgumentException("Instance is not type of " + type);
            }
            Register(type, new SingletonInstanceHandler(instance));
        }

        public void Register(Type type, Func<object> factory, Lifestyle lifestyle) {
            Register(type, new LifestyleHandler<object>(type, factory, lifestyle));
        }

        public void Register<T>(Func<T> factory, Lifestyle lifestyle) {
            Register(typeof(T), new LifestyleHandler<T>(factory, lifestyle));
        }

        public void Register<T>(Lifestyle lifestyle) {
            Register(typeof(T), lifestyle);
        }

        public void Register(Type type, Lifestyle lifestyle) {
            Register(type, new LifestyleHandler<object>(() => Activator.CreateInstance(type), lifestyle));
        }

        public void Register(Type type, IService service) {
            _singletons[type] = service;
        }

        public T Resolve<T>() {
            return (T)Resolve(typeof(T));
        }

        public object Resolve(Type type) {
            var service = _singletons[type];
            var o = service.Resolve(this);
            return o;
        }

        public void AfterCreate<T>(T instance) {
            OnInstanceCreated?.Invoke(instance);
            if (instance is Node node) _owner.AddChild(node);
            AutoWire(instance);
        }

        public void Scan() {
            var assemblies = new[] { _owner.GetType().Assembly };
            Scan(assemblies);
        }

        public void Scan(Assembly assembly) {
            Scan(new Assembly[] { assembly });
        }

        public void Scan(IEnumerable<Assembly> assemblies) {
            Stopwatch stopwatch = Stopwatch.StartNew();
            int types = 0, added = 0;
            foreach (var assembly in assemblies) {
                foreach (Type type in assembly.GetTypes()) {
                    types++;
                    if (Attribute.GetCustomAttribute(type, typeof(SingletonAttribute),
                            false) is SingletonAttribute sa) {
                        Register(type, Lifestyle.Singleton);
                        added++;
                        _logger.Info("Added Singleton " + type.Name + " (" + type.FullName + ", Assembly: " +
                                     assembly.FullName + ")");
                    }
                }
            }
            _logger.Info(
                $"Scanned {types} types. Singletons: {added}. Elapsed time: {stopwatch.ElapsedMilliseconds} ms");
            stopwatch.Stop();
        }

        public void AutoWire(object instance) {
            InjectFields(instance);
        }

        private void InjectFields(object target) {
            var fields = target.GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in fields) {
                if (!(Attribute.GetCustomAttribute(property, typeof(InjectAttribute), false) is InjectAttribute inject))
                    continue;
                try {
                    var found = Resolve(property.FieldType);
                    property.SetValue(target, found);
                } catch (KeyNotFoundException) {
                    throw new Exception("Injectable property [" + property.FieldType.Name + " " + property.Name +
                                  "] not found while injecting fields in " + target.GetType().Name);
                }
            }
        }

        public void LoadOnReadyNodes(Node target) {
            var fields = target.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Public |
                                                    BindingFlags.Instance);
            foreach (var property in fields) {
                if (!(Attribute.GetCustomAttribute(property, typeof(OnReadyAttribute), false) is OnReadyAttribute
                        onReady))
                    continue;
                var instance = target.GetNode(onReady.Path);
                var fieldInfo = "[OnReady(\"" + onReady.Path + "\")] " + property.FieldType.Name + " " +
                                property.Name;
                if (instance == null) {
                    throw new Exception("OnReady path is null in field " + fieldInfo + ", class " +
                                        target.GetType().Name);
                } else if (instance.GetType() != property.FieldType) {
                    throw new Exception("OnReady path returned a wrong type (" + instance.GetType().Name +
                                        ") in field " + fieldInfo + ", class " +
                                        target.GetType().Name);
                }
                property.SetValue(target, instance);
            }
        }

        public void Dispose() {
            foreach (var instance in _singletons.Values) {
                if (instance is IDisposable obj) {
                    try {
                        _logger.Info("Disposing singleton: " + obj.GetType());
                        obj.Dispose();
                    } catch (Exception e) {
                        _logger.Error(e);
                    }
                }
            }
        }

    }

    public abstract class Di {
        protected Di() => DiBootstrap.DefaultRepository.AutoWire(this);
    }

    public abstract class DiNode : Node {
        protected DiNode() => DiBootstrap.DefaultRepository.AutoWire(this);

        public sealed override void _Ready() {
            DiBootstrap.DefaultRepository.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }

    public abstract class DiControl : Control {
        protected DiControl() => DiBootstrap.DefaultRepository.AutoWire(this);

        public sealed override void _Ready() {
            DiBootstrap.DefaultRepository.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }

    public abstract class DiButton : Button {
        protected DiButton() => DiBootstrap.DefaultRepository.AutoWire(this);

        public sealed override void _Ready() {
            DiBootstrap.DefaultRepository.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }

    public abstract class DiNode2D : Node2D {
        protected DiNode2D() => DiBootstrap.DefaultRepository.AutoWire(this);

        public sealed override void _Ready() {
            DiBootstrap.DefaultRepository.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }

    public abstract class DiKinematicBody2D : KinematicBody2D {
        protected DiKinematicBody2D() => DiBootstrap.DefaultRepository.AutoWire(this);

        public sealed override void _Ready() {
            DiBootstrap.DefaultRepository.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }

    public abstract class DiCamera2D : Camera2D {
        protected DiCamera2D() => DiBootstrap.DefaultRepository.AutoWire(this);

        public sealed override void _Ready() {
            DiBootstrap.DefaultRepository.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }

    public abstract class DiArea2D : Area2D {
        protected DiArea2D() => DiBootstrap.DefaultRepository.AutoWire(this);

        public sealed override void _Ready() {
            DiBootstrap.DefaultRepository.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }


    /**
     * DiBootstrap. Singleton + Node + Special Di (scan all + autowire ifself)
     */
    public abstract class
        DiBootstrap : Node /* needed because 1) it's an autoload 2) all Node singletons scanned will be added as child */ {
        public static DiRepository DefaultRepository;
        public static DiBootstrap Instance;
        public static readonly bool IsDevelopment = OS.HasFeature("editor");

        public DiBootstrap() {
            if (Instance != null) {
                throw new Exception("DiBootstrap can't be instantiated more than once: " + GetType().Name);
            }
            Instance = this;
            DefaultRepository = CreateDiRepository();
            DefaultRepository.Scan();
            DefaultRepository.RegisterSingleton<Func<SceneTree>>(GetTree);
            DefaultRepository.AutoWire(this);
        }

        public virtual DiRepository CreateDiRepository() {
            return new DiRepository(this);
        }

        public void CheckErrorPolicy(UnhandledExceptionPolicy unhandledExceptionPolicyConfig) {
            var unhandledExceptionPolicySetting = GetUnhandledExceptionPolicySetting();
            if (!IsDevelopment) {
                // This is a release
                if (unhandledExceptionPolicySetting == UnhandledExceptionPolicy.TerminateApplication) {
                    // If the app can crash with just an exception, then better to crash as soon as possible
                    throw new Exception(
                        "Please, don't use export a release with the mono/unhandled_exception_policy as Terminate Application");
                }
            }
            if (unhandledExceptionPolicySetting == UnhandledExceptionPolicy.LogError) {
                GD.Print("Current unhandled exception policy: Log Error");
                if (unhandledExceptionPolicyConfig == UnhandledExceptionPolicy.TerminateApplication) {
                    WriteUnhandledExceptionPolicy(UnhandledExceptionPolicy.TerminateApplication);
                }
            } else if (unhandledExceptionPolicySetting == UnhandledExceptionPolicy.TerminateApplication) {
                GD.Print("Current unhandled exception policy: Terminate Application");
                if (unhandledExceptionPolicyConfig == UnhandledExceptionPolicy.LogError) {
                    WriteUnhandledExceptionPolicy(UnhandledExceptionPolicy.LogError);
                }
            }
        }

        private void WriteUnhandledExceptionPolicy(UnhandledExceptionPolicy policy) {
            GD.PushWarning("Writing mono/unhandled_exception_policy=" + policy +
                           " in project.godot configuration. Restart to reload changes. Aborting...");
            ProjectSettings.SetSetting("mono/unhandled_exception_policy", (int)policy);
            ProjectSettings.Save();
            GetTree().Quit();
        }

        private static UnhandledExceptionPolicy GetUnhandledExceptionPolicySetting() {
            var x = ProjectSettings.GetSetting("mono/unhandled_exception_policy");
            return x is int policy && policy == (int)UnhandledExceptionPolicy.LogError
                ? UnhandledExceptionPolicy.LogError
                : UnhandledExceptionPolicy.TerminateApplication;
        }
    }

    public enum UnhandledExceptionPolicy {
        TerminateApplication = 0,
        LogError = 1
    }
}