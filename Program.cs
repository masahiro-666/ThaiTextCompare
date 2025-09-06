
using System;
using System.IO;
using System.Linq;
using ThaiTextCompare.Core;
using System.Text.Json;

partial class Program
{
    public class TestCase
    {
        public string text1 { get; set; } = "";
        public string text2 { get; set; } = "";
        public string description { get; set; } = "";
    }

    public class TestCasesData
    {
        public object metadata { get; set; } = new object();
        public TestCase[] testCases { get; set; } = Array.Empty<TestCase>();
    }

#if !UNITY_BUILD
    static void Main()
    {
        Console.WriteLine("🏥 Thai Medical Text Compare - Enhanced Version");
        Console.WriteLine("================================================\n");

        RunOriginalTests();
    }
#endif

    static (string, string, string)[] LoadTestCasesFromJson()
    {
        try
        {
            var jsonPath = Path.Combine(AppContext.BaseDirectory, "Data", "test-cases.json");
            if (!File.Exists(jsonPath))
            {
                Console.WriteLine($"⚠️ Test cases JSON file not found at {jsonPath}. Using fallback test cases.");
                return GetFallbackTestCases();
            }

            var jsonString = File.ReadAllText(jsonPath);
            var testData = JsonSerializer.Deserialize<TestCasesData>(jsonString);

            if (testData?.testCases == null)
            {
                Console.WriteLine("⚠️ Invalid test cases JSON format. Using fallback test cases.");
                return GetFallbackTestCases();
            }

            Console.WriteLine($"📖 Loaded {testData.testCases.Length} test cases from JSON file");
            return testData.testCases.Select(tc => (tc.text1, tc.text2, tc.description)).ToArray();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error loading test cases from JSON: {ex.Message}. Using fallback test cases.");
            return GetFallbackTestCases();
        }
    }

    static (string, string, string)[] GetFallbackTestCases()
    {
        return new[]
        {
            ("ไข้ ไอ เจ็บคอ มีน้ำมูก", "ไข้ ไอ เจ็บคอ มีน้ำมูก", "Exact match"),
            ("ไข้ ไอ เจ็บคอ มีน้ำมูก", "ไข้ ไอ เจ็บคอ น้ำมูก", "Synonym variation"),
            ("ไข้", "ไข้ ไอ", "Subset vs superset"),
        };
    }

    static void RunOriginalTests()
    {
        // Initialize the system
        var tokenizer = new ThaiMedicalTokenizer(SymptomDictionary.DefaultSymptomDict);
        var comparisonEngine = new ComparisonEngine(tokenizer);

        // Refresh tokenizer with new dictionary
        tokenizer = ThaiMedicalTokenizer.CreateWithDynamicDictionary();
        comparisonEngine = new ComparisonEngine(tokenizer);

        var stats = SymptomDictionary.GetDictionaryStats();
        Console.WriteLine($"✅ Dictionary updated: {stats.TotalWords} total words, {stats.AddedWords} added\n");

        // Load test cases from JSON file
        var testCases = LoadTestCasesFromJson();

        Console.WriteLine("=== Test Case Scenarios Analysis (Coverage-Based Matching) ===\n");

        foreach (var (text1, text2, description) in testCases)
        {
            // Use the comparison engine for comparison
            var result = comparisonEngine.CompareThaiMedicalTexts(text1, text2, 100.0); // 100% threshold means ALL words must be present

            var tokens1 = tokenizer.TokenizeThaiSymptoms(text1);
            var tokens2 = tokenizer.TokenizeThaiSymptoms(text2);
            var set1 = tokens1.Distinct().ToHashSet();
            var set2 = tokens2.Distinct().ToHashSet();

            var missingWords = result.MissingWords;
            var extraWords = result.ExtraWords;
            var matchedWords = result.MatchedWords;

            // Calculate Jaccard for reference
            double jaccardSimilarity = result.Union == 0 ? 0 : Math.Round((double)result.Intersection / result.Union * 100, 2);

            Console.WriteLine($"{description}:");
            Console.WriteLine($"  Text1: '{text1}'");
            Console.WriteLine($"  Text2: '{text2}'");
            Console.WriteLine($"  Tokens1: [{string.Join(", ", set1)}] ({set1.Count} tokens)");
            Console.WriteLine($"  Tokens2: [{string.Join(", ", set2)}] ({set2.Count} tokens)");
            Console.WriteLine($"  ✅ Matched: [{string.Join(", ", matchedWords)}] ({matchedWords.Count} words)");
            Console.WriteLine($"  ❌ Missing from Text2: [{string.Join(", ", missingWords)}] ({missingWords.Count} words)");
            Console.WriteLine($"  ➕ Extra in Text2: [{string.Join(", ", extraWords)}] ({extraWords.Count} words)");

            // Display typo corrections if any
            if (result.TypoCorrections.Count > 0)
            {
                Console.WriteLine($"  🔧 Typo Corrections: [{string.Join(", ", result.TypoCorrections)}] ({result.TypoCorrections.Count} corrections)");
            }

            Console.WriteLine($"  Text1→Text2 Coverage: {result.Coverage1}% (Primary Score)");
            Console.WriteLine($"  Text2→Text1 Coverage: {result.Coverage2}%");
            Console.WriteLine($"  Jaccard Similarity: {jaccardSimilarity}% (Reference)");

            // Show match result with typo correction context
            var matchContext = result.TypoCorrections.Count > 0 ? $"{result.IsMatch} (Typo-corrected match)" : $"{result.IsMatch}";
            Console.WriteLine($"  Match Result: {matchContext} (Text2 contains ALL Text1 words: {result.Coverage1 >= 100}%)");
            Console.WriteLine();
        }
    }
}
