<script setup lang="ts">
import { ref } from "vue";
import { ElMessage, type UploadProps } from "element-plus";

// 上传成功后返回的 URL 列表
const uploadedUrls = ref<string[]>([]);

// 自定义上传请求（模拟）
const custUpload = (option: any) => {
  // 真实场景：使用 FormData 上传到后端 API
  // const formData = new FormData();
  // formData.append('file', option.file);
  // axios.post('/api/upload', formData) ...

  // 模拟：1.5 秒后返回假 URL
  setTimeout(() => {
    const fakeUrl = "https://picsum.photos/400/200?random" + Date.now();
    uploadedUrls.value.push(fakeUrl);
    ElMessage.success("文件上传成功");
    option.onSuccess();
  }, 1500);
};

//移除文件
const handleRemove: UploadProps["onRemove"] = (uploadFile) => {
  const url = uploadFile.url;
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
    <!-- action：上传地址（必填，否则无法触发上传流程）此处用占位符 -->
    <!-- auto-upload：是否选择文件后自动上传 -->
    <!-- list-type：文件列表类型，picture-card 为图片卡片模式 -->
    <el-upload
      action="#"
      :auto-upload="false"
      :on-remove="handleRemove"
      :on-success="handleSuccess"
      :http-request="custUpload"
      list-type="picture-card"
    >
      <el-icon><Plus /></el-icon>
    </el-upload>
  </div>
</template>

<style scoped>
.file-upload {
  margin-top: 10px;
}
.tip {
  font-size: 12px;
  color: #999;
}
</style>
