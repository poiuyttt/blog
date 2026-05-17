<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useRoute, useRouter } from "vue-router";
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
const router = useRouter();
const article = ref<Post | null>(null);
const loading = ref<boolean>(false);

onMounted(async () => {
  const articleId = Number(route.params.id);
  loading.value = true;

  // 调用 API 获取文章详情
  try {
    const res = await getPostById(articleId);
    article.value = res.data;
  } catch (error) {
    console.error("获取文章详情失败:", error);
  } finally {
    loading.value = false;
  }
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

      <div v-if="article.summary" class="article-summary">
        <el-divider />
        <div class="summary-box">
          <strong>摘要：</strong>{{ article.summary }}
        </div>
      </div>

      <el-divider />

      <!-- <v-md-preview>：Markdown 渲染组件，在 main.ts 中全局注册后可直接使用 -->
      <v-md-preview :text="article.content" />

      <el-divider />
      <!--评论组件-->
      <CommentList :post-id="article.id" />
      <div class="article-footer">
        <span class="back-link" @click="router.push('/')">← 返回首页</span>
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

.article-summary {
  margin: 15px 0;
}

.summary-box {
  background-color: #f4f3ec;
  padding: 15px 20px;
  border-radius: 8px;
  color: #555;
  line-height: 1.6;
}
.article-footer {
  margin-top: 30px;
}
.back-link {
  color: #409eff;
  text-decoration: none;
  cursor: pointer;
}
.loading {
  padding: 50px 0;
}
</style>
