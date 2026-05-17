<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch } from "vue";
import { useRouter, useRoute } from "vue-router";
import PostList from "../components/PostList.vue";
import { getPagedPosts, searchPosts, getCategories, type Post, type Category } from "../api/posts";

const posts = ref<Post[]>([]);
const loading = ref<boolean>(false);
const currentPage = ref<number>(1);
const pageSize = ref<number>(5);
const totalCount = ref<number>(0);
const route = useRoute();
const router = useRouter();
const isShow = ref(false);

const categories = ref<Category[]>([]);
const selectedCategoryId = ref<number | null>(null);

const loadCategories = async () => {
  try {
    const res = await getCategories();
    categories.value = res.data;
  } catch (error) {
    console.error("获取分类失败:", error);
  }
};

const loadPosts = async (page: number = 1) => {
  currentPage.value = page;
  loading.value = true;
  try {
    if (selectedCategoryId.value) {
      const res = await searchPosts(
        "",
        selectedCategoryId.value || undefined,
        currentPage.value,
        pageSize.value
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

const handleCategoryClick = (categoryId: number | null) => {
  selectedCategoryId.value = categoryId;
  currentPage.value = 1;
  loadPosts();
};

const clearFilters = () => {
  selectedCategoryId.value = null;
  loadPosts();
};

function handleScroll(): void {
  isShow.value = window.scrollY > 300;
}

function scrollToTop(): void {
  window.scrollTo({ top: 0, behavior: "smooth" });
}

onMounted(() => {
  loadCategories();
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
    <div class="sidebar">
      <div class="sidebar-section">
        <h3>分类</h3>
        <div class="category-list">
          <div
            class="category-item"
            :class="{ active: selectedCategoryId === null }"
            @click="handleCategoryClick(null)"
          >
            全部
          </div>
          <div
            v-for="category in categories"
            :key="category.id"
            class="category-item"
            :class="{ active: selectedCategoryId === category.id }"
            @click="handleCategoryClick(category.id)"
          >
            {{ category.name }}
            <span class="count">({{ category.postCount }})</span>
          </div>
        </div>
      </div>

      <el-button
        v-if="selectedCategoryId"
        type="primary"
        plain
        size="small"
        @click="clearFilters"
        style="margin-top: 10px;"
      >
        清除筛选
      </el-button>
    </div>

    <div class="main-content">
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

    <div v-if="isShow" class="back-to-top">
      <el-button type="primary" @click="scrollToTop">↑ 返回顶部</el-button>
    </div>
  </div>
</template>

<style scoped>
.home {
  display: flex;
  gap: 20px;
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px;
}

.sidebar {
  width: 250px;
  flex-shrink: 0;
}

.sidebar-section {
  background: #f5f5f5;
  padding: 15px;
  border-radius: 8px;
  margin-bottom: 20px;
}

.sidebar-section h3 {
  margin: 0 0 10px 0;
  font-size: 16px;
}

.category-list {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.category-item {
  padding: 8px 12px;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.3s;
}

.category-item:hover {
  background: #e0e0e0;
}

.category-item.active {
  background: #409eff;
  color: white;
}

.category-item .count {
  color: #999;
  font-size: 12px;
}

.category-item.active .count {
  color: #fff;
}

.main-content {
  flex: 1;
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
