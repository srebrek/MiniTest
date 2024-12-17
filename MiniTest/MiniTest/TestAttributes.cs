namespace MiniTest
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TestClassAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class TestMethodAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class BeforeEachAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class AfterEachAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class PriorityAttribute(int priority) : Attribute
    {
        public int Priority { get; } = priority;
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class DataRowAttribute(params object?[] data) : Attribute
    {
        public object?[] Data { get; } = data;
        public string? Description { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class DescriptionAttribute(string description) : Attribute
    {
        public string Description { get; } = description;
    }
}
