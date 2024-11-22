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

    public async Task<TestReport> Run(SceneTree sceneTree) {
        TestExtensions.SceneTree = sceneTree;
        SceneTree = sceneTree;
        TestReport testReport = new () {
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
            if (OneTimeSetUp != null) foreach (var methodInfo in OneTimeSetUp) methodInfo.Invoke(testClassInstance, Array.Empty<object>());

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

            if (OneTimeTearDown != null) foreach (var methodInfo in OneTimeTearDown) methodInfo.Invoke(testClassInstance, Array.Empty<object>());

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

        public TestMethod(MethodInfo method, TestClass type, string? description, bool only) {
            TestClass = type;
            Method = method;
            Name = method.Name;
            Description = description;
            Only = only;
        }

        public async Task Execute(TestReport testReport, object? testClassInstance) {
            try {
                Stopwatch.Start();
                if (Setup != null) foreach (var methodInfo in Setup) methodInfo.Invoke(testClassInstance, Array.Empty<object>());
                var obj = Method.Invoke(testClassInstance, Array.Empty<object>());
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
                if (TearDown != null) foreach (var methodInfo in TearDown) methodInfo.Invoke(testClassInstance, Array.Empty<object>());
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
                var testClassAttribute = GetAttribute(type, "Test") ?? GetAttribute(type, "TestFixture") ?? GetAttribute(type, "TestClass");
                if (testClassAttribute != null && !HasAttribute(type, "Ignore") && !HasAttribute(type, "Skip")) {
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

    private static TestClass CreateTestClass(Type type, Attribute testClassAttribute) {
        List<TestMethod> testMethods = new();
        List<MethodInfo>? oneTimeSetUp = null;
        List<MethodInfo>? setup = null;
        List<MethodInfo>? oneTimeTearDown = null;
        List<MethodInfo>? tearDown = null;
        var isAnyMethodWithOnly = false;
        var onlyThisType =  HasAttribute(type, "Only") || HasAttributeField(testClassAttribute, "Only");
        var testClass = new TestClass(type);
        foreach (var method in type.GetMethods()) {
            var testAttribute = GetAttribute(method, "Test") ?? GetAttribute(method, "Fact") ?? GetAttribute(method, "TestMethod");
            if (testAttribute != null) {
                if (HasAttribute(method, "Ignore") ||
                    HasAttribute(method, "Skip") || 
                    HasAttributeField(testAttribute, "Skip") || 
                    HasAttributeField(testAttribute, "Skip")) continue;
                var onlyThisMethod = HasAttribute(method, "Only") || HasAttributeField(testAttribute, "Only");
                isAnyMethodWithOnly = isAnyMethodWithOnly || onlyThisMethod;
                var testMethod = new TestMethod(method, testClass, GetFieldValue(testAttribute, "Description"), onlyThisMethod);
                testMethods.Add(testMethod);
            } else {
                if (HasAttribute(method, "OneTimeSetUp") || HasAttribute(method, "SetUpClass")) {
                    oneTimeSetUp ??= new List<MethodInfo>();
                    oneTimeSetUp.Add(method);
                }
                if (HasAttribute(method, "OneTimeTearDown") || HasAttribute(method, "TearDownClass")) {
                    oneTimeTearDown ??= new List<MethodInfo>();
                    oneTimeTearDown.Add(method);
                }
                if (HasAttribute(method, "SetUp") || HasAttribute(method, "TestInitialize")) {
                    setup ??= new List<MethodInfo>();
                    setup.Add(method);
                }
                if (HasAttribute(method, "TearDown") || HasAttribute(method, "TestCleanup")) {
                    tearDown ??= new List<MethodInfo>();
                    tearDown.Add(method);
                }
            }
        }
        testMethods.ForEach(testMethod => {
            testMethod.Setup = setup;
            testMethod.TearDown = tearDown;
        });
        if (!isAnyMethodWithOnly && onlyThisType) {
            // If none of the methods has Only, but the TestClass has Only, then mark all methods as Only too
            testMethods.ForEach(method => method.Only = true);
        } else if (isAnyMethodWithOnly && !onlyThisType) {
            // If at least one of the methods has Only, but the TestClass has not Only, mark the TestClass as Only too
            onlyThisType = true;
        }
        testClass.Only = onlyThisType;
        testClass.Methods = testMethods;
        testClass.OneTimeSetUp = oneTimeSetUp;
        testClass.OneTimeTearDown = oneTimeTearDown;
        return testClass;
    }

    private static string? GetFieldValue(Attribute testAttribute, string field) {
        return testAttribute.GetType().GetField(field)?.GetValue(testAttribute) as string;
    }

    public static Attribute? GetAttribute(MemberInfo member, string name, bool inherit = false) {
        return Attribute.GetCustomAttributes(member, inherit).FirstOrDefault(a => a.GetType().Name == name+"Attribute");
    }

    public static T? GetAttribute<T>(MemberInfo member, bool inherit = false) where T : Attribute {
        return Attribute.GetCustomAttribute(member, typeof(T), inherit) as T;
    }

    public static bool HasAttribute(MemberInfo member, string name, bool inherit = false) {
        return Attribute.GetCustomAttributes(member, inherit).Any(a => a.GetType().Name == name+"Attribute");
    }
    
    public static bool HasAttributeField(Attribute member, string propertyName, bool inherit = false) {
        var property = member.GetType().GetProperties().FirstOrDefault(f=> f.Name == propertyName);
        if (property != null) {
            return member.GetType().GetProperty(propertyName)?.GetValue(member) != null;
        }
        return false;
    }
    
    public static bool HasAttribute<T>(MemberInfo member, bool inherit = false) where T : Attribute {
        return Attribute.GetCustomAttribute(member, typeof(T), inherit) is T;
    }
}