<script setup lang="ts">
import { ref, watch } from "vue";
import { useRoute } from "vue-router";
import PostList from "../components/PostList.vue";
import { getPagedPosts, searchPosts, type Post } from "../api/posts";

const posts = ref<Post[]>([]);
const loading = ref<boolean>(false);
const currentPage = ref<number>(1);
const pageSize = ref<number>(5);
const totalCount = ref<number>(0);
const route = useRoute();

const selectedCategoryId = ref<number | null>(null);

const loadPosts = async (page: number = 1) => {
  currentPage.value = page;
  loading.value = true;
  try {
    if (selectedCategoryId.value) {
      const res = await searchPosts(
        "",
        selectedCategoryId.value || undefined,
        currentPage.value,
        pageSize.value,
      );
      posts.value = res.data.data;
      totalCount.value = res.data.totalCount;
    } else {
      const res = await getPagedPosts(currentPage.value, pageSize.value);
      posts.value = res.data.data;
      totalCount.value = res.data.totalCount;
    }
  } catch (error) {
    console.error("获取文章列表失败：", error);
  } finally {
    loading.value = false;
  }
};

// 监听路由 query 变化
watch(
  () => route.query,
  (query) => {
    if (route.path === "/") {
      const categoryId = query.category
        ? parseInt(query.category as string)
        : null;
      selectedCategoryId.value = !isNaN(categoryId!) ? categoryId : null;
      currentPage.value = 1;
      loadPosts();
    }
  },
  { immediate: true },
);
</script>

<template>
  <div class="home">
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
  </div>
</template>

<style scoped>
.home {
  width: 100%;
}

.section-title {
  border-left: 4px solid #409eff;
  padding-left: 15px;
  margin-bottom: 25px;
  margin-top: 0;
  color: #2c3e50;
}
</style>
