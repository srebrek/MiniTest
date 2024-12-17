using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Loader;

namespace MiniTestRunner
{
    public static class DependencyLoader
    {
        public static (Assembly assembly, AssemblyLoadContext ALC) RecursiveLoader(string assemblyPath, AssemblyLoadContext context)
        {
            // var context = new AssemblyLoadContext("TestContext", isCollectible: true);
            //foreach (var cos in AppDomain.CurrentDomain.GetAssemblies())
            //{
            //    Console.WriteLine(cos.Location);
            //}
            //Console.WriteLine();
            //foreach (var cos in context.Assemblies)
            //{
            //    Console.WriteLine(cos.FullName);
            //}
            //Console.WriteLine("\n");

            Assembly assembly;
            string assemblyFullName = AssemblyName.GetAssemblyName(assemblyPath).FullName;
            if (!AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName!.Equals(assemblyFullName, StringComparison.OrdinalIgnoreCase)))
            {
                assembly = context.LoadFromAssemblyPath(assemblyPath);
            }
            else
            {
                assembly = AppDomain.CurrentDomain.GetAssemblies().First();
            }

            // assembly.GetReferencedAssemblies();

            foreach (var cos in assembly.GetReferencedAssemblies())
            {
                // Console.WriteLine(cos.FullName);
                var resolver = new AssemblyDependencyResolver(assemblyPath);
                var path = resolver.ResolveAssemblyToPath(cos);
                // Console.WriteLine(path);
                if (path != null)//&& path != @"C:\Users\zlote\Desktop\programowanie_3_laby\P3Z_24Z_Project1\MiniTest\AuthenticationService.Tests\bin\Debug\net8.0\MiniTest.dll")
                {
                    // context.LoadFromAssemblyPath(path);
                    RecursiveLoader(path, context);
                }
            }

            

            // Console.WriteLine($"Loaded assembly: {assembly.FullName}");

            return (assembly, context);
        }
    }
}
