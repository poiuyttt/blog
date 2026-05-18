<script setup lang="ts">
import { ref, onMounted, computed, watch, nextTick, onUnmounted } from "vue";
import { useRoute, useRouter } from "vue-router";
import { ElMessage, ElMessageBox } from "element-plus";
import { getPostById, getPosts, deletePost, type Post } from "../api/posts";
import CommentList from "../components/CommentList.vue";
import VMdPreview from "@kangc/v-md-editor/lib/preview";
import githubTheme from "@kangc/v-md-editor/lib/theme/github";
import lineNumberPlugin from "@kangc/v-md-editor/lib/plugins/line-number/index";
import * as hljs from "highlight.js";
import { ArrowLeft, ArrowRight } from "@element-plus/icons-vue";
import { formatDateTime } from "../utils/format";
import { hasRole, getCurrentUsername } from "../utils/auth";
import { useAuthStore } from "../stores/auth";

VMdPreview.use(githubTheme, { Hljs: hljs });
VMdPreview.use(
  lineNumberPlugin.default ? lineNumberPlugin.default() : lineNumberPlugin(),
);

const route = useRoute();
const router = useRouter();
const authStore = useAuthStore();
const article = ref<Post | null>(null);
const loading = ref<boolean>(false);
const allPosts = ref<Post[]>([]);
const prevPost = ref<Post | null>(null);
const nextPost = ref<Post | null>(null);
const isAdmin = ref<boolean>(false);
const isAuthor = computed<boolean>(() => {
  if (!article.value) return false;
  const currentUser = getCurrentUsername();
  return currentUser === article.value.author;
});
const canDelete = computed<boolean>(() => isAdmin.value || isAuthor.value);
const canEdit = computed<boolean>(() => isAuthor.value);

interface TocItem {
  id: string;
  level: number;
  text: string;
}

const toc = ref<TocItem[]>([]);
const activeTocId = ref<string>("");

const parseToc = (content: string): TocItem[] => {
  const items: TocItem[] = [];
  const headingRegex = /^(#{1,6})\s+(.+)$/gm;
  let match;

  while ((match = headingRegex.exec(content)) !== null) {
    const level = match[1].length;
    const text = match[2];
    const id = `heading-${Date.now()}-${items.length}`;
    items.push({ id, level, text });
  }

  return items;
};

const scrollToHeading = (id: string) => {
  const element = document.getElementById(id);
  if (element) {
    element.scrollIntoView({ behavior: "smooth", block: "start" });
  }
};

const handleScroll = () => {
  if (toc.value.length === 0) return;

  const scrollPosition = window.scrollY + 100;
  let currentActiveId = toc.value[0].id;

  for (let i = toc.value.length - 1; i >= 0; i--) {
    const element = document.getElementById(toc.value[i].id);
    if (element && element.offsetTop <= scrollPosition) {
      currentActiveId = toc.value[i].id;
      break;
    }
  }

  activeTocId.value = currentActiveId;
};

onMounted(async () => {
  isAdmin.value = hasRole("Admin");
  // 先加载所有文章，再加载当前文章
  await loadAllPosts();
  const articleId = Number(route.params.id);
  await loadArticle(articleId);

  // 添加滚动监听
  window.addEventListener("scroll", handleScroll);
});

onUnmounted(() => {
  // 移除滚动监听
  window.removeEventListener("scroll", handleScroll);
});

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

    // 解析目录
    toc.value = parseToc(article.value.content);

    // 等待DOM渲染完成后给标题添加id
    nextTick(() => {
      const headings = document.querySelectorAll(
        ".article-body h1, .article-body h2, .article-body h3, .article-body h4, .article-body h5, .article-body h6",
      );
      headings.forEach((heading, index) => {
        if (toc.value[index]) {
          heading.id = toc.value[index].id;
        }
      });

      // 设置第一个为激活
      if (toc.value.length > 0) {
        activeTocId.value = toc.value[0].id;
      }
    });
  } catch (error) {
    console.error("获取文章详情失败:", error);
  } finally {
    loading.value = false;
  }
};

const handleDeletePost = async () => {
  if (!article.value) return;

  try {
    await ElMessageBox.confirm(
      "确定要删除这篇文章吗？删除后无法恢复。",
      "删除确认",
      {
        confirmButtonText: "删除",
        cancelButtonText: "取消",
        type: "warning",
      },
    );
    await deletePost(article.value.id);
    ElMessage.success("文章已删除");
    router.push("/");
  } catch (err: any) {
    if (err !== "cancel") {
      const msg = err.response?.data?.message || "删除失败";
      ElMessage.error(msg);
    }
  }
};

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

    <div v-else class="article-layout">
      <div class="article-content">
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
            <el-button
              v-if="canEdit"
              type="primary"
              size="small"
              @click="router.push(`/article/${article.id}/edit`)"
              style="margin-left: 10px"
            >
              编辑
            </el-button>
            <el-button
              v-if="canDelete"
              type="danger"
              size="small"
              @click="handleDeletePost"
              style="margin-left: 10px"
            >
              删除
            </el-button>
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
          <CommentList :post-id="article.id" :post-author="article.author" />
        </div>
      </div>

      <!-- 目录导航 -->
      <div v-if="toc.length > 0" class="article-toc">
        <div class="toc-sticky">
          <h4 class="toc-title">目录</h4>
          <div class="toc-list">
            <div
              v-for="item in toc"
              :key="item.id"
              class="toc-item"
              :class="{ active: activeTocId === item.id }"
              :style="{ paddingLeft: `${(item.level - 1) * 12}px` }"
              @click="scrollToHeading(item.id)"
            >
              {{ item.text }}
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.article-page {
}

.article-layout {
  display: flex;
  gap: 32px;
  max-width: 1200px;
  margin: 0 auto;
}

.article-content {
  flex: 1;
  min-width: 0;
  background: white;
  border-radius: 12px;
  padding: 40px;
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.05);
}

.article-toc {
  flex-shrink: 0;
  width: 240px;
}

.toc-sticky {
  position: sticky;
  top: 20px;
  background: white;
  border-radius: 8px;
  padding: 20px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
  max-height: calc(100vh - 40px);
  overflow-y: auto;
}

.toc-title {
  font-size: 16px;
  font-weight: 600;
  color: #1f2937;
  margin: 0 0 16px 0;
  padding-bottom: 12px;
  border-bottom: 1px solid #f3f4f6;
}

.toc-list {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.toc-item {
  font-size: 14px;
  color: #6b7280;
  cursor: pointer;
  padding: 6px 8px;
  border-radius: 4px;
  transition: all 0.2s;
  line-height: 1.5;
  word-break: break-word;
}

.toc-item:hover {
  color: #3b82f6;
  background: #f8fafc;
}

.toc-item.active {
  color: #3b82f6;
  font-weight: 500;
  background: #eff6ff;
  border-left: 3px solid #3b82f6;
  padding-left: 5px;
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
