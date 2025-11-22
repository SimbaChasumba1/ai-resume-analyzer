import axios from "axios";

const api = axios.create({
  baseURL: "http://localhost:5240", 
});

export default api;
