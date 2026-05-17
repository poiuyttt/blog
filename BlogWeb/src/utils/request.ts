import axios, {
  type AxiosInstance,
  type InternalAxiosRequestConfig,
  type AxiosResponse,
} from "axios";
import { ElMessage } from "element-plus";

const instance: AxiosInstance = axios.create({
  // baseURL：后端 API 基地址（根据你的后端端口修改）
  baseURL: "https://localhost:7126/api",
  timeout: 10000,
  headers: {
    "Content-Type": "application/json",
  },
});

// 请求拦截器：自动添加 Token
instance.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const token = localStorage.getItem("token");
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error),
);

// 响应拦截器：统一处理错误
instance.interceptors.response.use(
  (response: AxiosResponse) => {
    const body = response.data;
    if (body && body.success === false) {
      ElMessage.error(body.message || "操作失败");
      return Promise.reject(new Error(body.message || "操作失败"));
    }
    return body;
  },
  (error) => {
    // 网络错误或 HTTP 错误状态码
    if (error.response) {
      const status = error.response.status;
      const msg = error.response.data?.message || error.response.statusText;
      switch (status) {
        case 401:
          ElMessage.error("登录过期，请重新登录。");
          localStorage.removeItem("token");
          window.location.href = "/login";
          break;
        case 403:
          ElMessage.error("权限不足，请联系管理员。");
          break;
        case 404:
          ElMessage.error("资源不存在，请检查管理员。");
          break;
        case 500:
          ElMessage.error("服务器内部错误，请联系管理员。");
          break;
        default:
          ElMessage.error(msg || "请求失败");
          break;
      }
    } else if (error.request) {
      ElMessage.error("网络连接失败，请检查网络设置。");
    } else {
      ElMessage.error("请求配置错误");
    }
    return Promise.reject(error);
  },
);
export default instance;
