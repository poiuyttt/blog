<script setup lang="ts">
import { ref, onMounted } from "vue";
import { ElMessage, ElMessageBox, type UploadRawFile } from "element-plus";
import { UploadFilled, Delete } from "@element-plus/icons-vue";
import { useAuthStore } from "../stores/auth";
import { hasRole } from "../utils/auth";
import request from "../utils/request";

const authStore = useAuthStore();
const uploadUrl = "https://localhost:7126/api/file/upload";
const uploadedFiles = ref<
  { name: string; url: string; relativePath: string }[]
>([]);

async function loadFiles() {
  try {
    const response: any = await request.get("/file/list");
    if (response.success && response.data) {
      uploadedFiles.value = response.data;
    }
  } catch (error) {
    console.error("加载文件列表失败:", error);
  }
}

onMounted(() => {
  loadFiles();
});

function beforeUpload(file: UploadRawFile): boolean {
  const isImage = file.type.startsWith("image/");
  if (!isImage) {
    ElMessage.error("只能上传图片文件！");
    return false;
  }
  const isLt5M = file.size / 1024 / 1024 < 5;
  if (!isLt5M) {
    ElMessage.error("图片大小不能超过 5MB！");
    return false;
  }
  return true;
}

function handleUploadSuccess(response: any): void {
  if (response.success) {
    ElMessage.success("上传成功！");
    loadFiles();
  } else {
    ElMessage.error(response.message || "上传失败");
  }
}

function handleUploadError(): void {
  ElMessage.error("上传失败，请稍后重试");
}

function copyUrl(url: string): void {
  navigator.clipboard.writeText(url).then(() => {
    ElMessage.success("链接已复制到剪贴板");
  });
}

async function handleDeleteFile(
  index: number,
  relativePath: string,
): Promise<void> {
  try {
    await ElMessageBox.confirm("确定要删除这张图片吗？", "提示", {
      confirmButtonText: "确定",
      cancelButtonText: "取消",
      type: "warning",
    });

    await request.delete("/file/delete", {
      params: { path: relativePath },
    });

    ElMessage.success("删除成功");
    loadFiles();
  } catch (error: any) {
    if (error !== "cancel") {
      ElMessage.error(error.response?.data?.message || "删除失败");
    }
  }
}
</script>

<template>
  <div class="image-gallery">
    <div class="container">
      <div class="page-header">
        <h1>📷 图库</h1>
        <el-upload
          v-if="hasRole('Admin')"
          class="upload-compact"
          drag
          :action="uploadUrl"
          :headers="{ Authorization: `Bearer ${authStore.token}` }"
          :before-upload="beforeUpload"
          :on-success="handleUploadSuccess"
          :on-error="handleUploadError"
          :limit="10"
        >
          <el-icon class="el-icon--upload"><UploadFilled /></el-icon>
          <div class="el-upload__text">上传图片</div>
        </el-upload>
      </div>

      <div v-if="uploadedFiles.length > 0" class="image-list">
        <div
          v-for="(file, index) in uploadedFiles"
          :key="index"
          class="image-item"
        >
          <el-image :src="file.url" fit="cover" class="preview-image" />
          <div class="image-actions">
            <el-button size="small" @click="copyUrl(file.url)"
              >复制链接</el-button
            >
            <el-button
              v-if="hasRole('Admin')"
              type="danger"
              size="small"
              :icon="Delete"
              @click="handleDeleteFile(index, file.relativePath)"
              >删除</el-button
            >
          </div>
        </div>
      </div>

      <el-empty v-else description="暂无图片" />
    </div>
  </div>
</template>

<style scoped>
.image-gallery {
  padding: 20px 0;
}
.container {
  padding: 0 40px;
}
.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
}
.page-header h1 {
  margin: 0;
}
.upload-compact {
  width: 150px;
}
.upload-compact :deep(.el-upload-dragger) {
  padding: 20px 10px;
}
.image-list {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  gap: 20px;
}
.image-item {
  border: 1px solid #e4e7ed;
  border-radius: 8px;
  overflow: hidden;
  transition:
    transform 0.2s,
    box-shadow 0.2s;
}
.image-item:hover {
  transform: translateY(-4px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}
.preview-image {
  width: 100%;
  height: 250px;
  background-color: #f5f7fa;
}
.image-actions {
  padding: 12px;
  display: flex;
  justify-content: center;
  gap: 10px;
}
</style>
