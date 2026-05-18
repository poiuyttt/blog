<script setup lang="ts">
import { ref, reactive, computed, onMounted } from "vue";
import { useRoute, useRouter } from "vue-router";
import { ElMessage } from "element-plus";
import {
  createPost,
  updatePost,
  getPostById,
  getCategories,
  createCategory,
  deleteCategory,
  type Category,
} from "../api/posts";
import { hasRole, getCurrentUsername } from "../utils/auth";
import { useAuthStore } from "../stores/auth";

const route = useRoute();
const router = useRouter();
const authStore = useAuthStore();
const isAdmin = ref<boolean>(false);
const isAuthor = ref<boolean>(false);
const canEdit = computed<boolean>(() => !isEditMode.value || isAuthor.value);

onMounted(() => {
  isAdmin.value = hasRole("Admin");
});

const postId = route.params.id ? Number(route.params.id) : null;
const isEditMode = computed<boolean>(() => postId !== null);

const form = reactive({
  title: "",
  content: "",
  summary: "",
  categoryId: null as number | null,
});

const categories = ref<Category[]>([]);
const newCategoryName = ref("");

const loadCategories = async () => {
  try {
    const res = await getCategories();
    categories.value = res.data;
  } catch (error) {
    console.error("获取分类失败:", error);
  }
};

const handleCreateCategory = async () => {
  if (!newCategoryName.value.trim()) {
    ElMessage.warning("请输入分类名称");
    return;
  }
  try {
    await createCategory({ name: newCategoryName.value });
    ElMessage.success("分类创建成功");
    newCategoryName.value = "";
    await loadCategories();
  } catch (error) {
    ElMessage.error("创建分类失败");
  }
};

const handleDeleteCategory = async (categoryId: number) => {
  try {
    await deleteCategory(categoryId);
    ElMessage.success("分类删除成功");
    if (form.categoryId === categoryId) {
      form.categoryId = null;
    }
    await loadCategories();
  } catch (error) {
    ElMessage.error("删除分类失败");
  }
};

onMounted(async () => {
  await loadCategories();
  if (postId) {
    try {
      const res = await getPostById(postId);
      const post = res.data;
      const currentUsername = getCurrentUsername();
      isAuthor.value = currentUsername === post.author;

      // 如果是编辑模式且不是作者，跳转到文章详情页
      if (!isAuthor.value) {
        ElMessage.warning("只有文章作者能编辑这篇文章");
        router.push(`/article/${postId}`);
        return;
      }

      form.title = post.title;
      form.content = post.content;
      form.summary = post.summary || "";
      form.categoryId = post.categoryId || null;
    } catch (err) {
      ElMessage.error("加载文章失败");
      router.push("/");
    }
  }
});

const handleSubmit = async () => {
  if (!form.title.trim()) {
    ElMessage.warning("请输入文章标题");
    return;
  }
  if (!form.content.trim()) {
    ElMessage.warning("请输入文章内容");
    return;
  }
  if (form.content.trim().length < 10) {
    ElMessage.warning("文章内容至少需要 10 个字符");
    return;
  }
  try {
    if (isEditMode.value && postId) {
      await updatePost(postId, {
        title: form.title,
        content: form.content,
        summary: form.summary,
        categoryId: form.categoryId || undefined,
      });
      ElMessage.success("文章已更新");
      router.push("/article/" + postId);
    } else {
      const res = await createPost({
        title: form.title,
        content: form.content,
        summary: form.summary,
        categoryId: form.categoryId || undefined,
      });
      ElMessage.success("文章发布成功");
      router.push("/article/" + res.data.id);
    }
  } catch (err: any) {
    const msg =
      err.response?.data?.message ||
      (isEditMode.value ? "更新失败" : "发布失败");
    ElMessage.error(msg);
  }
};

const handleUploadImage = async (
  event: any,
  insertImage: Function,
  files: FileList,
) => {
  const file = files[0];
  if (!file) return;

  // 验证文件类型
  const isImage = file.type.startsWith("image/");
  if (!isImage) {
    ElMessage.error("只能上传图片文件！");
    return;
  }

  // 验证文件大小
  const isLt5M = file.size / 1024 / 1024 < 5;
  if (!isLt5M) {
    ElMessage.error("图片大小不能超过 5MB！");
    return;
  }

  const formData = new FormData();
  formData.append("file", file);

  try {
    const response = await fetch("https://localhost:7126/api/file/upload", {
      method: "POST",
      headers: {
        Authorization: `Bearer ${authStore.token}`,
      },
      body: formData,
    });

    const result = await response.json();
    if (result.success) {
      // 插入图片到编辑器
      insertImage({
        url: result.data.Url,
        desc: file.name,
      });
      ElMessage.success("图片上传成功！");
    } else {
      ElMessage.error(result.message || "图片上传失败");
    }
  } catch (error) {
    ElMessage.error("图片上传失败");
  }
};
</script>

