using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using ThaiTextCompare.Core;

namespace ThaiTextCompare.Tests.Performance;

[MemoryDiagnoser]
[SimpleJob]
public class ThaiTextComparisonBenchmarks
{
    private ComparisonEngine _comparisonEngine = null!;
    private string _shortText1 = null!;
    private string _shortText2 = null!;
    private string _mediumText1 = null!;
    private string _mediumText2 = null!;
    private string _longText1 = null!;
    private string _longText2 = null!;
    private string _concatenatedText1 = null!;
    private string _concatenatedText2 = null!;

    [GlobalSetup]
    public void Setup()
    {
        var tokenizer = new ThaiMedicalTokenizer(SymptomDictionary.DefaultSymptomDict);
        _comparisonEngine = new ComparisonEngine(tokenizer);

        // Short texts (typical use case)
        _shortText1 = "ไข้ ไอ เจ็บคอ มีน้ำมูก";
        _shortText2 = "ไข้ ไอ เจ็บคอ น้ำมูก";

        // Medium texts (realistic medical descriptions)
        _mediumText1 = "ไข้ ไอ เจ็บคอ มีน้ำมูก ปวดหัว เหงื่อออก หนาวสั่น ปวดท้อง ถ่ายเหลว";
        _mediumText2 = "ไข้ ไอ เจ็บคอ น้ำมูก ปวดศีรษะ เหงื่อออก หนาวสั่น ปวดท้อง ถ่ายเหลว เพลีย";

        // Long texts (stress test)
        _longText1 = string.Join(" ", Enumerable.Repeat("ไข้ ไอ เจ็บคอ มีน้ำมูก ปวดหัว", 50));
        _longText2 = string.Join(" ", Enumerable.Repeat("ไข้ ไอ เจ็บคอ น้ำมูก ปวดศีรษะ", 50));

        // Concatenated texts (no spaces)
        _concatenatedText1 = "ไข้ไอเจ็บคอมีน้ำมูกปวดหัวเหงื่อออกหนาวสั่น";
        _concatenatedText2 = "ไข้ไอเจ็บคอน้ำมูกปวดศีรษะเหงื่อออกหนาวสั่น";
    }

    [Benchmark]
    public ComparisonResult CompareShortTexts()
    {
        return _comparisonEngine.CompareThaiMedicalTexts(_shortText1, _shortText2, 100.0);
    }

    [Benchmark]
    public ComparisonResult CompareMediumTexts()
    {
        return _comparisonEngine.CompareThaiMedicalTexts(_mediumText1, _mediumText2, 100.0);
    }

    [Benchmark]
    public ComparisonResult CompareLongTexts()
    {
        return _comparisonEngine.CompareThaiMedicalTexts(_longText1, _longText2, 100.0);
    }

    [Benchmark]
    public ComparisonResult CompareConcatenatedTexts()
    {
        return _comparisonEngine.CompareThaiMedicalTexts(_concatenatedText1, _concatenatedText2, 100.0);
    }
}

[MemoryDiagnoser]
[SimpleJob]
public class TokenizationBenchmarks
{
    private ThaiMedicalTokenizer _tokenizer = null!;
    private string _shortText = null!;
    private string _mediumText = null!;
    private string _longText = null!;
    private string _concatenatedText = null!;
    private string _veryLongConcatenatedText = null!;

    [GlobalSetup]
    public void Setup()
    {
        _tokenizer = new ThaiMedicalTokenizer(SymptomDictionary.DefaultSymptomDict);

        _shortText = "ไข้ ไอ เจ็บคอ";
        _mediumText = "ไข้ ไอ เจ็บคอ มีน้ำมูก ปวดหัว เหงื่อออก หนาวสั่น";
        _longText = string.Join(" ", Enumerable.Repeat("ไข้ ไอ เจ็บคอ มีน้ำมูก", 100));
        _concatenatedText = "ไข้ไอเจ็บคอมีน้ำมูกปวดหัว";
        _veryLongConcatenatedText = "ไข้" + new string('A', 600) + "ไอเจ็บคอ";
    }

    [Benchmark]
    public List<string> TokenizeShortText()
    {
        return _tokenizer.TokenizeThaiSymptoms(_shortText);
    }

    [Benchmark]
    public List<string> TokenizeMediumText()
    {
        return _tokenizer.TokenizeThaiSymptoms(_mediumText);
    }

    [Benchmark]
    public List<string> TokenizeLongText()
    {
        return _tokenizer.TokenizeThaiSymptoms(_longText);
    }

    [Benchmark]
    public List<string> TokenizeConcatenatedText()
    {
        return _tokenizer.TokenizeThaiSymptoms(_concatenatedText);
    }

    [Benchmark]
    public List<string> TokenizeVeryLongConcatenatedText()
    {
        return _tokenizer.TokenizeThaiSymptoms(_veryLongConcatenatedText);
    }
}
