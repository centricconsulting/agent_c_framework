# How to Capture PLANE Login Request

We need to see what happens when you log into PLANE to find the authentication endpoint.

## Steps:

1. **Open PLANE in your browser:** `http://localhost/`

2. **Open DevTools:** Press F12

3. **Go to Network tab**

4. **Clear the network log** (click the 🚫 icon)

5. **Log out of PLANE** (if you're logged in)
   - Find logout button
   - Click it
   - You should be redirected to login page

6. **Now log back in:**
   - Enter your credentials
   - Click "Sign In" or "Login"
   - **Watch the Network tab!**

7. **Find the login request:**
   - Look for a POST request (usually turns red if 401/400, green if 200)
   - Common names: `sign-in`, `login`, `auth`, `session`, `token`
   - Click on it

8. **Copy the request:**
   - Right-click → Copy → Copy as cURL
   - Paste it here

## What we're looking for:

- **Request URL:** The login endpoint
- **Request Method:** Probably POST
- **Request Payload:** What data is being sent (email, password, etc.)
- **Response:** What comes back (cookies, tokens, etc.)
- **Response Cookies:** The cookies that get set after successful login

## Alternative: Check browser during fresh login

If you can't log out easily, try:
- Open in Incognito/Private window
- Go to `http://localhost/`
- Should show login page
- Follow steps above

---

## What to paste here:

1. The cURL command of the login request
2. Or just tell me:
   - Login endpoint URL (e.g., `/api/auth/sign-in/`)
   - Request method (POST)
   - What fields it needs (email? username? password?)
   - What cookies it returns
