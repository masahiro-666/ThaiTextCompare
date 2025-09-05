using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ThaiTextCompare.Tests.Utilities;

public static class TestHelper
{
    /// <summary>
    /// Gets the directory path of the executing test assembly
    /// </summary>
    public static string GetTestDirectory()
    {
        return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException("Cannot determine test directory");
    }

    /// <summary>
    /// Reads test data from an embedded resource
    /// </summary>
    public static string ReadEmbeddedResource(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(resourceName) ?? throw new FileNotFoundException($"Resource '{resourceName}' not found");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Creates temporary test files for testing file I/O operations
    /// </summary>
    public static string CreateTempFile(string content, string extension = ".txt")
    {
        var tempFile = Path.GetTempFileName();
        if (extension != ".tmp")
        {
            var newPath = Path.ChangeExtension(tempFile, extension);
            File.Move(tempFile, newPath);
            tempFile = newPath;
        }

        File.WriteAllText(tempFile, content);
        return tempFile;
    }

    /// <summary>
    /// Measures execution time of an action
    /// </summary>
    public static TimeSpan MeasureExecutionTime(Action action)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        action();
        stopwatch.Stop();
        return stopwatch.Elapsed;
    }

    /// <summary>
    /// Measures execution time of a function and returns both the result and the time
    /// </summary>
    public static (T result, TimeSpan time) MeasureExecutionTime<T>(Func<T> func)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = func();
        stopwatch.Stop();
        return (result, stopwatch.Elapsed);
    }

    /// <summary>
    /// Creates test data for stress testing
    /// </summary>
    public static IEnumerable<string> GenerateTestStrings(int count, int minLength, int maxLength, Random? random = null)
    {
        random ??= new Random(42); // Fixed seed for reproducibility
        var thaiChars = "กขฃคฅฆงจฉชซฌญฎฏฐฑฒณดตถทธนบปผฝพฟภมยรลวศษสหฬอฮ";

        for (int i = 0; i < count; i++)
        {
            var length = random.Next(minLength, maxLength + 1);
            var chars = new char[length];

            for (int j = 0; j < length; j++)
            {
                chars[j] = thaiChars[random.Next(thaiChars.Length)];
            }

            yield return new string(chars);
        }
    }

    /// <summary>
    /// Asserts that an operation completes within a specified time limit
    /// </summary>
    public static void AssertExecutionTime(Action action, TimeSpan maxTime, string? message = null)
    {
        var actualTime = MeasureExecutionTime(action);

        if (actualTime > maxTime)
        {
            throw new AssertFailedException(
                message ?? $"Expected operation to complete within {maxTime}, but took {actualTime}");
        }
    }

    /// <summary>
    /// Gets memory usage before and after an operation
    /// </summary>
    public static (long before, long after) MeasureMemoryUsage(Action action)
    {
        // Force garbage collection before measurement
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var memoryBefore = GC.GetTotalMemory(false);

        action();

        // Force garbage collection after operation
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var memoryAfter = GC.GetTotalMemory(false);

        return (memoryBefore, memoryAfter);
    }

    /// <summary>
    /// Disposes of temporary files created during testing
    /// </summary>
    public static void CleanupTempFiles(params string[] filePaths)
    {
        foreach (var filePath in filePaths)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }
}

/// <summary>
/// Custom test attributes for categorizing tests
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class PerformanceTestAttribute : Attribute
{
    public string Category { get; } = "Performance";
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class IntegrationTestAttribute : Attribute
{
    public string Category { get; } = "Integration";
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class StressTestAttribute : Attribute
{
    public string Category { get; } = "Stress";
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class SlowTestAttribute : Attribute
{
    public string Category { get; } = "Slow";
}
