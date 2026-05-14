<script setup lang="ts">
import { useRouter } from "vue-router";
import { useAuthStore } from "../stores/auth";

const router = useRouter();
const authStore = useAuthStore();

function isLoggedIn(): boolean {
  return !!authStore.token;
}

function handleLogout() {
  authStore.clearAuth();
  router.push("/");
}
</script>

<template>
  <header class="blog-header">
    <nav class="nav">
      <router-link to="/" class="logo">我的全栈博客</router-link>
      <div class="nav-links">
        <router-link to="/">首页</router-link>
        <router-link to="/about">关于</router-link>
        <router-link v-if="!authStore.isLoggedIn" to="/login">登录</router-link>
        <router-link v-if="!authStore.isLoggedIn" to="/register"
          >注册</router-link
        >
        <a
          v-if="authStore.isLoggedIn"
          @click="handleLogout"
          style="cursor: pointer"
          >退出</a
        >
      </div>
    </nav>
  </header>
</template>

<style scoped>
.blog-header {
  background-color: #2d80d3;
  color: white;
  padding: 0 20px;
}
.nav {
  max-width: 1000px;
  margin: 0 auto;
  display: flex;
  justify-content: space-between;
  align-items: center;
  height: 60px;
}
.logo {
  color: white;
  font-size: 20px;
  font-weight: bold;
  text-decoration: none;
}
.nav-links {
  display: flex;
  gap: 20px;
}
.nav-links a {
  color: white;
  text-decoration: none;
  opacity: 0.8;
  transition: opacity 0.2s;
}
.nav-links a:hover {
  opacity: 1;
}
</style>
