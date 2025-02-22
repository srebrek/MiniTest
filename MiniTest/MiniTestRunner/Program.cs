namespace MiniTestRunner;

public class Program
{
    public static void Main(string[] args)
    {
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
    }
}
