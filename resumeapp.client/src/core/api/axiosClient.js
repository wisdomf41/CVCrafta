import axios from "axios";

const apiBaseUrl =
    import.meta.env.VITE_API_BASE_URL || "https://localhost:7085/api";

const axiosClient = axios.create({
    baseURL: apiBaseUrl,
    headers: {
        "Content-Type": "application/json",
    },
    timeout: 30000,
});

axiosClient.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem("resume_app_token");

        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }

        return config;
    },
    (error) => Promise.reject(error)
);

axiosClient.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error.response?.status === 401) {
            localStorage.removeItem("resume_app_token");
            localStorage.removeItem("user_email");
            localStorage.removeItem("user_name");
        }

        return Promise.reject(error);
    }
);

export default axiosClient;