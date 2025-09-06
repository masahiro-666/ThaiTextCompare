using System.Text.Json;

namespace ThaiTextCompare.Core;

/// <summary>
/// Manages Thai medical symptom dictionary with dynamic word addition and compound pattern support
/// </summary>
public static class SymptomDictionary
{
    private static readonly List<string> _dynamicSymptomDict;
    private static readonly Dictionary<string, string> _compoundSymptomPatterns = new Dictionary<string, string>();

    /// <summary>
    /// Default Thai medical symptom dictionary loaded from JSON file
    /// </summary>
    public static readonly string[] DefaultSymptomDict = LoadDefaultSymptomsFromJson();

    /// <summary>
    /// Loads default symptoms from JSON file
    /// </summary>
    private static string[] LoadDefaultSymptomsFromJson()
    {
        try
        {
            var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "default-symptoms.json");

            // If file doesn't exist in Data folder, try current directory or embedded resource
            if (!File.Exists(jsonPath))
            {
                jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "default-symptoms.json");
            }

            if (!File.Exists(jsonPath))
            {
                // Fallback to hardcoded list if JSON file not found
                return GetFallbackSymptoms();
            }

            var jsonContent = File.ReadAllText(jsonPath);
            var symptomData = JsonSerializer.Deserialize<SymptomJsonData>(jsonContent);

