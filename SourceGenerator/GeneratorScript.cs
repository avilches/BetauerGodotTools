using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Generator {
    /**
     * Based on: https://github.com/semickolon/GodotRx/blob/6bf1a1ba9ffbe939e888d8d545c8d448a1f07bce/addons/godotrx/codegen.js
     * 
     * call with -s GeneratorScript.cs --no-window
     */
    public class GeneratorScript : SceneTree {
        public override void _Initialize() {
            while (Root.GetChildCount() > 0) Root.RemoveChild(Root.GetChild(0));
            var classes = LoadGodotClasses();
            GenerateGodotActionClasses.WriteAllActionClasses(classes);
            GenerateSignalHandlerExtensions.WriteSignalHandlerExtensionsClass(classes);
            GenerateGodotActionsExtensions.WriteGodotActionExtensionsClass(classes);
            GenerateSignalConstants.WriteSignalConstantsClass(classes);
            Quit(0);
        }

        private static List<GodotClass> LoadGodotClasses() {
            var classes = ClassDB.GetClassList()
                .Select(className => new GodotClass(className))
                .Where(godotClass => godotClass.IsValid)
                .OrderBy(godotClass => godotClass.class_name)
                .ToList();
            return classes;
        }
    }
}