<script setup lang="ts">
import { ref } from "vue";
import { useRouter } from "vue-router";
import { ElMessage } from "element-plus";
import { useAuthStore } from "../stores/auth";
import { useSearchStore } from "../stores/search";

const router = useRouter();
const authStore = useAuthStore();
const searchStore = useSearchStore();
const searchKeyword = ref<string>("");

function isLoggedIn(): boolean {
  return !!authStore.token;
}

function handleLogout() {
  authStore.clearAuth();
  router.push("/");
}

function goToSearch(): void {
  if (!searchKeyword.value.trim()) {
    ElMessage.warning("请输入搜索关键字");
    return;
  }
  searchStore.setKeyword(searchKeyword.value);
  searchStore.setPage(1);
  router.push("/search");
}
</script>

<template>
  <header class="blog-header">
    <nav class="nav">
      <router-link to="/" class="logo">我的全栈博客</router-link>
      <div class="nav-links">
        <router-link to="/">首页</router-link>
        <router-link to="/about">关于</router-link>
      </div>
      <div class="search-box">
        <el-input
          v-model="searchKeyword"
          placeholder="搜索文章..."
          clearable
          size="small"
          @keyup.enter="goToSearch"
        >
          <template #append>
            <el-button @click="goToSearch" size="small">搜索</el-button>
          </template>
        </el-input>
      </div>
      <div class="nav-links">
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
  max-width: 1200px;
  margin: 0 auto;
  display: flex;
  justify-content: space-between;
  align-items: center;
  height: 60px;
  gap: 20px;
}
.logo {
  color: white;
  font-size: 20px;
  font-weight: bold;
  text-decoration: none;
  white-space: nowrap;
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
.search-box {
  flex: 1;
  max-width: 400px;
  min-width: 200px;
}
</style>
