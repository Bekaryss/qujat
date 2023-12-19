import axios from "axios";

const instance = axios.create({
  baseURL: "https://qujat-temp-backoffice-api.zonakomforta.kz",
  headers: {
    "Content-Type": "application/json-patch+json",
    Accept: "application/json",
  },
});

instance.interceptors.request.use((config) => {
  config.headers.Authorization = window.localStorage.getItem("token");
  return config;
});

export default instance;
