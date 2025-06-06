import os
import random
import hashlib
from typing import Optional, Union

random.seed(int.from_bytes(os.urandom(4), byteorder='big'))

class MnemonicSlugs:
    """
    MnemonicSlugs generates easy to remember slugs. It uses a slightly enhanced
    word list curated by Oren Tirosh as part of his mnemonic encoding project.
    The original project can be found at https://github.com/devshane/mnemonicode.
    """

    _WORDS = [
        "abraham", "absent", "absorb", "absurd", "academy", "accent", "acid", "acrobat", "action", "active", "actor", "adam",
        "address", "adios", "admiral", "adrian", "africa", "agatha", "agenda", "agent", "airline", "airport", "alabama", "aladdin",
        "alamo", "alarm", "alaska", "albert", "albino", "album", "alcohol", "alert", "alex", "alfonso", "alfred", "algebra",
        "alias", "alibi", "alice", "alien", "almanac", "almond", "aloha", "alpha", "alpine", "amadeus", "amanda", "amazon",
        "amber", "ambient", "america", "amigo", "ammonia", "anagram", "analog", "analyze", "anatomy", "andrea", "android", "andy",
        "angel", "animal", "anita", "annex", "annual", "answer", "antenna", "antonio", "anvil", "apollo", "appear", "apple",
        "april", "apropos", "arcade", "archer", "archive", "arctic", "arena", "ariel", "arizona", "armada", "armani", "armor",
        "arnold", "aroma", "arrow", "arsenal", "arthur", "artist", "asia", "aspect", "aspirin", "athena", "athlete", "atlanta",
        "atlas", "atomic", "audio", "august", "aurora", "austin", "austria", "avalon", "avatar", "avenue", "average", "axiom",
        "axis", "aztec", "baboon", "baby", "bagel", "baggage", "bahama", "baker", "balance", "bali", "ballad", "ballet",
        "balloon", "balsa", "bambino", "bamboo", "banana", "bandit", "banjo", "bank", "barbara", "barcode", "baron", "basic",
        "basil", "basket", "battery", "bazaar", "bazooka", "beach", "beast", "beatles", "beauty", "before", "begin", "belgium",
        "benefit", "benny", "berlin", "bermuda", "bernard", "betty", "between", "beyond", "bicycle", "bikini", "billy", "binary",
        "bingo", "biology", "biscuit", "bishop", "bison", "blast", "bless", "blitz", "block", "blonde", "blue", "bogart",
        "bombay", "bonanza", "bonjour", "bonus", "book", "border", "boris", "boston", "botanic", "bottle", "boxer", "brain",
        "brandy", "brave", "bravo", "brazil", "bread", "break", "brenda", "bridge", "brigade", "british", "broken", "bronze",
        "brother", "brown", "bruce", "bruno", "brush", "bucket", "budget", "buenos", "buffalo", "bundle", "burger", "burma",
        "button", "buzzer", "byte", "cabaret", "cabinet", "cable", "cactus", "cadet", "caesar", "cafe", "cairo", "cake",
        "calibre", "calypso", "camel", "camera", "camilla", "campus", "canada", "canal", "canary", "candid", "candle", "cannon",
        "canoe", "cantina", "canvas", "canyon", "capital", "capsule", "caramel", "caravan", "carbon", "career", "cargo", "carlo",
        "carmen", "carol", "carpet", "carrot", "cartel", "cartoon", "casino", "castle", "castro", "catalog", "cave", "caviar",
        "cecilia", "cello", "celtic", "cement", "center", "century", "ceramic", "chamber", "chance", "change", "channel", "chant",
        "chaos", "chapter", "chariot", "charlie", "charm", "charter", "cheese", "chef", "chemist", "cherry", "chess", "chicago",
        "chicken", "chief", "child", "china", "choice", "chris", "chrome", "cigar", "cinema", "cipher", "circle", "circus",
        "citizen", "citrus", "city", "civil", "clara", "clarion", "clark", "classic", "claudia", "clean", "clever", "client",
        "cliff", "climax", "clinic", "clock", "clone", "cloud", "club", "cobalt", "cobra", "cockpit", "coconut", "cola",
        "collect", "colombo", "colony", "color", "combat", "combine", "comedy", "comet", "command", "common", "compact", "company",
        "complex", "comrade", "conan", "concert", "condor", "conduct", "confide", "congo", "connect", "consul", "contact", "context",
        "contour", "control", "convert", "cool", "copper", "copy", "coral", "corner", "corona", "correct", "cosmos", "costume",
        "cotton", "couple", "courage", "cover", "cowboy", "crack", "craft", "crash", "crater", "credit", "cricket", "crimson",
        "critic", "crown", "crystal", "cuba", "cubic", "culture", "cupid", "current", "cycle", "cyclone", "dallas", "dance",
        "daniel", "danube", "darwin", "data", "david", "deal", "decade", "decide", "decimal", "declare", "define", "degree",
        "delete", "deliver", "delphi", "delta", "deluxe", "demand", "demo", "denmark", "denver", "depend", "deposit", "derby",
        "desert", "design", "desire", "detail", "detect", "develop", "dexter", "diagram", "dialog", "diamond", "diana", "diego",
        "diesel", "diet", "digital", "dilemma", "dinner", "diploma", "direct", "disco", "disney", "dispute", "distant", "divide",
        "doctor", "dolby", "dollar", "dolphin", "domain", "domino", "donald", "donor", "door", "double", "dragon", "drama",
        "dream", "drink", "driver", "druid", "drum", "dublin", "duet", "dynamic", "dynasty", "eagle", "earth", "east",
        "easy", "echo", "eclipse", "ecology", "economy", "eddie", "edgar", "edison", "edition", "editor", "educate", "edward",
        "effect", "ego", "egypt", "elastic", "electra", "elegant", "element", "elite", "elvis", "email", "emerald", "emotion",
        "empire", "empty", "energy", "engine", "english", "enigma", "enjoy", "enrico", "episode", "epoxy", "equal", "equator",
        "eric", "erosion", "escape", "escort", "eternal", "ethnic", "europe", "evening", "event", "everest", "evident", "evita",
        "exact", "example", "except", "exhibit", "exile", "exit", "exodus", "exotic", "expand", "explain", "explore", "export",
        "express", "extend", "extra", "extreme", "fabric", "factor", "falcon", "fame", "family", "famous", "fantasy", "farmer",
        "fashion", "fast", "father", "fax", "felix", "ferrari", "fiber", "fiction", "fidel", "field", "fiesta", "figure",
        "film", "filter", "final", "finance", "finish", "finland", "fiona", "fire", "first", "fish", "flag", "flame",
        "flash", "flex", "flipper", "float", "flood", "floor", "florida", "flower", "fluid", "flute", "focus", "folio",
        "food", "forbid", "ford", "forest", "forever", "forget", "formal", "formula", "fortune", "forum", "forward", "fossil",
        "fractal", "fragile", "frame", "france", "frank", "freddie", "freedom", "fresh", "friday", "friend", "frog", "front",
        "frozen", "fruit", "fuel", "fuji", "future", "gabriel", "galaxy", "galileo", "gallery", "gallop", "game", "gamma",
        "garage", "garbo", "garcia", "garden", "garlic", "gate", "gelatin", "gemini", "general", "genesis", "genetic", "geneva",
        "genius", "gentle", "genuine", "george", "germany", "giant", "gibson", "gilbert", "ginger", "giraffe", "gizmo", "glass",
        "global", "gloria", "goblin", "gold", "golf", "gondola", "gong", "good", "gopher", "gordon", "gorilla", "gossip",
        "grace", "gram", "grand", "granite", "graph", "gravity", "gray", "greek", "green", "gregory", "grid", "griffin",
        "grille", "ground", "group", "guest", "guide", "guitar", "guru", "gustav", "gyro", "habitat", "hair", "halt",
        "hamlet", "hammer", "hand", "happy", "harbor", "harlem", "harmony", "harris", "harvard", "harvest", "havana", "hawaii",
        "hazard", "heart", "heaven", "heavy", "helena", "helium", "hello", "henry", "herbert", "herman", "heroic", "hexagon",
        "hilton", "hippie", "history", "hobby", "holiday", "honey", "hope", "horizon", "horse", "hostel", "hotel", "house",
        "human", "humor", "hunter", "husband", "hydro", "ibiza", "iceberg", "icon", "idea", "idiom", "igloo", "igor",
        "image", "imagine", "imitate", "immune", "impact", "import", "impress", "inca", "inch", "include", "index", "india",
        "indigo", "infant", "info", "ingrid", "initial", "input", "insect", "inside", "instant", "invent", "invest", "invite",
        "iris", "ironic", "isabel", "isotope", "italian", "ivan", "ivory", "jacket", "jackson", "jacob", "jaguar", "jamaica",
        "james", "janet", "japan", "jargon", "jasmine", "jason", "java", "jazz", "jeep", "jerome", "jessica", "jester",
        "jet", "jimmy", "job", "joel", "john", "join", "joker", "jordan", "joseph", "joshua", "journal", "judge",
        "judo", "juice", "juliet", "julius", "july", "jumbo", "jump", "june", "jungle", "junior", "jupiter", "justice",
        "kansas", "karate", "karl", "karma", "kayak", "kermit", "kevin", "kilo", "kimono", "kinetic", "king", "kitchen",
        "kiwi", "koala", "korea", "labor", "ladder", "lady", "lagoon", "lake", "laptop", "laser", "latin", "laura",
        "lava", "lazarus", "learn", "lecture", "left", "legacy", "legal", "legend", "lemon", "leonid", "leopard", "lesson",
        "letter", "level", "lexicon", "liberal", "libra", "license", "life", "light", "lily", "lima", "limbo", "limit",
        "linda", "linear", "lion", "liquid", "list", "liter", "lithium", "little", "llama", "lobby", "lobster", "local",
        "locate", "logic", "logo", "lola", "london", "lopez", "lorenzo", "lotus", "love", "loyal", "lucas", "lucky",
        "lunar", "lunch", "machine", "macro", "madam", "madonna", "madrid", "maestro", "magenta", "magic", "magnet", "magnum",
        "mailbox", "major", "malta", "mama", "mambo", "mammal", "manager", "mango", "manila", "manual", "marble", "marco",
        "margin", "margo", "marina", "marion", "market", "mars", "martin", "marvin", "mary", "mask", "master", "match",
        "matrix", "maximum", "maxwell", "mayday", "mayor", "maze", "meaning", "media", "medical", "medusa", "mega", "melody",
        "melon", "member", "memo", "memphis", "mental", "mentor", "menu", "mercury", "mercy", "message", "metal", "meteor",
        "meter", "method", "metro", "mexico", "miami", "michael", "micro", "middle", "miguel", "mike", "milan", "mile",
        "milk", "miller", "million", "mimic", "mimosa", "mineral", "minimum", "minus", "minute", "miracle", "mirage", "miranda",
        "mirror", "mission", "mister", "mixer", "mobile", "model", "modem", "modern", "modest", "modular", "moment", "monaco",
        "monarch", "monday", "money", "monica", "monitor", "monkey", "mono", "monster", "montana", "moral", "morgan", "morning",
        "morph", "morris", "moses", "motel", "mother", "motif", "motor", "mouse", "mozart", "multi", "museum", "music",
        "mustang", "mystery", "mystic", "nadia", "nancy", "natasha", "native", "nato", "natural", "navy", "nebula", "nectar",
        "needle", "nelson", "neon", "nepal", "neptune", "nerve", "network", "neuron", "neutral", "nevada", "never", "news",
        "newton", "next", "nice", "nickel", "night", "nikita", "nina", "ninja", "nirvana", "nissan", "nitro", "nixon",
        "nobel", "nobody", "noise", "nominal", "normal", "north", "norway", "nothing", "nova", "novel", "nuclear", "null",
        "number", "numeric", "nurse", "nylon", "oasis", "oberon", "object", "obscure", "observe", "ocean", "octavia", "october",
        "octopus", "office", "ohio", "olga", "oliver", "olivia", "olympic", "omega", "open", "opera", "opinion", "optic",
        "optimal", "option", "opus", "orange", "orbit", "orca", "orchid", "order", "oregano", "organic", "orient", "origami",
        "origin", "orinoco", "orion", "orlando", "oscar", "othello", "outside", "oval", "owner", "oxford", "oxygen", "ozone",
        "pablo", "pacific", "package", "page", "pagoda", "paint", "palace", "palma", "pamela", "panama", "pancake", "panda",
        "pandora", "panel", "panic", "panther", "papa", "paper", "paprika", "paradox", "pardon", "parent", "paris", "parker",
        "parking", "parlor", "parody", "parole", "partner", "passage", "passive", "pasta", "pastel", "patent", "patient", "patriot",
        "patrol", "pattern", "paul", "peace", "pearl", "pedro", "pegasus", "pelican", "pencil", "penguin", "people", "pepper",
        "percent", "perfect", "perform", "perfume", "period", "permit", "person", "peru", "phantom", "philips", "phoenix", "phone",
        "photo", "phrase", "piano", "picasso", "picnic", "picture", "pierre", "pigment", "pilgrim", "pilot", "pinball", "pioneer",
        "pirate", "pixel", "pizza", "place", "planet", "plasma", "plastic", "plate", "plato", "plaza", "plume", "pluto",
        "pocket", "podium", "poem", "poetic", "pogo", "point", "poker", "polaris", "police", "politic", "polka", "polo",
        "polygon", "poncho", "pony", "popcorn", "popular", "portal", "postage", "potato", "powder", "prague", "precise", "prefer",
        "prefix", "prelude", "premium", "prepare", "present", "press", "presto", "pretend", "pretty", "price", "prime", "prince",
        "printer", "prism", "private", "prize", "process", "prodigy", "product", "profile", "program", "project", "promise", "promo",
        "prosper", "protect", "proton", "provide", "proxy", "public", "pulse", "puma", "pump", "pupil", "puzzle", "pyramid",
        "python", "quality", "quarter", "queen", "quest", "quick", "quiet", "quiz", "quota", "rabbit", "race", "rachel",
        "radar", "radical", "radio", "radius", "rainbow", "raja", "ralph", "ramirez", "random", "ranger", "rapid", "ravioli",
        "raymond", "rebel", "record", "recycle", "reflex", "reform", "regard", "region", "regular", "relax", "remark", "remote",
        "rent", "repair", "reply", "report", "reptile", "respect", "respond", "result", "resume", "reverse", "reward", "rhino",
        "ribbon", "ricardo", "richard", "rider", "right", "ringo", "rio", "risk", "ritual", "rival", "river", "riviera",
        "road", "robert", "robin", "robot", "rocket", "rodent", "rodeo", "roger", "roman", "romeo", "rondo", "rose",
        "round", "rover", "royal", "rubber", "ruby", "rudolf", "rufus", "russian", "sabine", "sabrina", "saddle", "safari",
        "saga", "sahara", "sailor", "saint", "salad", "salami", "salary", "salmon", "salon", "salsa", "salt", "salute",
        "samba", "sample", "samuel", "sandra", "santana", "sardine", "saturn", "savage", "scale", "scarlet", "scholar", "school",
        "scoop", "scorpio", "scratch", "screen", "script", "scroll", "scuba", "season", "second", "secret", "section", "sector",
        "secure", "segment", "select", "seminar", "senator", "senior", "sensor", "serial", "serpent", "service", "shadow", "shake",
        "shallow", "shampoo", "shannon", "sharon", "sharp", "shave", "shelf", "shelter", "sheriff", "sherman", "shine", "ship",
        "shirt", "shock", "shoe", "short", "shrink", "side", "sierra", "sigma", "signal", "silence", "silicon", "silk",
        "silver", "similar", "simon", "simple", "sinatra", "sincere", "single", "siren", "sister", "size", "ski", "slalom",
        "slang", "sleep", "slogan", "slow", "small", "smart", "smile", "smoke", "snake", "snow", "social", "society",
        "soda", "sofia", "solar", "solid", "solo", "sonar", "sonata", "song", "sonic", "soprano", "sound", "source",
        "south", "soviet", "spain", "spark", "sparta", "special", "speech", "speed", "spell", "spend", "sphere", "spider",
        "spiral", "spirit", "split", "sponsor", "spoon", "sport", "spray", "spring", "square", "stadium", "stage", "stamp",
        "stand", "star", "state", "static", "status", "stella", "stereo", "stick", "sting", "stock", "stone", "stop",
        "store", "storm", "story", "strange", "street", "stretch", "strong", "stuart", "student", "studio", "style", "subject",
        "subway", "sugar", "sulfur", "sultan", "summer", "sunday", "sunset", "super", "support", "supreme", "survive", "susan",
        "sushi", "suzuki", "sweden", "sweet", "swim", "swing", "switch", "symbol", "system", "table", "taboo", "tactic",
        "tahiti", "talent", "tango", "tape", "target", "tarzan", "tavern", "taxi", "teacher", "telecom", "telex", "temple",
        "tempo", "tennis", "texas", "textile", "theory", "thermos", "think", "thomas", "tibet", "ticket", "tictac", "tiger",
        "time", "tina", "titanic", "toast", "tobacco", "today", "toga", "tokyo", "tomato", "tommy", "tonight", "topic",
        "torch", "tornado", "toronto", "torpedo", "torso", "total", "totem", "touch", "tourist", "tower", "toyota", "tractor",
        "trade", "traffic", "transit", "trapeze", "travel", "tribal", "tribune", "trick", "trident", "trilogy", "trinity", "tripod",
        "triton", "trivial", "tropic", "truck", "trumpet", "trust", "tulip", "tuna", "tunnel", "turbo", "turtle", "twin",
        "twist", "type", "ultra", "uncle", "under", "unicorn", "uniform", "union", "unique", "unit", "update", "uranium",
        "urban", "urgent", "user", "vacuum", "valery", "valid", "value", "vampire", "vanilla", "vatican", "vega", "velvet",
        "vendor", "venice", "ventura", "venus", "verona", "version", "vertigo", "veteran", "vibrate", "victor", "video", "vienna",
        "viking", "village", "vincent", "violet", "violin", "virgo", "virtual", "virus", "visa", "visible", "vision", "visitor",
        "vista", "visual", "vital", "vitamin", "viva", "vocal", "vodka", "voice", "volcano", "voltage", "volume", "voodoo",
        "vortex", "voyage", "waiter", "warning", "watch", "water", "wave", "weather", "wedding", "weekend", "welcome", "western",
        "wheel", "whiskey", "william", "window", "winter", "wisdom", "wizard", "wolf", "wonder", "world", "xray", "yankee",
        "year", "yellow", "yes", "yoga", "yogurt", "young", "yoyo", "zebra", "zero", "zigzag", "zipper", "zodiac", "zoom"
    ]

    _WORD_TO_INDEX = {word: index for index, word in enumerate(_WORDS)}



    @classmethod
    def generate_slug(cls, count: Optional[int] = None) -> str:
        if count is None:
            count = 2
        return "-".join(random.choices(cls._WORDS, k=count))

    @classmethod
    def from_number(cls, number: int):
        slug = ''
        while number:
            number, i = divmod(number, len(cls._WORDS))
            slug = cls._WORDS[i] + '-' + slug
        return slug.strip('-')

    @classmethod
    def to_number(cls, slug):
        words = slug.lower().split('-')
        number = 0
        for i in range(len(words)):
            number += cls._WORD_TO_INDEX[words[i]] * (len(cls._WORDS) ** (len(words) - i - 1))
        return number

    @classmethod
    def from_number_array(cls, numbers: list[int]):
        slugs = []
        for number in numbers:
            new_slug = cls.from_number(number)
            slugs.append(new_slug)

        return "-".join(slugs)

    @classmethod
    def to_number_array(cls, slug_in: str):
        numbers = []
        for part in slug_in.lower().split('-'):
            numbers.append(cls.to_number(part))

        return numbers

    @classmethod
    def generate_id_slug(cls, word_count: int = 2, seed: Union[int, str, None] = None) -> str:
        """
        Generate a deterministic slug based on a seed value.
        
        Args:
            word_count: Number of words to include in the slug (default: 2)
            seed: A seed value that determines the generated slug
                 - If int: Used directly as random seed
                 - If str: Hashed to create a consistent integer seed
                 - If None: Random slug is generated (non-deterministic)
                 
        Returns:
            A hyphen-separated string of words (a slug)
        """
        if seed is None:
            # No seed provided, generate a random slug
            return cls.generate_slug(word_count)
            
        # Save the current random state to restore later
        state = random.getstate()
        
        try:
            if isinstance(seed, str):
                # Convert string to a deterministic integer using hash
                # Use SHA-256 for stable cross-platform results
                hash_obj = hashlib.sha256(seed.encode('utf-8'))
                # Convert first 8 bytes of hash to integer
                seed_int = int.from_bytes(hash_obj.digest()[:8], byteorder='big')
                random.seed(seed_int)
            else:
                # Use the int seed directly
                random.seed(seed)
                
            # Generate the slug with the seeded random state
            result = "-".join(random.choices(cls._WORDS, k=word_count))
            return result
            
        finally:
            # Always restore the random state
            random.setstate(state)
            
    @classmethod
    def generate_hierarchical_id(cls, id_parts: list[tuple[Union[int, str, None], int]], delimiter: str = ":") -> str:
        """
        Generate a hierarchical ID from multiple parts.
        
        Args:
            id_parts: List of tuples, each containing (seed, word_count)
                     - seed: Same as in generate_id_slug
                     - word_count: Number of words for this part
            delimiter: Character(s) to join the parts (default: ":")
            
        Returns:
            A hierarchical ID with parts joined by the delimiter
            
        Example:
            >>> MnemonicSlugs.generate_hierarchical_id([
            ...     ("user_123", 2),  # User ID with 2 words
            ...     ("session_456", 1),  # Session ID with 1 word
            ...     ("message_789", 1)   # Message ID with 1 word
            ... ])
            'abraham-absent:acid:actor'
        """
        parts = []
        for seed, word_count in id_parts:
            parts.append(cls.generate_id_slug(word_count, seed))
        return delimiter.join(parts)
    
    @classmethod
    def parse_hierarchical_id(cls, hierarchical_id: str, delimiter: str = ":") -> list[str]:
        """
        Parse a hierarchical ID back into its component parts.
        
        Args:
            hierarchical_id: A hierarchical ID string
            delimiter: Character(s) that separate the parts (default: ":")
            
        Returns:
            List of individual slug parts
            
        Example:
            >>> MnemonicSlugs.parse_hierarchical_id('abraham-absent:acid:actor')
            ['abraham-absent', 'acid', 'actor']
        """
        return hierarchical_id.split(delimiter)

