
using ThaiTextCompare.Core;

class Program
{
    static void Main()
    {
        // Initialize the system
        var tokenizer = new ThaiMedicalTokenizer(SymptomDictionary.DefaultSymptomDict);
        var comparisonEngine = new ComparisonEngine(tokenizer);

        // Test cases based on your TestCase scenarios
        var testCases = new[]
        {
            // Your specific test cases
            ("ไข้ ไอ เจ็บคอ มีน้ำมูก", "ไข้ ไอ เจ็บคอ มีน้ำมูก รถยนต์", "Exact match with extra unrelated word 1"),
            ("ไข้ ไอ เจ็บคอ มีน้ำมูก", "ไข้ ไอ เจ็บคอ มีน้ำมูก รถยนต์ ปลากระป๋องตราCoSI", "Exact match with extra unrelated word 2"),
            ("ไข้ ไอ เจ็บคอ มีน้ำมูก", "ไข้ ไอ เจ็บคอ มีน้ำมูกรถยนต์ปลากระป๋องตราCoSI", "Exact match with extra unrelated word 3"),
            ("ไข้ ไอ เจ็บคอ มีน้ำมูก", "ไข้ ไอ เจ็บคอ มีน้ำมูกรถยนต์ปลากระป๋องตราCoSIสีแดง", "Exact match with extra unrelated word 4"),
            ("ไข้ ไอ เจ็บคอ มีน้ำมูก", "ไข้ ไอ เจ็บคอ มีน้ำมูก", "Exact match"),
            ("ไข้ ไอ เจ็บคอ มีน้ำมูก", "มีไข้ ไอ เจ็บคอ มีน้ำมูก", "With added prefix 'มี' on fever"),
            ("ไข้ ไอ เจ็บคอ มีน้ำมูก", "ไอ ไข้ มีน้ำมูก เจ็บคอ", "Different word order"),
            ("ไข้ ไอ เจ็บคอ มีน้ำมูก", "ไอไข้มีน้ำมูกเจ็บคอ", "All concatenated (no spaces)"),
            ("ไข้ ไอ เจ็บคอ มีน้ำมูก", "ไข้ไอ เจ็บคอ มีน้ำมูก", "Partially concatenated"),
            ("ไข้ ไอ เจ็บคอ มีน้ำมูก", "ไข้", "Single symptom subset"),
            ("ไข้ ไอ เจ็บคอ มีน้ำมูก", "ไอ เจ็บคอ", "Partial symptom subset"),

            // Additional interesting scenarios
            ("เจ็บคอ กลืนลำบาก", "เจ็บคอ", "Complex vs simple symptom"),
            ("ไข้ ไอ เจ็บคอ มีน้ำมูก", "ไข้ ไอ เจ็บคอ น้ำมูก", "Synonym variation (มีน้ำมูก vs น้ำมูก)"),
            ("หายใจลำบาก ผื่นขึ้น", "หายใจลำบาก ผื่นขึ้น BP ต่ำ", "Subset vs superset"),
            
            // Compound symptoms
            ("แขนซ้าย-ขาซ้ายอ่อนแรง", "แขนซ้ายอ่อนแรง ขาซ้ายอ่อนแรง", "Compound limb weakness expanded"),

            ("ไข้ ไอ เจ็บคอ มีน้ำมูก", "มีน้ำมูก เจ็บคอ ไอ ไข้", "Partial concatenation"),
            ("ไข้ ไอ เจ็บคอ มีน้ำมูก", "มีน้ำมูกเจ็บคอไอไข้", "Partial concatenation"),

            // STRESS TEST CASES - Edge Cases and Error Detection
            
            // Empty and null-like cases
            ("ไข้", "", "Empty text2"),
            ("", "ไข้", "Empty text1"),
            ("", "", "Both empty"),
            ("   ", "ไข้", "Whitespace only text1"),
            ("ไข้", "   ", "Whitespace only text2"),
            
            // Single character and very short cases
            ("ไ", "ไ", "Single Thai character match"),
            ("ไ", "ข", "Single Thai character mismatch"),
            ("ไข้", "ไ", "Partial character match"),
            
            // Mixed language cases
            ("ไข้ fever", "ไข้ fever", "Mixed Thai-English exact"),
            ("ไข้ fever", "fever ไข้", "Mixed Thai-English reordered"),
            ("ไข้", "ไข้ fever", "Thai vs Mixed Thai-English"),
            ("ไข้ 123", "ไข้ 123", "Thai with numbers"),
            ("ไข้ ABC ไอ", "ไข้ ABC ไอ", "Thai with English letters"),
            
            // Special characters and symbols
            ("ไข้!@#", "ไข้!@#", "Thai with special symbols"),
            ("ไข้-ไอ", "ไข้-ไอ", "Thai with hyphen exact"),
            ("ไข้_ไอ", "ไข้_ไอ", "Thai with underscore"),
            ("ไข้(ไอ)", "ไข้(ไอ)", "Thai with parentheses"),
            
            // Very long concatenated cases
            ("ไข้", "ไข้ไอเจ็บคอมีน้ำมูกปวดหัวปวดท้องหนาวสั่นเหงื่อออก", "Short vs very long concatenated"),
            ("ไข้ ไอ เจ็บคอ", "ไข้ไอเจ็บคอรถยนต์บ้านโรงเรียนมหาวิทยาลัยโรงพยาบาล", "Medical vs mixed very long"),
            
            // Duplicate symptoms
            ("ไข้ ไข้", "ไข้", "Duplicate in text1"),
            ("ไข้", "ไข้ ไข้", "Duplicate in text2"),
            ("ไข้ ไข้", "ไข้ ไข้", "Duplicates in both"),
            ("ไข้ ไอ ไข้", "ไข้ ไอ", "Mixed with duplicates"),
            
            // Typo and fuzzy matching edge cases
            ("ปวดหัว", "ปวดศีรษะ", "Synonym mapping"),
            ("หายใจลำบาด", "หายใจลำบาก", "Common typo correction"),
            ("ไข้ไข้ไข้", "ไข้", "Triple repetition vs single"),
            ("เจ็บคล", "เจ็บคอ", "Typo correction test"),
            ("น้ำมุก", "น้ำมูก", "Character substitution typo"),
            
            // Boundary cases for compound symptoms
            ("แขนซ้ายอ่อนแรง", "แขนซ้าย-ขาซ้ายอ่อนแรง", "Single vs compound limb weakness"),
            ("ขาขวาอ่อนแรง", "แขนขวา-ขาขวาอ่อนแรง", "Partial compound match"),
            
            // Number and frequency edge cases
            ("ไข้ 2 ครั้ง", "ไข้", "With frequency vs without"),
            ("ปวดหัว 3 ข้าง", "ปวดหัว", "With location count vs without"),
            ("ไข้ (สูงมาก)", "ไข้", "With parenthetical info vs without"),
            
            // Extreme concatenation with mixed content
            ("น้ำมูก", "น้ำมูกABCDEFGHIJKLMNOPQRSTUVWXYZ123456789", "Medical + long English+numbers"),
            ("ไอ", "ไอรถไฟรถยนต์เครื่องบินเรือ", "Medical + transportation words"),
            
            // Case sensitivity and English mixing
            ("BP ต่ำ", "bp ต่ำ", "Case sensitivity test"),
            ("BPต่ำ", "BP ต่ำ", "Space variation in English part"),
            
            // Multiple extra words scenarios
            ("ไข้", "ไข้ A B C D E F G", "Single medical + many extra"),
            ("ไข้ ไอ", "ไข้ X ไอ Y", "Medical words separated by extra words"),
            
            // Extreme edge cases
            ("ไข้" + new string('A', 1000), "ไข้", "Very long text1 with extra"),
            // VERIFICATION TESTS for improvements
            ("hr 60", "HR 60", "Case normalization test - HR"),
            ("ecg normal", "ECG NORMAL", "Case normalization test - ECG"),
            ("ไข้ ct scan", "ไข้ CT SCAN", "Mixed case normalization"),
            ("น้ำมูกรถไฟABCDรถยนต์123", "น้ำมูก", "Thai-Latin boundary test"),
            ("ไข้" + new string('C', 600), "ไข้", "Performance test >500 chars"),

            ("ไข้ ไอ","ไข้", "Partial match with extra word in text"),
        };

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
            Console.WriteLine($"  Text1→Text2 Coverage: {result.Coverage1}% (Primary Score)");
            Console.WriteLine($"  Text2→Text1 Coverage: {result.Coverage2}%");
            Console.WriteLine($"  Jaccard Similarity: {jaccardSimilarity}% (Reference)");
            Console.WriteLine($"  Match Result: {result.IsMatch} (Text2 contains ALL Text1 words: {result.Coverage1 >= 100}%)");
            Console.WriteLine();
        }
    }
}
