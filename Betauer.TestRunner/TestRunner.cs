using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Godot;

namespace Betauer.TestRunner; 

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class OnlyAttribute : Attribute {
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class TestAttribute : Attribute {
    /// <summary>
    /// Descriptive text for this test
    /// </summary>
    public string? Description { get; set; }

    public bool Only { get; set; } = false;
    public bool Ignore { get; set; } = false;
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class IgnoreAttribute : Attribute {
    /// <summary>
    /// Descriptive text for this test
    /// </summary>
    public string Reason { get; set; }

    public IgnoreAttribute(string reason) {
        Reason = reason;
    }
}

[AttributeUsage(AttributeTargets.Method)]
public class SetUpClassAttribute : Attribute {
}

[AttributeUsage(AttributeTargets.Method)]
public class SetUpAttribute : Attribute {
}

[AttributeUsage(AttributeTargets.Method)]
public class TearDownClassAttribute : Attribute {
}

[AttributeUsage(AttributeTargets.Method)]
public class TearDownAttribute : Attribute {
}

public static class TestExtensions {
    internal static SceneTree SceneTree = null!;

    public static SignalAwaiter AwaitPhysicsFrame(this object _) =>
        AwaitPhysicsFrame();
        
    public static SignalAwaiter AwaitPhysicsFrame() =>
        SceneTree.ToSignal(SceneTree, "physics_frame");

    public static SignalAwaiter AwaitProcessFrame(this object _) =>
        AwaitProcessFrame();
        
    public static SignalAwaiter AwaitProcessFrame() =>
        SceneTree.ToSignal(SceneTree, "process_frame");
        
}

public class TestReport {
    public int TestsTotal { get; internal set; } = 0;
    public int TestsExecuted { get; internal set; } = 0;
    public int TestsFailed { get; internal set; } = 0;
    public int TestsPassed { get; internal set; } = 0;
    public List<TestRunner.TestMethod> TestsFailedResults { get;  } = new();
}

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
        public List<MethodInfo>? SetupClass { get; internal set; }
        public List<MethodInfo>? TearDownClass { get; internal set; }

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
            if (SetupClass != null) foreach (var methodInfo in SetupClass) methodInfo.Invoke(testClassInstance, Array.Empty<object>());

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

            if (TearDownClass != null) foreach (var methodInfo in TearDownClass) methodInfo.Invoke(testClassInstance, Array.Empty<object>());

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
                if (GetAttribute<TestAttribute>(type) is { Ignore: false } && !HasAttribute<IgnoreAttribute>(type)) {
                    GD.Print($"+ Test class {type}");
                    var testClass = CreateTestClass(type);
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

    private static TestClass CreateTestClass(Type type) {
        List<TestMethod> testMethods = new();
        List<MethodInfo>? setupClass = null;
        List<MethodInfo>? setup = null;
        List<MethodInfo>? tearDownClass = null;
        List<MethodInfo>? tearDown = null;
        var isAnyMethodWithOnly = false;
        var onlyThisType = HasAttribute<OnlyAttribute>(type) || GetAttribute<TestAttribute>(type)!.Only;
        var testClass = new TestClass(type);
        foreach (var method in type.GetMethods()) {
            var testAttribute = GetAttribute<TestAttribute>(method);
            if (testAttribute != null) {
                if (HasAttribute<IgnoreAttribute>(method) || testAttribute.Ignore) continue;
                var onlyThisMethod = HasAttribute<OnlyAttribute>(method);
                isAnyMethodWithOnly = isAnyMethodWithOnly || onlyThisMethod;
                var testMethod = new TestMethod(method, testClass, testAttribute.Description, onlyThisMethod);
                testMethods.Add(testMethod);
            } else {
                if (HasAttribute<SetUpClassAttribute>(method)) {
                    setupClass ??= new List<MethodInfo>();
                    setupClass.Add(method);
                }
                if (HasAttribute<TearDownClassAttribute>(method)) {
                    tearDownClass ??= new List<MethodInfo>();
                    tearDownClass.Add(method);
                }
                if (HasAttribute<SetUpAttribute>(method)) {
                    setup ??= new List<MethodInfo>();
                    setup.Add(method);
                }
                if (HasAttribute<TearDownAttribute>(method)) {
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
        testClass.SetupClass = setupClass;
        testClass.TearDownClass = tearDownClass;
        return testClass;
    }
    
    public static T? GetAttribute<T>(MemberInfo member, bool inherit = false) where T : Attribute {
        return Attribute.GetCustomAttribute(member, typeof(T), inherit) as T;
    }

    public static bool HasAttribute<T>(MemberInfo member, bool inherit = false) where T : Attribute {
        return Attribute.GetCustomAttribute(member, typeof(T), inherit) is T;
    }
}