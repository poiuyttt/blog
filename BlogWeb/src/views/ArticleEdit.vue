<script setup lang="ts">
import { ref, reactive, onMounted } from "vue";
import { useRoute, useRouter } from "vue-router";
import { ElMessage, type FormInstance, type FormRules } from "element-plus";
import VMdPreview from "@kangc/v-md-editor/lib/preview";

const route = useRoute();
const router = useRouter();

// 是否为编辑模式（根据路由参数判断）
const isEdit = ref<boolean>(!!route.params.id);

//表单数据
const form = reactive({
  title: "",
  content: "",
  summary: "",
});

// el-form 的 ref 实例，用于调用 validate 方法
const formRef = ref<FormInstance>();

// 表单验证规则{
const rules: FormRules = {
  title: [
    { required: true, message: "请输入标题", trigger: ["blur"] },
    {
      min: 3,
      max: 20,
      message: "标题长度必须在3到20个字符之间",
      trigger: ["blur"],
    },
  ],
  content: [
    { required: true, message: "请输入内容", trigger: ["blur"] },
    {
      min: 10,
      message: "内容长度必须在10个字符以上",
      trigger: ["blur"],
    },
  ],
};

// 是否正在预览 Markdown
const previewMode = ref<boolean>(false);

// 模拟：如果是编辑模式，加载已有文章
onMounted(() => {
  if (isEdit.value) {
    // 模拟加载已有文章
    form.title = "编辑中的文章";
    form.content = "##这是编辑文章的内容";
    form.summary = "这是文章的摘要";
  }
});

// 提交表单
const handleSubmit = async () => {
  if (!formRef.value) return;
  await formRef.value.validate((valid) => {
    if (valid) {
      if (isEdit.value) {
        ElMessage.success("文章更新成功");
      } else {
        // 模拟创建文章
        ElMessage.success("文章发布成功");
      }
      router.push("/");
    } else {
      ElMessage.error("请检查表单填写");
    }
  });
};
</script>

<template>
  <div class="edit-page">
    <h2>{{ isEdit ? "编辑文章" : "发布文章" }}</h2>
    <el-form ref="formRef" :model="form" :rules="rules" label-width="80px">
      <el-form-item label="标题" prop="title">
        <el-input v-model="form.title" placeholder="请输入标题" />
      </el-form-item>
      <el-form-item label="摘要" prop="summary">
        <el-input
          v-model="form.summary"
          type="textarea"
          :rows="2"
          placeholder="请输入摘要（可选）"
        />
      </el-form-item>
      <el-form-item label="内容" prop="content">
        <div class="editor-container">
          <div style="margin-bottom: 10px">
            <el-button size="small" @click="previewMode = !previewMode">
              {{ previewMode ? "编辑" : "预览" }}
            </el-button>
          </div>
          <el-input
            v-if="!previewMode"
            v-model="form.content"
            type="textarea"
            :rows="15"
            placeholder="支持Markdown格式"
          />
          <div v-else class="markdown-preview">
            <v-md-preview :text="form.content" />
          </div>
        </div>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="handleSubmit">
          {{ isEdit ? "更新" : "发布" }}
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
.markdown-preview {
  border: 1px solid #ddd;
  padding: 15px;
  border-radius: 4px;
  min-height: 300px;
  background: white;
}
</style>
