using FluentAssertions;
using ThaiTextCompare.Core;

namespace ThaiTextCompare.Tests.Unit;

[TestClass]
public class ComparisonEngineTests
{
    private ComparisonEngine _comparisonEngine = null!;
    private ThaiMedicalTokenizer _tokenizer = null!;

    [TestInitialize]
    public void Setup()
    {
        _tokenizer = new ThaiMedicalTokenizer(SymptomDictionary.DefaultSymptomDict);
        _comparisonEngine = new ComparisonEngine(_tokenizer);
    }

    [TestClass]
    public class CompareThaiMedicalTextsTests : ComparisonEngineTests
    {
        [TestMethod]
        public void CompareThaiMedicalTexts_WithExactMatch_ShouldReturn100PercentSimilarity()
        {
            // Arrange
            var text1 = "ไข้ ไอ เจ็บคอ";
            var text2 = "ไข้ ไอ เจ็บคอ";

            // Act
            var result = _comparisonEngine.CompareThaiMedicalTexts(text1, text2, 100.0);

            // Assert
            result.Similarity.Should().Be(100.0);
            result.IsMatch.Should().BeTrue();
            result.Coverage1.Should().Be(100.0);
            result.Coverage2.Should().Be(100.0);
            result.MatchedWords.Should().HaveCount(3);
            result.MissingWords.Should().BeEmpty();
            result.ExtraWords.Should().BeEmpty();
        }

        [TestMethod]
        public void CompareThaiMedicalTexts_WithPartialMatch_ShouldReturnCorrectCoverage()
        {
            // Arrange
            var text1 = "ไข้ ไอ เจ็บคอ น้ำมูก";
            var text2 = "ไข้ ไอ เจ็บคอ";

            // Act
            var result = _comparisonEngine.CompareThaiMedicalTexts(text1, text2, 100.0);

            // Assert
            result.Coverage1.Should().Be(75.0); // 3 out of 4 words found
            result.Coverage2.Should().Be(100.0); // All words in text2 found in text1
            result.IsMatch.Should().BeFalse(); // Because coverage1 < 100%
            result.MissingWords.Should().Contain("น้ำมูก");
        }

        [TestMethod]
        public void CompareThaiMedicalTexts_WithExtraWords_ShouldIdentifyThem()
        {
            // Arrange
            var text1 = "ไข้ ไอ";
            var text2 = "ไข้ ไอ เจ็บคอ น้ำมูก";

            // Act
            var result = _comparisonEngine.CompareThaiMedicalTexts(text1, text2, 100.0);

            // Assert
            result.Coverage1.Should().Be(100.0); // All words in text1 found
            result.Coverage2.Should().Be(50.0);  // 2 out of 4 words in text2 found in text1
            result.ExtraWords.Should().HaveCount(2);
            result.ExtraWords.Should().Contain("เจ็บคอ");
            result.ExtraWords.Should().Contain("น้ำมูก");
        }

        [TestMethod]
        public void CompareThaiMedicalTexts_WithDifferentWordOrder_ShouldMatchCorrectly()
        {
            // Arrange
            var text1 = "ไข้ ไอ เจ็บคอ น้ำมูก";
            var text2 = "น้ำมูก เจ็บคอ ไอ ไข้";

            // Act
            var result = _comparisonEngine.CompareThaiMedicalTexts(text1, text2, 100.0);

            // Assert
            result.Similarity.Should().Be(100.0);
            result.IsMatch.Should().BeTrue();
            result.MatchedWords.Should().HaveCount(4);
        }

        [TestMethod]
        public void CompareThaiMedicalTexts_WithSynonyms_ShouldRecognizeThem()
        {
            // Arrange
            var text1 = "มีน้ำมูก ปวดศีรษะ";
            var text2 = "น้ำมูก ปวดหัว";

            // Act
            var result = _comparisonEngine.CompareThaiMedicalTexts(text1, text2, 100.0);

            // Assert
            result.Similarity.Should().Be(100.0);
            result.IsMatch.Should().BeTrue();
        }

        [TestMethod]
        public void CompareThaiMedicalTexts_WithEmptyStrings_ShouldHandleGracefully()
        {
            // Arrange
            var text1 = "";
            var text2 = "";

            // Act
            var result = _comparisonEngine.CompareThaiMedicalTexts(text1, text2, 100.0);

            // Assert
            result.Similarity.Should().Be(100.0);
            result.IsMatch.Should().BeTrue();
        }

        [TestMethod]
        public void CompareThaiMedicalTexts_WithOneEmptyString_ShouldReturnZeroSimilarity()
        {
            // Arrange
            var text1 = "ไข้";
            var text2 = "";

            // Act
            var result = _comparisonEngine.CompareThaiMedicalTexts(text1, text2, 100.0);

            // Assert
            result.Similarity.Should().Be(0.0);
            result.IsMatch.Should().BeFalse();
        }

        [TestMethod]
        public void CompareThaiMedicalTexts_WithCustomThreshold_ShouldRespectThreshold()
        {
            // Arrange
            var text1 = "ไข้ ไอ เจ็บคอ น้ำมูก";
            var text2 = "ไข้ ไอ เจ็บคอ";
            var threshold = 75.0;

            // Act
            var result = _comparisonEngine.CompareThaiMedicalTexts(text1, text2, threshold);

            // Assert
            result.Coverage1.Should().Be(75.0);
            result.IsMatch.Should().BeTrue(); // Because coverage1 >= threshold
        }

        [TestMethod]
        public void CompareThaiMedicalTexts_WithConcatenatedText_ShouldParseCorrectly()
        {
            // Arrange
            var text1 = "ไข้ ไอ เจ็บคอ";
            var text2 = "ไข้ไอเจ็บคอ";

            // Act
            var result = _comparisonEngine.CompareThaiMedicalTexts(text1, text2, 100.0);

            // Assert
            result.Similarity.Should().Be(100.0);
            result.IsMatch.Should().BeTrue();
        }

        [TestMethod]
        public void CompareThaiMedicalTexts_WithNonMedicalWords_ShouldIncludeThem()
        {
            // Arrange
            var text1 = "ไข้ รถยนต์";
            var text2 = "ไข้ รถยนต์ ปลากระป๋อง";

            // Act
            var result = _comparisonEngine.CompareThaiMedicalTexts(text1, text2, 100.0);

            // Assert
            result.Coverage1.Should().Be(100.0); // All words in text1 found
            result.Coverage2.Should().BeLessThan(100.0); // Extra word in text2
            result.ExtraWords.Should().Contain("ปลากระป๋อง");
        }

        [TestMethod]
        public void CompareThaiMedicalTexts_WithDuplicateWords_ShouldHandleCorrectly()
        {
            // Arrange
            var text1 = "ไข้ ไข้ ไอ";
            var text2 = "ไข้ ไอ";

            // Act
            var result = _comparisonEngine.CompareThaiMedicalTexts(text1, text2, 100.0);

            // Assert
            result.Similarity.Should().Be(100.0); // Duplicates are removed
            result.IsMatch.Should().BeTrue();
        }
    }

    [TestClass]
    public class ComparisonEngineConstructorTests : ComparisonEngineTests
    {
        [TestMethod]
        public void Constructor_WithNullTokenizer_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            Action act = () => new ComparisonEngine(null!);
            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("tokenizer");
        }
    }
}
