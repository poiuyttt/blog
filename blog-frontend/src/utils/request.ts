import axios, {
  type AxiosInstance,
  type AxiosRequestConfig,
  type AxiosResponse,
} from "axios";

const instance: AxiosInstance = axios.create({
  baseURL: "http://localhost:5000/api",
  timeout: 10000,
  headers: {
    "Content-Type": "application/json",
  },
});

instance.interceptors.request.use(
  (config: AxiosRequestConfig) => {
    const token = localStorage.getItem("token");
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    console.log("发送请求：", config.method?.toUpperCase(), config.url);
    return config;
  },
  (error) => {
    return Promise.reject(error);
  },
);

instance.interceptors.response.use(
  (response: AxiosResponse) => {
    const data = response.data;
    console.log("收到响应：", data);
    return data;
  },
  (error) => {
    if (error.response) {
      const status = error.response.status;
      if (status === 401) {
        localStorage.removeItem("token");
        alert("登录过期，请重新登录。");
      } else if (status === 404) {
        console.log("请求的资源不存在");
      } else {
        console.log(`服务器错误：${status}`);
      }
    } else if (error.request) {
      console.log("网络错误，无法连接到服务器");
    } else {
      console.log("请求配置错误：", error.message);
    }
    return Promise.reject(error);
  },
);
export default instance;
