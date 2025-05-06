export const msalConfig = {
  auth: {
    clientId: '',
    authority: '',
    redirectUri: '',
  },
  cache: {
    cacheLocation: 'localStorage',
    storeAuthStateInCookie: false,
  },
};

export const loginRequest = {
  scopes: ['User.Read'],
};