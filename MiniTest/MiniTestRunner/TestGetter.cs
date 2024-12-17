using MiniTest;
using System.Reflection;

namespace MiniTestRunner
{
    public class TestGetter
    {
        public static IEnumerable<Type> GetTestClasses(Assembly assembly)
        {
            foreach (var testClass in assembly.GetTypes())
            {
                if (testClass.GetCustomAttribute(typeof(TestClassAttribute)) != null)
                    yield return testClass;
            }
        }

        public static IEnumerable<MethodInfo> GetMethodWithAttribute(Type testClass, Type attributeType)
        {
            foreach (var beforeEachMethod in testClass.GetMethods())
            {
                if (beforeEachMethod.GetCustomAttribute(attributeType) != null)
                    yield return beforeEachMethod;
            }
        }
    }
}
