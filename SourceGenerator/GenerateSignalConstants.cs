using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Generator {
    public class GenerateSignalConstants {
        private const string FileName = "../Betauer.Core/Signal/SignalConstants.cs";

        public static void Write(List<GodotClass> classes) {
            List<string> allMethods = classes
                .Where(godotClass => godotClass.Signals.Count > 0)
                .SelectMany(godotClass => godotClass.Signals)
                .Select(GenerateConst)
                .ToList();
            var bodySignalConstantsClass = GenerateBodyClass(allMethods);
            Console.WriteLine($"Generated {System.IO.Path.GetFullPath(FileName)}");
            File.WriteAllText(FileName, bodySignalConstantsClass);
        }

        private static string GenerateConst(Signal signal) {
            return $@"        public const string {signal.SignalCsharpConstantName} = ""{signal.signal_name}"";";
        }

        private static string GenerateBodyClass(IEnumerable<string> methods) {
            return $@"using System;
namespace Betauer.Signal {{
    public static partial class SignalConstants {{
{string.Join("\n", methods)}
    }}
}}";
        }
    }
}