namespace MiniTestRunner;

public static class CLInputValidator
{
    public static List<String> Validate(string[] args)
    {
        List<string> assemblyPaths = [];

        if (args.Length == 0)
        {
            Console.WriteLine("Usage: MiniTestRunner <testPath1.dll> <testPath2.dll>");
            return assemblyPaths;
        }

        foreach (string arg in args)
        {
            if (File.Exists(arg))
                assemblyPaths.Add(arg);
            else
                Console.WriteLine($"Error: File not found: {arg}");
        }
        if (assemblyPaths.Count == 0)
            Console.WriteLine("No valid assembly paths provided.");

        return assemblyPaths;
    }
}
