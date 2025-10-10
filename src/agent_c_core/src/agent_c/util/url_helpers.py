#!/usr/bin/env python3
import os
import base64
import argparse

def bytes_to_data_uri(data: bytes, src_path: str = "image.png") -> str:
    """
    Convert bytes to a data URI.

    Args:
        data (bytes): The byte data to convert.
        src_path (str): The source path for the data URI.

    Returns:
        str: The data URI string.
    """
    ext = os.path.splitext(src_path)[1].lower()
    mime_map = {
        '.png':  'image/png',
        '.jpg':  'image/jpeg',
        '.jpeg': 'image/jpeg',
        '.gif':  'image/gif',
        '.webp': 'image/webp',
        '.bmp':  'image/bmp',
        '.svg':  'image/svg+xml',
    }
    mime_type = mime_map.get(ext, 'application/octet-stream')
    b64 = base64.b64encode(data).decode('ascii')
    return f"data:{mime_type};base64,{b64}"

def main():
    parser = argparse.ArgumentParser(
        description="Convert an image file to a base64 data URI"
    )
    parser.add_argument(
        "src_path",
        help="Path to the image file (e.g. ./images/logo.png)"
    )
    args = parser.parse_args()

    if not os.path.isfile(args.src_path):
        parser.error(f"File not found: {args.src_path}")

    with open(args.src_path, "rb") as f:
        img_bytes = f.read()

    print(bytes_to_data_uri(img_bytes, args.src_path))

if __name__ == "__main__":
    main()
