using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;
using Betauer.Core;
using Betauer.Tools.FastReflection;
using Godot;

namespace Betauer.NodePath; 

public static class NodePathScanner {

    private static readonly StringName MetaInjected = "__node_path_injected";

    private const BindingFlags NodePathFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    public static void ConfigureAutoInject(SceneTree sceneTree) {
        sceneTree.NodeAdded += node => ScanAndInject(node);
    }

    private static void SetAlreadyInjected(Node node) => node.SetMeta(MetaInjected, true);
    private static bool IsInjected(Node node) => node.HasMeta(MetaInjected);

    public static void ScanAndInject(Node target, bool force = false) {
        if (target.GetScript().AsGodotObject() is not CSharpScript) return;
        if (!force && IsInjected(target)) return;
        
        var type = target.GetType();
        foreach (var setter in type.GetSettersCached<NodePathAttribute>(MemberTypes.Field | MemberTypes.Property, NodePathFlags))
            LoadNodePathField(target, setter);
        SetAlreadyInjected(target); // This avoid nodes are injected twice if they are added to the tree later
    }

    private static void LoadNodePathField(Node target, ISetter<NodePathAttribute> getterSetter) {
        var nullable = getterSetter.SetterAttribute.Nullable; 
        var path = getterSetter.SetterAttribute.Path?.Trim(); 
        if (path == null) return;
        // [NodePath("path/to/node")
        // private Sprite2D sprite = this.GetNode<Sprite2D>("path/to/node");
        var node = target.GetNode(path);
        string FieldInfo() => $"[NodePath(\"{path}\")] {getterSetter.MemberType.GetTypeName()} {getterSetter.Name}";

        if (node == null) {
            if (nullable) return;
            throw new NodePathFieldException(getterSetter.Name, target, $"Path returns a null value for field {FieldInfo()}, class {target.GetType().Name}");
        }
        
        if (getterSetter.MemberType.IsArray) {
            var elementType = getterSetter.MemberType.GetElementType()!;
            var nodeArray = node.GetChildren().Where(child => elementType.IsInstanceOfType(child)).ToArray();
            var elementArray = Array.CreateInstance(elementType, nodeArray.Length);
            Array.Copy(nodeArray, elementArray, nodeArray.Length);                
            getterSetter.SetValue(target, elementArray);
                
        } else if (getterSetter.MemberType.ImplementsInterface(typeof(IList<>))) {
            var valueType = getterSetter.MemberType.GenericTypeArguments[0];
            var list = (IList)Activator.CreateInstance(getterSetter.MemberType)!;
            foreach (var child in node.GetChildren()) {
                if (valueType == null || valueType.IsInstanceOfType(child)) list.Add(child);
            }
            getterSetter.SetValue(target, list);

        } else if (getterSetter.MemberType.ImplementsInterface(typeof(IDictionary<,>))) {
            var keyType = getterSetter.MemberType.GenericTypeArguments[0];
            if (keyType != typeof(string)) 
                throw new NodePathFieldException(getterSetter.Name, target, $"IDictionary compatible type {node.GetType().Name} for field {FieldInfo()}, class {target.GetType().Name} only accepts string as key: {getterSetter.MemberType.GenericTypeArguments[0]}");
                
            var valueType = getterSetter.MemberType.GenericTypeArguments[1];
            IDictionary dictionary = (IDictionary)Activator.CreateInstance(getterSetter.MemberType)!;
            foreach (var child in node.GetChildren()) {
                if (valueType == null || valueType.IsInstanceOfType(child)) dictionary[(string)child.Name] = child; 
            }
            getterSetter.SetValue(target, dictionary);

        } else if (getterSetter.MemberType.IsInstanceOfType(node)) {
            getterSetter.SetValue(target, node);
                
        } else {
            throw new NodePathFieldException(getterSetter.Name, target,
                $"Path returns an incompatible type {node.GetType().Name} for field {FieldInfo()}, class {target.GetType().Name}");
        }
    }
}