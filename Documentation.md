# MiniTest Framework

## Overview
**MiniTest** is a lightweight unit testing framework for .NET applications. It consists of two components:

- **MiniTest** - A library containing attributes for marking test classes and methods, along with assertion methods.
- **MiniTestRunner** - A console application that dynamically loads assemblies, discovers tests, executes them, and displays results in the console.

## Features
- Custom test attributes for marking test classes and methods.
- Assertion methods to verify test conditions.
- Dynamic assembly loading and unloading.
- Test discovery and execution with support for parameterized tests.
- Console output with color-coded test results and detailed failure reasons.

---

## Example

Example test class is provided in `AuthenticationService.Tests`

---

## MiniTest Library

### Test Attributes
The following attributes are available in the **MiniTest** library:

1. **TestClassAttribute** - Marks a class as a test container.
2. **TestMethodAttribute** - Marks a method as a test case.
3. **BeforeEachAttribute** - Marks a method to be executed before each test.
4. **AfterEachAttribute** - Marks a method to be executed after each test.
5. **PriorityAttribute** - Defines test execution priority (lower values run first).
6. **DataRowAttribute** - Enables parameterized tests with test data.
7. **DescriptionAttribute** - Provides an optional description for tests and test classes.

### Assertions
Assertions are provided via the `Assert` static class:

- `ThrowsException<TException>(Action action, string message = "")` - Ensures an exception of type `TException` is thrown.
- `AreEqual<T>(T? expected, T? actual, string message = "")` - Verifies two values are equal.
- `AreNotEqual<T>(T? notExpected, T? actual, string message = "")` - Ensures values are different.
- `IsTrue(bool condition, string message = "")` - Verifies that a condition is `true`.
- `IsFalse(bool condition, string message = "")` - Verifies that a condition is `false`.
- `Fail(string message = "")` - Explicitly marks a test as failed.

All assertion failures throw an `AssertionException` with a clear message.

---

## MiniTestRunner

### Usage
Run `MiniTestRunner` by passing test assembly paths as arguments:

```sh
MiniTestRunner path/to/test-assembly1.dll path/to/test-assembly2.dll
```

### Assembly Loading
- Uses `AssemblyLoadContext` for dynamic loading.
- Ensures assemblies are unloaded after execution to free resources.

### Test Discovery
- Finds all classes marked with `TestClassAttribute`.
- Identifies test methods marked with `TestMethodAttribute`.
- Recognizes `BeforeEach` and `AfterEach` methods for setup/teardown.
- Handles `DataRowAttribute` for parameterized tests.
- Ignores classes without a parameterless constructor and logs a warning.

### Test Execution
Tests are executed based on priority (lower values first). The execution order within the same priority is alphabetical.
For each test method:
1. Calls `BeforeEach` method (if present).
2. Executes the test method.
3. Calls `AfterEach` method (if present).

A test fails if an unhandled exception occurs.

### Output Formatting
Console output includes:
- **PASSED** (green) or **FAILED** (red) status.
- Failure details for failed tests.
- Warnings (yellow) for ignored or misconfigured tests.
- Summary for each test class.

Example output:
```
Default Path has been chosen: C:\Users\zlote\Desktop\programowanie_3_laby\P3Z_24Z_Project1\MiniTest\AuthenticationService.Tests/bin/Debug/net8.0/AuthenticationService.Tests.dll
Running tests from class AuthenticationService.Tests.AuthServiceTests...
GetRegisteredUserData_ExistingUsername_ShouldThrowError      : PASSED
GetRegisteredUserData_NonExistingUsername_ShouldThrowError   : PASSED
Login_InvalidPassword_ShouldFail                             : PASSED
Login_NonExistingUsername_ShouldFail                         : PASSED
Login_ValidPassword_ShouldSucceed                            : PASSED
Register_ExistingUsername_ShouldRejectRegisteringUser        : PASSED
Register_InvalidPassword_ShouldRejectNewUser                 : PASSED
Register_InvalidUsername_ShouldRejectNewUser                 : PASSED
Register_NewUsername_ShouldAddNewUser                        : PASSED
Register_TwoDifferentUsernames_ShouldAddBothUsers            : PASSED
ChangePassword_InvalidNewPassword_ShouldFail                 : FAILED
Expected: False. Actual: True. User should not be able to change password to something invalid.
This test is supposed to fail, just for testing purposes.
ChangePassword_NonExistingUsername_ShouldThrowError          : FAILED
Expected exception of type:<AuthenticationService.UserNotFoundException>. Actual exception type:<System.NotImplementedException>.
This test is supposed to fail, just for testing purposes.
ChangePassword_ValidUserAndPassword_ShouldSucceed            : FAILED
Expected: True. Actual: False. Existing user should be able to change password to something valid.
This test is supposed to fail, just for testing purposes.
******************************
* Test passed:    10 / 13    *
* Failed:          3         *
******************************
################################################################################
```

---

## License
Distributed under the MIT License. See [LICENSE](LICENSE) for details.

---

## Author
Developed by **srebrek**.

---

## Acknowledgments
Inspired by existing unit testing frameworks such as MSTest and xUnit.

