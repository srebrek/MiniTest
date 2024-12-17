using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTestRunner
{
    public static class CLInputValidator
    {
        public static List<String> Validate(string[] args)
        {
            List<string> assemblyPaths = new List<string>();

            if (args.Length == 0)
            {
                Console.WriteLine("Usage: MiniTestRunner <path/to/test-assembly1.dll> <path/to/test-assembly2.dll>");
                return assemblyPaths;
            }
            
            foreach (string arg in args)
            {
                if (File.Exists(arg))
                {
                    assemblyPaths.Add(arg);
                }
                else
                {
                    Console.WriteLine($"Error: File not found: {arg}");
                }
            }
            if (assemblyPaths.Count == 0)
            {
                Console.WriteLine("No valid assembly paths provided.");
            }
            return assemblyPaths;
        }
    }
}
