<script setup lang="ts">
import { useRoute } from "vue-router";
import { useSearchStore } from "../stores/search";
import { ref, onMounted, computed, watch } from "vue";
import { ElMessage } from "element-plus";
import { searchPosts, type Post } from "../api/posts";
import PostList from "../components/PostList.vue";

const route = useRoute();
const searchStore = useSearchStore();
const keyword = computed<string>(() => searchStore.keyword);
const posts = ref<Post[]>([]);
const loading = ref<boolean>(false);
const currentPage = ref<number>(1);
const pageSize = ref<number>(10);
const totalCount = ref<number>(0);

const performSearch = async (): Promise<void> => {
  if (!keyword.value.trim()) {
    posts.value = [];
    totalCount.value = 0;
    return;
  }

  loading.value = true;
  try {
    const res = await searchPosts(
      keyword.value,
      undefined,
      currentPage.value,
      pageSize.value,
    );
    posts.value = res.data.data;
    totalCount.value = res.data.totalCount;
    ElMessage.success(`找到 ${totalCount.value} 条结果`);
  } catch (error) {
    ElMessage.error("搜索失败");
    console.error("搜索失败:", error);
  } finally {
    loading.value = false;
  }
};

const handlePageChange = (page: number) => {
  currentPage.value = page;
  searchStore.setPage(page);
  performSearch();
};

watch(
  () => searchStore.keyword,
  () => {
    currentPage.value = 1;
    performSearch();
  },
);

onMounted(() => {
  performSearch();
});
</script>

<template>
  <div class="search-page">
    <h2>搜索结果:{{ keyword || "无关键词" }}</h2>
    <div class="update-keyword">
      <label>修改搜索词：</label>
      <el-input
        :modelValue="searchStore.keyword"
        @update:modelValue="searchStore.setKeyword"
        placeholder="输入新关键词"
        clearable
        @keyup.enter="performSearch"
        style="width: 300px; margin-left: 10px"
      >
        <template #append>
          <el-button @click="performSearch">搜索</el-button>
        </template>
      </el-input>
    </div>

    <PostList
      :posts="posts"
      :loading="loading"
      :currentPage="currentPage"
      :totalCount="totalCount"
      :pageSize="pageSize"
      @page-change="handlePageChange"
    />

    <div class="footer-actions" v-if="keyword">
      <p>共找到 {{ totalCount }} 条结果</p>
    </div>
  </div>
</template>

<style scoped>
.search-page {
  width: 100%;
  padding: 0;
}

.search-page h2 {
  margin-top: 0;
}

.update-keyword {
  margin: 20px 0;
}

.footer-actions {
  margin-top: 20px;
  text-align: center;
  color: #666;
}
</style>
