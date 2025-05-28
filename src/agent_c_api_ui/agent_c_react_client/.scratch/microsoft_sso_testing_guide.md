# Microsoft SSO Integration Testing Guide

## Prerequisites
1. Ensure all required dependencies are installed:
   ```bash
   npm install @azure/msal-browser @azure/msal-react --save
   ```

## Testing Steps

1. **Start the application**:
   ```bash
   npm run dev
   ```

2. **Verify Login Flow**:
   - Navigate to the application URL (typically http://localhost:3000)
   - You should be automatically redirected to the login page
   - Click the "Continue with Microsoft Account" button
   - A Microsoft login popup should appear
   - After successful login, you should be redirected to the home page

3. **Verify Protected Routes**:
   - Try accessing various routes directly (e.g., /chat, /settings)
   - If not logged in, you should be redirected to the login page
   - After login, you should be taken to the originally requested page

4. **Verify Logout**:
   - Add a logout button to the UI if needed
   - Click logout and verify you're redirected to the login page
   - Verify that protected routes are no longer accessible

## Troubleshooting

- Check browser console for any errors
- Verify the correct clientId and authority are set in authConfig.js
- Make sure the redirectUri matches your application's URL
- If using a development environment, make sure the redirect URI is registered in the Azure portal