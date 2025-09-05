using FluentAssertions;
using ThaiTextCompare.Core;

namespace ThaiTextCompare.Tests.Unit;

/// <summary>
/// Test data builder for creating test scenarios systematically
/// </summary>
public class TestDataBuilder
{
    public static IEnumerable<object[]> GetBasicComparisonScenarios()
    {
        return new[]
        {
            // Exact matches
            new object[] { "ไข้", "ไข้", true, 100.0, "Single symptom exact match" },
            new object[] { "ไข้ ไอ", "ไข้ ไอ", true, 100.0, "Multiple symptoms exact match" },
            new object[] { "", "", true, 100.0, "Both empty strings" },

            // Partial matches
            new object[] { "ไข้ ไอ เจ็บคอ", "ไข้ ไอ", false, 66.67, "Missing one symptom" },
            new object[] { "ไข้", "ไข้ ไอ", false, 100.0, "Extra symptom in text2" },

            // Order variations
            new object[] { "ไข้ ไอ เจ็บคอ", "เจ็บคอ ไอ ไข้", true, 100.0, "Different order" },
            new object[] { "ไข้ ไอ เจ็บคอ", "ไอไข้เจ็บคอ", true, 100.0, "Concatenated same order" },

            // Synonym handling
            new object[] { "มีน้ำมูก", "น้ำมูก", true, 100.0, "Synonym normalization" },
            new object[] { "ปวดศีรษะ", "ปวดหัว", true, 100.0, "Head pain synonym" },

            // No match
            new object[] { "ไข้", "ปวดท้อง", false, 0.0, "Completely different symptoms" },
            new object[] { "ไข้", "", false, 0.0, "One empty string" },
        };
    }

    public static IEnumerable<object[]> GetTokenizationScenarios()
    {
        return new[]
        {
            new object[] { "ไข้", new[] { "ไข้" }, "Single symptom" },
            new object[] { "ไข้ ไอ", new[] { "ไข้", "ไอ" }, "Space separated" },
            new object[] { "ไข้-ไอ", new[] { "ไข้", "ไอ" }, "Hyphen separated" },
            new object[] { "ไข้ไอ", new[] { "ไข้", "ไอ" }, "Concatenated symptoms" },
            new object[] { "", new string[0], "Empty string" },
            new object[] { "   ", new string[0], "Whitespace only" },
            new object[] { "ไข้รถยนต์", new[] { "ไข้", "รถยนต์" }, "Medical + non-medical concatenated" },
        };
    }

    public static IEnumerable<object[]> GetSynonymScenarios()
    {
        return new[]
        {
            new object[] { "มีน้ำมูก", "น้ำมูก", "Nasal discharge synonym" },
            new object[] { "ปวดศีรษะ", "ปวดหัว", "Headache synonym" },
            new object[] { "BP ต่ำ", "BPต่ำ", "Blood pressure spacing" },
            new object[] { "2 ข้าง", "2ข้าง", "Number spacing" },
            new object[] { "มีไข้", "ไข้", "Prefix removal" },
        };
    }

    public static IEnumerable<object[]> GetTypoCorrectionScenarios()
    {
        return new[]
        {
            new object[] { "หายใจลำบาก", "หายใจลำบาด", "Common breathing difficulty typo" },
            new object[] { "เจ็บคอ", "เจ็บคล", "Throat pain typo" },
            new object[] { "น้ำมูก", "น้ำมุก", "Nasal discharge typo" },
            new object[] { "อ่อนแรง", "อ่อนแรค", "Weakness typo" },
        };
    }

    public static IEnumerable<object[]> GetCompoundSymptomScenarios()
    {
        return new[]
        {
            new object[] { "แขนซ้าย-ขาซ้ายอ่อนแรง", new[] { "แขนซ้ายอ่อนแรง", "ขาซ้ายอ่อนแรง" }, "Left side weakness" },
            new object[] { "แขนขวา-ขาขวาอ่อนแรง", new[] { "แขนขวาอ่อนแรง", "ขาขวาอ่อนแรง" }, "Right side weakness" },
            new object[] { "ขาซ้าย-แขนซ้ายอ่อนแรง", new[] { "ขาซ้ายอ่อนแรง", "แขนซ้ายอ่อนแรง" }, "Left side weakness reversed" },
        };
    }
}

[TestClass]
public class DataDrivenTests
{
    private ComparisonEngine _comparisonEngine = null!;

    [TestInitialize]
    public void Setup()
    {
        var tokenizer = new ThaiMedicalTokenizer(SymptomDictionary.DefaultSymptomDict);
        _comparisonEngine = new ComparisonEngine(tokenizer);
    }

    [TestMethod]
    [DynamicData(nameof(TestDataBuilder.GetBasicComparisonScenarios), typeof(TestDataBuilder), DynamicDataSourceType.Method)]
    public void CompareThaiMedicalTexts_BasicScenarios(string text1, string text2, bool expectedMatch, double expectedSimilarity, string description)
    {
        // Act
        var result = _comparisonEngine.CompareThaiMedicalTexts(text1, text2, 100.0);

        // Assert
        result.IsMatch.Should().Be(expectedMatch, $"Match result for scenario: {description}");

        if (expectedSimilarity > 0)
        {
            result.Similarity.Should().BeApproximately(expectedSimilarity, 0.5,
                $"Similarity for scenario: {description}");
        }
    }

    [TestMethod]
    [DynamicData(nameof(TestDataBuilder.GetTokenizationScenarios), typeof(TestDataBuilder), DynamicDataSourceType.Method)]
    public void TokenizeThaiSymptoms_VariousScenarios(string input, string[] expected, string description)
    {
        // Arrange
        var tokenizer = new ThaiMedicalTokenizer(SymptomDictionary.DefaultSymptomDict);

        // Act
        var result = tokenizer.TokenizeThaiSymptoms(input);

        // Assert
        result.Should().BeEquivalentTo(expected, $"Tokenization for scenario: {description}");
    }

    [TestMethod]
    [DynamicData(nameof(TestDataBuilder.GetCompoundSymptomScenarios), typeof(TestDataBuilder), DynamicDataSourceType.Method)]
    public void ExpandCompoundSymptoms_VariousScenarios(string input, string[] expectedTokens, string description)
    {
        // Arrange
        var tokenizer = new ThaiMedicalTokenizer(SymptomDictionary.DefaultSymptomDict);

        // Act
        var result = tokenizer.TokenizeThaiSymptoms(input);

        // Assert
        result.Should().Contain(expectedTokens, $"Compound expansion for scenario: {description}");
    }
}
