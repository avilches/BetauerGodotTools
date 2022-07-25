using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Betauer.DI;
using Betauer.Signal;
using Godot;

namespace Betauer.Loader {
    public class ResourceLoaderException : Exception {
        public ResourceLoaderException(string message) : base(message) {
        }
    }

    public class BaseResourceMetadataRegistry {
        protected Dictionary<string, ResourceMetadata> _registry;

        public bool Contains(string res) => _registry.ContainsKey(res);
        public T? Resource<T>(string res) where T : class => _registry[res].Resource as T;
        public T? Scene<T>(string res) where T : class => Resource<PackedScene>(res)?.Instance<T>();
    }

    public class ResourceMetadataRegistry : BaseResourceMetadataRegistry {
        public async Task Load(IEnumerable<string> resourcesToLoad, Action<LoadingContext> progress,
            Func<Task> awaiter) {
            _registry = await Loader.Load(resourcesToLoad, progress, awaiter);
        }

        public void Unload() {
            foreach (var resourceMetadata in _registry.Values) resourceMetadata.Resource.Dispose();
            _registry.Clear();
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ResourceAttribute : Attribute {
        public readonly string Path;

        public ResourceAttribute(string path) {
            Path = path;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SceneAttribute : Attribute {
        public readonly string Path;

        public SceneAttribute(string path) {
            Path = path;
        }
    }

    public abstract class ResourceLoaderContainer : ResourceMetadataRegistry {
        private LinkedList<Action<LoadingContext>>? _onProgressList;
        private Func<Task>? _awaiter;
        private int _maxTime = 100;
        private readonly LinkedList<object> _targets = new LinkedList<object>();
        [Inject] public SceneTree SceneTree;

        protected ResourceLoaderContainer() {
            _targets.AddLast(this);
        }

        public ResourceLoaderContainer SetMaxPollTime(int maxTime) {
            _maxTime = maxTime;
            return this;
        }

        public ResourceLoaderContainer SetAwaiter(Func<Task>? awaiter) {
            _awaiter = awaiter;
            return this;
        }

        public ResourceLoaderContainer OnProgress(Action<LoadingContext> progress) {
            _onProgressList ??= new LinkedList<Action<LoadingContext>>();
            _onProgressList.AddLast(progress);
            return this;
        }

        public ResourceLoaderContainer Bind(params object[] targets) {
            foreach (var target in targets)
                if (!_targets.Contains(target))
                    _targets.AddLast(target);
            return this;
        }

        public ResourceLoaderContainer Unbind(params object[] targets) {
            foreach (var target in targets) _targets.Remove(target);
            return this;
        }

        public async Task Load(params object[] moreTargets) {
            var progress = CombineOnProgress(_onProgressList);
            Func<Task> awaiter = _awaiter ?? (async () => await SceneTree.AwaitIdleFrame());
            IEnumerable<object> targets = _targets.Concat(moreTargets);
            _registry = await Loader.Load(GetResourcesToLoad(targets), progress, awaiter, _maxTime);
            InjectResources(_registry, targets);
        }

        public ResourceLoaderContainer Unload() {
            foreach (var key in _registry.Keys) {
                _registry[key].Resource.Dispose();
                _registry.Remove(key);
            }
            // Set to null all fields with attributes [Scene] and [Resource]
            foreach (var setter in GetType().GetPropertiesAndFields<SceneAttribute>(Flags))
                setter.SetValue(this, null);

            foreach (var setter in GetType().GetPropertiesAndFields<ResourceAttribute>(Flags))
                setter.SetValue(this, null);
            return this;
        }

        private static Action<LoadingContext>? CombineOnProgress(IEnumerable<Action<LoadingContext>>? progress) {
            if (progress == null) return null;
            void OnProgress(LoadingContext context) {
                foreach (var action in progress) action(context);
            }
            return OnProgress;
        }

        private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        private static IEnumerable<string> GetResourcesToLoad(IEnumerable<object> targets) {
            var resources = new HashSet<string>();
            foreach (var target in targets) {
                foreach (var setter in target.GetType().GetPropertiesAndFields<SceneAttribute>(Flags))
                    resources.Add(setter.Attribute.Path);
                
                foreach (var setter in target.GetType().GetPropertiesAndFields<ResourceAttribute>(Flags))
                    resources.Add(setter.Attribute.Path);
            }
            return resources;
        }

        private static void InjectResources(IReadOnlyDictionary<string, ResourceMetadata> resources, IEnumerable<object> targets) {
            foreach (var target in targets) {
                foreach (var setter in target.GetType().GetPropertiesAndFields<SceneAttribute>(Flags))
                    InjectScene(target, setter, resources[setter.Attribute.Path]);
                
                foreach (var setter in target.GetType().GetPropertiesAndFields<ResourceAttribute>(Flags))
                    InjectResource(target, setter, resources[setter.Attribute.Path]);
            }
        }

        private static Delegate CreatePackedSceneToInstanceFunction(PackedScene resource, Type returnType) {
            var closure = (Delegate)ComposeMethod.MakeGenericMethod(returnType.GetGenericArguments()[0])
                .Invoke(null, new[] { resource });
            return closure;
        }

        private static Func<T> PackedSceneToInstance<T>(PackedScene value) where T : class => () => value.Instance<T>();

        private static readonly MethodInfo ComposeMethod = typeof(ResourceLoaderContainer)
            .GetMethod(nameof(PackedSceneToInstance), BindingFlags.Static | BindingFlags.NonPublic)!;

        private static void InjectResource(object target, GetterSetter<ResourceAttribute> getterSetter, ResourceMetadata resource) {
            if (getterSetter.Type == typeof(ResourceMetadata)) {
                getterSetter.SetValue(target, resource);
            } else if (getterSetter.Type.IsGenericType &&
                       getterSetter.Type.GetGenericTypeDefinition() == typeof(ResourceMetadata<>)) {
                var genericType = getterSetter.Type.GetGenericArguments()[0];
                if (genericType.IsInstanceOfType(resource.Resource)) {
                    getterSetter.SetValue(target, ResourceMetadata.DynamicConstructor(resource, genericType));
                } else {
                    throw new ResourceLoaderException("Incompatible type ResourceMetadata<" + getterSetter.Type + "> for " +
                                                      resource.Resource.GetType() + ": " + resource.Path);
                }
            } else if (getterSetter.Type.IsInstanceOfType(resource.Resource)) {
                getterSetter.SetValue(target, resource.Resource);
            } else {
                throw new ResourceLoaderException("Incompatible type " + getterSetter.Type + " for " +
                                                  resource.Resource.GetType() + ": " + resource.Path);
            }
        }

        private static void InjectScene(object target, GetterSetter<SceneAttribute> getterSetter, ResourceMetadata resource) {
            if (!(resource.Resource is PackedScene packedScene)) {
                throw new ResourceLoaderException("Resource type: " + resource.Resource.GetType() +
                                                  " should be a PackedScene. " + resource.Path);
            }
            if (getterSetter.Type.IsGenericType && getterSetter.Type.GetGenericTypeDefinition() == typeof(Func<>)) {
                var packedSceneToInstanceFunction = CreatePackedSceneToInstanceFunction(packedScene, getterSetter.Type);
                getterSetter.SetValue(target, packedSceneToInstanceFunction);
            } else if (typeof(Node).IsAssignableFrom(getterSetter.Type)) {
                Node o = packedScene.Instance();
                if (getterSetter.Type.IsInstanceOfType(o)) {
                    getterSetter.SetValue(target, o);
                } else {
                    throw new ResourceLoaderException("Scene " + o.GetType() + " created should be " + getterSetter.Type +
                                                      " for " + resource.Path);
                }
            } else {
                throw new ResourceLoaderException("Incompatible type " + getterSetter.Type + " for " +
                                                  resource.Resource.GetType() + ": " + resource.Path);
            }
        }
    }
}