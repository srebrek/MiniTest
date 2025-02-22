using MiniTest;
using System.Reflection;
using System.Runtime.Loader;

namespace MiniTestRunner;

public class TestRunner
{
    private AssemblyLoadContext Context { get; set; }
    private Assembly MainAssembly { get; set; }
    private Action? BeforeEach { get; set; } = null;
    private Action? AfterEach { get; set; } = null;
    private readonly List<MethodInfo> Tests = [];
    private object? TestClassInstance = null;


    public IEnumerable<Type> GetTestClasses()
    {
        Type? testClassAttributeType = GetTypeFromContextAssembly("MiniTest.TestClassAttribute");

        foreach (var testClass in MainAssembly.GetTypes())
        {
            if (testClass.GetCustomAttribute(testClassAttributeType!) != null)
                yield return testClass;
        }
    }

    private Type? GetTypeFromContextAssembly(string typeFullName)
    {
        return Context.Assemblies
        .SelectMany(a => a.GetTypes())
        .FirstOrDefault(t => t.FullName == typeFullName);
    }

    public TestRunner(string assemblyPath)
    {
        Context = new AssemblyLoadContext(Guid.NewGuid().ToString(), isCollectible: true);
        MainAssembly = LoadAssemblyRecursively(assemblyPath);      
    }

    private Assembly LoadAssemblyRecursively(string assemblyPath)
    {
        string assemblyFullName = AssemblyName.GetAssemblyName(assemblyPath).FullName;
        Assembly assembly;

        var loadedAssembly = Context.Assemblies.FirstOrDefault(a => a.FullName == assemblyFullName);
        if (loadedAssembly == null)
            assembly = Context.LoadFromAssemblyPath(assemblyPath);
        else
            assembly = loadedAssembly;

        foreach (var dependency in assembly.GetReferencedAssemblies())
        {
            var resolver = new AssemblyDependencyResolver(assemblyPath);
            var path = resolver.ResolveAssemblyToPath(dependency);
            if (path != null)
                LoadAssemblyRecursively(path);
        }
        return assembly;
    }

    public void PrepareTests(Type testClass)
    {
        Tests.Clear();
        TestClassInstance = testClass.GetConstructor(Type.EmptyTypes)?.Invoke(null);
        if (TestClassInstance == null)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{testClass.Name}: No parameterless constructor");
            Console.ResetColor();
            return;
        }

        foreach (var beforeEachMethod in GetMethodWithAttribute(testClass, typeof(BeforeEachAttribute)))
            BeforeEach += (Action)Delegate.CreateDelegate(typeof(Action), TestClassInstance, beforeEachMethod);

        foreach (var afterEachMethod in GetMethodWithAttribute(testClass, typeof(AfterEachAttribute)))
            AfterEach += (Action)Delegate.CreateDelegate(typeof(Action), TestClassInstance, afterEachMethod);

        foreach (var testMethod in GetMethodWithAttribute(testClass, typeof(TestMethodAttribute)))
            Tests.Add(testMethod);

