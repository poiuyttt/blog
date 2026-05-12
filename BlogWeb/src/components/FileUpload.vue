<script setup lang="ts">
import { ref } from "vue";
import { ElMessage, type UploadProps } from "element-plus";
import { Plus } from "@element-plus/icons-vue";
import request from "../utils/request";

// 上传成功后返回的 URL 列表
const uploadedUrls = ref<string[]>([]);

// 自定义上传请求
// 作用：接管 el-upload 的默认上传行为，使用 FormData 发送到后端 API
const custUpload = async (option: any) => {
  // FormData：浏览器原生对象，用于构造 multipart/form-data 格式的请求体
  const formData = new FormData();
  // append：向 FormData 中添加一个字段
  // 'file' 必须与后端 [FromForm] IFormFile file 的参数名匹配
  formData.append("file", option.file);

  try {
    // axios.post：发送 POST 请求到文件上传 API
    const result = await request.post("/file/upload", formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });

    uploadedUrls.value.push(result.url);
    ElMessage.success("上传成功");
    option.onSuccess(result, option.file);
  } catch (error: any) {
    console.error("上传失败:", error);
    ElMessage.error(error.response?.data?.message || "文件上传失败");
    // onError：通知 el-upload 上传失败
    option.onError(error);
  }
};

//移除文件
const handleRemove: UploadProps["onRemove"] = (uploadFile) => {
  const responseData = uploadFile.response as { url?: string } | null;
  const url = uploadFile.url || responseData?.url || "";
  if (url) {
    const index = uploadedUrls.value.indexOf(url);
    if (index > -1) {
      uploadedUrls.value.splice(index, 1);
    }
  }
};

//图片上传成功回调
const handleSuccess: UploadProps["onSuccess"] = (response, uploadFile) => {
  console.log("上传成功", uploadFile.name);
};
</script>
<template>
  <div class="file-upload">
    <!-- el-upload：Element Plus 文件上传组件 -->
    <!-- action="#"：占位符，因为实际上传由 http-request 接管 -->
    <!-- auto-upload="true"：选择文件后自动上传 -->
    <!-- list-type="picture-card"：图片卡片模式展示 -->
    <!-- accept="image/*"：只允许选择图片文件 -->
    <el-upload
      action="#"
      :auto-upload="true"
      :http-request="custUpload"
      :on-remove="handleRemove"
      :on-success="handleSuccess"
      list-type="picture-card"
      accept="image/*"
    >
      <el-icon><Plus /></el-icon>
    </el-upload>
    <p class="tip">点击 + 上传图片（支持 JPG/PNG/GIF，最大 5MB）</p>
    <!-- 上传成功的图片预览 -->
    <div v-if="uploadedUrls.length > 0" class="uploaded-images">
      <p>已上传图片：</p>
      <img
        v-for="url in uploadedUrls"
        :key="url"
        :src="url"
        class="preview-img"
      />
    </div>
  </div>
</template>

<style scoped>
.file-upload {
  margin-top: 10px;
}
.tip {
  font-size: 12px;
  color: #999;
  margin-top: 8px;
}
.uploaded-images {
  margin-top: 15px;
}
.preview-img {
  width: 100px;
  height: 100px;
  object-fit: cover;
  border-radius: 4px;
  margin-right: 10px;
  border: 1px solid #ddd;
}
</style>
