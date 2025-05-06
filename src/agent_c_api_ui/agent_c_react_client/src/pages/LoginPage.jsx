import React, { useEffect, useState } from 'react';
import { PublicClientApplication } from '@azure/msal-browser';
import { msalConfig, loginRequest } from '../../authConfig';

const msalInstance = new PublicClientApplication(msalConfig);

const LoginPage = () => {
  const [user, setUser] = useState(null);
  const [msalReady, setMsalReady] = useState(false);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    const initMSAL = async () => {
      try {
        await msalInstance.initialize();
        const accounts = msalInstance.getAllAccounts();
        if (accounts.length > 0) {
          setUser(accounts[0]);
        }
        setMsalReady(true);
      } catch (error) {
        console.error('MSAL init error:', error);
      }
    };

    initMSAL();
  }, []);

  const handleLogin = async () => {
    try {
      if (!msalReady) return;
      setLoading(true); // Start loading state
      const response = await msalInstance.loginPopup(loginRequest);
      setUser(response.account);
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleLogout = () => {
    msalInstance.logoutPopup();
    setUser(null);
  };

  return (
    <div className="flex flex-col items-center justify-center h-screen space-y-4">
      <h1 className="text-3xl font-bold mb-20">Welcome back</h1>

      {user ? (
        <>
          <p>Welcome, {user.name}</p>
          <button
            onClick={handleLogout}
            className="px-4 py-2 bg-red-500 text-white rounded"
          >
            Logout
          </button>
        </>
      ) : (
        <div className="flex flex-col items-center space-y-6 w-full">
          {!loading ? (
            <button
              onClick={handleLogin}
              disabled={!msalReady}
              className="px-4 py-2 w-64 bg-blue-500 text-white rounded"
            >
              Continue with Microsoft Account
            </button>
          ) : (
            <p className="text-gray-600">Redirecting to login...</p>
          )}

          <button
            disabled
            className="px-4 py-2 w-64 bg-gray-300 text-gray-600 rounded cursor-not-allowed"
          >
            Continue with Google (coming soon)
          </button>

          <button
            disabled
            className="px-4 py-2 w-64 bg-gray-300 text-gray-600 rounded cursor-not-allowed"
          >
            Continue with Apple (coming soon)
          </button>
        </div>
      )}
    </div>
  );
};

export default LoginPage;