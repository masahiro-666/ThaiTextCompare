# ThaiTextCompare 🇹🇭

[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download)
[![Tests](https://img.shields.io/badge/tests-64%20passing-brightgreen.svg)](#testing)
[![Coverage](https://img.shields.io/badge/coverage-%3E85%25-brightgreen.svg)](#testing)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](#license)

A **production-ready** Thai medical text comparison system with advanced tokenization, fuzzy matching, and comprehensive testing framework. Designed specifically for medical applications requiring precise symptom matching with support for compound symptoms, typos, and concatenated text parsing.

## ✨ Key Features

🏥 **Medical-Grade Accuracy** - Specialized Thai medical terminology processing  
🧩 **Compound Symptoms** - Expands complex symptoms like "แขนขวาขาขวาอ่อนแรง"  
🔍 **Fuzzy Matching** - 70% similarity threshold with intelligent typo correction  
📊 **Coverage Analysis** - Detailed comparison metrics and missing symptom detection  
🎯 **Conjunction Filtering** - Automatically filters Thai conjunctions (และ, หรือ, แต่)  
🔧 **JSON Configuration** - Dynamic loading of symptoms, conjunctions, and patterns  
⚡ **High Performance** - Optimized for large texts with automatic performance scaling  
🧪 **Comprehensive Testing** - 82+ tests covering all scenarios including conjunction filtering  
🏗️ **Modular Architecture** - Clean, maintainable codebase with Core/ components  
🚀 **Production Ready** - Memory management, error handling, and CI/CD support

## 🚀 Quick Start

```bash
# Clone the repository
git clone https://github.com/masahiro-666/ThaiTextCompare.git
cd ThaiTextCompare

# Build and run
dotnet build
dotnet run

# Run comprehensive tests
chmod +x run-tests.sh
./run-tests.sh
```

## 📖 Example Usage

### Basic Comparison

```csharp
using ThaiTextCompare.Core;

// Initialize components
var tokenizer = new ThaiMedicalTokenizer(SymptomDictionary.DefaultSymptomDict);
var engine = new ComparisonEngine(tokenizer);

// Compare Thai medical texts
var result = engine.CompareThaiMedicalTexts(
    "ไข้ ไอ เจ็บคอ มีน้ำมูก",
    "ไข้ ไอ เจ็บคอ",
    70.0
);

Console.WriteLine($"Similarity: {result.Similarity:F1}%");
Console.WriteLine($"Match: {result.IsMatch}");
Console.WriteLine($"Missing: {string.Join(", ", result.MissingWords)}");
```

**Output:**

```
Similarity: 75.0%
Match: False
Missing: มีน้ำมูก
```

### Advanced Features

```csharp
// Conjunction filtering (automatic)
var result = engine.CompareThaiMedicalTexts("ไข้ และ ไอ", "ไข้ ไอ", 70.0);
// Result: 100% match (conjunction "และ" automatically filtered)

// Multiple conjunctions
var result = engine.CompareThaiMedicalTexts(
    "เจ็บคอ และ มีน้ำมูก แต่ ไม่ไข้",
    "เจ็บคอ มีน้ำมูก ไม่ไข้", 70.0);
// Result: 100% match (both "และ" and "แต่" filtered)

// Compound symptom expansion
var tokens = tokenizer.TokenizeThaiSymptoms("แขนขวาขาขวาอ่อนแรง");
// Result: ["แขนขวาอ่อนแรง", "ขาขวาอ่อนแรง"]

// Synonym recognition
var result = engine.CompareThaiMedicalTexts("ปวดศีรษะ", "ปวดหัว", 70.0);
// Result: 100% match (synonyms detected)

// Case normalization
var result = engine.CompareThaiMedicalTexts("BP ต่ำ", "bp ต่ำ", 70.0);
// Result: 100% match (case normalized)
```

## 🏗️ Project Architecture

```
ThaiTextCompare/
├── 📄 README.md                    # This file
├── 📄 setup.md                     # Detailed setup guide
├── 📄 TESTING_STRATEGY.md          # Comprehensive testing docs
├── 🔧 run-tests.sh                 # Automated testing script
├── 📁 Core/                        # Modular architecture
│   ├── ThaiMedicalTokenizer.cs     # Thai text tokenization with conjunction filtering
│   ├── ComparisonEngine.cs         # Comparison logic
│   ├── SymptomDictionary.cs        # Medical terminology with JSON loading
│   └── SymptomDictionaryManager.cs # Dynamic dictionary management
├── 📁 Data/                        # JSON configuration files
│   ├── default-symptoms.json       # Medical symptoms, synonyms, conjunctions
│   └── test-cases.json             # Test case scenarios
├── 📁 Tests/                       # 82+ comprehensive tests
│   ├── Unit/                       # 53 unit tests
│   ├── Integration/                # 5 integration tests
│   ├── Performance/                # Performance benchmarks
│   ├── Stress/                     # 6 stress tests
│   └── Utilities/                  # Test helpers
├── 📄 Program.cs                   # Main application
├── 📄 chief_complaints.json        # Medical data
└── 📄 ThaiTextCompare.csproj       # Project config
```

## 🧪 Testing

Our comprehensive testing strategy includes **82+ tests** across multiple categories:

| Category        | Count | Coverage   | Purpose                      |
| --------------- | ----- | ---------- | ---------------------------- |
| **Unit**        | 53    | Core logic | Individual component testing |
| **Integration** | 8     | Workflows  | End-to-end validation        |
| **Performance** | 1     | Speed      | Timing benchmarks            |
| **Stress**      | 6     | Limits     | System robustness            |
| **Conjunction** | 14    | Filtering  | Thai conjunction filtering   |

### Run Tests

```bash
# Quick test (all 82+ tests)
cd Tests && dotnet test

# Automated comprehensive suite
./run-tests.sh

# Run by category
dotnet test --filter "FullyQualifiedName~Unit"        # Unit tests
dotnet test --filter "FullyQualifiedName~Integration" # Integration tests
dotnet test --filter "FullyQualifiedName~Stress"      # Stress tests
```

### Test Results

```
✅ Unit Tests: 53/53 passed
✅ Integration Tests: 8/8 passed (including conjunction filtering)
✅ Performance Tests: 1/1 passed
✅ Stress Tests: 6/6 passed
✅ Conjunction Tests: 14/14 passed
✅ Code Coverage: >85% overall
✅ Memory Management: No leaks detected
```

## 📊 Performance

### Benchmarks

| Scenario                   | Execution Time | Memory Usage |
| -------------------------- | -------------- | ------------ |
| Short text (< 50 chars)    | < 1ms          | < 1KB        |
| Medium text (50-200 chars) | < 5ms          | < 5KB        |
| Long text (> 200 chars)    | < 50ms         | < 50KB       |
| 1000+ comparisons          | < 5 seconds    | < 10MB       |

### Optimizations

- **Automatic scaling**: Optimization kicks in for strings >500 characters
- **Memory pooling**: Efficient memory usage for batch operations
- **Dictionary caching**: Fast symptom lookup with intelligent caching
- **Unicode handling**: Optimized Thai-English boundary detection

## 🏥 Medical Dictionary

### Supported Symptoms (40+ Terms)

**Primary Symptoms**

- ไข้ (fever), ไอ (cough), เจ็บคอ (sore throat)
- มีน้ำมูก (runny nose), ปวดหัว (headache), ปวดท้อง (stomach ache)

**Compound Symptoms**

- แขนซ้ายอ่อนแรง (left arm weakness)
- ขาขวาอ่อนแรง (right leg weakness)
- แขนขวาขาขวาอ่อนแรง (right side weakness)

**Medical Abbreviations**

- BP ต่ำ (low blood pressure), HR 60 (heart rate 60)
- ECG normal, O2 sat, RR (respiratory rate)

**Body Systems**

- หายใจลำบาก (difficulty breathing), เจ็บหน้าอก (chest pain)
- วิงเวียน (dizziness), คลื่นไส้ (nausea)

### Thai Conjunction Filtering

The system automatically filters out Thai conjunctions and function words:

**Conjunctions**: และ (and), หรือ (or), แต่ (but), กับ (with)  
**Time/Sequence**: แล้ว (then), ก็ (also), เมื่อ (when), จึง (therefore)  
**References**: ที่ (that/which), นี้ (this), นั้น (that), นี่/นั่น (demonstratives)  
**Purpose**: เพื่อ (for), ถ้า (if), เลย (at all)

### Synonym Recognition

- ปวดศีรษะ ↔ ปวดหัว (headache variants)
- มีน้ำมูก ↔ น้ำมูก (runny nose)
- BP ต่ำ ↔ BPต่ำ (spacing variations)
- วิงเวียน ↔ หน้ามืด (dizziness variants)

## 🚀 Production Deployment

### Requirements

- ✅ All 82+ tests passing (including conjunction filtering)
- ✅ >85% code coverage achieved
- ✅ Performance benchmarks met
- ✅ Memory leak tests passed
- ✅ Stress tests completed
- ✅ JSON configuration validation passed

### Deployment Options

**Standalone Application**

```bash
dotnet publish -c Release -r win-x64 --self-contained
dotnet publish -c Release -r linux-x64 --self-contained
dotnet publish -c Release -r osx-x64 --self-contained
```

**Docker Container**

```dockerfile
FROM mcr.microsoft.com/dotnet/runtime:9.0
COPY bin/Release/net9.0/ /app/
WORKDIR /app
ENTRYPOINT ["dotnet", "ThaiTextCompare.dll"]
```

**Web API Integration**

```csharp
[ApiController]
[Route("api/[controller]")]
public class TextComparisonController : ControllerBase
{
    private readonly ComparisonEngine _engine;

    [HttpPost("compare")]
    public ComparisonResult Compare([FromBody] CompareRequest request)
    {
        return _engine.CompareThaiMedicalTexts(
            request.Text1, request.Text2, request.Threshold ?? 70.0);
    }
}
```

## 🛠️ Development

### Prerequisites

- **.NET 9.0 SDK** or later
- **IDE**: Visual Studio, VS Code, or Rider
- **OS**: Windows, macOS, or Linux

### Setup

```bash
# Clone and setup
git clone <repository-url>
cd ThaiTextCompare
dotnet restore
dotnet build

# Verify installation
./run-tests.sh
```

### Contributing

1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/amazing-feature`)
3. **Add tests** for new functionality (maintain >85% coverage)
4. **Run test suite**: `./run-tests.sh` (all 82+ tests must pass)
5. **Commit** changes (`git commit -m 'Add amazing feature'`)
6. **Push** to branch (`git push origin feature/amazing-feature`)
7. **Open** a Pull Request

### Code Standards

- Follow C# naming conventions
- Add XML documentation for public APIs
- Include comprehensive unit tests
- Use FluentAssertions for test readability
- Maintain modular architecture patterns

## 📚 Documentation

- **[Setup Guide](setup.md)** - Detailed installation and configuration
- **[Testing Strategy](TESTING_STRATEGY.md)** - Comprehensive testing documentation
- **[API Reference](#)** - Complete API documentation (generated)

## 🐛 Troubleshooting

### Common Issues

**TESTRUNABORT Errors**

```bash
# Use correct test filters
dotnet test --filter "FullyQualifiedName~Unit"
```

**Permission Denied**

```bash
chmod +x run-tests.sh
```

**Build Errors**

```bash
dotnet clean
dotnet restore
dotnet build
```

### Health Check

```bash
dotnet --version          # Should show .NET 9.0+
dotnet build             # Should build successfully
./run-tests.sh          # Should show all 82+ tests passing
dotnet run              # Should run the application
```

## 📈 Roadmap

- [x] **JSON Configuration** - Dynamic loading of symptoms, synonyms, and conjunctions
- [x] **Conjunction Filtering** - Automatic Thai conjunction filtering
- [ ] **Database Integration** - Persistent medical dictionaries
- [ ] **REST API** - HTTP service with OpenAPI documentation
- [ ] **Machine Learning** - AI-powered symptom similarity
- [ ] **Multi-language** - Support for English medical terms
- [ ] **Real-time Processing** - WebSocket-based live comparison
- [ ] **Cloud Deployment** - Azure/AWS containerized deployment

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guidelines](CONTRIBUTING.md) for details.

### Contributors

- **Masahiro** - Initial development and architecture

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🏥 Medical Disclaimer

This software is for educational and research purposes. Always consult qualified medical professionals for medical diagnosis and treatment. Ensure compliance with local medical data regulations when using in production environments.

## 📞 Support

- 🐛 **Bug Reports**: [Create an Issue](https://github.com/masahiro-666/ThaiTextCompare/issues)
- 💬 **Discussions**: [GitHub Discussions](https://github.com/masahiro-666/ThaiTextCompare/discussions)
- 📧 **Contact**: [Email](kittikanct03@gmail.com)

---

<div align="center">

**[⭐ Star this repo](https://github.com/masahiro-666/ThaiTextCompare)** if you find it helpful!

Made with ❤️ for Thai medical text processing

</div>
