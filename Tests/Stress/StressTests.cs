using FluentAssertions;
using System.Diagnostics;
using ThaiTextCompare.Core;

namespace ThaiTextCompare.Tests.Stress;

[TestClass]
public class StressTests
{
    private ComparisonEngine _comparisonEngine = null!;

    [TestInitialize]
    public void Setup()
    {
        var tokenizer = new ThaiMedicalTokenizer(SymptomDictionary.DefaultSymptomDict);
        _comparisonEngine = new ComparisonEngine(tokenizer);
    }

    [TestMethod]
    public void StressTest_MultipleSimultaneousComparisons_ShouldHandleCorrectly()
    {
        // Arrange
        const int numberOfComparisons = 1000;
        var text1 = "ไข้ ไอ เจ็บคอ มีน้ำมูก ปวดหัว";
        var text2 = "ไข้ ไอ เจ็บคอ น้ำมูก ปวดศีรษะ";
        var results = new List<ComparisonResult>();

        // Act
        var stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < numberOfComparisons; i++)
        {
            var result = _comparisonEngine.CompareThaiMedicalTexts(text1, text2, 100.0);
            results.Add(result);
        }

        stopwatch.Stop();

        // Assert
        results.Should().HaveCount(numberOfComparisons);
        results.Should().OnlyContain(r => r.Similarity > 0);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000, "Should complete 1000 comparisons within 5 seconds");

        Console.WriteLine($"Completed {numberOfComparisons} comparisons in {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"Average time per comparison: {stopwatch.ElapsedMilliseconds / (double)numberOfComparisons:F2}ms");
    }

    [TestMethod]
    public void StressTest_VeryLongTexts_ShouldOptimizeAutomatically()
    {
        // Arrange
        var veryLongText1 = "ไข้" + new string('A', 1000) + "ไอ" + new string('B', 1000) + "เจ็บคอ";
        var veryLongText2 = "ไข้" + new string('C', 1000) + "ไอ" + new string('D', 1000) + "เจ็บคอ";

        // Act
        var stopwatch = Stopwatch.StartNew();
        var result = _comparisonEngine.CompareThaiMedicalTexts(veryLongText1, veryLongText2, 100.0);
        stopwatch.Stop();

        // Assert
        result.Should().NotBeNull();
        result.MatchedWords.Should().Contain("ไข้");
        result.MatchedWords.Should().Contain("ไอ");
        result.MatchedWords.Should().Contain("เจ็บคอ");
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(2000, "Should handle very long texts efficiently");

        Console.WriteLine($"Very long text comparison completed in {stopwatch.ElapsedMilliseconds}ms");
    }

    [TestMethod]
    public void StressTest_ManyUniqueSymptoms_ShouldScaleCorrectly()
    {
        // Arrange
        var manySymptoms1 = string.Join(" ", SymptomDictionary.DefaultSymptomDict.Take(20));
        var manySymptoms2 = string.Join(" ", SymptomDictionary.DefaultSymptomDict.Skip(5).Take(20));

        // Act
        var stopwatch = Stopwatch.StartNew();
        var result = _comparisonEngine.CompareThaiMedicalTexts(manySymptoms1, manySymptoms2, 100.0);
        stopwatch.Stop();

        // Assert
        result.Should().NotBeNull();
        result.MatchedWords.Should().HaveCountGreaterThan(0);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100, "Should handle many symptoms quickly");

        Console.WriteLine($"Many symptoms comparison completed in {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"Matched words: {result.MatchedWords.Count}");
        Console.WriteLine($"Coverage1: {result.Coverage1}%");
        Console.WriteLine($"Coverage2: {result.Coverage2}%");
    }

    [TestMethod]
    public void StressTest_RandomizedInputs_ShouldBeRobust()
    {
        // Arrange
        var random = new Random(42); // Fixed seed for reproducibility
        var symptoms = SymptomDictionary.DefaultSymptomDict.ToList();
        var results = new List<ComparisonResult>();

        // Act
        for (int i = 0; i < 100; i++)
        {
            // Generate random combinations of symptoms
            var count1 = random.Next(1, Math.Min(symptoms.Count, 10));
            var count2 = random.Next(1, Math.Min(symptoms.Count, 10));

            var text1 = string.Join(" ", symptoms.OrderBy(x => random.Next()).Take(count1));
            var text2 = string.Join(" ", symptoms.OrderBy(x => random.Next()).Take(count2));

            var result = _comparisonEngine.CompareThaiMedicalTexts(text1, text2, 100.0);
            results.Add(result);
        }

        // Assert
        results.Should().HaveCount(100);
        results.Should().OnlyContain(r => r.Similarity >= 0 && r.Similarity <= 100);
        results.Should().OnlyContain(r => r.Coverage1 >= 0 && r.Coverage1 <= 100);
        results.Should().OnlyContain(r => r.Coverage2 >= 0 && r.Coverage2 <= 100);

        var averageSimilarity = results.Average(r => r.Similarity);
        Console.WriteLine($"Average similarity across 100 random comparisons: {averageSimilarity:F2}%");
    }

    [TestMethod]
    public void StressTest_EdgeCaseCombinations_ShouldNotThrow()
    {
        // Arrange
        var edgeCases = new[]
        {
            ("", ""),
            ("", "ไข้"),
            ("ไข้", ""),
            ("   ", "   "),
            ("ไข้", "   "),
            ("   ", "ไข้"),
            (new string('ไ', 1), new string('ข', 1)),
            (new string('A', 1000), new string('B', 1000)),
            ("ไข้" + new string('\0', 10), "ไข้"),
            ("ไข้\n\r\t", "ไข้"),
            ("!@#$%^&*()", ")(*)&^%$#@!"),
            ("ไข้123ABC", "ไข้456DEF"),
        };

        // Act & Assert
        foreach (var (text1, text2) in edgeCases)
        {
            Action act = () => _comparisonEngine.CompareThaiMedicalTexts(text1, text2, 100.0);
            act.Should().NotThrow($"Should handle edge case: '{text1}' vs '{text2}'");

            var result = _comparisonEngine.CompareThaiMedicalTexts(text1, text2, 100.0);
            result.Should().NotBeNull();
            result.Similarity.Should().BeInRange(0, 100);
        }
    }

    [TestMethod]
    public void MemoryUsageTest_ShouldNotLeakMemory()
    {
        // Arrange
        const int iterations = 1000;
        var text1 = "ไข้ ไอ เจ็บคอ มีน้ำมูก";
        var text2 = "ไข้ ไอ เจ็บคอ น้ำมูก";

        // Measure memory before
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        var memoryBefore = GC.GetTotalMemory(false);

        // Act
        for (int i = 0; i < iterations; i++)
        {
            var result = _comparisonEngine.CompareThaiMedicalTexts(text1, text2, 100.0);
            // Don't store results to allow garbage collection
        }

        // Force garbage collection
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        var memoryAfter = GC.GetTotalMemory(false);

        // Assert
        var memoryIncrease = memoryAfter - memoryBefore;
        Console.WriteLine($"Memory before: {memoryBefore / 1024.0 / 1024.0:F2} MB");
        Console.WriteLine($"Memory after: {memoryAfter / 1024.0 / 1024.0:F2} MB");
        Console.WriteLine($"Memory increase: {memoryIncrease / 1024.0 / 1024.0:F2} MB");

        // Allow for some memory increase but not excessive
        memoryIncrease.Should().BeLessThan(10 * 1024 * 1024, "Memory increase should be less than 10MB");
    }
}
