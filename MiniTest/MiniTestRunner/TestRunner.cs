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
            int passed = 0;
            int failed = 0;
            int total = 0;

            Console.WriteLine($"Running tests from class {classInstance}...");
            foreach (var testMethod in Tests)
            {
                if (testMethod.GetCustomAttributes(typeof(DataRowAttribute)).Any() != false)
                {
                    Console.WriteLine($"{testMethod.Name}");
                    foreach (DataRowAttribute dataRow in testMethod.GetCustomAttributes(typeof(DataRowAttribute)))
                    {
                        try
                        {
                            BeforeEach?.Invoke();
                            testMethod.Invoke(classInstance, dataRow.Data);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"{dataRow.Description, -60} : PASSED");
                            Console.ResetColor();
                            passed++;
                            total++;
                            AfterEach?.Invoke();
                        }
                        catch (TargetInvocationException ex) when (ex.InnerException is MiniTest.AssertionException assertionEx)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"{dataRow.Description, -60} : FAILED");
                            Console.ResetColor();
                            failed++;
                            total++;
                            Console.WriteLine($"{assertionEx.Message}");
                        }
                        catch (TargetInvocationException ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"{dataRow.Description,-60} : ERROR");
                            Console.ResetColor();
                            failed++;
                            total++;
                            if (ex.InnerException != null)
                            {
                                Console.WriteLine($"{ex.InnerException.Message}");
                            }
                            else
                            {
                                Console.WriteLine($"{ex.Message}");
                            }
                        }
                    }
                }
                else
                {
                    try
                    {
                        BeforeEach?.Invoke();
                        testMethod.Invoke(classInstance, null);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{testMethod.Name,-60} : PASSED");
                        Console.ResetColor();
                        passed++;
                        total++;
                        AfterEach?.Invoke();
                    }
                    catch (TargetInvocationException ex) when (ex.InnerException is MiniTest.AssertionException assertionEx)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{testMethod.Name,-60} : FAILED");
                        Console.ResetColor();
                        failed++;
                        total++;
                        Console.WriteLine($"{assertionEx.Message}");
                    }
                    catch (TargetInvocationException ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"{testMethod.Name,-60} : ERROR");
                        Console.ResetColor();
                        failed++;
                        total++;
                        if (ex.InnerException != null)
                        {
                            Console.WriteLine($"{ex.InnerException.Message}");
                        }
                        else
                        {
                            Console.WriteLine($"{ex.Message}");
                        }
                    }
                }
                if (testMethod.GetCustomAttribute(typeof(MiniTest.DescriptionAttribute)) is MiniTest.DescriptionAttribute description)
                {
                    Console.WriteLine($"{description.Description}");
                }
            }
            Console.WriteLine("******************************");
            Console.WriteLine($"* Test passed:    {passed} / {total}    *");
            Console.WriteLine($"* Failed:          {failed}         *");
            Console.WriteLine("******************************");
            Console.WriteLine("################################################################################");
        }

    }
}
