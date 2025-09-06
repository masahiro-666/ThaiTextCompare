# Changelog

All notable changes to the Thai Text Compare Unity package will be documented in this file.

## [1.0.0] - 2025-09-06

### Added

- Initial Unity Package Manager support
- Git URL installation via Package Manager
- Core Thai medical text comparison engine
- Fuzzy matching with Levenshtein distance
- Typo correction for common Thai character substitutions
- Medical symptom dictionary with 800+ terms
- Compound symptom pattern recognition
- Real-time performance (~0.2ms per comparison)
- Unity MonoBehaviour example scripts
- Sample scenes and documentation
- Assembly definition files for proper Unity integration
- Thread-safe operations for Unity's threading model

### Features

- **Performance**: Average 0.20ms per comparison, 5000+ comparisons/second
- **Memory**: Stable 1.8MB memory usage with no leaks
- **Compatibility**: Unity 2021.3 LTS or higher
- **Platform Support**: Windows, macOS, iOS, Android
- **Language Support**: Full Thai UTF-8 character support

### Package Structure

- Runtime assembly with precompiled DLL
- Sample scripts demonstrating basic and advanced usage
- Comprehensive API documentation
- Medical terminology data files
- Unity Package Manager integration

### API

- `ThaiMedicalTokenizer`: Core tokenization engine
- `ComparisonEngine`: Main comparison algorithms
- `ComparisonResult`: Detailed result analysis
- Support for synonym mapping and abbreviation normalization
- Function word filtering and conjunction handling

### Documentation

- Complete API reference
- Unity integration guide
- Installation instructions for Git URL and manual methods
- Performance benchmarks and optimization tips
- Sample code and usage examples
