import { defineStore } from "pinia";
import { ref, computed } from "vue";
import type { UserProfile } from "../api/user";

export const useAuthStore = defineStore(
  "auth",
  () => {
    // ========== 状态 ==========
    const token = ref<string>(localStorage.getItem("token") || "");
    // 用户信息
    const user = ref<UserProfile | null>(null);
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
    // setUser：更新用户信息（个人资料编辑后调用）
    function setUser(userData: typeof user.value) {
      user.value = userData;
    }
    // clearAuth：清除登录信息
    function clearAuth() {
      token.value = "";
      user.value = null;
      localStorage.removeItem("token");
    }

    return { token, user, isLoggedIn, setAuth, setUser, clearAuth };
  },
  {
    persist: {
      key: "auth-store",
      pick: ["token", "user"],
    },
  },
);
