# Dictionary JSON Configuration

## Overview

The Thai Medical Text Compare system now uses a JSON file to store its default symptom dictionary, making it easy to maintain and update medical terminology without recompiling the code.

## JSON File Location

- **File**: `Data/default-symptoms.json`
- **Purpose**: Stores the default medical symptom dictionary
- **Format**: Structured JSON with symptoms, synonyms, and metadata

## JSON Structure

```json
{
  "symptoms": ["เจ็บหน้าอกร้าวไปแขนซ้าย", "อาเจียนเป็นเลือดแดงสด", "..."],
  "synonyms": {
    "ปวดหัว": "ปวดศีรษะ",
    "น้ำมูก": "มีน้ำมูก"
  },
  "metadata": {
    "version": "1.0",
    "description": "Default Thai medical symptom dictionary",
    "lastUpdated": "2025-09-06"
  }
}
```

## Features

### ✅ Automatic JSON Loading

- System automatically loads symptoms from JSON file on startup
- Falls back to hardcoded list if JSON file is missing or corrupted
- Robust error handling ensures system always works

### ✅ Easy Maintenance

- Medical professionals can update symptoms by editing JSON file
- No need to recompile code for dictionary updates
- Version tracking and metadata support

### ✅ Development Benefits

- Clear separation between data and code
- Easy to add new symptoms via simple JSON editing
- Supports synonyms and metadata for better organization

## Usage Examples

### Loading and Testing

```csharp
// The system automatically loads from JSON
var symptoms = SymptomDictionary.DefaultSymptomDict;

// Check if JSON was loaded successfully
var jsonPath = SymptomDictionary.GetJsonFilePath();
Console.WriteLine($"Dictionary loaded from: {jsonPath ?? "fallback hardcoded list"}");

// Reload from JSON if file was updated
bool reloaded = SymptomDictionary.ReloadFromJson();
Console.WriteLine($"Reload successful: {reloaded}");
```

### Manual JSON Updates

To add new symptoms, simply edit `Data/default-symptoms.json`:

```json
{
  "symptoms": ["เจ็บหน้าอกร้าวไปแขนซ้าย", "ปวดฟันกราม", "หูตึง", "จมูกแห้ง"]
}
```

### Runtime Dictionary Management

The JSON-based system works seamlessly with dynamic dictionary features:

```csharp
// Add symptoms at runtime (in addition to JSON-loaded symptoms)
SymptomDictionary.AddSymptomWords("ปวดฟันใหม่", "อาการใหม่");

// Create compound patterns
SymptomDictionary.AddCompoundSymptomPattern("ปวดฟัน-หูตึง", "ปวดฟัน หูตึง");

// Get current statistics
var stats = SymptomDictionary.GetDictionaryStats();
Console.WriteLine($"Total: {stats.TotalWords}, From JSON: {stats.DefaultWords}, Added: {stats.AddedWords}");
```

## Production Benefits

1. **🔧 Easy Updates**: Medical staff can update terminology without developer involvement
2. **📊 Version Control**: JSON files can be version controlled and tracked
3. **🚀 Zero Downtime**: Dictionary updates don't require application restart
4. **🛡️ Robust Fallback**: System never fails even if JSON file is corrupted
5. **📈 Scalability**: Easy to maintain large medical vocabularies

## Migration Notes

- **Backward Compatible**: Existing code continues to work unchanged
- **Same API**: `SymptomDictionary.DefaultSymptomDict` works exactly as before
- **Enhanced Features**: New JSON loading capabilities are additive
- **Fallback Safety**: Original hardcoded symptoms serve as fallback

This JSON-based approach maintains all existing functionality while providing modern, maintainable dictionary management for medical professionals.
