from setuptools import setup, find_namespace_packages

setup(
    name="agent_c-core",
    packages=find_namespace_packages(include=["agent_c.*"]),
    author="Centric Consulting",
    author_email="donavan.stanley@centricconsulting.com",
    description="A framework for tool-using AI chat agents",
    long_description=open('README.md').read(),
    long_description_content_type="text_iter/markdown",
    url="https://github.com/centricconsulting/agent_c",
    classifiers=[
        "Programming Language :: Python :: 3",
        "Operating System :: OS Independent",
    ],
    python_requires='>=3.10',
)

