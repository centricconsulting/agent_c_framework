from agent_c_tools.tools.rss.model import RSSToolFeed

STD_SCHEMA=['title', 'link', 'pubDate', 'description']

PP_FEED_URL = "https://www.propublica.org/feeds/propublica/main"
PP_SCHEMA = ['title', 'link', 'published', 'author']

CENTRIC_CONSULTING_FEED_URL = "https://centricconsulting.com/feed"
CENTRIC_SCHEMA = ['title', 'link', 'published', 'author', 'summary']

CNN_TOP_STORIES_URL = "http://rss.cnn.com/rss/cnn_topstories.rss"
CNN_LATEST_STORIES_URL = "http://rss.cnn.com/rss/cnn_latest.rss"
OPEN_AI_BLOG_FEED_URL = "https://openai.com/blog/rss.xml" # due to revamp fo OpenAI in May 2024, this no longer works
LA_TIMES_MAIN_FEED_URL = "https://www.latimes.com/rss2.0.xml"
MIT_NEWS_FEED_URL = "https://news.mit.edu/topic/mitartificial-intelligence2-rss.xml"
KD_NUGGETS_FEED_URL = "https://feeds.feedburner.com/kdnuggets-data-mining-analytics"
TOWARDS_AI_FEED_URL = "https://feeds.feedburner.com/towards-ai"
TOWARDS_DATA_SCIENCE_FEED_URL = "https://towardsdatascience.com/feed"
AWS_NEWS_BLOG="https://feeds.feedburner.com/AmazonWebServicesBlog"
AZURE_NEWS_BLOG="https://azure.microsoft.com/en-us/blog/feed/"


# SEEKING_ALPHA_NEWS="https://seekingalpha.com/news/trending_news"
SEEKING_ALPHA_CURRENT_MARKET_NEWS="https://seekingalpha.com/market_currents.xml"
SEEKING_ALPHA_POPULAR_ARTICLES="https://seekingalpha.com/most-popular-articles.xml"
SEEKING_ALPHA_FINANCIAL_NEWS="https://seekingalpha.com/financial.xml"
SA_SCHEMA = ['title', 'uri', 'published_on', 'id']

# markovate consulting
MARKOVATE_BLOG="https://markovate.com/feed/"
MARKOVATE_STORIES="https://markovate.com/web-stories/feed/"

# RTS Labs consulting
RTS_LABS_BLOG="https://rtslabs.com/feed"


default_feeds = [RSSToolFeed(id="propublica_news", url=PP_FEED_URL, fields_wanted=PP_SCHEMA, desc="Latest headlines from ProPublica (Investigative Journalism in the Public Interest)"),
                 RSSToolFeed(id="cnn_top_stories", url=CNN_TOP_STORIES_URL, fields_wanted=STD_SCHEMA, desc="Top stories from CNN."),
                 RSSToolFeed(id="cnn_latest", url=CNN_LATEST_STORIES_URL, fields_wanted=STD_SCHEMA, desc="Latest headlines from CNN."),
                 RSSToolFeed(id="latimes", url=LA_TIMES_MAIN_FEED_URL, fields_wanted=STD_SCHEMA, desc="Latest headlines from the LA Times."),
                 RSSToolFeed(id="openai_blog", url=OPEN_AI_BLOG_FEED_URL, fields_wanted=STD_SCHEMA, desc="Latest blog posts from Open AI."),
                 RSSToolFeed(id="mit_news", url=MIT_NEWS_FEED_URL, fields_wanted=STD_SCHEMA, desc="Latest news from MIT"),
                 RSSToolFeed(id="kd_nuggets", url=KD_NUGGETS_FEED_URL, fields_wanted=STD_SCHEMA, desc="Data Science, Machine Learning, AI & Analytics news"),
                 RSSToolFeed(id="towards_ai", url=TOWARDS_AI_FEED_URL, fields_wanted=STD_SCHEMA, desc="Making AI accessible to all"),
                 RSSToolFeed(id="towards_data_science", url=TOWARDS_DATA_SCIENCE_FEED_URL, fields_wanted=STD_SCHEMA, desc="Your home for data science news."),
                 RSSToolFeed(id="centric_consulting_blog", url=CENTRIC_CONSULTING_FEED_URL, fields_wanted=CENTRIC_SCHEMA, desc="Latest posts on the Centric Consulting blog."),
                 RSSToolFeed(id="aws_blog", url=AWS_NEWS_BLOG, fields_wanted=STD_SCHEMA, desc="Latest AWS news from Amazon"),
                 RSSToolFeed(id="azure_blog", url=AWS_NEWS_BLOG, fields_wanted=STD_SCHEMA, desc="Latest Azure news from Microsoft"),
                 RSSToolFeed(id="seeking_alpha_current_market", url=SEEKING_ALPHA_CURRENT_MARKET_NEWS, fields_wanted=SA_SCHEMA, desc="Latest Market News from Seeking Alpha"),
                 RSSToolFeed(id="seeking_alpha_popular_articles", url=SEEKING_ALPHA_POPULAR_ARTICLES, fields_wanted=SA_SCHEMA, desc="Popular Articles from Seeking Alpha"),
                 RSSToolFeed(id="seeking_alpha_financial_news", url=SEEKING_ALPHA_FINANCIAL_NEWS, fields_wanted=SA_SCHEMA, desc="Financial Industry News from Seeking Alpha"),
                 RSSToolFeed(id="markvovate_blog", url=MARKOVATE_BLOG, fields_wanted=STD_SCHEMA, desc="Blog posts frm Markovate Consulting a leader in GenAI consulting"),
                 RSSToolFeed(id="markvovate_stories", url=MARKOVATE_STORIES, fields_wanted=STD_SCHEMA, desc="Web Stories from Markovate Consulting a leader in GenAI consulting"),
                 RSSToolFeed(id="rts_labs_blog", url=RTS_LABS_BLOG, fields_wanted=STD_SCHEMA, desc="Blog posts from RTS Labs | Enterprise AI Consulting, Data Engineering & DevOps")]
