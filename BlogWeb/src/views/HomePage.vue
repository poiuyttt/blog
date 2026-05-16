<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch } from "vue";
import { useRouter, useRoute } from "vue-router";
import { useSearchStore } from "../stores/search";
import PostList from "../components/PostList.vue";
import { getPagedPosts, type Post } from "../api/posts";

const posts = ref<Post[]>([]);
const loading = ref<boolean>(false);
const currentPage = ref<number>(1);
const pageSize = ref<number>(5);
const totalCount = ref<number>(0);
const route = useRoute();
const router = useRouter();
const searchStore = useSearchStore();
const isShow = ref(false);
const searchKeyword = ref<string>("");

// 加载中状态

function goToSearch(): void {
  if (!searchKeyword.value.trim()) {
    alert("请输入搜索关键字");
    return;
  }
  searchStore.setKeyword(searchKeyword.value);
  searchStore.setPage(1);
  router.push("/search");
}

// onMounted：页面加载后从后端获取文章列表
const loadPosts = async () => {
  loading.value = true;
  try {
    const res = await getPagedPosts(currentPage.value, pageSize.value);
    posts.value = res.data.data;
    totalCount.value = res.data.totalCount;
  } catch (error) {
    console.error("获取文章列表失败：", error);
  } finally {
    loading.value = false;
  }
};

function handleScroll(): void {
  isShow.value = window.scrollY > 300;
}

function scrollToTop(): void {
  window.scrollTo({ top: 0, behavior: "smooth" });
}

onMounted(() => {
  loadPosts();
});

onMounted(() => {
  window.addEventListener("scroll", handleScroll);
});

onUnmounted(() => {
  window.removeEventListener("scroll", handleScroll);
});

// 路由变化时重新加载数据
watch(
  () => route.path,
  () => {
    if (route.path === "/") {
      loadPosts();
    }
  },
);
</script>
<template>
  <div class="home">
    <div class="search-box">
      <input
        v-model="searchKeyword"
        placeholder="搜索文章..."
        @keyup.enter="goToSearch"
      />
      <button @click="goToSearch">搜索</button>
    </div>
  </div>

  <h2 class="section-title">最新文章</h2>
  <div v-if="loading">
    <el-skeleton :rows="5" animated />
  </div>
  <PostList
    v-else
    :posts="posts"
    :loading="loading"
    :currentPage="currentPage"
    :totalCount="totalCount"
    :pageSize="pageSize"
    @page-change="loadPosts"
  />
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
