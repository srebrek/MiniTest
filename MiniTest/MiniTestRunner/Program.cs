namespace MiniTestRunner;

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            args = [GetDefaultPath()];
            Console.WriteLine("Default Path has been chosen: " + args[0]);
        }
        List<string> assemblyPaths = CLInputValidator.Validate(args);
        if (assemblyPaths.Count == 0)
            return;

        foreach (string assemblyPath in assemblyPaths)
        {
            var testRunner = new TestRunner(assemblyPath);
            foreach (var testClass in testRunner.GetTestClasses())
            {
                testRunner.PrepareTests(testClass);
                testRunner.RunTests();
            }
            testRunner.Clear();
        }
        Console.WriteLine("Testing Finished. Press any key to close.");
        Console.ReadKey();
    }

    public static string GetDefaultPath()
    {
        var solutionDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)!.Parent!.Parent!.Parent!.Parent!.FullName;
        return Path.Combine(solutionDirectory, "AuthenticationService.Tests/bin/Debug/net8.0/AuthenticationService.Tests.dll");
    }
}
