import axios from "axios";

const instance = axios.create({
    baseURL: "https://qujat-temp-front-api.zonakomforta.kz/",
    headers: {
        "Content-Type": "application/json",
        Accept: "application/json",
    },
});

export default instance;