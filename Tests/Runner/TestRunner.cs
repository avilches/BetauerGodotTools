using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Godot;
using NUnit.Framework;

namespace Veronenger.Tests.Runner {
    public class TestRunner {
        bool requireTestFixture;

        public int currentTest { get; private set; }

        public int testCount {
            get {
                if (testMethods != null) {
                    return testMethods.Count;
                }

                return 0;
            }
        }

        SceneTree sceneTree;

        Dictionary<MethodInfo, object> testMethods = new Dictionary<MethodInfo, object>();
        Dictionary<Type, MethodInfo> setupMethods = new Dictionary<Type, MethodInfo>();
        Dictionary<Type, MethodInfo> teardownMethods = new Dictionary<Type, MethodInfo>();

        List<TestResult> testResults = new List<TestResult>();

        public TestResult[] TestResults {
            get { return testResults.ToArray(); }
        }

        public delegate void TestResultDelegate(TestResult testResult);

        public TestRunner(SceneTree sceneTree, bool requireTestFixture = true) {
            this.sceneTree = sceneTree;
            this.requireTestFixture = requireTestFixture;
            IterateThroughAssemblies();
        }

        public async Task Run(TestResultDelegate resultCallback = null) {
            int testsFailed = 0;
            int testsPassed = 0;
            foreach (MethodInfo method in testMethods.Keys) {
                object testObject = testMethods[method];

                TestResult testResult = new TestResult(method, null, TestResult.Result.Passed);
                GD.Print($"+++{testResult.classType.Name}.{testResult.testMethod.Name}");

                try {
                    if (testObject is Node node) {
                        await Task.Delay(100);
                        sceneTree.Root.AddChild(node);
                    }

                    if (setupMethods.ContainsKey(method.DeclaringType)) {
                        setupMethods[method.DeclaringType].Invoke(testObject, new object[] { });
                    }

                    object obj = method.Invoke(testObject, new object[] { });
                    if (obj is IEnumerator coroutine) {
                        while (coroutine.MoveNext()) {
                            await Task.Delay(100);
                        }
                    }
                    testsPassed++;
                } catch (Exception e) {
                    testsFailed++;
                    testResult = new TestResult(method, e.InnerException ?? e, TestResult.Result.Failed);
                    GD.Print(
                        $"*** Failed test: {testResult.classType.Name}.{testResult.testMethod.Name}\n{testResult.exception.Message}\n{testResult.exception.StackTrace}");
                }
                testResults.Add(testResult);
                if (teardownMethods.ContainsKey(method.DeclaringType)) {
                    teardownMethods[method.DeclaringType].Invoke(testObject, new object[] { });
                }

                if (testObject is Node node2) {
                    node2.QueueFree();
                }

                if (resultCallback != null) {
                    resultCallback(testResult);
                }

                await Task.Delay(20);
            }
            if (testsFailed > 0) {
                GD.Print($"*!*!* Passed: {testsPassed} | Failed: {testsFailed}");
            } else {
                GD.Print($"***** All passed: {testsPassed}!");
            }
        }

        private void IterateThroughAssemblies() {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies) {
                IterateThroughTypes(assembly.GetTypes());
            }
        }

        private void IterateThroughTypes(Type[] types) {
            foreach (Type type in types) {
                if (Attribute.GetCustomAttribute(type, typeof(IgnoreAttribute), false) is IgnoreAttribute) {
                    // TODO: this is not tested
                    continue;
                }
                if (requireTestFixture &&
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
                                testMethods[method] = curTestObject;
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
        }
    }

    public struct TestResult {
        public Type classType {
            get { return testMethod.DeclaringType; }
        }

        public MethodInfo testMethod { get; private set; }
        public Exception exception { get; private set; }
        public Result result { get; private set; }

        public enum Result {
            Passed,
            Failed
        }

        public TestResult(MethodInfo testMethod, Exception exception, Result result) {
            this.testMethod = testMethod;
            this.exception = exception;
            this.result = result;
        }
    }
}