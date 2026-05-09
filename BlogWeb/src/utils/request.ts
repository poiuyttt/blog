import axios, {
  type AxiosInstance,
  type InternalAxiosRequestConfig,
  type AxiosResponse,
} from "axios";

const instance: AxiosInstance = axios.create({
  // baseURL：后端 API 基地址（根据你的后端端口修改）
  baseURL: "https://localhost:7126/api",
  timeout: 10000,
  headers: {
    "Content-Type": "application/json",
  },
});

// 请求拦截器：自动添加 Token（如有）
instance.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const token = localStorage.getItem("token");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    console.log("发送请求：", config.method?.toUpperCase(), config.url);
    return config;
  },
  (error) => Promise.reject(error),
);

// 响应拦截器：统一处理错误
instance.interceptors.response.use(
  (response: AxiosResponse) => response.data,
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
