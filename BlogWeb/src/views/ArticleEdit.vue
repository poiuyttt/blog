<script setup lang="ts">
import { ref, reactive, computed, onMounted } from "vue";
import { useRoute, useRouter } from "vue-router";
import { ElMessage, type FormInstance, type FormRules } from "element-plus";
import {
  createPost,
  updatePost,
  getPostById,
  getCategories,
  createCategory,
  deleteCategory,
  type Category,
} from "../api/posts";

const route = useRoute();
const router = useRouter();

// ========== 判断编辑模式还是新建模式 ==========
const postId = route.params.id ? Number(route.params.id) : null;
const isEditMode = computed<boolean>(() => postId !== null);

// ========== 表单引用 ==========
const formRef = ref<FormInstance>();

// ========== 表单数据 ==========
const form = reactive({
  title: "",
  content: "",
  summary: "",
  categoryId: null as number | null,
});

// ========== 分类数据 ==========
const categories = ref<Category[]>([]);
const newCategoryName = ref("");

// ========== 表单验证规则 ==========
const rules: FormRules = {
  title: [
    { required: true, message: "请输入文章标题", trigger: "blur" },
    {
      min: 3,
      max: 100,
      message: "标题长度在 3 到 100 个字符",
      trigger: "blur",
    },
  ],
  content: [
    { required: true, message: "请输入文章内容", trigger: "blur" },
    { min: 10, message: "内容至少需要 10 个字符", trigger: "blur" },
  ],
};

// ========== 加载分类 ==========
const loadCategories = async () => {
  try {
    const res = await getCategories();
    categories.value = res.data;
  } catch (error) {
    console.error("获取分类失败:", error);
  }
};

// ========== 创建分类 ==========
const handleCreateCategory = async () => {
  if (!newCategoryName.value.trim()) {
    ElMessage.warning("请输入分类名称");
    return;
  }

  try {
    await createCategory({
      name: newCategoryName.value,
    });
    ElMessage.success("分类创建成功");
    newCategoryName.value = "";
    await loadCategories();
  } catch (error) {
    ElMessage.error("创建分类失败");
  }
};

// ========== 删除分类 ==========
const handleDeleteCategory = async (categoryId: number) => {
  try {
    await deleteCategory(categoryId);
    ElMessage.success("分类删除成功");
    // 如果删除了当前选中的分类，清空选中
    if (form.categoryId === categoryId) {
      form.categoryId = null;
    }
    await loadCategories();
  } catch (error) {
    ElMessage.error("删除分类失败");
  }
};

// ========== 编辑模式：加载已有文章数据 ==========
onMounted(async () => {
  await loadCategories();

  if (postId) {
    try {
      const res = await getPostById(postId);
      const post = res.data;
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

// ========== 提交表单 ==========
const handleSubmit = async () => {
  if (!formRef.value) return;

  await formRef.value.validate(async (valid) => {
    if (!valid) {
      ElMessage.error("请检查表单填写");
      return;
    }

    try {
      if (isEditMode.value && postId) {
        // 编辑模式：调用 updatePost
        await updatePost(postId, {
          title: form.title,
          content: form.content,
          summary: form.summary,
          categoryId: form.categoryId || undefined,
        });
        ElMessage.success("文章已更新");
      } else {
        // 新建模式：调用 createPost
        const res = await createPost({
          title: form.title,
          content: form.content,
          summary: form.summary,
          categoryId: form.categoryId || undefined,
        });
        ElMessage.success("文章发布成功");
        // 新建成功后跳转到新文章详情页
        router.push("/article/" + res.data.id);
        return;
      }
      // 编辑成功后跳转到文章详情页
      router.push("/article/" + postId);
    } catch (err: any) {
      const msg =
        err.response?.data?.message ||
        (isEditMode.value ? "更新失败" : "发布失败");
      ElMessage.error(msg);
    }
  });
};

// ========== Markdown 预览切换 ==========
const previewMode = ref<boolean>(false);
</script>

<template>
  <div class="edit-page">
    <!-- 标题根据模式动态显示 -->
    <h2>{{ isEditMode ? "编辑文章" : "发布新文章" }}</h2>

    <el-form ref="formRef" :model="form" :rules="rules" label-width="80px">
      <el-form-item label="标题" prop="title">
        <el-input v-model="form.title" placeholder="请输入文章标题" />
      </el-form-item>

      <el-form-item label="分类">
        <div class="category-selector">
          <div class="category-list">
            <div
              v-for="category in categories"
              :key="category.id"
              class="category-item"
              :class="{ active: form.categoryId === category.id }"
              @click="form.categoryId = category.id"
            >
              <span class="category-name">
                {{ category.name }}
                <span class="category-count">({{ category.postCount }}篇)</span>
              </span>
              <el-button
                type="danger"
                size="small"
                @click.stop="handleDeleteCategory(category.id)"
                >删除</el-button
              >
            </div>
            <div
              class="category-item"
              :class="{ active: form.categoryId === null }"
              @click="form.categoryId = null"
            >
              <span class="category-name">不选分类</span>
            </div>
          </div>
          <div class="create-category">
            <el-input
              v-model="newCategoryName"
              placeholder="新分类名称"
              style="width: 200px; margin-right: 8px"
            />
            <el-button type="primary" size="small" @click="handleCreateCategory"
              >创建分类</el-button
            >
          </div>
        </div>
      </el-form-item>

      <el-form-item label="摘要" prop="summary">
        <el-input
          v-model="form.summary"
          type="textarea"
          :rows="2"
          placeholder="文章摘要（可选）"
        />
      </el-form-item>

      <el-form-item label="内容" prop="content">
        <div class="editor-container">
          <div class="editor-toolbar">
            <el-button size="small" @click="previewMode = !previewMode">
              {{ previewMode ? "编辑" : "预览" }}
            </el-button>
          </div>
          <el-input
            v-if="!previewMode"
            v-model="form.content"
            type="textarea"
            :rows="15"
            placeholder="支持 Markdown 格式"
          />
          <div v-else class="markdown-preview">
            <v-md-preview :text="form.content" />
          </div>
        </div>
      </el-form-item>

      <el-form-item>
        <el-button type="primary" @click="handleSubmit">
          {{ isEditMode ? "更新" : "发布" }}
        </el-button>
        <el-button @click="router.back()">取消</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<style scoped>
.edit-page {
  max-width: 900px;
  margin: 0 auto;
  padding: 20px;
}

.category-selector {
  width: 100%;
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.category-list {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
  margin-bottom: 8px;
}

.category-item {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 12px;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.2s;
  background: white;
}

.category-item:hover {
  border-color: #c0c4cc;
  background: #f5f7fa;
}

.category-item.active {
  border-color: #409eff;
  background: #ecf5ff;
  color: #409eff;
}

.category-name {
  font-size: 14px;
}

.category-count {
  font-size: 12px;
  color: #909399;
  margin-left: 4px;
}

.create-category {
  display: flex;
  align-items: center;
  gap: 8px;
}

.markdown-preview {
  border: 1px solid #ddd;
  padding: 15px;
  border-radius: 4px;
  min-height: 300px;
  background: white;
}
</style>
