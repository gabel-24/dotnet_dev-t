import axios from "axios";
import {clearSession, sessionExpiredEvent} from "../context/session";

const axiosClient = axios.create({
  baseURL: "http://localhost:5182/api",
  headers: {
    "Content-Type": "application/json",
  },
});

axiosClient.interceptors.request.use((config) =>
{
  const token = localStorage.getItem("token")
  if(token)
  {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

axiosClient.interceptors.response.use(
  response => response,
  error => {
    const token = localStorage.getItem("token");
    // A late response from a previous session must not sign out a new login.
    if (error.response?.status === 401 && token &&
        error.config?.headers?.Authorization === `Bearer ${token}` &&
        !error.config?.url?.startsWith("/auth/")) {
      clearSession();
      window.dispatchEvent(new Event(sessionExpiredEvent));
    }
    return Promise.reject(error);
  }
);

export default axiosClient;