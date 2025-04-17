const isDevelopment = () => import.meta.env.VITE_APP_ENV === 'development';

export const hostUrl = (): string => isDevelopment()
    ? "http://localhost:5023"
    : "";

export const identityUrl = (): string => isDevelopment()
    ? "http://localhost:5791"
    : "https://identity.survivethething.com";