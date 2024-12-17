using MiniTest;
using System;
using System.Reflection;
using System.Runtime.Loader;

namespace MiniTestRunner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            List<string> assemblyPaths = new List<string> { @"C:\Users\zlote\Desktop\programowanie_3_laby\P3Z_24Z_Project1\MiniTest\AuthenticationService.Tests\bin\Debug\net8.0\AuthenticationService.Tests.dll" }; // CLInputValidator.Validate(args); // uncomment before release
            if (assemblyPaths.Count == 0)
                return;

            foreach (string assemblyPath in assemblyPaths)
            {
                var testRunner = new TestRunner(assemblyPath);
                foreach (var testMethod in testRunner.Tests)
                {
                    Console.WriteLine(TestRunner.GetPriority(testMethod));
                    Console.WriteLine(testMethod.Name);
                }
                //var context1 = new AssemblyLoadContext("TestContext", isCollectible: true);
                //(Assembly assembly, AssemblyLoadContext context) = DependencyLoader.RecursiveLoader(assemblyPath, context1);

                //foreach (var cos in context.Assemblies)
                //{
                //    Console.WriteLine(cos.FullName);
                //}

                //Console.WriteLine();
                //foreach (var testClass in TestGetter.GetTestClasses(assembly))
                //{
                //    Action beforeEach = null!;
                //    Console.WriteLine(testClass.FullName);
                //    object? testClassInstance =  testClass.GetConstructor(Type.EmptyTypes)?.Invoke(null);
                //    if (testClassInstance == null)
                //    {
                //        Console.WriteLine("No parametless constructor");
                //        continue;
                //    }
                //    foreach (var beforeEachMethod in TestGetter.GetMethodWithAttribute(testClass, typeof(BeforeEachAttribute)))
                //    {
                //        beforeEach += (Action)Delegate.CreateDelegate(typeof(Action), testClassInstance, beforeEachMethod);
                //    }
                //    beforeEach?.Invoke();
                //}

                //context.Unload();
                //GC.Collect();
                //GC.WaitForPendingFinalizers();
            }
        }
    }
}