        Tests.Sort((a, b) =>
        {
            int priority1 = GetPriority(a);
            int priority2 = GetPriority(b);

            if (priority1 < priority2) return -1;
            if (priority1 > priority2) return 1;

            return string.Compare(a.Name, b.Name, StringComparison.Ordinal);
        });
    }

    public void Clear()
    {
        Context.Unload();
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    private IEnumerable<MethodInfo> GetMethodWithAttribute(Type testClass, Type attributeType)
    {
        Type? testClassAttributeType = GetTypeFromContextAssembly(attributeType.FullName!);
        foreach (var method in testClass.GetMethods())
        {
            if (method.GetCustomAttribute(testClassAttributeType!) != null)
                yield return method;
        }
    }

    private int GetPriority(MethodInfo method)
    {
        Type? priorityClassAttributeType = GetTypeFromContextAssembly("MiniTest.PriorityAttribute");
        var attribute = method.GetCustomAttributes(priorityClassAttributeType!).FirstOrDefault();
        int? priority = (int?)attribute?.GetType().GetProperty("Priority")?.GetValue(attribute);
        return priority ?? 0;
    }

    public void RunTests()
    {
        int passed = 0;
        int failed = 0;
        int total = 0;

        Console.WriteLine($"Running tests from class {TestClassInstance}...");
        foreach (var testMethod in Tests)
        {
            if (IsDataRow(testMethod))
            {
                RunParameterizedTest(testMethod, ref passed, ref failed, ref total);
            }
            else
            {
                RunSingleTest(testMethod, ref passed, ref failed, ref total);
            }
            string? description = GetDescription(testMethod);
            if (description != null)
                Console.WriteLine($"{description}");
        }
        PrintSummary(passed, failed, total);
    }

    private bool IsDataRow(MethodInfo method)
    {
        Type? dataRowAttributeType = GetTypeFromContextAssembly("MiniTest.DataRowAttribute");
        return method.GetCustomAttributes(dataRowAttributeType!).Any();
    }

    private void RunParameterizedTest(MethodInfo testMethod, ref int passed, ref int failed, ref int total)
    {
        Console.WriteLine($"{testMethod.Name}");
        foreach (Attribute dataRowAttribute in GetDataRowAttributes(testMethod))
        {
            (object[] Data, string Description) = GetDataRowDataAndDescription(dataRowAttribute);
            try
            {
                BeforeEach?.Invoke();
                testMethod.Invoke(TestClassInstance, Data);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($" - {Description,-57} : PASSED");
                Console.ResetColor();
                passed++;
                total++;
                AfterEach?.Invoke();
            }
            catch (TargetInvocationException ex)
            {
                HandleTestException(Description, ex, ref failed, ref total);
            }
        }
    }

    private IEnumerable<Attribute> GetDataRowAttributes(MethodInfo method)
    {
        Type? dataRowAttributeType = GetTypeFromContextAssembly("MiniTest.DataRowAttribute");
        foreach (var attribute in method.GetCustomAttributes(dataRowAttributeType!))
        {
            yield return attribute;
        }
    }

    private (object[] data, string description) GetDataRowDataAndDescription(Attribute dataRowAttribute)
    {
        Type? dataRowAttributeType = GetTypeFromContextAssembly("MiniTest.DataRowAttribute");
        object[] data = (object[])dataRowAttributeType!.GetProperty("Data")!.GetValue(dataRowAttribute)!;
        string description = (string)dataRowAttributeType.GetProperty("Description")!.GetValue(dataRowAttribute)!;
        return (data, description);
    }

    private void RunSingleTest(MethodInfo testMethod, ref int passed, ref int failed, ref int total)
    {
        try
        {
            BeforeEach?.Invoke();
            testMethod.Invoke(TestClassInstance, null);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{testMethod.Name,-60} : PASSED");
            Console.ResetColor();
            passed++;
            total++;
            AfterEach?.Invoke();
        }
        catch (TargetInvocationException ex)
        {
            HandleTestException(testMethod.Name, ex, ref failed, ref total);
        }
    }

    private void HandleTestException(string testName, TargetInvocationException ex, ref int failed, ref int total)
    {
        Type? assertionExceptionType = GetAssertionExceptionType();
        Console.ForegroundColor = ex.InnerException?.GetType() == assertionExceptionType ? ConsoleColor.Red : ConsoleColor.Yellow;
        Console.WriteLine($"{testName,-60} : {(ex.InnerException?.GetType() == assertionExceptionType ? "FAILED" : "ERROR")}");
        Console.ResetColor();
        failed++;
        total++;
        Console.WriteLine($"{ex.InnerException?.Message ?? ex.Message}");
    }

    private string? GetDescription(MethodInfo method)
    {
        Type? descriptionAttributeType = GetTypeFromContextAssembly("MiniTest.DescriptionAttribute");
        var attribute = method.GetCustomAttribute(descriptionAttributeType!);
        if (attribute == null)
            return null;
        return (string)attribute.GetType().GetProperty("Description")!.GetValue(attribute)!;
    }

    private void PrintSummary(int passed, int failed, int total)
    {
        Console.WriteLine("******************************");
        Console.WriteLine($"* Test passed:{passed,6} / {total,-6}{'*'}");
        Console.WriteLine($"* Failed:{failed,11}{'*',10}");
        Console.WriteLine("******************************");
        Console.WriteLine("################################################################################");
    }

    private Type? GetAssertionExceptionType()
    {
        Type? assertionExceptionType = GetTypeFromContextAssembly("MiniTest.AssertionException");
        return assertionExceptionType;
    }
}
