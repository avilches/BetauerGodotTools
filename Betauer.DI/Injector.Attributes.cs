using System;

namespace Betauer.DI {
    [AttributeUsage(AttributeTargets.Method  | AttributeTargets.Property)]
    public class InjectAttribute : Attribute {
        public bool Nullable { get; set; } = false;
        public string? Name { get; set; }

        public InjectAttribute() {
        }

        public InjectAttribute(string name) {
            Name = name;
        }
    }
}