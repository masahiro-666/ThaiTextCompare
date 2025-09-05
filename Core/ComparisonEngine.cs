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
            return new ComparisonResult(100.0, true, 0, 0, 0, 0, new List<string>(), new List<string>(), new List<string>());
        }

        if (string.IsNullOrEmpty(text1) || string.IsNullOrEmpty(text2))
        {
            return new ComparisonResult(0.0, false, 0, 1, 0, 0, new List<string>(), new List<string>(), new List<string>());
        }

        var tokens1 = _tokenizer.TokenizeThaiSymptoms(text1);
        var tokens2 = _tokenizer.TokenizeThaiSymptoms(text2);

        var set1 = tokens1.Distinct().ToHashSet();
        var set2 = tokens2.Distinct().ToHashSet();

        var matchedWords = set1.Intersect(set2).ToList();
        var missingWords = set1.Except(set2).ToList();
        var extraWords = set2.Except(set1).ToList();

        int intersection = matchedWords.Count;
        int union = set1.Union(set2).Count();

        double jaccardSimilarity = union == 0 ? 0 : Math.Round((double)intersection / union * 100, 2);
        double coverage1 = set1.Count == 0 ? 0 : Math.Round((double)intersection / set1.Count * 100, 2);
        double coverage2 = set2.Count == 0 ? 0 : Math.Round((double)intersection / set2.Count * 100, 2);

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
            extraWords
        );
    }
}

public record ComparisonResult(
    double Similarity,
    bool IsMatch,
    int Intersection,
    int Union,
    double Coverage1,
    double Coverage2,
    List<string> MatchedWords,
    List<string> MissingWords,
    List<string> ExtraWords
);
