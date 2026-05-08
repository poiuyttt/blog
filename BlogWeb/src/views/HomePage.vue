<script setup lang="ts">
import { ref, onMounted, onUnmounted } from "vue";
import { useRouter } from "vue-router";
import { useSearchStore } from "../stores/search";
import PostList from "../components/PostList.vue";

interface Post {
  id: number;
  title: string;
  summary: string;
  author: string;
  createdAt: string;
}

const isShow = ref(false);

const posts = ref<Post[]>([]);

const router = useRouter();
const searchStore = useSearchStore();

const searchKeyword = ref<string>("");

function goToSearch(): void {
  if (searchKeyword.value.trim() === "") {
    alert("请输入搜索关键字");
    return;
  }
  searchStore.setKeyword(searchKeyword.value);
  searchStore.setPage(1);
  router.push("/search");
}

function handleScroll(): void {
  isShow.value = window.scrollY > 300;
}

function scrollToTop(): void {
  window.scrollTo({ top: 0, behavior: "smooth" });
}

onMounted(() => {
  window.addEventListener("scroll", handleScroll);
});

onUnmounted(() => {
  window.removeEventListener("scroll", handleScroll);
});

onMounted(() => {
  posts.value = [
    {
      id: 1,
      title: "开始学习全栈开发",
      summary: "记录我从 C# 开发者转向全栈的学习历程，分享第一周的学习心得。",
      author: "张三",
      createdAt: "2026-05-01",
    },
    {
      id: 2,
      title: "Vue3 组合式 API 入门",
      summary:
        "详解 ref、reactive、computed、watch 等核心 API 的用法和最佳实践。",
      author: "张三",
      createdAt: "2026-05-02",
    },
    {
      id: 3,
      title: "ASP.NET Core Web API 搭建",
      summary: "从零创建 Web API 项目，配置 Swagger，实现第一个 Controller。",
      author: "张三",
      createdAt: "2026-05-03",
    },
    {
      id: 4,
      title: "Element Plus 组件库实践",
      summary:
        "使用 Element Plus 快速构建后台管理界面，表单验证和表格组件的使用技巧。",
      author: "张三",
      createdAt: "2026-05-04",
    },
    {
      id: 5,
      title: "依赖注入与配置管理",
      summary: "深入理解 ASP.NET Core 的 DI 容器和多种配置读取方式。",
      author: "张三",
      createdAt: "2026-05-05",
    },
  ];
});
</script>
<template>
  <div>
    <h2>首页</h2>
    <div class="search-box">
      <input
        v-model="searchKeyword"
        placeholder="搜索文章..."
        @keyup.enter="goToSearch"
      />
      <button @click="goToSearch">搜索</button>
    </div>
    <p>欢迎来到我的全栈博客！</p>
  </div>
  <div class="home">
    <h2 class="section-title">最新文章</h2>
    <PostList :posts="posts" />
  </div>
  <div v-if="isShow" class="back-to-top">
    <el-button type="primary" @click="scrollToTop">↑ 返回顶部</el-button>
  </div>
</template>
<style scoped>
button {
  padding: 8px 20px;
  font-size: 16px;
  cursor: pointer;
}

.search-box {
  margin-bottom: 20px;
}

.search-box input {
  padding: 8px;
  width: 250px;
  margin-right: 10px;
}

.home {
  max-width: 800px;
  margin: 0 auto;
  padding: 20px;
}

.section-title {
  border-left: 4px solid #409eff;
  padding-left: 15px;
  margin-bottom: 25px;
  color: #2c3e50;
}

.back-to-top {
  position: fixed;
  bottom: 40px;
  right: 40px;
  z-index: 1000;
}
</style>
