
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

class Program
{
    // Dictionary built from all ChiefComplaint terms + base terms
    static readonly string[] SymptomDict = new[]
    {
        // Comprehensive symptom list with synonyms
        "เจ็บหน้าอกร้าวไปแขนซ้าย",
        "อาเจียนเป็นเลือดแดงสด",
        "หายใจเหนื่อยมาก",
        "ทานอาหารไม่ได้",
        "หายใจลำบาก",
        "มีแผลเล็กที่เข่า",
        "ถ่ายเหลวเป็นน้ำ",
        "มีเลือดไหลจากหู",
        "ไข้สูงและชัก",
        "โดนผึ้งต่อย",
        "แขนซ้ายอ่อนแรง",
        "ขาซ้ายอ่อนแรง",
        "แขนขวาอ่อนแรง",
        "ขาขวาอ่อนแรง",
        "ไม่ปัสสาวะ",
        "ปวดศีรษะ",
        "ปวดหัว", // synonym for ปวดศีรษะ
        "เหงื่อออก",
        "ถ่ายเหลว",
        "หนาวสั่น",
        "ปวดท้อง",
        "ปวดหลัง",
        "หน้ามืด",
        "เจ็บคอ",
        "มีน้ำมูก",
        "น้ำมูก", // synonym for มีน้ำมูก
        "ผื่นขึ้น",
        "หมดสติ",
        "ตาแดง",
        "หกล้ม",
        "เพลีย",
        "BPต่ำ",
        "BP ต่ำ",
        "2ข้าง",
        "2 ข้าง",
        "ข้าง",
        "ครั้ง",
        "ไข้",
        "ไอ"
    };

    static void Main()
    {
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
        };

        Console.WriteLine("=== Test Case Scenarios Analysis (Coverage-Based Matching) ===\n");

