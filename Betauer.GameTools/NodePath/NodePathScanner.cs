using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Betauer.Tools.Reflection;
using Godot;

namespace Betauer.NodePath; 

public class NodePathScanner {

    private static readonly StringName MetaInjected = "__node_path_injected";

    private const BindingFlags NodePathFlags =
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    public static void ConfigureAutoInject(SceneTree sceneTree) {
        sceneTree.NodeAdded += node => {
            if (node.GetScript().AsGodotObject() is CSharpScript) ScanAndInject(node);
        };
    }

    private static void SetAlreadyInjected(Node node) => node.SetMeta(MetaInjected, true);
    private static bool IsInjected(Node node) => node.HasMeta(MetaInjected);

    public static void ScanAndInject(Node target, bool force = false) {
        if (!force && IsInjected(target)) return;
        SetAlreadyInjected(target); // This avoid nodes are injected twice if they are added to the tree later
        
        var type = target.GetType();
        foreach (var setter in type.GetSettersCached<NodePathAttribute>(MemberTypes.Field | MemberTypes.Property, NodePathFlags))
            LoadNodePathField(target, setter);
    }

    private static void LoadNodePathField(Node target, ISetter<NodePathAttribute> getterSetter) {
        var nullable = getterSetter.SetterAttribute.Nullable; 
        var path = getterSetter.SetterAttribute.Path?.Trim(); 
        if (path == null) return;
        // [NodePath("path/to/node")
        // private Sprite2D sprite = this.GetNode<Sprite2D>("path/to/node");
        var node = target.GetNode(path);
        var fieldInfo = "[NodePath(\"" + path + "\")] " + getterSetter.Type.Name + " " +
                        getterSetter.Name;

        if (node == null) {
            if (nullable) return;
            throw new NodePathFieldException(getterSetter.Name, target,
                "Path returns a null value for field " + fieldInfo + ", class " + target.GetType().Name);
        }
        if (getterSetter.Type.IsArray) {
            var elementType = getterSetter.Type.GetElementType()!;
            var nodeArray = node.GetChildren().Cast<Node>().Where(child => elementType.IsInstanceOfType(child)).ToArray();
            var elementArray = Array.CreateInstance(elementType, nodeArray.Length);
            Array.Copy(nodeArray, elementArray, nodeArray.Length);                
            getterSetter.SetValue(target, elementArray);
                
        } else if (getterSetter.Type.ImplementsInterface(typeof(IList))) {
            IList list = (IList)Activator.CreateInstance(getterSetter.Type);
            var valueType = getterSetter.Type.IsGenericType ? getterSetter.Type.GenericTypeArguments[0] : null;
            foreach (var child in node.GetChildren()) {
                if (valueType == null || valueType.IsInstanceOfType(child)) list.Add(child);
            }
            getterSetter.SetValue(target, list);

        } else if (getterSetter.Type.ImplementsInterface(typeof(IDictionary))) {
            if (getterSetter.Type.IsGenericType &&
                getterSetter.Type.GenericTypeArguments[0] != typeof(string)) 
                throw new NodePathFieldException(getterSetter.Name, target,
                    $"IDictionary compatible type {node.GetType().Name} for field {fieldInfo}, class {target.GetType().Name} only accepts string as key: {getterSetter.Type.GenericTypeArguments[0]}");
                
            IDictionary dictionary = (IDictionary)Activator.CreateInstance(getterSetter.Type);
            var valueType = getterSetter.Type.IsGenericType ? getterSetter.Type.GenericTypeArguments[1] : null;
            foreach (var child in node.GetChildren()) {
                if (valueType == null || valueType.IsInstanceOfType(child)) dictionary[(string)child.Name] = child; 
            }
            getterSetter.SetValue(target, dictionary);

        } else if (getterSetter.Type.IsInstanceOfType(node)) {
            getterSetter.SetValue(target, node);
                
        } else {
            throw new NodePathFieldException(getterSetter.Name, target,
                $"Path returns an incompatible type {node.GetType().Name} for field {fieldInfo}, class {target.GetType().Name}");
        }
    }
        
}