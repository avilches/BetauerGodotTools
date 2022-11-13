using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Godot;
using NUnit.Framework;

namespace Betauer.TestRunner {
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = false,
        Inherited = false)]
    public class OnlyAttribute : Attribute {
    }

    public static class TestExtensions {
        public static SceneTree SceneTree;

        public static SignalAwaiter AwaitPhysicsFrame(this object _) =>
            AwaitPhysicsFrame();
        
        public static SignalAwaiter AwaitPhysicsFrame() =>
            SceneTree.ToSignal(SceneTree, "physics_frame");

        public static SignalAwaiter AwaitProcessFrame(this object _) =>
            AwaitProcessFrame();
        
        public static SignalAwaiter AwaitProcessFrame() =>
            SceneTree.ToSignal(SceneTree, "process_frame");
        
    }

    public class TestRunner {
        private readonly List<TestMethod> _testMethods = new List<TestMethod>();
        public readonly List<TestMethod> TestsFailedResults = new List<TestMethod>();
        public int TestsTotal;
        public int TestsExecuted;
        public int TestsFailed;
        public int TestsPassed;

        public TestRunner(Assembly[]? assemblies = null) {
            ScanFixturesFromAssemblies(assemblies)
                .ForEach(fixture => fixture.Methods.ForEach(testMethod => _testMethods.Add(testMethod)));
            GD.Print("Loaded " + _testMethods.Count + " tests");
        }

        public enum Result {
            Passed,
            Failed
        }

        public class TestFixture {
            public readonly Type Type;
            public readonly bool Only;
            public readonly List<TestMethod> Methods;

            public TestFixture(Type type, bool only, List<TestMethod> methods) {
                Type = type;
                Only = only;
                Methods = methods;
            }
        }

        public class TestMethod {
            private static readonly object[] EmptyParameters = { };

            public readonly Stopwatch Stopwatch = new();

            public readonly MethodInfo Method;

            public Type FixtureType { get; }
            public string Name { get; }
            public string? Description { get; }
            public Exception Exception { get; private set; }
            public Result Result { get; private set; }
            public bool Only { get; set; }

            public IEnumerable<MethodInfo>? Setup { get; set; }
            public IEnumerable<MethodInfo>? TearDown { get; set; }
            public string Id { get; set; }

            public TestMethod(MethodInfo method, Type type, string? description, bool only) {
                FixtureType = type;
                Method = method;
                Name = method.Name;
                Description = description;
                Only = only;
            }

            public async Task Execute(SceneTree sceneTree) {
                object? fixtureInstance = null;
                Node? node = fixtureInstance as Node;
                try {
                    Stopwatch.Start();
                    fixtureInstance = Activator.CreateInstance(FixtureType);
                    node = fixtureInstance as Node;
                    if (node != null) {
                        sceneTree.Root.AddChild(node);
                        await TestExtensions.AwaitProcessFrame();
                    }
                    if (Setup != null) foreach (var methodInfo in Setup) methodInfo.Invoke(fixtureInstance, EmptyParameters);
                    var obj = Method.Invoke(fixtureInstance, EmptyParameters);
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
                    if (TearDown != null) foreach (var methodInfo in TearDown) methodInfo.Invoke(fixtureInstance, EmptyParameters);
                } catch (Exception e) {
                    Exception = e.InnerException ?? e;
                    Result = Result.Failed;
                    try {
                        if (TearDown != null && fixtureInstance != null) {
                            foreach (var methodInfo in TearDown) methodInfo.Invoke(fixtureInstance, EmptyParameters);
                        }
                    } catch (Exception) {
                        // ignore tearDown error in failed tests
                    }
                }
                if (Godot.Object.IsInstanceValid(node)) {
                    node.QueueFree();
                    await TestExtensions.AwaitProcessFrame();
                }
                Stopwatch.Stop();
            }
        }


        public async Task Run(SceneTree sceneTree, Action<TestMethod>? startCallback = null,
            Action<TestMethod>? resultCallback = null) {
            TestExtensions.SceneTree = sceneTree;
            TestsFailedResults.Clear();
            TestsFailed = 0;
            TestsPassed = 0;
            TestsExecuted = 0;
            TestsTotal = _testMethods.Count;
            foreach (var testMethod in _testMethods) {
                TestsExecuted++;
                testMethod.Id = TestsExecuted.ToString();
                startCallback?.Invoke(testMethod);
                await testMethod.Execute(sceneTree);
                if (testMethod.Result == Result.Passed) {
                    TestsPassed++;
                } else {
                    TestsFailed++;
                    TestsFailedResults.Add(testMethod);
                }
                resultCallback?.Invoke(testMethod);
            }
        }

        private List<TestFixture> ScanFixturesFromAssemblies(Assembly[]? assemblies = null) {
            Stopwatch st = new Stopwatch();
            st.Start();
            assemblies ??= AppDomain.CurrentDomain.GetAssemblies();
            var cleanNotOnly = false;
            var fixtures = new List<TestFixture>();
            foreach (var assembly in assemblies) {
                GD.Print($"Scanning assembly {assembly}...");
                foreach (Type type in assembly.GetTypes()) {
                    if (IsTestFixture(type)) {
                        GD.Print($"+ Fixture {type}");
                        var testFixture = CreateFixture(type);
                        fixtures.Add(testFixture);
                        cleanNotOnly = cleanNotOnly || testFixture.Only;
                    }
                }
            }
            if (cleanNotOnly) {
                fixtures.RemoveAll(t => !t.Only);
                fixtures.ForEach(fixture => fixture.Methods.RemoveAll(m => !m.Only));
            }
            st.Stop();
            GD.Print($"Added {fixtures.Sum((fixture => fixture.Methods.Count))} tests in {st.ElapsedMilliseconds}ms");
            return fixtures;
        }

        private bool IsTestFixture(Type type) {
            if (Attribute.GetCustomAttribute(type, typeof(TestFixtureAttribute), false) is TestFixtureAttribute) {
                return !(Attribute.GetCustomAttribute(type, typeof(IgnoreAttribute), false) is IgnoreAttribute);
            }
            return false;
        }

        private static TestFixture CreateFixture(Type type) {
            var onlyThisType = Attribute.GetCustomAttribute(type, typeof(OnlyAttribute), false) is OnlyAttribute;
            var testMethods = new List<TestMethod>();
            var setup = new List<MethodInfo>();
            var tearDown = new List<MethodInfo>();
            var isAnyMethodWithOnly = false;
            foreach (var method in type.GetMethods()) {
                if (Attribute.GetCustomAttribute(method, typeof(TestAttribute), false) is TestAttribute testAttribute) {
                    try {
                        if (Attribute.GetCustomAttribute(method, typeof(IgnoreAttribute), false) is IgnoreAttribute) {
                            continue;
                        }

                        var onlyThisMethod = false;
                        if (Attribute.GetCustomAttribute(method, typeof(OnlyAttribute),
                                false) is OnlyAttribute) {
                            onlyThisMethod = true;
                            isAnyMethodWithOnly = true;
                        }
                        testMethods.Add(new TestMethod(method, type, testAttribute.Description, onlyThisMethod));
                    } catch {
                        // Fail the test here?
                    }
                } else {
                    if (Attribute.GetCustomAttribute(method, typeof(SetUpAttribute),false) is SetUpAttribute)
                        setup.Add(method);
                    if (Attribute.GetCustomAttribute(method, typeof(TearDownAttribute), false) is TearDownAttribute)
                        tearDown.Add(method);
                }
            }
            testMethods.ForEach(testMethod => {
                if (setup.Count > 0) testMethod.Setup = setup;
                if (tearDown.Count > 0) testMethod.TearDown = tearDown;
            });
            if (!isAnyMethodWithOnly && onlyThisType) {
                // If none of the methods has Only, but the Fixture has Only, then mark all methods as Only too
                testMethods.ForEach(method => method.Only = true);
            } else if (isAnyMethodWithOnly && !onlyThisType) {
                // If at least one of the methods has Only, but the Fixture has not Only, mark the Fixture as Only too
                onlyThisType = true;
            }
            return new TestFixture(type, onlyThisType, testMethods);
        }
    }
}