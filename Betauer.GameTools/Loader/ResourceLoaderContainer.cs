using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Betauer.DI;
using Betauer.Tools.Reflection;
using Betauer.Signal;
using Godot;

namespace Betauer.Loader {
    public class ResourceLoaderContainer {
        public event Action<LoadingProgress> OnProgress;
        public Func<Task>? Awaiter { get; private set; }
        public float MaxTime  { get; private set; } = 0.100f;
        protected Dictionary<string, ResourceMetadata>? Registry;
        
        // Never change this private, the Container can't inject it if the current class inherits ResourceLoaderContainer 
        [Inject] protected SceneTree? SceneTree { get; set; }

        private readonly HashSet<object> _sources = new HashSet<object>();

        public ResourceLoaderContainer(SceneTree sceneTree = null) {
            SceneTree = sceneTree;
            if (GetType() != typeof(ResourceLoaderContainer)) {
                // Only add itself if the class inherits ResourceLoaderContainer
                _sources.Add(this);
            }
            OnProgress += DoOnProgress;
        }

        public bool Contains(string res) => Registry?.ContainsKey(res) ?? false;
        public T Resource<T>(string res) where T : class => 
            Registry != null ? 
                (T)((object)Registry[res].Resource): 
                throw new KeyNotFoundException($"Resource with name {res} not found");
        
        public T? Scene<T>(string res) where T : class => Resource<PackedScene>(res).Instance<T>();

        public virtual void DoOnProgress(LoadingProgress progress) {
        }

        public ResourceLoaderContainer SetMaxPollTime(float maxTime) {
            MaxTime = maxTime;
            return this;
        }

        public ResourceLoaderContainer SetAwaiter(Func<Task>? awaiter) {
            Awaiter = awaiter;
            return this;
        }

        public ResourceLoaderContainer From(object source) {
            _sources.Add(source);
            return this;
        }

        public ResourceLoaderContainer Inject(object target) {
            InjectResources(Registry, target);
            return this;
        }

        public ResourceLoaderContainer From(params object[] sources) {
            sources.ForEach(source => From(source));
            return this;
        }

        public ResourceLoaderContainer Inject(params object[] targets) {
            targets.ForEach(target => Inject(target));
            return this;
        }

        private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        private const MemberTypes MemberFlags = MemberTypes.Field | MemberTypes.Property;

        public async Task Load() {
            if (_sources == null || _sources.Count == 0)
                throw new ResourceLoaderException("Can't load: no sources defines with From(...)");
            
            // Get all unique resource paths combined from all sources
            var resourcesPaths = _sources
                .SelectMany(source => source.GetType().GetSettersCached<LoadAttribute>(MemberFlags, Flags))
                .Select(setter => setter.SetterAttribute.Path)
                .ToHashSet();
            
            // Load resources
            Func<Task> awaiter = Awaiter ?? (async () => {
                if (SceneTree != null) await SceneTree.AwaitIdleFrame();
                else await Task.Delay(10);
            });
            Unload();
            var resources =
                await Loader.Load(resourcesPaths, progress => OnProgress?.Invoke(progress), awaiter, MaxTime);
            
            // Add resources to registry by Path                
            Registry = resources.ToDictionary(r => r.Path);

            // Inject the sources and create the name -> resourceMetadata dictionary
            foreach (var source in _sources) {
                var loadSetters = source.GetType().GetSettersCached<LoadAttribute>(MemberFlags, Flags);
                foreach (var setter in loadSetters) {
                    var resourceMetadata = Registry[setter.SetterAttribute.Path];
                    var o = Convert(resourceMetadata, setter.Type);
                    setter.SetValue(source, o);
                    var resourceName = setter.SetterAttribute.ResourceName;
                    if (resourceName != null) {
                        if (Registry.ContainsKey(resourceName))
                            throw new ResourceLoaderException($"Duplicated resource name: {resourceName}");
                        // Add resources to registry by Name                
                        Registry[resourceName] = resourceMetadata;
                    }
                }
            }
        }

