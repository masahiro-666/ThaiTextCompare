using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace ThaiTextCompare.Core
{

/// <summary>
/// Utility class for managing symptom dictionary and compound patterns
/// </summary>
public static class SymptomDictionaryManager
{
    /// <summary>
    /// Adds new symptom words and optionally creates a compound pattern
    /// </summary>
    /// <param name="symptoms">New symptom words to add</param>
    /// <param name="compoundPattern">Optional compound pattern (e.g., "แขนซ้าย-ขาซ้ายอ่อนแรง")</param>
    /// <param name="expandedFormat">Optional expanded format (e.g., "แขนซ้ายอ่อนแรง ขาซ้ายอ่อนแรง")</param>
    /// <returns>Result with details of the operation</returns>
    public static AddSymptomsResult AddSymptoms(
        string[] symptoms,
        string? compoundPattern = null,
        string? expandedFormat = null)
    {
        var result = new AddSymptomsResult();

        // Add individual symptoms
        if (symptoms?.Length > 0)
        {
            result.AddedWordsCount = SymptomDictionary.AddSymptomWords(symptoms);
            result.AddedWords.AddRange(symptoms.Where(s => !string.IsNullOrWhiteSpace(s)));
        }

        // Add compound pattern if provided
        if (!string.IsNullOrWhiteSpace(compoundPattern) && !string.IsNullOrWhiteSpace(expandedFormat))
        {
            result.CompoundPatternAdded = SymptomDictionary.AddCompoundSymptomPattern(compoundPattern, expandedFormat);
            if (result.CompoundPatternAdded)
            {
                result.CompoundPattern = compoundPattern;
                result.ExpandedFormat = expandedFormat;
            }
        }

        // Add the individual words from the expanded format to ensure they're in the dictionary
        if (result.CompoundPatternAdded && !string.IsNullOrWhiteSpace(expandedFormat))
        {
            var expandedWords = expandedFormat.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var additionalWords = SymptomDictionary.AddSymptomWords(expandedWords);
            result.AddedWordsCount += additionalWords;
        }

        return result;
    }

    /// <summary>
    /// Creates common compound patterns for body side weakness
    /// </summary>
    /// <param name="side">Side (ซ้าย or ขวา)</param>
    /// <param name="bodyParts">Body parts (e.g., แขน, ขา)</param>
    /// <param name="condition">Condition (e.g., อ่อนแรง)</param>
    /// <returns>List of created patterns</returns>
    public static List<string> CreateSideWeaknessPatterns(string side, string[] bodyParts, string condition)
    {
        var createdPatterns = new List<string>();

        if (bodyParts?.Length < 2 || string.IsNullOrWhiteSpace(side) || string.IsNullOrWhiteSpace(condition))
            return createdPatterns;

        // Create compound pattern for multiple body parts
        var compoundPattern = string.Join("", bodyParts?.Where(part => part != null).Select(part => $"{part}{side}") ?? Enumerable.Empty<string>()) + condition;
        var expandedFormat = string.Join(" ", bodyParts?.Where(part => part != null).Select(part => $"{part}{side}{condition}") ?? Enumerable.Empty<string>());

        if (SymptomDictionary.AddCompoundSymptomPattern(compoundPattern, expandedFormat))
        {
            createdPatterns.Add(compoundPattern);
        }

        // Create hyphenated version
        var hyphenatedPattern = string.Join("-", bodyParts?.Select(part => $"{part}{side}") ?? Enumerable.Empty<string>()) + condition;
        if (SymptomDictionary.AddCompoundSymptomPattern(hyphenatedPattern, expandedFormat))
        {
            createdPatterns.Add(hyphenatedPattern);
        }

        // Add individual symptoms to dictionary
        var individualSymptoms = bodyParts?.Select(part => $"{part}{side}{condition}").ToArray() ?? Array.Empty<string>();
        SymptomDictionary.AddSymptomWords(individualSymptoms);

        return createdPatterns;
    }

    /// <summary>
    /// Loads symptoms and patterns from a JSON configuration
    /// </summary>
    /// <param name="jsonConfig">JSON configuration string</param>
    /// <returns>Result of the loading operation</returns>
    public static LoadConfigResult LoadFromJson(string jsonConfig)
    {
        try
        {
            var config = JsonSerializer.Deserialize<SymptomConfig>(jsonConfig);
            var result = new LoadConfigResult { Success = true };

            if (config?.Symptoms?.Length > 0)
            {
                result.AddedSymptoms = SymptomDictionary.AddSymptomWords(config.Symptoms);
            }

            if (config?.CompoundPatterns?.Any() == true)
            {
                foreach (var pattern in config.CompoundPatterns)
                {
                    if (SymptomDictionary.AddCompoundSymptomPattern(pattern.Key, pattern.Value))
                    {
                        result.AddedPatterns++;
                    }
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            return new LoadConfigResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>
    /// Exports current configuration to JSON
    /// </summary>
    /// <returns>JSON representation of current symptoms and patterns</returns>
    public static string ExportToJson()
    {
        var stats = SymptomDictionary.GetDictionaryStats();
        var currentDict = SymptomDictionary.GetCurrentSymptomDict();
        var compoundPatterns = SymptomDictionary.GetCompoundSymptomPatterns();

        var config = new SymptomConfig
        {
            Symptoms = currentDict,
            CompoundPatterns = compoundPatterns,
            Statistics = new ConfigStatistics
            {
                TotalWords = stats.TotalWords,
                DefaultWords = stats.DefaultWords,
                AddedWords = stats.AddedWords,
                CompoundPatterns = stats.CompoundPatterns,
                ExportedAt = DateTime.Now
            }
        };

        return JsonSerializer.Serialize(config, new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
    }

    /// <summary>
    /// Creates a new tokenizer with the current dynamic dictionary
    /// </summary>
    /// <returns>New tokenizer instance</returns>
    public static ThaiMedicalTokenizer CreateTokenizer()
    {
        return ThaiMedicalTokenizer.CreateWithDynamicDictionary();
    }

    /// <summary>
    /// Quick setup for common medical patterns
    /// </summary>
    public static void SetupCommonPatterns()
    {
        // Load patterns from JSON instead of hardcoding
        SymptomDictionary.LoadAndApplyCompoundPatternsFromJson();
    }
}

/// <summary>
/// Result of adding symptoms operation
/// </summary>
public class AddSymptomsResult
{
    public int AddedWordsCount { get; set; }
    public List<string> AddedWords { get; set; } = new List<string>();
    public bool CompoundPatternAdded { get; set; }
    public string? CompoundPattern { get; set; }
    public string? ExpandedFormat { get; set; }
}

/// <summary>
/// Result of loading configuration operation
/// </summary>
public class LoadConfigResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int AddedSymptoms { get; set; }
    public int AddedPatterns { get; set; }
}

/// <summary>
/// JSON configuration structure
/// </summary>
public class SymptomConfig
{
    public string[]? Symptoms { get; set; }
    public Dictionary<string, string>? CompoundPatterns { get; set; }
    public ConfigStatistics? Statistics { get; set; }
}

/// <summary>
/// Statistics for configuration export
/// </summary>
public class ConfigStatistics
{
    public int TotalWords { get; set; }
    public int DefaultWords { get; set; }
    public int AddedWords { get; set; }
    public int CompoundPatterns { get; set; }
    public DateTime ExportedAt { get; set; }
}
}
