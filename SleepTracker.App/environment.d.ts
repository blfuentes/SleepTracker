declare global {
    namespace NodeJS {
      interface ProcessEnv {
        API_URL: string;
        API_URL_HTTP: string;
        API_URL_HTTPS: string;
      }
    }
  }
  
  export {};
  