# Setup
1) Set up a Google Cloud Project https://console.cloud.google.com/projectcreate
2) Enable the Gmail API 
3) Create OAuth 2.0 credentials 
    - Click Create Credentials
    - Select OAuth client ID
    - Configure consent screen
      - External Users
        - AppName - Gmail
        - Support Email - your email
        - Developer Email - your email
        - Save and Continue
    - Go back to Credentials https://console.cloud.google.com/apis/credentials (select your project)
      - Click Create Credentials
      - Application Type: Desktop
      - Name: Gmail Agent_C
      - You'll get an 'OAuth Client Created' screen
        - Download the json file and save to project root [config/](config/) folder
          - Windows: [../../../../../../config/gmail.json](../../../../../../config/gmail.json)
5) Update .env file with location of gmail.json credentials file
6) Enable GMAIL API - https://console.cloud.google.com/apis/library/gmail.googleapis.com
   - search for gmail
   - `Enable` Gmail API
7) Go back to OAuth consent screen and add yourself as test user - https://console.cloud.google.com/apis/credentials/consent
   - For production apps, you'd publish from here with appropriate domain restrictions that we have not added yet!
   - If not a production app, you will get a series of warning notices from google that you have to accept in order to authenticate
```dotenv
GOOGLE_CREDENTIALS_FILE='gmail.json'
```
# TODO's
This is a work in progress. More thought and work needs to be put into the following areas:
- [ ] Always stripping emails to text, never html
- [ ] Always stripping emails of attachments and saving.  Return email text and UNC path to all attachments, only reading when asked
- [ ] Always stripping emails of images and saving.  Return email text and UNC path to all images, only reading when asked
- [ ] Better token limiting of results
