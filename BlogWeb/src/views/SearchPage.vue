<script setup lang="ts">
import { useRoute } from "vue-router";
import { useSearchStore } from "../stores/search";
import { ref, onMounted, computed, watch } from "vue";

// useRoute：获取当前路由对象，用于读取 URL query 参数（备用）
const route = useRoute();

// useSearchStore：获取搜索 Store 实例
const searchStore = useSearchStore();

// 从 Store 中读取关键词（持久化后的值）
const keyword = computed<string>(() => searchStore.keyword);

// ----- 搜索结果相关数据 -----
interface SearchResult {
  id: number;
  title: string;
  excerpt: string;
}

// ref：搜索结果列表（响应式数组）
const results = ref<SearchResult[]>([]);
// 是否正在加载
const loading = ref<boolean>(false);

// performSearch：执行搜索（模拟）
// 作用：根据当前关键词，从模拟数据中筛选匹配的结果
const performSearch = async (): Promise<void> => {
  if (!keyword.value.trim()) {
    results.value = [];
    return;
  }

  loading.value = true;

  // 模拟异步搜索（后续替换为 axios.get('/api/post/search', { params: { keyword } })）
  await new Promise((resolve) => setTimeout(resolve, 800));

  // 模拟文章库
  const allArticles: SearchResult[] = [
    {
      id: 1,
      title: "Vue3 组合式 API 入门",
      excerpt: "详细介绍 ref、reactive、computed、watch 的用法...",
    },
    {
      id: 2,
      title: "Vue3 组件化开发",
      excerpt: "学习如何使用 Vue3 组件化开发，构建可维护的代码...",
    },
    {
      id: 3,
      title: "Vue3 优化性能",
      excerpt: "探索 Vue3 性能优化技巧，提升应用响应速度...",
    },
    {
      id: 4,
      title: "Vue3 项目管理",
      excerpt: "使用 Vue3 项目管理工具，如 Vue CLI、Vite 等，简化项目开发...",
    },
    {
      id: 5,
      title: "Vue3 性能优化",
      excerpt: "学习 Vue3 性能优化技巧，提升应用响应速度...",
    },
  ];

  const kw = keyword.value.toLowerCase();
  // filter：筛选标题或摘要包含关键词的文章
  results.value = allArticles.filter(
    (item) =>
      item.title.toLowerCase().includes(kw) ||
      item.excerpt.toLowerCase().includes(kw),
  );

  loading.value = false;
};
// 当关键词变化时，自动重新搜索
// watch：监听 searchStore.keyword 的变化，变化后执行 performSearch
watch(
  () => searchStore.keyword,
  () => {
    performSearch();
  },
);

// onMounted：页面首次加载时执行搜索
onMounted(() => {
  performSearch();
});
</script>
<template>
  <div class="search-page">
    <h2>搜索结果:{{ keyword || "无关键词" }}</h2>
    <div class="update-keyword">
      <label>修改搜索词：</label>
      <input
        :value="searchStore.keyword"
        @input="
          searchStore.setKeyword(($event.target as HTMLInputElement).value)
        "
        placeholder="输入新关键词"
      />
    </div>
    <!-- 加载中 -->
    <div v-if="loading" class="loading">
      <!-- el-skeleton：骨架屏，加载时展示占位动画 -->
      <el-skeleton :rows="3" animated />
    </div>

    <!-- 无结果 -->
    <div v-else-if="!loading && results.length === 0 && keyword" class="empty">
      未找到与 "<strong>{{ keyword }}</strong
      >" 相关的文章。
    </div>

    <!-- 搜索结果列表 -->
    <div v-else-if="results.length > 0" class="results">
      <div v-for="item in results" :key="item.id" class="result-item">
        <h3>
          <!-- router-link：点击跳转到文章详情页 -->
          <router-link :to="'/article/' + item.id">{{
            item.title
          }}</router-link>
        </h3>
        <p class="excerpt">{{ item.excerpt }}</p>
      </div>
    </div>

    <!-- 底部操作 -->
    <div class="footer-actions">
      <p>当前页码：{{ searchStore.page }}</p>
      <div class="persist-hint">
        <span v-if="searchStore.keyword"
          >💾 搜索关键词已持久化存储，刷新页面不会丢失。</span
        >
        <span v-else>请在输入框输入关键词进行搜索。</span>
      </div>
      <button @click="searchStore.reset()" class="reset-btn">重置搜索</button>
    </div>
  </div>
</template>
<style scoped>
.search-page {
  max-width: 700px;
  margin: 0 auto;
  padding: 20px;
}

.update-keyword {
  margin: 15px 0;
}

.update-keyword input {
  padding: 6px;
  width: 300px;
  margin-left: 10px;
  border: 1px solid #ddd;
  border-radius: 4px;
}

.loading {
  padding: 20px 0;
}

.empty {
  color: #999;
  text-align: center;
  padding: 50px;
}

.results {
  margin-top: 20px;
}

.result-item {
  background: white;
  border: 1px solid #e8e8e8;
  border-radius: 8px;
  padding: 20px;
  margin-bottom: 15px;
  transition: box-shadow 0.2s;
}

.result-item:hover {
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.result-item h3 {
  margin: 0 0 10px 0;
}

.result-item a {
  color: #2c3e50;
  text-decoration: none;
}

.result-item a:hover {
  color: #409eff;
}

.excerpt {
  color: #666;
  line-height: 1.6;
  margin: 0;
}

.footer-actions {
  margin-top: 30px;
  text-align: center;
}

.persist-hint {
  color: #888;
  font-size: 14px;
  margin: 15px 0;
}

.reset-btn {
  padding: 8px 16px;
  background-color: #e74c3c;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  margin-top: 15px;
}

.reset-btn:hover {
  background-color: #c0392b;
}
</style>