if __name__ == "__main__":
    # Example usage
    print(f"MnemonicSlugs word count: {len(MnemonicSlugs._WORDS)}")
    slug = MnemonicSlugs.generate_slug(3)
    print(f"Generated slug: {slug}")

    number = MnemonicSlugs.to_number(slug)
    print(f"Converted to number: {number}")

    slug_from_number = MnemonicSlugs.from_number(number)
    print(f"Converted back to slug: {slug_from_number}")

    assert slug == slug_from_number, "Conversion failed!"
    
    # Test deterministic slug generation
    username = "johnsmith@example.com"
    id_slug1 = MnemonicSlugs.generate_id_slug(2, username)
    id_slug2 = MnemonicSlugs.generate_id_slug(2, username)
    print(f"Deterministic slug for '{username}': {id_slug1}")
    assert id_slug1 == id_slug2, "Deterministic generation failed!"
    
    # Test hierarchical ID generation
    hierarchical_id = MnemonicSlugs.generate_hierarchical_id([
        ("user_12345", 2),        # User ID with 2 words
        ("session_456", 1),       # Session ID with 1 word
        ("message_789", 1)        # Message ID with 1 word
    ])
    print(f"Hierarchical ID: {hierarchical_id}")
    
    # Test parsing hierarchical ID
    parts = MnemonicSlugs.parse_hierarchical_id(hierarchical_id)
    print(f"Parsed parts: {parts}")
    
    # Test nesting context - using parent IDs to seed child IDs
    user_seed = "user_12345"
    user_id = MnemonicSlugs.generate_id_slug(2, user_seed)
    
    # Use the user_id as part of the session seed
    session_seed = f"session_{user_id}"
    session_id = MnemonicSlugs.generate_id_slug(1, session_seed)
    
    # Use the session_id as part of the message seed
    message_seed = f"message_{session_id}"
    message_id = MnemonicSlugs.generate_id_slug(1, message_seed)
    
    print(f"Nested context ID: {user_id}:{session_id}:{message_id}")
    
    # Demonstrate that the same seeds always produce the same slugs
    hierarchical_id2 = MnemonicSlugs.generate_hierarchical_id([
        ("user_12345", 2),
        ("session_456", 1),
        ("message_789", 1)
    ])
    assert hierarchical_id == hierarchical_id2, "Deterministic hierarchical ID generation failed!"
