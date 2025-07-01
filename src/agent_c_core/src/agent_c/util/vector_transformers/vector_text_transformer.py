import re
import spacy
import spacy.cli

from nltk.tokenize import word_tokenize
from nltk.corpus import stopwords

from agent_c.util.vector_transformers.vector_transform_options_model import VectorTransformOptionsModel


class VectorTextTransformer:
    """
    Allows the "cleaning" of text in preparation for vectorization
    """

    def __init__(self, options: VectorTransformOptionsModel = VectorTransformOptionsModel()):
        self.make_lower: bool = options.make_lower
        self.remove_punct: bool = options.remove_punct
        self.remove_newlines: bool = options.remove_newlines
        self.remove_unicode: bool = options.remove_unicode
        self.remove_markdown: bool = options.remove_markdown
        self.lemmatize: bool = options.lemmatize
        self.strip_stop_words: bool = options.strip_stop_words
        self.language = options.language

        try:
            self.nlp = spacy.load(self.language) if self.lemmatize else None
        except OSError:
            print(f"lemmatization model ({self.language}) not found. Downloading now...")
            spacy.cli.download(self.language)
            self.nlp = spacy.load(self.language)

    def transform_text(self, text) -> str:
        if self.make_lower:
            text = text.lower()

        if self.remove_markdown:
            # Remove markdown headers from the start of lines
            text = re.sub(r'^#+\s*', '', text, flags=re.M)

            # Remove bullets
            text = re.sub(r'^\*+\s*', '', text, flags=re.M)

        if self.remove_newlines:
            # Replace newlines with spaces
            text = text.replace("\n", " ")

        if self.remove_punct:
            # Remove punctuation that could be part of a number but isn't
            # period, commas and dashes that are preceded or followed by a number
            text = re.sub(r"(?<!\d)[.,\-](?!\d)", '', text)
            text = re.sub(r',(?!\d)', '',
                          text)  # Grab the commas that follow numbers but don't have numbers following them

            # now the rest
            text = re.sub(r"[;:?!\'()\"]", "", text)

        if self.remove_unicode:
            # Remove unicode characters
            text = text.encode('ascii', 'ignore').decode()

        if self.strip_stop_words:
            stop_words = set(stopwords.words('english'))
            word_tokens = word_tokenize(text)
            text = ' '.join([w for w in word_tokens if w not in stop_words])

        # lemmatize the text, this should always be the last step
        if self.lemmatize:
            paragraphs = text.split('\n')

            lemmatized_paragraphs = []
            for paragraph in paragraphs:
                doc = self.nlp(paragraph)
                lemmatized_paragraph = " ".join([token.lemma_ for token in doc])
                lemmatized_paragraphs.append(lemmatized_paragraph)

            text = "\n".join(lemmatized_paragraphs)

        return text
