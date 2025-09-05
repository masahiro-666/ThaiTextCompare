# Thai Medical Text Comparison System - Setup Guide

## 📋 Overview

This is a C# .NET console application that compares Thai medical texts using advanced tokenization, fuzzy matching, and coverage-based analysis. The system is designed for medical applications requiring precise symptom matching with support for compound symptoms, typos, and concatenated text parsing.

## 🎯 Features

- **Thai Medical Text Processing**: Specialized tokenization for Thai medical terminology
- **Compound Symptom Handling**: Expands compound symptoms like "แขนขวาขาขวาอ่อนแรง"
- **Fuzzy Matching**: 70% similarity threshold for typo correction
- **Coverage-Based Matching**: Exact match logic requiring 100% coverage
- **Concatenated Text Parsing**: Handles mixed medical/non-medical concatenated text
- **Case Normalization**: Case-insensitive medical abbreviations (BP, HR, ECG, etc.)
- **Performance Optimization**: Optimized algorithms for strings >500 characters
- **Thai Unicode Boundary Detection**: Proper word segmentation for Thai-Latin boundaries

## 🛠️ Prerequisites

### Required Software

- **.NET 9.0 SDK** or later
- **Operating System**: Windows, macOS, or Linux
- **IDE** (Optional): Visual Studio, Visual Studio Code, or JetBrains Rider

### System Requirements

- **Memory**: Minimum 512 MB RAM
- **Storage**: 50 MB free disk space
- **Processor**: Any modern CPU (optimized for multi-core)

## 🚀 Quick Start

### 1. Clone or Download the Project

```bash
# If using Git
git clone <repository-url>
cd ThaiTextCompareDict

# Or download and extract the ZIP file
```

### 2. Navigate to Project Directory

```bash
cd ThaiTextCompare
```

### 3. Restore Dependencies (if needed)

```bash
dotnet restore
```

### 4. Build the Application

```bash
dotnet build
```

### 5. Run the Application

```bash
dotnet run
```

## 📁 Project Structure

```
ThaiTextCompareDict/
├── .gitignore                    # Git ignore rules
├── setup.md                     # This setup guide
├── Program.cs                   # Root program file (if any)
└── ThaiTextCompare/
    ├── Program.cs               # Main application file
    ├── ThaiTextCompare.csproj   # Project configuration
    ├── chief_complaints.json    # Medical dictionary data
    ├── bin/                     # Build output (ignored by git)
    │   └── Debug/net9.0/
    └── obj/                     # Build cache (ignored by git)
```

## ⚙️ Configuration

### Medical Dictionary

The system uses a built-in symptom dictionary with ~40 Thai medical terms:

- Primary symptoms: `ไข้`, `ไอ`, `เจ็บคอ`, `มีน้ำมูก`
- Compound symptoms: `แขนซ้ายอ่อนแรง`, `ขาขวาอ่อนแรง`
- Medical abbreviations: `BP ต่ำ`, `HR 60`, `ECG normal`

### Performance Settings

- **Fuzzy Match Threshold**: 70% similarity
- **Exact Match Threshold**: 100% coverage required
- **Optimization Threshold**: Strings >500 characters use optimized parsing
- **Max Segment Length**: 50 characters per non-dictionary segment

### Synonym Mappings

The system automatically handles common variations:

- `มีน้ำมูก` ↔ `น้ำมูก`
- `ปวดศีรษะ` ↔ `ปวดหัว`
- `BP ต่ำ` ↔ `BPต่ำ`
- `มีไข้` → `ไข้`

## 🧪 Testing

### Built-in Test Cases

The application includes 50+ comprehensive test cases covering:

- **Basic Matching**: Exact matches, partial matches
- **Compound Symptoms**: Complex symptom expansion
- **Fuzzy Matching**: Typo correction and synonyms
- **Edge Cases**: Empty strings, mixed languages, special characters
- **Performance**: Large concatenated strings, optimization tests
- **Case Sensitivity**: Medical abbreviation normalization

### Running Tests

```bash
# Run the application to see all test results
dotnet run

# Build and run with specific configuration
dotnet run --configuration Release
```

### Sample Test Cases

```
✅ PASS: "ไข้ ไอ เจ็บคอ มีน้ำมูก" vs "ไข้ ไอ เจ็บคอ มีน้ำมูก" = 100% match
✅ PASS: "BP ต่ำ" vs "bp ต่ำ" = 100% match (case normalization)
✅ PASS: "แขนขวาขาขวาอ่อนแรง" expands to individual limb symptoms
❌ FAIL: "ไข้ ไอ" vs "ไข้" = 50% coverage (missing symptoms)
```

