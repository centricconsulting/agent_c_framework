#!/usr/bin/env python3
"""
Quick dependency check for PDF Converter Tool
"""

def check_dependencies():
    """Check if all required dependencies are available"""
    print("🔍 Checking PDF Converter Dependencies...")
    
    dependencies = {
        'PyPDF2': 'Required for PDF parsing',
        'reportlab': 'Optional - for creating test PDFs',
        'base64': 'Built-in - for encoding/decoding',
        'json': 'Built-in - for JSON output',
        'io': 'Built-in - for BytesIO operations'
    }
    
    results = {}
    
    for dep, description in dependencies.items():
        try:
            if dep == 'PyPDF2':
                import PyPDF2
                version = getattr(PyPDF2, '__version__', 'unknown')
                results[dep] = f"✅ Available (v{version}) - {description}"
            elif dep == 'reportlab':
                import reportlab
                version = getattr(reportlab, '__version__', 'unknown')
                results[dep] = f"✅ Available (v{version}) - {description}"
            else:
                __import__(dep)
                results[dep] = f"✅ Available - {description}"
        except ImportError:
            results[dep] = f"❌ Missing - {description}"
    
    # Print results
    for dep, status in results.items():
        print(f"  {status}")
    
    # Check if critical dependencies are available
    critical_missing = []
    if "❌" in results.get('PyPDF2', ''):
        critical_missing.append('PyPDF2')
    
    if critical_missing:
        print(f"\n❌ Critical dependencies missing: {', '.join(critical_missing)}")
        print("   Install with: pip install PyPDF2")
        return False
    else:
        print(f"\n✅ All critical dependencies available!")
        return True

if __name__ == "__main__":
    success = check_dependencies()
    exit(0 if success else 1)