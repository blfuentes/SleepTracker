const baseUrl =
  process.env.API_URL_HTTPS || process.env.API_URL_HTTP || "http://localhost:5104";

const port = process.env.PORT || "7266";

export const apiConfig = {
  baseUrl,
  port,
};
