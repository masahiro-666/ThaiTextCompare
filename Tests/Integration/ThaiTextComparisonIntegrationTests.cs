using FluentAssertions;
using ThaiTextCompare.Core;

namespace ThaiTextCompare.Tests.Integration;

[TestClass]
public class ThaiTextComparisonIntegrationTests
{
    private ComparisonEngine _comparisonEngine = null!;

    [TestInitialize]
    public void Setup()
    {
        var tokenizer = new ThaiMedicalTokenizer(SymptomDictionary.DefaultSymptomDict);
        _comparisonEngine = new ComparisonEngine(tokenizer);
    }

    [TestMethod]
    public void CompleteWorkflow_WithRealMedicalScenarios_ShouldWorkCorrectly()
    {
        // Test cases from the original program
        var testCases = new[]
        {
            ("ไข้ ไอ เจ็บคอ มีน้ำมูก", "ไข้ ไอ เจ็บคอ มีน้ำมูก", true, "Exact match"),
            ("ไข้ ไอ เจ็บคอ มีน้ำมูก", "ไข้ ไอ เจ็บคอ มีน้ำมูก รถยนต์", false, "Exact match with extra unrelated word"),
            ("ไข้ ไอ เจ็บคอ มีน้ำมูก", "มีไข้ ไอ เจ็บคอ มีน้ำมูก", true, "With added prefix 'มี' on fever"),
            ("ไข้ ไอ เจ็บคอ มีน้ำมูก", "ไอ ไข้ มีน้ำมูก เจ็บคอ", true, "Different word order"),
            ("ไข้ ไอ เจ็บคอ มีน้ำมูก", "ไอไข้มีน้ำมูกเจ็บคอ", true, "All concatenated (no spaces)"),
            ("ไข้ ไอ เจ็บคอ มีน้ำมูก", "ไข้", false, "Single symptom subset"),
            ("ไข้ ไอ เจ็บคอ มีน้ำมูก", "ไอ เจ็บคอ", false, "Partial symptom subset"),
            ("ไข้ ไอ เจ็บคอ มีน้ำมูก", "ไข้ ไอ เจ็บคอ น้ำมูก", true, "Synonym variation"),
            ("แขนซ้าย-ขาซ้ายอ่อนแรง", "แขนซ้ายอ่อนแรง ขาซ้ายอ่อนแรง", true, "Compound limb weakness expanded"),
            ("hr 60", "HR 60", true, "Case normalization test - HR"),
            ("ecg normal", "ECG NORMAL", true, "Case normalization test - ECG"),
        };

        foreach (var (text1, text2, expectedMatch, description) in testCases)
        {
            // Act
            var result = _comparisonEngine.CompareThaiMedicalTexts(text1, text2, 100.0);

            // Assert
            result.IsMatch.Should().Be(expectedMatch,
                $"Test case '{description}' failed. Text1: '{text1}', Text2: '{text2}'");
        }
    }

    [TestMethod]
    public void EdgeCaseHandling_ShouldBeRobust()
    {
        var edgeCases = new[]
        {
            ("", "", true, "Both empty"),
            ("ไข้", "", false, "Empty text2"),
            ("", "ไข้", false, "Empty text1"),
            ("   ", "ไข้", false, "Whitespace only text1"),
            ("ไข้", "   ", false, "Whitespace only text2"),
        };

        foreach (var (text1, text2, expectedMatch, description) in edgeCases)
        {
            // Act
            var result = _comparisonEngine.CompareThaiMedicalTexts(text1, text2, 100.0);

            // Assert
            result.IsMatch.Should().Be(expectedMatch,
                $"Edge case '{description}' failed. Text1: '{text1}', Text2: '{text2}'");
        }
    }

    [TestMethod]
    public void PerformanceTest_WithLargeTexts_ShouldCompleteWithinReasonableTime()
    {
        // Arrange
        var largeText1 = string.Join(" ", Enumerable.Repeat("ไข้ ไอ เจ็บคอ น้ำมูก", 100));
        var largeText2 = string.Join(" ", Enumerable.Repeat("ไข้ ไอ เจ็บคอ น้ำมูก", 100));

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = _comparisonEngine.CompareThaiMedicalTexts(largeText1, largeText2, 100.0);
        stopwatch.Stop();

        // Assert
        result.IsMatch.Should().BeTrue();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000, "Should complete within 1 second");
    }

    [TestMethod]
    public void ComplexMedicalScenarios_ShouldHandleCorrectly()
    {
        // Test with real chief complaints from the JSON file
        var chiefComplaints = new[]
        {
            "ไข้ ไอ เจ็บคอ มีน้ำมูก",
            "หกล้ม มีแผลเล็กที่เข่า 2 ข้าง",
            "เพลีย ปวดท้อง ถ่ายเหลวเป็นน้ำ 2 ครั้ง",
            "ไข้ เพลีย เจ็บคอ ทานอาหารไม่ได้ ปวดศีรษะ",
            "ปวดหลัง",
            "เจ็บหน้าอกร้าวไปแขนซ้าย",
            "หายใจลำบาก ผื่นขึ้น BP ต่ำ (โดนผึ้งต่อย)",
            "หมดสติ มีเลือดไหลจากหู",
            "อาเจียนเป็นเลือดแดงสด หน้ามืด เหงื่อออก",
            "หายใจเหนื่อยมาก",
            "ไข้ หนาวสั่น ไม่ปัสสาวะ",
            "เพลีย ปวดท้อง ถ่ายเหลว 10 ครั้ง",
            "ไข้สูงและชัก",
            "แขนซ้าย-ขาซ้ายอ่อนแรง"
        };

        // Each complaint should match itself
        foreach (var complaint in chiefComplaints)
        {
            var result = _comparisonEngine.CompareThaiMedicalTexts(complaint, complaint, 100.0);
            result.IsMatch.Should().BeTrue($"Chief complaint should match itself: {complaint}");
            result.Similarity.Should().Be(100.0);
        }

        // Test cross-comparisons that should NOT match
        var result1 = _comparisonEngine.CompareThaiMedicalTexts(
            chiefComplaints[0], chiefComplaints[5], 100.0);
        result1.IsMatch.Should().BeFalse("Different complaints should not match exactly");

        // Test partial matches with lower threshold
        var result2 = _comparisonEngine.CompareThaiMedicalTexts(
            "ไข้ ไอ เจ็บคอ", "ไข้ ไอ", 60.0);
        result2.IsMatch.Should().BeTrue("Should match with lower threshold");
    }

    [TestMethod]
    public void TypoCorrection_ShouldWorkInIntegratedScenario()
    {
        // Test that typos are corrected in the full workflow
        var testCases = new[]
        {
            ("หายใจลำบาก", "หายใจลำบาด", true, "Common typo correction"),
            ("น้ำมูก", "น้ำมุก", true, "Character substitution typo"),
            ("เจ็บคอ", "เจ็บคล", true, "Typo correction test"),
        };

        foreach (var (correct, typo, shouldMatch, description) in testCases)
        {
            var result = _comparisonEngine.CompareThaiMedicalTexts(correct, typo, 100.0);
            result.IsMatch.Should().Be(shouldMatch, description);
        }
    }
}
