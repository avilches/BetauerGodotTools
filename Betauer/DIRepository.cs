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
    public class ServiceAttribute : Attribute {
        public Lifestyle Lifestyle { get; set; } = Lifestyle.Singleton;
    }

    public interface IService {
        public Type GetServiceType();
        public Lifestyle GetLifestyle();
        public object Resolve(DiRepository repository);
    }

    public class SingletonInstanceHandler : IService {
        private readonly Type _type;
        private readonly object _instance;

        public SingletonInstanceHandler(Type type, object instance) {
            _type = type;
            _instance = instance;
        }

        public object Resolve(DiRepository repository) => _instance;
        public Type GetServiceType() => _type;
        public Lifestyle GetLifestyle() => Lifestyle.Singleton;
    }

    public class LifestyleHandler<T> : IService {
        private readonly Logger _logger = LoggerFactory.GetLogger(typeof(DiRepository));
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
                    _singleton = CreateInstance(repository);
                }
                return _singleton;
            }
            // Transient or Scene
            return CreateInstance(repository);
        }

        private T CreateInstance(DiRepository repository) {
            var o = _factory();
            _logger.Debug("Creating " + _lifestyle + " instance: " + _type.Name + " " + o.GetType().Name);
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

        public IService RegisterSingleton<T>(T instance) {
            return RegisterSingleton(typeof(T), instance);
        }

        public IService RegisterSingleton(Type type, object instance) {
            if (!type.IsInstanceOfType(instance)) {
                throw new ArgumentException("Instance is not type of " + type);
            }
            var singletonInstanceHandler = new SingletonInstanceHandler(type, instance);
            Register(type, singletonInstanceHandler);
            AfterCreate(instance);
            return singletonInstanceHandler;
        }

        public IService Register(Lifestyle lifestyle, Type type, Func<object> factory) {
            return Register(type, new LifestyleHandler<object>(type, factory, lifestyle));
        }

        public IService Register<T>(Lifestyle lifestyle, Func<T> factory) {
            return Register(typeof(T), new LifestyleHandler<T>(factory, lifestyle));
        }

        public IService Register<T>(Lifestyle lifestyle) {
            return Register(lifestyle, typeof(T));
        }

        public IService Register(Lifestyle lifestyle, Type type) {
            return Register(type, new LifestyleHandler<object>(type, () => Activator.CreateInstance(type), lifestyle));
        }

        public IService Register(Type type, IService service) {
            _singletons[type] = service;
            _logger.Info("Registered " + service.GetLifestyle() + " Type " + type.Name + ": " +
                         service.GetServiceType().Name + " (Assembly: " +
                         type.Assembly.GetName() + ")");
            return service;
        }

        public T Resolve<T>() {
            return (T)Resolve(typeof(T));
        }

        public object Resolve(Type type) {
            var service = _singletons[type];
            var o = service.Resolve(this);
            return o;
        }

        internal void AfterCreate<T>(T instance) {
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
            int types = 0, assembliesCount = 0;
            Stopwatch stopwatch = Stopwatch.StartNew();
            foreach (var assembly in assemblies) {
                assembliesCount++;
                foreach (Type type in assembly.GetTypes()) {
                    types++;
                    Scan(type);
                }
            }
            stopwatch.Stop();
            _logger.Info(
                $"Registered {types} types in {assembliesCount} assemblies. Elapsed time: {stopwatch.ElapsedMilliseconds} ms");
        }

        public void Scan(Type type) {
            if (Attribute.GetCustomAttribute(type, typeof(ServiceAttribute),
                    false) is ServiceAttribute sa) {
                Register(sa.Lifestyle, type);
            }
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