<script setup lang="ts">
import { ref, onMounted, computed, watch } from "vue";
import { useRoute, useRouter } from "vue-router";
import { getPostById, getPosts, type Post } from "../api/posts";
import CommentList from "../components/CommentList.vue";
import VMdPreview from "@kangc/v-md-editor/lib/preview";
import githubTheme from "@kangc/v-md-editor/lib/theme/github";
import lineNumberPlugin from "@kangc/v-md-editor/lib/plugins/line-number/index";
import * as hljs from "highlight.js";
import { ArrowLeft, ArrowRight } from "@element-plus/icons-vue";
import { formatDateTime } from "../utils/format";

VMdPreview.use(githubTheme, { Hljs: hljs });
VMdPreview.use(
  lineNumberPlugin.default ? lineNumberPlugin.default() : lineNumberPlugin(),
);

const route = useRoute();
const router = useRouter();
const article = ref<Post | null>(null);
const loading = ref<boolean>(false);
const allPosts = ref<Post[]>([]);
const prevPost = ref<Post | null>(null);
const nextPost = ref<Post | null>(null);

const formattedDate = computed<string>(() => {
  if (!article.value) return "";
  return formatDateTime(article.value.createdAt);
});

const loadAllPosts = async () => {
  try {
    const res = await getPosts();
    // 按创建时间降序排序
    allPosts.value = (res as any).data || res || [];
    allPosts.value.sort(
      (a, b) =>
        new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime(),
    );
  } catch (error) {
    console.error("获取文章列表失败:", error);
  }
};

const findPrevNext = (currentId: number) => {
  const idx = allPosts.value.findIndex((p) => p.id === currentId);
  if (idx === -1) {
    prevPost.value = null;
    nextPost.value = null;
    return;
  }
  prevPost.value = idx > 0 ? allPosts.value[idx - 1] : null;
  nextPost.value =
    idx < allPosts.value.length - 1 ? allPosts.value[idx + 1] : null;
};

const loadArticle = async (articleId: number) => {
  loading.value = true;

  try {
    const res = await getPostById(articleId);
    article.value = res.data;
    findPrevNext(articleId);
  } catch (error) {
    console.error("获取文章详情失败:", error);
  } finally {
    loading.value = false;
  }
};

onMounted(async () => {
  // 先加载所有文章，再加载当前文章
  await loadAllPosts();
  const articleId = Number(route.params.id);
  await loadArticle(articleId);
});

watch(
  () => route.params.id,
  async (newId) => {
    if (newId) {
      await loadArticle(Number(newId));
    }
  },
);
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
        <div class="meta-left">
          <span class="author">作者：{{ article.author }}</span>
          <span class="date">发布于：{{ formattedDate }}</span>
        </div>
        <div class="meta-right">
          <el-tag v-if="article.categoryName" type="success" size="small">
            {{ article.categoryName }}
          </el-tag>
        </div>
      </div>

      <div v-if="article.summary" class="article-summary">
        <div class="summary-box">
          <span class="summary-label">摘要</span>
          <p class="summary-text">{{ article.summary }}</p>
        </div>
      </div>

      <div class="article-body">
        <v-md-preview :text="article.content" />
      </div>

      <div class="article-footer">
        <div class="nav-buttons">
          <div class="nav-button prev" v-if="prevPost">
            <div class="nav-label">上一篇</div>
            <div
              class="nav-title"
              @click="router.push(`/article/${prevPost.id}`)"
            >
              <el-icon class="nav-icon"><ArrowLeft /></el-icon>
              {{ prevPost.title }}
            </div>
          </div>
          <div class="nav-button next" v-if="nextPost">
            <div class="nav-label">下一篇</div>
            <div
              class="nav-title"
              @click="router.push(`/article/${nextPost.id}`)"
            >
              {{ nextPost.title }}
              <el-icon class="nav-icon"><ArrowRight /></el-icon>
            </div>
          </div>
        </div>
      </div>

      <div class="comments-section">
        <h3 class="comments-title">评论</h3>
        <CommentList :post-id="article.id" />
      </div>
    </div>
  </div>
</template>

<style scoped>
.article-page {
  width: 100%;
  padding: 0;
}

.article-content {
  background: white;
  border-radius: 12px;
  padding: 40px;
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.05);
}

.article-title {
  font-size: 32px;
  font-weight: 700;
  color: #1f2937;
  margin-bottom: 24px;
  line-height: 1.4;
  word-break: break-word;
}

.article-meta {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-bottom: 20px;
  border-bottom: 1px solid #f3f4f6;
  margin-bottom: 24px;
  flex-wrap: wrap;
  gap: 12px;
}

.meta-left {
  display: flex;
  gap: 24px;
  color: #6b7280;
  font-size: 14px;
}

.meta-right {
  display: flex;
  align-items: center;
}

.article-summary {
  margin-bottom: 32px;
}

.summary-box {
  background: linear-gradient(135deg, #f8fafc 0%, #f1f5f9 100%);
  padding: 24px;
  border-radius: 10px;
  border-left: 4px solid #409eff;
}

.summary-label {
  display: inline-block;
  font-size: 12px;
  font-weight: 600;
  color: #409eff;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  margin-bottom: 8px;
}

.summary-text {
  margin: 0;
  color: #4b5563;
  line-height: 1.7;
  font-size: 15px;
}

.article-body {
  margin-bottom: 32px;
}

.article-footer {
  padding-top: 24px;
  border-top: 1px solid #f3f4f6;
  margin-bottom: 32px;
}

.nav-buttons {
  display: flex;
  justify-content: space-between;
  gap: 24px;
}

.nav-button {
  flex: 1;
}

.nav-button.next {
  text-align: right;
}

.nav-label {
  font-size: 13px;
  color: #9ca3af;
  margin-bottom: 8px;
}

.nav-title {
  font-size: 15px;
  color: #3b82f6;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  gap: 6px;
  transition: color 0.2s;
}

.nav-title:hover {
  color: #2563eb;
}

.nav-icon {
  font-size: 16px;
}

.comments-section {
  padding-top: 24px;
  border-top: 1px solid #f3f4f6;
}

.comments-title {
  font-size: 20px;
  font-weight: 600;
  color: #1f2937;
  margin-bottom: 24px;
}

.loading {
  padding: 50px 0;
}
</style>
