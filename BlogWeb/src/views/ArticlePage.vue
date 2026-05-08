<!-- 📄 src/views/ArticlePage.vue -->
<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useRoute } from "vue-router";
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

interface Article {
  id: number;
  title: string;
  author: string;
  createdAt: string;
  content: string;
}

const article = ref<Article | null>(null);

onMounted(() => {
  const articleId = route.params.id;
  console.log("正在加载文章，ID：", articleId);

  // 模拟文章数据（后续替换为 API 调用）
  article.value = {
    id: Number(articleId),
    title: "Vue3 组合式 API 入门",
    author: "张三",
    createdAt: "2026-05-02",
    content: `## 什么是组合式 API？

Vue3 引入的**组合式 API**（Composition API）提供了一种全新的组织组件逻辑的方式。

### 核心概念

\`\`\`typescript
// ref：创建响应式基础类型
const count = ref<number>(0);

// computed：计算派生值，有缓存
const double = computed(() => count.value * 2);
\`\`\`

### 优势

1. **更好的逻辑复用**
2. **更好的类型推导**
3. **更清晰的代码组织**
`,
  };
});

interface Comment {
  id: number;
  author: string;
  content: string;
  createdAt: string;
}

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
