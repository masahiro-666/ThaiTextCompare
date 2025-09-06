````markdown
# Thai Text Compare - Unity Package

## 📦 Installation Methods

### Method 1: Git URL (Recommended)

1. Open Unity Package Manager (Window → Package Manager)
2. Click the `+` button in the top-left corner
3. Select "Add package from git URL..."
4. Enter: `https://github.com/masahiro-666/ThaiTextCompare.git?path=Unity`
5. Click "Add"

### Method 2: Manual Installation

1. Download the repository
2. Copy `Unity/Runtime/ThaiTextCompare.dll` to `Assets/Plugins/`
3. Copy `Unity/Data/` folder to `Assets/StreamingAssets/Data/`
4. Import sample scripts as needed

## 🚀 Quick Start

### 1. Basic Usage

```csharp
using ThaiTextCompare.Core;

// Initialize
var tokenizer = ThaiMedicalTokenizer.CreateWithDynamicDictionary();
var engine = new ComparisonEngine(tokenizer);

// Compare texts
var result = engine.CompareThaiMedicalTexts("ไข้ ไอ เจ็บคอ", "ไข้ ไอ เจ็บคอ มีน้ำมูก");

// Use results
Debug.Log($"Match: {result.IsMatch}");
Debug.Log($"Similarity: {result.Similarity}%");
```

### 2. Sample Scripts

After installation via Package Manager:

1. Go to Package Manager → Thai Text Compare → Samples
2. Import "Basic Thai Text Comparison" sample
3. Add `BasicThaiComparison` component to a GameObject

## ✨ Features

- ✅ **Real-time comparison** (~0.2ms per operation)
- ✅ **Thai character support** with proper UTF-8 handling
- ✅ **Medical terminology** dictionary with 800+ terms
- ✅ **Fuzzy matching** with typo correction
- ✅ **Thread-safe** for Unity's threading model
- ✅ **Memory optimized** (stable ~1.8MB usage)
- ✅ **Unity Package Manager** compatible

## 📋 Requirements

- Unity 2021.3 LTS or higher
- .NET Standard 2.1 compatible runtime
- Thai font for proper text display

## 🔧 API Reference

### Core Classes

- **`ThaiMedicalTokenizer`**: Tokenizes Thai medical text into symptoms
- **`ComparisonEngine`**: Main comparison logic and algorithms
- **`ComparisonResult`**: Contains detailed match results and statistics

### Key Methods

- `CompareThaiMedicalTexts(text1, text2)`: Primary comparison method
- `TokenizeThaiSymptoms(text)`: Tokenize text into medical symptoms
- `GetTypoSuggestions(word)`: Get typo correction suggestions

### Performance Metrics

| Metric                  | Value                     |
| ----------------------- | ------------------------- |
| Average comparison time | 0.20ms                    |
| Memory usage            | 1.8MB stable              |
| Throughput              | ~5,000 comparisons/second |
| Startup time            | <1s                       |

## 🎮 Unity Integration

### Inspector Integration

```csharp
[Header("Thai Text Comparison")]
[TextArea(3, 5)]
public string text1 = "ไข้ ไอ เจ็บคอ";

[TextArea(3, 5)]
public string text2 = "ไข้ ไอ เจ็บคอ น้ำมูก";

[ContextMenu("Compare Texts")]
public void CompareTexts() { /* ... */ }
```

### Runtime API

```csharp
// Get detailed comparison results
var result = engine.CompareThaiMedicalTexts(input1, input2);

// Access results
Debug.Log($"Similarity: {result.Similarity}%");
Debug.Log($"Matched words: {result.MatchedWords.Count}");
Debug.Log($"Missing words: {result.MissingWords.Count}");
Debug.Log($"Typo corrections: {result.TypoCorrections.Count}");
```

## 📁 Package Structure

```
Unity/
├── package.json              # Unity Package Manager configuration
├── README.md                 # This file
├── Runtime/                  # Runtime scripts and DLLs
│   ├── ThaiTextCompare.dll   # Core library
│   ├── ThaiTextCompare.xml   # API documentation
│   └── *.asmdef              # Assembly definitions
├── Data/                     # Medical terminology data
│   └── *.json               # Symptom dictionaries
└── Samples~/                # Sample scripts and scenes
    └── BasicComparison/     # Basic usage example
```

## 🚀 Ready to Use!

The package is production-ready and optimized for Unity applications requiring high-performance Thai medical text comparison.

For advanced usage examples, see the imported samples or check the main repository documentation.
````
