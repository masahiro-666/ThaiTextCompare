using UnityEngine;
using ThaiTextCompare.Core;
using System.IO;

/// <summary>
/// Example Unity script demonstrating Thai Text Compare integration
/// </summary>
public class ThaiTextCompareExample : MonoBehaviour
{
    [Header("Thai Text Comparison")]
    [TextArea(3, 5)]
    public string text1 = "ไข้ ไอ เจ็บคอ มีน้ำมูก";

    [TextArea(3, 5)]
    public string text2 = "ไข้ ไอ เจ็บคอ น้ำมูก";

    [Space]
    [Header("Results")]
    public bool isMatch;
    public float similarity;
    public float coverage;

    private ThaiMedicalTokenizer tokenizer;
    private ComparisonEngine comparisonEngine;

    void Start()
    {
        InitializeThaiTextCompare();

        // Perform initial comparison
        CompareTexts();
    }

    void InitializeThaiTextCompare()
    {
        try
        {
            // Initialize the Thai text comparison system
            tokenizer = ThaiMedicalTokenizer.CreateWithDynamicDictionary();
            comparisonEngine = new ComparisonEngine(tokenizer);

            Debug.Log("✅ Thai Text Compare initialized successfully!");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ Failed to initialize Thai Text Compare: {ex.Message}");
        }
    }

    [ContextMenu("Compare Texts")]
    public void CompareTexts()
    {
        if (comparisonEngine == null)
        {
            Debug.LogError("Thai Text Compare not initialized!");
            return;
        }

        var result = comparisonEngine.CompareThaiMedicalTexts(text1, text2);

        // Update public fields for Inspector
        isMatch = result.IsMatch;
        similarity = (float)result.Similarity;
        coverage = (float)result.Coverage1;

        // Log detailed results
        Debug.Log($"🔍 Comparison Results:");
        Debug.Log($"  Text 1: '{text1}'");
        Debug.Log($"  Text 2: '{text2}'");
        Debug.Log($"  Match: {result.IsMatch}");
        Debug.Log($"  Similarity: {result.Similarity}%");
        Debug.Log($"  Coverage: {result.Coverage1}%");
        Debug.Log($"  ✅ Matched: [{string.Join(", ", result.MatchedWords)}]");
        Debug.Log($"  ❌ Missing: [{string.Join(", ", result.MissingWords)}]");
        Debug.Log($"  ➕ Extra: [{string.Join(", ", result.ExtraWords)}]");

        if (result.TypoCorrections.Count > 0)
        {
            Debug.Log($"  🔧 Typo Corrections: [{string.Join(", ", result.TypoCorrections)}]");
        }
    }

    /// <summary>
    /// Example method for dynamic text comparison from UI
    /// </summary>
    public void CompareTextFields(string inputText1, string inputText2)
    {
        text1 = inputText1;
        text2 = inputText2;
        CompareTexts();
    }

    /// <summary>
    /// Get detailed comparison result as formatted string
    /// </summary>
    public string GetComparisonDetails(string inputText1, string inputText2)
    {
        if (comparisonEngine == null)
            return "Thai Text Compare not initialized!";

        var result = comparisonEngine.CompareThaiMedicalTexts(inputText1, inputText2);

        return $@"Thai Text Comparison Result:
Text 1: {inputText1}
Text 2: {inputText2}
Match: {result.IsMatch}
Similarity: {result.Similarity:F1}%
Coverage: {result.Coverage1:F1}%
Matched Words: {result.MatchedWords.Count}
Missing Words: {result.MissingWords.Count}
Extra Words: {result.ExtraWords.Count}";
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 30), "Compare Texts"))
        {
            CompareTexts();
        }
    }
}
