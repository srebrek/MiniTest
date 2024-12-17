using MiniTest;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace MiniTestRunner
{
    public class TestRunner
    {
        public string AssemblyPath { get; set; }
        public AssemblyLoadContext Context { get; set; }
        public Assembly MainAssembly { get; set; }
        public Action? BeforeEach { get; set; } = null;
        public Action? AfterEach { get; set; } = null;
        public List<MethodInfo> Tests;

        public TestRunner(string assemblyPath)
        {
            AssemblyPath = assemblyPath;
            var context = new AssemblyLoadContext("TestContext", isCollectible: true);
            (MainAssembly, Context) = DependencyLoader.RecursiveLoader(assemblyPath, context);
            Tests = new List<MethodInfo>();

            foreach (var testClass in TestGetter.GetTestClasses(MainAssembly))
            {
                Tests.Clear();
                object? testClassInstance = testClass.GetConstructor(Type.EmptyTypes)?.Invoke(null);
                if (testClassInstance == null)
                {
                    Console.WriteLine("No parameterless constructor");
                    continue;
                }
                foreach (var beforeEachMethod in TestGetter.GetMethodWithAttribute(testClass, typeof(BeforeEachAttribute)))
                {
                    BeforeEach += (Action)Delegate.CreateDelegate(typeof(Action), testClassInstance, beforeEachMethod);
                }
                foreach (var afterEachMethod in TestGetter.GetMethodWithAttribute(testClass, typeof(AfterEachAttribute)))
                {
                    AfterEach += (Action)Delegate.CreateDelegate(typeof(Action), testClassInstance, afterEachMethod);
                }
                foreach (var testMethod in TestGetter.GetMethodWithAttribute(testClass, typeof(TestMethodAttribute)))
                {
                    Tests.Add(testMethod);
                }

                Tests.Sort((a, b) =>
                {
                    int priority1 = GetPriority(a);
                    int priority2 = GetPriority(b);

                    if (priority1 < priority2) return -1;
                    if (priority1 > priority2) return 1;

                    return string.Compare(a.Name, b.Name, StringComparison.Ordinal);
                });

                RunTests(testClassInstance);
            }
            context.Unload();
            GC.Collect();
            GC.WaitForPendingFinalizers();

        }

        public static int GetPriority(MethodInfo method)
        {
            var attribute = method.GetCustomAttributes(typeof(PriorityAttribute)).FirstOrDefault() as PriorityAttribute;
            return attribute?.Priority ?? 0;
        }

        public void RunTests(object classInstance)
        {
            foreach (var testMethod in Tests)
            {
                if (testMethod.GetCustomAttributes(typeof(DataRowAttribute)).Any() != false)
                {
                    foreach (DataRowAttribute dataRow in testMethod.GetCustomAttributes(typeof(DataRowAttribute)))
                    {
                        try
                        {
                            BeforeEach?.Invoke();
                            testMethod.Invoke(classInstance, dataRow.Data);
                            Console.WriteLine($"Runned {testMethod.Name}");
                            AfterEach?.Invoke();
                        }
                        catch (TargetInvocationException ex) when (ex.InnerException is MiniTest.AssertionException assertionEx)
                        {
                            Console.WriteLine($"Test {testMethod.Name} failed: {assertionEx.Message}");
                        }
                        catch (TargetInvocationException ex)
                        {
                            Console.WriteLine($"Test {testMethod.Name} failed: {ex.InnerException?.Message ?? ex.Message}");
                        }
                    }
                }
                else
                {
                    try
                    {
                        BeforeEach?.Invoke();
                        testMethod.Invoke(classInstance, null);
                        Console.WriteLine($"Runned {testMethod.Name}");
                        AfterEach?.Invoke();
                    }
                    catch (TargetInvocationException ex) when (ex.InnerException is MiniTest.AssertionException assertionEx)
                    {
                        Console.WriteLine($"Test {testMethod.Name} failed: {assertionEx.Message}");
                    }
                    catch (TargetInvocationException ex)
                    {
                        Console.WriteLine($"Test {testMethod.Name} failed: {ex.InnerException?.Message ?? ex.Message}");
                    }
                }
            }
        }

    }
}
