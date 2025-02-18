using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Godot;

namespace Betauer.TestRunner;

public class TestRunner {
    private List<TestClass> _testClasses;
    private List<TestMethod> _testMethos;
    public SceneTree SceneTree;

    public void Load(params Assembly[] assemblies) {
        _testClasses = ScanTestClassesFromAssemblies(assemblies);
    }

    public enum Result {
        Passed,
        Failed
    }

    public event Action<TestReport, TestMethod>? OnStart = null;
    public event Action<TestReport, TestMethod>? OnResult = null;

    public static readonly object[] EmptyParameters = Array.Empty<object>();

    public async Task<TestReport> Run(SceneTree sceneTree) {
        TestExtensions.SceneTree = sceneTree;
        SceneTree = sceneTree;
        TestReport testReport = new() {
            TestsTotal = _testClasses.SelectMany(testClass => testClass.Methods).Count()
        };
        GD.Print($"Running {testReport.TestsTotal} tests");
        foreach (var testClass in _testClasses) {
            await testClass.Execute(this, testReport);
        }
        return testReport;
    }

    public class TestClass {
        public Type Type { get; }
        public bool Only { get; internal set; }
        public List<TestMethod> Methods { get; internal set; }
        public List<MethodInfo>? OneTimeSetUp { get; internal set; }
        public List<MethodInfo>? OneTimeTearDown { get; internal set; }

        public TestClass(Type type) {
            Type = type;
        }

        public async Task Execute(TestRunner testRunner, TestReport testReport) {
            var testClassInstance = Activator.CreateInstance(Type);
            Node? node = testClassInstance as Node;
            if (node != null) {
                testRunner.SceneTree.Root.AddChild(node);
                await TestExtensions.AwaitProcessFrame();
            }
            if (OneTimeSetUp != null)
                foreach (var methodInfo in OneTimeSetUp)
                    methodInfo.Invoke(testClassInstance, EmptyParameters);

            foreach (var testMethod in Methods) {
                testReport.TestsExecuted++;
                testMethod.Id = testReport.TestsExecuted.ToString();
                testRunner.OnStart?.Invoke(testReport, testMethod);
                await testMethod.Execute(testReport, testClassInstance);
                if (testMethod.Result == Result.Passed) {
                    testReport.TestsPassed++;
                } else {
                    testReport.TestsFailed++;
                    testReport.TestsFailedResults.Add(testMethod);
                }
                testRunner.OnResult?.Invoke(testReport, testMethod);
            }

            if (OneTimeTearDown != null)
                foreach (var methodInfo in OneTimeTearDown)
                    methodInfo.Invoke(testClassInstance, EmptyParameters);

            if (GodotObject.IsInstanceValid(node)) {
                node!.QueueFree();
                await TestExtensions.AwaitProcessFrame();
            }
        }
    }

    public class TestMethod {
        public readonly Stopwatch Stopwatch = new();
        public readonly MethodInfo Method;

        public TestClass TestClass { get; }
        public string Name { get; }
        public string? Description { get; }
        public Exception Exception { get; private set; }
        public Result Result { get; private set; }
        public bool Only { get; set; }

        public IEnumerable<MethodInfo>? Setup { get; set; }
        public IEnumerable<MethodInfo>? TearDown { get; set; }
        public string Id { get; set; }

        public object[]? Parameters { get; }
        public ParameterInfo[] MethodParameters => Method.GetParameters();

        public TestMethod(MethodInfo method, TestClass type, string? description, bool only, IEnumerable<MethodInfo>? setup, IEnumerable<MethodInfo>? tearDown, object[]? parameters = null) {
            TestClass = type;
            Method = method;
            Parameters = parameters;
            // El nombre ahora incluirá los parámetros si existen
            Name = BuildTestName(method.Name, parameters);
            Description = description;
            Only = only;
            Setup = setup;
            TearDown = tearDown;
        }

        private string BuildTestName(string methodName, object[]? parameters) {
            if (parameters == null || parameters.Length == 0) return methodName;
            var paramString = string.Join(", ", parameters.Select(p => p?.ToString() ?? "null"));
            return $"{methodName}({paramString})";
        }

