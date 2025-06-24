# Get GoogleAPIs Key Setup Instructions
1) create project https://console.cloud.google.com/projectcreate
    - will have a name and be assigned a project ID, wait for it to be created
2) go to project and create API Key https://console.cloud.google.com/apis/credentials?project=zeta-axiom-439718-f0
3) goto console library https://console.cloud.google.com/apis/library and select your project
4) Search for Youtube Data API v3, and click enable
5) Create quotas/alerts to avoid abuse

# Testing before using
* Create a scratch file
```python
import requests
comments = []
page_token = ""
base_url = "https://www.googleapis.com/youtube/v3/commentThreads"
key='xxxxxxx'
max_comments = 100

while len(comments) < max_comments:
    params = {
        "key": key,
        "part": "snippet",
        "videoId": 'ffbcU3RLml4',
        "maxResults": min(100, max_comments - len(comments)),
        "pageToken": page_token,
        "textFormat": "plainText",
    }

    response = requests.get(base_url, params=params)
    if response.status_code == 200:
        data = response.json()
        for item in data.get("items", []):
            comment_data = item["snippet"]["topLevelComment"]["snippet"]
            comments.append({
                "text": comment_data["textDisplay"],
                "author": comment_data["authorDisplayName"],
                "likes": comment_data["likeCount"],
                "published_at": comment_data["publishedAt"]
            })

        page_token = data.get("nextPageToken")
        if not page_token or len(comments) >= max_comments:
            break
    else:
       pass

print(comments)

```