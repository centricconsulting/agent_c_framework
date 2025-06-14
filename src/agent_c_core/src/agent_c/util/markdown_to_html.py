from typing import List, Optional

import markdown

default_markdown_extensions: List[str] = ['nl2br', # Will cause newlines to be treated as hard breaks;
                                                   # like StackOverflow and GitHub flavored Markdown do.
                                          'attr_list', # adds a syntax to define attributes on the various HTML elements in markdown’s output.
                                          'pymdownx.blocks.details', # Creates collapsible elements that hide their content
                                          'pymdownx.betterem', # improve emphasis (bold and italic) handling
                                          'pymdownx.superfences', # Allows the nesting of fences under blockquotes, lists,
                                                                  # or other block elements (see Limitations for more info).
                                                                  # Give the ability to specify custom fences to provide
                                                                  # features like flowcharts, sequence diagrams, or other custom blocks.
                                          'markdown.extensions.footnotes', # Adds syntax for defining footnotes in Markdown documents.
                                          'markdown.extensions.def_list',  # Adds the ability to create definition lists in Markdown documents.
                                          'markdown.extensions.tables', # Adds the ability to create tables in Markdown documents.
                                                                        # Tables are defined using the syntax established in PHP Markdown Extra.
                                                                        # https://michelf.ca/projects/php-markdown/extra/#table
                                          'markdown.extensions.abbr', # Adds the ability to define abbreviations.
                                                                      # Specifically, any defined abbreviation is wrapped in an <abbr> tag.
                                          'markdown.extensions.md_in_html', # Parses Markdown inside of HTML tags.
                                                                            # (By default, Markdown ignores any content within a raw HTML block-level element)
                                          'pymdownx.b64', # B64 allows for the embedding of local PNG, JPEG, and GIF image references with base64 encoding.
                                                          # It simply needs a base path to resolve relative links in the Markdown source.
                                                          # The base path is the assumed location of the Markdown source at time of conversion.
                                                          # Using the base path, B64 will search and find the actual img tag references (both absolute and relative)
                                                          # and base64 encode and embed them in the HTML output.
                                          'pymdownx.caret', # adds two different features which are syntactically built around the ^ character.
                                                            # The first is insert which inserts <ins></ins> tags.
                                                            # The second is superscript which inserts <sup></sup> tags.
                                          'pymdownx.fancylists', # Extends the list handling formats to support parenthesis style lists along with additional ordered formats.
                                                                # Support ordered lists with either a trailing dot or single right-parenthesis: 1. or 1).
                                                                # Support ordered lists with roman numeral formats, both lowercase and uppercase.
                                                                #   Uppercase is treated as a different list type than lowercase.
                                                                # Support ordered lists with alphabetical format, both lowercase and uppercase.
                                                                #   Uppercase is treated as a different list type than lowercase.
                                                                # Support a generic ordered list marker via #. or #).
                                                                #   These can be used in place of numerals and will inherit the type of the current list as long as
                                                                #   they use the same convention (. or )). If used to start a list, decimal format will be assumed.
                                                                # Using a different list type will start a new list.
                                                                #   Trailing dot vs parenthesis are treated as separate types.
                                                                # Ordered lists are sensitive to the starting value and can restart a list or
                                                                # create a new list using the first value in the list.
                                          'pymdownx.saneheaders', # Requires headers to have spaces after the hashes (#) in order to be recognized as headers.
                                                                  # This allows for other extension syntaxes to use # in their syntaxes as long as no spaces
                                                                  # follow the # at the beginning of a line.
                                          'pymdownx.highlight', # adds support for code highlighting.
                                                                # Its purpose is to provide a single place to configure syntax highlighting for code blocks.
                                                                # Both InlineHilite and SuperFences can use Highlight to configure their highlight settings,
                                                                # but Highlight will also run all non-fence code blocks through the highlighter as well.
                                          'pymdownx.inlinehilite', # utilizes the following syntax to insert inline highlighted code:
                                                                   # `:::language mycode` or `#!language mycode`.
                                                                   # In CodeHilite, #! means "use line numbers", but line numbers will never be used in inline code
                                                                   # regardless of which form is used. Use of one form or the other is purely for personal preference.
                                          'pymdownx.keys', # Makes entering and styling keyboard key presses easier by converting them to images.
                                                           # Syntactically, Keys is built around the + symbol.
                                                           # A key or combination of key presses is surrounded by ++ with each key press separated with a single +.
                                                           # For example, to create a key press images for Ctrl+Alt+Del, you would use ++Ctrl+Alt+Del++.
                                          'pymdownx.magiclink', # Provides a number of useful link related features.
                                                                # MagicLink can auto-link HTML, FTP, and email links.
                                                                # It can auto-convert repository links (GitHub, GitLab, and Bitbucket) and display them in a more concise,
                                                                # shorthand format.
                                                                # MagicLink can also be configured to directly auto-link the aforementioned shorthand format.
                                          'pymdownx.mark', # Adds the ability to insert <mark></mark> tags.
                                                           # The syntax requires the text to be surrounded by double equal signs. It can optionally be configured to use smart logic. Syntax behavior for smart and non-smart variants of mark models that of BetterEm.
                                                           # To Mark some text, simply surround the text with double =.
                                          'pymdownx.pathconverter', # can convert local, relative reference paths to absolute or relative paths for links and images.
                                          'pymdownx.progressbar', # Adds support for progress/status bars. It can take percentages or fractions, and it can optionally
                                                                  # generate classes for percentages at specific value levels.
                                                                  # It also works with the attr_list extension.
                                                                  # The basic syntax for progress bars is: [= <percentage or fraction> "optional single or double quoted title"].
                                                                  # The opening [ can be followed by one or more = characters.
                                                                  # After the = char(s) the percentage is specified as either a fraction or percentage and can
                                                                  # optionally be followed by a title surrounded in either double quotes or single quotes.
                                                                  #
                                                                  # [=0% "0%"]
                                                                  # [=5% "5%"]
                                                                  # [=25% "25%"]
                                                                  # [=45% "45%"]
                                                                  # [=65% "65%"]
                                                                  # [=85% "85%"]
                                                                  # [=100% "100%"]
                                          'smarty', # Converts ASCII dashes, quotes and ellipses to their HTML entity equivalents.
                                          'pymdownx.smartsymbols', # Adds syntax for creating special characters such as trademarks, arrows, fractions, etc.
                                          'pymdownx.tasklist', # adds GFM style task lists. They follow the same syntax as GFM.
                                                               # Simply start each list item with a square bracket pair containing either a space (an unchecked item)
                                                               # or a x (a checked item).
                                          'pymdownx.tilde', #  adds two different features which are syntactically built around the ~ character:
                                                            # delete which inserts <del></del> tags and subscript which inserts <sub></sub> tags.
                                          'pymdownx.blocks.admonition',  # Adds rST-style admonitions to Markdown documents.
                                          'pymdownx.blocks.caption',    # Allows for wrapping blocks in <figure> tags and inserting a <figcaption> tag with
                                                                        # specified content.
                                          'pymdownx.blocks.definition', # Adds the ability to create definition lists in Markdown documents.
                                          'pymdownx.blocks.html', # The HTML block allows a user to wrap Markdown in arbitrary HTML elements.
                                          'pymdownx.blocks.tabbed', # creates collapsible elements that hide their content.
                                                                    # It uses the HTML5 <details><summary> tags to accomplish this.
                                                                    # It supports nesting and you can also force the default state to be open
                                          'toc', # Generates a Table of Contents from a Markdown document and adds it into the resulting HTML document.
                                          'wikilinks', # The WikiLinks extension adds support for WikiLinks. Specifically, any [[bracketed]] word
                                                       # is converted to a link.
                                         ]

def mmd_to_html(markdown_text: str, extensions: Optional[List[str]] = None) -> str:
    """
    Convert Markdown text to HTML.

    Args:
        markdown_text (str): The Markdown text to convert.
        extensions (Optional[List[str]]): A list of Markdown extensions to use for conversion.
            If None, the default extensions will be used.

    Returns:
        str: The converted HTML text.
    """
    if extensions is None:
        extensions = default_markdown_extensions

    html = markdown.markdown(markdown_text, extensions=extensions)

    return html