<template>
  <div class="edit-page">
    <div class="edit-header">
      <h2>{{ isEditMode ? "编辑文章" : "发布新文章" }}</h2>
      <div class="edit-actions">
        <el-button @click="router.back()">取消</el-button>
        <el-button type="primary" @click="handleSubmit">
          {{ isEditMode ? "更新" : "发布" }}
        </el-button>
      </div>
    </div>

    <div class="edit-body">
      <div class="edit-sidebar">
        <el-card class="sidebar-card">
          <template #header>
            <span>分类</span>
          </template>

          <div class="category-tags">
            <el-tag
              v-for="category in categories"
              :key="category.id"
              :type="form.categoryId === category.id ? 'primary' : 'info'"
              effect="plain"
              class="category-tag"
              @click="form.categoryId = category.id"
              closable
              @close.stop="handleDeleteCategory(category.id)"
            >
              {{ category.name }}
              <span class="tag-count">({{ category.postCount }})</span>
            </el-tag>
            <el-tag
              :type="form.categoryId === null ? 'primary' : 'info'"
              effect="plain"
              class="category-tag"
              @click="form.categoryId = null"
            >
              不选分类
            </el-tag>
            <el-popover
              placement="bottom"
              title="新建分类"
              trigger="click"
              width="220"
            >
              <template #reference>
                <el-button size="small" circle class="add-category-btn">
                  +
                </el-button>
              </template>
              <div class="popover-content">
                <el-input
                  v-model="newCategoryName"
                  placeholder="分类名称"
                  size="small"
                  @keyup.enter="handleCreateCategory"
                />
                <el-button
                  type="primary"
                  size="small"
                  style="margin-top: 8px; width: 100%"
                  @click="handleCreateCategory"
                >
                  创建
                </el-button>
              </div>
            </el-popover>
          </div>
        </el-card>

        <el-card class="sidebar-card">
          <template #header>
            <span>摘要</span>
          </template>
          <el-input
            v-model="form.summary"
            type="textarea"
            :rows="12"
            placeholder="文章摘要（可选，会显示在文章卡片上）"
          />
        </el-card>
      </div>

      <div class="edit-main">
        <div class="title-section">
          <el-input
            v-model="form.title"
            placeholder="输入文章标题..."
            class="title-input"
          />
        </div>

        <div class="editor-wrapper">
          <v-md-editor
            v-model="form.content"
            placeholder="输入文章内容..."
            height="600px"
            class="content-editor"
            @upload-image="handleUploadImage"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.edit-page {
  padding: 24px;
}

.edit-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.edit-header h2 {
  margin: 0;
  font-size: 24px;
  color: #1f2937;
}

.edit-actions {
  display: flex;
  gap: 12px;
}

.edit-body {
  display: flex;
  gap: 24px;
  align-items: flex-start;
}

.edit-main {
  flex: 1;
  min-width: 0;
}

.title-section {
  margin-bottom: 16px;
}

.title-input :deep(.el-input__inner) {
  font-size: 20px;
  font-weight: 600;
  height: 50px;
  border: none;
  padding-left: 0;
  background: transparent;
}

.title-input :deep(.el-input__inner:focus) {
  box-shadow: none;
}

.editor-wrapper {
  background: white;
  border-radius: 8px;
  box-shadow: 0 1px 4px rgba(0, 0, 0, 0.08);
  overflow: hidden;
}

.content-editor {
  border: none;
}

.edit-sidebar {
  width: 300px;
  flex-shrink: 0;
}

.sidebar-card {
  margin-bottom: 16px;
}

.category-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.category-tag {
  cursor: pointer;
}

.tag-count {
  font-size: 12px;
  opacity: 0.7;
}

.add-category-btn {
  flex-shrink: 0;
}

.popover-content {
  display: flex;
  flex-direction: column;
}
</style>