        public async Task Execute(TestReport testReport, object? testClassInstance) {
            try {
                Stopwatch.Start();
                if (Setup != null)
                    foreach (var methodInfo in Setup)
                        methodInfo.Invoke(testClassInstance, EmptyParameters);

                // Modificado para usar los parámetros
                var obj = Parameters == null ? Method.Invoke(testClassInstance, EmptyParameters) : Method.Invoke(testClassInstance, Parameters);

                if (obj is Task task) {
                    await task;
                } else if (obj is IEnumerator coroutine) {
                    while (coroutine.MoveNext()) {
                        var next = coroutine.Current;
                        if (next is Task coTask) {
                            await coTask;
                        } else {
                            await TestExtensions.AwaitProcessFrame();
                        }
                    }
                }
                Result = Result.Passed;
                if (TearDown != null)
                    foreach (var methodInfo in TearDown)
                        methodInfo.Invoke(testClassInstance, EmptyParameters);
            } catch (Exception e) {
                Exception = e.InnerException ?? e;
                Result = Result.Failed;
            }
            Stopwatch.Stop();
        }
    }

    private List<TestClass> ScanTestClassesFromAssemblies(Assembly[]? assemblies = null) {
        Stopwatch st = new Stopwatch();
        st.Start();
        assemblies = assemblies == null || assemblies.Length == 0 ? AppDomain.CurrentDomain.GetAssemblies() : assemblies;
        var cleanNotOnly = false;
        var testClasses = new List<TestClass>();
        foreach (var assembly in assemblies) {
            GD.Print($"Scanning assembly {assembly}...");
            foreach (Type type in assembly.GetTypes()) {
                // if (GetAttribute<TestAttribute>(type) is { Ignore: false } && !HasAttribute<IgnoreAttribute>(type)) {
                var testClassAttribute = GetAttribute(type, "Test", "TestFixture", "TestClass");
                if (testClassAttribute != null && !HasAttribute(type, "Ignore", "Skip")) {
                    // GD.Print($"+ Test class {type}");
                    var testClass = CreateTestClass(type, testClassAttribute);
                    testClasses.Add(testClass);
                    cleanNotOnly = cleanNotOnly || testClass.Only;
                }
            }
        }
        if (cleanNotOnly) {
            testClasses.RemoveAll(testClass => !testClass.Only);
            testClasses.ForEach(testClass => testClass.Methods.RemoveAll(m => !m.Only));
        }
        st.Stop();
        GD.Print($"Added {testClasses.Sum((testClass => testClass.Methods.Count))} tests in {st.ElapsedMilliseconds}ms");
        return testClasses;
    }

    private record TestMethodDefinition(MethodInfo Method, string? Description, bool Skip, bool Only) {
        public static TestMethodDefinition? GetTestMethodAttribute(MethodInfo method) {
            var testAttribute = GetAttribute(method, "Test", "Fact", "TestMethod", "TestCase");
            if (testAttribute == null) return null;
            var skip = HasAttribute(method, "Ignore", "Skip") || HasAttributeField(testAttribute, "Ignore", "Skip");
            var only = HasAttribute(method, "Only") || HasAttributeField(testAttribute, "Only");
            return new TestMethodDefinition(method, GetFieldValue(testAttribute, "Description"), skip, only);
        }
    }

    private static TestClass CreateTestClass(Type type, Attribute testClassAttribute) {
        var testClass = new TestClass(type);
        List<TestMethod> testMethods = [];
        var isAnyMethodWithOnly = false;
        var onlyThisType = HasAttribute(type, "Only") || HasAttributeField(testClassAttribute, "Only");
        var setup = GetMethodsInHierarchicalOrder(type, "SetUp", "TestInitialize").ToList();
        var tearDown = GetMethodsInHierarchicalOrder(type, "TearDown", "TestCleanup").ToList();
        foreach (var (method, description, _, onlyThisMethod) in type.GetMethods()
                     .Select(TestMethodDefinition.GetTestMethodAttribute)
                     .Where(t => t != null)
                     .Select(t => t!)
                     .Where(t => !t.Skip)) {

            isAnyMethodWithOnly = isAnyMethodWithOnly || onlyThisMethod;

            var testCases = GetTestCaseAttributes(method).ToArray();
            if (testCases.Length > 0) {
                // Crear un TestMethod por cada TestCase
                foreach (var testCase in testCases) {
                    var parameters = GetTestCaseParameters(testCase, method);
                    var testCaseDescription = GetFieldValue(testCase, "Description");
                    testMethods.Add(new TestMethod(method, testClass, testCaseDescription ?? description, onlyThisMethod, setup, tearDown, parameters));
                }
            } else {
                testMethods.Add(new TestMethod(method, testClass, description, onlyThisMethod, setup, tearDown));
            }
        }
        if (!isAnyMethodWithOnly && onlyThisType) {
            // If none of the methods has Only, but the TestClass has Only, then mark all methods as Only too
            testMethods.ForEach(method => method.Only = true);
        } else if (isAnyMethodWithOnly && !onlyThisType) {
            // If at least one of the methods has Only, but the TestClass has not Only, mark the TestClass as Only too
            onlyThisType = true;
        }
        testClass.Only = onlyThisType;
        testClass.Methods = testMethods;
        testClass.OneTimeSetUp = GetMethodsInHierarchicalOrder(type, "OneTimeSetUp", "SetUpClass").ToList();
        testClass.OneTimeTearDown = GetMethodsInHierarchicalOrder(type, "OneTimeTearDown", "TearDownClass").ToList();
        return testClass;
    }

