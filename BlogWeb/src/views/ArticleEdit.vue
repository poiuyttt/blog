<script setup lang="ts">
import { ref, reactive, computed, onMounted } from "vue";
import { useRoute, useRouter } from "vue-router";
import { ElMessage, type FormInstance, type FormRules } from "element-plus";
import { createPost, updatePost, getPostById } from "../api/posts";

const route = useRoute();
const router = useRouter();

// ========== 判断编辑模式还是新建模式 ==========
// 如果路由中有 id 参数，就是编辑模式；否则是新建模式
const postId = route.params.id ? Number(route.params.id) : null;
const isEditMode = computed<boolean>(() => postId !== null);

// ========== 表单引用 ==========
const formRef = ref<FormInstance>();

// ========== 表单数据 ==========
const form = reactive({
  title: "",
  content: "",
  summary: "",
});

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

// ========== 编辑模式：加载已有文章数据 ==========
onMounted(async () => {
  if (postId) {
    try {
      const res = await getPostById(postId);
      const post = res.data;
      form.title = post.title;
      form.content = post.content;
      form.summary = post.summary || "";
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
        });
        ElMessage.success("文章已更新");
      } else {
        // 新建模式：调用 createPost
        const res = await createPost({
          title: form.title,
          content: form.content,
          summary: form.summary,
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
        <!-- 按钮文字根据模式动态显示 -->
        <el-button type="primary" @click="handleSubmit">
          {{ isEditMode ? "更新" : "发布" }}
        </el-button>
        <el-button @click="router.back()">取消</el-button>
      </el-form-item>
    </el-form>
  </div>
  <div class="edit-page">
    <el-divider />
    <h3>测试文件上传</h3>
    <FileUpload />
  </div>
</template>
<style scoped>
.edit-page {
  max-width: 900px;
  margin: 0 auto;
  padding: 20px;
}
.markdown-preview {
  border: 1px solid #ddd;
  padding: 15px;
  border-radius: 4px;
  min-height: 300px;
  background: white;
}
</style>
