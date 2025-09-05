using System.Text;
using System.Text.RegularExpressions;

namespace ThaiTextCompare.Core;

public class ThaiMedicalTokenizer
{
    private readonly string[] _symptomDict;
    private readonly HashSet<string> _dictSet;
    private readonly List<string> _sortedDict;
    private readonly Dictionary<string, string> _synonymMappings;

    public ThaiMedicalTokenizer(string[] symptomDict)
    {
        _symptomDict = symptomDict ?? throw new ArgumentNullException(nameof(symptomDict));
        _dictSet = new HashSet<string>(_symptomDict);
        _sortedDict = _symptomDict.OrderByDescending(d => d.Length).ToList();
        _synonymMappings = InitializeSynonymMappings();
    }

    public List<string> TokenizeThaiSymptoms(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new List<string>();

        text = NormalizeSpaces(text);
        text = ExpandCompoundSymptoms(text);

        if (text.Contains(' ') || text.Contains('-'))
        {
            return TokenizeSpacedText(text);
        }

        return TokenizeConcatenatedText(text);
    }

    private List<string> TokenizeSpacedText(string text)
    {
        var words = text.Split(new char[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
        var result = new List<string>();

        foreach (var word in words)
        {
            var normalizedWord = NormalizeWord(word);
            if (_dictSet.Contains(normalizedWord))
            {
                result.Add(normalizedWord);
            }
            else
            {
                var bestMatch = FindBestFuzzyMatch(normalizedWord);
                if (bestMatch != null)
                {
                    result.Add(bestMatch);
                }
                else
                {
                    var matches = _dictSet.Where(d => normalizedWord.Contains(d)).ToList();
                    if (matches.Any())
                    {
                        if (matches.Any(m => m != normalizedWord))
                        {
                            var subTokens = ParseConcatenatedWord(normalizedWord);
                            result.AddRange(subTokens);
                        }
                        else
                        {
                            var longestMatch = matches.OrderByDescending(m => m.Length).First();
                            if (!result.Contains(longestMatch))
                            {
                                result.Add(longestMatch);
                            }
                        }
                    }
                    else
                    {
                        var subTokens = ParseConcatenatedWord(normalizedWord);
                        result.AddRange(subTokens);
                    }
                }
            }
        }

        return result;
    }

    private List<string> TokenizeConcatenatedText(string text)
    {
        if (text.Length > 500)
        {
            return OptimizedConcatenatedParsing(text);
        }

        return ParseConcatenatedWord(text);
    }

    private List<string> ParseConcatenatedWord(string word)
    {
        var result = new List<string>();
        int i = 0;

        while (i < word.Length)
        {
            string? hit = null;
            foreach (var term in _sortedDict)
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
                var nonDictSegment = ExtractNonDictionarySegment(word, i);
                if (!string.IsNullOrEmpty(nonDictSegment))
                {
                    result.Add(nonDictSegment);
                    i += nonDictSegment.Length;
                }
                else
                {
                    result.Add(word.Substring(i, 1));
                    i++;
                }
            }
        }

        return result;
    }

    private List<string> OptimizedConcatenatedParsing(string text)
    {
        var result = new List<string>();
        var dictLookup = BuildDictionaryLookup();
        int i = 0;

        while (i < text.Length)
        {
            string? hit = null;
            char currentChar = text[i];

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
                var segment = ExtractNonDictionarySegment(text, i);
                if (!string.IsNullOrEmpty(segment))
                {
                    result.Add(segment);
                    i += segment.Length;
                }
                else
                {
                    result.Add(text.Substring(i, 1));
                    i++;
                }
            }
        }

        return result;
    }

    private Dictionary<char, List<string>> BuildDictionaryLookup()
    {
        var dictLookup = new Dictionary<char, List<string>>();
        foreach (var term in _sortedDict)
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
        return dictLookup;
    }

    private string? FindBestFuzzyMatch(string word)
    {
        if (word.Length < 3) return null;

        var candidates = _dictSet
            .Where(d => Math.Abs(d.Length - word.Length) <= 2)
            .Where(d => CalculateSimilarity(word, d) >= 0.7)
            .OrderByDescending(d => CalculateSimilarity(word, d))
            .ToList();

        return candidates.FirstOrDefault();
    }

