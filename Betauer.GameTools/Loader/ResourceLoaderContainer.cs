using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Betauer.Core;
using Betauer.DI;
using Betauer.Tools.Reflection;
using Betauer.Core.Signal;
using Godot;

namespace Betauer.Loader {
    public class ResourceLoaderContainer {
        public event Action<float>? OnProgress;
        public Func<Task>? Awaiter { get; private set; }
        protected Dictionary<string, Resource>? Registry;
        
        // Never change this private, the Container can't inject it if the current class inherits ResourceLoaderContainer 
        [Inject] protected SceneTree? SceneTree { get; set; }

        private readonly HashSet<object> _sources = new();

        public ResourceLoaderContainer(SceneTree sceneTree = null) {
            SceneTree = sceneTree;
            if (GetType() != typeof(ResourceLoaderContainer)) {
                // Only add itself if the class inherits ResourceLoaderContainer
                _sources.Add(this);
            }
            OnProgress += DoOnProgress;
        }

        public bool Contains(string res) => Registry?.ContainsKey(res) ?? false;
        public T Resource<T>(string res) where T : Resource => 
            Registry != null ? 
                (T)Registry[res]: 
                throw new KeyNotFoundException($"Resource with name {res} not found");
        
        public T? Scene<T>(string res) where T : class => Resource<PackedScene>(res).Instantiate<T>();

        public virtual void DoOnProgress(float progress) {
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
            Array.ForEach(sources, source => From(source));
            return this;
        }

        public ResourceLoaderContainer Inject(params object[] targets) {
            Array.ForEach(targets, target => Inject(target));
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
                if (SceneTree != null) await SceneTree.AwaitProcessFrame();
                else await Task.Delay(10);
            });
            Unload();
            var resources =
                await Loader.Load(resourcesPaths, awaiter, progress => OnProgress?.Invoke(progress));
            
            // Add resources to registry by Path                
            Registry = resources.ToDictionary(r => r.ResourcePath);

            // Inject the sources and create the name -> resource dictionary
            foreach (var source in _sources) {
                var loadSetters = source.GetType().GetSettersCached<LoadAttribute>(MemberFlags, Flags);
                foreach (var setter in loadSetters) {
                    var resource = Registry[setter.SetterAttribute.Path];
                    var o = Convert(resource, setter.Type);
                    setter.SetValue(source, o);
                    var resourceName = setter.SetterAttribute.ResourceName;
                    if (resourceName != null) {
                        if (Registry.ContainsKey(resourceName))
                            throw new ResourceLoaderException($"Duplicated resource name: {resourceName}");
                        // Add resources to registry by Name                
                        Registry[resourceName] = resource;
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
            IReadOnlyDictionary<string, Resource>? resourcesByName, 
            object target) {
            var loadSetters = target.GetType().GetSettersCached<ResourceAttribute>(MemberFlags, Flags);
            foreach (var setter in loadSetters) {
                var resourceName = setter.SetterAttribute.ResourceName;
                if (resourcesByName != null && resourcesByName.TryGetValue(resourceName, out var resource)) {
                    var o = Convert(resource, setter.Type);
                    setter.SetValue(target, o);
                } else {
                    throw new KeyNotFoundException($"Resource with name {resourceName} not found");
                }
            }
        }

        public static object Convert(Resource resource, Type type) {
            if (type.IsInstanceOfType(resource)) {
                // [Resource/Load] public Resource field // matching the Resource type with the resource loaded
                return resource;
                
            } else if (resource is PackedScene packedScene) {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Func<>)) {
                    // [Resource/Load] private Func<object> field
                    var packedSceneToInstanceFunction = CreatePackedScene.CreateFunc(packedScene, type);
                    return packedSceneToInstanceFunction;
                } else if (typeof(Node).IsAssignableFrom(type)) {
                    // [Resource/Load] private Node(or child of) field
                    Node nodeCreated = packedScene.Instantiate();
                    if (type.IsInstanceOfType(nodeCreated)) {
                        return nodeCreated;
                    }
                    throw new ResourceLoaderException(
                        $"Scene {nodeCreated.GetType()} created should be {type} for {packedScene.ResourcePath}");
                }
            }
            throw new ResourceLoaderException(
                $"Incompatible type {type} for {resource.GetType()}: {resource.ResourcePath}");
        }
        
        private static class CreatePackedScene {
            private static Func<T> PackedSceneToInstance<T>(PackedScene value) where T : class =>
                () => value.Instantiate<T>();

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