<script setup lang="ts">
import { ref, computed } from "vue";

interface Post {
  id: number;
  title: string;
  summary: string;
  author: string;
  createdAt: string;
}
interface Props {
  posts: Post[];
}

const props = defineProps<Props>();

const currentPage = ref<number>(1);
const pageSize = ref<number>(5);

const totalPages = computed(() =>
  Math.ceil(props.posts.length / pageSize.value),
);

const paginatedPosts = computed<Post[]>(() => {
  const start = (currentPage.value - 1) * pageSize.value;
  const end = start + pageSize.value;
  return props.posts.slice(start, end);
});

const handlePageChange = (page: number): void => {
  currentPage.value = page;
};
</script>

<template>
  <div class="post-list">
    <div v-if="paginatedPosts.length > 0">
      <div v-for="post in paginatedPosts" :key="post.id" class="post-card">
        <h3>{{ post.title }}</h3>
        <p class="post-meta">{{ post.author }} {{ post.createdAt }}</p>
        <p class="post-summary">{{ post.summary }}</p>
        <router-link :to="'/article/' + post.id" class="read-more"
          >阅读全文-></router-link
        >
      </div>
    </div>
    <div v-else class="empty">暂无文章</div>

    <div class="pagination-container" v-if="totalPages > 1">
      <!--
                el-pagination 分页组件
                background：带背景色的页码按钮
                layout：分页器布局，"prev, pager, next" 表示上一页、页码、下一页
                :total：总条数
                :page-size：每页条数
                :current-page：当前页码
                @current-change：页码切换事件
            -->
      <el-pagination
        background
        layout="prev, pager, next"
        :total="props.posts.length"
        :page-size="pageSize"
        :current-page="currentPage"
        @current-change="handlePageChange"
      />
    </div>
  </div>
</template>

<style scoped>
.post-list {
  max-width: 800px;
  margin: 0 auto;
  padding: 20px;
}

.post-card {
  background: white;
  border: 1px solid #ddd;
  border-radius: 8px;
  padding: 20px;
  margin-bottom: 15px;
  transition: box-shadow 0.3s;
}

.post-card:hover {
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1);
}

.post-card h3 {
  margin: 0 0 8px;
  font-size: 20px;
  color: #2c3e50;
}

.post-meta {
  color: #999;
  font-size: 14px;
  margin-bottom: 10px;
}

.post-summary {
  color: #555;
  line-height: 1.6;
  margin-bottom: 12px;
}

.read-more {
  color: #409eff;
  text-decoration: none;
  font-size: 14px;
}

.read-more:hover {
  color: #337ecc;
}

.empty {
  text-align: center;
  color: #999;
  padding: 40px 0;
  font-size: 16px;
}

.pagination-container {
  display: flex;
  justify-content: center;
  margin-top: 20px;
}
</style>
