using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Godot;
using NUnit.Framework;

namespace Veronenger.Tests.Runner {
    public class TestRunner {
        public delegate void TestResultDelegate(TestMethod testResult);

        private readonly bool _requireTestFixture;
        private readonly List<TestMethod> _testMethods = new List<TestMethod>();
        public int TestsTotal = 0;
        public int TestsExecuted = 0;
        public int TestsFailed = 0;
        public int TestsPassed = 0;

        private const int Delay = 50;

        public TestRunner(bool requireTestFixture = true) {
            _requireTestFixture = requireTestFixture;
            ScanTestsFromAssemblies();
        }

        public enum Result {
            Passed,
            Failed
        }

        public class TestMethod {
            private readonly object _instance;

            public MethodInfo Method { get; }
            public Type Type { get; }
            public string Name { get; }
            public Exception Exception { get; private set; }
            public Result Result { get; private set; }

            public MethodInfo Setup { get; set; }
            public MethodInfo TearDown { get; set; }

            public TestMethod(MethodInfo method, object instance) {
                _instance = instance;
                Method = method;
                Type = method.DeclaringType;
                Name = method.Name;
            }

            public async Task Execute(SceneTree sceneTree) {
                try {
                    if (_instance is Node node) {
                        await Task.Delay(Delay);
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
                                await Task.Delay(Delay);
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
                    await Task.Delay(Delay);
                }
            }
        }

        public async Task Run(SceneTree sceneTree, TestResultDelegate resultCallback = null) {
            TestsFailed = 0;
            TestsPassed = 0;
            TestsExecuted = 0;
            TestsTotal = _testMethods.Count;
            for (var i = 0; i < TestsTotal; i++) {
                TestsExecuted++;
                var testMethod = _testMethods[i];
                await testMethod.Execute(sceneTree);
                if (testMethod.Result == Result.Passed) {
                    TestsPassed++;
                } else {
                    TestsFailed++;
                }
                resultCallback?.Invoke(testMethod);
                await Task.Delay(20);
            }
        }

        private void ScanTestsFromAssemblies() {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies) {
                IterateThroughTypes(assembly.GetTypes());
            }
        }

        private void IterateThroughTypes(Type[] types) {
            Dictionary<Type, MethodInfo> setupMethods = new Dictionary<Type, MethodInfo>();
            Dictionary<Type, MethodInfo> teardownMethods = new Dictionary<Type, MethodInfo>();

            foreach (Type type in types) {
                if (Attribute.GetCustomAttribute(type, typeof(IgnoreAttribute), false) is IgnoreAttribute) {
                    continue;
                }
                if (_requireTestFixture &&
                    !(Attribute.GetCustomAttribute(type, typeof(TestFixtureAttribute), false) is TestFixtureAttribute))
                    continue;

                MethodInfo[] methods = type.GetMethods();
                foreach (var method in methods) {
                    if (Attribute.GetCustomAttribute(method, typeof(TestAttribute), false) is TestAttribute
                        testAttribute) {
                        // TODO: add the testAttribute.Description
                        try {
                            if (Attribute.GetCustomAttribute(method, typeof(IgnoreAttribute),
                                false) is IgnoreAttribute) {
                                // TODO: show the method is ignored
                                continue;
                            }

                            ConstructorInfo[] constructors = type.GetConstructors();
                            object curTestObject = null;
                            if (constructors.Length > 0) {
                                curTestObject = constructors[0].Invoke(null);
                            }

                            if (curTestObject != null) {
                                _testMethods.Add(new TestMethod(method, curTestObject));
                            }
                        } catch {
                            // Fail the test here?
                        }
                    } else if (Attribute.GetCustomAttribute(method, typeof(SetUpAttribute), false) is SetUpAttribute) {
                        setupMethods.Add(type, method);
                    } else if (Attribute.GetCustomAttribute(method, typeof(TearDownAttribute), false) is
                        TearDownAttribute) {
                        teardownMethods.Add(type, method);
                    }
                }
            }
            foreach (var testMethod in _testMethods) {
                if (setupMethods.ContainsKey(testMethod.Type)) {
                    testMethod.Setup = setupMethods[testMethod.Type];
                }
                if (teardownMethods.ContainsKey(testMethod.Type)) {
                    testMethod.TearDown = teardownMethods[testMethod.Type];
                }
            }
        }
    }
}