import { defineStore } from "pinia";
import { ref, computed } from "vue";

export const useAuthStore = defineStore(
  "auth",
  () => {
    // ========== 状态 ==========
    const token = ref<string>(localStorage.getItem("token") || "");
    const user = ref<{
      id: number;
      username: string;
      email: string;
      avatar: string;
    } | null>(null);
    // ========== 计算属性 ==========
    // computed：是否已登录
    const isLoggedIn = computed<boolean>(() => !!token.value);
    // ========== 方法 ==========
    // setAuth：保存登录信息
    function setAuth(newToken: string, userData: typeof user.value) {
      token.value = newToken;
      user.value = userData;
      localStorage.setItem("token", newToken);
    }
    // clearAuth：清除登录信息
    function clearAuth() {
      token.value = "";
      user.value = null;
      localStorage.removeItem("token");
    }

    return { token, user, isLoggedIn, setAuth, clearAuth };
  },
  {
    persist: {
      key: "auth-store",
      paths: ["token", "user"],
    },
  },
);
