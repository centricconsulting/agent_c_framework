from setuptools import setup, find_namespace_packages

setup(
    name="agent_c-api",
    packages=find_namespace_packages(include=["agent_c.*"]),
    package_dir={'': 'src'},
    author="Centric Consulting",
    author_email="joseph.ours@centricconsulting.com",
    description="FastAPI API Wrapper Agent C.",
    long_description=open('README.md').read(),
    long_description_content_type="text/markdown",
    url="https://github.com/centricconsulting/agent_c",
    classifiers=[
        "Programming Language :: Python :: 3",
        "Operating System :: OS Independent",
    ],
    python_requires='>=3.12',
    setup_requires=["setuptools>=40.1.0"]
)
