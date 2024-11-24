const baseUrl =
  process.env.API_URL_HTTP || process.env.API_URL_HTTPS || "https://localhost";

const port = process.env.PORT || "7266";

export const apiConfig = {
  baseUrl,
  port,
};
