using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTest
{
    public static class Assert
    {
        public static void ThrowsException<TException>(Action action, string message = "") where TException : Exception
        {
            try
            {
                action();
            }
            catch (TException)
            {
                return;
            }
            catch (Exception ex)
            {
                throw new AssertionException($"Expected exception of type:<{typeof(TException)}>. Actual exception type:<{ex.GetType()}>. {message}");
            }
            throw new AssertionException($"Expected exception of type:<{typeof(TException)}> but no exception was thrown. {message}");
        }

        public static void AreEqual<T>(T? expected, T? actual, string message = "")
        {
            if (expected != null && expected.Equals(actual))
                return;
            if (expected == null && actual == null)
                return;
            throw new AssertionException($"Expected: {expected?.ToString()}. Actual: {actual?.ToString()}. {message}");
        }

        public static void AreNotEqual<T>(T? expected, T? actual, string message = "")
        {
            if (expected != null && !expected.Equals(actual))
                return;
            if (expected == null && actual != null)
                return;
            throw new AssertionException($"Expected any value except: {expected?.ToString()}. Actual: {actual?.ToString()}. {message}");
        }

        public static void IsTrue(bool condition, string message = "")
        {
            if (condition)
                return;
            throw new AssertionException($"Expected: True. Actual: False. {message}");
        }

        public static void IsFalse(bool condition, string message = "")
        {
            if (!condition)
                return;
            throw new AssertionException($"Expected: False. Actual: True. {message}");
        }

        public static void Fail(string message = "")
        {
            throw new AssertionException($"Fail. {message}");
        }
    }
}
