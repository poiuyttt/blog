<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useRoute } from "vue-router";
import { getPostById, type Post } from "../api/posts";
import CommentList from "../components/CommentList.vue";
import VMdPreview from "@kangc/v-md-editor/lib/preview";
import githubTheme from "@kangc/v-md-editor/lib/theme/github";
import lineNumberPlugin from "@kangc/v-md-editor/lib/plugins/line-number/index";
import * as hljs from "highlight.js";

VMdPreview.use(githubTheme, { Hljs: hljs });
VMdPreview.use(
  lineNumberPlugin.default ? lineNumberPlugin.default() : lineNumberPlugin(),
);

const route = useRoute();
const article = ref<Post | null>(null);
const loading = ref<boolean>(false);

const comments = ref<Comment[]>([]);
const commentsLoading = ref<boolean>(false);

const handleRefreshComments = async () => {
  commentsLoading.value = true;
  //模拟异步加载评论
  setTimeout(() => {
    comments.value = [
      {
        id: 1,
        author: "张三",
        content: "这是一条评论",
        createdAt: "2026-05-02",
      },
      {
        id: 2,
        author: "李四",
        content: "这是另一条评论",
        createdAt: "2026-05-02",
      },
    ];
    commentsLoading.value = false;
  }, 500);
};

const handleDeleteComment = (id: number) => {
  comments.value = comments.value.filter((comment) => comment.id !== id);
  console.log("删除评论ID:", id);
};

onMounted(async () => {
  const articleId = Number(route.params.id);
  loading.value = true;

  // 调用 API 获取文章详情
  try {
    const data = await getPostById(articleId);
    article.value = data;
  } catch (error) {
    console.error("获取文章详情失败:", error);
  } finally {
    loading.value = false;
  }
});

interface Comment {
  id: number;
  author: string;
  content: string;
  createdAt: string;
}

onMounted(() => {
  //原有模拟文章数据加载
  handleRefreshComments();
});
</script>

<template>
  <div class="article-page">
    <div v-if="!article" class="loading">
      <!-- el-skeleton：骨架屏组件，数据加载时展示占位效果 -->
      <el-skeleton :rows="10" animated />
    </div>

    <div v-else class="article-content">
      <h1 class="article-title">{{ article.title }}</h1>
      <div class="article-meta">
        <span>作者：{{ article.author }}</span>
        <span>发布于：{{ article.createdAt }}</span>
      </div>

      <el-divider />

      <!-- <v-md-preview>：Markdown 渲染组件，在 main.ts 中全局注册后可直接使用 -->
      <v-md-preview :text="article.content" />

      <el-divider />
      <!--评论组件-->
      <CommentList
        :comments="comments"
        :loading="commentsLoading"
        @refresh="handleRefreshComments"
        @delete="handleDeleteComment"
      />
      <div class="article-footer">
        <router-link to="/" class="back-link">← 返回首页</router-link>
      </div>
    </div>
  </div>
</template>

<style scoped>
.article-page {
  max-width: 800px;
  margin: 0 auto;
  padding: 20px;
}
.article-title {
  font-size: 28px;
  color: #2c3e50;
  margin-bottom: 15px;
}
.article-meta {
  color: #999;
  font-size: 14px;
  display: flex;
  gap: 20px;
  justify-content: center;
}
.article-footer {
  margin-top: 30px;
}
.back-link {
  color: #409eff;
  text-decoration: none;
}
.loading {
  padding: 50px 0;
}
</style>
