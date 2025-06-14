from setuptools import setup, find_packages

setup(
    name="ts_tool",
    version="0.1.0",
    packages=find_packages(where="src"),
    package_dir={"":"src"},
    install_requires=[
        "tree-sitter>=0.20.1",
        "tree-sitter-python",
        "tree-sitter-c-sharp",
        "tree-sitter-javascript",
    ],
    extras_require={
        "dev": [
            "pytest>=7.0.0",
            "pytest-cov>=4.0.0",
            "pytest-mock>=3.10.0"
        ],
    },
    author="Your Name",
    author_email="your.email@example.com",
    description="A tool for code analysis using tree-sitter",
    long_description=open("README.md").read(),
    long_description_content_type="text/markdown",
    url="https://github.com/yourusername/ts_tool",
    classifiers=[
        "Development Status :: 3 - Alpha",
        "Intended Audience :: Developers",
        "License :: OSI Approved :: MIT License",
        "Programming Language :: Python :: 3",
        "Programming Language :: Python :: 3.8",
    ],
)