using FluentAssertions;
using ThaiTextCompare.Core;

namespace ThaiTextCompare.Tests.Unit;

[TestClass]
public class ThaiMedicalTokenizerTests
{
    private ThaiMedicalTokenizer _tokenizer = null!;

    [TestInitialize]
    public void Setup()
    {
        _tokenizer = new ThaiMedicalTokenizer(SymptomDictionary.DefaultSymptomDict);
    }

    [TestClass]
    public class TokenizeThaiSymptomsTests : ThaiMedicalTokenizerTests
    {
        [TestMethod]
        public void TokenizeThaiSymptoms_WithEmptyString_ShouldReturnEmptyList()
        {
            // Arrange
            var input = "";

            // Act
            var result = _tokenizer.TokenizeThaiSymptoms(input);

            // Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void TokenizeThaiSymptoms_WithWhitespaceOnly_ShouldReturnEmptyList()
        {
            // Arrange
            var input = "   ";

            // Act
            var result = _tokenizer.TokenizeThaiSymptoms(input);

            // Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void TokenizeThaiSymptoms_WithSingleSymptom_ShouldReturnSingleToken()
        {
            // Arrange
            var input = "ไข้";

            // Act
            var result = _tokenizer.TokenizeThaiSymptoms(input);

            // Assert
            result.Should().ContainSingle().Which.Should().Be("ไข้");
        }

        [TestMethod]
        public void TokenizeThaiSymptoms_WithMultipleSymptomsSpaceSeparated_ShouldReturnAllTokens()
        {
            // Arrange
            var input = "ไข้ ไอ เจ็บคอ";

            // Act
            var result = _tokenizer.TokenizeThaiSymptoms(input);

            // Assert
            result.Should().HaveCount(3);
            result.Should().Contain(new[] { "ไข้", "ไอ", "เจ็บคอ" });
        }

        [TestMethod]
        public void TokenizeThaiSymptoms_WithConcatenatedSymptoms_ShouldReturnAllTokens()
        {
            // Arrange
            var input = "ไข้ไอเจ็บคอ";

            // Act
            var result = _tokenizer.TokenizeThaiSymptoms(input);

            // Assert
            result.Should().HaveCount(3);
            result.Should().Contain(new[] { "ไข้", "ไอ", "เจ็บคอ" });
        }

        [TestMethod]
        public void TokenizeThaiSymptoms_WithSynonyms_ShouldNormalizeToPreferredForm()
        {
            // Arrange
            var input = "มีน้ำมูก ปวดศีรษะ";

            // Act
            var result = _tokenizer.TokenizeThaiSymptoms(input);

            // Assert
            result.Should().Contain("น้ำมูก");
            result.Should().Contain("ปวดหัว");
        }

        [TestMethod]
        public void TokenizeThaiSymptoms_WithTypos_ShouldCorrectCommonMistakes()
        {
            // Arrange
            var input = "หายใจลำบาด เจ็บคล";

            // Act
            var result = _tokenizer.TokenizeThaiSymptoms(input);

            // Assert
            result.Should().Contain("หายใจลำบาก");
            result.Should().Contain("เจ็บคอ");
        }

        [TestMethod]
        public void TokenizeThaiSymptoms_WithMixedLanguage_ShouldHandleCorrectly()
        {
            // Arrange
            var input = "ไข้ BP ต่ำ";

            // Act
            var result = _tokenizer.TokenizeThaiSymptoms(input);

            // Assert
            result.Should().Contain("ไข้");
            result.Should().Contain("BPต่ำ");
        }

        [TestMethod]
        public void TokenizeThaiSymptoms_WithCompoundSymptoms_ShouldExpand()
        {
            // Arrange
            var input = "แขนซ้าย-ขาซ้ายอ่อนแรง";

            // Act
            var result = _tokenizer.TokenizeThaiSymptoms(input);

            // Assert
            result.Should().Contain("แขนซ้ายอ่อนแรง");
            result.Should().Contain("ขาซ้ายอ่อนแรง");
        }

        [TestMethod]
        public void TokenizeThaiSymptoms_WithNonMedicalWords_ShouldIncludeAsTokens()
        {
            // Arrange
            var input = "ไข้ รถยนต์";

            // Act
            var result = _tokenizer.TokenizeThaiSymptoms(input);

            // Assert
            result.Should().Contain("ไข้");
            result.Should().Contain("รถยนต์");
        }

        [TestMethod]
        public void TokenizeThaiSymptoms_WithLongConcatenatedText_ShouldUseOptimizedParsing()
        {
            // Arrange
            var longText = "ไข้" + new string('A', 500) + "ไอ";

            // Act
            var result = _tokenizer.TokenizeThaiSymptoms(longText);

            // Assert
            result.Should().Contain("ไข้");
            result.Should().Contain("ไอ");
            result.Count.Should().BeGreaterThan(2); // Should include the long non-medical segment
        }
    }

    [TestClass]
    public class ExpandCompoundSymptomsTests : ThaiMedicalTokenizerTests
    {
        [TestMethod]
        public void ExpandCompoundSymptoms_WithLeftSideWeakness_ShouldExpandCorrectly()
        {
            // Arrange
            var input = "แขนซ้าย-ขาซ้ายอ่อนแรง";

            // Act
            var result = _tokenizer.ExpandCompoundSymptoms(input);

            // Assert
            result.Should().Be("แขนซ้ายอ่อนแรง ขาซ้ายอ่อนแรง");
        }

        [TestMethod]
        public void ExpandCompoundSymptoms_WithRightSideWeakness_ShouldExpandCorrectly()
        {
            // Arrange
            var input = "แขนขวา-ขาขวาอ่อนแรง";

            // Act
            var result = _tokenizer.ExpandCompoundSymptoms(input);

            // Assert
            result.Should().Be("แขนขวาอ่อนแรง ขาขวาอ่อนแรง");
        }

        [TestMethod]
        public void ExpandCompoundSymptoms_WithFrequencyIndicators_ShouldRemoveThem()
        {
            // Arrange
            var input = "ปวดท้อง 3 ครั้ง";

            // Act
            var result = _tokenizer.ExpandCompoundSymptoms(input);

            // Assert
            result.Should().Be("ปวดท้อง");
        }

        [TestMethod]
        public void ExpandCompoundSymptoms_WithParentheticalContent_ShouldRemoveIt()
        {
            // Arrange
            var input = "ไข้ (สูงมาก)";

            // Act
            var result = _tokenizer.ExpandCompoundSymptoms(input);

            // Assert
            result.Should().Be("ไข้");
        }
    }

    [TestClass]
    public class NormalizeSpacesTests : ThaiMedicalTokenizerTests
    {
        [TestMethod]
        public void NormalizeSpaces_WithMultipleSpaces_ShouldNormalizeToSingleSpace()
        {
            // Arrange
            var input = "ไข้    ไอ     เจ็บคอ";

            // Act
            var result = _tokenizer.NormalizeSpaces(input);

            // Assert
            result.Should().Be("ไข้ ไอ เจ็บคอ");
        }

        [TestMethod]
        public void NormalizeSpaces_WithLeadingTrailingSpaces_ShouldTrim()
        {
            // Arrange
            var input = "   ไข้ ไอ   ";

            // Act
            var result = _tokenizer.NormalizeSpaces(input);

            // Assert
            result.Should().Be("ไข้ ไอ");
        }

        [TestMethod]
        public void NormalizeSpaces_WithMedicalAbbreviations_ShouldNormalizeCase()
        {
            // Arrange
            var input = "bp ต่ำ hr 60 ecg normal";

            // Act
            var result = _tokenizer.NormalizeSpaces(input);

            // Assert
            result.Should().Be("BP ต่ำ HR 60 ECG normal");
        }
    }

    [TestClass]
    public class IsThaiCharacterTests : ThaiMedicalTokenizerTests
    {
        [TestMethod]
        public void IsThaiCharacter_WithThaiCharacter_ShouldReturnTrue()
        {
            // Arrange & Act & Assert
            ThaiMedicalTokenizer.IsThaiCharacter('ก').Should().BeTrue();
            ThaiMedicalTokenizer.IsThaiCharacter('ไ').Should().BeTrue();
            ThaiMedicalTokenizer.IsThaiCharacter('๙').Should().BeTrue();
        }

        [TestMethod]
        public void IsThaiCharacter_WithNonThaiCharacter_ShouldReturnFalse()
        {
            // Arrange & Act & Assert
            ThaiMedicalTokenizer.IsThaiCharacter('A').Should().BeFalse();
            ThaiMedicalTokenizer.IsThaiCharacter('1').Should().BeFalse();
            ThaiMedicalTokenizer.IsThaiCharacter('!').Should().BeFalse();
        }
    }
}
