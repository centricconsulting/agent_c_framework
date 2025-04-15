from setuptools import setup, find_namespace_packages

setup(
    name="agent_c-tools",
    packages=find_namespace_packages(include=["agent_c.*"]),
    package_dir={'': 'src'},
    author="Centric Consulting",
    author_email="donavan.stanley@centricconsulting.com",
    description="Reference toolsets for Agent C.",
    long_description=open('README.md').read(),
    long_description_content_type="text_iter/markdown",
    url="https://github.com/centricconsulting/agent_c",
    classifiers=[
        "Programming Language :: Python :: 3",
        "Operating System :: OS Independent",
    ],
    python_requires='>=3.10',
    install_requires=[
        "agent_c-core",
        "mcp",
        "pyyaml",
    ],
    entry_points={
        'console_scripts': [
            'mcp-tools-server=agent_c_tools.server.cli:main',
        ],
    },
)