        foreach (var (text1, text2, description) in testCases)
        {
            // Use the helper method for comparison
            var (coverageSimilarity, isMatch, intersection, union, coverage1, coverage2) =
                CompareThaiMedicalTexts(text1, text2, 100.0); // 100% threshold means ALL words must be present

            var tokens1 = TokenizeThaiSymptoms(text1, SymptomDict);
            var tokens2 = TokenizeThaiSymptoms(text2, SymptomDict);
            var set1 = tokens1.Distinct().ToHashSet();
            var set2 = tokens2.Distinct().ToHashSet();

            var missingWords = set1.Except(set2).ToList();
            var extraWords = set2.Except(set1).ToList();
            var matchedWords = set1.Intersect(set2).ToList();

            // Calculate Jaccard for reference
            double jaccardSimilarity = union == 0 ? 0 : Math.Round((double)intersection / union * 100, 2);

            Console.WriteLine($"{description}:");
            Console.WriteLine($"  Text1: '{text1}'");
            Console.WriteLine($"  Text2: '{text2}'");
            Console.WriteLine($"  Tokens1: [{string.Join(", ", set1)}] ({set1.Count} tokens)");
            Console.WriteLine($"  Tokens2: [{string.Join(", ", set2)}] ({set2.Count} tokens)");
            Console.WriteLine($"  ✅ Matched: [{string.Join(", ", matchedWords)}] ({matchedWords.Count} words)");
            Console.WriteLine($"  ❌ Missing from Text2: [{string.Join(", ", missingWords)}] ({missingWords.Count} words)");
            Console.WriteLine($"  ➕ Extra in Text2: [{string.Join(", ", extraWords)}] ({extraWords.Count} words)");
            Console.WriteLine($"  Text1→Text2 Coverage: {coverage1}% (Primary Score)");
            Console.WriteLine($"  Text2→Text1 Coverage: {coverage2}%");
            Console.WriteLine($"  Jaccard Similarity: {jaccardSimilarity}% (Reference)");
            Console.WriteLine($"  Match Result: {isMatch} (Text2 contains ALL Text1 words: {coverage1 >= 100}%)");
            Console.WriteLine();
        }
    }

    static List<string> TokenizeThaiSymptoms(string text, IEnumerable<string> dict)
    {
        text = NormalizeSpaces(text);
        text = ExpandCompoundSymptoms(text);

        // Create a set of all dictionary terms for quick lookup
        var dictSet = new HashSet<string>(dict);
        var sortedDict = dict.OrderByDescending(d => d.Length).ToList();

        if (text.Contains(' ') || text.Contains('-'))
        {
            var words = text.Split(new char[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
            var result = new List<string>();

            foreach (var word in words)
            {
                // Direct match first
                if (dictSet.Contains(word))
                {
                    result.Add(word);
                }
                else
                {
                    // Try fuzzy matching for typos
                    var bestMatch = FindBestFuzzyMatch(word, dictSet);
                    if (bestMatch != null)
                    {
                        result.Add(bestMatch);
                    }
                    else
                    {
                        // Try to find the longest matching substring
                        var matches = dictSet.Where(d => word.Contains(d)).ToList(); // Only check if word contains dict term, not vice versa
                        if (matches.Any())
                        {
                            // If word contains dictionary terms but has extra characters, parse it as concatenated
                            if (matches.Any(m => m != word)) // word contains dict terms but is not an exact match
                            {
                                var subTokens = ParseConcatenatedWord(word, dictSet, sortedDict);
                                result.AddRange(subTokens);
                            }
                            else
                            {
                                // Add the longest match
                                var longestMatch = matches.OrderByDescending(m => m.Length).First();
                                if (!result.Contains(longestMatch))
                                {
                                    result.Add(longestMatch);
                                }
                            }
                        }
                        else
                        {
                            // This word might be concatenated dictionary words + non-dictionary words
                            // Use the concatenated parsing logic on this single word
                            var subTokens = ParseConcatenatedWord(word, dictSet, sortedDict);
                            result.AddRange(subTokens);
                        }
                    }
                }
            }

            return result;
        }

        // No spaces -> greedy maximum matching using dict (with performance optimization)
        // Performance optimization: use optimized approach for very long strings
        if (text.Length > 500)
        {
            return OptimizedConcatenatedParsing(text, dictSet, sortedDict);
        }

        var matchResult = new List<string>();
        int i = 0;
        while (i < text.Length)
        {
            string? hit = null;
            foreach (var term in sortedDict)
            {
                if (i + term.Length <= text.Length &&
                    string.CompareOrdinal(text, i, term, 0, term.Length) == 0)
                {
                    hit = term;
                    break;
                }
            }
            if (hit != null)
            {
                matchResult.Add(hit);
                i += hit.Length;
            }
            else
            {
                // For unmatched characters, collect consecutive non-dictionary characters as one word
                int nonDictStart = i;

                // Find the end of the non-dictionary sequence
                while (i < text.Length)
                {
                    bool foundDictWordAtI = false;
                    foreach (var term in sortedDict)
                    {
                        if (i + term.Length <= text.Length &&
                            string.CompareOrdinal(text, i, term, 0, term.Length) == 0)
                        {
                            foundDictWordAtI = true;
                            break;
                        }
                    }
                    if (foundDictWordAtI)
                    {
                        break;
                    }
                    i++;
                }

                // Add the non-dictionary segment as a single token
                if (i > nonDictStart)
                {
                    var nonDictWord = text.Substring(nonDictStart, i - nonDictStart);
                    matchResult.Add(nonDictWord);
                }
            }
        }
        return matchResult;
    }

    // Parse a single concatenated word that might contain dictionary words + non-dictionary words
    static List<string> ParseConcatenatedWord(string word, HashSet<string> dictSet, List<string> sortedDict)
    {
        var result = new List<string>();
        int i = 0;
        while (i < word.Length)
        {
            string? hit = null;
            foreach (var term in sortedDict)
            {
                if (i + term.Length <= word.Length &&
                    string.CompareOrdinal(word, i, term, 0, term.Length) == 0)
                {
                    hit = term;
                    break;
                }
            }
            if (hit != null)
            {
                result.Add(hit);
                i += hit.Length;
            }
            else
            {
                // Enhanced Thai word boundary detection
                var nonDictSegment = ExtractNonDictionarySegment(word, i, sortedDict);
                if (!string.IsNullOrEmpty(nonDictSegment))
                {
                    result.Add(nonDictSegment);
                    i += nonDictSegment.Length;
                }
                else
                {
                    // Fallback: single character
                    result.Add(word.Substring(i, 1));
                    i++;
                }
            }
        }
        return result;
    }

    // Enhanced method to extract meaningful non-dictionary segments
    static string ExtractNonDictionarySegment(string word, int startIndex, List<string> sortedDict)
    {
        int i = startIndex;
        var segment = new StringBuilder();

        while (i < word.Length)
        {
            // Check if we've hit the start of a dictionary word
            bool foundDictWordAtI = false;
            foreach (var term in sortedDict)
            {
                if (i + term.Length <= word.Length &&
                    string.CompareOrdinal(word, i, term, 0, term.Length) == 0)
                {
                    foundDictWordAtI = true;
                    break;
                }
            }

            if (foundDictWordAtI)
            {
                break;
            }

            char currentChar = word[i];
            segment.Append(currentChar);

            // Smart boundary detection for better word segmentation
            if (segment.Length > 0)
            {
                // Break at script boundaries (Thai to Latin, numbers, etc.)
                if (i + 1 < word.Length)
                {
                    char nextChar = word[i + 1];

                    // Thai character followed by Latin/number
                    if (IsThaiCharacter(currentChar) && !IsThaiCharacter(nextChar))
                    {
                        i++;
                        break;
                    }

                    // Latin/number followed by Thai character
                    if (!IsThaiCharacter(currentChar) && IsThaiCharacter(nextChar))
                    {
                        i++;
                        break;
                    }

                    // Break at reasonable lengths to prevent extremely long segments
                    if (segment.Length >= 50) // Configurable max segment length
                    {
                        i++;
                        break;
                    }
                }
            }

            i++;
        }

        return segment.ToString();
    }

    // Helper method to identify Thai characters
    static bool IsThaiCharacter(char c)
    {
        // Thai Unicode range: U+0E00–U+0E7F
        return c >= 0x0E00 && c <= 0x0E7F;
    }

    // Optimized parsing for very long concatenated strings
    static List<string> OptimizedConcatenatedParsing(string text, HashSet<string> dictSet, List<string> sortedDict)
    {
        var result = new List<string>();
        int i = 0;

        // Use a more efficient approach for very long strings
        // Build a lookup table for faster dictionary matching
        var dictLookup = new Dictionary<char, List<string>>();
        foreach (var term in sortedDict)
        {
            if (term.Length > 0)
            {
                char firstChar = term[0];
                if (!dictLookup.ContainsKey(firstChar))
                {
                    dictLookup[firstChar] = new List<string>();
                }
                dictLookup[firstChar].Add(term);
            }
        }

        while (i < text.Length)
        {
            string? hit = null;
            char currentChar = text[i];

            // Only check dictionary words that start with the current character
            if (dictLookup.ContainsKey(currentChar))
            {
                foreach (var term in dictLookup[currentChar])
                {
                    if (i + term.Length <= text.Length &&
                        string.CompareOrdinal(text, i, term, 0, term.Length) == 0)
                    {
                        hit = term;
                        break;
                    }
                }
            }

            if (hit != null)
            {
                result.Add(hit);
                i += hit.Length;
            }
            else
            {
                // Enhanced segment extraction for better word boundaries
                var segment = ExtractNonDictionarySegment(text, i, sortedDict);
                if (!string.IsNullOrEmpty(segment))
                {
                    result.Add(segment);
                    i += segment.Length;
                }
                else
                {
                    // Fallback
                    result.Add(text.Substring(i, 1));
                    i++;
                }
            }
        }

        return result;
    }    // Simple fuzzy matching for Thai typos
    static string? FindBestFuzzyMatch(string word, HashSet<string> dictSet)
    {
        if (word.Length < 3) return null; // Too short for fuzzy matching

        var candidates = dictSet
            .Where(d => Math.Abs(d.Length - word.Length) <= 2) // Similar length
            .Where(d => CalculateSimilarity(word, d) >= 0.7) // At least 70% similar
            .OrderByDescending(d => CalculateSimilarity(word, d))
            .ToList();

        return candidates.FirstOrDefault();
    }

    // Calculate character-level similarity (Jaccard similarity for characters)
    static double CalculateSimilarity(string s1, string s2)
    {
        if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2)) return 0;
        if (s1 == s2) return 1;

        var chars1 = new HashSet<char>(s1);
        var chars2 = new HashSet<char>(s2);

        var intersection = chars1.Intersect(chars2).Count();
        var union = chars1.Union(chars2).Count();

        return union == 0 ? 0 : (double)intersection / union;
    }

    // Compare two Thai medical texts and return detailed comparison results
    static (double similarity, bool isMatch, int intersection, int union, double coverage1, double coverage2)
        CompareThaiMedicalTexts(string text1, string text2, double threshold = 75.0)
    {
        var tokens1 = TokenizeThaiSymptoms(text1, SymptomDict);
        var tokens2 = TokenizeThaiSymptoms(text2, SymptomDict);

        var set1 = tokens1.Distinct().ToHashSet();
        var set2 = tokens2.Distinct().ToHashSet();

        int inter = set1.Intersect(set2).Count();
        int union = set1.Union(set2).Count();
        double jaccardSimilarity = union == 0 ? 0 : Math.Round((double)inter / union * 100, 2);

        double coverage1 = set1.Count == 0 ? 0 : Math.Round((double)inter / set1.Count * 100, 2);
        double coverage2 = set2.Count == 0 ? 0 : Math.Round((double)inter / set2.Count * 100, 2);

        // New logic: Text2 must contain ALL words from Text1 AND no extra words for True result
        // The score decreases based on missing words from Text1
        bool isMatch = coverage1 >= threshold && coverage2 >= threshold; // Both coverages must meet threshold (exact match)

        // Use coverage1 as the primary similarity score (how much of Text1 is found in Text2)
        double primarySimilarity = coverage1;

        return (primarySimilarity, isMatch, inter, union, coverage1, coverage2);
    }

    static string ExpandCompoundSymptoms(string text)
    {
        // Handle synonyms, variations, and common typos
        var synonymMappings = new Dictionary<string, string>
        {
            // Synonyms
            {"มีน้ำมูก", "น้ำมูก"},
            {"ปวดศีรษะ", "ปวดหัว"},
            {"BP ต่ำ", "BPต่ำ"},
            {"2 ข้าง", "2ข้าง"},
            {"มีไข้", "ไข้"},         // Common prefix variation
            
            // Common Thai typos and character variations
            // {"ไข", "ไข้"},           // Removed: This causes problems with existing "ไข้" words
            {"ปวดศรีษะ", "ปวดศีรษะ"},  // Common misspelling
            {"หายใจลำบาด", "หายใจลำบาก"}, // Common typo
            {"หายใจลำบาค", "หายใจลำบาก"}, // Another common typo
            {"เจ็บคล", "เจ็บคอ"},      // Typo
            {"มีน้ำมุก", "มีน้ำมูก"},   // Nasal discharge typo
            {"น้ำมุก", "น้ำมูก"},      // Short form typo
            {"อ่อนแรค", "อ่อนแรง"},    // Common typo
            {"หมดสิต", "หมดสติ"},     // Unconscious typo
            {"หกล้น", "หกล้ม"},       // Typo
            {"ผื่นขึน", "ผื่นขึ้น"},    // Missing tone mark
            {"เหงือออก", "เหงื่อออก"},  // Common typo
            {"ถ่ายเหลอ", "ถ่ายเหลว"},  // Typo
            {"วิงเวียน", "หน้ามืด"},    // Dizziness synonym
            {"เวียนหัว", "หน้ามืด"},    // Another dizziness synonym
        };

        // Apply synonym mappings and typo corrections
        foreach (var (from, to) in synonymMappings)
        {
            text = text.Replace(from, to);
        }

        // Remove numbers and frequency indicators for better matching
        text = Regex.Replace(text, @"\d+\s*(ครั้ง|ข้าง)", "");
        text = Regex.Replace(text, @"\(.*?\)", ""); // Remove parenthetical content
        text = text.Trim();

        // Handle left side compound patterns
        if (text == "แขนซ้าย-ขาซ้ายอ่อนแรง" || text == "แขนซ้ายขาซ้ายอ่อนแรง")
        {
            return "แขนซ้ายอ่อนแรง ขาซ้ายอ่อนแรง";
        }
        if (text == "ขาซ้าย-แขนซ้ายอ่อนแรง" || text == "ขาซ้ายแขนซ้ายอ่อนแรง")
        {
            return "ขาซ้ายอ่อนแรง แขนซ้ายอ่อนแรง";
        }

        // Handle right side compound patterns
        if (text == "แขนขวา-ขาขวาอ่อนแรง" || text == "แขนขวาขาขวาอ่อนแรง")
        {
            return "แขนขวาอ่อนแรง ขาขวาอ่อนแรง";
        }
        if (text == "ขาขวา-แขนขวาอ่อนแรง" || text == "ขาขวาแขนขวาอ่อนแรง")
        {
            return "ขาขวาอ่อนแรง แขนขวาอ่อนแรง";
        }

        return text;
    }

    static string NormalizeSpaces(string s)
    {
        s = s.Trim();
        s = Regex.Replace(s, @"\s+", " ");

        // Case normalization for English medical abbreviations
        s = NormalizeMedicalAbbreviations(s);

        return s;
    }

    static string NormalizeMedicalAbbreviations(string text)
    {
        // Common medical abbreviations that should be case-insensitive
        var medicalAbbreviations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"bp", "BP"},
            {"hr", "HR"},
            {"rr", "RR"},
            {"pr", "PR"},
            {"ecg", "ECG"},
            {"ekg", "EKG"},
            {"ct", "CT"},
            {"mri", "MRI"},
            {"xray", "XRAY"},
            {"x-ray", "XRAY"},
            {"iv", "IV"},
            {"im", "IM"},
            {"po", "PO"},
            {"prn", "PRN"},
            {"bid", "BID"},
            {"tid", "TID"},
            {"qid", "QID"},
            {"qd", "QD"},
            {"icu", "ICU"},
            {"er", "ER"},
            {"or", "OR"},
            {"cc", "CC"},
            {"ml", "ML"},
            {"mg", "MG"},
            {"kg", "KG"},
            {"lb", "LB"},
            {"cm", "CM"},
            {"mm", "MM"},
            {"bpm", "BPM"}
        };

        // Use word boundaries to replace whole words only
        foreach (var (abbrev, normalized) in medicalAbbreviations)
        {
            // Replace both standalone and with Thai text
            text = Regex.Replace(text, $@"\b{Regex.Escape(abbrev)}\b", normalized, RegexOptions.IgnoreCase);
        }

        return text;
    }
}
