export const msalConfig = {
  auth: {
    clientId: "REPLACE_ME_CLIENT_ID",
    authority: "https://login.microsoftonline.com/REPLACE_TENANT_ID",
    redirectUri: "/"
  },
  cache: { cacheLocation: 'localStorage' }
};

export const loginRequest = {
  scopes: ["api://REPLACE_API_APP_ID/.default"]
};