    private static double CalculateSimilarity(string s1, string s2)
    {
        if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2)) return 0;
        if (s1 == s2) return 1;

        var chars1 = new HashSet<char>(s1);
        var chars2 = new HashSet<char>(s2);

        var intersection = chars1.Intersect(chars2).Count();
        var union = chars1.Union(chars2).Count();

        return union == 0 ? 0 : (double)intersection / union;
    }

    private string ExtractNonDictionarySegment(string word, int startIndex)
    {
        int i = startIndex;
        var segment = new StringBuilder();

        while (i < word.Length)
        {
            bool foundDictWordAtI = false;
            foreach (var term in _sortedDict)
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

            if (segment.Length > 0)
            {
                if (i + 1 < word.Length)
                {
                    char nextChar = word[i + 1];

                    if (IsThaiCharacter(currentChar) && !IsThaiCharacter(nextChar))
                    {
                        i++;
                        break;
                    }

                    if (!IsThaiCharacter(currentChar) && IsThaiCharacter(nextChar))
                    {
                        i++;
                        break;
                    }

                    if (segment.Length >= 50)
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

    public static bool IsThaiCharacter(char c)
    {
        return c >= 0x0E00 && c <= 0x0E7F;
    }

    public string ExpandCompoundSymptoms(string text)
    {
        foreach (var (from, to) in _synonymMappings)
        {
            text = text.Replace(from, to);
        }

        text = Regex.Replace(text, @"\d+\s*(ครั้ง|ข้าง)", "");
        text = Regex.Replace(text, @"\(.*?\)", "");
        text = text.Trim();

        // Handle compound patterns
        if (text == "แขนซ้าย-ขาซ้ายอ่อนแรง" || text == "แขนซ้ายขาซ้ายอ่อนแรง")
        {
            return "แขนซ้ายอ่อนแรง ขาซ้ายอ่อนแรง";
        }
        if (text == "ขาซ้าย-แขนซ้ายอ่อนแรง" || text == "ขาซ้ายแขนซ้ายอ่อนแรง")
        {
            return "ขาซ้ายอ่อนแรง แขนซ้ายอ่อนแรง";
        }
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

    public string NormalizeSpaces(string s)
    {
        s = s.Trim();
        s = Regex.Replace(s, @"\s+", " ");
        s = NormalizeMedicalAbbreviations(s);
        return s;
    }

    private string NormalizeWord(string word)
    {
        // For Thai text, don't normalize case
        if (word.Any(IsThaiCharacter))
        {
            return word;
        }

        // For English words, normalize case to lowercase for comparison
        // But preserve the original casing for display
        var lowercaseWord = word.ToLowerInvariant();

        // If it's a medical abbreviation, it will be normalized by NormalizeMedicalAbbreviations
        var normalizedAbbrev = NormalizeMedicalAbbreviations(word);
        if (normalizedAbbrev != word)
        {
            return normalizedAbbrev;
        }

        return lowercaseWord;
    }

    private static string NormalizeMedicalAbbreviations(string text)
    {
        var medicalAbbreviations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"bp", "BP"}, {"hr", "HR"}, {"rr", "RR"}, {"pr", "PR"},
            {"ecg", "ECG"}, {"ekg", "EKG"}, {"ct", "CT"}, {"mri", "MRI"},
            {"xray", "XRAY"}, {"x-ray", "XRAY"}, {"iv", "IV"}, {"im", "IM"}
        };

        foreach (var (abbrev, normalized) in medicalAbbreviations)
        {
            text = Regex.Replace(text, $@"\b{Regex.Escape(abbrev)}\b", normalized, RegexOptions.IgnoreCase);
        }

        return text;
    }

    private static Dictionary<string, string> InitializeSynonymMappings()
    {
        return new Dictionary<string, string>
        {
            {"มีน้ำมูก", "น้ำมูก"},
            {"ปวดศีรษะ", "ปวดหัว"},
            {"BP ต่ำ", "BPต่ำ"},
            {"2 ข้าง", "2ข้าง"},
            {"มีไข้", "ไข้"},
            {"ปวดศรีษะ", "ปวดศีรษะ"},
            {"หายใจลำบาด", "หายใจลำบาก"},
            {"หายใจลำบาค", "หายใจลำบาก"},
            {"เจ็บคล", "เจ็บคอ"},
            {"มีน้ำมุก", "มีน้ำมูก"},
            {"น้ำมุก", "น้ำมูก"},
            {"อ่อนแรค", "อ่อนแรง"},
            {"หมดสิต", "หมดสติ"},
            {"หกล้น", "หกล้ม"},
            {"ผื่นขึน", "ผื่นขึ้น"},
            {"เหงือออก", "เหงื่อออก"},
            {"ถ่ายเหลอ", "ถ่ายเหลว"},
            {"วิงเวียน", "หน้ามืด"},
            {"เวียนหัว", "หน้ามืด"}
        };
    }
}
