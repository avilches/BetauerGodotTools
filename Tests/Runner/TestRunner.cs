using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Godot;
using NUnit.Framework;

namespace Veronenger.Tests.Runner {
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = false,
        Inherited = false)]
    public class OnlyAttribute : Attribute {
    }


    public class TestRunner {
        public delegate void TestResultDelegate(TestMethod testResult);

        private readonly bool _requireTestFixture;
        private readonly ICollection<TestMethod> _testMethods = new LinkedList<TestMethod>();
        public int TestsTotal = 0;
        public int TestsExecuted = 0;
        public int TestsFailed = 0;
        public int TestsPassed = 0;

        public TestRunner(bool requireTestFixture = true) {
            _requireTestFixture = requireTestFixture;
            ScanTestsFromAssemblies();
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
            private readonly object _instance;
            private readonly MethodInfo Method;

            public Type Type { get; }
            public string Name { get; }
            public string Description { get; }
            public Exception Exception { get; private set; }
            public Result Result { get; private set; }
            public bool Only { get; set; }

            public MethodInfo Setup { get; set; }
            public MethodInfo TearDown { get; set; }

            public TestMethod(MethodInfo method, object instance, string description, bool only) {
                _instance = instance;
                Method = method;
                Type = method.DeclaringType;
                Name = method.Name;
                Description = description;
                Only = only;
            }

            public async Task Execute(SceneTree sceneTree) {
                try {
                    if (_instance is Node node) {
                        await SafeWait(sceneTree);
                        sceneTree.Root.AddChild(node);
                    }
                    Setup?.Invoke(_instance, new object[] { });
                    var obj = Method.Invoke(_instance, new object[] { });
                    if (obj is Task task) {
                        await task;
                    } else if (obj is IEnumerator coroutine) {
                        while (coroutine.MoveNext()) {
                            var next = coroutine.Current;
                            if (next is Task coTask) {
                                await coTask;
                            } else {
                                await SafeWait(sceneTree);
                                await sceneTree.ToSignal(sceneTree, "physics_frame");
                            }
                        }
                    }
                    Result = Result.Passed;
                    TearDown?.Invoke(_instance, new object[] { });
                } catch (Exception e) {
                    Exception = e.InnerException ?? e;
                    Result = Result.Failed;
                    try {
                        TearDown?.Invoke(_instance, new object[] { });
                    } catch (Exception tearDownError) {
                        // ignore tearDown error in failed tests
                    }
                }
                if (_instance is Node node2) {
                    node2.QueueFree();
                    await SafeWait(sceneTree);
                }
            }

            private async Task SafeWait(SceneTree sceneTree) {
                await sceneTree.ToSignal(sceneTree, "idle_frame");
            }
        }



        public async Task Run(SceneTree sceneTree, TestResultDelegate resultCallback = null) {
            TestsFailed = 0;
            TestsPassed = 0;
            TestsExecuted = 0;
            TestsTotal = _testMethods.Count;
            foreach (var testMethod in _testMethods) {
                TestsExecuted++;
                GD.Print($"#{TestsExecuted}/{TestsTotal}: {testMethod.Type.Name}.{testMethod.Name} \"{testMethod.Description}\" ...");
                await sceneTree.ToSignal(sceneTree, "idle_frame");
                await testMethod.Execute(sceneTree);
                if (testMethod.Result == Result.Passed) {
                    TestsPassed++;
                } else {
                    TestsFailed++;
                }
                resultCallback?.Invoke(testMethod);
            }
        }

        private void ScanTestsFromAssemblies() {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies) {
                IterateThroughTypes(assembly.GetTypes());
            }
        }

        private void IterateThroughTypes(Type[] types) {
            var cleanNotOnly = false;
            List<TestFixture> fixtures = new List<TestFixture>();
            foreach (Type type in types) {
                if (Attribute.GetCustomAttribute(type, typeof(IgnoreAttribute), false) is IgnoreAttribute) {
                    continue;
                }
                if (_requireTestFixture &&
                    !(Attribute.GetCustomAttribute(type, typeof(TestFixtureAttribute), false) is TestFixtureAttribute))
                    continue;

                var onlyThisType = false;
                if (Attribute.GetCustomAttribute(type, typeof(OnlyAttribute), false) is OnlyAttribute) {
                    onlyThisType = true;
                    cleanNotOnly = true;
                }

                List<TestMethod> testMethods = new List<TestMethod>();
                MethodInfo setup = null;
                MethodInfo tearDown = null;
                var isAnyMethodWithOnly = false;
                foreach (var method in type.GetMethods()) {
                    if (Attribute.GetCustomAttribute(method, typeof(TestAttribute), false) is TestAttribute
                        testAttribute) {
                        try {
                            if (Attribute.GetCustomAttribute(method, typeof(IgnoreAttribute),
                                false) is IgnoreAttribute) {
                                continue;
                            }

                            ConstructorInfo[] constructors = type.GetConstructors();
                            object curTestObject = null;
                            if (constructors.Length > 0) {
                                curTestObject = constructors[0].Invoke(null);
                            }

                            if (curTestObject != null) {
                                var onlyThisMethod = false;
                                if (Attribute.GetCustomAttribute(method, typeof(OnlyAttribute),
                                    false) is OnlyAttribute) {
                                    onlyThisMethod = true;
                                    isAnyMethodWithOnly = true;
                                    cleanNotOnly = true;
                                }
                                testMethods.Add(new TestMethod(method, curTestObject,
                                    testAttribute.Description, onlyThisMethod));
                            }
                        } catch {
                            // Fail the test here?
                        }
                    } else if (Attribute.GetCustomAttribute(method, typeof(SetUpAttribute), false) is SetUpAttribute) {
                        setup = method;
                    } else if (Attribute.GetCustomAttribute(method, typeof(TearDownAttribute), false) is
                        TearDownAttribute) {
                        tearDown = method;
                    }
                }
                testMethods.ForEach(testMethod => {
                    testMethod.Setup = setup;
                    testMethod.TearDown = tearDown;
                });
                if (!isAnyMethodWithOnly && onlyThisType) {
                    // None of the methods has Only, but the Fixture has, so mark all methods as Only too
                    testMethods.ForEach(method => method.Only = true);
                } else if (isAnyMethodWithOnly && !onlyThisType) {
                    // At least one of the methods has Only, but the Fixture has not, so mark the Fixture as Only too
                    onlyThisType = true;
                }
                fixtures.Add(new TestFixture(type, onlyThisType, testMethods));
            }

            if (cleanNotOnly) {
                fixtures.RemoveAll(t => !t.Only);
                fixtures.ForEach(fixture => fixture.Methods.RemoveAll(m => !m.Only));
            }
            fixtures.ForEach(fixture => fixture.Methods.ForEach(testMethod => _testMethods.Add(testMethod)));
        }
    }
}