    private static IEnumerable<Attribute> GetTestCaseAttributes(MethodInfo method) {
        return Attribute.GetCustomAttributes(method, true)
            .Where(attr => attr.GetType().Name == "TestCaseAttribute");
    }

    private static object[]? GetTestCaseParameters(Attribute testCaseAttr, MethodInfo method) {
        // Obtener el array de argumentos del TestCase
        var argsProperty = testCaseAttr.GetType().GetProperty("Arguments");
        if (argsProperty == null) return null;

        var args = argsProperty.GetValue(testCaseAttr) as object[];
        if (args == null) return null;

        // Convertir los argumentos al tipo correcto según los parámetros del método
        var parameters = method.GetParameters();
        var convertedArgs = new object[parameters.Length];

        for (var i = 0; i < parameters.Length && i < args.Length; i++) {
            convertedArgs[i] = Convert.ChangeType(args[i], parameters[i].ParameterType);
        }

        return convertedArgs;
    }

    private static string? GetFieldValue(Attribute testAttribute, string field) {
        return testAttribute.GetType().GetField(field)?.GetValue(testAttribute) as string;
    }

    public static Attribute? GetAttribute(MemberInfo member, string name, bool inherit = false) {
        return Attribute.GetCustomAttributes(member, inherit).FirstOrDefault(a => a.GetType().Name == name + "Attribute");
    }

    public static Attribute? GetAttribute(MemberInfo member, params string[] name) {
        return Attribute.GetCustomAttributes(member, false).FirstOrDefault(a => name.Any(n => a.GetType().Name == n + "Attribute"));
    }

    public static T? GetAttribute<T>(MemberInfo member, bool inherit = false) where T : Attribute {
        return Attribute.GetCustomAttribute(member, typeof(T), inherit) as T;
    }

    public static bool HasAttribute(MemberInfo member, string name, bool inherit = false) {
        return Attribute.GetCustomAttributes(member, inherit).Any(a => a.GetType().Name == name + "Attribute");
    }

    public static bool HasAttribute(MemberInfo member, params string[] name) {
        return Attribute.GetCustomAttributes(member, false).Any(a => name.Any(n => a.GetType().Name == n + "Attribute"));
    }

    public static bool HasAttributeField(Attribute member, string propertyName, bool inherit = false) {
        var property = member.GetType().GetProperties(InheritFlags(inherit)).FirstOrDefault(f => f.Name == propertyName);
        if (property != null) {
            return member.GetType().GetProperty(propertyName, InheritFlags(inherit))?.GetValue(member) != null;
        }
        return false;
    }

    public static bool HasAttributeField(Attribute member, params string[] propertyNames) {
        foreach (var propertyName in propertyNames) {
            var property = member.GetType().GetProperties().FirstOrDefault(f => f.Name == propertyName);
            if (property != null && member.GetType().GetProperty(propertyName)?.GetValue(member) != null) {
                return true;
            }
        }
        return false;
    }

    public static bool HasAttribute<T>(MemberInfo member, bool inherit = false) where T : Attribute {
        return Attribute.GetCustomAttribute(member, typeof(T), inherit) is T;
    }

    public static BindingFlags InheritFlags(bool inherit) => inherit
        ? BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy
        : BindingFlags.Public | BindingFlags.Instance;

    public static IEnumerable<MethodInfo> GetMethodsInHierarchicalOrder(Type type, params string[] attributeNames) {
        var methods = new List<MethodInfo>();
        var currentType = type;

        // Recorremos la jerarquía desde la clase base hasta la clase derivada
        while (currentType != null && currentType != typeof(object)) {
            // Obtenemos los métodos declarados en este nivel
            var methodsInCurrentType = currentType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                .Where(method => HasAttribute(method, attributeNames))
                .ToList();

            // Los añadimos al principio de la lista para que los métodos de la clase base vayan primero
            methods.InsertRange(0, methodsInCurrentType);

            // Subimos en la jerarquía
            currentType = currentType.BaseType;
        }

        return methods;
    }
}