using System;
using System.Collections.Generic;
using Godot;

namespace Tools {
    public class InjectAttribute : Attribute {
    }

    public class DiRepository {
        private readonly Dictionary<Type, object> _instancesByType = new Dictionary<Type, object>();

        public T AddSingleton<T>(T instance) {
            _instancesByType.Add(instance.GetType(), instance);
            return instance;
        }

        public void AutoWire(object target) {
            InjectFields(target);
        }

        public void InjectFields(object target) {
            foreach (var property in target.GetType().GetFields()) {
                if (!(Attribute.GetCustomAttribute(property, typeof(InjectAttribute), false) is InjectAttribute inject))
                    continue;
                var instance = _instancesByType[property.FieldType];
                property.SetValue(target, instance);
            }
        }

        public void Finish() {
            foreach (var instance in _instancesByType.Values) AutoWire(instance);
        }

    }

    public class GodotDiRepository : DiRepository{
        public static readonly GodotDiRepository Instance = new GodotDiRepository();

        private GodotDiRepository() {
        }
    }

    public class DiKinematicBody2D : KinematicBody2D {
        public DiKinematicBody2D() => GodotDiRepository.Instance.AutoWire(this);
    }

    public class Di {
        public Di() => GodotDiRepository.Instance.AutoWire(this);
    }

    public class DiNode : Node {
        public DiNode() => GodotDiRepository.Instance.AutoWire(this);
    }

    public class DiNode2D : Node2D {
        public DiNode2D() => GodotDiRepository.Instance.AutoWire(this);
    }

    public class DiCamera2D : Camera2D {
        public DiCamera2D() => GodotDiRepository.Instance.AutoWire(this);
    }

    public class DiArea2D : Area2D {
        public DiArea2D() => GodotDiRepository.Instance.AutoWire(this);
    }
}