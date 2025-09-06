#!/bin/bash

# Thai Text Compare Unity Build Script
# This script creates a Unity Package Manager compatible package

echo "🚀 Building Thai Text Compare for Unity Package Manager..."

# Clean previous builds
echo "🧹 Cleaning previous builds..."
dotnet clean > /dev/null 2>&1

# Build the project
echo "🔨 Building .NET 9.0 library..."
dotnet build --configuration Release > /dev/null 2>&1

if [ $? -eq 0 ]; then
    echo "✅ Build successful!"
    
    # Create Unity Package Manager structure
    echo "📦 Creating Unity Package Manager structure..."
    mkdir -p Unity/Runtime
    mkdir -p Unity/Samples~/BasicComparison
    
    # Copy DLL and documentation to Runtime folder
    cp bin/Release/net9.0/ThaiTextCompare.dll Unity/Runtime/
    cp bin/Release/net9.0/ThaiTextCompare.xml Unity/Runtime/
    
    # Copy data files
    cp -r Data Unity/
    
    echo "📁 Unity Package structure created at: ./Unity/"
    echo ""
    echo "� Git URL Installation:"
    echo "   https://github.com/masahiro-666/ThaiTextCompare.git?path=Unity"
    echo ""
    echo "📋 Manual Installation:"
    echo "1. Copy Unity/Runtime/ThaiTextCompare.dll to Assets/Plugins/"
    echo "2. Copy Unity/Data/ folder to Assets/StreamingAssets/Data/"
    echo "3. Import samples as needed"
    echo ""
    echo "🎯 Ready for Unity Package Manager!"
else
    echo "❌ Build failed! Check the error messages above."
    exit 1
fi