            return symptomData?.Symptoms ?? GetFallbackSymptoms();
        }
        catch (Exception ex)
        {
            // Log the error if needed and return fallback
            Console.WriteLine($"Warning: Could not load symptoms from JSON: {ex.Message}. Using fallback dictionary.");
            return GetFallbackSymptoms();
        }
    }

    /// <summary>
    /// Fallback symptom list if JSON loading fails
    /// </summary>
    private static string[] GetFallbackSymptoms()
    {
        return new[]
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
    }

    static SymptomDictionary()
    {
        _dynamicSymptomDict = new List<string>(DefaultSymptomDict);

        // Load compound patterns from JSON on initialization
        LoadAndApplyCompoundPatternsFromJson();
    }

    /// <summary>
    /// Gets the current symptom dictionary including dynamically added words
    /// </summary>
    public static string[] GetCurrentSymptomDict()
    {
        lock (_dynamicSymptomDict)
        {
            return _dynamicSymptomDict.ToArray();
        }
    }

    /// <summary>
    /// Adds new words to the symptom dictionary
    /// </summary>
    /// <param name="newWords">Array of new symptom words to add</param>
    /// <returns>Number of words successfully added</returns>
    public static int AddSymptomWords(params string[] newWords)
    {
        if (newWords == null || newWords.Length == 0)
            return 0;

        lock (_dynamicSymptomDict)
        {
            int addedCount = 0;
            foreach (var word in newWords)
            {
                if (!string.IsNullOrWhiteSpace(word) && !_dynamicSymptomDict.Contains(word))
                {
                    _dynamicSymptomDict.Add(word.Trim());
                    addedCount++;
                }
            }
            return addedCount;
        }
    }

    /// <summary>
    /// Adds a compound symptom pattern that will be automatically expanded
    /// </summary>
    /// <param name="compoundPattern">The compound pattern (e.g., "แขนซ้าย-ขาซ้ายอ่อนแรง")</param>
    /// <param name="expandedFormat">The expanded format (e.g., "แขนซ้ายอ่อนแรง ขาซ้ายอ่อนแรง")</param>
    /// <returns>True if added successfully, false if pattern already exists</returns>
    public static bool AddCompoundSymptomPattern(string compoundPattern, string expandedFormat)
    {
        if (string.IsNullOrWhiteSpace(compoundPattern) || string.IsNullOrWhiteSpace(expandedFormat))
            return false;

        lock (_compoundSymptomPatterns)
        {
            compoundPattern = compoundPattern.Trim();
            expandedFormat = expandedFormat.Trim();

            if (_compoundSymptomPatterns.ContainsKey(compoundPattern))
                return false;

            _compoundSymptomPatterns[compoundPattern] = expandedFormat;

            // Also add variations without hyphens
            var withoutHyphen = compoundPattern.Replace("-", "");
            if (!_compoundSymptomPatterns.ContainsKey(withoutHyphen))
            {
                _compoundSymptomPatterns[withoutHyphen] = expandedFormat;
            }

            return true;
        }
    }

    /// <summary>
    /// Gets all registered compound symptom patterns
    /// </summary>
    public static Dictionary<string, string> GetCompoundSymptomPatterns()
    {
        lock (_compoundSymptomPatterns)
        {
            return new Dictionary<string, string>(_compoundSymptomPatterns);
        }
    }

    /// <summary>
    /// Removes words from the symptom dictionary
    /// </summary>
    /// <param name="wordsToRemove">Words to remove</param>
    /// <returns>Number of words successfully removed</returns>
    public static int RemoveSymptomWords(params string[] wordsToRemove)
    {
        if (wordsToRemove == null || wordsToRemove.Length == 0)
            return 0;

        lock (_dynamicSymptomDict)
        {
            int removedCount = 0;
            foreach (var word in wordsToRemove)
            {
                if (_dynamicSymptomDict.Remove(word?.Trim() ?? ""))
                {
                    removedCount++;
                }
            }
            return removedCount;
        }
    }

    /// <summary>
    /// Resets the dictionary to the default state
    /// </summary>
    public static void ResetToDefault()
    {
        lock (_dynamicSymptomDict)
        {
            _dynamicSymptomDict.Clear();
            _dynamicSymptomDict.AddRange(DefaultSymptomDict);
        }

        lock (_compoundSymptomPatterns)
        {
            _compoundSymptomPatterns.Clear();
        }
    }

    /// <summary>
    /// Gets statistics about the current dictionary
    /// </summary>
    public static (int TotalWords, int DefaultWords, int AddedWords, int CompoundPatterns) GetDictionaryStats()
    {
        lock (_dynamicSymptomDict)
        {
            lock (_compoundSymptomPatterns)
            {
                return (
                    TotalWords: _dynamicSymptomDict.Count,
                    DefaultWords: DefaultSymptomDict.Length,
                    AddedWords: _dynamicSymptomDict.Count - DefaultSymptomDict.Length,
                    CompoundPatterns: _compoundSymptomPatterns.Count
                );
            }
        }
    }

    /// <summary>
    /// Reloads the default dictionary from JSON file
    /// </summary>
    /// <returns>True if reload was successful, false otherwise</returns>
    public static bool ReloadFromJson()
    {
        try
        {
            var newDefaultSymptoms = LoadDefaultSymptomsFromJson();

            lock (_dynamicSymptomDict)
            {
                _dynamicSymptomDict.Clear();
                _dynamicSymptomDict.AddRange(newDefaultSymptoms);
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets the path to the JSON file being used
    /// </summary>
    /// <returns>Path to JSON file or null if using fallback</returns>
    public static string? GetJsonFilePath()
    {
        var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "default-symptoms.json");

        if (File.Exists(jsonPath))
            return jsonPath;

        jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "default-symptoms.json");

        return File.Exists(jsonPath) ? jsonPath : null;
    }

    /// <summary>
    /// Loads synonym mappings from JSON file
    /// </summary>
    /// <returns>Dictionary of synonym mappings or fallback if JSON not available</returns>
    public static Dictionary<string, string> LoadSynonymMappingsFromJson()
    {
        try
        {
            var jsonPath = GetJsonFilePath();
            if (jsonPath == null || !File.Exists(jsonPath))
            {
                return GetFallbackSynonymMappings();
            }

            var jsonContent = File.ReadAllText(jsonPath);
            var symptomData = JsonSerializer.Deserialize<SymptomJsonData>(jsonContent);

            return symptomData?.Synonyms ?? GetFallbackSynonymMappings();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not load synonyms from JSON: {ex.Message}. Using fallback synonyms.");
            return GetFallbackSynonymMappings();
        }
    }

    /// <summary>
    /// Loads medical abbreviations from JSON file
    /// </summary>
    /// <returns>Dictionary of medical abbreviations or fallback if JSON not available</returns>
    public static Dictionary<string, string> LoadMedicalAbbreviationsFromJson()
    {
        try
        {
            var jsonPath = GetJsonFilePath();
            if (jsonPath == null || !File.Exists(jsonPath))
            {
                return GetFallbackMedicalAbbreviations();
            }

            var jsonContent = File.ReadAllText(jsonPath);
            var symptomData = JsonSerializer.Deserialize<SymptomJsonData>(jsonContent);

            return symptomData?.MedicalAbbreviations ?? GetFallbackMedicalAbbreviations();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not load medical abbreviations from JSON: {ex.Message}. Using fallback abbreviations.");
            return GetFallbackMedicalAbbreviations();
        }
    }

    /// <summary>
    /// Loads Thai conjunctions from JSON file
    /// </summary>
    /// <returns>Array of Thai conjunctions or fallback if JSON not available</returns>
    public static string[] LoadConjunctionsFromJson()
    {
        try
        {
            var jsonPath = GetJsonFilePath();
            if (jsonPath == null || !File.Exists(jsonPath))
            {
                return GetFallbackConjunctions();
            }

            var jsonContent = File.ReadAllText(jsonPath);
            var symptomData = JsonSerializer.Deserialize<SymptomJsonData>(jsonContent);

            return symptomData?.Conjunctions ?? GetFallbackConjunctions();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not load conjunctions from JSON: {ex.Message}. Using fallback conjunctions.");
            return GetFallbackConjunctions();
        }
    }

    /// <summary>
    /// Loads compound patterns from JSON and applies them to the dictionary
    /// </summary>
    public static void LoadAndApplyCompoundPatternsFromJson()
    {
        try
        {
            var jsonPath = GetJsonFilePath();
            if (jsonPath == null || !File.Exists(jsonPath))
            {
                ApplyFallbackCompoundPatterns();
                return;
            }

            var jsonContent = File.ReadAllText(jsonPath);
            var symptomData = JsonSerializer.Deserialize<SymptomJsonData>(jsonContent);

            // Apply default compound patterns
            if (symptomData?.DefaultCompoundPatterns != null)
            {
                foreach (var (pattern, expansion) in symptomData.DefaultCompoundPatterns)
                {
                    AddCompoundSymptomPattern(pattern, expansion);
                }
            }

            // Apply common patterns
            if (symptomData?.CommonPatterns != null)
            {
                foreach (var (pattern, expansion) in symptomData.CommonPatterns)
                {
                    AddCompoundSymptomPattern(pattern, expansion);
                }
            }

            // Apply side weakness patterns
            if (symptomData?.SideWeaknessPatterns != null)
            {
                ApplySideWeaknessPatternsFromJson(symptomData.SideWeaknessPatterns);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not load compound patterns from JSON: {ex.Message}. Using fallback patterns.");
            ApplyFallbackCompoundPatterns();
        }
    }

    /// <summary>
    /// Applies side weakness patterns from JSON configuration
    /// </summary>
    private static void ApplySideWeaknessPatternsFromJson(SideWeaknessPatterns patterns)
    {
        if (patterns.LeftSide != null && !string.IsNullOrWhiteSpace(patterns.LeftSide.Side))
        {
            foreach (var condition in patterns.LeftSide.Conditions ?? new[] { "อ่อนแรง" })
            {
                CreateSideWeaknessPatternsFromConfig(patterns.LeftSide.Side, patterns.LeftSide.BodyParts ?? new[] { "แขน", "ขา" }, condition);
            }
        }

        if (patterns.RightSide != null && !string.IsNullOrWhiteSpace(patterns.RightSide.Side))
        {
            foreach (var condition in patterns.RightSide.Conditions ?? new[] { "อ่อนแรง" })
            {
                CreateSideWeaknessPatternsFromConfig(patterns.RightSide.Side, patterns.RightSide.BodyParts ?? new[] { "แขน", "ขา" }, condition);
            }
        }
    }

    /// <summary>
    /// Creates side weakness patterns from JSON config
    /// </summary>
    private static void CreateSideWeaknessPatternsFromConfig(string side, string[] bodyParts, string condition)
    {
        if (bodyParts?.Length < 2 || string.IsNullOrWhiteSpace(side) || string.IsNullOrWhiteSpace(condition))
            return;

        // Create compound pattern for multiple body parts
        var compoundPattern = string.Join("", bodyParts.Select(part => $"{part}{side}")) + condition;
        var expandedFormat = string.Join(" ", bodyParts.Select(part => $"{part}{side}{condition}"));

        AddCompoundSymptomPattern(compoundPattern, expandedFormat);

        // Create hyphenated version
        var hyphenatedPattern = string.Join("-", bodyParts.Select(part => $"{part}{side}")) + condition;
        AddCompoundSymptomPattern(hyphenatedPattern, expandedFormat);

        // Add individual symptoms to dictionary
        var individualSymptoms = bodyParts.Select(part => $"{part}{side}{condition}").ToArray();
        AddSymptomWords(individualSymptoms);
    }

    /// <summary>
    /// Fallback synonym mappings if JSON loading fails
    /// </summary>
    private static Dictionary<string, string> GetFallbackSynonymMappings()
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

    /// <summary>
    /// Fallback medical abbreviations if JSON loading fails
    /// </summary>
    private static Dictionary<string, string> GetFallbackMedicalAbbreviations()
    {
        return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
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
            {"im", "IM"}
        };
    }

    /// <summary>
    /// Fallback Thai conjunctions if JSON loading fails
    /// </summary>
    private static string[] GetFallbackConjunctions()
    {
        return new string[]
        {
            "และ",      // and
            "หรือ",     // or
            "แต่",      // but
            "กับ",      // with
            "แล้ว",     // then/already
            "ก็",       // then/also
            "ที่",      // that/which (relative pronoun)
            "เพื่อ",    // for/in order to
            "ถ้า",      // if
            "เมื่อ",    // when
            "จึง",      // therefore
            "เลย",      // at all/extremely
            "นี้",      // this
            "นั้น",     // that
            "นี่",      // this (demonstrative)
            "นั่น"      // that (demonstrative)
        };
    }

    /// <summary>
    /// Applies fallback compound patterns if JSON loading fails
    /// </summary>
    private static void ApplyFallbackCompoundPatterns()
    {
        // Legacy compound patterns
        AddCompoundSymptomPattern("แขนซ้าย-ขาซ้ายอ่อนแรง", "แขนซ้ายอ่อนแรง ขาซ้ายอ่อนแรง");
        AddCompoundSymptomPattern("แขนซ้ายขาซ้ายอ่อนแรง", "แขนซ้ายอ่อนแรง ขาซ้ายอ่อนแรง");
        AddCompoundSymptomPattern("ขาซ้าย-แขนซ้ายอ่อนแรง", "ขาซ้ายอ่อนแรง แขนซ้ายอ่อนแรง");
        AddCompoundSymptomPattern("ขาซ้ายแขนซ้ายอ่อนแรง", "ขาซ้ายอ่อนแรง แขนซ้ายอ่อนแรง");
        AddCompoundSymptomPattern("แขนขวา-ขาขวาอ่อนแรง", "แขนขวาอ่อนแรง ขาขวาอ่อนแรง");
        AddCompoundSymptomPattern("แขนขวาขาขวาอ่อนแรง", "แขนขวาอ่อนแรง ขาขวาอ่อนแรง");
        AddCompoundSymptomPattern("ขาขวา-แขนขวาอ่อนแรง", "ขาขวาอ่อนแรง แขนขวาอ่อนแรง");
        AddCompoundSymptomPattern("ขาขวาแขนขวาอ่อนแรง", "ขาขวาอ่อนแรง แขนขวาอ่อนแรง");

        // Common patterns
        AddCompoundSymptomPattern("ปวดหัว-วิงเวียน", "ปวดหัว วิงเวียน");
        AddCompoundSymptomPattern("ไข้-หนาวสั่น", "ไข้ หนาวสั่น");
        AddCompoundSymptomPattern("เจ็บคอ-มีน้ำมูก", "เจ็บคอ มีน้ำมูก");
    }
}

