<script setup lang="ts">
import { computed } from "vue";
import { formatDateTime } from "../utils/format";

interface Post {
  id: number;
  title: string;
  summary?: string;
  author: string;
  createdAt: string;
  commentCount: number;
  categoryName?: string;
}

interface Props {
  posts: Post[];
  loading?: boolean;
  currentPage: number;
  totalCount: number;
  pageSize: number;
}

const props = withDefaults(defineProps<Props>(), {
  loading: false,
});

const emit = defineEmits<{
  (event: "page-change", page: number): void;
}>();

const totalPages = computed<number>(() =>
  Math.ceil(props.totalCount / props.pageSize),
);

const formatPostDate = (dateStr: string): string => {
  return formatDateTime(dateStr);
};
</script>

<template>
  <div class="post-list">
    <div v-if="loading">
      <el-skeleton :rows="5" animated />
    </div>
    <div v-else>
      <div v-if="totalCount === 0" class="empty">暂无文章</div>
      <div v-for="post in posts" :key="post.id" class="post-card">
        <h3>
          <router-link :to="'/article/' + post.id">{{
            post.title
          }}</router-link>
        </h3>
        <p class="post-meta">
          {{ post.author }} · {{ formatPostDate(post.createdAt) }} ·
          {{ post.commentCount }} 条评论
        </p>
        <div v-if="post.categoryName" class="post-categories">
          <el-tag type="success" size="small">
            {{ post.categoryName }}
          </el-tag>
        </div>
        <p class="post-summary">{{ post.summary || "暂无摘要" }}</p>
      </div>
    </div>
    <div class="pagination-container" v-if="totalPages > 1">
      <el-pagination
        background
        layout="prev, pager, next"
        :total="totalCount"
        :page-size="pageSize"
        :current-page="currentPage"
        @current-change="(page: number) => emit('page-change', page)"
      />
    </div>
  </div>
</template>

<style scoped>
.post-list {
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

.post-categories {
  margin-bottom: 10px;
}

.post-summary {
  color: #555;
  line-height: 1.6;
  margin-bottom: 12px;
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