## 🔧 Development Setup

### Using Visual Studio Code

1. Install C# extension
2. Open project folder
3. Use `Ctrl+Shift+P` → "C#: Generate Assets for Build and Debug"
4. Press `F5` to run with debugging

### Using Visual Studio

1. Open `ThaiTextCompare.csproj`
2. Set `ThaiTextCompare` as startup project
3. Press `F5` to run

### Using Command Line

```bash
# Development build
dotnet build --configuration Debug

# Production build
dotnet build --configuration Release

# Run with verbose output
dotnet run --verbosity detailed

# Clean build artifacts
dotnet clean
```

## 📊 Output Format

The application provides detailed comparison analysis:

```
Text1: 'ไข้ ไอ เจ็บคอ มีน้ำมูก'
Text2: 'ไข้ ไอ เจ็บคอ รถยนต์'
Tokens1: [ไข้, ไอ, เจ็บคอ, น้ำมูก] (4 tokens)
Tokens2: [ไข้, ไอ, เจ็บคอ, รถยนต์] (4 tokens)
✅ Matched: [ไข้, ไอ, เจ็บคอ] (3 words)
❌ Missing from Text2: [น้ำมูก] (1 words)
➕ Extra in Text2: [รถยนต์] (1 words)
Text1→Text2 Coverage: 75% (Primary Score)
Match Result: False
```

## 🚨 Troubleshooting

### Common Issues

#### 1. "No such command: dotnet"

**Solution**: Install .NET 9.0 SDK from https://dotnet.microsoft.com/download

#### 2. "Project file not found"

**Solution**: Ensure you're in the `ThaiTextCompare` directory with the `.csproj` file

#### 3. "Build failed" errors

**Solution**:

```bash
dotnet clean
dotnet restore
dotnet build
```

#### 4. "Out of memory" on large texts

**Solution**: The system automatically optimizes for strings >500 characters, but for extremely large inputs, consider chunking the text.

#### 5. Character encoding issues

**Solution**: Ensure your terminal/console supports UTF-8 encoding for proper Thai character display.

### Performance Issues

- For very large texts (>10,000 characters), consider implementing text chunking
- Monitor memory usage with large dictionaries
- Use Release build for production: `dotnet run --configuration Release`

### Debug Mode

```bash
# Run with detailed logging (if implemented)
dotnet run --configuration Debug

# Check build warnings
dotnet build --verbosity normal
```

## 🎯 Usage Examples

### Basic Comparison

```csharp
var result = CompareThaiMedicalTexts("ไข้ ไอ", "ไข้ ไอ เจ็บคอ", 100.0);
// Returns: coverage=100%, match=false (extra symptoms in text2)
```

### Compound Symptom Expansion

```csharp
// Input: "แขนขวาขาขวาอ่อนแรง"
// Expands to: "แขนขวาอ่อนแรง ขาขวาอ่อนแรง"
```

### Fuzzy Matching

```csharp
// "หายใจลำบาด" automatically corrects to "หายใจลำบาก"
// "น้ำมุก" automatically corrects to "น้ำมูก"
```

## 📈 Production Considerations

### For Production Deployment

1. Use Release build: `dotnet build --configuration Release`
2. Consider containerization with Docker
3. Implement proper logging and monitoring
4. Add input validation for web APIs
5. Consider caching for frequently compared texts

### API Integration

```csharp
// Example API wrapper
[HttpPost("compare")]
public ComparisonResult Compare([FromBody] CompareRequest request)
{
    return CompareThaiMedicalTexts(request.Text1, request.Text2, request.Threshold);
}
```

## 🤝 Contributing

### Code Style

- Follow C# naming conventions
- Use meaningful variable names
- Add XML documentation for public methods
- Include unit tests for new features

### Adding Medical Terms

Update the `SymptomDict` array in `Program.cs`:

```csharp
static readonly string[] SymptomDict = new[]
{
    // Add new terms here
    "ใหม่อาการ",
    // ...existing terms
};
```

## 📝 License

This project is for educational and medical research purposes. Please ensure compliance with your local medical data regulations when using in production environments.

## 📞 Support

For issues and questions:

1. Check this setup guide first
2. Review the troubleshooting section
3. Examine the built-in test cases for examples
4. Check the code comments for implementation details

---

**Version**: 1.0.0  
**Last Updated**: September 6, 2025  
**Compatibility**: .NET 9.0+, Windows/macOS/Linux