        public ResourceLoaderContainer Unload() {
            Registry?.Values.ForEach(r => r.Dispose());
            Registry?.Clear();
            // Set to null all fields with attribute [Load]
            foreach (var source in _sources) {
                var loadSetters = source.GetType().GetSettersCached<LoadAttribute>(MemberFlags, Flags);
                foreach (var setter in loadSetters)
                    setter.SetValue(source, null);
            }
            return this;
        }

        private static void InjectResources(
            IReadOnlyDictionary<string, ResourceMetadata>? resourcesByName, 
            object target) {
            var resourceSetters = target.GetType().GetSettersCached<ResourceAttribute>(MemberFlags, Flags);
            foreach (var setter in resourceSetters) {
                var resourceName = setter.SetterAttribute.ResourceName;
                if (resourcesByName != null && resourcesByName.TryGetValue(resourceName, out var resourceMetadata)) {
                    var o = Convert(resourceMetadata, setter.Type);
                    setter.SetValue(target, o);
                } else {
                    throw new KeyNotFoundException($"Resource with name {resourceName} not found");
                }
            }
        }

        public static object Convert(ResourceMetadata resourceMetadata, Type type) {
            if (type == typeof(ResourceMetadata)) {
                // [Resource/Load] public ResourceMetadata field
                return resourceMetadata;
            }
            if (type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(ResourceMetadata<>)) {
                var genericType = type.GetGenericArguments()[0];
                // [Resource/Load] public ResourceMetadata<Resource or children> field
                if (genericType.IsInstanceOfType(resourceMetadata.Resource)) {
                    var resourceMetadataWithGeneric =
                        ResourceMetadata.CreateGenericResourceMetadata(resourceMetadata, genericType);
                    return resourceMetadataWithGeneric;
                } else {
                    // [Resource/Load] public ResourceMetadata<non resource type like flost> field
                    throw new ResourceLoaderException(
                        $"Incompatible type ResourceMetadata<{type}> for {resourceMetadata.Resource.GetType()}: {resourceMetadata.Path}");
                }
                
            } else if (type.IsInstanceOfType(resourceMetadata.Resource)) {
                // [Resource/Load] public Resource field // matching the Resource type with the resource loaded
                return resourceMetadata.Resource;
                
            } else if (resourceMetadata.Resource is PackedScene packedScene) {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Func<>)) {
                    // [Resource/Load] private Func<object> field
                    var packedSceneToInstanceFunction = CreatePackedScene.CreateFunc(packedScene, type);
                    return packedSceneToInstanceFunction;
                } else if (typeof(Node).IsAssignableFrom(type)) {
                    // [Resource/Load] private Node(or child of) field
                    Node nodeCreated = packedScene.Instance();
                    if (type.IsInstanceOfType(nodeCreated)) {
                        return nodeCreated;
                    }
                    throw new ResourceLoaderException(
                        $"Scene {nodeCreated.GetType()} created should be {type} for {packedScene.ResourcePath}");
                }
            }
            throw new ResourceLoaderException(
                $"Incompatible type {type} for {resourceMetadata.Resource.GetType()}: {resourceMetadata.Path}");
        }
        
        private static class CreatePackedScene {
            private static Func<T> PackedSceneToInstance<T>(PackedScene value) where T : class =>
                () => value.Instance<T>();

            private static readonly MethodInfo ComposeMethod = typeof(CreatePackedScene)
                .GetMethod(nameof(PackedSceneToInstance), BindingFlags.Static | BindingFlags.NonPublic)!;

            internal static Delegate CreateFunc(PackedScene resource, Type returnType) {
                var closure = (Delegate)ComposeMethod.MakeGenericMethod(returnType.GetGenericArguments()[0])
                    .Invoke(null, new[] { resource });
                return closure;
            }
        }
    }
}