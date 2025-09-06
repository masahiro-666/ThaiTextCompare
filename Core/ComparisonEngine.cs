namespace ThaiTextCompare.Core;

public class ComparisonEngine
{
    private readonly ThaiMedicalTokenizer _tokenizer;

    public ComparisonEngine(ThaiMedicalTokenizer tokenizer)
    {
        _tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
    }

    public ComparisonResult CompareThaiMedicalTexts(string text1, string text2, double threshold = 75.0)
    {
        if (string.IsNullOrEmpty(text1) && string.IsNullOrEmpty(text2))
        {
            return new ComparisonResult(100.0, true, 0, 0, 0, 0, new List<string>(), new List<string>(), new List<string>(), new List<string>());
        }

        if (string.IsNullOrEmpty(text1) || string.IsNullOrEmpty(text2))
        {
            return new ComparisonResult(0.0, false, 0, 1, 0, 0, new List<string>(), new List<string>(), new List<string>(), new List<string>());
        }

        var tokens1 = _tokenizer.TokenizeThaiSymptoms(text1);
        var tokens2 = _tokenizer.TokenizeThaiSymptoms(text2);

        var set1 = tokens1.Distinct().ToHashSet();
        var set2 = tokens2.Distinct().ToHashSet();

        // Track typo corrections
        var typoCorrections = new List<string>();

        // Check for typos in text2 that could match text1
        var correctedSet2 = new HashSet<string>(set2);
        foreach (var word1 in set1)
        {
            if (!set2.Contains(word1))
            {
                // Look for potential typos in set2 that could match word1
                foreach (var word2 in set2)
                {
                    if (!set1.Contains(word2))
                    {
                        var suggestions = _tokenizer.GetTypoSuggestions(word2);
                        if (suggestions["character_substitution"].Contains(word1) ||
                            suggestions["missing_character"].Contains(word1) ||
                            suggestions["extra_character"].Contains(word1) ||
                            suggestions["fuzzy_match"].Contains(word1))
                        {
                            typoCorrections.Add($"{word2} → {word1}");
                            correctedSet2.Remove(word2);
                            correctedSet2.Add(word1);
                            break;
                        }
                    }
                }
            }
        }

        var matchedWords = set1.Intersect(correctedSet2).ToList();
        var missingWords = set1.Except(correctedSet2).ToList();
        var extraWords = correctedSet2.Except(set1).ToList();

        int intersection = matchedWords.Count;
        int union = set1.Union(correctedSet2).Count();

        double jaccardSimilarity = union == 0 ? 0 : Math.Round((double)intersection / union * 100, 2);
        double coverage1 = set1.Count == 0 ? 0 : Math.Round((double)intersection / set1.Count * 100, 2);
        double coverage2 = correctedSet2.Count == 0 ? 0 : Math.Round((double)intersection / correctedSet2.Count * 100, 2);

        bool isMatch = coverage1 >= threshold && coverage2 >= threshold;
        double primarySimilarity = coverage1;

        return new ComparisonResult(
            primarySimilarity,
            isMatch,
            intersection,
            union,
            coverage1,
            coverage2,
            matchedWords,
            missingWords,
            extraWords,
            typoCorrections
        );
    }
}

public class ComparisonResult
{
    public double Similarity { get; }
    public bool IsMatch { get; }
    public int Intersection { get; }
    public int Union { get; }
    public double Coverage1 { get; }
    public double Coverage2 { get; }
    public List<string> MatchedWords { get; }
    public List<string> MissingWords { get; }
    public List<string> ExtraWords { get; }
    public List<string> TypoCorrections { get; }

    public ComparisonResult(
        double similarity,
        bool isMatch,
        int intersection,
        int union,
        double coverage1,
        double coverage2,
        List<string> matchedWords,
        List<string> missingWords,
        List<string> extraWords,
        List<string> typoCorrections)
    {
        Similarity = similarity;
        IsMatch = isMatch;
        Intersection = intersection;
        Union = union;
        Coverage1 = coverage1;
        Coverage2 = coverage2;
        MatchedWords = matchedWords ?? new List<string>();
        MissingWords = missingWords ?? new List<string>();
        ExtraWords = extraWords ?? new List<string>();
        TypoCorrections = typoCorrections ?? new List<string>();
    }
}
