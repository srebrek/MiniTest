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
                
            }
        }
    }
}
