import axios from "axios";
import API from "./axios";

const headers = {
  Method: "GET",
  Accept: "application/json",
  Authorization:
    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiIxIiwibmJmIjoxNzAxNzkyOTM2LCJleHAiOjE3MDQ0NzEzMzYsImlhdCI6MTcwMTc5MjkzNn0.rrhkSbV64fXllvoj8lX64fzaWT-1KljrlrIkNjmOBjA",
};


const api = {
  login: async (data) => {
    try {
      const response = await API.post("/api/1/identity/sign-in", {
        method: "POST",
        headers: {
          Accept: "application/json",
          "Content-Type": "application/json",
        },
        body: JSON.stringify(data),
      });
      console.log(response);
    } catch (e) {
      console.error("Error:", error);
    }
  },
  getCategory: async () => {
    try {
      const response = await API.get("/api/1/categories", { headers });
      return response.data;
    } catch (error) {
      console.error("Error:", error); 
    }
  },
};

export default api;
