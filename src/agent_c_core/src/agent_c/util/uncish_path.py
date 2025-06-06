class UNCishPath:
    """
    A class to represent a UNC path.
    """

    def __init__(self, path: str):
        if not path.startswith("//"):
            raise ValueError(f"Invalid path format: {path}. Must start with //")

        source, path = path.removeprefix("//").split('/', 1)

        self.source = source
        self.path = path if path else "./"
        self.path_parts = self.path.split("/")


    def __str__(self):
        return f"//{self.source}/{self.path}"

    def __repr__(self):
        return f"UNCishPath(source='{self.source}', path='{self.path}')"