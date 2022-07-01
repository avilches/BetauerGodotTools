using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Generator {
    public class GenerateSignalConstants {
        private const string SignalConstantsFile = "../Betauer.Core/SignalConstants.cs";

        public static void WriteSignalConstantsClass(List<GodotClass> classes) {
            List<string> allMethods = classes
                .Where(godotClass => godotClass.Signals.Count > 0 &&
                                     !godotClass.IsEditor)
                .SelectMany(godotClass => godotClass.Signals)
                .Select(CreateSignalConstantField)
                .ToList();
            var bodySignalConstantsClass = CreateSignalConstantsClass(allMethods);
            Console.WriteLine($"Generated {System.IO.Path.GetFullPath(SignalConstantsFile)}");
            File.WriteAllText(SignalConstantsFile, bodySignalConstantsClass);
        }

        private static string CreateSignalConstantField(Signal signal) {
            return $@"        public const string {signal.SignalCsharpConstantName} = ""{signal.signal_name}"";";
        }

        private static string CreateSignalConstantsClass(IEnumerable<string> methods) {
            return $@"using System;
namespace Betauer {{
    public static partial class SignalConstants {{
{string.Join("\n", methods)}
    }}
}}";
        }
    }
}