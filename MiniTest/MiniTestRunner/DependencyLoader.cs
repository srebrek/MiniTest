using System.Reflection;
using System.Runtime.Loader;

namespace MiniTestRunner
{
    public static class DependencyLoader
    {
        public static (Assembly assembly, AssemblyLoadContext ALC) RecursiveLoader(string assemblyPath, AssemblyLoadContext context)
        {

            Assembly assembly;
            string assemblyFullName = AssemblyName.GetAssemblyName(assemblyPath).FullName;
            if (!AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName!.Equals(assemblyFullName, StringComparison.OrdinalIgnoreCase)))
                assembly = context.LoadFromAssemblyPath(assemblyPath);
            else
                assembly = AppDomain.CurrentDomain.GetAssemblies().First();

            foreach (var dependency in assembly.GetReferencedAssemblies())
            {
                var resolver = new AssemblyDependencyResolver(assemblyPath);
                var path = resolver.ResolveAssemblyToPath(dependency);
                if (path != null)
                    RecursiveLoader(path, context);
            }
            return (assembly, context);
        }
    }
}