/// <summary>
/// Data structure for loading symptoms from JSON
/// </summary>
internal class SymptomJsonData
{
    public string[]? Symptoms { get; set; }
    public Dictionary<string, string>? Synonyms { get; set; }
    public Dictionary<string, string>? DefaultCompoundPatterns { get; set; }
    public Dictionary<string, string>? CommonPatterns { get; set; }
    public Dictionary<string, string>? MedicalAbbreviations { get; set; }
    public string[]? Conjunctions { get; set; }
    public SideWeaknessPatterns? SideWeaknessPatterns { get; set; }
    public SymptomMetadata? Metadata { get; set; }
}

/// <summary>
/// Side weakness pattern configuration
/// </summary>
internal class SideWeaknessPatterns
{
    public SideWeaknessConfig? LeftSide { get; set; }
    public SideWeaknessConfig? RightSide { get; set; }
}

/// <summary>
/// Configuration for side weakness patterns
/// </summary>
internal class SideWeaknessConfig
{
    public string? Side { get; set; }
    public string[]? BodyParts { get; set; }
    public string[]? Conditions { get; set; }
}

/// <summary>
/// Metadata for symptom JSON file
/// </summary>
internal class SymptomMetadata
{
    public string? Version { get; set; }
    public string? Description { get; set; }
    public string? LastUpdated { get; set; }
    public string[]? Features { get; set; }
}
